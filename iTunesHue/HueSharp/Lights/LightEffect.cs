using System.Runtime.Serialization;

namespace HueSharp.Lights
{
    /// <summary>
    /// The dynamic effect of the light, can either be “none” or “colorloop”.
    /// </summary>
    [DataContract]
    public enum LightEffect
    {
        /// <summary>
        /// No effect.
        /// </summary>
        [DataMember(Name = "none")]
        None,
        /// <summary>
        /// The light will cycle through all hues using the current brightness and saturation settings.
        /// </summary>
        [DataMember(Name = "colorloop")]
        Colorloop
    }
}