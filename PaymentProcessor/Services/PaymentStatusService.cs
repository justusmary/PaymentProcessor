using Microsoft.Extensions.Logging;
using PaymentProcessor.Data;
using PaymentProcessor.Dto;
using PaymentProcessor.Entity;
using PaymentProcessor.Models;
using PaymentProcessor.Repository;
using PaymentProcessor.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessor.Services
{
    public class PaymentStatusService : IPaymentStatusService
    {
        private readonly IUnitOfWorkBase<PaymentContext> UnitOfWork;
        private readonly IRepositoryBase<PaymentStatus> PaymentStatusRepo;
        private readonly ILogger<PaymentStatusService> Logger;

        public PaymentStatusService(IUnitOfWorkBase<PaymentContext> unitOfWork, ILogger<PaymentStatusService> logger)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            if (PaymentStatusRepo == null)
                PaymentStatusRepo = UnitOfWork.GetRepository<PaymentStatus>();

            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaymentStatusDto> CreatePaymentStatusAsync(CreatePaymentStatusDto create)
        {
            Logger.LogInformation("About to Insert new payment status entry");
            
            var paymentStatus = await PaymentStatusRepo.InsertAsync(new PaymentStatus()
            {
                PaymentId = create.PaymentId,
                Status = create.Status
            });
            var commitResult = await UnitOfWork.CommitAsync();

            return commitResult == 1 ? new PaymentStatusDto()
            {
                Id = paymentStatus.Entity.Id,
                Status = paymentStatus.Entity.Status
            } : null;
        }

    }
}
