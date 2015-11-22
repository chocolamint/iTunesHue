using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HueSharp.Lights
{
    /// <summary>
    /// Indicates the color mode in which the light is working.
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ColorMode
    {
        /// <summary>
        /// Hue and saturation.
        /// </summary>
        [EnumMember(Value = "hs")]
        HueSaturation,
        /// <summary>
        /// CIE color space.
        /// </summary>
        [EnumMember(Value = "xy")]
        Xy,
        /// <summary>
        /// Color tempature.
        /// </summary>
        [EnumMember(Value = "ct")]
        ColorTempature
    }
}