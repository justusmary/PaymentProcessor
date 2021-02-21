using PaymentProcessor.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Swagger
{
    public class PaymentModelExample : IExamplesProvider<PaymentModel>
    {
        public PaymentModel GetExamples()
        {
            return new PaymentModel()
            {
                CardHolder = "John Doe",
                CreditCardNumber = "370000000000002",
                ExpirationDate = DateTime.Now,
                SecurityCode = "345",
                Amount = 50
            };
        }
    }
}
