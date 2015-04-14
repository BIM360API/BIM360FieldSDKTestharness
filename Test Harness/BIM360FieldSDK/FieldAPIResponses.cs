// Copyright 2012 Autodesk, Inc.  All rights reserved.
// Use of this software is subject to the terms of the Autodesk license agreement 
// provided at the time of installation or download, or which otherwise accompanies 
// this software in either electronic or hard copy form.   

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService
{
    //==================================================
    // Field Responses Here... Work in progess - MV
    // Not used by the sample currently.
    //==================================================
    [Serializable]
    public class security_login_response_v1
    {
        public string auth_token { get; set; }
        public string user_id { get; set; }
    }

    [Serializable]
    public class field_project_info_response_v1
    {
        // This is the Project Information
        public string id { get; set; }
        public string name { get; set; }
        public string a360_project_id { get; set; }
    }

    [Serializable]
    public class field_project_list_response_v1
    {
        // List of projects
        public field_project_info_response_v1[] project_list;
    }

    [Serializable]
    public class field_property
    {
        public string type { get; set; }
        public string value { get; set; }
        public string name { get; set; }
    }

    [Serializable]
    public class field_attachment
    {
        public string link { get; set; }
        public string name { get; set; }
    }

    [Serializable]
    public class field_properties_response_v1
    {
        public field_property[] properties { get; set; }
        public field_attachment[] attachments { get; set; }
        public string model_location { get; set; }
        public string[] model_locations { get; set; }
        public string equipment_b3f_id { get; set; }
        public string equipment_name { get; set; }
        public string bim_object_identifier { get; set; }
        public string item_guid { get; set; }
        public string equipment_updated_at { get; set; }
    }


}
