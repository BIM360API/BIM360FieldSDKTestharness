using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autodesk.BIM360Field.APIService.Support;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Template
    {
        public string id
        {
            get
            {
                return template_id;
            }
        }
        public string template_id { get; set; }
        public string name { get; set; }
        public string checklist_type { get; set; }
        public string footer { get; set; }
        public bool auto_create_issue { get; set; }
        public string issue_type_id { get; set; }
        public string description { get; set; }
        public List<TemplateItem> template_items {get;set;}
        public string company_id { get; set; }
        public string tags { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime updated_at { get; set; }
        public string header { get; set; }
        public string priority { get; set; }
    }
}
