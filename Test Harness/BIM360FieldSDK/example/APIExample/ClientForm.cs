// Copyright 2012 Autodesk, Inc.  All rights reserved.
// Use of this software is subject to the terms of the Autodesk license agreement 
// provided at the time of installation or download, or which otherwise accompanies 
// this software in either electronic or hard copy form.   

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using Utility.ModifyRegistry;
using Microsoft.Win32;
using Autodesk.BIM360Field.APIService;
using Autodesk.BIM360Field.APIService.Models;
using Autodesk.BIM360Field.APIService.Support;

namespace BIM360FieldSDKTestClient
{
    public partial class ClientForm : Form
    {
        static string username = String.Empty;
        static string password = String.Empty;
        static string server = "https://manage.velasystems.com";
        static string token = String.Empty;

        static API api = new API(server);

        static string regSubKey = "SOFTWARE\\Autodesk\\FieldTestHarness\\Settings";

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientForm"/> class.
        /// </summary>
        public ClientForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the ClientForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void ClientForm_Load(object sender, EventArgs e)
        {
            LoadSettings();

            this.Closing += new System.ComponentModel.CancelEventHandler(this.ClientForm_Closing);
            //Adding columns to the ListView control
            //lvProjects.Columns.Add("project_name", 150);
            //lvProjects.Columns.Add("created_date", 125);
            //lvProjects.Columns.Add("project_id", 200);
            //lvProjects.Columns.Add("is_Active", 125);
        }

        /// <summary>
        /// Handles the Closing event of the ClientForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void ClientForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            // {
            //   e.Cancel = true;
            // }

            StoreSettings();
        }

        //===============================================================
        // Save and restore registry settings
        //===============================================================
        #region Registry Routines
        void ShowHelperToolTip(object sender, EventArgs e)
        {
            toolTip1.Show("To get started, fill in the information about \nBIM 360 in this section.", txt_base_url, new Point(50, 80));
        }

        /// <summary>
        /// Stores the settings.
        /// </summary>
        void StoreSettings()
        {
            // Setup Registry Save
            // work in progress -MV
            ModifyRegistry myRegistry = new ModifyRegistry();
            myRegistry.BaseRegistryKey = Registry.CurrentUser;
            myRegistry.SubKey = regSubKey;

            myRegistry.Write("BASE_URL", txt_base_url.Text);
            myRegistry.Write("LOGIN_NAME", txt_login_name.Text);
            myRegistry.Write("AUTH_TOKEN", txt_auth_token.Text);

            // Password - Yes, this should really be encrypted, but we are going to just do something simple
            // to "scramble" the text
            myRegistry.Write("USER_PASSWORD", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(txt_password.Text)));
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        void LoadSettings()
        {
            ModifyRegistry myRegistry = new ModifyRegistry();
            myRegistry.BaseRegistryKey = Registry.CurrentUser;
            myRegistry.SubKey = regSubKey;

            txt_base_url.Text = myRegistry.Read("BASE_URL", "https://manage.velasystems.com");

            txt_login_name.Text = myRegistry.Read("LOGIN_NAME", "");

            string loadPW = myRegistry.Read("USER_PASSWORD");
            if (loadPW != null)
            {
                byte[] decbuff = Convert.FromBase64String(loadPW);
                string decPW = System.Text.Encoding.UTF8.GetString(decbuff);
                txt_password.Text = decPW;
            }

            //txt_auth_token.Text = myRegistry.Read("AUTH_TOKEN", "");

            //if (txt_auth_token.Text != "")
            //{
            //    buttonLogin.Enabled = false;
            //    buttonLogout.Enabled = true;
            //}
            //else
            //{
            //    buttonLogin.Enabled = true;
            //    buttonLogout.Enabled = false;
            //}
        }

        #endregion Registry Routines

        /// <summary>
        /// Handles the Click event of the buttonLogin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(txt_login_name.Text)) || (String.IsNullOrEmpty(txt_password.Text)))
            {
                MessageBox.Show("You must enter a login name and password for this service call", "BIM 360 Field API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }
            username = txt_login_name.Text;
            password = txt_password.Text;
            server = txt_base_url.Text;

            token = api.authenticate(username, password);
            ResponseTextBox.Text = "Authenticated successfully. Token = " + token + "\n\n";

            txt_auth_token.Text = token;
        }

        /// <summary>
        /// Handles the Click event of the buttonLogout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonLogout_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 Glue API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }
            API api = new API(server);
            api.logout();
            txt_auth_token.Text = String.Empty;

        }

        /// <summary>
        /// Handles the Click event of the button_getProjects control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void button_getProjects_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }

            List<Project> projects = api.getProjects();

            if (projects != null)
            {
                dgvProjects.DataSource = null;

                dgvProjects.AutoGenerateColumns = false;
                dgvProjects.ColumnCount = 4;

                dgvProjects.Columns[0].Name = "Name";
                dgvProjects.Columns[0].HeaderText = "Name";
                dgvProjects.Columns[0].DataPropertyName = "name";

                dgvProjects.Columns[1].Name = "CreatedAt";
                dgvProjects.Columns[1].HeaderText = "Created At";
                dgvProjects.Columns[1].DefaultCellStyle.Format = "MM/dd/yyyy";
                dgvProjects.Columns[1].DataPropertyName = "created_at";

                dgvProjects.Columns[2].Name = "ProjectId";
                dgvProjects.Columns[2].HeaderText = "Project Id";
                dgvProjects.Columns[2].DataPropertyName = "project_id";

                dgvProjects.Columns[3].Name = "IsActive";
                dgvProjects.Columns[3].HeaderText = "Active";
                dgvProjects.Columns[3].DataPropertyName = "is_active";
                dgvProjects.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; 

                dgvProjects.DataSource = projects;

            }
        }

        /// <summary>
        /// Handles the Click event of the button_getCheckLists control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void button_getCheckLists_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();

            // Set the default project on the API service object and you won't have to pass it each time you make a call.
            // Just DON'T forget to change it if you need to interact with other projects!
            Project selectedProject = api.getProject(selectedProjectId);
            if (selectedProject != null)
            {
                api.DefaultProject = selectedProject;
            }

            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                int pageNum = Convert.ToInt16(numPageNumChecklist.Value);
                int pageRowCount = Convert.ToInt16(numPageRowCountChecklist.Value);
                List<Checklist> checklists = api.getChecklists(null, selectedProjectId, pageNum, pageRowCount);

                if (checklists != null)
                {
                    dgvCheckLists.DataSource = null;

                    dgvCheckLists.AutoGenerateColumns = false;
                    dgvCheckLists.ColumnCount = 4;

                    dgvCheckLists.Columns[0].Name = "Name";
                    dgvCheckLists.Columns[0].HeaderText = "Name";
                    dgvCheckLists.Columns[0].DataPropertyName = "name";

                    dgvCheckLists.Columns[1].Name = "CreatedAt";
                    dgvCheckLists.Columns[1].HeaderText = "Created At";
                    dgvCheckLists.Columns[1].DefaultCellStyle.Format = "MM/dd/yyyy";
                    dgvCheckLists.Columns[1].DataPropertyName = "created_at";

                    dgvCheckLists.Columns[2].Name = "CheckListId";
                    dgvCheckLists.Columns[2].HeaderText = "Checklist Id";
                    dgvCheckLists.Columns[2].DataPropertyName = "id";

                    dgvCheckLists.Columns[3].Name = "Type";
                    dgvCheckLists.Columns[3].HeaderText = "Type";
                    dgvCheckLists.Columns[3].DataPropertyName = "checklist_type";
                    //dgvCheckLists.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    dgvCheckLists.DataSource = checklists;
                    //dgvCheckLists.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.Fill);
                }
            }

            #region Legacy

            //if (lvProjects.SelectedItems.Count > 0)
            //{
            //    lvCheckLists.Clear();

            //    string selectedProjectId = lvProjects.SelectedItems[0].SubItems[2].Text;
            //    Project selectedProject = api.getProject(selectedProjectId);

            //    if (selectedProject != null)
            //    {

            //        api.DefaultProject = selectedProject;

            //        List<Checklist> checklists = api.getChecklists(null, null, 0, 10000); // Defaults to 25

            //        if (checklists != null)
            //        {
            //            Console.WriteLine(string.Format("The project {0} has {1} checklist(s)", api.DefaultProject.name, checklists.Count));
            //            foreach (Checklist checklist in checklists)
            //            {
            //                ListViewItem newItem = new ListViewItem(HttpUtility.UrlDecode(checklist.name)); //Parent item
            //                ListViewItem.ListViewSubItem aSubItem1 = new ListViewItem.ListViewSubItem(newItem, checklist.created_at.ToString()); //Creating subitems for the parent item
            //                ListViewItem.ListViewSubItem aSubItem2 = new ListViewItem.ListViewSubItem(newItem, checklist.checklist_id);
            //                ListViewItem.ListViewSubItem aSubItem3 = new ListViewItem.ListViewSubItem(newItem, checklist.checklist_type);
            //                newItem.SubItems.Add(aSubItem1); //Associating these subitems to the parent item
            //                newItem.SubItems.Add(aSubItem2);
            //                newItem.SubItems.Add(aSubItem3);
            //                lvCheckLists.Items.Add(newItem); //Adding the parent item to the listview control
            //            }



            //        }
            //    }
            //}

            //List<Project> projects = api.getProjects();

            //Console.WriteLine("Project ID\t\t\t\tProject Name");
            //Console.WriteLine("----------\t\t\t\t------------");
            //foreach (Project project in projects)
            //{
            //    Console.WriteLine(string.Format("{0}\t{1}", project.project_id, project.name));
            //}

            //Console.WriteLine("Retrieving list of Checklists for first project\n\n");

            //// Set the default project on the API service object and you won't have to pass it each time you make a call.
            //// Just DON'T forget to change it if you need to interact with other projects!

            //api.DefaultProject = projects[0];

            //        List<Checklist> checklists = api.getChecklists(null, null, 0, 10000); // Defaults to 25
            //        Console.WriteLine(string.Format("The project {0} has {1} checklist(s)", api.DefaultProject.name, checklists.Count));

            //        if (checklists.Count > 0)
            //       {
            //            Console.WriteLine("Checklist ID\t\t\t\tName");
            //            Console.WriteLine("------------\t\t\t\t----");

            //            foreach (Checklist checklist in checklists)
            //            {
            //                Console.WriteLine(string.Format("{0}\t{1}", checklist.id, checklist.name));
            //            }

            //            Console.WriteLine("\n\n");

            //            Checklist firstChecklist = api.getChecklist(new string[] {checklists[0].id})[0];
            //            Console.WriteLine(string.Format("The checklist with ID {0} has {1} sections. Please inspect this object to see what else is available!", firstChecklist.id, firstChecklist.sections.Count));
            //        }

            //        List<Document> docs = api.getDocumentList();
            //    }
            //    //catch (BIM360FieldAPIException ex)
            //    //{
            //    //    Console.WriteLine(string.Format("API service threw an exception: {0} {1}", ex.Code, ex.Message));
            //    //}
            //    //catch (UnauthorizedAccessException ua)
            //    //{
            //    //    Console.WriteLine("Failed to authenticate with the supplied credentials.");
            //    //}

            #endregion
        }

        /// <summary>
        /// Handles the Click event of the cmdGetCompanies control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdGetCompanies_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                List<Company> companies = api.getCompanies(selectedProjectId);

                if (companies != null)
                {

                    //dgvProjectInfo
                    dgvProjectInfo.DataSource = null;

                    dgvProjectInfo.AutoGenerateColumns = false;
                    dgvProjectInfo.ColumnCount = 4;

                    dgvProjectInfo.Columns[0].Name = "Name";
                    dgvProjectInfo.Columns[0].HeaderText = "Name";
                    dgvProjectInfo.Columns[0].DataPropertyName = "name";

                    dgvProjectInfo.Columns[1].Name = "CompanyType";
                    dgvProjectInfo.Columns[1].HeaderText = "Type";
                    dgvProjectInfo.Columns[1].DataPropertyName = "company_type";

                    dgvProjectInfo.Columns[2].Name = "Telephone";
                    dgvProjectInfo.Columns[2].HeaderText = "Telephone";
                    dgvProjectInfo.Columns[2].DataPropertyName = "telephone";

                    dgvProjectInfo.Columns[3].Name = "Url";
                    dgvProjectInfo.Columns[3].HeaderText = "Web";
                    dgvProjectInfo.Columns[3].DataPropertyName = "url";
                    //dgvProjectInfo.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    dgvProjectInfo.DataSource = companies;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdGetIssues control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdGetIssues_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }

            //TODO: Add filters for...
            //area_ids : string - Comma separated list of area IDs. If specified, only retrieve issues in these areas.
            //checklist_template_id : string - A single checklist template ID. If specified, only retrieve issues assigned to checklist items created from this template.

            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                string issue_filter_id = null;
                if (cboIssueFilters.Items.Count > 0)
                {
                    issue_filter_id = cboIssueFilters.SelectedValue.ToString();
                }

                List<Issue> issues = api.getIssueList(null, issue_filter_id, selectedProjectId);

                if (issues != null)
                {
                    dgvIssues.DataSource = null;

                    dgvIssues.AutoGenerateColumns = false;
                    dgvIssues.ColumnCount = 4;

                    dgvIssues.Columns[0].Name = "Issue_Id";
                    dgvIssues.Columns[0].HeaderText = "Issue Id";
                    dgvIssues.Columns[0].DataPropertyName = "issue_id";

                    dgvIssues.Columns[1].Name = "Description";
                    dgvIssues.Columns[1].HeaderText = "Description";
                    dgvIssues.Columns[1].DataPropertyName = "description";

                    dgvIssues.Columns[2].Name = "Status";
                    dgvIssues.Columns[2].HeaderText = "Status";
                    dgvIssues.Columns[2].DataPropertyName = "status";

                    dgvIssues.Columns[3].Name = "CreatedAt";
                    dgvIssues.Columns[3].HeaderText = "Created At";
                    dgvIssues.Columns[3].DefaultCellStyle.Format = "MM/dd/yyyy";
                    dgvIssues.Columns[3].DataPropertyName = "created_at";

                    dgvIssues.DataSource = issues;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdCountIssues control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdCountIssues_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }

            //TODO: Add filters for...
            //area_ids : string - Comma separated list of area IDs. If specified, only retrieve issues in these areas.
            //checklist_template_id : string - A single checklist template ID. If specified, only retrieve issues assigned to checklist items created from this template.

            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                string issue_filter_id = null;
                if (cboIssueFilters.Items.Count > 0)
                {
                    issue_filter_id = cboIssueFilters.SelectedValue.ToString();
                }
                int issueCount = api.getIssueCount(null, issue_filter_id, selectedProjectId);
                MessageBox.Show("The current filter returned (" + issueCount.ToString() + ") issues", "Project Issues", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdGetIssueFilters control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdGetIssueFilters_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                PopulateIssueFilterComboBox(selectedProjectId);
            }
        }

        /// <summary>
        /// Populates the issue filter ComboBox.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        private void PopulateIssueFilterComboBox(string projectId)
        {
            if (!String.IsNullOrEmpty(projectId))
            {
                List<IssueFilter> issueFilters = api.getIssueFilters(projectId);
                if (issueFilters != null)
                {
                    cboIssueFilters.DataSource = issueFilters;
                    cboIssueFilters.DisplayMember = "name";
                    cboIssueFilters.ValueMember = "issue_filter_id";
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdGetTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdGetTemplates_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                List<Template> templates = api.getChecklistTemplates(selectedProjectId);

                if (templates != null)
                {
                    dgvProjectInfo.DataSource = null;

                    dgvProjectInfo.AutoGenerateColumns = false;
                    dgvProjectInfo.ColumnCount = 4;

                    dgvProjectInfo.Columns[0].Name = "Template_Id";
                    dgvProjectInfo.Columns[0].HeaderText = "Template Id";
                    dgvProjectInfo.Columns[0].DataPropertyName = "template_id";

                    dgvProjectInfo.Columns[1].Name = "Name";
                    dgvProjectInfo.Columns[1].HeaderText = "Name";
                    dgvProjectInfo.Columns[1].DataPropertyName = "name";

                    dgvProjectInfo.Columns[2].Name = "ChecklistType";
                    dgvProjectInfo.Columns[2].HeaderText = "Checklist Type";
                    dgvProjectInfo.Columns[2].DataPropertyName = "checklist_type";

                    dgvProjectInfo.Columns[3].Name = "Description";
                    dgvProjectInfo.Columns[3].HeaderText = "Description";
                    dgvProjectInfo.Columns[3].DataPropertyName = "description";

                    dgvProjectInfo.DataSource = templates;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdGetLocations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdGetLocations_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                List<Location> locations = api.getLocations(selectedProjectId);

                if (locations != null)
                {
                    dgvProjectInfo.DataSource = null;

                    dgvProjectInfo.AutoGenerateColumns = false;
                    dgvProjectInfo.ColumnCount = 4;

                    dgvProjectInfo.Columns[0].Name = "Location_Id";
                    dgvProjectInfo.Columns[0].HeaderText = "Location Id";
                    dgvProjectInfo.Columns[0].DataPropertyName = "id";

                    dgvProjectInfo.Columns[1].Name = "Name";
                    dgvProjectInfo.Columns[1].HeaderText = "Name";
                    dgvProjectInfo.Columns[1].DataPropertyName = "name";

                    dgvProjectInfo.Columns[2].Name = "FullPath";
                    dgvProjectInfo.Columns[2].HeaderText = "Full Path";
                    dgvProjectInfo.Columns[2].DataPropertyName = "full_path";

                    dgvProjectInfo.Columns[3].Name = "Path";
                    dgvProjectInfo.Columns[3].HeaderText = "Path";
                    dgvProjectInfo.Columns[3].DataPropertyName = "path";

                    dgvProjectInfo.DataSource = locations;
                }
            }
        }

        private void cmdGetContacts_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                List<Contact> contacts = api.getContacts(selectedProjectId);

                if (contacts != null)
                {
                    dgvProjectInfo.DataSource = null;

                    dgvProjectInfo.AutoGenerateColumns = false;
                    dgvProjectInfo.ColumnCount = 4;

                    dgvProjectInfo.Columns[0].Name = "Contact_Id";
                    dgvProjectInfo.Columns[0].HeaderText = "Contact Id";
                    dgvProjectInfo.Columns[0].DataPropertyName = "id";

                    dgvProjectInfo.Columns[1].Name = "FirstName";
                    dgvProjectInfo.Columns[1].HeaderText = "First Name";
                    dgvProjectInfo.Columns[1].DataPropertyName = "first_name";

                    dgvProjectInfo.Columns[2].Name = "LastName";
                    dgvProjectInfo.Columns[2].HeaderText = "Last Name";
                    dgvProjectInfo.Columns[2].DataPropertyName = "last_name";

                    dgvProjectInfo.Columns[3].Name = "Email";
                    dgvProjectInfo.Columns[3].HeaderText = "Email";
                    dgvProjectInfo.Columns[3].DataPropertyName = "email";

                    dgvProjectInfo.DataSource = contacts;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdGetTasks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdGetTasks_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                MessageBox.Show("Under development", "Work in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                int pageNum = Convert.ToInt16(numPageNumTask.Value);
                int pageRowCount = Convert.ToInt16(numPageRowCountTask.Value);
                List<Checklist> checklists = api.getChecklists(null, selectedProjectId, pageNum, pageRowCount);
                //List<Task> tasks = api.getTasks(selectedProjectId);

                // Task model is not yet defined

                //dgvTasks

            }
        }

        /// <summary>
        /// Handles the Click event of the cmdGetEquipment control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdGetEquipment_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                int pageNum = Convert.ToInt16(numPageNumEquip.Value);
                int pageRowCount = Convert.ToInt16(numPageRowCountEquip.Value);
                List<Equipment> equipments = api.getEquipment(null, selectedProjectId, pageNum, pageRowCount);

                if (equipments != null)
                {
                    dgvEquipment.DataSource = null;

                    dgvEquipment.AutoGenerateColumns = false;
                    dgvEquipment.ColumnCount = 4;

                    dgvEquipment.Columns[0].Name = "Equipment_Id";
                    dgvEquipment.Columns[0].HeaderText = "Equipment Id";
                    dgvEquipment.Columns[0].DataPropertyName = "equipment_id";

                    dgvEquipment.Columns[1].Name = "Name";
                    dgvEquipment.Columns[1].HeaderText = "Name";
                    dgvEquipment.Columns[1].DataPropertyName = "name";

                    dgvEquipment.Columns[2].Name = "TagNumber";
                    dgvEquipment.Columns[2].HeaderText = "Tag Number";
                    dgvEquipment.Columns[2].DataPropertyName = "tag_number";

                    dgvEquipment.Columns[3].Name = "Description";
                    dgvEquipment.Columns[3].HeaderText = "Description";
                    dgvEquipment.Columns[3].DataPropertyName = "description";

                    dgvEquipment.DataSource = equipments;
                }
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the dgvProjects control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void dgvProjects_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProjects.SelectedRows.Count > 0)
            {
                string selectedProjectName = dgvProjects.SelectedRows[0].Cells[0].Value.ToString();
                lblSelectedProject.Text = "Selected Project: " + selectedProjectName;
            }
            else
            {
                lblSelectedProject.Text = "Selected Project: NONE";
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdGetLibrary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void cmdGetLibrary_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txt_auth_token.Text))
            {
                MessageBox.Show("You must login and get an auth_token for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_auth_token.Focus();
                return;
            }

            if (dgvProjects.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select a project for this service call", "BIM 360 API Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_login_name.Focus();
                return;
            }
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                List<Folder> folders = api.getLibraryFolders(selectedProjectId);
                IList<string> folderNames = new List<string>();
                if (folders != null)
                {
                    foreach (Folder folder in folders)
                    {
                        folderNames.Add(folder.path);
                    }
                    char delimeter = Convert.ToChar(@"/");
                    PopulateTreeView(this.treeLibrary, folderNames, delimeter);
                }
            }
        }

        /// <summary>
        /// Populates the TreeView.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="folders">The folders.</param>
        /// <param name="pathSeparator">The path separator.</param>
        private void PopulateTreeView(TreeView treeView, IList<string> folders, char pathSeparator)
        {
            foreach (string folder in folders)
            {
                string[] folderArray = folder.Split(pathSeparator);
                TreeNode lastNode = null;
                string subFolders = String.Empty;
                foreach (string subFolder in folderArray)
                {
                    subFolders += subFolder + pathSeparator;
                    TreeNode[] nodes = treeView.Nodes.Find(subFolders, true);
                    if (nodes.Length == 0)
                    {
                        if (lastNode == null)
                        {
                            lastNode = treeView.Nodes.Add(subFolders, subFolder);
                        }
                        else
                        {
                            lastNode = lastNode.Nodes.Add(subFolders, subFolder);
                        }
                    }
                    else
                    {
                        lastNode = nodes[0];
                    }
                }
            }
        }

        /// <summary>
        /// Handles the NodeMouseClick event of the treeLibrary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TreeNodeMouseClickEventArgs"/> instance containing the event data.</param>
        private void treeLibrary_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string selectedProjectId = dgvProjects.SelectedRows[0].Cells[2].Value.ToString();
            if (!String.IsNullOrEmpty(selectedProjectId))
            {
                lstFiles.Items.Clear();

                TreeNode clickedNode = e.Node;
                char forwardSlash = Convert.ToChar(@"/");
                char backSlash = Convert.ToChar(@"\");

                string folderPath = clickedNode.FullPath;
                folderPath = folderPath.Replace(backSlash, forwardSlash);
                List<Document> documents = api.getFolderDocuments(folderPath, selectedProjectId);
                if (documents != null)
                {
                    foreach (Document document in documents)
                    {
                        lstFiles.Items.Add(document.filename);
                    }
                }
            }
        }



        #region Signature Routines
        //private string GenerateAPISignature(string aTimestamp)
        //{
        //    // To build a signature, create an MD5 has of the following concatenated information (no delimiters):
        //    // API Key
        //    // API Secret
        //    // Unix Epoch Timestamp
        //    //
        //    string baseString = txt_api_key.Text + txt_api_secret.Text + aTimestamp;
        //    string newSig = ComputeMD5Hash(baseString);
        //    return newSig;
        //}

        //public static int GetUNIXEpochTimestamp()
        //{
        //    TimeSpan tSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        //    int timestamp = (int)tSpan.TotalSeconds;
        //    return timestamp;
        //}

        //public string ComputeMD5Hash(string aString)
        //{
        //    // step 1, calculate MD5 hash from aString
        //    MD5 md5 = System.Security.Cryptography.MD5.Create();
        //    byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(aString);
        //    byte[] hash = md5.ComputeHash(inputBytes);

        //    // step 2, convert byte array to hex string
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < hash.Length; i++)
        //    {
        //        sb.Append(hash[i].ToString("x2"));
        //    }
        //    return sb.ToString();
        //}

        #endregion Signature Routines

    }

}
