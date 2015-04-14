using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autodesk.BIM360Field.APIService.Support;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Document
    {
        public string document_id { get; set; }
        public string filename { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime updated_at { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime created_at { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime fmod_date { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime fcreate_date { get; set; }
        public int? num_pages { get; set; }
        public string caption { get; set; }
        public int? revision_position { get; set; }
        public int? revision_count { get; set; }
        public string tags { get; set; }
        public long? size { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public string content_type { get; set; }
        public string path { get; set; }
    }
}
