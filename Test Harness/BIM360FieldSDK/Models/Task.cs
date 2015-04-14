using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//TODO: Finish Task Object definition

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Task
    {
        public string id
        {
            get
            {
                return task_id;
            }
        }
        public string task_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string template { get; set; }
        public string identifier { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string company_id { get; set; }
        public string assigned_user_id { get; set; }
        public string location_detail { get; set; }
        public string send_reminders { get; set; }
        public DateTime scheduled_at { get; set; }
        public string name { get; set; }
        //task_types:  A JSON collection of task type IDs to associate with this Task/template. Each task type is specified using 'customizable_category_id'.
        //areas: A JSON collection of location IDs to associate with this Task/template. Each location is specified using 'area_id'.
        //checklists: A JSON collection of checklist IDs to associate with this Task/template. Each checklist is specified using 'checklist_id'.
        public List<Comment> comments { get; set; }
        public List<CustomFieldValue> custom_field_values { get; set; }
        public List<UriReference> uri_references { get; set; }
        //document_folder_references: A JSON collection of document folder references to associate with this Task/template.
    }
}
