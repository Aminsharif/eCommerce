using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Admin
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int? ParentCategoryId { get; set; }

        public int ProductCount { get; set; }
    }
}