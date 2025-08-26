using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MessageForAzarab.Models;

namespace MessageForAzarab.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            Input = new InputModel();
            ReturnUrl = string.Empty;
            ExternalLogins = new List<AuthenticationScheme>();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "ایمیل را وارد کنید")]
            [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
            [Display(Name = "ایمیل")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "رمز عبور را وارد کنید")]
            [StringLength(100, ErrorMessage = "{0} باید حداقل {2} و حداکثر {1} کاراکتر باشد.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "تکرار رمز عبور")]
            [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن مطابقت ندارند.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "نام و نام خانوادگی را وارد کنید")]
            [Display(Name = "نام و نام خانوادگی")]
            public string FullName { get; set; } = string.Empty;

            [Phone(ErrorMessage = "شماره تلفن نامعتبر است")]
            [Display(Name = "شماره تماس")]
            public string PhoneNumber { get; set; } = string.Empty;

            [Display(Name = "نوع کاربر")]
            public UserType UserType { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? string.Empty;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FullName = Input.FullName,
                    PhoneNumber = Input.PhoneNumber,
                    UserType = Input.UserType,
                    IsActive = true,
                    RegisterDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("کاربر جدید با موفقیت ایجاد شد.");

                    // برای ادمین، بعد از ایجاد کاربر جدید، کاربر را لاگین نمی‌کنیم
                    // و به صفحه لیست کاربران برمی‌گردیم
                    return RedirectToAction("Index", "Users");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
} 