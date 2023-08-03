using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doris.Models
{
    public class Bank
    {
        public int Id { get; set; }
        [Display(Name = "Tên ngân hàng"), Required(ErrorMessage = "Bạn chưa nhập tên ngân hàng"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [StringLength(500), Display(Name = "Hình ảnh")]
        public string Image { get; set; }
        [Display(Name = "Số thứ tự"), Required(ErrorMessage = "Bạn chưa nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        public virtual ICollection<BankUser> BankUsers { get; set; }
    }

    public class BankUser
    {
        [Key, Column(Order = 1)]
        public int UserId { get; set; }
        [Key, Column(Order = 2)]
        public int BankId { get; set; }
        [Display(Name = "Số tài khoản"), Required(ErrorMessage = "Bạn chưa nhập số tài khoản"), UIHint("TextBox"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số")]
        public string Name { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox"), Display(Name = "Chi nhánh")]
        public string Brand { get; set; }
        [Display(Name = "Số thứ tự"), Required(ErrorMessage = "Bạn chưa nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        public virtual User User { get; set; }
        public virtual Bank Bank { get; set; }
        [Display(Name = "Chủ tài khoản"), Required(ErrorMessage = "Bạn chưa nhập chủ tài khoản"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string AccountName { get; set; }
        public bool Default { get; set; }

        public BankUser()
        {
            Default = true;
            Active = true;
        }
    }
}