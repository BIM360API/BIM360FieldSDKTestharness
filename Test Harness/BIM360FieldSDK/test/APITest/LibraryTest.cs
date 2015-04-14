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
    public class LibraryTest
    {
        static API _api = new API("https://manage.velasystems.com");

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
        public void TestFetchDirectories()
        {
           List<Folder> folders = _api.getLibraryFolders();
           Assert.IsNotNull(folders);
           Assert.IsTrue(folders.Count == 4);
           Assert.AreEqual("BIM360Field/Documents", folders[0].path);
           Assert.AreEqual("BIM360Field/Images", folders[1].path);
           Assert.AreEqual("BIM360Field/Models", folders[2].path);
           Assert.AreEqual("BIM360Field/Plans", folders[3].path);
        }

        [TestMethod]
        public void TestFetchFolderDocuments()
        {
            List<Document> docs = _api.getFolderDocuments("BIM360Field/Images");
            Assert.IsNotNull(docs);
            Assert.IsTrue(docs.Count == 46);
            Assert.AreEqual("image/jpeg", docs[0].content_type);
            Assert.AreEqual("BIM360Field/Images", docs[0].path);
            Assert.AreEqual(1, docs[0].revision_count);
            Assert.AreEqual(0, docs[0].revision_position);
        }

        [TestMethod]
        public void TestFetchFile()
        {
            List<Document> docs = _api.getFolderDocuments("BIM360Field/Images");
            Assert.IsNotNull(docs);
            Assert.IsTrue(docs.Count == 46);

            Document doc = docs[0];
            byte[] firstDoc = _api.getBinaryFile("Document", doc.document_id);
            Assert.IsNotNull(firstDoc);
            Assert.IsTrue(firstDoc.Length == doc.size);
        }

        [TestMethod]
        public void TestPublishAndDelete()
        {
        }

        [TestMethod]
        public void TestRevisionDocument()
        {
        }
    }
}
