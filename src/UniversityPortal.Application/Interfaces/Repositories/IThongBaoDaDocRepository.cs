using UniversityPortal.Domain.Entities;

namespace UniversityPortal.Application.Interfaces.Repositories;

public interface IThongBaoDaDocRepository : IRepository<ThongBaoDaDoc>
{
    Task<ThongBaoDaDoc?> GetAsync(int thongBaoId, int taiKhoanId);
}
