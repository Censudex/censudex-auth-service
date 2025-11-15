using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Src.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Src.Data
{
    /// <summary>
    /// Contexto de base de datos para la autenticación.
    /// </summary>
    public class AuthDbContext : DbContext
    {
        /// <summary>
        /// Constructor del contexto de base de datos.
        /// </summary>
        /// <param name="options">Opciones para el contexto de base de datos.</param>
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Conjunto de tokens en la blacklist.
        /// </summary>
        public DbSet<TokenBlacklist> TokenBlacklists { get; set; }

        /// <summary>
        /// Configuración del modelo de datos.
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo.</param>
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