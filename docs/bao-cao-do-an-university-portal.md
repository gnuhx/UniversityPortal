# BÁO CÁO ĐỒ ÁN

## Đề tài: Xây dựng hệ thống Cổng thông tin Đại học - University Portal

> Sinh viên thực hiện: ................................................  
> Mã sinh viên: .......................................................  
> Lớp: .................................................................  
> Giảng viên hướng dẫn: ................................................  
> Khoa/Bộ môn: .........................................................  
> Trường: ..............................................................  
> Thời gian thực hiện: .................................................

---

# Mục Lục

- [Danh Mục Các Từ Viết Tắt](#danh-mục-các-từ-viết-tắt)
- [Danh Mục Bảng Biểu](#danh-mục-bảng-biểu)
- [Danh Mục Các Hình Vẽ](#danh-mục-các-hình-vẽ)
- [Lời Cảm Ơn](#lời-cảm-ơn)
- [Lời Nói Đầu](#lời-nói-đầu)
- [Chương 1. Tổng Quan Đề Tài](#chương-1-tổng-quan-đề-tài)
- [Chương 2. Cơ Sở Lý Thuyết Và Công Nghệ Sử Dụng](#chương-2-cơ-sở-lý-thuyết-và-công-nghệ-sử-dụng)
- [Chương 3. Phân Tích Và Thiết Kế Hệ Thống](#chương-3-phân-tích-và-thiết-kế-hệ-thống)
- [Chương 4. Triển Khai Hệ Thống](#chương-4-triển-khai-hệ-thống)
- [Chương 5. Kiểm Thử Hệ Thống](#chương-5-kiểm-thử-hệ-thống)
- [Chương 6. Kết Quả Đạt Được Và Demo](#chương-6-kết-quả-đạt-được-và-demo)
- [Chương 7. Kết Luận Và Hướng Phát Triển](#chương-7-kết-luận-và-hướng-phát-triển)
- [Tài Liệu Tham Khảo](#tài-liệu-tham-khảo)
- [Phụ Lục](#phụ-lục)

---

# Danh Mục Các Từ Viết Tắt

| STT | Từ viết tắt | Ý nghĩa |
|---:|---|---|
| 1 | API | Application Programming Interface - giao diện lập trình ứng dụng |
| 2 | CRUD | Create, Read, Update, Delete - thêm, đọc, sửa, xóa dữ liệu |
| 3 | CSDL | Cơ sở dữ liệu |
| 4 | DTO | Data Transfer Object - đối tượng truyền dữ liệu |
| 5 | EF Core | Entity Framework Core |
| 6 | ERD | Entity Relationship Diagram - sơ đồ thực thể liên kết |
| 7 | HTTP | HyperText Transfer Protocol |
| 8 | JWT | JSON Web Token |
| 9 | REST | Representational State Transfer |
| 10 | SPA | Single Page Application |
| 11 | SQL | Structured Query Language |
| 12 | UI | User Interface - giao diện người dùng |

---

# Danh Mục Bảng Biểu

| Số hiệu | Tên bảng |
|---|---|
| Bảng 1.1 | Phạm vi chức năng của hệ thống |
| Bảng 2.1 | Công nghệ sử dụng trong đồ án |
| Bảng 3.1 | Tác nhân và quyền sử dụng hệ thống |
| Bảng 3.2 | Danh sách module chức năng theo controller |
| Bảng 3.3 | Các bảng dữ liệu chính của hệ thống |
| Bảng 4.1 | Cấu trúc solution backend |
| Bảng 4.2 | Cấu trúc frontend |
| Bảng 5.1 | Kịch bản kiểm thử API bằng Postman |
| Bảng 6.1 | Tài khoản demo |

---

# Danh Mục Các Hình Vẽ

| Số hiệu | Tên hình |
|---|---|
| Hình 3.1 | Sơ đồ use case tổng quát |
| Hình 3.2 | Sơ đồ kiến trúc tổng thể hệ thống |
| Hình 3.3 | Sơ đồ luồng xử lý request |
| Hình 3.4 | Sơ đồ ERD cơ sở dữ liệu |
| Hình 4.1 | Giao diện đăng nhập |
| Hình 4.2 | Giao diện dashboard |
| Hình 4.3 | Giao diện quản lý sinh viên |
| Hình 4.4 | Giao diện quản lý chương trình đào tạo |
| Hình 5.1 | Kết quả kiểm thử API bằng Postman |
| Hình 6.1 | Giao diện Swagger API |

---

# Lời Cảm Ơn

Trước hết, em xin gửi lời cảm ơn chân thành đến quý thầy cô trong khoa đã tận tình giảng dạy, truyền đạt kiến thức nền tảng và định hướng chuyên môn trong suốt quá trình học tập. Những kiến thức về lập trình, cơ sở dữ liệu, phân tích thiết kế hệ thống, công nghệ web và quy trình phát triển phần mềm là cơ sở quan trọng để em có thể thực hiện đồ án này.

Em xin gửi lời cảm ơn sâu sắc đến giảng viên hướng dẫn đã dành thời gian theo dõi, góp ý và hỗ trợ em trong quá trình xây dựng đề tài "Xây dựng hệ thống Cổng thông tin Đại học - University Portal". Những nhận xét và định hướng của thầy/cô đã giúp em nhìn nhận rõ hơn về yêu cầu nghiệp vụ, cách tổ chức kiến trúc hệ thống, phương pháp triển khai chức năng và cách trình bày báo cáo đồ án một cách khoa học.

Em cũng xin cảm ơn bạn bè, gia đình và những người đã hỗ trợ, động viên em trong quá trình thực hiện đồ án. Sự khích lệ và góp ý từ mọi người giúp em có thêm động lực để hoàn thiện sản phẩm, kiểm tra lại các chức năng và cải thiện chất lượng của hệ thống.

Do thời gian thực hiện có hạn và kinh nghiệm thực tế còn đang được tích lũy, đồ án khó tránh khỏi những thiếu sót trong quá trình phân tích, thiết kế và triển khai. Em rất mong nhận được sự góp ý từ quý thầy cô để có thể hoàn thiện hơn trong các sản phẩm tiếp theo.

Em xin chân thành cảm ơn!

---

# Lời Nói Đầu

Trong bối cảnh chuyển đổi số đang được triển khai mạnh mẽ trong lĩnh vực giáo dục, việc xây dựng các hệ thống quản lý thông tin tập trung đóng vai trò quan trọng đối với nhà trường, giảng viên và sinh viên. Các nghiệp vụ như quản lý tài khoản, quản lý hồ sơ sinh viên, quản lý giảng viên, chương trình đào tạo, môn học, lớp học phần, điểm số, học phí, thông báo và yêu cầu hành chính nếu được xử lý trên một nền tảng thống nhất sẽ giúp giảm thời gian thao tác thủ công, hạn chế sai sót và nâng cao hiệu quả quản lý.

Xuất phát từ nhu cầu đó, đồ án lựa chọn đề tài "Xây dựng hệ thống Cổng thông tin Đại học - University Portal". Hệ thống hướng đến việc cung cấp một nền tảng web cho nhiều nhóm người dùng khác nhau gồm quản trị viên, giáo vụ, giảng viên và sinh viên. Mỗi nhóm người dùng có phạm vi chức năng riêng, được kiểm soát thông qua cơ chế xác thực và phân quyền. Người dùng có thể đăng nhập, truy cập các chức năng phù hợp với vai trò, thực hiện thao tác quản lý dữ liệu và theo dõi thông tin học tập, tài chính, thông báo hoặc yêu cầu liên quan.

Về mặt kỹ thuật, hệ thống được xây dựng theo mô hình tách biệt backend và frontend. Backend sử dụng ASP.NET Core Web API, Entity Framework Core, SQL Server, JWT Authentication và tổ chức theo định hướng Clean Architecture với các lớp Domain, Application, Infrastructure và API. Frontend sử dụng React, Vite, TypeScript, Ant Design, React Router, React Query và Zustand để xây dựng giao diện người dùng dạng SPA. Ngoài ra, hệ thống còn hỗ trợ triển khai bằng Docker, cung cấp Swagger cho tài liệu API và Postman collection phục vụ kiểm thử thủ công.

Báo cáo này trình bày quá trình thực hiện đồ án từ tổng quan đề tài, cơ sở lý thuyết, phân tích yêu cầu, thiết kế hệ thống, triển khai chức năng, kiểm thử, kết quả đạt được cho đến kết luận và hướng phát triển. Nội dung báo cáo được xây dựng dựa trên mã nguồn hiện có của dự án University Portal, đồng thời phản ánh các chức năng đã được triển khai trong hệ thống.

---

# Nội Dung Của Quyển Báo Cáo

## Chương 1. Tổng Quan Đề Tài

### 1.1. Lý do chọn đề tài

Trong môi trường đại học, số lượng thông tin cần quản lý ngày càng lớn và có liên quan đến nhiều nhóm người dùng khác nhau. Nhà trường cần quản lý hồ sơ sinh viên, giảng viên, lớp sinh hoạt, ngành học, chương trình đào tạo, môn học, lớp học phần, điểm số, học phí, thông báo và các yêu cầu hành chính. Nếu các nghiệp vụ này được xử lý rời rạc bằng nhiều công cụ khác nhau, quá trình vận hành sẽ dễ phát sinh trùng lặp dữ liệu, sai sót khi cập nhật và khó tra cứu thông tin.

Vì vậy, việc xây dựng một hệ thống cổng thông tin đại học có khả năng tập trung hóa dữ liệu, phân quyền người dùng và hỗ trợ các nghiệp vụ quản lý cơ bản là cần thiết. Đề tài University Portal được thực hiện nhằm mô phỏng một hệ thống quản lý thông tin dành cho trường đại học, giúp người dùng thao tác trên nền tảng web với giao diện trực quan và API có cấu trúc rõ ràng.

### 1.2. Mục tiêu đề tài

Mục tiêu chính của đồ án là xây dựng hệ thống University Portal với các yêu cầu sau:

- Xây dựng backend API phục vụ các nghiệp vụ quản lý đại học.
- Xây dựng frontend web cho các nhóm người dùng tương tác với hệ thống.
- Hỗ trợ đăng nhập, xác thực bằng JWT và phân quyền theo vai trò.
- Quản lý các danh mục và dữ liệu chính như sinh viên, giảng viên, tài khoản, ngành học, môn học, chương trình đào tạo, lớp sinh hoạt, lớp học phần, điểm, học phí và thông báo.
- Cung cấp chức năng xử lý yêu cầu hành chính và yêu cầu sửa điểm.
- Cung cấp tài liệu API bằng Swagger và bộ kiểm thử thủ công bằng Postman.
- Hỗ trợ chạy hệ thống bằng Docker Compose để thuận tiện triển khai.

### 1.3. Đối tượng sử dụng

Hệ thống phục vụ bốn nhóm người dùng chính:

- Admin: quản trị hệ thống, quản lý tài khoản và dữ liệu toàn cục.
- Giáo vụ: quản lý thông tin đào tạo, sinh viên, giảng viên, lớp, môn học và các nghiệp vụ học vụ.
- Giáo viên: theo dõi lớp học phần, nhập điểm và xử lý các yêu cầu liên quan.
- Sinh viên: xem hồ sơ, bảng điểm, học phí, thông báo và gửi yêu cầu hành chính.

### 1.4. Phạm vi đề tài

**Bảng 1.1. Phạm vi chức năng của hệ thống**

| Nhóm chức năng | Nội dung |
|---|---|
| Xác thực | Đăng nhập, cấp access token, refresh token, phân quyền theo vai trò |
| Quản lý người dùng | Quản lý tài khoản, thông tin sinh viên, thông tin giảng viên |
| Quản lý đào tạo | Quản lý ngành học, môn học, chương trình đào tạo, chi tiết chương trình đào tạo |
| Quản lý lớp | Quản lý lớp sinh hoạt, lớp học phần, danh sách lớp học phần |
| Quản lý học tập | Xem bảng điểm, nhập điểm, gửi và duyệt yêu cầu sửa điểm |
| Quản lý tài chính | Quản lý học phí và trạng thái đóng tiền |
| Truyền thông | Tạo, xem và quản lý thông báo |
| Hành chính | Gửi và duyệt yêu cầu hành chính |
| Triển khai | Chạy bằng Docker, cấu hình SQL Server, phục vụ file upload |

### 1.5. Phương pháp thực hiện

Đề tài được thực hiện theo hướng phân tích nghiệp vụ, thiết kế cơ sở dữ liệu, xây dựng API, xây dựng giao diện và kiểm thử. Quá trình triển khai ưu tiên kiến trúc phân lớp để dễ mở rộng và dễ bảo trì. Các chức năng được kiểm thử thông qua Swagger, Postman collection và kiểm tra trực tiếp trên giao diện frontend.

---

## Chương 2. Cơ Sở Lý Thuyết Và Công Nghệ Sử Dụng

### 2.1. Kiến trúc Clean Architecture

Clean Architecture là cách tổ chức hệ thống thành nhiều lớp có trách nhiệm tách biệt. Trong đồ án, backend được chia thành các project chính:

- Domain: chứa entity, enum và exception nghiệp vụ.
- Application: chứa DTO, service, interface, validator và mapping.
- Infrastructure: chứa DbContext, repository, Unit of Work, cấu hình Entity Framework Core và các service hạ tầng.
- API: chứa controller, middleware, cấu hình Swagger, xác thực, phân quyền và endpoint HTTP.

Cách tổ chức này giúp nghiệp vụ cốt lõi không phụ thuộc trực tiếp vào framework, giảm sự phụ thuộc giữa các thành phần và thuận tiện khi bảo trì hoặc mở rộng hệ thống.

### 2.2. ASP.NET Core Web API

ASP.NET Core Web API được sử dụng để xây dựng backend theo phong cách RESTful API. Các controller tiếp nhận request từ frontend hoặc công cụ kiểm thử, gọi service ở tầng Application và trả về response theo định dạng JSON. Hệ thống cũng sử dụng Swagger để mô tả API, giúp lập trình viên dễ kiểm tra endpoint và thử nghiệm request.

### 2.3. Entity Framework Core và SQL Server

Entity Framework Core là ORM giúp ánh xạ giữa entity trong C# và bảng dữ liệu trong SQL Server. Trong đồ án, `AppDbContext` khai báo các `DbSet` tương ứng với các bảng như `TaiKhoan`, `SinhVien`, `GiaoVien`, `NganhHoc`, `MonHoc`, `LopHocPhan`, `HocPhi`, `ThongBao`, `YeuCauHanhChinh` và `YeuCauSuaDiem`. Hệ thống sử dụng migration để đồng bộ cấu trúc cơ sở dữ liệu khi chạy ứng dụng.

### 2.4. Repository Pattern và Unit of Work

Repository Pattern giúp tách logic truy cập dữ liệu khỏi logic nghiệp vụ. Unit of Work đóng vai trò quản lý nhiều repository và gom các thao tác thay đổi dữ liệu vào một đơn vị xử lý. Cách tiếp cận này giúp service không thao tác trực tiếp với `DbContext`, từ đó mã nguồn dễ đọc và dễ kiểm thử hơn.

### 2.5. JWT Authentication và phân quyền

Hệ thống sử dụng JWT để xác thực người dùng. Sau khi đăng nhập thành công, backend trả về access token và refresh token. Frontend lưu token và gửi kèm trong header `Authorization: Bearer <token>` khi gọi các API cần xác thực. Phân quyền được thực hiện dựa trên vai trò người dùng như Admin, Giáo vụ, Giáo viên và Sinh viên.

### 2.6. React, Vite và TypeScript

Frontend được xây dựng bằng React kết hợp Vite và TypeScript. React Router hỗ trợ định tuyến giữa các trang, React Query hỗ trợ quản lý trạng thái gọi API, Zustand hỗ trợ lưu trạng thái xác thực, Ant Design cung cấp bộ component giao diện. Cách tổ chức này giúp frontend phát triển nhanh, có cấu trúc và dễ mở rộng.

### 2.7. Docker và Docker Compose

Docker được sử dụng để đóng gói backend và frontend, giúp hệ thống chạy ổn định trên nhiều môi trường. Docker Compose giúp khởi động các service với cấu hình thống nhất. Theo README của dự án, frontend có thể chạy tại `http://localhost:3100`, API tại `http://localhost:8080` và Swagger tại `http://localhost:8080/swagger`.

**Bảng 2.1. Công nghệ sử dụng trong đồ án**

| Thành phần | Công nghệ |
|---|---|
| Backend | ASP.NET Core Web API, C# |
| ORM | Entity Framework Core |
| Cơ sở dữ liệu | SQL Server |
| Frontend | React, Vite, TypeScript |
| UI | Ant Design |
| State/API client | Zustand, React Query, Axios |
| Xác thực | JWT Bearer Authentication |
| Tài liệu API | Swagger/OpenAPI |
| Kiểm thử thủ công | Postman |
| Triển khai | Docker, Docker Compose, Nginx cho frontend |

---

## Chương 3. Phân Tích Và Thiết Kế Hệ Thống

### 3.1. Phân tích tác nhân

**Bảng 3.1. Tác nhân và quyền sử dụng hệ thống**

| Tác nhân | Mô tả | Chức năng tiêu biểu |
|---|---|---|
| Admin | Người quản trị hệ thống | Quản lý tài khoản, sinh viên, giảng viên, dữ liệu đào tạo, thông báo, học phí |
| Giáo vụ | Người phụ trách nghiệp vụ đào tạo | Quản lý sinh viên, giảng viên, lớp, ngành, chương trình đào tạo, học phí, yêu cầu hành chính |
| Giáo viên | Người giảng dạy | Xem lớp học phần, nhập điểm, xử lý yêu cầu sửa điểm |
| Sinh viên | Người học | Xem hồ sơ, bảng điểm, học phí, thông báo, gửi yêu cầu hành chính |

### 3.2. Yêu cầu chức năng

Hệ thống cần đáp ứng các yêu cầu chức năng sau:

- Người dùng đăng nhập bằng tên đăng nhập và mật khẩu.
- Hệ thống xác định vai trò người dùng và điều hướng đến các chức năng phù hợp.
- Admin và giáo vụ có thể quản lý sinh viên, giảng viên, lớp sinh hoạt, tài khoản và dữ liệu đào tạo.
- Giáo viên có thể xem lớp học phần và nhập điểm cho sinh viên.
- Sinh viên có thể xem hồ sơ cá nhân, bảng điểm, học phí và thông báo.
- Sinh viên có thể gửi yêu cầu hành chính; giáo vụ hoặc admin có thể duyệt yêu cầu.
- Sinh viên có thể gửi yêu cầu sửa điểm; giáo viên hoặc admin có thể xử lý theo quyền.
- Hệ thống cung cấp API có tài liệu Swagger và bộ request Postman phục vụ kiểm thử.

### 3.3. Yêu cầu phi chức năng

- Bảo mật: API cần xác thực bằng JWT và phân quyền theo vai trò.
- Dễ bảo trì: mã nguồn được tổ chức theo Clean Architecture.
- Dễ mở rộng: các module nghiệp vụ được tách thành controller, service, repository và DTO riêng.
- Khả dụng: hệ thống có thể chạy bằng Docker Compose và cấu hình qua biến môi trường.
- Dễ kiểm thử: API có Swagger và Postman collection.
- Giao diện thân thiện: frontend cung cấp các trang quản lý và thao tác theo vai trò.

### 3.4. Use case tổng quát

> Chèn hình tại đây: Hình 3.1. Sơ đồ use case tổng quát.

Mô tả use case chính:

- Đăng nhập hệ thống.
- Quản lý tài khoản.
- Quản lý sinh viên.
- Quản lý giảng viên.
- Quản lý ngành học.
- Quản lý môn học.
- Quản lý chương trình đào tạo.
- Quản lý lớp sinh hoạt.
- Quản lý lớp học phần.
- Nhập và xem điểm.
- Quản lý học phí.
- Quản lý thông báo.
- Gửi và duyệt yêu cầu hành chính.
- Gửi và xử lý yêu cầu sửa điểm.

### 3.5. Danh sách module chức năng

**Bảng 3.2. Danh sách module chức năng theo controller**

| Controller | Chức năng chính | Vai trò sử dụng |
|---|---|---|
| `AuthController` | Đăng nhập, làm mới token, đăng xuất | Tất cả |
| `TaiKhoanController` | Quản lý tài khoản, đổi mật khẩu | Admin |
| `SinhVienController` | Quản lý thông tin sinh viên | Admin, Giáo vụ, Sinh viên |
| `GiaoVienController` | Quản lý thông tin giảng viên | Admin, Giáo vụ, Giáo viên |
| `NganhHocController` | Quản lý ngành học | Admin, Giáo vụ |
| `ChuongTrinhDTController` | Quản lý chương trình đào tạo | Admin, Giáo vụ |
| `ChiTietCTDTController` | Quản lý chi tiết chương trình đào tạo | Admin, Giáo vụ |
| `MonHocController` | Quản lý môn học | Admin, Giáo vụ |
| `LopSinhHoatController` | Quản lý lớp sinh hoạt | Admin, Giáo vụ, Giáo viên |
| `LopHocPhanController` | Quản lý lớp học phần | Admin, Giáo vụ, Giáo viên |
| `DanhSachLopHPController` | Quản lý danh sách lớp học phần, nhập điểm | Giáo viên, Admin |
| `HocPhiController` | Quản lý học phí và trạng thái đóng tiền | Admin, Giáo vụ, Sinh viên |
| `ThongBaoController` | Quản lý thông báo | Admin, Giáo vụ, Sinh viên, Giáo viên |
| `YeuCauHanhChinhController` | Gửi và duyệt yêu cầu hành chính | Sinh viên, Giáo vụ, Admin |
| `YeuCauSuaDiemController` | Gửi và xử lý yêu cầu sửa điểm | Sinh viên, Giáo viên, Admin |

### 3.6. Thiết kế cơ sở dữ liệu

Cơ sở dữ liệu được thiết kế quanh các nhóm nghiệp vụ: người dùng, đào tạo, lớp học, học tập, tài chính, thông báo và yêu cầu. Các entity được định nghĩa trong project `UniversityPortal.Domain`, cấu hình ánh xạ trong project `UniversityPortal.Infrastructure`.

**Bảng 3.3. Các bảng dữ liệu chính của hệ thống**

| Nhóm dữ liệu | Bảng/entity tiêu biểu |
|---|---|
| Người dùng và phân quyền | `TaiKhoan`, `VaiTro`, `GiaoVien`, `SinhVien`, `PhongBan` |
| Đào tạo | `NganhHoc`, `ChuongTrinhDT`, `ChiTietCTDT`, `MonHoc` |
| Lớp học | `LopSinhHoat`, `LopHocPhan`, `DanhSachLopHP` |
| Học tập | `HocBa`, `DiemRenLuyen`, `ThoiKhoaBieu`, `DanhSachThiLai`, `KetQuaAnhVanDauVao` |
| Tài chính | `HocPhi` |
| Thông báo | `ThongBao`, `ThongBaoDaDoc`, `BinhLuanThongBao` |
| Sinh hoạt chủ nhiệm | `BienBanSHCN`, `ChiTietCongViec`, `ChiTietVangSHCN` |
| Yêu cầu | `YeuCauHanhChinh`, `YeuCauSuaDiem` |
| Tiện ích mở rộng | `DatPhongThucHanh`, `KhaoSatYKien`, `DienDanGiaoVien` |

> Chèn hình tại đây: Hình 3.4. Sơ đồ ERD cơ sở dữ liệu. Có thể xuất từ file `docs/erd_schema.html`.

### 3.7. Thiết kế kiến trúc hệ thống

> Chèn hình tại đây: Hình 3.2. Sơ đồ kiến trúc tổng thể hệ thống.

Kiến trúc tổng thể gồm ba phần chính:

- Frontend React: cung cấp giao diện cho người dùng, gọi API thông qua Axios.
- Backend ASP.NET Core API: xử lý request, xác thực, phân quyền, điều phối nghiệp vụ.
- SQL Server: lưu trữ dữ liệu nghiệp vụ của hệ thống.

Luồng xử lý request cơ bản:

1. Người dùng thao tác trên giao diện frontend.
2. Frontend gửi request đến backend API.
3. Middleware kiểm tra lỗi, xác thực JWT và phân quyền.
4. Controller nhận request và gọi service tương ứng.
5. Service xử lý nghiệp vụ, dùng Unit of Work và repository để truy cập dữ liệu.
6. Entity Framework Core thực hiện truy vấn hoặc cập nhật SQL Server.
7. Backend trả kết quả về frontend dưới dạng JSON.

> Chèn hình tại đây: Hình 3.3. Sơ đồ luồng xử lý request.

---

## Chương 4. Triển Khai Hệ Thống

### 4.1. Cấu trúc backend

**Bảng 4.1. Cấu trúc solution backend**

| Project | Vai trò |
|---|---|
| `UniversityPortal.API` | Chứa controller, middleware, cấu hình Swagger, JWT, CORS và endpoint |
| `UniversityPortal.Application` | Chứa DTO, service, interface, validator và AutoMapper profile |
| `UniversityPortal.Domain` | Chứa entity, enum, exception và các thành phần nghiệp vụ cốt lõi |
| `UniversityPortal.Infrastructure` | Chứa DbContext, repository, Unit of Work, migration, service hạ tầng |
| `UniversityPortal.Tests` | Chứa test project phục vụ kiểm thử |

### 4.2. Cấu trúc frontend

**Bảng 4.2. Cấu trúc frontend**

| Thư mục/file | Vai trò |
|---|---|
| `frontend/src/pages` | Chứa các trang như đăng nhập, dashboard, sinh viên, giảng viên, học phí, thông báo |
| `frontend/src/components` | Chứa layout, protected route, bảng CRUD và drawer nhập điểm |
| `frontend/src/api` | Chứa cấu hình Axios client và các hàm gọi API |
| `frontend/src/store` | Chứa trạng thái xác thực người dùng |
| `frontend/src/constants` | Chứa hằng số vai trò |
| `frontend/src/types` | Chứa kiểu dữ liệu TypeScript |

### 4.3. Triển khai chức năng đăng nhập và phân quyền

Chức năng đăng nhập được triển khai thông qua `AuthController` và `AuthService`. Người dùng gửi tên đăng nhập và mật khẩu đến API. Backend kiểm tra thông tin tài khoản, xác định vai trò và sinh access token, refresh token. Token được frontend lưu lại để sử dụng cho các request tiếp theo.

Ở frontend, `ProtectedRoute` kiểm tra trạng thái đăng nhập và vai trò người dùng. Các route như quản lý sinh viên, giảng viên, tài khoản chỉ cho phép Admin hoặc Giáo vụ truy cập; các route như hồ sơ, bảng điểm, học phí dành cho Sinh viên; các route lớp học phần và yêu cầu sửa điểm dành cho Giáo viên hoặc Admin theo từng trường hợp.

### 4.4. Triển khai quản lý sinh viên

Module sinh viên gồm controller, service, repository, DTO và validator. Người dùng có quyền có thể thêm, sửa, xóa, xem danh sách hoặc xem chi tiết sinh viên. Dữ liệu sinh viên liên kết với tài khoản, lớp sinh hoạt, ngành học và các thông tin học tập khác. Frontend cung cấp trang `SinhVienPage` để thao tác dữ liệu thông qua bảng quản lý.

### 4.5. Triển khai quản lý giảng viên

Module giảng viên hỗ trợ quản lý thông tin giảng viên và liên kết với các nghiệp vụ như lớp sinh hoạt, lớp học phần hoặc yêu cầu sửa điểm. Tương tự module sinh viên, hệ thống sử dụng DTO để truyền dữ liệu, validator để kiểm tra dữ liệu đầu vào và service để xử lý nghiệp vụ.

### 4.6. Triển khai quản lý chương trình đào tạo

Chương trình đào tạo gồm các module `NganhHoc`, `ChuongTrinhDT`, `ChiTietCTDT` và `MonHoc`. Giáo vụ hoặc Admin có thể quản lý ngành học, tạo chương trình đào tạo, thêm môn học vào chương trình, xác định số tín chỉ và các thông tin liên quan. Đây là nhóm chức năng quan trọng giúp hệ thống mô phỏng nghiệp vụ đào tạo trong trường đại học.

### 4.7. Triển khai quản lý lớp học phần và điểm

Module lớp học phần cho phép quản lý lớp mở theo từng môn học. Danh sách lớp học phần lưu thông tin sinh viên tham gia và điểm số. Giáo viên có thể nhập điểm thông qua giao diện `NhapDiemDrawer`. Sinh viên có thể xem kết quả học tập trên trang bảng điểm.

### 4.8. Triển khai học phí

Module học phí quản lý các khoản học phí của sinh viên và trạng thái đóng tiền. Admin hoặc Giáo vụ có thể cập nhật trạng thái học phí. Sinh viên có thể xem thông tin học phí của bản thân trên giao diện.

### 4.9. Triển khai thông báo và yêu cầu

Module thông báo giúp nhà trường gửi thông tin đến người dùng. Module yêu cầu hành chính cho phép sinh viên gửi yêu cầu và người có thẩm quyền duyệt hoặc xử lý. Module yêu cầu sửa điểm hỗ trợ sinh viên phản hồi khi có sai lệch điểm số, giáo viên hoặc admin xử lý theo quyền.

### 4.10. Middleware xử lý lỗi toàn cục

Backend sử dụng `GlobalExceptionMiddleware` để xử lý lỗi tập trung. Khi service phát sinh lỗi như không tìm thấy dữ liệu, dữ liệu không hợp lệ hoặc không đủ quyền, middleware chuyển đổi lỗi thành response chuẩn. Cách làm này giúp API trả lỗi nhất quán và frontend dễ xử lý thông báo cho người dùng.

### 4.11. Triển khai bằng Docker

Dự án cung cấp `Dockerfile`, `docker-compose.yml` và cấu hình Nginx cho frontend. Khi chạy bằng Docker Compose, hệ thống có thể khởi động frontend và backend bằng một lệnh:

```bash
docker compose up --build
```

Các địa chỉ mặc định:

- Frontend: `http://localhost:3100`
- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

---

## Chương 5. Kiểm Thử Hệ Thống

### 5.1. Mục tiêu kiểm thử

Kiểm thử nhằm đảm bảo các chức năng chính hoạt động đúng, API trả về dữ liệu hợp lệ, phân quyền được áp dụng đúng vai trò và frontend có thể thao tác với backend. Các nhóm chức năng cần kiểm thử gồm đăng nhập, quản lý dữ liệu, nhập điểm, học phí, thông báo và yêu cầu.

### 5.2. Kiểm thử API bằng Swagger

Swagger được tích hợp trong backend để liệt kê endpoint, mô tả request/response và hỗ trợ gọi thử API. Người kiểm thử có thể đăng nhập lấy token, nhập token vào Swagger và thực hiện các request cần xác thực.

### 5.3. Kiểm thử thủ công bằng Postman

Dự án có sẵn Postman collection tại `docs/UniversityPortal.postman_collection.json`. Collection này chứa các request đăng nhập theo vai trò, tự lưu access token và dùng token cho các request tiếp theo.

**Bảng 5.1. Kịch bản kiểm thử API bằng Postman**

| STT | Nhóm kiểm thử | Kịch bản | Kết quả mong đợi |
|---:|---|---|---|
| 1 | Auth | Đăng nhập bằng tài khoản Admin hợp lệ | Trả về status 200, access token và refresh token |
| 2 | Auth | Đăng nhập bằng tài khoản Giáo vụ hợp lệ | Trả về thông tin người dùng có vai trò Giáo vụ |
| 3 | Auth | Đăng nhập sai mật khẩu | Trả về lỗi xác thực |
| 4 | Sinh viên | Lấy danh sách sinh viên bằng tài khoản có quyền | Trả về danh sách sinh viên |
| 5 | Sinh viên | Tạo sinh viên với dữ liệu không hợp lệ | Trả về lỗi validation |
| 6 | Môn học | Thêm, sửa, xóa môn học | Dữ liệu được cập nhật đúng |
| 7 | Lớp học phần | Giáo viên nhập điểm | Điểm được lưu vào danh sách lớp học phần |
| 8 | Học phí | Cập nhật trạng thái học phí | Trạng thái đóng tiền thay đổi đúng |
| 9 | Thông báo | Tạo và xem thông báo | Người dùng nhận được thông báo phù hợp |
| 10 | Yêu cầu | Sinh viên gửi yêu cầu hành chính | Yêu cầu được tạo và chờ duyệt |

> Chèn hình tại đây: Hình 5.1. Kết quả kiểm thử API bằng Postman.

### 5.4. Kiểm thử frontend

Các màn hình frontend cần được kiểm thử:

- Màn hình đăng nhập.
- Dashboard sau khi đăng nhập.
- Trang quản lý sinh viên.
- Trang quản lý giảng viên.
- Trang quản lý ngành học, môn học, chương trình đào tạo.
- Trang lớp học phần và nhập điểm.
- Trang hồ sơ, bảng điểm và học phí của sinh viên.
- Trang thông báo, yêu cầu hành chính và yêu cầu sửa điểm.

### 5.5. Đánh giá kết quả kiểm thử

Qua quá trình kiểm thử thủ công, hệ thống đáp ứng các luồng nghiệp vụ chính. Các API đăng nhập, phân quyền và CRUD dữ liệu hoạt động theo thiết kế. Postman collection giúp kiểm thử nhanh các endpoint quan trọng. Trong các bước phát triển tiếp theo, cần bổ sung thêm unit test, integration test và kiểm thử tự động cho frontend để tăng độ tin cậy.

---

## Chương 6. Kết Quả Đạt Được Và Demo

### 6.1. Kết quả đạt được

Sau quá trình thực hiện, đồ án đã đạt được các kết quả chính:

- Xây dựng backend ASP.NET Core Web API theo kiến trúc phân lớp.
- Xây dựng frontend React + Vite cho nhiều nhóm người dùng.
- Hoàn thiện cơ chế đăng nhập bằng JWT và phân quyền theo vai trò.
- Triển khai các module quản lý sinh viên, giảng viên, tài khoản, ngành học, môn học, chương trình đào tạo, lớp sinh hoạt, lớp học phần, học phí, thông báo, yêu cầu hành chính và yêu cầu sửa điểm.
- Tích hợp Swagger để tài liệu hóa và kiểm thử API.
- Chuẩn bị Postman collection phục vụ kiểm thử thủ công.
- Hỗ trợ chạy hệ thống bằng Docker Compose.
- Chuẩn bị dữ liệu mẫu phục vụ demo.

### 6.2. Tài khoản demo

**Bảng 6.1. Tài khoản demo**

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Admin | `admin` | `Admin@123` |
| Giáo vụ | `giaovu01` | `Giaovu@123` |
| Giáo viên | `gv.tuan` | `Giaovien@123` |
| Sinh viên | `sv.an` | `Sinhvien@123` |

### 6.3. Kịch bản demo đề xuất

1. Khởi động hệ thống bằng Docker Compose.
2. Mở Swagger tại `http://localhost:8080/swagger` để giới thiệu API.
3. Đăng nhập bằng tài khoản Admin và trình bày token JWT.
4. Truy cập frontend tại `http://localhost:3100`.
5. Demo quản lý sinh viên hoặc môn học bằng tài khoản Admin/Giáo vụ.
6. Demo quản lý chương trình đào tạo và chi tiết chương trình đào tạo.
7. Đăng nhập bằng tài khoản Giáo viên, demo lớp học phần và nhập điểm.
8. Đăng nhập bằng tài khoản Sinh viên, demo hồ sơ, bảng điểm, học phí và gửi yêu cầu hành chính.
9. Demo Postman collection với một số request tiêu biểu.

### 6.4. Hình ảnh minh họa

> Chèn hình tại đây: Hình 4.1. Giao diện đăng nhập.

> Chèn hình tại đây: Hình 4.2. Giao diện dashboard.

> Chèn hình tại đây: Hình 4.3. Giao diện quản lý sinh viên.

> Chèn hình tại đây: Hình 4.4. Giao diện quản lý chương trình đào tạo.

> Chèn hình tại đây: Hình 6.1. Giao diện Swagger API.

---

## Chương 7. Kết Luận Và Hướng Phát Triển

### 7.1. Kết luận

Đồ án "Xây dựng hệ thống Cổng thông tin Đại học - University Portal" đã xây dựng được một hệ thống web mô phỏng các nghiệp vụ quản lý thông tin trong môi trường đại học. Hệ thống có backend API, frontend web, cơ sở dữ liệu, phân quyền người dùng, tài liệu API và công cụ hỗ trợ kiểm thử. Các chức năng chính như quản lý người dùng, sinh viên, giảng viên, chương trình đào tạo, lớp học phần, điểm, học phí, thông báo và yêu cầu đã được triển khai ở mức đáp ứng mục tiêu đề tài.

Thông qua đồ án, em đã củng cố kiến thức về phát triển ứng dụng web full-stack, thiết kế API, tổ chức mã nguồn theo Clean Architecture, sử dụng Entity Framework Core, xác thực JWT, xây dựng giao diện React và triển khai ứng dụng bằng Docker. Đây là nền tảng quan trọng để tiếp tục phát triển các hệ thống phần mềm thực tế có quy mô lớn hơn.

### 7.2. Hạn chế

Bên cạnh các kết quả đạt được, hệ thống vẫn còn một số hạn chế:

- Một số nghiệp vụ phức tạp của trường đại học thực tế chưa được mô phỏng đầy đủ.
- Kiểm thử tự động còn cần được bổ sung để bao phủ nhiều service và API hơn.
- Chưa tích hợp thông báo thời gian thực.
- Chưa có chức năng thanh toán học phí trực tuyến.
- Giao diện có thể tiếp tục cải thiện trải nghiệm người dùng và khả năng hiển thị trên nhiều thiết bị.

### 7.3. Hướng phát triển

Trong tương lai, hệ thống có thể được mở rộng theo các hướng sau:

- Bổ sung thanh toán học phí trực tuyến.
- Tích hợp thông báo realtime bằng SignalR hoặc WebSocket.
- Xây dựng app mobile cho sinh viên và giảng viên.
- Bổ sung chức năng xuất báo cáo PDF/Excel.
- Mở rộng module khảo sát, diễn đàn, đặt phòng thực hành và sinh hoạt chủ nhiệm.
- Bổ sung integration test, end-to-end test và CI/CD.
- Tối ưu hiệu năng truy vấn, phân trang, tìm kiếm và cache dữ liệu.
- Hoàn thiện phân quyền chi tiết theo từng nghiệp vụ và từng đơn vị quản lý.

---

# Tài Liệu Tham Khảo

1. Microsoft Docs, ASP.NET Core documentation.
2. Microsoft Docs, Entity Framework Core documentation.
3. Microsoft Docs, JWT Bearer authentication in ASP.NET Core.
4. React documentation.
5. Vite documentation.
6. Ant Design documentation.
7. Docker documentation.
8. Tài liệu mã nguồn dự án University Portal: `README.md`, `docs/schema.txt`, `docs/UniversityPortal.postman_collection.json`, `docs/erd_schema.html`.

---

# Phụ Lục

## Phụ lục A. Hướng dẫn chạy hệ thống

### Chạy bằng Docker Compose

1. Cấu hình connection string trong `docker-compose.yml`.
2. Chạy lệnh:

```bash
docker compose up --build
```

3. Truy cập:

- Frontend: `http://localhost:3100`
- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

### Chạy local backend

```bash
cd src/UniversityPortal.API
dotnet run
```

### Chạy local frontend

```bash
cd frontend
npm install
npm run dev
```

## Phụ lục B. Tài nguyên trong thư mục `docs`

| File | Mục đích |
|---|---|
| `docs/UniversityPortal.postman_collection.json` | Bộ request Postman kiểm thử API |
| `docs/erd_schema.html` | Sơ đồ ERD của hệ thống |
| `docs/schema.txt` | Mô tả schema cơ sở dữ liệu |
| `docs/seed_data.sql` | Dữ liệu mẫu phục vụ demo |
| `docs/deploy-ec2.md` | Tài liệu triển khai lên EC2 |
| `docs/presentation-guide.md` | Gợi ý trình bày/demo |

## Phụ lục C. Gợi ý hoàn thiện trước khi nộp

- Điền đầy đủ thông tin sinh viên, giảng viên hướng dẫn, khoa, trường và thời gian thực hiện ở trang đầu.
- Xuất hình ERD từ `docs/erd_schema.html` và chèn vào Chương 3.
- Chụp ảnh màn hình frontend, Swagger và Postman để thay thế các vị trí "Chèn hình tại đây".
- Chạy lại kiểm thử API bằng Postman và cập nhật bảng kết quả kiểm thử.
- Nếu chuyển sang Word/PDF, cập nhật lại số trang, mục lục, danh mục bảng và danh mục hình.
