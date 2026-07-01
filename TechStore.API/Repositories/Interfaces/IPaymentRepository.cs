using TechStore.API.Entities;

namespace TechStore.API.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllAsync();

        Task<Payment?> GetByIdAsync(int id);

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task AddAsync(Payment payment);

        void Delete(Payment payment);

        Task SaveChangesAsync();
    }
}
