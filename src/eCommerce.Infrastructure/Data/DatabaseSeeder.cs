using eCommerce.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eCommerce.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<User> _userManager;

        public DatabaseSeeder(ApplicationDbContext context, RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        private async Task SeedCategoriesAsync()
        {
            if (!await _context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Electronic devices and accessories", IsActive = true },
                    new Category { Name = "Clothing", Description = "Fashion and apparel", IsActive = true },
                    new Category { Name = "Books", Description = "Books and publications", IsActive = true },
                    new Category { Name = "Home & Garden", Description = "Home decor and gardening supplies", IsActive = true },
                    new Category { Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear", IsActive = true }
                };

                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedCustomersAsync()
        {
            var customerEmails = new[] 
            {
                "john.doe@example.com",
                "jane.smith@example.com",
                "mike.wilson@example.com"
            };

            foreach (var email in customerEmails)
            {
                if (!await _context.Users.AnyAsync(u => u.Email == email))
                {
                    var firstName = email.Split('@')[0].Split('.')[0];
                    var lastName = email.Split('@')[0].Split('.')[1];
                    firstName = char.ToUpper(firstName[0]) + firstName.Substring(1);
                    lastName = char.ToUpper(lastName[0]) + lastName.Substring(1);

                    var customer = new User
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(customer, "Customer@123");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(customer, "Customer");
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Role, "Customer"),
                            new Claim(ClaimTypes.Name, customer.UserName),
                            new Claim(ClaimTypes.Email, customer.Email),
                            new Claim("FirstName", customer.FirstName),
                            new Claim("LastName", customer.LastName)
                        };
                        
                        foreach (var claim in claims)
                        {
                            await _userManager.AddClaimAsync(customer, claim);
                        }
                    }
                }
            }
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedCategoriesAsync();
            await SeedCustomersAsync();

            // Check and create admin user if not exists
            if (!await _context.Users.AnyAsync(u => u.UserName == "admin"))
            {
                // Create admin user
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@ecommerce.com",
                    FirstName = "Admin",
                    LastName = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    // Add claims for the admin user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.Name, adminUser.UserName),
                        new Claim(ClaimTypes.Email, adminUser.Email),
                        new Claim("FirstName", adminUser.FirstName),
                        new Claim("LastName", adminUser.LastName)
                    };
                    
                    foreach (var claim in claims)
                    {
                        await _userManager.AddClaimAsync(adminUser, claim);
                    }
                }
            }

            // Check and create vendor user if not exists
            if (!await _context.Users.AnyAsync(u => u.UserName == "vendor"))
            {
                var vendorUser = new User
                {
                    UserName = "vendor",
                    Email = "vendor@ecommerce.com",
                    FirstName = "Vendor",
                    LastName = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(vendorUser, "Vendor@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(vendorUser, "Vendor");
                    // Add claims for the vendor user
                    var vendorClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, "Vendor"),
                        new Claim(ClaimTypes.Name, vendorUser.UserName),
                        new Claim(ClaimTypes.Email, vendorUser.Email),
                        new Claim("FirstName", vendorUser.FirstName),
                        new Claim("LastName", vendorUser.LastName)
                    };
                    
                    foreach (var claim in vendorClaims)
                    {
                        await _userManager.AddClaimAsync(vendorUser, claim);
                    }
                }
            }
        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));

            if (!await _roleManager.RoleExistsAsync("Vendor"))
                await _roleManager.CreateAsync(new IdentityRole<int>("Vendor"));

            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new IdentityRole<int>("Customer"));
        }
    }
}