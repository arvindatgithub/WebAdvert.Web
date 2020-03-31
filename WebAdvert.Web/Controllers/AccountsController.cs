
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;
        public AccountsController(SignInManager<CognitoUser> signinManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            this._signInManager = signinManager;
            this._userManager = userManager;
            this._pool = pool;

        }
        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (ModelState.IsValid)
            {
                CognitoUser user = _pool.GetUser(model.Email);
                if(user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exist");
                    return View(model);
                }
                user.Attributes.Add(CognitoAttributesConstants.Name, model.Email);
                user.Attributes.Add(CognitoAttributesConstants.BirthDate, model.Birthdate);
                var createdUser=await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (createdUser.Succeeded)
                {
                    RedirectToAction("Confirm", "Accounts");
                }
                else
                {
                    foreach (var error in createdUser.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
              
            }
            return View();
        }
        [HttpGet]
        [ActionName("Confirm")]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
                     return View(model);
        }
       [HttpPost]
       [ActionName("Confirm")]
        public async Task<IActionResult> ConfirmPost(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                if(user == null)
                {
                    ModelState.AddModelError("Not Found", "A user with the given email address was not found");
                    return View(model);
                }
                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code,true).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach( var item in result.Errors)
                    {
                        ModelState.TryAddModelError(item.Code, item.Description);
                    }
                }
            }
            return View(model);
         }
        
        [HttpGet]
        public async Task<IActionResult> Login(LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result =  await  _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    RedirectToAction("Index", "Home");
                }
                else
                {
                   
                        ModelState.TryAddModelError("LoginError","Email or password mismatch!");

                }

            }

                return View("Login", model);
        }


    }

}