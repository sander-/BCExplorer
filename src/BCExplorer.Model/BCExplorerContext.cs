using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BCExplorer.Model
{
    public class BCExplorerContext : DbContext
    {
        static string _connectionString;

        public DbSet<Statistics> Statistics { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressTransaction> AddressTransactions { get; set; }

        public BCExplorerContext()
        { }

        public BCExplorerContext(DbContextOptions options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString == null)
            {
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                var directory = Path.GetDirectoryName(location);
                var csPath = Path.Combine(directory, "connectionstring.secret");
                if (File.Exists(csPath))
                    _connectionString = File.ReadAllText(csPath).Trim();
                else
                {
                    // running from EF Tools
                    string pathForEfTools = @"C:\Projects\Github\BCExplorer\src\BCExplorer.Model\connectionstring.secret";
                    _connectionString = File.ReadAllText(pathForEfTools).Trim();
                }
            }
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Block>()
                .HasIndex(p => new { p.BlockHash }).ForSqlServerIsClustered(false).IsUnique();
        }
    }
}
