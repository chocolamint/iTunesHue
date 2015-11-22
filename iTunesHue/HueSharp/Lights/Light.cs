using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace HueSharp.Lights
{
    /// <summary>
    /// Represents a hue light.
    /// </summary>
    [DataContract]
    [DebuggerDisplay(@"\{ Light: {Id} - {Name} \}")]
    public class Light
    {
        internal Func<string, object, Task> SetStateCallback { get; set; }
        internal Func<Light, Task> GetStateCallback { get; set; }

        internal Light()
        {
            
        }

        /// <summary>
        /// Get Identifier of light.
        /// </summary>
        public string Id { get; internal set; }
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


        public Task SetXyAsync(Xy xy)
        {
            return SetStateCallback(Id, new {xy});
        }
        public Task SetXyAsync(float x, float y)
        {
            return SetXyAsync(new Xy(x, y));
        }

        public Task ReloadStateAsync()
        {
            return GetStateCallback(this);
        }
    }
}