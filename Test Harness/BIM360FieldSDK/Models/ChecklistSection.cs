using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Models
{
    public class ChecklistSection
    {
        public string section_name { get; set; }
        public List<ChecklistItem> items { get; set; }
    }
}
