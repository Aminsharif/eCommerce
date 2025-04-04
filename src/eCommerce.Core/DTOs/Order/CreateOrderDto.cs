using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace eCommerce.Core.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();

        [Required]
        public int ShippingAddressId { get; set; }

        public string? Notes { get; set; }
    }

    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}