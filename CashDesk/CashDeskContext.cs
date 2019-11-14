using CashDesk.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk
{
    public class CashDeskContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public DbSet<Membership> Memberships { get; set; }

        public DbSet<Deposit> Deposits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseInMemoryDatabase("CashDeskDB");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // primary key
            builder.Entity<Member>()
                .HasKey(m => m.MemberNumber);
            // required
            builder.Entity<Member>()
                .Property(m => m.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            // required
            builder.Entity<Member>()
                .Property(m => m.LastName)
                .IsRequired()
                .HasMaxLength(100);
            // unique
            builder.Entity<Member>()
                .HasIndex(u => u.LastName)
                .IsUnique();
            // required
            builder.Entity<Member>()
                .Property(m => m.Birthday)
                .IsRequired();

            // primary key
            builder.Entity<Membership>()
                .HasKey(m => m.MembershipId);
            // required
            builder.Entity<Membership>()
                .Property(m => m.Member)
                .IsRequired();
            // required
            builder.Entity<Membership>()
                .Property(m => m.Begin)
                .IsRequired();

            // primary key
            builder.Entity<Deposit>()
                .HasKey(d => d.DepositId);
            // required
            builder.Entity<Deposit>()
                .Property(d => d.Membership)
                .IsRequired();
            // required
            builder.Entity<Deposit>()
                .Property(d => d.Amount)
                .IsRequired();

            // Cascade Delete
            builder.Entity<Member>()
                .HasMany(m => m.Memberships)
                .WithOne(m => m.Member)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Membership>()
                .HasMany(m => m.Deposits)
                .WithOne(m => m.Membership)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}