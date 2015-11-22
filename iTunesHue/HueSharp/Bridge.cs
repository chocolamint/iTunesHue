using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HueSharp.Lights;

namespace HueSharp
{
    /// <summary>
    /// Indicates the Hue bridge.
    /// </summary>
    public class Bridge
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _apiRoot;

        /// <summary>
        /// Get or set authorized user.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Get bridge ip address.
        /// </summary>
        public IPAddress IPAddress { get; }

        static Bridge()
        {
            Mapper.CreateMap<Light, Light>();
            Mapper.CreateMap<Xy, Xy>();
            Mapper.CreateMap<LightState, LightState>();
        }

        /// <summary>
        /// Initialize new instance of <see cref="Bridge"/> class with bridge ip address.
        /// </summary>
        /// <param name="ipAddress">Bridge ip address.</param>
        public Bridge(IPAddress ipAddress)
        {
            IPAddress = ipAddress;
            _apiRoot = $"http://{IPAddress}/api";
        }
        /// <summary>
        /// Initialize new instance of <see cref="Bridge"/> class with bridge ip address.
        /// </summary>
        /// <param name="ipAddress">Bridge ip address.</param>
        public Bridge(string ipAddress)
            : this(IPAddress.Parse(ipAddress))
        {
        }

        /// <summary>
        /// Authorize specified device type, and set the <see cref="UserName"/> property.
        /// </summary>
        /// <param name="deviceType">Device type you want to authorize.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>It is <see cref="Task"/> that will be completed when the button pressed on the bridge.</returns>
        public async Task AuthorizeAsync(string deviceType, CancellationToken cancellationToken)
        {
            while (true)
            {
                var content = JsonContent.Create(new
                {
                    devicetype = deviceType
                });

                var response = await _client.PostAsync(_apiRoot, content, cancellationToken).ConfigureAwait(false);
                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var results = (dynamic)JsonConvert.DeserializeObject(body);
                var result = results[0];

                if (result.success != null)
                {
                    UserName = result.success.username;
                    return;
                }

                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// Gets a list of all lights that have been discovered by the bridge.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>List of all lights that have been discovered by the bridge.</returns>
        public async Task<LightCollection> GetLightsAsync(CancellationToken cancellationToken)
        {
            var lights = await GetAsync<LightCollection>("lights", cancellationToken).ConfigureAwait(false);
            foreach (var light in lights)
            {
                light.SetStateCallback = SetLightStateAsync;
                light.GetStateCallback = GetLightStateAsync;
            }
            return lights;
        }

        internal async Task SetLightStateAsync(string id, object state)
        {
            object api = $"lights/{id}/state";
            var content = JsonContent.Create(state);
            var response = await _client.PutAsync($"{_apiRoot}/{UserName}/{api}", content);
        }

        internal async Task GetLightStateAsync(Light light)
        {
            //TODO
            var newLight = await GetAsync<Light>($"lights/{light.Id}", CancellationToken.None);
            light.State.Xy = newLight.State.Xy;
            light.State.Brightness = newLight.State.Brightness;
        } 

        private async Task<T> GetAsync<T>(string api, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync($"{_apiRoot}/{UserName}/{api}", cancellationToken).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
