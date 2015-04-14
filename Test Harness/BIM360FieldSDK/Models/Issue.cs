using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autodesk.BIM360Field.APIService.Support;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Issue : ModelBase
    {
        // id is a synonym for issue_id because the json that comes back only contains issue_id and we'd RATHER refer to the primary
        // key field as 'id'
        public string id
        {
            get
            {
                return issue_id;
            }
        }
        public string issue_id { get; set; }
        public string description { get; set; }
        public string identifier { get; set; }
        public string created_by { get; set; }
        public string source_id { get; set; }
        public string source_type { get; set; }
        public string status { get; set; }
        public bool has_pushpin { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime created_at { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime updated_at { get; set; }
        public string area_id { get; set; }
        public List<Comment> comments { get; set; }
        public List<CustomFieldValue> custom_field_values { get; set; }
        public string creator_company_id { get; set; }
        public string company_id { get; set; }
        public string priority { get; set; }
    }
}
