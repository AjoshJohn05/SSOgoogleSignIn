using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestSingleSignOn.Models;

namespace TestSingleSignOn.Controllers
{
   
    public class GoogleSignInController : Controller
    {
        private readonly IConfiguration _configuration;
        public GoogleSignInController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
      
        public IActionResult SignInPage()
        {
            return View();
        }
        public RedirectResult VerifyUser()
        {
            var clientId = _configuration["GoogleSSO:ClientId"];
            var secretKey = _configuration["GoogleSSO:SecretKey"];
            var redirectUrl= _configuration["GoogleSSO:successurl"];
            var authUrl = GoogleAuth.GetAuthUrl(clientId, redirectUrl);
            return Redirect(authUrl);
        }
        public async Task<IActionResult> SuccessVerify(string code)
        {
            var clientId = _configuration["GoogleSSO:ClientId"];
            var secretKey = _configuration["GoogleSSO:SecretKey"];
            var redirectUrl = _configuration["GoogleSSO:successurl"];
            var token = await GoogleAuth.GetAuthAccessToken(code, clientId, secretKey, redirectUrl);
            var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());
            if (userProfile != null)
            {
                var userData =JsonConvert.DeserializeObject<SignInResponse>(userProfile);
                ViewBag.userProfile = userData;
                return View("SuccessLogin");
            }
            return RedirectToAction("SignInPage", "GoogleSignIn");
        }
    }
}
