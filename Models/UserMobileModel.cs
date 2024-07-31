using System.ComponentModel;

namespace WebApplicationASP.Models
{
    public class UserMobileModel
    {
        [DisplayName("รหัสพนักงาน")]
        public string EMPCODE { get; set; } = "";
        public string NAME { get; set; } = "";
        public string EMAIL { get; set; } = "";
        public string CREATE_DATE { get; set; } = "";
    }
}
