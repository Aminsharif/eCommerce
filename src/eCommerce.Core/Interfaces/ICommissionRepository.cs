using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface ICommissionRepository : IGenericRepository<Commission>
    {
        Task<IEnumerable<Commission>> GetByVendorIdAsync(int vendorId);
        Task<IEnumerable<Commission>> GetByStatusAsync(CommissionStatus status);
        Task<decimal> GetTotalCommissionsByVendorAsync(int vendorId);
        Task<decimal> GetTotalCommissionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Commission>> GetPendingCommissionsAsync();
        Task<IEnumerable<Commission>> GetPaidCommissionsAsync();
        Task<bool> MarkAsPaidAsync(int commissionId);
        Task<IEnumerable<Commission>> GetByOrderIdAsync(int orderId);
        Task<decimal> GetVendorCommissionByDateRangeAsync(int vendorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Commission>> GetCommissionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Commission> AddCommissionAsync(Commission commission);
    }
} 