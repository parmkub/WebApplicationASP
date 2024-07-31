using System.ComponentModel.DataAnnotations;

namespace WebApplicationASP.Models
{
	public class ContectPersonalModel
	{
		[Display(Name = "ID")]
		public int Id { get; set; }
		[Display(Name = "ชื่อ")]

		public string First_name { get; set; }
		[Display(Name = "นามสกุล")]

		public string Last_name { get; set; }

		[Display(Name = "รหัสผู้ติดต่อ")]
		
        public string Employee_code { get; set; }
        [Display(Name = "อาชีพ")]
		public string career { get; set; }
		[Display(Name = "รหัสหลังบัตร")]
		public string Card_raw { get; set; }
		
	}

}
