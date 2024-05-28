using Newtonsoft.Json;
using Sitecore.Diagnostics;
using Sitecore.XmCloud.Common.Abstractions;
using Sitecore.XmCloud.Common.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sitecore.XmCloud.Common.Services
{
    public class SitecoreSupportSiteRegistrationService : ISiteRegistrationService
    {
        private readonly HttpClient _httpClient;
        private readonly ClientSettings _clientSettings;

        public SitecoreSupportSiteRegistrationService(IClientSettingsFactory clientSettingsFactory)
        {
            _clientSettings = clientSettingsFactory.CreateClientSettings();
            _httpClient = new HttpClient(
                new AuthTokenMessageHandler(_clientSettings.ClientId,
                    _clientSettings.ClientSecret,
                    _clientSettings.AuthUrl,
                    _clientSettings.Audience));
        }


        public async Task RegisterSite(SiteRegistrationInfo siteRegistrationInfo)
        {
            Assert.ArgumentNotNull(siteRegistrationInfo, "siteRegistrationInfo");

            if(string.IsNullOrEmpty(siteRegistrationInfo.SiteName) || siteRegistrationInfo.SiteName.Equals("$name", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(siteRegistrationInfo.EnvironmentId))
                {
                    Log.Warn("[SiteRegistrationService] Failed to obtain environment id.", this);
                    return;
                }

                Log.Debug($"[SiteRegistrationService] Sending request: {JsonConvert.SerializeObject(siteRegistrationInfo)}", this);
                var url = new Uri(new Uri(_clientSettings.ApiBaseUrl), $"{Constants.SitesEndpoint}/{siteRegistrationInfo.EnvironmentId}");  
                var response = await _httpClient.PutAsJsonAsync(url, siteRegistrationInfo).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Log.Error($"[SitesRegistrationService] Failed to register site '{siteRegistrationInfo.SiteName}': {ex.Message}", ex, this);
            }
        }

        public async Task UnregisterSite(SiteRegistrationInfo siteRegistrationInfo)
        {
            Assert.ArgumentNotNull(siteRegistrationInfo, "siteRegistrationInfo");
            Log.Info($"[Sitecore.Support.Diagnostics.CS0444820]: Test", this);
            try
            {
                if(string.IsNullOrEmpty(siteRegistrationInfo.EnvironmentId))
                {
                    Log.Warn("[SiteRegistrationService] Failed to obtain environment id.", this);
                    return;
                }

                var url = new Uri(new Uri(_clientSettings.ApiBaseUrl), $"{Constants.SitesEndpoint}/{siteRegistrationInfo.EnvironmentId}/{siteRegistrationInfo.SiteName}");
                Log.Info($"[Sitecore.Support.Diagnostics.CS0444820]: URL: {url.ToString()}", this);
                Log.Info($"[Sitecore.Support.Diagnostics.CS0444820]: _clientSettings.ApiBaseUrl: {_clientSettings.ApiBaseUrl}", this);
                Log.Info($"[Sitecore.Support.Diagnostics.CS0444820]: siteRegistrationInfo.EnvironmentId: {siteRegistrationInfo.EnvironmentId}", this);
                Log.Info($"[Sitecore.Support.Diagnostics.CS0444820]: siteRegistrationInfo.SiteName: {siteRegistrationInfo.SiteName}", this);
                var response = await _httpClient.DeleteAsync(url).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Log.Error($"[SitesRegistrationService] Failed to unregister site '{siteRegistrationInfo.SiteName}': {ex.Message}", ex, this);
                Log.Info($"[Sitecore.Support.Diagnostics.CS0444820]: CallStack: {Environment.StackTrace}", this);
            }
        }
    }
}
