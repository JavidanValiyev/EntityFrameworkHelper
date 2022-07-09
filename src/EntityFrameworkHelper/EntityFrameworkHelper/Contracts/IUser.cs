using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkHelper.Contracts
{
    public interface IUser : IBaseContract
    {
        public Guid UserId { get; set; }
    }
}
