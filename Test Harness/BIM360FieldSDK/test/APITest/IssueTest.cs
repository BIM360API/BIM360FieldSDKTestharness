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
using System.Reflection;

namespace APITest
{
    [TestClass]
    public class IssueTest
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
        public void TestIssueTypes()
        {
            List<IssueType> issueTypes = _api.getIssueTypes();
            Assert.IsNotNull(issueTypes);
            Assert.IsTrue(issueTypes.Count == 24);
        }

        [TestMethod]
        public void TestCreateIssueType()
        {
            List<IssueType> oldTypes = _api.getIssueTypes();
            IssueType oldType = oldTypes[0];

            IssueType newType = _api.createIssueType("Safety", "API Test");
            Assert.IsNotNull(newType);
            Assert.IsTrue(newType.category_name == "Safety");
            Assert.IsTrue(newType.name == "API Test");
            Assert.IsTrue(newType.id != "");

            _api.destroyIssueType(newType.id, oldType.id); 
        }

        [TestMethod]
        public void TestGetIssueList()
        {
            List<Issue> issues = _api.getIssueList();
            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count == 4);
        }

        [TestMethod]
        public void TestGetLocations()
        {
            List<Location> locs = _api.getLocations();
            Assert.IsNotNull(locs);
            Assert.IsTrue(locs.Count == 4);
        }

        [TestMethod]
        public void TestGetSingleIssue()
        {
            List<Issue> issues = _api.getIssueList();
            Issue issue = _api.getIssue(issues[0].id);
        }

        [TestMethod]
        public void TestAddIssueAttachment()
        {
            FileInfo original = new FileInfo("C_12.pdf");
            FileInfo thumb = new FileInfo("C_12_thumb.png");
            _api.createAttachment(original, thumb, "37669f00-d084-11e2-862f-0a5e82e5854e", "Issue", "This is a caption", "tag1, tag2, tag3");
        }
    }
}
