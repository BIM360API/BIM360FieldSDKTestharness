using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Equipment : ModelBase
    {
        public string install_date { get; set; }
        public string equipment_id { get; set; }
        public List<UriReference> uri_references { get; set; }
        public string warranty_start_date { get; set; }
        public string bim_object_identifier { get; set; }
        public List<AttachmentType> document_references { get; set; }
        public List<AttachmentType> attachments { get; set; }
        public string submittal { get; set; }
        public string bim_file_id { get; set; }
        public string equipment_status_id { get; set; }
        public string tag_number { get; set; }
        public string created_by { get; set; }
        public List<Comment> comments { get; set; }
        public string source { get; set; }
        public string asset_identifier { get; set; }
        public string serial_number { get; set; }
        public string equipment_type_id { get; set; }
        public string warranty_end_date { get; set; }
        public string expected_life { get; set; }
        public bool deleted { get; set; }
        public string purchase_order { get; set; }
        public List<CustomFieldValue> custom_field_values { get; set; }
        public string area_id { get; set; }
        public string purchase_date { get; set; }
        public string description { get; set; }
        public string parent_id { get; set; }
        public string barcode { get; set; }
        public string name { get; set; }
    }
}
