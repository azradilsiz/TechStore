using TechStore.API.Constants;
using TechStore.API.DTOs.Payments;
using TechStore.API.Entities;
using TechStore.API.Repositories.Interfaces;

namespace TechStore.API.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            List<Payment> payments = await _paymentRepository.GetAllAsync();

            return payments.Select(payment => MapPaymentToDto(payment)).ToList();
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int id)
        {
            Payment? payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                return null;
            }

            return MapPaymentToDto(payment);
        }

        public async Task<PaymentDto?> CreatePaymentAsync(int orderId, int userId, bool isAdmin, CreatePaymentDto dto)
        {
            Order? order = await _paymentRepository.GetOrderByIdAsync(orderId);

            if (order == null || order.Payment != null || (!isAdmin && order.UserId != userId))
            {
                return null;
            }

            Payment payment = new Payment
            {
                OrderId = orderId,
                Order = order,
                Amount = order.TotalPrice,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = PaymentRules.GetInitialStatus(dto.PaymentMethod),
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return MapPaymentToDto(payment);
        }

        public async Task<bool> UpdatePaymentAsync(int id, UpdatePaymentDto dto)
        {
            Payment? payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                return false;
            }

            payment.PaymentStatus = dto.PaymentStatus;

            await _paymentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            Payment? payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                return false;
            }

            payment.PaymentStatus = "Cancelled";
            await _paymentRepository.SaveChangesAsync();

            return true;
        }

        private static PaymentDto MapPaymentToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                UserId = payment.Order?.UserId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                PaymentDate = payment.PaymentDate
            };
        }
    }
}
