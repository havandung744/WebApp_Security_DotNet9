using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebApp.Authorization;
using WebApp.DTO;

namespace WebApp.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public List<WeatherForecastDto> weatherForecastItems { get; set; } = new List<WeatherForecastDto>();


        public async Task OnGetAsync()
        {
            JwtToken token = new JwtToken();
            var strTokenObj = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(strTokenObj))
            {
                token = await Authenticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj) ?? new JwtToken();
            }
            if (token == null ||
                string.IsNullOrWhiteSpace(token.AccessToken) ||
                token.ExpiresAt <= DateTime.Now)
            {
                token = await Authenticate();
            }
            var httpClient = httpClientFactory.CreateClient("OurWebAPI");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast/GetWeatherForecast") ?? new List<WeatherForecastDto>();
        }

        private async Task<JwtToken> Authenticate()
        {
            var httpClient = httpClientFactory.CreateClient("OurWebAPI");
            var res = await httpClient.PostAsJsonAsync("auth/Authenticate", new Credential { UserName = "admin", Password = "password" });
            res.EnsureSuccessStatusCode();
            string strJwt = await res.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", strJwt);
            return JsonConvert.DeserializeObject<JwtToken>(strJwt) ?? new JwtToken();
        }
    }
}
