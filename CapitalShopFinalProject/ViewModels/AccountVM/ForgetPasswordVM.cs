using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;


namespace CapitalShopFinalProject.ViewModels.AccountVM
{
    public class ForgetPasswordVM
    {
        [System.ComponentModel.DataAnnotations.Required]
        [EmailAddress]
        public string Email { get; set; }



    }
}
