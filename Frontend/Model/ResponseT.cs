using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    internal class ResponseT<T>
    {
        public T ReturnValue;
        public string ErrorMessage;

        [Newtonsoft.Json.JsonConstructor]
        public ResponseT(T ret, string err) { 
        
            ReturnValue = ret;
            ErrorMessage = err;
        }


        public bool ErrorOccured()
        {
            return ErrorMessage != null;
        }
    }
}
