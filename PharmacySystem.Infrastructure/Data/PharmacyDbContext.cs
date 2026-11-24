using Microsoft.EntityFrameworkCore;
using System;
using PharmacySystem.Domain.Entities;
using System.Collections.Generic;
using System.Text;

namespace PharmacySystem.Infrastructure.Data
{
    public class PharmacyDbContext : DbContext
    {
        public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options)
        {

        }


        public DbSet<Customer> Customers { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<SaleInvoice> SaleInvoices { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<DebtPayment> DebtPayments { get; set; }
    } 
}
