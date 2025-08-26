using Microsoft.AspNetCore.Mvc;
using MessageForAzarab.Services.Interface;
using System.Threading.Tasks;

namespace MessageForAzarab.ViewComponents
{
    public class UserSentAttachmentsViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public UserSentAttachmentsViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var attachments = await _userService.GetUserSentAttachmentsAsync(userId);
            return View(attachments);
        }
    }
} 