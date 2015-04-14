using System;
using System.Collections.Generic;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Project
    {
        public string name { get; set; }
        public string project_id { get; set; }
        public string user_company_id { get; set; }
        public string timezone { get; set; }
        public DateTime created_at { get; set; }
        public string address_id { get; set; }
        public string area { get; set; }
        public string status { get; set; }
        public bool bim_enabled { get; set; }
        public string account_id { get; set; }
        public string user_company { get; set; }
        public string email { get; set; }
        public int? default_issue_due_date { get; set; }
        public decimal? cost { get; set; }
        public string ptype { get; set; }
        public bool lock_closed_checklists { get; set; }
        public bool is_junior { get; set; }
        public string area_units { get; set; }
        public string description { get; set; }
        public bool is_trial { get; set; }
        public string issue_workflow_rule { get; set; }
        public bool lock_closed_tasks { get; set; }
        public string identifier { get; set; }
        public string user_roles { get; set; }
        public DateTime? completion_date { get; set; }
        public List<string> locale_info { get; set; }
        public string locale { get; set; }
        public bool always_allow_attachments { get; set; }
        public string telephone { get; set; }
        public string cost_currency { get; set; }
        public string owner_id { get; set; }
        public DateTime? start_date { get; set; }
        public bool task_edit_by_assignee_only { get; set; }
        public int percent_complete { get; set; }
        public DateTime updated_at { get; set; }
        public string fax { get; set; }
        public List<IssueFilter> issue_filters { get; set; }
        public List<Filter> filters { get; set; }
        public string brand_color { get; set; }
        public List<Permission> permissions { get; set; }
        public string brand { get; set; }
        public bool is_active { get; set; }
        public bool needs_duns { get; set; }
        public List<DocumentPath> document_paths { get; set; }
    }
}
