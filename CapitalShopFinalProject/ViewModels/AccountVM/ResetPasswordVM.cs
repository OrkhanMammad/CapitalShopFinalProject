using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace CapitalShopFinalProject.ViewModels.AccountVM
{
    public class ResetPasswordVM
    {
        [System.ComponentModel.DataAnnotations.Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

       
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        
        public string Email { get; set; }

        public string Token { get; set; }


    }
}
