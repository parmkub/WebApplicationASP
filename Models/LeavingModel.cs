using System.ComponentModel.DataAnnotations;

namespace WebApplicationASP.Models
{
    public class LeavingModel
    {
        [Display(Name = "เลขที่เอกสาร")]
        public string absence_document { get; set; } = "";
        
        [Display(Name = "วันเริ่ม-สิ้นสุด")]
        public string absence_date { get; set; } = "";


        [Display(Name = "รหัสพนักงาน")]
        public string employee_code { get; set; } = "";
        [Display(Name = "ประเภทการลา")]
        public string absence_code { get; set; }  = "";
        [Display(Name = "ที่เริ่ม-สิ้นสุด")]
        public string absence_day { get; set; } = "0";

        [Display(Name = "วันที่ลา")]
        public string absence_day_start { get; set; } = "";

        [Display(Name = "จำนวนชั่วโมง")]
        public double absence_hour { get; set; } = 0;
        public int absence_status { get; set; } = 0;
        [Display(Name = "รายละเอียดการลา")]
        public string absence_detail { get; set; } = "0";
        public string absence_token { get; set; } = "0";
        public string dayCount { get; set; } = "0";
        [Display(Name ="กรุณาทาบบัตรพนักงาน")]
        public string card_row { get; set; } = "0";

        public string sect_code { get; set; } = "0";
        public string divi_code { get; set; } = "0";
        public string depart_code { get; set; } = "0";


    }
}
