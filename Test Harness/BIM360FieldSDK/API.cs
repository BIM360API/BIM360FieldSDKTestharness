using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using Autodesk.BIM360Field.APIService.Models;
using Newtonsoft.Json;
using RestSharp.Contrib;
using RestSharp;
using Autodesk.BIM360Field.APIService.Support;
using Microsoft.Win32;

namespace Autodesk.BIM360Field.APIService
{
    public class API
    {
        private string _url;
        private Token _token;
        private Project _defaultProject;

        private string _login = "/api/login";
        private string _logout = "/api/logout";
        private string _projects = "/api/projects";
        private string _project = "/api/project";
        private string _templates = "/api/templates";
        private string _contacts = "/api/contacts";
        private string _get_tasks = "/api/get_tasks";
        private string _checklist = "/fieldapi/checklists/v1"; // I'm RESTful (but, not all operations are supported!)
        private string _create_checklist = "/api/checklists";
        private string _get_checklists = "/api/get_checklists";
        private string _get_equipment = "/api/get_equipment";
        private string _issues = "/fieldapi/issues/v2"; // I'm RESTful
        private string _get_locations = "/fieldapi/admin/v1/locations";
        private string _get_companies = "/fieldapi/admin/v1/companies";
        private string _get_issue_types = "/fieldapi/issues/v1/types";
        private string _create_issue_type = "/fieldapi/issues/v1/create_type";
        private string _destroy_issue_type = "/fieldapi/issues/v1/destroy_type";
        private string _create_issue = "/fieldapi/issues/v1/create";
        private string _get_issues = "/api/get_issues";
        private string _issue_filters = "/api/issue_filters";
        private string _get_users = "/fieldapi/admin/v1/users";
        private string _publish = "/api/library/publish";
        private string _delete_document = "/api/library/delete";
        private string _all_folders = "/api/library/all_folders";
        private string _all_files = "/api/library/all_files";
        private string _custom_fields = "/api/custom_fields";
        private string _create_custom_field = "/fieldapi/admin/v1/custom_field_create";
        private string _vela_fields = "/api/vela_fields";
        private string _binary_data = "/api/binary_data";
        private string _create_project = "/fieldapi/admin/v1/project_create";
        private string _attachment = "/api/attachments";

        private string _datetime_format = "yyyy-MM-dd HH:mm:ss zzz";
        public API()
        {
            this._url = "https://bim360field.autodesk.com";
        }

        public API(string url)
        {
            this._url = url;
        }

        public Token Token
        {
            get
            {
                return _token;
            }
        }

        public string Server
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        /// <summary>
        /// Get or Set the default project to use for all API calls. Can still be overridden by passing the project ID to the call.
        /// </summary>
        public Project DefaultProject
        {
            get
            {
                return _defaultProject;
            }
            set
            {
                _defaultProject = value;
            }
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="username">The BIM 360 Field username to authenticate using</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string authenticate(string username, string password)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("username", username);
            p.Add("password", password);

            string response = performRESTRequest(_login, p, Method.GET);

            // TODO: Check for errors

            _token = JsonConvert.DeserializeObject<Token>(response);

            return _token.ticket.ToString();
        }

        /// <summary>
        /// Log out
        /// </summary>
        public void logout()
        {
            performRESTRequest(_logout, null, Method.POST);
        }

        /// <summary>
        /// Get all projects for the authenticated user
        /// </summary>
        /// <returns></returns>
        public List<Project> getProjects()
        {
            string response = performRESTRequest(_projects, null, Method.GET);
            return JsonConvert.DeserializeObject<List<Project>>(response);
        }

        public Project getProject(Project project)
        {
            return getProject(project.project_id);
        }

        /// <summary>
        /// Return detailed information about a project. Does NOT include everything that the UI shows, however!
        /// Includes information about filters for issues, tasks, checklists, etc
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public Project getProject(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_project, p, Method.POST);

            return JsonConvert.DeserializeObject<Project>(response);
        }

        #region Checklists

        /// <summary>
        /// Retrieve a single checklist
        /// </summary>
        /// <param name="checklist_ids">The IDs of the checklists to return</param>
        /// <param name="project_id">The ID of the project to get the checklist from</param>
        /// <returns></returns>
        public List<Checklist> getChecklist(string[] checklist_ids, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("checklist_ids", string.Join(",", checklist_ids));
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_get_checklists, p, Method.GET);

            return JsonConvert.DeserializeObject<List<Checklist>>(response);
        }

        /// <summary>
        /// Delete a checklist. Beware! This does not prompt, and is NOT reversable!
        /// </summary>
        /// <param name="id">The ID of the checklist to delete</param>
        /// <param name="project_id">The ID of the project to delete the checklist from</param>
        /// <remarks>NOT currently implemented! If you need this functionality, please contact support.</remarks>
        public void deleteChecklist(string id, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("id", id);
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string uri = _checklist + "/{id}";
            performRESTRequest(uri, p, Method.DELETE);
        }

        /// <summary>
        /// Retrieve a list of checklists in a project. The checklists returned in this call do NOT contain full data! This
        /// call is mostly to get the IDs, name, created_at, and identifiers.
        /// </summary>
        /// <param name="source_id">If you only want checklists attached to a particular source (Equipment or Task), pass that object's ID here</param>
        /// <param name="project_id">The ID of the project to retrieve checklists for</param>
        /// <param name="offset">Used to chunk the checklists. This is the starting point of the next chunk. Defaults to 0</param>
        /// <param name="limit">The maximum number of checklists to return. Defaults to 25</param>
        /// <returns></returns>
        public List<Checklist> getChecklists(string source_id = null, string project_id = null, int offset = 0, int limit = 25)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            p.Add("limit", limit.ToString());
            p.Add("offset", offset.ToString());

            if (source_id != null)
            {
                p.Add("source_id", source_id);
            }

            string response = performRESTRequest(_checklist + ".json", p, Method.GET);

            return JsonConvert.DeserializeObject<List<Checklist>>(response);
        }

        /// <summary>
        /// Retrieve a list of checklist templates in a project
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public List<Template> getChecklistTemplates(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_templates, p, Method.GET);
            return JsonConvert.DeserializeObject<List<Template>>(response);
        }

        /// <summary>
        /// Create a new blank checklist from a template
        /// </summary>
        /// <param name="template">The template to base the new checklist on</param>
        /// <param name="project_id"></param>
        /// <returns>The GUID of the newly created checklist</returns>
        public string createChecklist(Template template, string source_id = null, string source_type = null, string identifier = "", string location_id = "", string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string checklist_id = Guid.NewGuid().ToString();

            Checklist checklist = new Checklist();
            checklist.completed_checklist_id = checklist_id;
            checklist.created_at = DateTime.UtcNow;
            checklist.priority = template.priority;
            checklist.tags = template.tags;
            checklist.template = template;
            checklist.status = "Open";
            checklist.identifier = identifier;
            checklist.name = template.name;
            checklist.checklist_type = template.checklist_type;
            checklist.description = template.description;
            checklist.area_id = location_id;
            checklist.source_id = source_id;
            checklist.source_type = source_type;
            checklist.checklist_items = new List<ChecklistItem>();

            // Create the checklist items
            foreach (TemplateItem item in template.template_items)
            {
                ChecklistItem ci = new ChecklistItem();
                ci.id = Guid.NewGuid().ToString();
                ci.comment = ""; // TODO
                ci.position = item.position;
                ci.spec_ref = item.spec_ref;
                ci.response = item.default_answer;
                ci.checklist = checklist;
                ci.template_item = item;
                ci.template_item_id = item.template_item_id;

                checklist.checklist_items.Add(ci);
            }

            return updateChecklist(checklist, project_id);
        }

        /// <summary>
        /// Updates a checklist on the server. You can't change the structure of the checklist from outside the web app, and if you
        /// add or remove items, the API call will fail.
        /// </summary>
        /// <param name="checklist">The checklist to update. The best way to do this is to get it from the server, 
        /// make changes and then send it back using this method.</param>
        /// <param name="project_id">The ID of the project to update the checklist for</param>
        /// <remarks>NOT currently implemented. If you need this functionality, please contact support.</remarks>
        public string updateChecklist(Checklist cl, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            Dictionary<string, object> checklist = new Dictionary<string, object>();
            checklist.Add("id", cl.completed_checklist_id);
            checklist.Add("created_at", DateTime.Now.ToString(_datetime_format));
            checklist.Add("priority", cl.priority);
            checklist.Add("tags", cl.tags);
            if (cl.template == null || cl.template.id == null )
                checklist.Add("template_id", cl.checklist_id);
            else
                checklist.Add("template_id", cl.template.id);
            checklist.Add("status", cl.status);
            checklist.Add("identifier", cl.identifier);
            checklist.Add("name", cl.name);
            checklist.Add("checklist_type", cl.checklist_type);
            checklist.Add("description", cl.description);
            checklist.Add("area_id", cl.area_id);
            checklist.Add("source_id", cl.source_id);
            checklist.Add("source_type", cl.source_type);
            checklist.Add("company_id", cl.company_id);

            if (cl.custom_field_values != null && cl.custom_field_values.Count > 0)
            {
                List<Dictionary<string, string>> cfvs = new List<Dictionary<string, string>>();

                // Add in any custom fields
                foreach (CustomFieldValue cfv in cl.custom_field_values)
                {
                    Dictionary<string, string> c = new Dictionary<string, string>();
                    c.Add("container_id", cfv.container_id);
                    c.Add("container_type", cfv.container_type);
                    c.Add("custom_field_definition_id", cfv.custom_field_definition_id);
                    c.Add("custom_field_value_id", cfv.custom_field_value_id);
                    c.Add("value", cfv.value);
                    c.Add("id", cfv.id);

                    cfvs.Add(c);
                }

                checklist.Add("custom_field_values", cfvs); 
            }

            List<Dictionary<string, string>> items = new List<Dictionary<string, string>>();
            
            // Create the checklist items
            foreach (ChecklistItem item in cl.checklist_items)
            {
                Dictionary<string, string> ci = new Dictionary<string, string>();
                ci.Add("id", item.id);
                ci.Add("template_item_id", item.template_item_id);
                ci.Add("checklist_id", cl.id);
                ci.Add("comment", item.comment);
                ci.Add("position", item.position.ToString());
                ci.Add("spec_ref", item.spec_ref);
                if (item.template_item != null)
                {
                    if (item.template_item.response_type.display_type == "date")
                    {
                        ci.Add("response", DateTime.Today.ToShortDateString());
                    }
                    else
                    {
                        ci.Add("response", item.template_item.default_answer);
                    }
                }
                else
                {
                    ci.Add("response", item.response);
                }

                ci.Add("location_detail", item.location_detail);
                ci.Add("company_id", item.company_id);
                ci.Add("area_id", item.area_id);

                items.Add(ci);
            }

            checklist.Add("checklist_items", items);

            List<Dictionary<string, object>> checklists = new List<Dictionary<string, object>>();
            checklists.Add(checklist);

            p.Add("checklists", JsonConvert.SerializeObject(checklists));

            string response = performRESTRequest(_create_checklist, p, Method.POST);
            return cl.id;
        }

        #endregion

        #region Equipment

        /// <summary>
        /// Gets the equipment.
        /// </summary>
        /// <param name="equipment_ids">The equipment_ids.</param>
        /// <param name="project_id">The project_id.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public List<Equipment> getEquipment(string[] equipment_ids = null, string project_id = null, int offset = 0, int limit = 25, string step = null, string details = "all")
        {
            //TODO: Document what this is all about
            if (step == null)
            {
                step = "headers";
            }
            if (details == null)
            {
                details = "all";
            }

            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            if (equipment_ids != null)
            {
                p.Add("equipment_ids", string.Join(",", equipment_ids));
            }
            p.Add("limit", limit.ToString());
            p.Add("offset", offset.ToString());
            p.Add("step", step);
            p.Add("details", details);

            string response = performRESTRequest(_get_equipment, p, Method.GET);
            return JsonConvert.DeserializeObject<List<Equipment>>(response);
        }

        #endregion

        #region Tasks

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <param name="project_id">The project_id.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="limit">The limit.</param>
        /// <returns></returns>
        public List<Task> getTasks(string project_id = null, int offset = 0, int limit = 25) //max_date, filter_id
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            p.Add("limit", limit.ToString());
            p.Add("offset", offset.ToString());

            string response = performRESTRequest(_get_tasks, p, Method.GET);

            //return JsonConvert.DeserializeObject<List<Task>>(response);
            return new List<Task>();
        }

        #endregion

        /// <summary>
        /// Gets the contacts.
        /// </summary>
        /// <param name="project_id">The project_id.</param>
        /// <returns></returns>
        public List<Contact> getContacts(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_contacts, p, Method.GET);

            return JsonConvert.DeserializeObject<List<Contact>>(response);
        }


        #region Issues

        /// <summary>
        /// Retrieve a list of issues
        /// </summary>
        /// <param name="area_id">Optionally, a list of area_ids to retrieve issues for</param>
        /// <param name="project_id"></param>
        public List<Issue> getIssueList(string[] area_ids = null, string issue_filter_id = null, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();

            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            if (area_ids != null && area_ids.Length > 0)
            {
                p.Add("area_ids", String.Join(",", area_ids));
            }
            if (issue_filter_id != null)
            {
                p.Add("issue_filter_id", issue_filter_id);
            }

            string response = performRESTRequest(_get_issues, p, Method.GET);
            return JsonConvert.DeserializeObject<List<Issue>>(response);
        }

        /// <summary>
        /// Gets the issue count.
        /// </summary>
        /// <param name="area_ids">The area_ids.</param>
        /// <param name="project_id">The project_id.</param>
        /// <returns></returns>
        public int getIssueCount(string[] area_ids = null, string issue_filter_id = null, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();

            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            p.Add("count_only", "true");
            if (area_ids != null && area_ids.Length > 0)
            {
                p.Add("area_ids", String.Join(",", area_ids));
            }
            if (issue_filter_id != null)
            {
                p.Add("issue_filter_id", issue_filter_id);
            }

            string response = performRESTRequest(_get_issues, p, Method.GET);
            Count issueCount = JsonConvert.DeserializeObject<Count>(response);
            int retVal = 0;
            if (issueCount != null)
            {
                retVal = issueCount.count;
            }
            return retVal;
        }

        /// <summary>
        /// Gets the issue filters.
        /// </summary>
        /// <param name="project_id">The project_id.</param>
        /// <returns></returns>
        public List<IssueFilter> getIssueFilters(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();

            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_issue_filters, p, Method.GET);
            return JsonConvert.DeserializeObject<List<IssueFilter>>(response);
        }

        /// <summary>
        /// Return a single issue
        /// </summary>
        /// <param name="id">The ID of the issue to return</param>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public Issue getIssue(string id, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("id", id);
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string uri = _issues + "/{id}";

            string response = performRESTRequest(uri, p, Method.GET);
            return JsonConvert.DeserializeObject<Issue>(response);
        }

        /// <summary>
        /// Delete an issue
        /// </summary>
        /// <param name="id">The ID of the issue to delete</param>
        /// <param name="project_id"></param>
        public void deleteIssue(string id, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();

            p.Add("id", id);
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string uri = _issues + "/{id}";
            performRESTRequest(uri, p, Method.DELETE);
        }

        /// <summary>
        /// Delete an issue
        /// </summary>
        /// <param name="issue">The issue object to delete</param>
        /// <param name="project_id"></param>
        public void deleteIssue(Issue issue, string project_id = null)
        {
            deleteIssue(issue.id, project_id);
        }

        #endregion

        /// <summary>
        /// Retrurn a list of all locations for a project.
        /// </summary>
        /// <param name="project_id">The ID of the project to retrieve locations for</param>
        /// <returns>List of Location objects</returns>
        public List<Location> getLocations(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_get_locations, p, Method.GET);
            return JsonConvert.DeserializeObject<List<Location>>(response);
        }

        /// <summary>
        /// Return a list of all companies for a project
        /// </summary>
        /// <param name="project_id">The ID of the project to retrieve companies for</param>
        /// <returns>List of Company objects</returns>
        public List<Company> getCompanies(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_get_companies, p, Method.GET);
            return JsonConvert.DeserializeObject<List<Company>>(response);
        }

        /// <summary>
        /// Return a list of all users in a project
        /// </summary>
        /// <param name="project_id">The ID of the project to retrieve users for</param>
        /// <returns>List of User objects</returns>
        public List<User> getUsers(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_get_users, p, Method.GET);
            return JsonConvert.DeserializeObject<List<User>>(response);
        }

        #region Custom Fields

        /// <summary>
        /// Retrieve a list of all custom fields defined for a project. These are just the definitions of the fields - not the values!
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public List<CustomField> getCustomFieldDefinitions(string project_id)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id);

            string response = performRESTRequest(_custom_fields, p, Method.POST);
            return JsonConvert.DeserializeObject<List<CustomField>>(response);
        }

        /// <summary>
        /// Create a new custom field definition
        /// TODO: Our JSON representations of objects are inconsistent and is rendering the deserialization almost useless for some high level types. There's probably a way
        /// to tell the JSON.NET deserializer to map to different fields, but that just seems wrong.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="required"></param>
        /// <param name="possible_values"></param>
        /// <param name="default_value"></param>
        /// <param name="sub_category_id"></param>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public CustomField createCustomFieldDefinition(string category, string name, string type, bool required, List<string> possible_values = null, string default_value = null, string sub_category_id = null, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            p.Add("category", category);
            p.Add("name", name);
            p.Add("type", type);
            p.Add("required", required ? "1" : "0");
            p.Add("possible_values", string.Join("::", possible_values.ToArray()));
            p.Add("default_value", default_value);
            p.Add("sub_category_id", sub_category_id);

            string response = performRESTRequest(_create_custom_field, p, Method.POST);
            return JsonConvert.DeserializeObject<CustomField>(response);
        }

        /// <summary>
        /// Retrieve a list of all vela fields defined for a project. These are just the definitions of the fields - not the values!
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public List<VelaField> getVelaFieldDefinitions(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_vela_fields, p, Method.POST);
            return JsonConvert.DeserializeObject<List<VelaField>>(response);
        }

        #endregion

        #region Attachments, Documents, Binaries, etc

        /// <summary>
        /// Return metadata for documents contained within a project's document library
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public List<Document> getFolderDocuments(string directory = null, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            if (directory != null)
                p.Add("directory", directory);

            string response = performRESTRequest(_all_files, p, Method.GET);
            return JsonConvert.DeserializeObject<List<Document>>(response);
        }

        /// <summary>
        /// Download a binary file from BIM 360 Field
        /// </summary>
        /// <param name="object_type">The type of object to retrieve (Attachment, DocumentReference, Signature, Document)</param>
        /// <param name="object_id">The ID of the object to retrieve</param>
        /// <param name="image_type">Which representation of the file to retrieve (original, thumb, vv, composite)</param>
        /// <param name="page">Which page of the document to retrieve. This only matters for vv and composite images</param>
        public byte[] getBinaryFile(string object_type, string object_id, string image_type = "original", int page = 0, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            p.Add("object_type", object_type);
            p.Add("object_id", object_id);
            p.Add("image_type", image_type);
            p.Add("page", page.ToString());

            return performRESTDownload(_binary_data, p);
        }

        public byte[] getBinaryFile(AttachmentType attachment, string image_type = "original", int page = 0, string project_id = null)
        {
            return getBinaryFile(attachment.type, attachment.id, image_type, page, project_id);
        }

        /// <summary>
        /// Publish a document to the BIM 360 Field document library
        /// </summary>
        /// <param name="project_id"></param>
        /// <param name="file"></param>
        public Document publishDocument(FileInfo file, string directory, string filename = null, bool replace = false, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            p.Add("directory", directory);
            p.Add("filename", filename == null ? file.Name : filename);
            p.Add("replace", replace ? "1" : "0");

            Dictionary<string, FileInfo> doc = new Dictionary<string, FileInfo>();
            doc.Add("Filedata", file);
            string response = performRESTRequest(_publish, p, Method.POST, doc);
            return JsonConvert.DeserializeObject<Document>(response);
        }

        /// <summary>
        /// Remove a document from the document library
        /// </summary>
        /// <param name="document">The document to delete</param>
        /// <param name="revision">The specific revision to delete or all revisions if null</param>
        /// <param name="project_id"></param>
        public void deleteDocument(Document document, int? revision = null, string project_id = null)
        {
            deleteDocument(document.document_id, revision, project_id);
        }

        /// <summary>
        /// Remove a document from the document library
        /// </summary>
        /// <param name="document_id">The ID of the document to delete</param>
        /// <param name="revision">The specific revision to delete or all revisions if null</param>
        /// <param name="project_id"></param>
        public void deleteDocument(string document_id, int? revision = null, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string uri = _delete_document + "/{id}";
            if (revision != null)
            {
                uri += "/{rev}";
            }

            performRESTRequest(uri, p, Method.GET);
        }

        /// <summary>
        /// Update a new revision of an existing document
        /// </summary>
        /// <param name="doc">The document to base the revision on</param>
        /// <param name="file">The file to use as the new revision</param>
        /// <param name="filename"></param>
        public void revisionDocument(Document doc, FileInfo file, string filename = null, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id :project_id);
            p.Add("document_id", doc.document_id);
            p.Add("filename", filename == null ? file.Name : filename);
            p.Add("replace", "0");

            Dictionary<string, FileInfo> revdoc = new Dictionary<string, FileInfo>();
            revdoc.Add("Filedata", file);

            performRESTRequest(_publish, p, Method.POST, revdoc);
        }

        /// <summary>
        /// Retrieve a list of folders available in a project's document library
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public List<Folder> getLibraryFolders(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_all_folders, p);

            return JsonConvert.DeserializeObject<List<Folder>>(response);
        }

        /// <summary>
        /// Retrieve a list of documents in a project's document library, optionally specifying a directory
        /// </summary>
        /// <param name="project_id"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public List<Document> getDocumentList(string project_id = null, string directory = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id :project_id);
            if(directory != null)
                p.Add("directory", directory);

            string response = performRESTRequest(_all_files, p);

            return JsonConvert.DeserializeObject<List<Document>>(response);
        }

        #endregion

        #region Attachments

        /// <summary>
        /// Delete an attachment
        /// </summary>
        /// <param name="id">The unique ID of the attachment to delete</param>
        /// <param name="project_id">The project to delete the attachment from</param>
        public void deleteAttachment(string id, string project_id = null)
        {
        }

        /// <summary>
        /// Upload and attach a file to a parent object (Issue, Equipment, etc)
        /// </summary>
        /// <param name="original">The file to upload</param>
        /// <param name="thumbnail">A thumbnail representation of the file in png format</param>
        /// <param name="source_id">The unique ID of the parent object</param>
        /// <param name="source_type">The type of parent object (Issue, Equipment, Task)</param>
        /// <param name="caption">The caption for the attachment</param>
        /// <param name="tags">A comma separated list of tags</param>
        /// <param name="project_id">The project ID to upload the attachment to</param>
        public void createAttachment(FileInfo original, FileInfo thumbnail, string source_id, string source_type, string caption = "", string tags = "", string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            Dictionary<string, string> att = new Dictionary<string, string>();
            att["id"] = Guid.NewGuid().ToString().ToLower();
            att["deleted"] = "0";
            att["fcreate_date"] = original.CreationTimeUtc.ToString(_datetime_format);
            att["fmod_date"] = original.LastWriteTimeUtc.ToString(_datetime_format);
            att["created_at"] = original.CreationTimeUtc.ToString(_datetime_format);
            att["updated_at"] = original.LastWriteTimeUtc.ToString(_datetime_format);
            att["caption"] = caption;
            att["tags"] = tags;
            att["size"] = original.Length.ToString();
            att["content_type"] = MIMEAssistant.GetMIMEType(original.Name);
            att["filename"] = original.Name;
            att["container_id"] = source_id;
            att["container_type"] = source_type;

            p.Add("attachment", JsonConvert.SerializeObject(att));
            Dictionary<string, FileInfo> files = new Dictionary<string, FileInfo>();
            files.Add("original", original);
            files.Add("thumb", thumbnail);

            performRESTRequest(_attachment, p, Method.POST, files);
        }
        #endregion

        #region Issue Types

        /// <summary>
        /// Retrieve complete set of issue types defined in a project
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        public List<IssueType> getIssueTypes(string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);

            string response = performRESTRequest(_get_issue_types, p, Method.GET);
            return JsonConvert.DeserializeObject<List<IssueType>>(response);
        }

        /// <summary>
        /// Create a new issue type
        /// </summary>
        /// <param name="category_name">Name of category (e.g. Punch List, Safety)</param>
        /// <param name="name">Name of issue type within category (e.g. Observation, Concealment Work to Complete)</param>
        /// <returns></returns>
        public IssueType createIssueType(string category_name, string name, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            p.Add("category_name", category_name);
            p.Add("name", name);

            string response = performRESTRequest(_create_issue_type, p, Method.POST);
            return JsonConvert.DeserializeObject<IssueType>(response);
        }

        /// <summary>
        /// Destroy an issue type and reassign all associated issues to another issue type
        /// </summary>
        /// <param name="issue_type_id">The ID of the issue type to destroy</param>
        /// <param name="alternate_issue_type_id">The ID of the issue type to assign to affected issues</param>
        public void destroyIssueType(string issue_type_id, string alternate_issue_type_id, string project_id = null)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("project_id", project_id == null ? DefaultProject.project_id : project_id);
            p.Add("issue_type_id", issue_type_id);
            p.Add("alternate_issue_type_id", alternate_issue_type_id);

            performRESTRequest(_destroy_issue_type, p, Method.POST);
        }

        #endregion

        #region Service Related Code

        string GetMimeType(FileInfo fileInfo)
        {
            string mimeType = "application/unknown";

            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(
                fileInfo.Extension.ToLower()
                );

            if (regKey != null)
            {
                object contentType = regKey.GetValue("Content Type");

                if (contentType != null)
                    mimeType = contentType.ToString();
            }

            return mimeType;
        }

        /// <summary>
        /// Used to download binary data and return it as a byte array
        /// </summary>
        /// <param name="uri">The URI to call (eg. api/binary_data)</param>
        /// <param name="p">A dictionary of the parameters to pass to the API call</param>
        /// <param name="method"></param>
        /// <returns></returns>
        private byte[] performRESTDownload(string uri, Dictionary<string, string> p, Method method = Method.GET)
        {
            IRestResponse response = buildRESTRequest(uri, p, method);
            return response.RawBytes;
        }

        /// <summary>
        /// Make a REST call to the BIM 360 Field API
        /// </summary>
        /// <param name="uri">The URI to call (eg. api/login)</param>
        /// <param name="p">A dictionary of the parameters to pass to the API call</param>
        /// <param name="method">The HTTP method to use</param>
        /// <returns>JSON result or empty string if the call doesn't return data</returns>
        private string performRESTRequest(string uri, Dictionary<string, string> p, Method method = Method.GET, Dictionary<string,FileInfo> files = null, object body = null)
        {
           
            IRestResponse response = buildRESTRequest(uri, p, method, files, body);
            return response.Content;
        }

        /// <summary>
        /// Build up and execute a REST request
        /// </summary>
        /// <param name="uri">The URI of the REST request. Don't prepend the host information since that will be set automatically.</param>
        /// <param name="p">A Dictionary of parameters</param>
        /// <param name="method">Which HTTP method to use</param>
        /// <returns>The REST response object</returns>
        private IRestResponse buildRESTRequest(string uri, Dictionary<string, string> p, Method method = Method.GET, Dictionary<string,FileInfo> files = null, object body = null)
        {
            if (p == null)
                p = new Dictionary<string, string>();

            p.Add("application_version", "10.0");
            p.Add("device_type", "BIM 360 Field .NET API Client");

            IRestClient client = new RestClient();
            client.BaseUrl = _url;

            IRestRequest request = new RestRequest(uri, method);
            request.RequestFormat = DataFormat.Json;

            if (body != null && (method == Method.PUT || method == Method.POST))
            {
                if (_token != null)
                {
                    ((Dictionary<string, object>)body).Add("ticket", _token.ticket);
                }
                request.AddBody(body);
                request.AddUrlSegment("id", p["id"]);

                // This is a hack for the document delete method
                if (p.ContainsKey("rev"))
                {
                    request.AddUrlSegment("rev", p["rev"]);
                }
            }
            else
            {
                if (_token != null)
                {
                    p.Add("ticket", _token.ticket);
                }

                if (files != null)
                {
                    foreach(var upload in files)
                    {
                        request.AddFile(upload.Key, upload.Value.FullName);
                    }
                }

                if (p != null)
                {
                    foreach (string key in p.Keys)
                    {
                        if (key == "id")
                        {
                            request.AddUrlSegment(key, p[key]);
                        }
                        else
                        {
                            request.AddParameter(key, p[key]);
                        }
                    }
                }
            }

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException(response.Content);
            }

            int code = (int)response.StatusCode;
            if (code >= 400)
            {
                throw new BIM360FieldAPIException(response.Content, code);
            }

            return response;
        }

        #endregion
    }
}
