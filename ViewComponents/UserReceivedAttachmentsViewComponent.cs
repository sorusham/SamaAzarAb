using Microsoft.AspNetCore.Mvc;
using MessageForAzarab.Services.Interface;
using System.Threading.Tasks;

namespace MessageForAzarab.ViewComponents
{
    public class UserReceivedAttachmentsViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public UserReceivedAttachmentsViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var attachments = await _userService.GetUserReceivedAttachmentsAsync(userId);
            return View(attachments);
        }
    }
} 