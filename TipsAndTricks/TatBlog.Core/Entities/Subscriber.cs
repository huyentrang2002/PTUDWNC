using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.Entities
{
    /*
    3a.Tạo lớp Subscriber để lưu trữ Email của người đăng ký, ngày đăng ký, 
    ngày hủy theo dõi, lý do hủy, trường cờ báo cho biết người dùng tự hủy
    theo dõi hay bị người quản trị ngăn chặn và ghi chú của người quản trị website.

    */
    public class Subscriber
    {
        public int Id { get; set; }
        public string SubEmail { get; set; }
        public DateTime SubDated { get; set; }
        public DateTime UnSubDated { get; set; }
        public string Cancel { get; set; }
        public bool Block { get; set; }
        public string AdNotes { get; set; }
    }
}
