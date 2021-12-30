using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SupaMVCSample.Models;
using System.Security.Claims;
using WebSocketSharp;

namespace SupaMVCSample.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _config;
        private Supabase.Client _client;

        public AccountController(ILogger<AccountController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignInAsync(SignInWithEmailAndPassword signInUser)
        {
            if(_client == null)
            {
                await Supabase.Client.InitializeAsync(_config["Supabase:ApiUrl"], _config["Supabase:SystemApiKey"]);
                _client = Supabase.Client.Instance;
            }

            if(_client.Auth.CurrentSession == null)
            {
                var userSession = await _client.Auth.SignIn(signInUser.Email, signInUser.Password);

                if(userSession != null)
                {
                    //Claims Example
                    var claims = new List<Claim>();

                    foreach (var data in userSession.User.UserMetadata)
                    {
                            claims.Add(new Claim(data.Key, data.Value.ToString()));
                    }

                    claims.Add(new Claim("email", signInUser.Email));

                    //this can also either be from database request for data stored on a profile table or data added to the auth schema
                    claims.Add(new Claim("name", userSession.User.UserMetadata["DisplayName"].ToString()));

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role")),
                    new AuthenticationProperties
                    {
                        AllowRefresh = true, //allow the cookie to be refreshed similar as supabase session
                        IssuedUtc = new DateTimeOffset(DateTime.Now),
                    });
                }
            }
            else
            {
                //session already exist for the user
                await _client.Auth.RefreshSession();
            }

            if (string.IsNullOrEmpty(signInUser.RedirectUrl) || signInUser.RedirectUrl == "/")
            {
                string url = Url.ActionLink("UserAuthenticationDetails", "Home");
                return Ok(url);
            }
            else
            {
                //TODO: should validate the url that was passed in when the login page was routed
                //example: https://stackoverflow.com/questions/7578857/how-to-check-whether-a-string-is-a-valid-http-url
                Uri uriResult;
                bool result = Uri.TryCreate(signInUser.RedirectUrl, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;
                if (result)
                {
                    return Ok(signInUser.RedirectUrl);
                }
                else
                {
                    return BadRequest("Unable to redirect. Invalid Url.");
                }

            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpAsync(SignUpWithEmailAndPassword newUser)
        {
            await Supabase.Client.InitializeAsync(_config["Supabase:ApiUrl"], _config["Supabase:SystemApiKey"]);
            _client = Supabase.Client.Instance;

            //Example saving user metadata against auth schema
            Dictionary<string, object> userMetadata = new Dictionary<string, object>
            {
                { "DisplayName", newUser.DisplayName },
                { "UserName", newUser.UserName },
                { "IsActive", newUser.IsActive }
            };

            //TODO: Pending library update for User and Session returns
            var userCreated = await _client.Auth.SignUp(newUser.Email, newUser.Password);
            
            //TODO: Perform any database request


            return NoContent();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult EmailConfirmation()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
