using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Src.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Src.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<TokenBlacklist> TokenBlacklists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar índice para búsquedas rápidas por token
            modelBuilder.Entity<TokenBlacklist>()
                .HasIndex(t => t.Token);

            // Configurar índice para limpiar tokens expirados
            modelBuilder.Entity<TokenBlacklist>()
                .HasIndex(t => t.ExpiresAt);
        }
    }
}