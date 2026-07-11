// Script dùng chung để export 1 sơ đồ ERD tĩnh (kiểu erd_simple.html) ra PNG độ phân giải cao.
// Cách chạy (bắt buộc từ thư mục frontend/ vì playwright chỉ có trong frontend/node_modules):
//   node scripts/export-erd.mjs --in ../docs/erd/04_lop_sinh_hoat_bien_ban.html --out ../docs/exports/so-do-lop-sinh-hoat-va-bien-ban-sinh-hoat.png
import { chromium } from 'playwright';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

function parseArgs(argv) {
  const args = {};
  for (let i = 0; i < argv.length; i++) {
    if (argv[i] === '--in') args.in = argv[++i];
    if (argv[i] === '--out') args.out = argv[++i];
  }
  if (!args.in || !args.out) {
    console.error('Usage: node export-erd.mjs --in <path-to-html> --out <path-to-png>');
    process.exit(1);
  }
  return args;
}

const { in: inArg, out: outArg } = parseArgs(process.argv.slice(2));
const htmlPath = 'file://' + path.resolve(process.cwd(), inArg);
const outPath = path.resolve(process.cwd(), outArg);
const MIN_WIDTH = 2400;
const MARGIN = 30;

const browser = await chromium.launch();

// ---- Pass 1: đo kích thước nội dung thật (viewport dư lớn để không bị cắt) ----
const measurePage = await browser.newPage({ viewport: { width: 1900, height: 1100 } });
await measurePage.goto(htmlPath, { waitUntil: 'load' });
await measurePage.waitForSelector('.table-card');
await measurePage.waitForTimeout(200);

const expected = await measurePage.evaluate(() => ({
  tableCount: Object.keys(TABLES).length,
  relationCount: RELATIONS.length,
}));
const rendered = await measurePage.evaluate(() => ({
  cardCount: document.querySelectorAll('.table-card').length,
  lineCount: document.querySelectorAll('#svg-lines path').length,
}));
console.log(path.basename(inArg), '— Expected:', expected, '| Rendered:', rendered);
if (rendered.cardCount !== expected.tableCount || rendered.lineCount !== expected.relationCount) {
  console.log('  WARNING: rendered card/line counts do not match expected data.');
}

const bbox = await measurePage.evaluate(() => {
  const els = [...document.querySelectorAll('.table-card'), ...document.querySelectorAll('#note')];
  const rects = els.map((c) => c.getBoundingClientRect());
  return {
    maxX: Math.max(...rects.map((r) => r.right)),
    maxY: Math.max(...rects.map((r) => r.bottom)),
  };
});
await measurePage.close();

const contentWidth = Math.ceil(bbox.maxX) + MARGIN;
const contentHeight = Math.ceil(bbox.maxY) + MARGIN;
const deviceScaleFactor = Math.max(2, Math.ceil(MIN_WIDTH / contentWidth));

// ---- Pass 2: export độ phân giải cao, viewport khớp đúng nội dung (không cắt xén) ----
const context = await browser.newContext({
  viewport: { width: contentWidth, height: contentHeight },
  deviceScaleFactor,
});
const page = await context.newPage();
await page.goto(htmlPath, { waitUntil: 'load' });
await page.waitForSelector('.table-card');
await page.waitForTimeout(200);

await page.screenshot({ path: outPath });
console.log(
  `  Saved: ${path.relative(path.resolve(__dirname, '../..'), outPath)}` +
  ` (${contentWidth * deviceScaleFactor}x${contentHeight * deviceScaleFactor}px)`,
);

await browser.close();
