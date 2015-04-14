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

namespace APITest
{
    [TestClass]
    public class ExtrasTest
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
        public void TestGetLocations()
        {
            List<Location> locations = _api.getLocations();
            Assert.IsNotNull(locations);
            Assert.IsTrue(locations.Count == 4);

            Assert.AreEqual("Building 1", locations[0].name);
            Assert.AreEqual("", locations[0].path);
            Assert.AreEqual("Building 1", locations[0].full_path);
            Assert.IsTrue(locations[0].id != "");

            Assert.AreEqual("Floor 1", locations[1].name);
            Assert.AreEqual("Building 1", locations[1].path);
            Assert.AreEqual("Building 1>Floor 1", locations[1].full_path);
            Assert.IsTrue(locations[1].id != "");

            Assert.AreEqual("Room 101", locations[2].name);
            Assert.AreEqual("Building 1>Floor 1", locations[2].path);
            Assert.AreEqual("Building 1>Floor 1>Room 101", locations[2].full_path);
            Assert.IsTrue(locations[2].id != "");

            Assert.AreEqual("Building 2", locations[3].name);
            Assert.AreEqual("", locations[3].path);
            Assert.AreEqual("Building 2", locations[3].full_path);
            Assert.IsTrue(locations[3].id != "");
        }

        [TestMethod]
        public void TestGetCompanies()
        {
            List<Company> companies = _api.getCompanies();
            Assert.IsNotNull(companies);
            Assert.IsTrue(companies.Count == 2);

            Assert.AreEqual("API Test Account", companies[0].name);
            Assert.AreEqual("Acme Drywall", companies[1].name);
        }

        [TestMethod]
        public void TestGetUsers()
        {
            List<User> users = _api.getUsers();
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count == 2);
            Assert.AreEqual("xxx@autodesk.com.com", users[0].email);
            Assert.AreEqual("xxx@gmail.com", users[1].email);
        }
    }
}
