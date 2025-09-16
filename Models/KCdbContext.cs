using Microsoft.EntityFrameworkCore;

namespace Koves.UserOrder.WebApi.Models;

public partial class KCdbContext : DbContext
{
    public KCdbContext()
    {
    }

    public KCdbContext(DbContextOptions<KCdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<UserRoles> UserRoles { get; set; }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<OrderItems> OrderItems { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<UserRoles>()
        .HasKey(ur => new { ur.UserId, ur.RoleId });

        //設定 UserRoles.UserId 對應到父 User.Id
        modelBuilder.Entity<UserRoles>()
        .HasOne(ur => ur.Users)
        .WithMany(u => u.UserRoles)
        .HasForeignKey(ur => ur.UserId);

        //設定 UserRoles.RoleId 對應到父 Roles.RoleId
        modelBuilder.Entity<UserRoles>()
            .HasOne(ur => ur.Roles)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        //設定 Order.UserId 對應到父 User.Id
        modelBuilder.Entity<Orders>()
        .HasOne(ur => ur.Users)
        .WithMany(u => u.Orders)
        .HasForeignKey(ur => ur.UserId);

        //設定 OrderItems.OrderId 對應到父 Orders.OrderId
        modelBuilder.Entity<OrderItems>()
        .HasOne(ur => ur.Orders)
        .WithMany(u => u.OrderItems)
        .HasForeignKey(ur => ur.OrderId);

        //設定 OrderItems.ProductId 對應到父 Products.ProductId
        modelBuilder.Entity<OrderItems>()
        .HasOne(ur => ur.Products)
        .WithMany(u => u.OrderItems)
        .HasForeignKey(ur => ur.ProductId);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
