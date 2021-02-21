using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentProcessor.Controllers;
using PaymentProcessor.Data;
using PaymentProcessor.Dto;
using PaymentProcessor.Models;
using PaymentProcessor.Services;
using PaymentProcessor.Uow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class PaymentTest
    {
        public PaymentController PaymentController { get; private set; }

        [SetUp]
        public void Setup()
        {
            var builder = new DbContextOptionsBuilder<PaymentContext>();
            builder.UseInMemoryDatabase("Payments");
            var options = builder.Options;

            var expensivePaymentGateway = new Mock<IExpensivePaymentGateway>();
            var cheapPaymentGateway = new Mock<ICheapPaymentGateway>();
            var premiumPaymentService = new Mock<IPremiumPaymentService>();
            var controllerLogger = new Mock<ILogger<PaymentController>>();
            var paymentServiceLogger = new Mock<ILogger<PaymentService>>();
            var paymentStatusServiceLogger = new Mock<ILogger<PaymentStatusService>>();

            var paymentContext = new PaymentContext(options);
            var unitOfwork = new UnitOfWorkBase<PaymentContext>(paymentContext);
            var paymentStatusService = new PaymentStatusService(unitOfwork, paymentStatusServiceLogger.Object);
            var paymentService = new PaymentService(paymentStatusService, unitOfwork, paymentServiceLogger.Object,
                expensivePaymentGateway.Object, cheapPaymentGateway.Object, premiumPaymentService.Object);

            PaymentController = new PaymentController(paymentService, controllerLogger.Object);
        }

        [Test]
        public async Task TestValidInput()
        {
            // Arrange
            var payment = new PaymentModel()
            {
                CardHolder = "John Doe",
                CreditCardNumber = "370000000000002",
                ExpirationDate = DateTime.Now.AddDays(3),
                SecurityCode = "345",
                Amount = 50
            };

            // Act
            var result = await PaymentController.Post(payment);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.IsNotNull(okResult.Value as PaymentDto);
            Assert.AreEqual(1, (okResult.Value as PaymentDto).Id);
        }

        [Test]
        public async Task TestInvalidCCNInput()
        {
            // Arrange
            var payment = new PaymentModel()
            {
                CardHolder = "John Doe",
                CreditCardNumber = "37000000000000",
                ExpirationDate = DateTime.Now.AddDays(3),
                SecurityCode = "345",
                Amount = 50
            };

            // Act
            var result = await PaymentController.Post(payment);
            var badResult = result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(badResult);
            Assert.IsNotNull(badResult.Value);
            Assert.IsNotNull(badResult.Value as string);
            Assert.AreEqual("Invalid Credit Card Number", badResult.Value as string);
        }
    }
}
