using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebsiteBanHang.Models
{
    public class LoginViewModel
    {
        public string EmailOrUsername { get; set; }
        public string Password { get; set; }
    }
}