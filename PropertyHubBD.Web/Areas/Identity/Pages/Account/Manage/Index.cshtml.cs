using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting; // For IWebHostEnvironment
using PropertyHubBD.Web.Models;

namespace PropertyHubBD.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Display(Name = "Occupation")]
            public string Occupation { get; set; }

            [Display(Name = "Bio")]
            public string Bio { get; set; }
            
            [Display(Name = "Profile Picture")]
            public IFormFile ProfilePicture { get; set; }
            
            [Display(Name = "Gender")]
            public string Gender { get; set; }
            
            [Display(Name = "Age")]
            public int? Age { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = await _userManager.GetUserNameAsync(user);

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FullName = user.FullName,
                Occupation = user.Occupation,
                Bio = user.Bio,
                Gender = user.Gender,
                Age = user.Age
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Update custom fields
            if (Input.FullName != user.FullName)
            {
                user.FullName = Input.FullName;
            }
            if (Input.Occupation != user.Occupation)
            {
                user.Occupation = Input.Occupation;
            }
            if (Input.Bio != user.Bio)
            {
                user.Bio = Input.Bio;
            }
             if (Input.Gender != user.Gender)
            {
                user.Gender = Input.Gender;
            }
            if (Input.Age != user.Age)
            {
                user.Age = Input.Age;
            }

            // Handle Profile Picture Upload
            if (Input.ProfilePicture != null && Input.ProfilePicture.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "users");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.ProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(fileStream);
                }
                
                // Save current profile picture path to delete later if needed? For now just overwrite reference.
                user.ProfilePictureUrl = "/images/users/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to update profile.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
