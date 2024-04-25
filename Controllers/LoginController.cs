using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;

namespace OpenIdClient.Controllers
{
    [ApiController]
    [Route("")]
    public class LoginController : Controller
    {

        //private string _codeVerifier;
        //private string _state;


    

        [HttpGet]
        public async Task<IActionResult> Home()
        {
            return View();
        }





        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {

            Config.codeVerifier = SecureValueGenerator.GenerateRandomString(22);
            Config.state = SecureValueGenerator.GenerateRandomString(33);


            //_state = "97tvtZHsHTV4I5parGxBJ-sRF5Lml_JGmb21VXwtoaE";
            //_codeVerifier = "es1kPi2mRaxvo4Y3cb8gRFRmYrpJzyO9FelyjMrgy0w";




            string keyCloakUrl = Config.keyCloakUrl;
            string clientId = Config.clientId;
            string callback = "https://localhost:7082/callback";



            var parameters = new Dictionary<string, string?>
            {
                { "client_id", clientId },
                { "scope", "openid email phone address profile" },
                { "response_type", "code" },
                { "redirect_uri", callback },
                { "prompt", "login" },
                { "state", Config.state },
                { "code_challenge_method", "plain" },
                { "code_challenge", Config.codeVerifier }
            };
            var authorizationUri = QueryHelpers.AddQueryString(keyCloakUrl, parameters);


            // Redirects to Keycloak
            return Redirect(authorizationUri);
        }





        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string state, string session_state, string code)
        {
            var parameters = new Dictionary<string, string?>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri",  "https://localhost:7082/callback"},
                { "code_verifier", Config.codeVerifier },
                { "client_id", Config.clientId },
                { "client_secret", Config.clientSecret }
            };

            var response = await new HttpClient().PostAsync(Config.tokenEndpoint, new FormUrlEncodedContent(parameters));

            TokenResponse tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();




            return Redirect($"https://localhost:7082/User?accessToken={tokenResponse.access_token}");
        }




        [HttpGet("User")]
        public async Task<IActionResult> User(string accessToken)
        {
            var http = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    { "Authorization", "Bearer " + accessToken }
                }
            };

            var response = await http.GetAsync(Config.userInfoEndpoint);
            var content = await response.Content.ReadFromJsonAsync<object?>();

            var jsonObject = (JsonElement)content;
            string username = "";
            string emailVerified = "";

            if (content != null)
            {
                if (jsonObject.TryGetProperty("preferred_username", out var preferredUsernameProperty) && preferredUsernameProperty.ValueKind == JsonValueKind.String)
                    username = preferredUsernameProperty.GetString();

                if (jsonObject.TryGetProperty("email_verified", out var emailVerifiedProperty) && emailVerifiedProperty.ValueKind == JsonValueKind.String)
                    emailVerified = emailVerifiedProperty.GetString();
            }


            var viewModel = new UserInfoViewModel
            {
                PreferredUsername = username,
                EmailVerified = emailVerified
            };

            return View(viewModel);
        }
        




    }
}
