﻿using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreAIML.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public long ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("ProductId")]
        public ProductViewModel Product { get; set; }
    }
}
