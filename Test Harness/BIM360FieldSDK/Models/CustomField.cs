using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
   public class CustomField
    {
       public bool required { get; set; }
       public string display_type { get; set; }
       public int position { get; set; }
       public string default_value { get; set; }
       public string name { get; set; }
       public string container_type { get; set; }
       public string custom_field_id { get; set; }
       public List<string> possible_values { get; set; }
       public DateTime updated_at { get; set; }
       public string container_id { get; set; }
    }
}
