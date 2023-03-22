using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailTemplate.Models.LoginAndLogoutPage
{
    public class ReturnMsg
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string Auth001Id { get; set; }
        public string UserName { get; set; }
    }
}
