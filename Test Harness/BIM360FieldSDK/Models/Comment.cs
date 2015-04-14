using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autodesk.BIM360Field.APIService.Support;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Comment
    {
        public string id { get; set; }
        public string comment_text { get; set; }
        public string created_by { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime created_at { get; set; }
    }
}
