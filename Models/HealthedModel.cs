using System.ComponentModel.DataAnnotations;

namespace WebApplicationASP.Models
{
    public class HealthedModel
    {
        [Display(Name = "รหัสบัตร")]
        public string ID_PERSONAL_HEALTHY { get; set; } = "";
        [Display(Name = "ชื่อ-นามสกุล")]
        public string NAME { get; set; } = "";
        [Display(Name = "ตำแหน่ง")]
        public string POSITION { get; set; } = "";
        [Display(Name = "สร้างโดย")]
        public string CREATE_BY { get; set; } = "";
        [Display(Name = "วันที่สร้าง")]
            
        public string CREATE_DATE { get; set; } = "";
        [Display(Name = "แก้ไขล่าสุดโดย")]
        public string LAST_UPDATE_DATE { get; set; } = "";
        [Display(Name = "วันที่แก้ไขล่าสุด")]
        public string LAST_UPDATE_BY { get; set; } = "";

         

    }
}
