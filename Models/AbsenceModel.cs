using System.ComponentModel;

namespace WebApplicationASP.Models
{
    public class AbsenceModel
    {
        [DisplayName("เลขที่เอกสาร")]
        public string ABSENCE_DOCUMENT { get; set; } = "";
        [DisplayName("วันที่ลา")]
        public string ABSENCE_DATE { get; set; } = "";

        public int editStatus_date { get; set; } = 0;    

        [DisplayName("แก้วันที่ลา")]
        public DateTime ABSENCE_DATE_NEW { get; set; } = DateTime.Now;

        [DisplayName("รหัสพนักงาน")]
        public string EMPLOYEE_CODE { get; set; } = "";
        [DisplayName("Code")]
        public string ABSENCE_CODE { get; set; } = "";
        [DisplayName("จำนวนวันลา")]
        public string ABSENCE_DAY { get; set; } = "";
        [DisplayName("จำนวนชั่วโมง")]
        public string ABSENCE_HOUR { get; set; } = "";
        [DisplayName("ผู้ทบทวน")]
        public string ABSENCE_REVIEW { get; set; } = "";
        [DisplayName("ผู้อนุมัติ")]
        public string ABSENCE_APPROVE { get; set; } = "";
        [DisplayName("สถานะ")]
        public string ABSENCE_STATUS { get; set; } = "";
    }
}
