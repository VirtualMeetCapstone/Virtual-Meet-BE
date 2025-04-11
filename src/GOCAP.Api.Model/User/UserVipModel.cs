using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCAP.Api.Model
{
    public class UserVipModel
    {
        public string Level { get; set; } = "free";
        public DateTime? ExpireAt { get; set; }
    }
}
