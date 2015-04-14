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
    public class EquipmentTest
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
        public void TestGetEquipment()
        {
            List<Equipment> equipment = _api.getEquipment();
            Assert.IsNotNull(equipment);

        }
    }
}
