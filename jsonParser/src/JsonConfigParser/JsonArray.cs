
using System.Collections.Generic;

namespace JsonConfigParser
{
    public class JsonArray : JsonType
    {
        public List<JsonType> Values {get;set;}

        public JsonArray()
        {
            Values = new List<JsonType>();
        }
    }
}