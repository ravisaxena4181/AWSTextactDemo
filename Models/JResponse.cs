using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAWSTextract.Models
{
    public class JResponse
    {
        public int success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
       
    }
}