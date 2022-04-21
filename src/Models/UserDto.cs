using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Unico.Admin.Api.Models
{
    public class UserDto
    {
        public string givenName { get; set; }
        public string sureName { get; set; }
        public string name { get; set; }
        public string samAccountName { get; set; }
        public string userPrincipalName { get; set; }
        public string emailAddress { get; set; }
        public string path { get; set; }
    }
}
