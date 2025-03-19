using System;
using System.Collections.Generic;
using eCommerce.Core.Enums;

namespace eCommerce.Core.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public int MinimumStockLevel { get; set; }
        public int MaximumStockLevel { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsActive { get; set; }
        public int ReorderPoint { get; set; }
        public int ReorderQuantity { get; set; }
        public string Location { get; set; }
        public string SKU { get; set; }
        public DateTime? LastRestocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<InventoryTransaction> Transactions { get; set; }

        public Inventory()
        {
            CreatedAt = DateTime.UtcNow;
            Quantity = 0;
            ReorderPoint = 10;
            ReorderQuantity = 20;
            Location = string.Empty;
            SKU = string.Empty;
            Transactions = new List<InventoryTransaction>();
        }
    }
} 