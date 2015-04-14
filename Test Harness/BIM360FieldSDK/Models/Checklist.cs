using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Autodesk.BIM360Field.APIService.Support;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Checklist : ModelBase
    {
        public Checklist()
        {
            attachments = new List<AttachmentType>();
            signatures = new List<AttachmentType>();
            sections = new List<ChecklistSection>();
            comments = new List<Comment>();
            template = new Template();
        }

        public string project_id { get; set; }
        public string status { get; set; }
        public string priority { get; set; }
        public string name { get; set; }
        public string checklist_type { get; set; }
        public string phase { get; set; }
        public string description { get; set; }
        public string identifier { get; set; }
        public Template template { get; set; }
        public List<Comment> comments { get; set; }
        public List<ChecklistSection> sections { get; set; }
        public List<AttachmentType> attachments { get; set; }
        public List<AttachmentType> signatures { get; set; }
        public Company company { get; set; }
        public List<CustomFieldValue> custom_field_values { get; set; }
        public string tags { get; set; }
        public string area_id { get; set; }
        public string source_id { get; set; }
        public string source_type { get; set; }
        public List<ChecklistItem> checklist_items { get; set; }
        public string company_id { get; set; }
        public string checklist_id { get; set; }
        public string completed_checklist_id 
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
    }
}
