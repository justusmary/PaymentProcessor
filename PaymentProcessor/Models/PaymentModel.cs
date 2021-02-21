using PaymentProcessor.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Models
{
    public class PaymentModel
    {
        public string CreditCardNumber { get; set; }
        public string CardHolder { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string? SecurityCode { get; set; }
        public decimal Amount { get; set; }
    }
}
