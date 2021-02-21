using Microsoft.Extensions.Logging;
using PaymentProcessor.Data;
using PaymentProcessor.Dto;
using PaymentProcessor.Models;
using PaymentProcessor.Repository;
using PaymentProcessor.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentProcessor.Entity;

namespace PaymentProcessor.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentStatusService PaymentStatusService;
        private readonly IUnitOfWorkBase<PaymentContext> UnitOfWork;
        private readonly IRepositoryBase<Payment> PaymentRepo;
        private readonly IExpensivePaymentGateway ExpensivePaymentGateway;
        private readonly ICheapPaymentGateway CheapPaymentGateway;
        private readonly IPremiumPaymentService PremiumPaymentService;
        private readonly ILogger<PaymentService> Logger;

        public PaymentService(IPaymentStatusService paymentStatusService, IUnitOfWorkBase<PaymentContext> unitOfWork, ILogger<PaymentService> logger,
            IExpensivePaymentGateway expensivePaymentGateway, ICheapPaymentGateway cheapPaymentGateway, IPremiumPaymentService premiumPaymentService)
        {
            PaymentStatusService = paymentStatusService;

            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            if (PaymentRepo == null)
                PaymentRepo = UnitOfWork.GetRepository<Payment>();

            ExpensivePaymentGateway = expensivePaymentGateway;
            CheapPaymentGateway = cheapPaymentGateway;
            PremiumPaymentService = premiumPaymentService;

            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto create)
        {
            Logger.LogInformation("About to call External Provider");

            string status = "failed";
            if (create.Amount < 20)
            {
                status = CheapPaymentGateway.MakePayment(create);
            }
            else if (create.Amount > 21 && create.Amount < 500)
            {
                status = ExpensivePaymentGateway.MakePayment(create);
                if (status == "failed")
                {
                    status = CheapPaymentGateway.MakePayment(create);
                }
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    status = PremiumPaymentService.MakePayment(create);
                    if (status != "failed")
                    {
                        break;
                    }
                }
            }

            if (status == "failed")
            {
                return null;
            }

            Logger.LogInformation("About to Insert new payment entry");
            var paymentInfo = await PaymentRepo.InsertAsync(new Payment()
            {
                CreditCardNumber = create.CreditCardNumber,
                CardHolder = create.CardHolder,
                ExpirationDate = create.ExpirationDate,
                SecurityCode = create.SecurityCode,
                Amount = create.Amount
            });

            var commitResult = await UnitOfWork.CommitAsync();

            var paymentStatus = await PaymentStatusService.CreatePaymentStatusAsync(new CreatePaymentStatusDto()
            {
                PaymentId = paymentInfo.Entity.Id,
                Status = status
            });

            return commitResult == 1 ?  new PaymentDto()
            {
                Id = paymentInfo.Entity.Id,
                Amount = paymentInfo.Entity.Amount,
                Status = paymentStatus != null ? paymentStatus.Status : null
            } : null;
        }

        public async Task DeletePaymentAsync(int Id)
        {
            Logger.LogInformation("About to Find payment entry with Id {@Id}", Id);

            var paymentInfo = await PaymentRepo.SingleOrDefaultAsync(x => x.Id == Id);

            if (paymentInfo != null)
            {
                Logger.LogInformation("Found payment entry with Id {@Id}", Id);
                Logger.LogInformation("About to Delete payment entry with Id {@Id}", Id);

                await PaymentRepo.DeleteAsync(paymentInfo);
                var commitResult = await UnitOfWork.CommitAsync();

                if (commitResult == 1)
                {
                    await UnitOfWork.CommitAsync();
                }
            }
        }

        public async Task<PaymentDto> GetPaymentAsync(int Id)
        {
            Logger.LogInformation("About to Find payment entry with Id {@Id}", Id);
            var paymentInfo = await PaymentRepo.SingleOrDefaultAsync(x => x.Id == Id, include: x => x.Include(y => y.Status));
            return paymentInfo != null ? new PaymentDto()
            {
                Id = paymentInfo.Id,
                Amount = paymentInfo.Amount,
                Status = paymentInfo.Status.Status
            } : null;
        }

        public async Task<IList<PaymentDto>> GetPaymentsAsync()
        {
            Logger.LogInformation("About to get all payment entry");
            var paymentInfos = await PaymentRepo.GetListAsync(include: x => x.Include(x => x.Status));
            return paymentInfos.Select(x => new PaymentDto()
            {
                Id = x.Id,
                Amount = x.Amount,
                Status = x.Status.Status
            }).ToList();
        }
    }
}
