using Microsoft.EntityFrameworkCore;
using Projekt.Entities.AuthModels;
using Projekt.Entities.ClientModels;
using Projekt.Entities.OrderModels;
using Projekt.Entities.ProductModels;
using System.Data;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Projekt.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<PrivatePerson> PrivateClients { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.Phone).IsRequired().HasMaxLength(9);
                e.Property(c => c.Email).IsRequired().HasMaxLength(50);
                e.Property(c => c.Address).IsRequired().HasMaxLength(300);
                e.HasDiscriminator<string>("ClientType")
                    .HasValue<Company>("Company")
                    .HasValue<PrivatePerson>("Private Person");
                e.ToTable("Clients");
            });

            modelBuilder.Entity<Shipment>(e =>
            {
                e.HasKey(s => s.Id);
                e.Property(s => s.Name).IsRequired().HasMaxLength(50);
                e.Property(s => s.MaxWeight).IsRequired().HasColumnType("decimal(8,2)");
                e.ToTable("Shipments");
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.Name).IsRequired().HasMaxLength(50);
                e.Property(c => c.Description).HasMaxLength(300);
                e.ToTable("Categories");
            });

            modelBuilder.Entity<Order>(e =>
            {
                e.HasKey(o => o.Id);
                e.Property(o=>o.TotalWeight).IsRequired().HasColumnType("decimal(8,2)");
                e.Property(o => o.TotalPrice).IsRequired().HasColumnType("decimal(8,2)");
                e.Property(o => o.Status).IsRequired();
                e.Property(o => o.Date).IsRequired();
                e.Property(o => o.IsDeleted).IsRequired().HasColumnType("bit");
                e.HasOne(o => o.Shipment).WithMany(s => s.Orders).HasForeignKey(o => o.ShipmentId).IsRequired();
                e.HasOne(o => o.Client).WithMany(c => c.Orders).HasForeignKey(o=>o.ClientId).IsRequired();
                e.ToTable("Orders");
            });

            modelBuilder.Entity<Payment>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Amount).IsRequired().HasColumnType("decimal(8,2)");
                e.Property(p => p.Date).IsRequired();
                e.Property(p => p.Method).IsRequired();
                e.HasOne(p=> p.Order).WithMany(o => o.Payments).HasForeignKey(p => p.OrderId).IsRequired();
                e.ToTable("Payments");
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(50);
                e.Property(p => p.Description).HasMaxLength(300);
                e.Property(p => p.Weight).IsRequired().HasColumnType("decimal(8,2)");
                e.Property(p => p.CurrentPrice).IsRequired().HasColumnType("decimal(8,2)");
                e.Property(p => p.IsDeleted).IsRequired().HasColumnType("bit");
                e.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).IsRequired();
                e.ToTable("Products");
            });

            modelBuilder.Entity<OrderProduct>(e =>
            {
                e.Property(op => op.Quantity).IsRequired();
                e.Property(op => op.UnitPrice).IsRequired().HasColumnType("decimal(8,2)");
                e.HasKey(op => new { op.OrderId, op.ProductId });
                e.HasOne(op => op.Product).WithMany(p => p.OrderProducts).HasForeignKey(op => op.ProductId).IsRequired();
                e.HasOne(op => op.Order).WithMany(o => o.OrderProducts).HasForeignKey(op => op.OrderId).IsRequired();
                e.ToTable("OrderProducts");
            });

            modelBuilder.Entity<Shipment>().HasData(
                
                new Shipment()
                {
                    Id = 1,
                    Name = "A Shipment",
                    MaxWeight = new decimal(10)

                },

                new Shipment()
                {
                    Id = 2,
                    Name = "B Shipment",
                    MaxWeight = new decimal(150)
                }, 

                new Shipment()
                {
                    Id = 3,
                    Name = "C Shipment",
                    MaxWeight = new decimal(250)
                }, 
                
                new Shipment()
                {
                    Id = 4,
                    Name = "D Shipment",
                    MaxWeight = new decimal(500)
                }

             );

            modelBuilder.Entity<Category>().HasData(
                new Category()
                {
                    Id = 1,
                    Name = "Category A",
                    Description = "First category"
                },
                new Category()
                {
                    Id = 2,
                    Name = "Category B",
                }, 
                new Category()
                {
                    Id = 3,
                    Name = "Category C",
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product()
                {
                    Id = 1,
                    Name = "Product A",
                    Description = "First product",
                    Weight = new decimal(10),
                    CurrentPrice = new decimal(10),
                    CategoryId = 1,
                    IsDeleted = false,
                },
                new Product()
                {
                    Id = 2,
                    Name = "Product B",
                    Weight = new decimal(3),
                    CurrentPrice = new decimal(100),
                    CategoryId = 2,
                    IsDeleted = false,
                }
            );

            modelBuilder.Entity<Company>().HasData(
                new Company()
                {
                    Id = 1,
                    Phone = "123456789",
                    Email = "contact@a.com",
                    Address = "address",
                    Name = "Company A",
                    NIP = "1234567890"
                },
                new Company()
                {
                    Id = 2,
                    Phone = "987654321",
                    Email = "contact@b.com",
                    Address = "address",
                    Name = "Company B",
                    NIP = "1231231231"
                }
            );

            modelBuilder.Entity<PrivatePerson>().HasData(
                new PrivatePerson()
                {
                    Id = 3,
                    Phone = "111222333",
                    Email = "contact@person.com",
                    Address = "address",
                    FirstName = "John",
                    LastName = "Doe",
                    PESEL = "00000000000"
                }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order()
                {
                    Id = 1,
                    TotalPrice = new decimal(110),
                    TotalWeight = new decimal(13),
                    ShipmentId = 2,
                    ClientId = 1,
                    Date = new DateTime(2026, 7, 13),
                    Status = Enums.State.ACTIVE,
                    IsDeleted = false,
                },
                new Order()
                {
                    Id = 2,
                    TotalPrice = new decimal(200),
                    TotalWeight = new decimal(6),
                    ShipmentId = 1,
                    ClientId = 2,
                    Date = new DateTime(2026, 7, 1),
                    Status = Enums.State.ACTIVE,
                    IsDeleted = false,
                }
             );

            modelBuilder.Entity<OrderProduct>().HasData(
                new OrderProduct()
                {
                    ProductId = 1,
                    OrderId = 1,
                    Quantity = 1,
                    UnitPrice = 10
                },
                new OrderProduct()
                {
                    ProductId = 2,
                    OrderId = 1,
                    Quantity = 1,
                    UnitPrice = 100
                },
                new OrderProduct()
                {
                    ProductId = 2,
                    OrderId = 2,
                    Quantity = 2,
                    UnitPrice = 100
                }
             );

            modelBuilder.Entity<Payment>().HasData(
                new Payment()
                {
                    Id = 1,
                    Amount = new decimal(40),
                    Date = new DateTime(2026, 7, 13),
                    Method = Enums.Method.CARD,
                    OrderId = 1

                },
                new Payment()
                {
                    Id = 2,
                    Amount = new decimal(40),
                    Date = new DateTime(2026, 7, 13),
                    Method = Enums.Method.CASH,
                    OrderId = 1
                }
            );

            modelBuilder.Entity<User>(u =>
            {
                u.HasKey(x => x.Id);
                u.Property(x => x.Login).IsRequired().HasMaxLength(50);
                u.Property(x => x.Role).IsRequired().HasMaxLength(50);
                u.Property(x => x.HashPassword).IsRequired();
                u.ToTable("Users");

            });

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Login = "admin", HashPassword = "$2a$12$gbZykxjbstTqpePg7i2R7.d/YJyyWHoCXXAO.Y.vk6uAWa/wRs4g6", Role = "Admin" }, //admin123
                new User { Id = 2, Login = "user", HashPassword = "$2a$12$9eOyIdjw7ryRAzB4RtYuc.qkrmZo9r5vtWhIXJ1zeW4cbzXcM6HfG", Role = "User" } //user123
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
