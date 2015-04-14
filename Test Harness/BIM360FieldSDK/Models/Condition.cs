using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class Condition
    {
        public string identifier { get; set; }
        public string sort_direction { get; set; }
        public bool sort_field { get; set; }
        public string operation { get; set; }
        public string values { get; set; }
    }
}
