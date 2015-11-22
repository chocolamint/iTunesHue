using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using HueSharp.Converters;
using Newtonsoft.Json;

namespace HueSharp.Lights
{
    /// <summary>
    /// Represents a collection of hue lights that can be accessed light identifier.
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(LightCollectionConverter))]
    public class LightCollection : KeyedCollection<string, Light>
    {
        protected override string GetKeyForItem(Light item)
        {
            return item.Id;
        }
    }
}