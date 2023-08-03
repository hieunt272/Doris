using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Doris.Models
{
    public class Address
    {
        public int Id { get; set; }
        [Display(Name = "Họ tên"), Required(ErrorMessage = "Hãy nhập họ tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Fullname { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
         Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string Mobile { get; set; }
        [Display(Name = "Địa chỉ cụ thể"), Required(ErrorMessage = "Hãy nhập địa chỉ cụ thể"), DataType(DataType.MultilineText), StringLength(4000)]
        public string SpecificAddress { get; set; }
        public bool Default { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public int UserId { get; set; }
        public virtual City City { get; set; }
        public virtual District District { get; set; }
        public virtual Ward Ward { get; set; }
        public virtual User User { get; set; }

        public Address()
        {
            Default = true;
        }
    }
}