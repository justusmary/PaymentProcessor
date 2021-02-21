using PaymentProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Helpers
{
    public static class PaymentValidator
    {
        public static PaymentValidationStatus Validate(PaymentModel payment)
        {
            if (payment.Amount < 0)
            {
                return new PaymentValidationStatus
                {
                    IsValid = false,
                    Message = "Amount cannot be less than 0"
                };
            }

            if (payment.ExpirationDate < DateTime.Now)
            {
                return new PaymentValidationStatus
                {
                    IsValid = false,
                    Message = "Card is Expired"
                };
            }

            if (payment.SecurityCode != null && (payment.SecurityCode == "" || payment.SecurityCode.Length != 3 || !payment.SecurityCode.All(char.IsDigit)))
            {
                return new PaymentValidationStatus
                {
                    IsValid = false,
                    Message = "Invalid Security Code"
                };
            }

            if (!payment.CreditCardNumber.All(char.IsDigit))
            {
                return new PaymentValidationStatus
                {
                    IsValid = false,
                    Message = "Invalid Credit Card Number"
                };
            }

            if (!IsCardNumberValid(payment.CreditCardNumber))
            {
                return new PaymentValidationStatus
                {
                    IsValid = false,
                    Message = "Invalid Credit Card Number"
                };
            }

            return new PaymentValidationStatus
            {
                IsValid = true
            };
        }

        public static bool IsCardNumberValid(string cardNumber)
        {
            int i, checkSum = 0;

            for (i = cardNumber.Length - 1; i >= 0; i -= 2)
                checkSum += (cardNumber[i] - '0');

            for (i = cardNumber.Length - 2; i >= 0; i -= 2)
            {
                int val = ((cardNumber[i] - '0') * 2);
                while (val > 0)
                {
                    checkSum += (val % 10);
                    val /= 10;
                }
            }

            return ((checkSum % 10) == 0);
        }
    }

    public class PaymentValidationStatus
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
