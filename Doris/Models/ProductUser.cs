using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Doris.Models
{
    public class ProductUser
    {
        [Key, Column(Order = 1)]
        public int ProductId { get; set; }
        [Key, Column(Order = 2)]
        public int UserId { get; set; }
        public decimal? UserPrice { get; set; }
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}