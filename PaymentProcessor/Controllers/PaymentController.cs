using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentProcessor.Dto;
using PaymentProcessor.Helpers;
using PaymentProcessor.Models;
using PaymentProcessor.Services;

namespace PaymentProcessor.Controllers
{
    [Route("api/ProcessPayment")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService PaymentService;
        private readonly ILogger<PaymentController> Logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            PaymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Adds a new Payment
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post([FromBody] PaymentModel payment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var validationStatus = PaymentValidator.Validate(payment);
                if (!validationStatus.IsValid)
                {
                    return BadRequest(validationStatus.Message);
                }

                var createPaymentResponse = await PaymentService.CreatePaymentAsync(new CreatePaymentDto()
                {
                    CreditCardNumber = payment.CreditCardNumber,
                    CardHolder = payment.CardHolder,
                    ExpirationDate = payment.ExpirationDate,
                    SecurityCode = payment.SecurityCode,
                    Amount = payment.Amount
                });

                if (createPaymentResponse?.Id > 0)
                {
                    return Ok(createPaymentResponse);
                }
                else
                {
                    Logger.LogError("Transaction could not be completed");
                    return BadRequest("Transaction could not be completed");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.ToString());
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Get a specific Payment
        /// </summary>
        /// <returns></returns>
        /// <param name="Id" example="1">The Applicant id</param>
        
        [HttpGet("{Id:int}")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get(int Id)
        {
            try
            {
                if (Id <= 0)
                {
                    Logger.LogInformation("Invalid Payment Id {0}", Id);
                    return BadRequest("Invalid Id");
                }

                PaymentDto paymentInfo = await PaymentService.GetPaymentAsync(Id);

                if (paymentInfo != null)
                {
                    return Ok(paymentInfo);
                }
                else
                {
                    Logger.LogError("Could not find entry [{Id}]", Id);
                    return BadRequest($"Could not find entry for {Id}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.ToString());
                return BadRequest(ex);
            }
        }
    }
}
