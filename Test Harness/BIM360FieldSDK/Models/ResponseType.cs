using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class ResponseType
    {
        public string name { get; set; }
        public int position { get; set; }
        public string display_type { get; set; }
        public string response_type_id { get; set; }
        public bool required { get; set; }
        public List<string> possible_values { get; set; }
    }
}
