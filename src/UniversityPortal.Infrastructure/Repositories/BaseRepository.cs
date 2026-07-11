using Microsoft.EntityFrameworkCore;
using UniversityPortal.Application.DTOs.Common;
using UniversityPortal.Application.Interfaces.Repositories;
using UniversityPortal.Domain.Entities.Common;
using UniversityPortal.Infrastructure.Persistence;

namespace UniversityPortal.Infrastructure.Repositories;

/// <summary>
/// Repository generic cung cấp CRUD cơ bản cho mọi entity kế thừa AuditableEntity.
/// Repository cụ thể (VD: SinhVienRepository) kế thừa lớp này và bổ sung truy vấn đặc thù.
/// </summary>
public class BaseRepository<T>(AppDbContext context) : IRepository<T> where T : AuditableEntity
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    /// <summary>Tìm một bản ghi theo khóa chính. Trả về null nếu không tìm thấy.</summary>
    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    /// <summary>Lấy toàn bộ bản ghi trong bảng (không phân trang — dùng khi dữ liệu ít).</summary>
    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();

    /// <summary>
    /// Lấy danh sách có phân trang theo số trang và kích thước trang.
    /// Công thức bỏ qua bản ghi: (page - 1) × pageSize.
    /// </summary>
    public async Task<PagedResultDto<T>> GetPagedAsync(int page, int pageSize)
    {
        var tongSoBanGhi = await DbSet.CountAsync();
        var danhSach     = await DbSet.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResultDto<T> { Data = danhSach, Total = tongSoBanGhi, Page = page, PageSize = pageSize };
    }

    /// <summary>Thêm entity vào ChangeTracker (chưa lưu DB; cần gọi CommitAsync sau).</summary>
    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

    /// <summary>Đánh dấu entity là Modified trong ChangeTracker (chưa lưu DB).</summary>
    public void Update(T entity) => DbSet.Update(entity);

    /// <summary>Đánh dấu entity là Deleted trong ChangeTracker — xoá vật lý khi CommitAsync.</summary>
    public void Delete(T entity) => DbSet.Remove(entity);
}
