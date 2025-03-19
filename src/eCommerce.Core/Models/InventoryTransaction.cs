using eCommerce.Core.Enums;
using System;

namespace eCommerce.Core.Models
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public TransactionType Type { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string CreatedBy { get; set; }
        public string ProcessedBy { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string Location { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public int? RelatedOrderId { get; set; }
        public int? RelatedReturnId { get; set; }
        public int? RelatedAdjustmentId { get; set; }
        public int? RelatedTransferId { get; set; }
        public int? RelatedDamageId { get; set; }
        public int? RelatedLossId { get; set; }
        public string RelatedDocumentNumber { get; set; }
        public string RelatedDocumentType { get; set; }
        public string RelatedDocumentStatus { get; set; }
        public string RelatedDocumentNotes { get; set; }
        public string RelatedDocumentCreatedBy { get; set; }
        public DateTime? RelatedDocumentCreatedAt { get; set; }
        public string RelatedDocumentProcessedBy { get; set; }
        public int InventoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime Date { get; set; }
        public virtual Inventory Inventory { get; set; }
    }
}
