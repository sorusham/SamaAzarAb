using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MessageForAzarab.Controllers
{
    [Authorize]
    public class LettersController : Controller
    {
        private readonly ILetterService _letterService;
        private readonly IUserService _userService;

        public LettersController(ILetterService letterService, IUserService userService)
        {
            _letterService = letterService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var letters = await _letterService.GetAllLettersAsync();
            return View(letters);
        }

        public async Task<IActionResult> Details(int id)
        {
            var letter = await _letterService.GetLetterByIdAsync(id);

            if (letter == null)
                return NotFound();

            return View(letter);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Letter
            {
                StartDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddMonths(1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Letter letter)
        {
            if (ModelState.IsValid)
            {
                // تنظیم کاربر فعلی به عنوان فرستنده
                var currentUser = await _userService.GetCurrentUserAsync(User);
                if (currentUser != null)
                {
                    letter.SenderId = currentUser.Id;
                    letter.From = currentUser.FullName;
                }
                
                // تنظیم نام گیرنده
                if (!string.IsNullOrEmpty(letter.ReceiverId))
                {
                    var receiverUser = await _userService.GetUserByIdAsync(letter.ReceiverId);
                    if (receiverUser != null)
                    {
                        letter.To = receiverUser.FullName;
                    }
                }
                else
                {
                    // اگر گیرنده‌ای انتخاب نشده، فیلد To را خالی می‌کنیم
                    letter.To = string.Empty;
                }

                await _letterService.CreateLetterAsync(letter);
                return RedirectToAction(nameof(Index));
            }
            return View(letter);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var letter = await _letterService.GetLetterByIdAsync(id);
            if (letter == null)
                return NotFound();

            return View(letter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Letter letter)
        {
            if (id != letter.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // در اینجا فیلدهای SenderId و ReceiverId را حفظ می‌کنیم
                    var existingLetter = await _letterService.GetLetterByIdAsync(id);
                    if (existingLetter == null)
                        return NotFound();
                    
                    // حفظ اطلاعات فرستنده
                    letter.SenderId = existingLetter.SenderId;
                    letter.From = existingLetter.From;

                    // بررسی تغییر گیرنده و به‌روزرسانی فیلد To
                    if (!string.IsNullOrEmpty(letter.ReceiverId))
                    {
                        if (letter.ReceiverId != existingLetter.ReceiverId)
                        {
                            var receiver = await _userService.GetUserByIdAsync(letter.ReceiverId);
                            if (receiver != null)
                            {
                                letter.To = receiver.FullName;
                            }
                            else
                            {
                                letter.To = string.Empty;
                            }
                        }
                        else
                        {
                            letter.To = existingLetter.To;
                        }
                    }
                    else
                    {
                        // اگر گیرنده‌ای انتخاب نشده، فیلد To را خالی می‌کنیم
                        letter.To = string.Empty;
                        letter.ReceiverId = string.Empty;
                    }

                    await _letterService.UpdateLetterAsync(letter);
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // اگر به این خطا برخوردیم یعنی نامه حذف شده است
                    return NotFound();
                }
                catch (Exception ex)
                {
                    // ثبت خطا
                    Console.WriteLine($"خطا در ویرایش نامه: {ex.Message}");
                    ModelState.AddModelError("", "خطایی در ویرایش نامه رخ داد. لطفاً دوباره تلاش کنید.");
                }
            }
            return View(letter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _letterService.DeleteLetterAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
