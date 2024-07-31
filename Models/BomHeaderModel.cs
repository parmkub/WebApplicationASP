using System.ComponentModel.DataAnnotations;

namespace WebApplicationASP.Models
{
	public class BomHeaderModel
	{
		[Key]
		[Display(Name = "BOM_ID")]
		public int BOM_ID { get; set; } = 0;


		[Display(Name = "ITEM_CODE")]
		public string ITEM_CODE { get; set; } = "";


		[Display(Name = "INVENTORY_ITEM_ID")]
		public int INVENTORY_ITEM_ID { get; set; } = 0;


		[Display(Name = "APPROVE_PATH_ID")]
		public int APPROVE_PATH_ID { get; set; } = 0;


		[Display(Name = "APPROVE_FLAG")]
		public string APPROVE_FLAG { get; set; } = "";


		[Display(Name = "INTERFACE_FLAG")]
		public string INTERFACE_FLAG { get; set; } = "";


		[Display(Name = "CREATION_DATE")]
		public DateTime CREATION_DATE { get; set; } = DateTime.Now;


		[Display(Name = "CREATED_BY")]
		public string CREATED_BY { get; set; } = "";

		[Display(Name = "LAST_UPDATE_DATE")]
		public DateTime LAST_UPDATE_DATE { get; set; } = DateTime.Now;


		[Display(Name = "LAST_UPDATED_BY")]
		public string LAST_UPDATED_BY { get; set; } = "";
	}
}
