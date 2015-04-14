using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class CustomFieldValue
    {
        public string id { get; set; }
        public string value { get; set; }
        public string display_type { get; set; }
        public List<String> possible_values { get; set; }

        public string custom_field_value_id { get; set; }
        public string custom_field_definition_id { get; set; }
        public string container_id { get; set; }
        public string container_type { get; set; }
    }
}
