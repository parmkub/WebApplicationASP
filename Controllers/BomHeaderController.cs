using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using WebApplicationASP.Models;

namespace WebApplicationASP.Controllers
{
	public class BomHeaderController : Controller
	
	{
		private readonly OracleConnection _connection;
		public BomHeaderController(OracleConnection connection)
		{
			_connection = connection;
		}
		public IActionResult Index()
		{
			List<BomHeaderModel> bomHeaderList = new List<BomHeaderModel>();
			OracleCommand cmd = _connection.CreateCommand();
			cmd.CommandText = "SELECT * FROM MMTCST_BOM_BOM_HEADER";
			_connection.Open();
			OracleDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				BomHeaderModel bomHeader = new BomHeaderModel();
				bomHeader.BOM_ID = Convert.ToInt32(reader["BOM_ID"]);
				bomHeader.ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]);
				bomHeader.INVENTORY_ITEM_ID = Convert.ToInt32(reader["INVENTORY_ITEM_ID"]);
				bomHeader.APPROVE_PATH_ID = Convert.ToInt32(reader["APPROVE_PATH_ID"]);
				bomHeader.APPROVE_FLAG = Convert.ToString(reader["APPROVE_FLAG"]);
				bomHeader.INTERFACE_FLAG = Convert.ToString(reader["INTERFACE_FLAG"]);
				bomHeader.CREATION_DATE = Convert.ToDateTime(reader["CREATION_DATE"]);
				bomHeader.CREATED_BY = Convert.ToString(reader["CREATED_BY"]);
				bomHeader.LAST_UPDATE_DATE = Convert.ToDateTime(reader["LAST_UPDATE_DATE"]);
				bomHeader.LAST_UPDATED_BY = Convert.ToString(reader["LAST_UPDATED_BY"]);
				bomHeaderList.Add(bomHeader);
			}	

			return View(bomHeaderList);
		}
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(BomHeaderModel bomHeaderModel)
		{
			using (OracleCommand cmd = new OracleCommand())
			{
				cmd.Connection = _connection;
				cmd.Connection.Open();
				cmd.CommandText = "INSERT INTO MMTCST_BOM_BOM_HEADER (ITEM_CODE, INVENTORY_ITEM_ID, APPROVE_PATH_ID, APPROVE_FLAG, INTERFACE_FLAG, CREATION_DATE, CREATED_BY, LAST_UPDATE_DATE, LAST_UPDATED_BY) VALUES (:ITEM_CODE, :INVENTORY_ITEM_ID, :APPROVE_PATH_ID, :APPROVE_FLAG, :INTERFACE_FLAG, SYSDATE, :CREATED_BY, SYSDATE, :LAST_UPDATED_BY)";
				cmd.Parameters.Add("ITEM_CODE", OracleDbType.Varchar2).Value = bomHeaderModel.ITEM_CODE;
				cmd.Parameters.Add("INVENTORY_ITEM_ID", OracleDbType.Int32).Value = bomHeaderModel.INVENTORY_ITEM_ID;
				cmd.Parameters.Add("APPROVE_PATH_ID", OracleDbType.Int32).Value = bomHeaderModel.APPROVE_PATH_ID;
				cmd.Parameters.Add("APPROVE_FLAG", OracleDbType.Varchar2).Value = bomHeaderModel.APPROVE_FLAG;
				cmd.Parameters.Add("INTERFACE_FLAG", OracleDbType.Varchar2).Value = bomHeaderModel.INTERFACE_FLAG;
				cmd.Parameters.Add("CREATED_BY", OracleDbType.Varchar2).Value = bomHeaderModel.CREATED_BY;
				cmd.Parameters.Add("LAST_UPDATED_BY", OracleDbType.Varchar2).Value = bomHeaderModel.LAST_UPDATED_BY;
				cmd.ExecuteNonQuery();
			}
			return RedirectToAction("Index");
		}

		public IActionResult Delete(int id)
		{
			using (OracleCommand cmd = new OracleCommand())
			{
				cmd.Connection = _connection;
				cmd.Connection.Open();
				cmd.CommandText = "DELETE FROM MMTCST_BOM_BOM_HEADER WHERE BOM_ID = :BOM_ID";
				cmd.Parameters.Add("BOM_ID", OracleDbType.Int32).Value = id;
				cmd.ExecuteNonQuery();
			}
			return RedirectToAction("Index");
		}
		[HttpGet]
		public IActionResult Edit(int? id)

		{
			BomHeaderModel bomHeaderModel = new BomHeaderModel();
			OracleCommand cmd = _connection.CreateCommand();
			cmd.CommandText = "SELECT * FROM MMTCST_BOM_BOM_HEADER WHERE BOM_ID = :BOM_ID";
			cmd.Parameters.Add("BOM_ID", OracleDbType.Int32).Value = id;
			_connection.Open();
			OracleDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
                bomHeaderModel.BOM_ID = Convert.ToInt32(reader["BOM_ID"]);
                bomHeaderModel.ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]);
                bomHeaderModel.INVENTORY_ITEM_ID = Convert.ToInt32(reader["INVENTORY_ITEM_ID"]);
                bomHeaderModel.APPROVE_PATH_ID = Convert.ToInt32(reader["APPROVE_PATH_ID"]);
                bomHeaderModel.APPROVE_FLAG = Convert.ToString(reader["APPROVE_FLAG"]);
                bomHeaderModel.INTERFACE_FLAG = Convert.ToString(reader["INTERFACE_FLAG"]);
                bomHeaderModel.CREATION_DATE = Convert.ToDateTime(reader["CREATION_DATE"]);
                bomHeaderModel.CREATED_BY = Convert.ToString(reader["CREATED_BY"]);
                bomHeaderModel.LAST_UPDATE_DATE = Convert.ToDateTime(reader["LAST_UPDATE_DATE"]);
                bomHeaderModel.LAST_UPDATED_BY = Convert.ToString(reader["LAST_UPDATED_BY"]);
            }

			return View(bomHeaderModel);

			
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(BomHeaderModel bomHeaderModel)
		{
			var id = bomHeaderModel.BOM_ID;
			using (OracleCommand cmd = new OracleCommand())
			{
                cmd.Connection = _connection;
                cmd.Connection.Open();
                cmd.CommandText = "UPDATE MMTCST_BOM_BOM_HEADER SET ITEM_CODE = :ITEM_CODE, INVENTORY_ITEM_ID = :INVENTORY_ITEM_ID, APPROVE_PATH_ID = :APPROVE_PATH_ID, APPROVE_FLAG = :APPROVE_FLAG, INTERFACE_FLAG = :INTERFACE_FLAG, LAST_UPDATE_DATE = SYSDATE, LAST_UPDATED_BY = :LAST_UPDATED_BY WHERE BOM_ID = :BOM_ID";
                cmd.Parameters.Add("ITEM_CODE", OracleDbType.Varchar2).Value = bomHeaderModel.ITEM_CODE;
                cmd.Parameters.Add("INVENTORY_ITEM_ID", OracleDbType.Int32).Value = bomHeaderModel.INVENTORY_ITEM_ID;
                cmd.Parameters.Add("APPROVE_PATH_ID", OracleDbType.Int32).Value = bomHeaderModel.APPROVE_PATH_ID;
                cmd.Parameters.Add("APPROVE_FLAG", OracleDbType.Varchar2).Value = bomHeaderModel.APPROVE_FLAG;
                cmd.Parameters.Add("INTERFACE_FLAG", OracleDbType.Varchar2).Value = bomHeaderModel.INTERFACE_FLAG;
                cmd.Parameters.Add("LAST_UPDATED_BY", OracleDbType.Varchar2).Value = bomHeaderModel.LAST_UPDATED_BY;
                cmd.Parameters.Add("BOM_ID", OracleDbType.Int32).Value = id;
				if(cmd.ExecuteNonQuery() > 0)
				{
					   return RedirectToAction("Index");
				}
				else { return RedirectToAction("Edit");
				}

				
				
            }
			
		}
	

	}

}
