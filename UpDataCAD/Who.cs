using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace UpDataCAD
{
    public class Who
    {
        public int desktop;
        
        public string name;
        public string lastname;
        public string phone;
        public string email;
        public string file;
        public string department;
        public DateTime start;
        public DateTime end;
        

        override public string ToString()
        {
            string tmp = string.Empty;

            tmp = "desktop: '" + desktop + "', " +
                  "name: '" + name + "', " +
                  "lastnem: '" + lastname + "', " +
                  "phone: '" + phone + "', " +
                  "email: '" + email + "', " +
                  "file: '" + file + "', " +
                  "department: '" + department + "', " +
                  "start: '" + start + "', " +
                  "end: '" + end + "'";


            return tmp;
        }
    }
       
}
