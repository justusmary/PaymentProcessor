using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Entity
{
    public class PaymentStatus
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Payment")]
        public int PaymentId { get; set; }
        public string Status { get; set; }
    }
}
