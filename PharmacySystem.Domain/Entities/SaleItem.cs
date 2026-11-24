using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacySystem.Domain.Entities
{
   public class SaleItem
    {
       public int Id { get; set; }
        public int SaleInvoiceId { get; set; }
        public SaleInvoice? SaleInvoice { get; set; } = null!;
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
