﻿using FirstMVCtest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstMVCtest.Data
{
    public class ItemDb : DbContext
    {
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Collection> Collections { get; set; } = null!;

        public ItemDb(DbContextOptions<ItemDb> options) : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connection = @"Data Source=.;Initial Catalog=Collectionbox;Integrated Security=true;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Collection to Category (one-to-many)
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Collection)
                .WithMany(c => c.Categories)
                .HasForeignKey(c => c.CollectionId);

            // Category to Item (many-to-many)
            modelBuilder.Entity<Item>()
                .HasMany(c => c.Categories)
                .WithMany(c => c.Items);
        }


    }
}
