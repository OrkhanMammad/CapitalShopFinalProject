﻿using Microsoft.AspNetCore.Identity;
using NuGet.Common;
using System.ComponentModel.DataAnnotations;

namespace CapitalShopFinalProject.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string SurName { get; set; }

        public IEnumerable<Review>? Reviews { get; set; }

        public IEnumerable<Address>? Addresses { get; set; }

        public List<Basket>? Baskets { get; set; }

        public IEnumerable<Order>? Orders { get; set; }

        public IEnumerable<CreditCard>? CreditCards { get;}

        public string? BlockedBy { get; set; }








    }
}
