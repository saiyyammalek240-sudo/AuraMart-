using Microsoft.AspNetCore.Identity;
using ParkEasePro.Models;

namespace ParkEasePro.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.EnsureCreatedAsync();

        // 1. Roles
        string[] roles = { "Admin", "Customer" };
        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
            {
                await roleManager.CreateAsync(new IdentityRole(r));
            }
        }

        // 2. Admin User
        var admin = await userManager.FindByEmailAsync("admin@auramart.com");
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin@auramart.com",
                Email = "admin@auramart.com",
                FullName = "Kavita Singhania (Store Director)",
                Address = "Executive HQ, Tech Hub Tower 4",
                City = "Surat",
                PostalCode = "395007",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // 3. Customer User
        var customer = await userManager.FindByEmailAsync("customer@auramart.com");
        if (customer == null)
        {
            customer = new ApplicationUser
            {
                UserName = "customer@auramart.com",
                Email = "customer@auramart.com",
                FullName = "Aarav Sharma",
                Address = "Flat 402, Skyline Luxury Towers, University Road",
                City = "Surat",
                PostalCode = "395007",
                PhoneNumber = "+91 98765 43210",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(customer, "Customer@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customer, "Customer");
            }
        }

        // 4. Categories & Products
        if (!context.Categories.Any())
        {
            var catElectronics = new Category
            {
                Name = "Smart Gadgets & Electronics",
                Slug = "electronics",
                Description = "Cutting-edge wearable technology, 4K displays, and smart accessories.",
                IconClass = "bi-laptop",
                ImageUrl = "https://images.unsplash.com/photo-1519389950473-47ba0277781c?auto=format&fit=crop&w=600&q=80",
                IsActive = true
            };

            var catAudio = new Category
            {
                Name = "Premium Audio & Acoustics",
                Slug = "audio",
                Description = "Studio-grade wireless headphones, noise-cancelling earbuds, and hi-fi audio gear.",
                IconClass = "bi-headphones",
                ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=600&q=80",
                IsActive = true
            };

            var catDesk = new Category
            {
                Name = "Desk Setup & Keyboards",
                Slug = "desk-setup",
                Description = "Ergonomic mechanical keyboards, desk mats, and productivity peripherals.",
                IconClass = "bi-keyboard",
                ImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=600&q=80",
                IsActive = true
            };

            var catFashion = new Category
            {
                Name = "Urban Lifestyle & Footwear",
                Slug = "urban-lifestyle",
                Description = "Modern streetwear, minimalist sneakers, and designer carry backpacks.",
                IconClass = "bi-bag-heart",
                ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&w=600&q=80",
                IsActive = true
            };

            context.Categories.AddRange(catElectronics, catAudio, catDesk, catFashion);
            await context.SaveChangesAsync();

            // Seed Products
            var products = new List<Product>
            {
                new Product
                {
                    Name = "SonicPro X7 Wireless Noise Cancelling Headphones",
                    Slug = "sonicpro-x7-wireless",
                    CategoryId = catAudio.Id,
                    Brand = "AuraAudio",
                    SKU = "AA-X7-001",
                    Price = 6499.00m,
                    OriginalPrice = 8999.00m,
                    ShortDescription = "Active noise cancelling with 40-hour playback and spatial 3D audio.",
                    Description = "The SonicPro X7 delivers unmatched audio fidelity with custom-tuned 40mm beryllium drivers, dual transparency modes, and aerospace-grade memory foam cushions for all-day comfort.",
                    StockQuantity = 18,
                    ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=800&q=80",
                    IsFeatured = true,
                    IsTrending = true,
                    IsOnSale = true,
                    Rating = 4.9,
                    ReviewCount = 42
                },
                new Product
                {
                    Name = "NovaWatch Ultra GPS Smartwatch (Titanium)",
                    Slug = "novawatch-ultra-titanium",
                    CategoryId = catElectronics.Id,
                    Brand = "NovaTech",
                    SKU = "NV-WCH-09",
                    Price = 11999.00m,
                    OriginalPrice = 14999.00m,
                    ShortDescription = "Aerospace grade titanium casing with 1.96-inch Always-on AMOLED display.",
                    Description = "Engineered for endurance and performance. Track heart rate variability, SpO2, sleep stages, and multisport GPS telemetry with up to 14 days of battery longevity.",
                    StockQuantity = 12,
                    ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&w=800&q=80",
                    IsFeatured = true,
                    IsTrending = true,
                    IsOnSale = true,
                    Rating = 4.8,
                    ReviewCount = 29
                },
                new Product
                {
                    Name = "KeyPulse 75% Wireless Mechanical Keyboard (Gateron Yellow)",
                    Slug = "keypulse-75-mechanical-keyboard",
                    CategoryId = catDesk.Id,
                    Brand = "KeyPulse",
                    SKU = "KP-KB-75Y",
                    Price = 4299.00m,
                    OriginalPrice = 5499.00m,
                    ShortDescription = "Hot-swappable PCB, gasket mounted acoustic dampening, and Bluetooth 5.3.",
                    Description = "Elevate your coding and typing experience. Featuring factory pre-lubed switches, dye-sub PBT keycaps, per-key RGB backlighting, and CNC anodized aluminum frame.",
                    StockQuantity = 25,
                    ImageUrl = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=800&q=80",
                    IsFeatured = true,
                    IsTrending = true,
                    IsOnSale = false,
                    Rating = 4.9,
                    ReviewCount = 63
                },
                new Product
                {
                    Name = "Aura Glide Minimalist Leather Sneakers",
                    Slug = "aura-glide-leather-sneakers",
                    CategoryId = catFashion.Id,
                    Brand = "AuraWear",
                    SKU = "AW-SNK-01W",
                    Price = 3299.00m,
                    OriginalPrice = 4499.00m,
                    ShortDescription = "Handcrafted full-grain Italian leather sneakers with orthopedic footbed.",
                    Description = "Sleek, versatile, and effortlessly stylish. Pairs seamlessly with tailored trousers, denim, or casual shorts. Features vulcanized rubber cupsole for durable daily traction.",
                    StockQuantity = 14,
                    ImageUrl = "https://images.unsplash.com/photo-1549298916-b41d501d3772?auto=format&fit=crop&w=800&q=80",
                    IsFeatured = false,
                    IsTrending = true,
                    IsOnSale = true,
                    Rating = 4.7,
                    ReviewCount = 18
                },
                new Product
                {
                    Name = "VisionDesk 34-Inch Curved Ultrawide 4K Monitor",
                    Slug = "visiondesk-34-curved-ultrawide",
                    CategoryId = catElectronics.Id,
                    Brand = "VisionView",
                    SKU = "VV-MON-34",
                    Price = 28999.00m,
                    OriginalPrice = 34999.00m,
                    ShortDescription = "144Hz 1ms IPS panoramic curvature with 90W USB-C Power Delivery.",
                    Description = "Maximize productivity and creative workflow. The 21:9 aspect ratio eliminates bezel gaps, offering 99% sRGB color gamut calibration right out of the box.",
                    StockQuantity = 8,
                    ImageUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=800&q=80",
                    IsFeatured = true,
                    IsTrending = false,
                    IsOnSale = true,
                    Rating = 5.0,
                    ReviewCount = 15
                },
                new Product
                {
                    Name = "Aura BassPod Studio 2.1 Hi-Fi Bluetooth Speaker",
                    Slug = "aura-basspod-studio-speaker",
                    CategoryId = catAudio.Id,
                    Brand = "AuraAudio",
                    SKU = "AA-BPD-21",
                    Price = 3899.00m,
                    OriginalPrice = 4999.00m,
                    ShortDescription = "Dual passive radiators, warm timber wood enclosure, and capacitive touch dial.",
                    Description = "Deep resonant bass meets crystal clear vocal frequencies. Perfect desktop companion with multi-room pairing and analog 3.5mm AUX input support.",
                    StockQuantity = 30,
                    ImageUrl = "https://images.unsplash.com/photo-1545454675-3531b543be5d?auto=format&fit=crop&w=800&q=80",
                    IsFeatured = false,
                    IsTrending = true,
                    IsOnSale = false,
                    Rating = 4.6,
                    ReviewCount = 21
                },
                new Product
                {
                    Name = "Nomad Tech Daypack (Weatherproof 22L)",
                    Slug = "nomad-tech-daypack-22l",
                    CategoryId = catFashion.Id,
                    Brand = "AuraWear",
                    SKU = "AW-BAG-22B",
                    Price = 2699.00m,
                    OriginalPrice = 3499.00m,
                    ShortDescription = "Cordura nylon fabric with dedicated suspended 16-inch laptop compartment.",
                    Description = "Designed for daily commutes and airport travel. Features hidden passport security pocket, luggage trolley passthrough, and magnetic Fidlock quick-release buckles.",
                    StockQuantity = 22,
                    ImageUrl = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?auto=format&fit=crop&w=800&q=80",
                    IsFeatured = false,
                    IsTrending = false,
                    IsOnSale = true,
                    Rating = 4.8,
                    ReviewCount = 37
                },
                new Product
                {
                    Name = "PrecisionFlow Ergonomic Vertical Wireless Mouse",
                    Slug = "precisionflow-ergonomic-mouse",
                    CategoryId = catDesk.Id,
                    Brand = "KeyPulse",
                    SKU = "KP-MS-01V",
                    Price = 1899.00m,
                    OriginalPrice = 2499.00m,
                    ShortDescription = "57-degree natural handshake angle reduces forearm strain by 40%.",
                    Description = "Work pain-free during long study and coding sessions. Features dual optical sensors up to 4000 DPI, silent click micro-switches, and USB-C fast charging.",
                    StockQuantity = 40,
                    ImageUrl = "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?auto=format&fit=crop&w=800&q=80",
                    IsFeatured = false,
                    IsTrending = true,
                    IsOnSale = false,
                    Rating = 4.7,
                    ReviewCount = 52
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // 5. Seed Coupons
            var coupons = new List<Coupon>
            {
                new Coupon { Code = "SAVE20", DiscountPercent = 20, MinOrderAmount = 1000, IsActive = true },
                new Coupon { Code = "FESTIVE10", DiscountPercent = 10, MinOrderAmount = 500, IsActive = true },
                new Coupon { Code = "STUDENT50", DiscountPercent = 50, MinOrderAmount = 3000, IsActive = true }
            };
            context.Coupons.AddRange(coupons);
            await context.SaveChangesAsync();

            // 6. Seed Sample Order for Demonstration
            if (customer != null)
            {
                var p1 = products[0]; // Headphones
                var p2 = products[2]; // Keyboard

                var demoOrder = new Order
                {
                    OrderNumber = "AUR-2026-X8921",
                    UserId = customer.Id,
                    OrderDate = DateTime.UtcNow.AddDays(-2),
                    SubTotal = p1.Price + p2.Price,
                    DiscountAmount = 500.00m,
                    TaxAmount = 0.00m,
                    ShippingFee = 0.00m,
                    GrandTotal = (p1.Price + p2.Price) - 500.00m,
                    Status = OrderStatus.Processing,
                    PaymentStatus = PaymentStatus.Paid,
                    PaymentMethod = PaymentMethod.UPI,
                    TransactionId = "UPI-TXN-9823412",
                    ShippingFullName = "Aarav Sharma",
                    ShippingPhone = "+91 98765 43210",
                    ShippingAddress = "Flat 402, Skyline Luxury Towers, University Road",
                    ShippingCity = "Surat",
                    ShippingPostalCode = "395007",
                    TrackingNumber = "AUR-TRK-789012",
                    EstimatedDelivery = DateTime.UtcNow.AddDays(2),
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = p1.Id,
                            ProductName = p1.Name,
                            ProductImageUrl = p1.ImageUrl,
                            UnitPrice = p1.Price,
                            Quantity = 1,
                            SubTotal = p1.Price
                        },
                        new OrderItem
                        {
                            ProductId = p2.Id,
                            ProductName = p2.Name,
                            ProductImageUrl = p2.ImageUrl,
                            UnitPrice = p2.Price,
                            Quantity = 1,
                            SubTotal = p2.Price
                        }
                    }
                };

                context.Orders.Add(demoOrder);
                await context.SaveChangesAsync();
            }
        }
    }
}
