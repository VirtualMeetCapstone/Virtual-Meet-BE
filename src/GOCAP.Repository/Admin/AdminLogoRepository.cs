using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCAP.Repository.Admin
{
    [RegisterService(typeof(IAdminLogoRepository))]
    internal class AdminLogoRepository(AppMongoDbContext _context)
    : MongoRepositoryBase<UserVip>(_context), IAdminLogoRepository
    {

    }
}
