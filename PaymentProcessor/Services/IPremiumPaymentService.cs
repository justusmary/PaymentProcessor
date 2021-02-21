using PaymentProcessor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Services
{
    public interface IPremiumPaymentService
    {
        public string MakePayment(CreatePaymentDto payment);
    }
}
