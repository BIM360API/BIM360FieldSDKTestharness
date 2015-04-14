using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autodesk.BIM360Field.APIService.Support
{
    public class BIM360FieldAPIException : SystemException
    {
        public string _message;
        public int _code;

        public BIM360FieldAPIException(string message, int code)
        {
            _message = message;
            _code = code;
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public int Code
        {
            get
            {
                return _code;
            }
        }
    }
}
