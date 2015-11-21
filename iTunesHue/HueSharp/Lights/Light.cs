using System.Runtime.Serialization;

namespace HueSharp.Lights
{
    /// <summary>
    /// Represents a hue light.
    /// </summary>
    [DataContract]
    public class Light
    {
        /// <summary>
        /// Get Identifier of light.
        /// </summary>
        public int Id { get; internal set; }
        /// <summary>
        /// Get details the state of the light.
        /// </summary>
        [DataMember(Name = "state")]
        public LightState State { get; internal set; }
        /// <summary>
        /// Get a fixed name describing the type of light e.g. “Extended color light”.
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; internal set; }
        /// <summary>
        /// Get a unique, editable name given to the light.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; internal set; }
        /// <summary>
        /// Get the hardware model of the light.
        /// </summary>
        [DataMember(Name = "modelid")]
        public string ModelId { get; internal set; }
        /// <summary>
        /// Get the manufacture name.
        /// </summary>
        [DataMember(Name = "manufacturename")]
        public string ManufactureName { get; internal set; }
        /// <summary>
        /// Get the unique id of the device.
        /// The MAC address of the device with a unique endpoint id in the form: AA:BB:CC:DD:EE:FF:00:11-XX.
        /// </summary>
        [DataMember(Name = "uniqueid")]
        public string UniqueId { get; internal set; }
        /// <summary>
        /// Get an identifier for the software version running on the light.
        /// </summary>
        [DataMember(Name = "swversion")]
        public string SoftwareVersion { get; internal set; }
    }
}