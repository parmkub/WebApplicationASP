using System.ComponentModel.DataAnnotations;

namespace WebApplicationASP.Models
{
    public class LeavingViewModel
    {

        [Display(Name = "วันลา")]
        public string START_DATE { get; set; } = "";
        [Display(Name = "วันที่สิ้นสุด")]
        public string END_DATE { get; set; } = "";
        [Display(Name = "จำนวนวันลา")]
        public string COUNT_DATE { get; set; } = "";
        [Display(Name = "ชื่อ-นามสกุล")]
        public string NAME { get; set; } = "";
        [Display(Name = "รหัสพนักงาน")]
        public string EMPLOYEE_CODE { get; set; } = "";
        [Display(Name = "ประเภทการลา")]
        public string ABSENCE_CODE { get; set; } = "";
        [Display(Name = "จำนวน/วัน")]
        public string ABSENCE_DAY { get; set; } = "";
        [Display(Name = "จำนวน/ชั่วโมง")]
        public string ABSENCE_HOUR { get; set; } = "";
        public string DELETE_MARK { get; set; } = "";
        public string REVIEW { get; set; } = "";
        public string APPROVE { get; set; } = "";
        public string ABSENCE_PERIOD { get; set; } = "";
        public string ABSENCE_STATUS { get; set; } = "";
        public string ABSENCE_TOKEN { get; set; } = "";

        [Display(Name = "รายละเอียดการลา")]
        public string ABSENCE_DETAIL { get; set; } = "";
        [Display(Name = "เลขที่เอกสาร")]
        public string ABSENCE_DOCUMENT { get; set; } = "";
        public string CREATION_DATE { get; set; } = "";
        public string STATUS_APPROVE { get; set; } = "";

        [Display(Name = "ส่วน")]
        public string DIVI_CODE { get; set; } = "";
        [Display(Name = "ฝ่าย")]
        public string DEPART_CODE { get; set; } = "";
        [Display(Name = "แผนก")]
        public string SECT_CODE { get; set; } = "";

        public string POSITION_NAME { get; set; } = "";

        public string DATE_REVIEW { get; set; } = "";
        public string DATE_APPROVE { get; set; } = "";


    }
}
