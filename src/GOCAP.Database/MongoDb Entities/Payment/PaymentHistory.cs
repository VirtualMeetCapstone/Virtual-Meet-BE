using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCAP.Database
{
    public class PaymentHistory : EntityMongoBase
    {
        public Guid UserId { get; set; }
        public string Level { get; set; } = "free"; 
        public int PackageId { get; set; }
        public string OrderCode { get; set; } = default!;
        public bool IsPaid { get; set; }
        public int Amount { get; set; }
    }
}
