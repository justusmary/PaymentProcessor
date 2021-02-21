using PaymentProcessor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Services
{
    public interface IPaymentService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<PaymentDto> GetPaymentAsync(int Id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IList<PaymentDto>> GetPaymentsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="create"></param>
        /// <returns></returns>
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto create);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task DeletePaymentAsync(int Id);
    }
}
