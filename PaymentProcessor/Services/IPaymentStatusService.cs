using PaymentProcessor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Services
{
    public interface IPaymentStatusService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="create"></param>
        /// <returns></returns>
        Task<PaymentStatusDto> CreatePaymentStatusAsync(CreatePaymentStatusDto create);
    }
}
