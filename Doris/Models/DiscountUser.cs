using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doris.Models
{
    public class DiscountUser
    {
        [Key, Column(Order = 1)]
        public int DiscountId { get; set; }
        [Key, Column(Order = 2)]
        public int UserId { get; set; }
        public bool Active { get; set; }
        public virtual User User { get; set; }
        public virtual Discount Discount { get; set; }

        public DiscountUser() { 
            Active = true;
        }
    }
}