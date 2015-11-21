using System.Runtime.Serialization;

namespace HueSharp.Lights
{
    /// <summary>
    /// Indicates the color mode in which the light is working.
    /// </summary>
    [DataContract]
    public enum ColorMode
    {
        /// <summary>
        /// Hue and saturation.
        /// </summary>
        [DataMember(Name = "hs")]
        HueSaturation,
        /// <summary>
        /// CIE color space.
        /// </summary>
        [DataMember(Name = "xy")]
        Xy,
        /// <summary>
        /// Color tempature.
        /// </summary>
        [DataMember(Name = "ct")]
        ColorTempature
    }
}