using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sklep.Models;

    public class CartContext : DbContext
    {
        public CartContext (DbContextOptions<CartContext> options)
            : base(options)
        {
        }

        public DbSet<sklep.Models.Cart> Cart { get; set; }
    }
