namespace HueSharp.Lights
{
    /// <summary>
    /// The alert effect, which is a temporary change to the bulbÅfs state.
    /// </summary>
    public enum AlertEffect
    {
        /// <summary>
        /// The light is not performing an alert effect.
        /// </summary>
        None,
        /// <summary>
        /// The light is performing one breathe cycle.
        /// </summary>
        Select,
        /// <summary>
        /// The light is performing breathe cycles for 15 seconds or until an "alert": "none" command is received.
        /// </summary>
        Lselect
    }
}