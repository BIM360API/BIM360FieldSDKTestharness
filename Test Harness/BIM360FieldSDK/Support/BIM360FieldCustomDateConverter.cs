using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;

namespace Autodesk.BIM360Field.APIService.Support
{
    /// <summary>
    /// The dates that come back from BIM360 Field aren't formatted as ISO8601. The format is: 2013-05-23 10:39:25 -0400
    /// </summary>
    public class BIM360FieldCustomDateConverter : DateTimeConverterBase
    {
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return DateTime.Parse(reader.Value.ToString());
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss zzz"));
        }
    }
}
