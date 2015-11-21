using System.Runtime.Serialization;

namespace HueSharp.Lights
{
    /// <summary>
    /// Details the state of the light.
    /// </summary>
    [DataContract]
    public class LightState
    {
        /// <summary>
        /// Get or set On/Off state of the light. On=<c>true</c>, Off=<c>false</c>.
        /// </summary>
        [DataMember(Name = "on")]
        public bool On { get; set; }
        /// <summary>
        /// Get or set brightness of the light.
        /// This is a scale from the minimum brightness the light is capable of, <c>1</c>, to the maximum capable brightness, <c>254</c>.
        /// </summary>
        [DataMember(Name = "bri")]
        public byte Brightness { get; set; }
        /// <summary>
        /// Get or set hue of the light.
        /// This is a wrapping value between <c>0</c> and <c>65535</c>. Both <c>0</c> and <c>65535</c> are red, <c>25500</c> is green and <c>46920</c> is blue.
        /// </summary>
        [DataMember(Name = "hue")]
        public int Hue { get; set; }
        /// <summary>
        /// Get or set saturation of the light. <c>254</c> is the most saturated (colored) and <c>0</c> is the least saturated (white).
        /// </summary>
        [DataMember(Name = "sat")]
        public byte Saturation { get; set; }
        /// <summary>
        /// Get or set the alert effect, which is a temporary change to the bulbÅfs state.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     The alert effect, which is a temporary change to the bulbÅfs state. This can take one of the following values:
        /// </para>
        /// <list type="term">
        ///     <listheader><see cref="AlertEffect.None"/></listheader>
        ///     <item>
        ///         The light is not performing an alert effect.
        ///     </item>
        ///     <listheader><see cref="AlertEffect.Select"/></listheader>
        ///     <item>
        ///         The light is performing one breathe cycle.
        ///     </item>
        ///     <listheader><see cref="AlertEffect.Lselect"/></listheader>
        ///     <item>
        ///         The light is performing breathe cycles for 15 seconds or until an "alert": "none" command is received.
        ///     </item>
        /// </list>
        /// <para>
        /// 	Note that this contains the last alert sent to the light and not its current state.
        ///     i.e. After the breathe cycle has finished the bridge does not reset the alert to <see cref="AlertEffect.None"/>.
        /// </para>
        /// </remarks>
        [DataMember(Name = "alert")]
        public AlertEffect Alert { get; set; }
        /// <summary>
        /// Get or set the dynamic effect of the light, can either be <see cref="LightEffect.None"/> or <see cref="LightEffect.Colorloop"/>.
        /// </summary>
        [DataMember(Name = "effect")]
        public LightEffect Effect { get; set; }
        /// <summary>
        /// Get or set the x and y coordinates of a color in CIE color space.
        /// </summary>
        [DataMember(Name = "xy")]
        public Xy Xy { get; set; }
        /// <summary>
        /// Get or set the Mired Color temperature of the light.
        /// 2012 connected lights are capable of <c>153</c> (6500K) to <c>500</c> (2000K).
        /// </summary>
        [DataMember(Name = "ct")]
        public int ColorTemperture { get; set; }
        /// <summary>
        /// Get or set the color mode in which the light is working, this is the last command type it received.
        /// Values are ÅghsÅh for Hue and Saturation, ÅgxyÅh for XY and ÅgctÅh for Color Temperature.
        /// This parameter is only present when the light supports at least one of the values.
        /// </summary>
        [DataMember(Name = "colormode")]
        public ColorMode ColorMode { get; set; }
        /// <summary>
        /// Get or set if a light can be reached by the bridge.
        /// </summary>
        [DataMember(Name = "reachable")]
        public bool Reachable { get; set; }
    }
}