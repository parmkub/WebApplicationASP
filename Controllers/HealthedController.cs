using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using WebApplicationASP.Models;

namespace WebApplicationASP.Controllers
{
	public class HealthedController : Controller
	{
		private readonly OracleConnection _connection;

		public  HealthedController(OracleConnection oracleConnection)
		{
			_connection = oracleConnection;
		}



		public IActionResult Index()
		{

            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username:" + username);

            OracleCommand cmd1 = _connection.CreateCommand();
            cmd1.CommandText = "SELECT EMPLOYEE_CODE,position_group_code,depart_code,divi_code,sect_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";
            _connection.Open();
            OracleDataReader readeruser1 = cmd1.ExecuteReader();
            string departCode = "";
            string diviCode = "";
            string sectCode = "";
            string positionGroupCode = "";

         
            while (readeruser1.Read())
            {
              
                departCode = Convert.ToString(readeruser1["depart_code"]);
                diviCode = Convert.ToString(readeruser1["divi_code"]);
                sectCode = Convert.ToString(readeruser1["sect_code"]);
                positionGroupCode = Convert.ToString(readeruser1["position_group_code"]);
            }
            _connection.Close();


            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            System.Diagnostics.Debug.WriteLine("sectCode:" + sectCode);
            System.Diagnostics.Debug.WriteLine("diviCode:" + diviCode);
            System.Diagnostics.Debug.WriteLine("departCode:" + departCode);

            if (sectCode == "5211" || sectCode == "5212" || diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }


            List<HealthedModel> model = new List<HealthedModel>();
			OracleCommand cmd = _connection.CreateCommand();
			cmd.CommandText = "select * from sf_personal_healthy";
			_connection.Open();
			OracleDataReader readeruser = cmd.ExecuteReader();
			while (readeruser.Read())
			{
				HealthedModel healthedModel = new HealthedModel();
				healthedModel.ID_PERSONAL_HEALTHY = Convert.ToString(readeruser["ID_PERSONAL_HEALTHY"]);
				healthedModel.NAME = Convert.ToString(readeruser["NAME"]);
				healthedModel.POSITION = Convert.ToString(readeruser["POSITION"]);
				healthedModel.CREATE_BY = Convert.ToString(readeruser["CREATE_BY"]);
				healthedModel.CREATE_DATE = Convert.ToString(readeruser["CREATE_DATE"]);
				healthedModel.LAST_UPDATE_DATE = Convert.ToString(readeruser["LAST_UPDATE_DATE"]);
				healthedModel.LAST_UPDATE_BY = Convert.ToString(readeruser["LAST_UPDATE_BY"]);
				model.Add(healthedModel);

			}

			return View(model);
		}

		public IActionResult Create()
		{
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username:" + username);

            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT EMPLOYEE_CODE,position_group_code,depart_code,divi_code,sect_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";
            _connection.Open();
            OracleDataReader readeruser = cmd.ExecuteReader();
            string diviCode = "";
            string sectCode = "";
            while (readeruser.Read())
            {


                diviCode = Convert.ToString(readeruser["divi_code"]);
                sectCode = Convert.ToString(readeruser["sect_code"]);

            }
            _connection.Close();
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (sectCode == "5211" || sectCode == "5212" || diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }
            return View();
		}

		[HttpPost]
		public IActionResult Create(HealthedModel healthed)
		{
            string[] names = User.Identity!.Name!.Split('\\');
            string username = names[1].ToUpper();
            string id = healthed.ID_PERSONAL_HEALTHY;
			string name = healthed.NAME;
			string position = healthed.POSITION;
			

			OracleCommand cmd = _connection.CreateCommand();
			cmd.CommandText = "insert into sf_personal_healthy (ID_PERSONAL_HEALTHY,NAME,POSITION,CREATE_BY,CREATE_DATE,LAST_UPDATE_DATE,LAST_UPDATE_BY) " +
				"values ('"+id+"','"+name+"','"+position+"','"+username+ "',SYSDATE,SYSDATE,'"+username+"')";
		
			_connection.Open();
			var result = cmd.ExecuteNonQuery();
			_connection.Close();
			if(result < 1)
			{
                return BadRequest();
            }
			else
			{
                return RedirectToAction("Index");
            }
	
		}
		
		public IActionResult Edit(string? id)
		{
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username:" + username);

            OracleCommand cmd1 = _connection.CreateCommand();
            cmd1.CommandText = "SELECT EMPLOYEE_CODE,position_group_code,depart_code,divi_code,sect_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";
            _connection.Open();
            OracleDataReader readeruser = cmd1.ExecuteReader();
            string diviCode = "";
            string sectCode = "";
            while (readeruser.Read())
            {


                diviCode = Convert.ToString(readeruser["divi_code"]);
                sectCode = Convert.ToString(readeruser["sect_code"]);

            }
            _connection.Close();
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (sectCode == "5211" || sectCode == "5212" || diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }

            OracleCommand cmd = _connection.CreateCommand();
			string sql = "select * from sf_personal_healthy where ID_PERSONAL_HEALTHY = '"+id+"'";
			System.Diagnostics.Debug.WriteLine("SQL:" + sql);
			cmd.CommandText = sql;
			_connection.Open();
			OracleDataReader reader = cmd.ExecuteReader();
			HealthedModel healthedModel = new HealthedModel();
			while (reader.Read())
			{
                healthedModel.ID_PERSONAL_HEALTHY = id.ToString();
                healthedModel.NAME = Convert.ToString(reader["NAME"]);
                healthedModel.POSITION = Convert.ToString(reader["POSITION"]);
                healthedModel.CREATE_BY = Convert.ToString(reader["CREATE_BY"]);
                healthedModel.CREATE_DATE = Convert.ToString(reader["CREATE_DATE"]);
                healthedModel.LAST_UPDATE_DATE = Convert.ToString(reader["LAST_UPDATE_DATE"]);
                healthedModel.LAST_UPDATE_BY = Convert.ToString(reader["LAST_UPDATE_BY"]);
            }
			System.Diagnostics.Debug.WriteLine("ID_PERSONAL_HEALTHY:" + healthedModel.ID_PERSONAL_HEALTHY);
			_connection.Close();
			return View(healthedModel);

			
		}

		[HttpPost]
		public IActionResult Edit(HealthedModel healthed)
		{
			string[] names = User.Identity!.Name!.Split('\\');
            string username = names[1].ToUpper();
            string id = healthed.ID_PERSONAL_HEALTHY;
            string name = healthed.NAME;
            string position = healthed.POSITION;

			OracleCommand cmd = _connection.CreateCommand();
			cmd.CommandText = "update sf_personal_healthy set NAME = '"+name+"',POSITION = '"+position+"',LAST_UPDATE_DATE = SYSDATE,LAST_UPDATE_BY = '"+username+"' where ID_PERSONAL_HEALTHY = '"+id+"'";
			_connection.Open();
			var result = cmd.ExecuteNonQuery();
			_connection.Close();
			if (result < 1)
			{
                return BadRequest();
            }
            else
			{
                return RedirectToAction("Index");
            }
		}

		public IActionResult Delete(string id)
		{
			OracleCommand cmd = _connection.CreateCommand();
			string sql = "delete from sf_personal_healthy where ID_PERSONAL_HEALTHY = '"+id+"'";
			cmd.CommandText = sql;
			_connection.Open();
			var result = cmd.ExecuteNonQuery();
			_connection.Close();
			if (result < 1)
			{
                return BadRequest();
            }
            else
			{
                return RedirectToAction("Index");
            }

		}
		
	}
}
