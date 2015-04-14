// Copyright 2012 Autodesk, Inc.  All rights reserved.
// Use of this software is subject to the terms of the Autodesk license agreement 
// provided at the time of installation or download, or which otherwise accompanies 
// this software in either electronic or hard copy form.   

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autodesk.BIM360Field.APIService;
using Autodesk.BIM360Field.APIService.Models;
using System.IO;

namespace APITest
{
    [TestClass]
    public class ChecklistTest
    {
        static API _api = new API("https://bim360field.autodesk.com");

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            _api.authenticate("", "");
            List<Project> projects = _api.getProjects();

            Assert.IsNotNull(projects, "No projects retrieved!");
            Assert.IsTrue(projects.Count == 1);
            Assert.IsTrue(projects[0].name == "API Project");

            _api.DefaultProject = projects[0];
        }

        [TestMethod]
        public void GetChecklists()
        {
            List<Checklist> checklists = _api.getChecklists();
            Assert.IsNotNull(checklists);
            Assert.IsTrue(checklists.Count > 0);
        }

        [TestMethod]
        public void GetChecklistDetails()
        {
            List<Checklist> checklists = _api.getChecklists();
            Assert.IsNotNull(checklists);

            List<Checklist> checklist = _api.getChecklist(new string[] {checklists[0].id});
            Assert.IsNotNull(checklist);
            Assert.IsTrue(checklist.Count == 1);
        }

        [TestMethod]
        public void GetTemplates()
        {
            List<Template> templates = _api.getChecklistTemplates();
            Assert.IsNotNull(templates);
            Assert.IsTrue(templates.Count > 0);
        }

        [TestMethod]
        public void TestCreateChecklist()
        {
            List<Template> templates = _api.getChecklistTemplates();
            Assert.IsNotNull(templates);

            string checklist_id = _api.createChecklist(templates[0]);
            Assert.IsNotNull(checklist_id);
        }

        [TestMethod]
        public void TestUpdateChecklistStatus()
        {
            List<Template> templates = _api.getChecklistTemplates();
            Assert.IsNotNull(templates);

            string checklist_id = _api.createChecklist(templates[0]);
            Assert.IsNotNull(checklist_id);

            Checklist checklist = _api.getChecklist(new string[] {checklist_id})[0];
            Assert.IsNotNull(checklist);
            Assert.IsTrue(checklist.status == "Open");
            Assert.IsTrue(checklist.checklist_items.Count == 10);
            checklist.status = "Closed";

            _api.updateChecklist(checklist);

            Checklist newChecklist = _api.getChecklist(new string[] {checklist.id})[0];
            Assert.IsTrue(newChecklist.status == "Closed");

            // Change the source to a piece of equipment
            newChecklist.source_type = "EquipmentType";
            newChecklist.source_id = "791ef506-82a3-11e3-bebe-0a5e82d4264f";
            _api.updateChecklist(newChecklist);

            Checklist equipmentChecklist = _api.getChecklist(new string[] { newChecklist.id })[0];
            Assert.AreEqual(equipmentChecklist.source_type, "EquipmentType");
            Assert.AreEqual(equipmentChecklist.source_id, "791ef506-82a3-11e3-bebe-0a5e82d4264f");

            // Update some responses
            equipmentChecklist.checklist_items[1].response = "-";
            equipmentChecklist.checklist_items[2].response = "True";
            equipmentChecklist.checklist_items[3].response = "N/A";
            equipmentChecklist.checklist_items[4].response = "Yes";
            equipmentChecklist.checklist_items[5].response = "This is a text response";
            equipmentChecklist.checklist_items[5].comment = "This is a comment for the text response";

            // Update the custom field values
            equipmentChecklist.custom_field_values[0].value = "I Was Here!";
            equipmentChecklist.custom_field_values[1].value = "But, now he's gone!";

            // Attach a PDF to checklist item #1
            FileInfo original = new FileInfo("C_12.pdf");
            FileInfo thumb = new FileInfo("C_12_thumb.png");
            _api.createAttachment(original, thumb, equipmentChecklist.checklist_items[1].id, "CompletedChecklistItem", "This is for checklist item #1", "tag1, tag2, tag3");

            _api.updateChecklist(equipmentChecklist);
        }
    }
}
