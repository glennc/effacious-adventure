using System;
using System.Collections.Generic;

namespace JsonConfigParser
{
    public class JsonObject : JsonType
    {
        public Dictionary<string, JsonType> Items {get;set;}

        public JsonObject()
        {
            Items = new Dictionary<string, JsonType>();
        }

        public void AddJsonType(string key, JsonType type)
        {
            Items.Add(key, type);
        }
    }
}