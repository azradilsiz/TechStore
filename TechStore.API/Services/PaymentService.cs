using Microsoft.EntityFrameworkCore;
using TechStore.API.Data;
using TechStore.API.DTOs.Payments;
using TechStore.API.Entities;

namespace TechStore.API.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Select(payment => new PaymentDto
                {
                    Id = payment.Id,
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentStatus = payment.PaymentStatus,
                    PaymentDate = payment.PaymentDate
                })
                .ToListAsync();
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Where(payment => payment.Id == id)
                .Select(payment => new PaymentDto
                {
                    Id = payment.Id,
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentStatus = payment.PaymentStatus,
                    PaymentDate = payment.PaymentDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PaymentDto?> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);

            if (order == null)
            {
                return null;
            }

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Pending",
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                PaymentDate = payment.PaymentDate
            };
        }

        public async Task<bool> UpdatePaymentAsync(int id, UpdatePaymentDto dto)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
            {
                return false;
            }

            payment.PaymentStatus = dto.PaymentStatus;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
            {
                return false;
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}