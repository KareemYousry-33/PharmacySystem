using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacySystem.Domain.Entities
{
 public class DebtPayment
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
