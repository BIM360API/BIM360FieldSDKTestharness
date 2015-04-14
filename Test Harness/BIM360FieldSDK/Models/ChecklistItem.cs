using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autodesk.BIM360Field.APIService.Support;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class ChecklistItem : ModelBase
    {
        public ChecklistItem()
        {
            attachments = new List<AttachmentType>();
            document_references = new List<AttachmentType>();
        }

        public int position { get; set; }
        public string display_type { get; set; }
        public string question_text { get; set; }
        public string created_by { get; set; }
        public string is_conforming { get; set; }
        public string comment { get; set; }
        public string display_number { get; set; }
        public string response { get; set; }
        public string spec_ref { get; set; }
        public Checklist checklist { get; set; }
        public List<string> possible_values { get; set; }
        public List<AttachmentType> attachments { get; set; }
        public List<AttachmentType> document_references { get; set; }
        public List<Issue> issues { get; set; }
        public CustomFieldValue custom_field_values { get; set; }
        public List<Comment> comments { get; set; }
        public string company_id { get; set; }
        public bool is_section { get; set; }
        public string area_id { get; set; }
        public string template_item_id { get; set; }
        public TemplateItem template_item { get; set; }
        public string location_detail { get; set; }
        public string completed_checklist_id { get; set; }
        public string details { get; set; }
        public string checklist_item_id
        {
            get
            {
                return template_item_id;
            }
            set
            {
                template_item_id = value;
            }
        }
        public string completed_checklist_item_id
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
