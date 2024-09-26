using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstMVCtest.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Quantity { get; set; }
        public double? Price { get; set; }
        public double? Value { get; set; }
        public string? PurchaseDate { get; set; }
        public string? Condition { get; set; }
        public string? Origin { get; set; }
        public bool? ForSale { get; set; }
        public bool? ForTrade { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();

    }
}
