using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Autodesk.BIM360Field.APIService.Support;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class ModelBase
    {
        public ModelBase()
        {
            this.id = Guid.NewGuid().ToString();
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
        }

        public string id { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime created_at { get; set; }
        [JsonConverter(typeof(BIM360FieldCustomDateConverter))]
        public DateTime updated_at { get; set; }
    }
}
