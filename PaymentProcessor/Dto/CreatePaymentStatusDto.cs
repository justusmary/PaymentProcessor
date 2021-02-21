using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Dto
{
    public class CreatePaymentStatusDto
    {
        public int PaymentId { get; set; }
        public string Status { get; set; }
    }
}
