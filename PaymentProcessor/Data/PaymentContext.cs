using Microsoft.EntityFrameworkCore;
using PaymentProcessor.Entity;
using PaymentProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Data
{
    public class PaymentContext : DbContext, IPaymentContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {

        }
        public DbSet<Payment> Payments { get; set; }
    }
}
