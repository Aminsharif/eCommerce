using eCommerce.Core.DTOs.Admin;
using eCommerce.Core.DTOs.Vendor;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.Extensions.Logging;

namespace eCommerce.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;

        public async Task<int> GetTotalOrdersCountAsync()
        {
            try
            {
                return await _orderRepository.CountAsync(o => true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total orders count");
                throw;
            }
        }


        public async Task<List<AdminDashboardDto.RecentOrder>> GetRecentOrdersAsync(int count)
        {
            try
            {
                var recentOrders = await _orderRepository.GetRecentOrdersAsync(count);
                return recentOrders.Select(o => new AdminDashboardDto.RecentOrder
                {
                    OrderNumber = o.OrderNumber,
                    OrderDate = o.CreatedAt,
                    CustomerName = $"{o.User.FirstName} {o.User.LastName}",
                    TotalAmount = o.Total,
                    Status = o.Status.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent orders");
                throw;
            }
        }

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<Order> GetOrderByNumber(string orderNumber)
        {
            return await _orderRepository.GetOrderByOrderNumber(orderNumber);
        }

        public async Task<IEnumerable<Order>> GetUserOrders(int userId)
        {
            return await _orderRepository.GetOrdersByUser(userId);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItems(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            return order?.Items ?? new List<OrderItem>();
        }

        public async Task<Order> CreateOrder(Order order)
        {
            try
            {
                if (!await ValidateOrder(order))
                {
                    throw new InvalidOperationException("Order validation failed");
                }

                // Generate order number
                order.OrderNumber = GenerateOrderNumber();
                order.OrderDate = DateTime.UtcNow;
                order.Status = OrderStatus.Pending;
                order.PaymentStatus = PaymentStatus.Pending;

                // Calculate order total
                var total = await CalculateOrderTotal(order.Items);
                order.SubTotal = total;
                order.Tax = total * 0.1m; // 10% tax
                order.ShippingCost = 10.0m; // Fixed shipping cost
                order.DiscountAmount = 0; // No discount by default
                order.Total = order.SubTotal + order.Tax + order.ShippingCost - order.DiscountAmount;

                // Create order
                var createdOrder = await _orderRepository.AddAsync(order);

                // Update stock quantities
                foreach (var item in order.Items)
                {
                    await _productRepository.UpdateStockQuantity(
                        item.ProductId,
                        (await _productRepository.GetByIdAsync(item.ProductId)).StockQuantity - item.Quantity
                    );
                }

                return createdOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<bool> ValidateOrder(Order order)
        {
            if (order == null || order.Items == null || !order.Items.Any())
            {
                return false;
            }

            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            try
            {
                await _orderRepository.UpdateOrderStatus(orderId, status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status for order {orderId}");
                return false;
            }
        }

        public async Task<bool> CancelOrder(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null || order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
                {
                    return false;
                }

                // Restore stock quantities
                foreach (var item in order.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    await _productRepository.UpdateStockQuantity(
                        item.ProductId,
                        product.StockQuantity + item.Quantity
                    );
                }

                await _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Cancelled);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {orderId}");
                return false;
            }
        }

        public async Task<bool> ProcessPayment(int orderId, PaymentMethodType paymentMethod, string transactionId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null || order.PaymentStatus == PaymentStatus.Paid)
                {
                    return false;
                }

                await _orderRepository.UpdatePaymentStatus(orderId, PaymentStatus.Paid);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment for order {orderId}");
                return false;
            }
        }

        public async Task<bool> UpdateShippingInfo(int orderId, string trackingNumber)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return false;
                }

                // Update shipping info logic here
                await _orderRepository.UpdateOrderStatus(orderId, OrderStatus.Shipped);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating shipping info for order {orderId}");
                return false;
            }
        }

        public async Task<decimal> CalculateOrderTotal(IEnumerable<OrderItem> items)
        {
            decimal total = 0;
            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                item.UnitPrice = product.Price;
                item.TotalPrice = item.UnitPrice * item.Quantity;
                total += item.TotalPrice;
            }
            return total;
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status)
        {
            return await _orderRepository.GetOrdersByStatus(status);
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate);
        }

        public async Task<decimal> GetTotalRevenue(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _orderRepository.GetTotalRevenue(startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting total revenue for date range {startDate} to {endDate}");
                return 0;
            }
        }

        public async Task<int> GetTotalOrders(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _orderRepository.GetTotalOrders(startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting total orders for date range {startDate} to {endDate}");
                return 0;
            }
        }

        public async Task<Dictionary<string, decimal>> GetSalesReport(DateTime startDate, DateTime endDate)
        {
            var orders = await GetOrdersByDateRange(startDate, endDate);
            var totalRevenue = await GetTotalRevenue(startDate, endDate);
            var totalOrders = await GetTotalOrders(startDate, endDate);

            return new Dictionary<string, decimal>
            {
                { "TotalRevenue", totalRevenue },
                { "AverageOrderValue", totalOrders > 0 ? totalRevenue / totalOrders : 0 },
                { "TotalOrders", totalOrders }
            };
        }

        public async Task<bool> UpdatePaymentStatus(int orderId, PaymentStatus paymentStatus)
        {
            try
            {
                await _orderRepository.UpdatePaymentStatus(orderId, paymentStatus);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating payment status for order {orderId}");
                return false;
            }
        }

        private string GenerateOrderNumber()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
        }

        public async Task<int> GetTotalOrdersCount()
        {
            return await _orderRepository.GetTotalCountAsync();
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            try
            {
                return await _orderRepository.CountAsync(o => o.Status == OrderStatus.Pending);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending orders count");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetOrders(int page, int pageSize)
        {
            return await _orderRepository.GetOrdersAsync(page, pageSize);
        }

        public async Task<Analytics> GetOrderAnalytics(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);

            var analytics = new Analytics
            {
                TotalRevenue = orders.Sum(o => o.Total),
                TotalOrders = orders.Count(),
                TotalCustomers = orders.Select(o => o.UserId).Distinct().Count(),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.Total) : 0,
                ConversionRate = 0, // TODO: Implement conversion rate calculation
                TotalCost = orders.Sum(o => o.Items.Sum(i => i.Product.Cost * i.Quantity)),
                GrossProfit = orders.Sum(o => o.Total) - orders.Sum(o => o.Items.Sum(i => i.Product.Cost * i.Quantity)),
                NetProfit = orders.Sum(o => o.Total) - orders.Sum(o => o.Items.Sum(i => i.Product.Cost * i.Quantity)) - orders.Sum(o => o.ShippingCost),
                ProfitMargin = orders.Any() ? (orders.Sum(o => o.Total) - orders.Sum(o => o.Items.Sum(i => i.Product.Cost * i.Quantity))) / orders.Sum(o => o.Total) : 0,
                TotalProducts = orders.Sum(o => o.Items.Count),
                LowStockProductCount = 0, // TODO: Implement low stock count
                AverageProductPrice = orders.Any() ? orders.Average(o => o.Items.Average(i => i.UnitPrice)) : 0,
                AverageProductCost = orders.Any() ? orders.Average(o => o.Items.Average(i => i.Product.Cost)) : 0,
                InventoryValue = 0, // TODO: Implement inventory value calculation
                InventoryTurnover = 0, // TODO: Implement inventory turnover calculation
                DaysOfInventory = 0, // TODO: Implement days of inventory calculation
                TopSellingProducts = new List<Product>(),
                TopViewedProducts = new List<Product>(),
                LowStockProducts = new List<Product>(),
                CategoryInventory = new List<CategoryInventory>()
            };

            return analytics;
        }

        public async Task<DailySales> GetDailySalesAsync(DateTime date)
        {
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(date.Date, date.Date.AddDays(1).AddSeconds(-1));
            
            if (!orders.Any())
                return new DailySales 
                { 
                    Date = date,
                    Category = "All",
                    OrderCount = 0,
                    TotalRevenue = 0,
                    AverageOrderValue = 0,
                    ItemsSold = 0,
                    ConversionRate = 0,
                    RefundRate = 0,
                    ProfitMargin = 0,
                    GrossProfit = 0,
                    NetProfit = 0,
                    TotalCost = 0,
                    AverageItemPrice = 0,
                    AverageItemCost = 0,
                    UniqueCustomers = 0,
                    NewCustomers = 0,
                    ReturningCustomers = 0,
                    CustomerRetentionRate = 0,
                    CustomerAcquisitionCost = 0,
                    CustomerLifetimeValue = 0
                };

            var totalRevenue = orders.Sum(o => o.Total);
            var totalItems = orders.Sum(o => o.Items.Sum(i => i.Quantity));
            var uniqueCustomers = orders.Select(o => o.UserId).Distinct().Count();
            var refundedOrders = orders.Count(o => o.Status == OrderStatus.Refunded);

            return new DailySales
            {
                Date = date,
                Category = "All",
                OrderCount = orders.Count(),
                TotalRevenue = totalRevenue,
                AverageOrderValue = orders.Any() ? totalRevenue / orders.Count() : 0,
                ItemsSold = totalItems,
                ConversionRate = 0, // Requires visitor data
                RefundRate = orders.Any() ? (decimal)refundedOrders / orders.Count() * 100 : 0,
                ProfitMargin = 0, // Requires cost data
                GrossProfit = 0, // Requires cost data
                NetProfit = 0, // Requires cost data
                TotalCost = 0, // Requires cost data
                AverageItemPrice = totalItems > 0 ? totalRevenue / totalItems : 0,
                AverageItemCost = 0, // Requires cost data
                UniqueCustomers = uniqueCustomers,
                NewCustomers = 0, // Requires historical data
                ReturningCustomers = 0, // Requires historical data
                CustomerRetentionRate = 0, // Requires historical data
                CustomerAcquisitionCost = 0, // Requires marketing data
                CustomerLifetimeValue = 0 // Requires historical data
            };
        }

        public async Task<ICollection<CategorySales>> GetCategorySalesAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
            var categorySales = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => i.Product.Category.Name)
                .Select(g => new CategorySales
                {
                    Category = g.Key,
                    TotalRevenue = g.Sum(i => i.UnitPrice * i.Quantity),
                    TotalOrders = g.Select(i => i.OrderId).Distinct().Count(),
                    TotalItems = g.Sum(i => i.Quantity),
                    AverageOrderValue = g.Select(i => i.OrderId).Distinct().Count() > 0
                        ? g.Sum(i => i.UnitPrice * i.Quantity) / g.Select(i => i.OrderId).Distinct().Count()
                        : 0,
                    TotalCost = g.Sum(i => i.Product.Cost * i.Quantity),
                    GrossProfit = g.Sum(i => i.UnitPrice * i.Quantity) - g.Sum(i => i.Product.Cost * i.Quantity),
                    NetProfit = g.Sum(i => i.UnitPrice * i.Quantity) - g.Sum(i => i.Product.Cost * i.Quantity),
                    ProfitMargin = g.Sum(i => i.UnitPrice * i.Quantity) > 0 
                        ? (g.Sum(i => i.UnitPrice * i.Quantity) - g.Sum(i => i.Product.Cost * i.Quantity)) / g.Sum(i => i.UnitPrice * i.Quantity)
                        : 0,
                    ConversionRate = 0, // TODO: Implement conversion rate calculation
                    RefundRate = 0, // TODO: Implement refund rate calculation
                    UniqueCustomers = g.Select(i => i.Order.UserId).Distinct().Count(),
                    CustomerRetentionRate = 0, // TODO: Implement customer retention rate calculation
                    MarketShare = 0, // TODO: Implement market share calculation
                    YearOverYearGrowth = 0, // TODO: Implement year over year growth calculation
                    StockLevel = 0, // TODO: Implement stock level calculation
                    InventoryTurnover = 0, // TODO: Implement inventory turnover calculation
                    DaysOfInventory = 0 // TODO: Implement days of inventory calculation
                })
                .ToList();

            return categorySales;
        }

        public async Task<ICollection<PaymentMethodStats>> GetPaymentMethodStatsAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
            var paymentMethodStats = orders
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new PaymentMethodStats
                {
                    Method = g.Key,
                    TotalTransactions = g.Count(),
                    TotalAmount = g.Sum(o => o.Total),
                    AverageTransactionAmount = g.Any() ? g.Average(o => o.Total) : 0,
                    RefundRate = 0, // TODO: Implement refund rate calculation
                    ChargebackRate = 0, // TODO: Implement chargeback rate calculation
                    FailureRate = g.Count(o => o.PaymentStatus == PaymentStatus.Failed) / (decimal)g.Count(),
                    ProcessingFees = 0, // TODO: Implement processing fees calculation
                    NetRevenue = g.Sum(o => o.Total), // TODO: Subtract processing fees and refunds
                    UsagePercentage = (decimal)g.Count() / orders.Count(),
                    ConversionRate = 0, // TODO: Implement conversion rate calculation
                    CustomerPreference = 0, // TODO: Implement customer preference calculation
                    FraudRate = 0, // TODO: Implement fraud rate calculation
                    AverageProcessingTime = 0, // TODO: Implement average processing time calculation
                    SuccessRate = g.Count(o => o.PaymentStatus == PaymentStatus.Paid) / (decimal)g.Count()
                })
                .ToList();

            return paymentMethodStats;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (!await ValidateOrder(order))
            {
                throw new InvalidOperationException("Order validation failed");
            }

            return await CreateOrder(order);
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found");
            }

            order.Status = status;
            if (status == OrderStatus.Shipped)
            {
                order.ShippedAt = DateTime.UtcNow;
            }
            else if (status == OrderStatus.Delivered)
            {
                order.DeliveredAt = DateTime.UtcNow;
            }

            await _orderRepository.UpdateAsync(order);
            return order;
        }

        public async Task<Order> UpdatePaymentStatusAsync(int orderId, PaymentStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found");
            }

            order.PaymentStatus = status;
            await _orderRepository.UpdateAsync(order);
            return order;
        }

        public async Task<Analytics> GetOrderAnalyticsAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);

            var analytics = new Analytics
            {
                TotalRevenue = orders.Sum(o => o.Total),
                TotalOrders = orders.Count(),
                TotalCustomers = orders.Select(o => o.UserId).Distinct().Count(),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.Total) : 0,
                ConversionRate = 0, // TODO: Implement conversion rate calculation
                TotalCost = orders.Sum(o => o.Items.Sum(i => i.Product.Cost * i.Quantity)),
                GrossProfit = orders.Sum(o => o.Total) - orders.Sum(o => o.Items.Sum(i => i.Product.Cost * i.Quantity)),
                NetProfit = orders.Sum(o => o.Total) - orders.Sum(o => o.Items.Sum(i => i.Product.Cost * i.Quantity)) - orders.Sum(o => o.ShippingCost),
                ProfitMargin = orders.Any() ? (orders.Sum(o => o.Total) - orders.Sum(o => o.Items.Sum(i => i.Product.Cost * i.Quantity))) / orders.Sum(o => o.Total) : 0,
                TotalProducts = orders.Sum(o => o.Items.Count),
                LowStockProductCount = 0, // TODO: Implement low stock count
                AverageProductPrice = orders.Any() ? orders.Average(o => o.Items.Average(i => i.UnitPrice)) : 0,
                AverageProductCost = orders.Any() ? orders.Average(o => o.Items.Average(i => i.Product.Cost)) : 0,
                InventoryValue = 0, // TODO: Implement inventory value calculation
                InventoryTurnover = 0, // TODO: Implement inventory turnover calculation
                DaysOfInventory = 0, // TODO: Implement days of inventory calculation
                TopSellingProducts = new List<Product>(),
                TopViewedProducts = new List<Product>(),
                LowStockProducts = new List<Product>(),
                CategoryInventory = new List<CategoryInventory>()
            };

            return analytics;
        }

        Task<int> IOrderService.GetVendorOrdersCountAsync(int vendorId)
        {
            throw new NotImplementedException();
        }

        Task<int> IOrderService.GetVendorPendingOrdersCountAsync(int vendorId)
        {
            throw new NotImplementedException();
        }

        Task<List<VendorDashboardDto.RecentOrder>> IOrderService.GetVendorRecentOrdersAsync(int vendorId, int count)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Order>> IOrderService.GetRecentOrders(int count)
        {
            throw new NotImplementedException();
        }
    }
}