using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class TemplateItem
    {
        public TemplateItem()
        {
            attachments = new List<AttachmentType>();
            document_references = new List<AttachmentType>();
        }

        public string template_id { get; set; }
        public string number { get; set; }
        public string root_cause_id { get; set; }
        public List<UriReference> uri_references { get; set; }
        public int position { get; set; }
        public string default_answer { get; set; }
        public string issue_description { get; set; }
        public string more_info { get; set; }
        public string company_info { get; set; }
        public string response_type_id { get; set; }
        public bool is_section { get; set; }
        public string template_item_id { get; set; }
        public string item_text { get; set; }
        public string spec_ref { get; set; }
        public ResponseType response_type { get; set; }
        public List<AttachmentType> attachments { get; set; }
        public List<AttachmentType> document_references { get; set; }
    }
}
