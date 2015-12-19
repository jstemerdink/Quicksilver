﻿using Microsoft.AspNet.Identity.EntityFramework;

namespace EPiServer.Reference.Commerce.Domain.Models.Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("EcfSqlConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}