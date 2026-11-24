using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacySystem.Domain.Entities
{
   public class SaleInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; } 
        public DateTime CreatedAtDateTime { get; set; }
       public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Note { get; set; } = string.Empty;
        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
    }

