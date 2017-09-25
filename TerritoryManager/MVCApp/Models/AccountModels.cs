using MVCApp.Translate;
using System.ComponentModel.DataAnnotations;

namespace MVCApp.Models
{
    public class LocalPasswordModel
    {
        #region Properties

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        #endregion
    }

    public class LoginModel
    {
        #region Properties

        [Required]
        [Display(ResourceType = typeof(Strings), Name = "AccountUserName")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Strings), Name = "AccountPassword")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Strings), Name = "AccountRememberMe")]
        public bool RememberMe { get; set; }

        #endregion
    }

    public class RegisterModel
    {
        #region Properties

        [Required]
        [Display(Name = "Nazwa użytkownika")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musi mieć co najmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasło i potwierdzenie hasła nie są takie same.")]
        public string ConfirmPassword { get; set; }

        #endregion
    }

    public enum SystemRoles
    {
        Admin, Elder
    }
}
