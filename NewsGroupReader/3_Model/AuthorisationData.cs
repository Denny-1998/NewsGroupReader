using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsGroupReader.Model
{
    public class AuthorisationData
    {
        public string Address { get; }
        public string UserName { get; }
        public string Password { get; }
        public int Port { get; }



        public AuthorisationData(string address, int port, string username, string password)
        {

            Address = address;
            UserName = username;
            Password = password;
            Port = port;

        }



    }
}
