using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCAP.Repository.Intention
{
    public interface IVipPaymentRepository
    {
        Task<PaymentHistory?> GetPaymentByUserAndPackageIdAsync(Guid userId, int packageId);
        Task AddPaymentAsync(PaymentHistory payment);
        Task<PaymentHistory?> GetByOrderCodeAsync(string orderCode);
        Task UpdatePaymentAsync(PaymentHistory payment);

    }
}
