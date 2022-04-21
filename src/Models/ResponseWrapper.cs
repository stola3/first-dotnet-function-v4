using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.unico.admin.api.model
{
    internal class ResponseWrapper
    {
        public string data{ get; set; }
        public string? errorMessage { get; set; }
    }
}
