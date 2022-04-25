using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Services.Interfaces
{
    public interface IUserService
    {
        public User Login(string email, string password);
        public void Register(string name, string email, string password);
    }
}
