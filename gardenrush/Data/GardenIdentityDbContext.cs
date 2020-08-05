using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace gardenrush.Data
{
    public class GardenIdentityDbContext : IdentityDbContext
    {
        public GardenIdentityDbContext(DbContextOptions<GardenIdentityDbContext> options)
            : base(options)
        {
        }
    }
}
