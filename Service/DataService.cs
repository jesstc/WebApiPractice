using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSKA.Model;

namespace WebSKA.Service
{
    public class DataService
    {
        public List<Account> _accountData = new List<Account>();
        public List<Account> accountData
        {
            get => _accountData;
            set => _accountData = value;
        }
    }
}
