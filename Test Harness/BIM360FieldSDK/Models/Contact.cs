using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autodesk.BIM360Field.APIService.Support;

//TODO: Finish Contact Object definition

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Contact
    {
        public string company_id { get; set; }
        public string last_name { get; set; }
        public string user_id { get; set; }
        public string roles { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string key_contact { get; set; }
        public string first_name { get; set; }
        public string id { get; set; }
        public string junior { get; set; }
    }
}
