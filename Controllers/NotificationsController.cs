using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessageForAzarab.Services.Interface;
using System.Threading.Tasks;

namespace MessageForAzarab.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public NotificationsController(INotificationService notificationService, IUserService userService)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userService.GetCurrentUserAsync(User);
            if (currentUser == null)
                return NotFound();

            var notifications = await _notificationService.GetUserNotificationsAsync(currentUser.Id);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var currentUser = await _userService.GetCurrentUserAsync(User);
            if (currentUser == null)
                return NotFound();

            await _notificationService.MarkAllAsReadAsync(currentUser.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var currentUser = await _userService.GetCurrentUserAsync(User);
            if (currentUser == null)
                return Json(new { count = 0 });

            var count = await _notificationService.GetUnreadNotificationCountAsync(currentUser.Id);
            return Json(new { count });
        }
    }
} 