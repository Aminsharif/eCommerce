using eCommerce.Core.Enums;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.Extensions.Logging;

namespace eCommerce.Infrastructure.Services
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICommissionRepository _commissionRepository;
        private readonly ILogger<VendorService> _logger;

        public async Task<int> GetTotalVendorsCountAsync()
        {
            try
            {
                return await _vendorRepository.CountAsync(v => true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total vendors count");
                throw;
            }
        }

        public VendorService(
            IVendorRepository vendorRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IReviewRepository reviewRepository,
            ICommissionRepository commissionRepository)
        {
            _vendorRepository = vendorRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _reviewRepository = reviewRepository;
            _commissionRepository = commissionRepository;
        }

        public async Task<Vendor> GetVendorById(int id)
        {
            return await _vendorRepository.GetByIdAsync(id);
        }

        public async Task<Vendor> GetVendorByUserId(int userId)
        {
            return await _vendorRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Vendor>> GetAllVendors(int page, int pageSize)
        {
            return await _vendorRepository.GetAllAsync();
        }

        public async Task<Vendor> CreateVendor(Vendor vendor)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(vendor.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {vendor.UserId} not found");
            }

            // Set initial status
            vendor.VendorStatus = VendorStatus.Active;
            vendor.CreatedAt = DateTime.UtcNow;
            vendor.UpdatedAt = DateTime.UtcNow;

            return await _vendorRepository.AddAsync(vendor);
        }

        public async Task<Vendor> UpdateVendor(Vendor vendor)
        {
            var existingVendor = await _vendorRepository.GetByIdAsync(vendor.Id);
            if (existingVendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {vendor.Id} not found");
            }

            // Update only allowed fields
            existingVendor.Name = vendor.Name;
            existingVendor.CompanyName = vendor.CompanyName;
            existingVendor.Description = vendor.Description;
            existingVendor.Email = vendor.Email;
            existingVendor.Phone = vendor.Phone;
            existingVendor.Address = vendor.Address;
            existingVendor.VendorStatus = VendorStatus.Active;
            existingVendor.UpdatedAt = DateTime.UtcNow;

            return await _vendorRepository.UpdateAsync(existingVendor);
        }

        public async Task<bool> DeactivateVendor(int id)
        {
            var vendor = await _vendorRepository.GetByIdAsync(id);
            if (vendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {id} not found");
            }

            vendor.VendorStatus = VendorStatus.Inactive;
            vendor.UpdatedAt = DateTime.UtcNow;

            await _vendorRepository.UpdateAsync(vendor);
            return true;
        }

        public async Task<bool> DeleteVendor(int id)
        {
            var vendor = await _vendorRepository.GetByIdAsync(id);
            if (vendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {id} not found");
            }

            await _vendorRepository.DeleteAsync(vendor);
            return true ;
        }

        public async Task<IEnumerable<Product>> GetVendorProducts(int vendorId, int page, int pageSize)
        {
            return await _productRepository.GetByVendorIdAsync(vendorId);
        }

        public async Task<IEnumerable<Order>> GetVendorOrders(int vendorId, int page, int pageSize)
        {
            return await _vendorRepository.GetVendorOrdersAsync(vendorId, page, pageSize);
        }

        public async Task<decimal> GetVendorRevenue(int vendorId, DateTime startDate, DateTime endDate)
        {
            return await _vendorRepository.GetVendorRevenueAsync(vendorId, startDate, endDate);

        }

        public async Task<IEnumerable<Commission>> GetVendorCommissions(int vendorId, int page, int pageSize)
        {
            return await _vendorRepository.GetCommissionsAsync(vendorId);
        }

        public async Task<Commission> CalculateCommission(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found");
            }

            // Get the vendor ID from the first order item
            var vendorId = order.Items.FirstOrDefault()?.Product.VendorId;
            if (!vendorId.HasValue)
            {
                throw new InvalidOperationException($"Order {orderId} has no items with associated vendors");
            }
            var vendor = await _vendorRepository.GetByIdAsync(order.UserId);
            if (vendor == null)
            {
                throw new KeyNotFoundException($"vendor with ID {order.UserId} not found");
            }

            // Calculate commission based on order total
            var commissionAmount = order.Total * 0.1m; // 10% commission

            var commission = new Commission
            {
                OrderId = orderId,
                VendorId = vendorId.Value,
                CommissionAmount = commissionAmount,
                Status = CommissionStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Order = order,
                Vendor = vendor,
            };

            return await _commissionRepository.AddCommissionAsync(commission);
        }

        public async Task<bool> MarkCommissionAsPaid(int commissionId)
        {
            var commission = await _vendorRepository.GetCommissionByIdAsync(commissionId);
            if (commission == null)
            {
                throw new KeyNotFoundException($"Commission with ID {commissionId} not found");
            }

            commission.Status = CommissionStatus.Paid;
            commission.PaidAt = DateTime.UtcNow;

            await _vendorRepository.UpdateCommissionAsync(commission);
            return true;
        }

        public async Task<VendorAnalytics> GetVendorAnalytics(int vendorId, DateTime startDate, DateTime endDate)
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {vendorId} not found");
            }

            var orders = await _commissionRepository.GetByVendorIdAsync(vendorId);
            var products = await _productRepository.GetByVendorIdAsync(vendorId);
            var commissions = await _vendorRepository.GetCommissionsAsync(vendorId);

            var analytics = new VendorAnalytics
            {
                VendorId = vendorId,
                StartDate = startDate,
                EndDate = endDate,
                TotalOrders = orders.Count(),
                TotalProducts = products.Count(),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.CommissionAmount) : 0,
                CommissionEarned = commissions.Sum(c => c.CommissionAmount),
                CommissionsPaid = commissions.Where(c => c.Status == CommissionStatus.Paid).Sum(c => c.CommissionAmount),
                PendingCommissions = commissions.Where(c => c.Status == CommissionStatus.Pending).Sum(c => c.CommissionAmount),
            };

            return analytics;
        }

        public async Task<VendorReview> AddVendorReview(VendorReview review)
        {
            var vendor = await _vendorRepository.GetByIdAsync(review.VendorId);
            if (vendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {review.VendorId} not found");
            }

            review.CreatedAt = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;

            return await _vendorRepository.AddVendorReview(review);
        }

        public async Task<VendorReview> UpdateVendorReview(VendorReview review)
        {
            var existingReview = await _vendorRepository.GetReviewByIdAsync(review.Id);
            if (existingReview == null)
            {
                throw new KeyNotFoundException($"Review with ID {review.Id} not found");
            }

            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
            existingReview.UpdatedAt = DateTime.UtcNow;

            return await _vendorRepository.UpdateReviewAsync(existingReview);
        }

        public async Task<bool> DeleteVendorReview(int reviewId)
        {
            var review = await _vendorRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {reviewId} not found");
            }

            await _vendorRepository.DeleteReview(reviewId);
            return true;
        }

        Task<decimal> IVendorService.GetVendorAverageRatingAsync(int vendorId)
        {
            throw new NotImplementedException();
        }
    }
}