using Microsoft.AspNetCore.Mvc;

using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using WebApplicationASP.Models;

namespace WebApplicationASP.Controllers
{
	public class ApprovePath
	{
		public string employeeCode { get; set; }
		public string nameFilter { get; set; }
		public string departCode { get; set; }
		public string diviCode { get; set; }
		public string sectCode { get; set; }

		public string positionGroupCode { get; set; }

		private readonly OracleConnection _connection;


		public ApprovePath(OracleConnection oracelConnection)
		{
			_connection = oracelConnection;
		}

		public string GetPosition(string username)
		{


			string columFilter = "";
			string nameFilter = "";
			_connection.Open();
			OracleCommand command = _connection.CreateCommand();
			/* string sql = "SELECT EMPLOYEE_CODE,position_group_code,depart_code,divi_code,sect_code FROM sf_per_employees_fnduser_v where User_name = :username";*/
			string sql = "SELECT p.depart_code,p.divi_code,p.sect_code,p.position_group_code FROM sf_per_employees_dup_v p,sf_per_employees_fnduser_v u where p.employee_code = u.employee_code  and u.resign_date is null and u.user_name = '" + username + "' ORDER BY 1 ASC"; // แก้ไข โดยเพิ่มเงื่อนไข u.resign_date is null
			Debug.WriteLine("sql : " + sql);
			command.CommandText = sql;
			command.Parameters.Add(new OracleParameter("username", username));
			OracleDataReader reader = command.ExecuteReader();
			List<CheckPosition> checkPositions = new List<CheckPosition>();


			while (reader.Read())
			{
				// employeeCode = reader["EMPLOYEE_CODE"].ToString();
				/*  departCode = reader["depart_code"].ToString();
				  diviCode = reader["divi_code"].ToString();
				  sectCode = reader["sect_code"].ToString();
				  positionGroupCode = reader["position_group_code"].ToString();*/
				checkPositions.Add(new CheckPosition
				{
					departCode = reader["depart_code"].ToString(),
					diviCode = reader["divi_code"].ToString(),
					sectCode = reader["sect_code"].ToString(),
					positionGroupCode = reader["position_group_code"].ToString()

				});

			}

			positionGroupCode = checkPositions[0].positionGroupCode;
			sectCode = checkPositions[0].sectCode;
			diviCode = checkPositions[0].diviCode;
			departCode = checkPositions[0].departCode;


			//Debug.WriteLine("positionGroupCode555 : " + positionGroupCode);
			reader.Close();
			_connection.Close();
			// print list checkPositions
			// สร้างสตริงแยกต่างหากสำหรับแต่ละฟิลด์
			string departCodes = $"{String.Join(",", checkPositions.Select(e => $"'{e.departCode}'"))}'";
			string diviCodes = $"{String.Join(",", checkPositions.Select(e => $"'{e.diviCode}'"))}";
			string sectCodes = $"{String.Join(",", checkPositions.Select(e => $"'{e.sectCode}'"))}";
			
		/*	Debug.WriteLine($"departCodes : {departCodes}");
			Debug.WriteLine($"diviCodes : {diviCodes}");
			Debug.WriteLine($"sectCodes : {sectCodes}");*/
			if (positionGroupCode == "052")
			{
				columFilter = $"a.depart_code IN ({departCodes})";
				nameFilter = departCodes;
			}
			else if (positionGroupCode == "042")
			{
				columFilter = $"a.divi_code IN ({diviCodes})";
				nameFilter = diviCodes;
			}
			else if (positionGroupCode == "032")
			{
				columFilter = $"a.sect_code IN ({sectCodes})";
				nameFilter = sectCodes;
			
			}
			else
			{
				columFilter = $"a.sect_code IN ({sectCodes})";
				nameFilter = sectCodes;
			}

			//Debug.WriteLine($"columFilter6666 : {columFilter}");




			

			/*  if(divi)
			  Debug.WriteLine("diviCodeList : " + diviCodeList);


			  if (positionGroupCode == "052" || positionGroupCode == "051")
			  {
				  columFilter = "a.depart_code = '" + departCode + "'";
				  nameFilter = departCode;
			  }
			  else if (positionGroupCode == "042" || positionGroupCode == "041")
			  {
				  columFilter = "a.divi_code = '" + diviCode + "'";
				  nameFilter = diviCode;

			  }
			  else if (positionGroupCode == "032" || positionGroupCode == "031")
			  {
				  columFilter = "a.sect_code = '" + sectCode + "'";
				  nameFilter = sectCode;
			  }
			  else
			  {
				  columFilter = "a.sect_code = '" + sectCode + "'";
				  nameFilter = sectCode;
			  }


			  //คุณอรุณีย์ 5300 ธุระการสำนักงานชุมพร, จัดซื่อทั่วไปชุมพร ,ขนส่งชุมพร
			  if (departCode == "5300" && positionGroupCode == "052")
			  {
				  columFilter = "a.depart_code IN ('5300','5100','7200')";

			  }

			  //คุณเจียบชนะชัย 4110 Shushi, FG2  
			  if (diviCode == "4110" && positionGroupCode == "042")
			  {
				  columFilter = "a.divi_code IN ('4110','1420')";

			  }
			  //คุณรัชนิดา 1220 FG1, FG3  
			  if (diviCode == "2110" && positionGroupCode == "042")
			  {
				  columFilter = "a.divi_code IN ('2110','3110')";

			  }

			  //คุณอรอุมา 1220 FG1, FG3  
			  if (diviCode == "1220" && positionGroupCode == "042")
			  {
				  columFilter = "a.divi_code IN ('1220')";

			  }


			  //IT 0410 ดู 0420 ด้วย
			  if (diviCode == "0410" && positionGroupCode == "042")
			  {
				  columFilter = "a.divi_code IN ('0410','0420')";

			  }

			  // บัญชีต้นทุน กับ บัญชีแยกประเภท พี่ใจ
			  if (diviCode == "7180" && positionGroupCode == "042")
			  {
				  columFilter = "a.divi_code IN ('7180','7170')";
			  }

			  //



			  //fG3 3112 ดู 3113 ด้วย
			  if (sectCode == "3112" && positionGroupCode == "032")
			  {
				  columFilter = "a.sect_code IN ('3112','3113','3116')";
			  }

			  //fG1 1311 ดูแล 1221 /1226/ 1225  พี่ปูอรุณรัตน์

			  if (sectCode == "1311" && positionGroupCode == "032")
			  {
				  columFilter = "a.sect_code IN ('1311','1221','1226','1225')";
			  }

			  //fg1 1223 ดู 1222 ด้วย

			  if (sectCode == "1223")
			  {
				  columFilter = "a.sect_code IN ('1222','1223')";
			  }

			  // 5242 ดู 5243 ด้วย
			  if (sectCode == "5242")
			  {
				  columFilter = "a.sect_code IN ('5242','5243')";
			  }*/

			return columFilter;

		}




	}
}
