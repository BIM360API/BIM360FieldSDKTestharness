using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class User
    {
        public string id { get; set; }
        public string roles { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public bool junior { get; set; }
        public Company company { get; set; }
    }
}
