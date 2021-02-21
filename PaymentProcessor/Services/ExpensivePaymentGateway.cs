using PaymentProcessor.Dto;
using PaymentProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Services
{
    public class ExpensivePaymentGateway : IExpensivePaymentGateway
    {
        public string MakePayment(CreatePaymentDto payment)
        {
            return "processed";
        }
    }
}
