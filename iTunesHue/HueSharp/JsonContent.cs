using System.Net.Http;
using Newtonsoft.Json;

namespace HueSharp
{
    internal class JsonContent : StringContent
    {
        private JsonContent(object obj)
            : base(JsonConvert.SerializeObject(obj))
        {

        }

        public static JsonContent Create(object obj)
        {
            return new JsonContent(obj);
        }
    }
}