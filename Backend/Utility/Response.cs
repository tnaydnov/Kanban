using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    ///<summary>Class <c>Response</c> represents the result of a call to a void function. 
    ///If an exception was thrown, <c>ErrorOccured = true</c> and <c>ErrorMessage != null</c>. 
    ///Otherwise, <c>ErrorOccured = false</c> and <c>ErrorMessage = null</c>.</summary>
    public class Response
    {
        public string ErrorMessage { get; } 
        public object ReturnValue { get; }


        public Response(object val)
        {
            ReturnValue = val;
        }

        public Response(string msg, bool err)
        {
            if(err)
                ErrorMessage = msg;
        }

        public Response() { }


        public bool ErrorOccured()
        {
            return ErrorMessage != null;
        }

        
    }
}
