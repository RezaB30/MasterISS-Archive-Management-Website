using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Archive_Management_Website.ViewModels
{
    public class LoginViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "Username")]
        [Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Localization.Model))]
        public string Username { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "Password")]
        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Localization.Model))]
        public string Password { get; set; }

    }
}