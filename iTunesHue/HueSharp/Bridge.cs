using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HueSharp.Converters;
using HueSharp.Lights;
using Newtonsoft.Json.Serialization;

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
                var json = JsonContent.Create(new
                {
                    devicetype = deviceType
                });

                var response = await _client.PostAsync(_apiRoot, json, cancellationToken).ConfigureAwait(false);
                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var jsonResults = (dynamic)JsonConvert.DeserializeObject(body);
                var jsonResult = jsonResults[0];

                if (jsonResult.success != null)
                {
                    UserName = jsonResult.success.username;
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
            var response = await _client.GetAsync($"{_apiRoot}/{UserName}/lights", cancellationToken).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<LightCollection>(json);
        }
    }
}
