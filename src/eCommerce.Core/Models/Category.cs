using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace eCommerce.Core.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Category Parent { get; set; }

        public virtual ICollection<Category> Children { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public new bool IsActive { get; set; } = true;

        [NotMapped]
        public int ProductCount { get; set; }

        public Category()
        {
            Children = new HashSet<Category>();
            Products = new HashSet<Product>();
        }
    }
}