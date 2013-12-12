using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApp.Exceptions
{
    [Serializable()]
    public class InvalidDistrictException : System.Exception
    {
        public InvalidDistrictException() : base() { }
        public InvalidDistrictException(string message) : base(message) { }
        public InvalidDistrictException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an 
        // exception propagates from a remoting server to the client.  
        protected InvalidDistrictException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}