using System.ComponentModel.DataAnnotations;

namespace WebApplicationASP.Models
{
	public class HealthyModel
	{
		[Key]
		[Display(Name = "ID")]
		public int ID { get; set; } = 0;
		
		public string CREATE_DATE { get; set; } = "";
		public string LAST_UPDATE_DATE { get; set; } = "";

		 public string CREATE_BY { get; set; } = "";
		public string LAST_UPDATE_BY { get; set; } = "";

		[Display(Name = "เลขที่เอกสาร")]
		public string DOCUMENT_ID { get; set; } = "";
		[Display(Name = "สถานะการตรวจ")]
		public string STATUS_HEALTHY { get; set; } = "";
		[Display(Name = "รายละเอียดการตรวจ")]
		[Required(ErrorMessage = "กรุณากรอกรายละเอียดการตรวจ")]
		public string DETAIL { get; set; } = "";

		[Display(Name = "บัตรประจำตัวแพทย์/พยาบาล")]
		[Required(ErrorMessage ="กรุณาทาบบัคร")]
		public string ID_PERSONAL_HEALTHY { get; set; } = "";

		[Display(Name = "ชื่อแพทย์/พยาบาล")]
		public string NAME_PERSONAL_HEALTHY { get; set; } = "";
	}
}
