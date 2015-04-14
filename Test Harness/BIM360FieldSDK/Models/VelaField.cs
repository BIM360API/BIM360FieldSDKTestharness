using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class VelaField
    {
        public bool hidden { get; set; }
        public string vela_field_id { get; set; }
        public bool required { get; set; }
        public string display_type { get; set; }
        public string default_value { get; set; }
        public string name { get; set; }
        public string container_type { get; set; }
        public DateTime updated_at { get; set; }
        public string container_id { get; set; }
    }
}
