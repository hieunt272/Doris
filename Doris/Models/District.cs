using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Doris.Models
{
    public class District
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập tên Quận/Huyện"), Display(Name = "Tên Quận/Huyện"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Name { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [StringLength(20)]
        public string Prefix { get; set; }

        public int CityId { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Ward> Wards { get; set; }
    }
}