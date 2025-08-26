using Microsoft.AspNetCore.Mvc;
using MessageForAzarab.Services.Interface;
using System.Threading.Tasks;

namespace MessageForAzarab.ViewComponents
{
    public class UserSentLettersViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public UserSentLettersViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var letters = await _userService.GetUserSentLettersAsync(userId);
            return View(letters);
        }
    }
} 