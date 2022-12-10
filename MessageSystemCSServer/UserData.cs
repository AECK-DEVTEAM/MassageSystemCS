using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSystemCSServer
{
    internal class UserData
    {
        private string _uid;
        private string _password;

        public UserData(string uid, string password)
        {
            _uid = uid;
            _password = password;
        }
    }
}
