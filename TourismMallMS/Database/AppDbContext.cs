using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TourismMallMS.Database.config;
using TourismMallMS.Models.Entities;

namespace TourismMallMS.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(new TouristRouteConfig().GetType().Assembly);

            var touristRouteJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

            var adminRole = new IdentityRole()
            {
                Id = new Guid().ToString(),
                Name = "Admin",
                NormalizedName = "Admin".ToUpper()
            };
            modelBuilder.Entity<IdentityRole>().HasData(
                adminRole
            );

            var adminUser = new ApplicationUser()
            {
                Id = new Guid().ToString(),
                UserName = "bleso624@gmail.com",
                NormalizedUserName = "bleso624@gmail.com".ToUpper(),
                Email = "bleso624@gmail.com",
                NormalizedEmail = "bleso624@gmail.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "15137667148",
                PhoneNumberConfirmed = false
            };
            adminUser.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(adminUser, "bleso624");
            modelBuilder.Entity<ApplicationUser>().HasData(
                adminUser
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = adminRole.Id,
                    UserId = adminUser.Id
                }
            );

            /*foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }*/
        }
    }
}