using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Company
    {
        public string id { get; set; }
        public bool account_company { get; set; }
        public string url { get; set; }
        public string duns_no { get; set; }
        public string ein_no { get; set; }
        public string telephone { get; set; }
        public string company_type { get; set; }
        public string name { get; set; }
        public string fax { get; set; }
        public string description { get; set; }
        public string company_category { get; set; }
        public Address address { get; set; }
    }
}
