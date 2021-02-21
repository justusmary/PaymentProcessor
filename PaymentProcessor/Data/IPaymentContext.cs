using Microsoft.EntityFrameworkCore;
using PaymentProcessor.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Data
{
    public interface IPaymentContext
    {
        public DbSet<Payment> Payments { get; set; }
    }
}
