using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autodesk.BIM360Field.APIService.Support;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Filter
    {
        public string filter_id { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime created_at { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime updated_at { get; set; }
        public string container { get; set; }
        public int position { get; set; }
        public string updated_by { get; set; }
        public string name { get; set; }
        public string roles { get; set; }
        //public List<Condition> conditions { get; set; }
    }
}
