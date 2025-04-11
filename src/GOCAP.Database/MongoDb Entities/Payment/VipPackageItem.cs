using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCAP.Database
{
    public class VipPackageItem 
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int Price { get; set; }
        public int DurationDays { get; set; }
    }
}
