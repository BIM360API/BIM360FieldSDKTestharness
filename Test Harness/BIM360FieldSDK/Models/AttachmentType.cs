using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class AttachmentType
    {
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string original_url { get; set; }
        public string thumb_url { get; set; }
        public int num_pages { get; set; }
        public string content_type { get; set; }
        public List<Composite> composites { get; set; }
    }
}
