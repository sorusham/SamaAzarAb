using Microsoft.AspNetCore.Mvc;
using MessageForAzarab.Services.Interface;
using System.Threading.Tasks;

namespace MessageForAzarab.ViewComponents
{
    public class UserReceivedLettersViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public UserReceivedLettersViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var letters = await _userService.GetUserReceivedLettersAsync(userId);
            return View(letters);
        }
    }
} 