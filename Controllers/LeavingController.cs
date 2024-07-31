
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using System.Reflection.Metadata;
using WebApplicationASP.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rotativa.AspNetCore;
using Org.BouncyCastle.Bcpg;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Client;
using System.Data.SqlTypes;

namespace WebApplicationASP.Controllers
{

    public class LeavingController : Controller
    {
        private readonly OracleConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;

        Uri baseAddress = new Uri("http://10.2.2.5:3001/");
        Uri baseAddressAPI = new Uri("http://10.2.2.5");
        private readonly HttpClient _client;
        private readonly HttpClient _client2;


        private string detailDelete;
        private string employeeCode;




        public LeavingController(OracleConnection connection, IWebHostEnvironment hostingEnvironment)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
            _client = new HttpClient { BaseAddress = baseAddress };
            _client2 = new HttpClient { BaseAddress = baseAddressAPI };

        }


        public IActionResult Index()
        {

            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username:" + username);
            string columFilter = "";



            ApprovePath approvePath = new ApprovePath(_connection);

            columFilter = approvePath.GetPosition(username);

            //

            System.Diagnostics.Trace.WriteLine("Employee Code:" + approvePath.employeeCode);
            System.Diagnostics.Trace.WriteLine("Colum Filter:" + columFilter);
            System.Diagnostics.Trace.WriteLine("Name Filter:" + approvePath.nameFilter);

            List<LeavingViewModel> list = new List<LeavingViewModel>();

            string sql = "SELECT " +
                "a.START_DATE,a.END_DATE," +
                "a.COUNT_DATE," +
                "a.name," +
                "a.EMPLOYEE_CODE," +
                "a.ABSENCE_CODE,a.ABSENCE_DAY," +
                "a.ABSENCE_HOUR,a.DELETE_MARK," +
                "a.REVIEW," +
                "a.APPROVE," +
                "a.ABSENCE_PERIOD," +
                "a.ABSENCE_STATUS," +
                "a.ABSENCE_TOKEN," +
                "a.ABSENCE_DETAIL," +
                "a.ABSENCE_DOCUMENT," +
                "a.CREATION_DATE," +
                "a.STATUS_APPROVE," +
                "a.DEPART_CODE," +
                "a.DIVI_CODE," +
                "a.SECT_CODE " +
                "FROM " +
                "sf_per_absence_moble_v a " +
                "WHERE  " +
                "" + columFilter + " and to_CHAR(a.creation_date,'DD-MM-YY') = TO_CHAR(SYSDATE,'DD-MM-YY')" +
                "ORDER BY a.creation_date DESC";
            System.Diagnostics.Trace.WriteLine("SQL:" + sql);

            OracleCommand cmd = _connection.CreateCommand();

            cmd.CommandText = sql;

            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                LeavingViewModel leaving = new LeavingViewModel();
                if (Convert.ToString(reader["ABSENCE_HOUR"]) == "0.3")
                {
                    leaving.ABSENCE_HOUR = "0.5";
                }
                else
                {
                    if (Convert.ToString(reader["ABSENCE_HOUR"]).Contains("."))
                    {
                        leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]).Split(".")[0] + ".5";
                    }
                    else
                    {
                        leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                    }

                }


                leaving.START_DATE = Convert.ToString(reader["START_DATE"]);
                leaving.END_DATE = Convert.ToString(reader["END_DATE"]);
                leaving.COUNT_DATE = Convert.ToString(reader["COUNT_DATE"]);
                leaving.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                leaving.NAME = Convert.ToString(reader["NAME"]);
                leaving.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                leaving.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);

                leaving.DELETE_MARK = Convert.ToString(reader["DELETE_MARK"]);
                leaving.REVIEW = Convert.ToString(reader["REVIEW"]);
                leaving.APPROVE = Convert.ToString(reader["APPROVE"]);
                leaving.ABSENCE_PERIOD = Convert.ToString(reader["ABSENCE_PERIOD"]);
                leaving.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                leaving.ABSENCE_TOKEN = Convert.ToString(reader["ABSENCE_TOKEN"]);
                leaving.ABSENCE_DETAIL = Convert.ToString(reader["ABSENCE_DETAIL"]);
                leaving.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                leaving.CREATION_DATE = Convert.ToString(reader["CREATION_DATE"]);
                leaving.STATUS_APPROVE = Convert.ToString(reader["STATUS_APPROVE"]);
                leaving.DIVI_CODE = Convert.ToString(reader["DIVI_CODE"]);
                leaving.SECT_CODE = Convert.ToString(reader["SECT_CODE"]);
                leaving.DEPART_CODE = Convert.ToString(reader["DEPART_CODE"]);
                list.Add(leaving);

            }


            ViewBag.departName = approvePath.nameFilter;
            TempData["dataToSend"] = "Index";

            System.Diagnostics.Debug.WriteLine("Depart Code:" + approvePath.departCode);
            System.Diagnostics.Debug.WriteLine("Divi Code:" + approvePath.diviCode);
            System.Diagnostics.Debug.WriteLine("Sect Code:" + approvePath.sectCode);

            if (approvePath.sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (approvePath.sectCode == "5211" || approvePath.sectCode == "5212" || approvePath.diviCode == "5210")
            {
                
                ViewBag.Admin = "Visable";
            }

            if (approvePath.departCode == "7000" && approvePath.positionGroupCode == "042")
            {
                System.Console.WriteLine("7000");
                ViewBag.AdminBkk = "Visable";
            }


            _connection.Close();

            return View(list);

        }

        public IActionResult LeaveEmpCode()
        {

            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username:" + username);
            string columFilter = "";



            ApprovePath approvePath = new ApprovePath(_connection);

            columFilter = approvePath.GetPosition(username);
            if (approvePath.sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (approvePath.sectCode == "5211" || approvePath.sectCode == "5212" || approvePath.diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }

            if (approvePath.departCode == "7000" && approvePath.positionGroupCode == "042")
            {
                System.Console.WriteLine("7000");
                ViewBag.AdminBkk = "Visable";
            }



            LeavingViewModelList leavingViewModelList = new LeavingViewModelList();
            leavingViewModelList.LeavingViewModel = new List<LeavingViewModel>();
            leavingViewModelList.EmployeeCode = "";
             
  

            return View(leavingViewModelList);
        }

        [HttpPost]
        public IActionResult LeaveEmpCode(LeavingViewModelList leaving)
        {

			string[] name = User.Identity!.Name!.Split('\\');
			string username = name[1].ToUpper();

			System.Diagnostics.Debug.WriteLine("Username:" + username);
			string columFilter = "";



			ApprovePath approvePath = new ApprovePath(_connection);

			columFilter = approvePath.GetPosition(username);
			if (approvePath.sectCode == "5251")
			{
				ViewBag.Health = "Visable";
			}

			if (approvePath.sectCode == "5211" || approvePath.sectCode == "5212" || approvePath.diviCode == "5210")
			{
				ViewBag.Admin = "Visable";
			}

			if (approvePath.departCode == "7000" && approvePath.positionGroupCode == "042")
			{
				System.Console.WriteLine("7000");
				ViewBag.AdminBkk = "Visable";
			}

			List<LeavingViewModel> list = new List<LeavingViewModel>();

            string sql = "SELECT " +
                "a.START_DATE,a.END_DATE," +
                "a.COUNT_DATE," +
                "a.name," +
                "a.EMPLOYEE_CODE," +
                "a.ABSENCE_CODE,a.ABSENCE_DAY," +
                "a.ABSENCE_HOUR,a.DELETE_MARK," +
                "a.REVIEW," +
                "a.APPROVE," +
                "a.ABSENCE_PERIOD," +
                "a.ABSENCE_STATUS," +
                "a.ABSENCE_TOKEN," +
                "a.ABSENCE_DETAIL," +
                "a.ABSENCE_DOCUMENT," +
                "a.CREATION_DATE," +
                "a.STATUS_APPROVE," +
                "a.DEPART_CODE," +
                "a.DIVI_CODE," +
                "a.SECT_CODE " +
                "FROM " +
                "sf_per_absence_moble_v a " +
                "WHERE  " +
                "a.EMPLOYEE_CODE = '" +leaving.EmployeeCode+"'" +
                "ORDER BY a.creation_date DESC";
            System.Diagnostics.Trace.WriteLine("SQL:" + sql);

            OracleCommand cmd = _connection.CreateCommand();

            cmd.CommandText = sql;

            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                LeavingViewModel leaving1 = new LeavingViewModel();
                if (Convert.ToString(reader["ABSENCE_HOUR"]) == "0.3")
                {
                    leaving1.ABSENCE_HOUR = "0.5";
                }
                else
                {
                    if (Convert.ToString(reader["ABSENCE_HOUR"]).Contains("."))
                    {
                        leaving1.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]).Split(".")[0] + ".5";
                    }
                    else
                    {
                        leaving1.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                    }

                }


                leaving1.START_DATE = Convert.ToString(reader["START_DATE"]);
                leaving1.END_DATE = Convert.ToString(reader["END_DATE"]);
                leaving1.COUNT_DATE = Convert.ToString(reader["COUNT_DATE"]);
                leaving1.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                leaving1.NAME = Convert.ToString(reader["NAME"]);
                leaving1.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                leaving1.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);

                leaving1.DELETE_MARK = Convert.ToString(reader["DELETE_MARK"]);
                leaving1.REVIEW = Convert.ToString(reader["REVIEW"]);
                leaving1.APPROVE = Convert.ToString(reader["APPROVE"]);
                leaving1.ABSENCE_PERIOD = Convert.ToString(reader["ABSENCE_PERIOD"]);
                leaving1.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                leaving1.ABSENCE_TOKEN = Convert.ToString(reader["ABSENCE_TOKEN"]);
                leaving1.ABSENCE_DETAIL = Convert.ToString(reader["ABSENCE_DETAIL"]);
                leaving1.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                leaving1.CREATION_DATE = Convert.ToString(reader["CREATION_DATE"]);
                leaving1.STATUS_APPROVE = Convert.ToString(reader["STATUS_APPROVE"]);
                leaving1.DIVI_CODE = Convert.ToString(reader["DIVI_CODE"]);
                leaving1.SECT_CODE = Convert.ToString(reader["SECT_CODE"]);
                leaving1.DEPART_CODE = Convert.ToString(reader["DEPART_CODE"]);
                list.Add(leaving1);

                System.Diagnostics.Debug.WriteLine("////////////////////////////////////////////////////////////////////");
                System.Diagnostics.Debug.WriteLine("Start date>>>" + leaving1.START_DATE);

            }
            _connection.Close();

            LeavingViewModelList leavingViewModelList = new LeavingViewModelList();
            leavingViewModelList.LeavingViewModel = list;
            leavingViewModelList.EmployeeCode = leaving.EmployeeCode;
           

            return View(leavingViewModelList);

        }

        public IActionResult LeavingAll()
        {

            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username:" + username);
            string columFilter = "";



            ApprovePath approvePath = new ApprovePath(_connection);

            columFilter = approvePath.GetPosition(username);




            //

            System.Diagnostics.Trace.WriteLine("Employee Code:" + approvePath.employeeCode);
            System.Diagnostics.Trace.WriteLine("Colum Filter:" + columFilter);
            System.Diagnostics.Trace.WriteLine("Name Filter:" + approvePath.nameFilter);

            List<LeavingViewModel> lists = new List<LeavingViewModel>();

            string sql = "SELECT " +
                "a.START_DATE,a.END_DATE," +
                "a.COUNT_DATE," +
                "a.name," +
                "a.EMPLOYEE_CODE," +
                "a.ABSENCE_CODE,a.ABSENCE_DAY," +
                "a.ABSENCE_HOUR,a.DELETE_MARK," +
                "a.REVIEW," +
                "a.APPROVE," +
                "a.ABSENCE_PERIOD," +
                "a.ABSENCE_STATUS," +
                "a.ABSENCE_TOKEN," +
                "a.ABSENCE_DETAIL," +
                "a.ABSENCE_DOCUMENT," +
                "a.CREATION_DATE," +
                "a.STATUS_APPROVE," +
                "a.DEPART_CODE," +
                "a.DIVI_CODE," +
                "a.SECT_CODE " +
                "FROM " +
                "sf_per_absence_moble_v a " +
                "WHERE  " +
                "" + columFilter + "" +
                "ORDER BY a.creation_date DESC";
            System.Diagnostics.Trace.WriteLine("SQL:" + sql);

            OracleCommand cmd = _connection.CreateCommand();

            cmd.CommandText = sql;

            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                LeavingViewModel leaving = new LeavingViewModel();
                if (Convert.ToString(reader["ABSENCE_HOUR"]) == "0.3")
                {
                    leaving.ABSENCE_HOUR = "0.5";
                }
                else
                {
                    if (Convert.ToString(reader["ABSENCE_HOUR"]).Contains("."))
                    {
                        leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]).Split(".")[0] + ".5";
                    }
                    else
                    {
                        leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                    }

                }


                leaving.START_DATE = Convert.ToString(reader["START_DATE"]);
                leaving.END_DATE = Convert.ToString(reader["END_DATE"]);
                leaving.COUNT_DATE = Convert.ToString(reader["COUNT_DATE"]);
                leaving.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                leaving.NAME = Convert.ToString(reader["NAME"]);
                leaving.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                leaving.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);

                leaving.DELETE_MARK = Convert.ToString(reader["DELETE_MARK"]);
                leaving.REVIEW = Convert.ToString(reader["REVIEW"]);
                leaving.APPROVE = Convert.ToString(reader["APPROVE"]);
                leaving.ABSENCE_PERIOD = Convert.ToString(reader["ABSENCE_PERIOD"]);
                leaving.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                leaving.ABSENCE_TOKEN = Convert.ToString(reader["ABSENCE_TOKEN"]);
                leaving.ABSENCE_DETAIL = Convert.ToString(reader["ABSENCE_DETAIL"]);
                leaving.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                leaving.CREATION_DATE = Convert.ToString(reader["CREATION_DATE"]);
                leaving.STATUS_APPROVE = Convert.ToString(reader["STATUS_APPROVE"]);
                leaving.DIVI_CODE = Convert.ToString(reader["DIVI_CODE"]);
                leaving.SECT_CODE = Convert.ToString(reader["SECT_CODE"]);
                leaving.DEPART_CODE = Convert.ToString(reader["DEPART_CODE"]);
                lists.Add(leaving);

            }



            ViewBag.departName = approvePath.nameFilter;
            TempData["dataToSend"] = "Index";

            if (approvePath.sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (approvePath.sectCode == "5211" || approvePath.sectCode == "5212" || approvePath.diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }

            if (approvePath.departCode == "7000" && approvePath.positionGroupCode == "042")
            {
                System.Console.WriteLine("7000");
                ViewBag.AdminBkk = "Visable";
            }





            _connection.Close();

            return View(lists);

        }
        public IActionResult Create()
        {
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            ApprovePath approvePath = new ApprovePath(_connection);
            approvePath.GetPosition(username);

            System.Diagnostics.Debug.WriteLine("Username:" + username);

            if (approvePath.sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (approvePath.sectCode == "5211" || approvePath.sectCode == "5212" || approvePath.diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }


            return View();
        }


        [HttpPost]
        public IActionResult Create(LeavingModel leaving)

        {


            ///Document ID////////////////////////
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random randNum = new Random();
            char[] chars = new char[8];
            for (int i = 0; i < 8; i++)
            {
                chars[i] = allowedChars[(int)((allowedChars.Length) * randNum.NextDouble())];
            }
            string docId = new string(chars);
            ViewBag.docId = docId;

            ///Employee Code////////////////////////

            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "select employee_code,sect_code,divi_code,depart_code,'คุณ'||first_name||' '||last_name name,position_group_code from sf_per_employees_v where resign_date is null and card_raw = :card_raw";
            cmd.Parameters.Add("card_raw", leaving.card_row);
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            string name = "";
            string positionGroupCode = "";
            while (reader.Read())
            {
                leaving.employee_code = Convert.ToString(reader["employee_code"]);
                leaving.sect_code = Convert.ToString(reader["sect_code"]);
                leaving.divi_code = Convert.ToString(reader["divi_code"]);
                leaving.depart_code = Convert.ToString(reader["depart_code"]);
                name = Convert.ToString(reader["name"]);
                positionGroupCode = Convert.ToString(reader["POSITION_GROUP_CODE"]);


                System.Diagnostics.Debug.WriteLine("Employee Code:" + leaving.employee_code);
                System.Diagnostics.Debug.WriteLine("Sect Code:" + leaving.sect_code);
                System.Diagnostics.Debug.WriteLine("Divi Code:" + leaving.divi_code);
                System.Diagnostics.Debug.WriteLine("Depart Code:" + leaving.depart_code);
                System.Diagnostics.Debug.WriteLine("Name:" + name);
                System.Diagnostics.Debug.WriteLine("Position Group Code:" + positionGroupCode);

            }
            /////End Employee Code////////////////////////////////////////////


            if (leaving.absence_day_start == null)
            {
                string[] substring = leaving.absence_day.Split(" - ");

                var dateLeave = substring[0];
                var dateLeaveEnd = substring[1];
                ////////////////////////////////////////////////


                ///Date Start Leave////////////////////////
                DateTime date1 = DateTime.ParseExact(dateLeave, "MM/dd/yyyy", null);

                DateTime date2 = DateTime.ParseExact(dateLeaveEnd, "MM/dd/yyyy", null);

                DateTime Start = DateTime.ParseExact(dateLeave, "MM/dd/yyyy", null);

                string dateStart = Start.ToString("dd/MMM/yyyy");

                TimeSpan duration = (date2 - date1);
                int dayCount = duration.Days + 1;
                ////////////////////////////////////////////////


                ///Day Or Hour////////////////////////
                var day = 0;
                if (leaving.absence_hour > 0 && leaving.absence_hour < 8)
                {
                    day = 0;
                }
                else if (leaving.absence_hour >= 8)
                {
                    day = 0;
                    leaving.absence_hour = 0.3;
                }
                else
                {
                    day = 1;
                    leaving.absence_hour = 0;
                }


                System.Diagnostics.Debug.WriteLine("////////////////////////////////////////////////////////////////////");
                System.Diagnostics.Debug.WriteLine("Start date>>>" + leaving.absence_day_start);
                System.Diagnostics.Debug.WriteLine("Card Row:" + leaving.card_row);
                System.Diagnostics.Debug.WriteLine("Document ID:" + docId);
                System.Diagnostics.Debug.WriteLine("วันที่ลา:" + dateStart);
                System.Diagnostics.Debug.WriteLine("รหัสพนักงาน:" + leaving.employee_code);
                System.Diagnostics.Debug.WriteLine("ประเภทการลา:" + leaving.absence_code);
                System.Diagnostics.Debug.WriteLine("จำนวนวัน:" + leaving.absence_day);

                System.Diagnostics.Debug.WriteLine("จำนวนชั่วโมง:" + leaving.absence_hour);
                System.Diagnostics.Debug.WriteLine("สถานะการลา" + leaving.absence_status);
                System.Diagnostics.Debug.WriteLine("รายละเอียด:" + leaving.absence_detail);
                System.Diagnostics.Debug.WriteLine("Token" + leaving.absence_token);
                System.Diagnostics.Debug.WriteLine("รวมวันลา" + dayCount + "วัน");


                for (int i = 0; i < dayCount; i++)
                {
                    dateStart = Start.AddDays(i).ToString("dd-MMM-yyyy");
                    System.Diagnostics.Debug.WriteLine("วันที่ลา:" + dateStart);

                    string command = "INSERT INTO sf_per_absence_mobile (" +
                        "absence_document," +
                        "absence_date, " +
                        "employee_code, " +
                        "absence_code, " +
                        "absence_day," +
                        "absence_hour," +
                        "delete_mark," +
                        "absence_period," +
                        "absence_status," +
                        "absence_detail," +
                        "absence_token," +
                        "CREATION_DATE," +
                        "CREATED_BY," +
                        "LAST_UPDATE_DATE," +
                        "LAST_UPDATED_BY) VALUES('" + docId + "','" + dateStart + "' ,'" + leaving.employee_code + "','" + leaving.absence_code + "'," + day + "," + leaving.absence_hour + ",0,to_char(SYSDATE,'MON-YY')," +
                        "" + leaving.absence_status + ",'" + leaving.absence_detail + "','token',SYSDATE,1165,SYSDATE,1165)";

                    System.Diagnostics.Debug.WriteLine(command);

                    cmd.CommandText = command;

                    cmd.ExecuteNonQuery();


                }
            }
            else
            {

                DateTime Start = DateTime.ParseExact(leaving.absence_day_start, "MM/dd/yyyy", null);
                string dateStart = Start.ToString("dd/MMM/yyyy");
                var dayCount = 0;
                System.Diagnostics.Debug.WriteLine("////////////////////////////////////////////////////////////////////");
                System.Diagnostics.Debug.WriteLine("วันที่ลา:" + dateStart);
                System.Diagnostics.Debug.WriteLine("Card Row:" + leaving.card_row);
                System.Diagnostics.Debug.WriteLine("Document ID:" + docId);
                System.Diagnostics.Debug.WriteLine("วันที่ลา:" + dateStart);
                System.Diagnostics.Debug.WriteLine("รหัสพนักงาน:" + leaving.employee_code);
                System.Diagnostics.Debug.WriteLine("ประเภทการลา:" + leaving.absence_code);
                System.Diagnostics.Debug.WriteLine("จำนวนวัน:" + leaving.absence_day);

                System.Diagnostics.Debug.WriteLine("จำนวนชั่วโมง:" + leaving.absence_hour);
                System.Diagnostics.Debug.WriteLine("สถานะการลา" + leaving.absence_status);
                System.Diagnostics.Debug.WriteLine("รายละเอียด:" + leaving.absence_detail);
                System.Diagnostics.Debug.WriteLine("Token" + leaving.absence_token);
                System.Diagnostics.Debug.WriteLine("รวมวันลา" + dayCount + "วัน");

                string command = "INSERT INTO sf_per_absence_mobile (" +
                    "absence_document," +
                    "absence_date, " +
                    "employee_code, " +
                    "absence_code, " +
                    "absence_day," +
                    "absence_hour," +
                    "delete_mark," +
                    "absence_period," +
                    "absence_status," +
                    "absence_detail," +
                    "absence_token," +
                    "CREATION_DATE," +
                    "CREATED_BY," +
                    "LAST_UPDATE_DATE," +
                    "LAST_UPDATED_BY) VALUES('" + docId + "','" + dateStart + "' ,'" + leaving.employee_code + "','" + leaving.absence_code + "'," + dayCount + "," + leaving.absence_hour + ",0,to_char(SYSDATE,'MON-YY')," +
                    "" + leaving.absence_status + ",'" + leaving.absence_detail + "','token',SYSDATE,1165,SYSDATE,1165)";

                System.Diagnostics.Trace.WriteLine(command);

                cmd.CommandText = command;

                cmd.ExecuteNonQuery();


            }


            _connection.Close();
            sendEmailToBoss(leaving.depart_code, leaving.divi_code, leaving.sect_code, name, positionGroupCode);

            return RedirectToAction("Index");

        }

        public IActionResult LeaveSickAll()
        {
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username:" + username);

            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT EMPLOYEE_CODE,position_group_code,depart_code,divi_code,sect_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";
            _connection.Open();
            OracleDataReader readeruser = cmd.ExecuteReader();
            string departCode = "";
            string diviCode = "";
            string sectCode = "";
            string positionGroupCode = "";

            string columFilter = "";
            string nameFilter = "";

            while (readeruser.Read())
            {
                employeeCode = Convert.ToString(readeruser["EMPLOYEE_CODE"]);
                departCode = Convert.ToString(readeruser["depart_code"]);
                diviCode = Convert.ToString(readeruser["divi_code"]);
                sectCode = Convert.ToString(readeruser["sect_code"]);
                positionGroupCode = Convert.ToString(readeruser["position_group_code"]);
            }
            _connection.Close();

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
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (sectCode == "5211" || sectCode == "5212" || diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }

            if (sectCode == "5251")
            {


                List<LeavingViewModel> list = new List<LeavingViewModel>();
                List<HealthyModel> healthyModels = new List<HealthyModel>();
                cmd.CommandText = "SELECT " +
                    "a.START_DATE," +
                    "a.END_DATE," +
                    "a.COUNT_DATE," +
                    "a.name," +
                    "a.EMPLOYEE_CODE," +
                    "a.ABSENCE_CODE," +
                    "a.ABSENCE_DAY," +
                    "a.ABSENCE_HOUR," +
                    "a.DELETE_MARK," +
                    "a.REVIEW," +
                    "a.APPROVE," +
                    "a.ABSENCE_PERIOD," +
                    "a.ABSENCE_STATUS," +
                    "a.ABSENCE_TOKEN," +
                    "a.ABSENCE_DETAIL," +
                    "a.ABSENCE_DOCUMENT," +
                    "a.CREATION_DATE," +
                    "a.STATUS_APPROVE," +
                    "a.DEPART_CODE," +
                    "a.DIVI_CODE," +
                    "a.SECT_CODE " +
                    "FROM sf_per_absence_moble_v a " +
                    "WHERE a.ABSENCE_CODE in('11','BA') " +
                    "AND TO_DATE(a.creation_date) = TO_DATE(SYSDATE)" +
                    "ORDER BY a.creation_date DESC";

                // System.Diagnostics.Trace.WriteLine("SQL:" + cmd.CommandText);
                _connection.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LeavingViewModel leaving = new LeavingViewModel();
                    if (Convert.ToString(reader["ABSENCE_HOUR"]) == "0.3")
                    {
                        leaving.ABSENCE_HOUR = "0.5";
                    }
                    else
                    {
                        if (Convert.ToString(reader["ABSENCE_HOUR"]).Contains("."))
                        {
                            leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]).Split(".")[0] + ".5";
                        }
                        else
                        {
                            leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                        }

                    }


                    leaving.START_DATE = Convert.ToString(reader["START_DATE"]);
                    leaving.END_DATE = Convert.ToString(reader["END_DATE"]);
                    leaving.COUNT_DATE = Convert.ToString(reader["COUNT_DATE"]);
                    leaving.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                    leaving.NAME = Convert.ToString(reader["NAME"]);
                    leaving.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                    leaving.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);

                    leaving.DELETE_MARK = Convert.ToString(reader["DELETE_MARK"]);
                    leaving.REVIEW = Convert.ToString(reader["REVIEW"]);
                    leaving.APPROVE = Convert.ToString(reader["APPROVE"]);
                    leaving.ABSENCE_PERIOD = Convert.ToString(reader["ABSENCE_PERIOD"]);
                    leaving.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                    leaving.ABSENCE_TOKEN = Convert.ToString(reader["ABSENCE_TOKEN"]);
                    leaving.ABSENCE_DETAIL = Convert.ToString(reader["ABSENCE_DETAIL"]);
                    leaving.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                    leaving.CREATION_DATE = Convert.ToString(reader["CREATION_DATE"]);
                    leaving.STATUS_APPROVE = Convert.ToString(reader["STATUS_APPROVE"]);
                    leaving.DIVI_CODE = Convert.ToString(reader["DIVI_CODE"]);
                    leaving.SECT_CODE = Convert.ToString(reader["SECT_CODE"]);
                    leaving.DEPART_CODE = Convert.ToString(reader["DEPART_CODE"]);

                    list.Add(leaving);



                }


                ViewBag.departName = nameFilter;
                TempData["dataToSend"] = "leaveSickAllPage";



                _connection.Close();


                return View(list);
            }
            else
            {

                return RedirectToAction("ContactSick");
            }


        }


        public IActionResult LeaveSickAllYear(string employeeCode)
        {
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username:" + username);

            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT EMPLOYEE_CODE,position_group_code,depart_code,divi_code,sect_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";
            _connection.Open();
            OracleDataReader readeruser = cmd.ExecuteReader();
            string departCode = "";
            string diviCode = "";
            string sectCode = "";
            string positionGroupCode = "";

            string columFilter = "";
            string nameFilter = "";

            while (readeruser.Read())
            {
                employeeCode = Convert.ToString(readeruser["EMPLOYEE_CODE"]);
                departCode = Convert.ToString(readeruser["depart_code"]);
                diviCode = Convert.ToString(readeruser["divi_code"]);
                sectCode = Convert.ToString(readeruser["sect_code"]);
                positionGroupCode = Convert.ToString(readeruser["position_group_code"]);
            }
            _connection.Close();

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
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (sectCode == "5211" || sectCode == "5212" || diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }

            if (sectCode == "5251")
            {


                List<LeavingViewModel> list = new List<LeavingViewModel>();
                List<HealthyModel> healthyModels = new List<HealthyModel>();
                cmd.CommandText = "SELECT " +
                    "a.START_DATE," +
                    "a.END_DATE," +
                    "a.COUNT_DATE," +
                    "a.name," +
                    "a.EMPLOYEE_CODE," +
                    "a.ABSENCE_CODE," +
                    "a.ABSENCE_DAY," +
                    "a.ABSENCE_HOUR," +
                    "a.DELETE_MARK," +
                    "a.REVIEW," +
                    "a.APPROVE," +
                    "a.ABSENCE_PERIOD," +
                    "a.ABSENCE_STATUS," +
                    "a.ABSENCE_TOKEN," +
                    "a.ABSENCE_DETAIL," +
                    "a.ABSENCE_DOCUMENT," +
                    "a.CREATION_DATE," +
                    "a.STATUS_APPROVE," +
                    "a.DEPART_CODE," +
                    "a.DIVI_CODE," +
                    "a.SECT_CODE " +
                    "FROM sf_per_absence_moble_v a " +
                    "WHERE a.ABSENCE_CODE in('11','BA') " +
                    /*"and TO_DATE('01-' || a.absence_period, 'DD-MON-YY') BETWEEN ADD_MONTHS(TRUNC(SYSDATE, 'MON'), -1) AND LAST_DAY(TRUNC(SYSDATE))"+*/


                    "ORDER BY a.creation_date DESC";

                // System.Diagnostics.Trace.WriteLine("SQL:" + cmd.CommandText);
                _connection.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LeavingViewModel leaving = new LeavingViewModel();
                    if (Convert.ToString(reader["ABSENCE_HOUR"]) == "0.3")
                    {
                        leaving.ABSENCE_HOUR = "0.5";
                    }
                    else
                    {
                        if (Convert.ToString(reader["ABSENCE_HOUR"]).Contains("."))
                        {
                            leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]).Split(".")[0] + ".5";
                        }
                        else
                        {
                            leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                        }

                    }


                    leaving.START_DATE = Convert.ToString(reader["START_DATE"]);
                    leaving.END_DATE = Convert.ToString(reader["END_DATE"]);
                    leaving.COUNT_DATE = Convert.ToString(reader["COUNT_DATE"]);
                    leaving.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                    leaving.NAME = Convert.ToString(reader["NAME"]);
                    leaving.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                    leaving.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);

                    leaving.DELETE_MARK = Convert.ToString(reader["DELETE_MARK"]);
                    leaving.REVIEW = Convert.ToString(reader["REVIEW"]);
                    leaving.APPROVE = Convert.ToString(reader["APPROVE"]);
                    leaving.ABSENCE_PERIOD = Convert.ToString(reader["ABSENCE_PERIOD"]);
                    leaving.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                    leaving.ABSENCE_TOKEN = Convert.ToString(reader["ABSENCE_TOKEN"]);
                    leaving.ABSENCE_DETAIL = Convert.ToString(reader["ABSENCE_DETAIL"]);
                    leaving.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                    leaving.CREATION_DATE = Convert.ToString(reader["CREATION_DATE"]);
                    leaving.STATUS_APPROVE = Convert.ToString(reader["STATUS_APPROVE"]);
                    leaving.DIVI_CODE = Convert.ToString(reader["DIVI_CODE"]);
                    leaving.SECT_CODE = Convert.ToString(reader["SECT_CODE"]);
                    leaving.DEPART_CODE = Convert.ToString(reader["DEPART_CODE"]);

                    list.Add(leaving);



                }


                ViewBag.departName = nameFilter;
                TempData["dataToSend"] = "leaveSickAllPage";



                _connection.Close();


                return View(list);
            }
            else
            {

                return RedirectToAction("ContactSick");
            }


        }
        public IActionResult CreateSick()
        {
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username:" + username);

            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT EMPLOYEE_CODE,position_group_code,depart_code,divi_code,sect_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";
            _connection.Open();
            OracleDataReader readeruser = cmd.ExecuteReader();
            string departCode = "";
            string diviCode = "";
            string sectCode = "";
            string positionGroupCode = "";

            string columFilter = "";
            string nameFilter = "";

            while (readeruser.Read())
            {
                employeeCode = Convert.ToString(readeruser["EMPLOYEE_CODE"]);
                departCode = Convert.ToString(readeruser["depart_code"]);
                diviCode = Convert.ToString(readeruser["divi_code"]);
                sectCode = Convert.ToString(readeruser["sect_code"]);
                positionGroupCode = Convert.ToString(readeruser["position_group_code"]);
            }
            _connection.Close();

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
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (sectCode == "5211" || sectCode == "5212" || diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }


            if (sectCode == "5251")
            {
                return View();
            }
            else
            {
                return RedirectToAction("ContactSick");
            }

        }
        [HttpPost]
        public IActionResult CreateSick(LeavingModel leaving)
        {

            ///Document ID////////////////////////
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random randNum = new Random();
            char[] chars = new char[8];
            for (int i = 0; i < 8; i++)
            {
                chars[i] = allowedChars[(int)((allowedChars.Length) * randNum.NextDouble())];
            }
            string docId = new string(chars);
            ViewBag.docId = docId;

            ///Employee Code////////////////////////

            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "select employee_code,sect_code,divi_code,depart_code,'คุณ'||first_name||' '||last_name name,position_group_code from sf_per_employees_v where resign_date is null and card_raw = :card_raw";
            cmd.Parameters.Add("card_raw", leaving.card_row);
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            string name = "";
            string positionGroupCode = "";
            while (reader.Read())
            {
                leaving.employee_code = Convert.ToString(reader["employee_code"]);
                leaving.sect_code = Convert.ToString(reader["sect_code"]);
                leaving.divi_code = Convert.ToString(reader["divi_code"]);
                leaving.depart_code = Convert.ToString(reader["depart_code"]);
                name = Convert.ToString(reader["name"]);
                positionGroupCode = Convert.ToString(reader["POSITION_GROUP_CODE"]);

            }
            /////End Employee Code////////////////////////////////////////////


            //ลามากกว่า 1 วัน
            if (leaving.absence_day_start == null)
            {
                string[] substring = leaving.absence_day.Split(" - ");

                var dateLeave = substring[0];
                var dateLeaveEnd = substring[1];
                ////////////////////////////////////////////////


                ///Date Start Leave////////////////////////
                DateTime date1 = DateTime.ParseExact(dateLeave, "MM/dd/yyyy", null);
                System.Diagnostics.Debug.WriteLine("Date1:" + date1);

                DateTime date2 = DateTime.ParseExact(dateLeaveEnd, "MM/dd/yyyy", null);
                System.Diagnostics.Debug.WriteLine("Date2:" + date2);

                DateTime Start = DateTime.ParseExact(dateLeave, "MM/dd/yyyy", null);

                string dateStart = Start.ToString("dd/MMM/yyyy");


                TimeSpan duration = (date2 - date1);

                System.Diagnostics.Debug.WriteLine("Duration:" + duration);

                int dayCount = duration.Days + 1;

                ////////////////////////////////////////////////


                ///Day Or Hour////////////////////////
                var day = 0;
                if (leaving.absence_hour > 0 && leaving.absence_hour < 8)
                {
                    day = 0;
                }
                else if (leaving.absence_hour >= 8)
                {
                    day = 0;
                    leaving.absence_hour = 0.3;
                }
                else
                {
                    day = 1;
                    leaving.absence_hour = 0;
                }


                System.Diagnostics.Debug.WriteLine("////////////////////////////////////////////////////////////////////");
                System.Diagnostics.Debug.WriteLine("Start date>>>" + leaving.absence_day_start);
                System.Diagnostics.Debug.WriteLine("Card Row:" + leaving.card_row);
                System.Diagnostics.Debug.WriteLine("Document ID:" + docId);
                System.Diagnostics.Debug.WriteLine("วันที่ลา:" + dateStart);
                System.Diagnostics.Debug.WriteLine("รหัสพนักงาน:" + leaving.employee_code);
                System.Diagnostics.Debug.WriteLine("ประเภทการลา:" + leaving.absence_code);
                System.Diagnostics.Debug.WriteLine("จำนวนวัน:" + leaving.absence_day);

                System.Diagnostics.Debug.WriteLine("จำนวนชั่วโมง:" + leaving.absence_hour);
                System.Diagnostics.Debug.WriteLine("สถานะการลา" + leaving.absence_status);
                System.Diagnostics.Debug.WriteLine("รายละเอียด:" + leaving.absence_detail);
                System.Diagnostics.Debug.WriteLine("Token" + leaving.absence_token);
                System.Diagnostics.Debug.WriteLine("รวมวันลา" + dayCount + "วัน");


                for (int i = 0; i < dayCount; i++)
                {
                    dateStart = Start.AddDays(i).ToString("dd-MMM-yyyy");
                    System.Diagnostics.Debug.WriteLine("วันที่ลา:" + dateStart);

                    string command = "INSERT INTO sf_per_absence_mobile (" +
                        "absence_document," +
                        "absence_date, " +
                        "employee_code, " +
                        "absence_code, " +
                        "absence_day," +
                        "absence_hour," +
                        "delete_mark," +
                        "absence_period," +
                        "absence_status," +
                        "absence_detail," +
                        "absence_token," +
                        "CREATION_DATE," +
                        "CREATED_BY," +
                        "LAST_UPDATE_DATE," +
                        "LAST_UPDATED_BY) VALUES('" + docId + "','" + dateStart + "' ,'" + leaving.employee_code + "','" + leaving.absence_code + "'," + day + "," + leaving.absence_hour + ",0,to_char(SYSDATE,'MON-YY')," +
                        "" + leaving.absence_status + ",'" + leaving.absence_detail + "','token',SYSDATE,1165,SYSDATE,1165)";

                    System.Diagnostics.Debug.WriteLine(command);

                    cmd.CommandText = command;

                    cmd.ExecuteNonQuery();


                }
            }
            else
            {

                DateTime Start = DateTime.ParseExact(leaving.absence_day_start, "MM/dd/yyyy", null);
                string dateStart = Start.ToString("dd/MMM/yyyy");
                var dayCount = 0;
                System.Diagnostics.Debug.WriteLine("////////////////////////////////////////////////////////////////////");
                System.Diagnostics.Debug.WriteLine("วันที่ลา:" + dateStart);
                System.Diagnostics.Debug.WriteLine("Card Row:" + leaving.card_row);
                System.Diagnostics.Debug.WriteLine("Document ID:" + docId);
                System.Diagnostics.Debug.WriteLine("วันที่ลา:" + dateStart);
                System.Diagnostics.Debug.WriteLine("รหัสพนักงาน:" + leaving.employee_code);
                System.Diagnostics.Debug.WriteLine("ประเภทการลา:" + leaving.absence_code);
                System.Diagnostics.Debug.WriteLine("จำนวนวัน:" + leaving.absence_day);

                System.Diagnostics.Debug.WriteLine("จำนวนชั่วโมง:" + leaving.absence_hour);
                System.Diagnostics.Debug.WriteLine("สถานะการลา" + leaving.absence_status);
                System.Diagnostics.Debug.WriteLine("รายละเอียด:" + leaving.absence_detail);
                System.Diagnostics.Debug.WriteLine("Token" + leaving.absence_token);
                System.Diagnostics.Debug.WriteLine("รวมวันลา" + dayCount + "วัน");

                string command = "INSERT INTO sf_per_absence_mobile (" +
                    "absence_document," +
                    "absence_date, " +
                    "employee_code, " +
                    "absence_code, " +
                    "absence_day," +
                    "absence_hour," +
                    "delete_mark," +
                    "absence_period," +
                    "absence_status," +
                    "absence_detail," +
                    "absence_token," +
                    "CREATION_DATE," +
                    "CREATED_BY," +
                    "LAST_UPDATE_DATE," +
                    "LAST_UPDATED_BY) VALUES('" + docId + "','" + dateStart + "' ,'" + leaving.employee_code + "','" + leaving.absence_code + "'," + dayCount + "," + leaving.absence_hour + ",0,to_char(SYSDATE,'MON-YY')," +
                    "" + leaving.absence_status + ",'" + leaving.absence_detail + "','token',SYSDATE,1165,SYSDATE,1165)";

                System.Diagnostics.Debug.WriteLine(command);

                cmd.CommandText = command;

                cmd.ExecuteNonQuery();


            }

            _connection.Close();



            sendEmailToBoss(leaving.depart_code, leaving.divi_code, leaving.sect_code, name, positionGroupCode);

            return RedirectToAction("LeaveSickAll");

        }

        public IActionResult ContactSick()
        {
            return View();
        }




        private void sendEmailToHR(string documentID, string employeeCode)
        {
            String name = "";
            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "select first_name ||' '|| last_name mame from sf_per_employees_v where employee_code = :employee_code";
            cmd.Parameters.Add("employee_code", employeeCode);
            _connection.Open();

            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                name = Convert.ToString(reader["mame"]);
            }
            _connection.Close();

            System.Diagnostics.Debug.WriteLine("leavingAlert?documentNo=" + documentID + "&name=" + name + "&typeDocument=2");

            HttpResponseMessage response = _client.GetAsync("leavingAlert?documentNo=" + documentID + "&name=" + name).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                System.Diagnostics.Debug.WriteLine(result);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

        }

        private void sendEmailToBoss(string depart, string diviCode, string sectCode, string name, string groupPosition)
        {
            System.Diagnostics.Trace.WriteLine("Depart:" + depart);
            System.Diagnostics.Trace.WriteLine("Divi Code:" + diviCode);
            System.Diagnostics.Trace.WriteLine("Sect Code:" + sectCode);
            System.Diagnostics.Trace.WriteLine("Name:" + name);
            System.Diagnostics.Trace.WriteLine("groupPosition:" + groupPosition);

            using (OracleCommand cmd = new OracleCommand())
            {
                System.Diagnostics.Trace.WriteLine("Send Email To Boss");
                System.Diagnostics.Trace.WriteLine("groupPosition:" + groupPosition);
                String sql = "";
                cmd.Connection = _connection;
                if (int.Parse(groupPosition) < 20)  // รายวัน   หัวหน้าแผนกอนุมัติ
                {
                    sql = "SELECT e.email_address FROM sf_per_employees_fnduser_v e " +
                        "WHERE e.sect_code = " + sectCode + " " +
                        "and e.email_address is not null and " +
                        "e.position_group_code = 032 " +
                        "and e.email_address is not null " +
                        "and e.resign_date is null";


                    cmd.CommandText = sql;
                }
                else if (int.Parse(groupPosition) > 19 && int.Parse(groupPosition) < 30) //รายเดือน หัวแผนกทบทวน ผู้จัดการฝ่ายอนุมัติ
                {
                    sql = "SELECT e.email_address FROM sf_per_employees_fnduser_v e " +
                        "WHERE e.divi_code = '" + diviCode + "' " +
                        "and e.position_group_code = 042 and e.email_address is not null " +
                        "UNION ALL " +
                        "SELECT e.email_address   FROM sf_per_employees_fnduser_v e WHERE e.sect_code = '" + sectCode + "' " +
                        "and e.position_group_code = 032 " +
                        "and e.email_address is not null";
                    cmd.CommandText = sql;

                }
                else if (int.Parse(groupPosition) > 29 && int.Parse(groupPosition) < 40) //รายเดือน หัวหน้าแผนก  ผู้จัดการส่วนทบทวน ผู้จัดการฝ่ายอนุมัติ
                {
                    sql = "SELECT e.email_address FROM sf_per_employees_fnduser_v e " +
                       "WHERE e.depart_code = '" + depart + "' " +
                       "and e.position_group_code = 052 and e.email_address is not null " +
                       "UNION ALL " +
                       "SELECT e.email_address FROM sf_per_employees_fnduser_v e WHERE e.divi_code = '" + diviCode + "' " +
                       "and e.position_group_code = 042 " +
                       "and e.email_address is not null";
                    cmd.CommandText = sql;
                }
                else if (int.Parse(groupPosition) > 39 && int.Parse(groupPosition) < 50) //รายเดือน ผู้จัดการส่วน ผู้จัดการฝ่ายทบทวน ผู้จัดการฝ่ายอนุมัติ
                {
                    sql = "SELECT e.email_address FROM sf_per_employees_fnduser_v e " +
                       "WHERE e.depart_code = '" + depart + "' " +
                       "and e.position_group_code = 052 and e.email_address is not null ";
                    cmd.CommandText = sql;

                }
                System.Diagnostics.Trace.WriteLine("SQL GET BOSS:" + sql);

                _connection.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string email = Convert.ToString(reader["email_address"]);
                    System.Diagnostics.Debug.WriteLine("Email:" + email);
                    HttpResponseMessage response = _client.GetAsync("leavingAlertBoss?email=" + email + "&name=" + name).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        System.Diagnostics.Debug.WriteLine("Send Email To Boss :" + email);
                        System.Diagnostics.Debug.WriteLine(result);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    }

                }

            }

        }
        private void InsertAbsenceAPI(string id)
        {
            HttpResponseMessage response = _client2.GetAsync("sfi-hr/insertAbsence.php?documentNo=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                System.Diagnostics.Debug.WriteLine(result);


            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }
        private void InsertAbsence(string id)
        {

            using (OracleCommand cmd = _connection.CreateCommand())
            {
                _connection.Open();
                cmd.CommandText = "SELECT * FROM sf_per_absence_mobile a where a.absence_document = '" + id + "'";
                OracleDataReader reader2 = cmd.ExecuteReader();

                System.Diagnostics.Debug.WriteLine("บันทึกข้อมูลจริง");
                var i = 0;


                while (reader2.Read())
                {
                    string columeDetail = "";

                    if (reader2["ABSENCE_CODE"].ToString() == "BA" || (reader2["ABSENCE_CODE"].ToString() == "11"))
                    {
                        columeDetail = "SICK_DESC";

                    }
                    else
                    {
                        columeDetail = "LEAVE_DESC";

                    }

                    System.Diagnostics.Debug.WriteLine("Colume Detail:" + columeDetail);

                    string sqlInsert = "INSERT INTO sf_per_absence (" +
                        "ABSENCE_DATE," +
                        "EMPLOYEE_CODE," +
                        "ABSENCE_CODE," +
                        "ABSENCE_DAY," +
                        "ABSENCE_HOUR," +
                        "ABSENCE_COMMENT," +
                        "DELETE_MARK," +
                        "CREATION_DATE," +
                        "CREATED_BY," +
                        "LAST_UPDATE_DATE," +
                        "LAST_UPDATED_BY," +
                        "ABSENCE_PERIOD," +
                        "" + columeDetail + "," +
                        "ABSENCE_DOCUMENT) " +
                        "VALUES (" +
                        "TO_DATE('" + reader2["ABSENCE_DATE"].ToString().Split(" ")[0] + "','DD-MM-YY')," +
                        "'" + reader2["EMPLOYEE_CODE"] + "'," +
                        "'" + reader2["ABSENCE_CODE"] + "'," +
                        "'" + reader2["ABSENCE_DAY"] + "'," +
                        "'" + reader2["ABSENCE_HOUR"] + "'," +
                        "'0'," +
                        "0," +
                        "SYSDATE," +
                        "'8888'," +
                        "SYSDATE," +
                        "'8888'," +
                        "to_char(SYSDATE,'MON-YY')," +
                        "'" + reader2["ABSENCE_DETAIL"] + "'," +
                        "'" + reader2["ABSENCE_DOCUMENT"] + "')";
                    System.Diagnostics.Debug.WriteLine(sqlInsert);


                    cmd.CommandText = sqlInsert;
                    cmd.ExecuteNonQuery();
                    i++;
                    System.Diagnostics.Debug.WriteLine("บันทึกข้อมูลจริง" + i);


                }
                _connection.Close();
            }

        }

        [HttpGet]
        public IActionResult Details(string? id)
        {

            string receivedPage = TempData["dataToSend"] as string;
            TempData.Remove("leaveSickAllPage");

            System.Diagnostics.Debug.WriteLine("Document ID:" + id);


            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT " +
                "a.START_DATE,a.END_DATE," +
                "a.COUNT_DATE," +
                "b.first_name||' '||" +
                "b.last_name name," +
                "a.EMPLOYEE_CODE," +
                "a.ABSENCE_CODE,a.ABSENCE_DAY," +
                "a.ABSENCE_HOUR,a.DELETE_MARK," +
                "a.REVIEW," +
                "a.APPROVE," +
                "a.ABSENCE_PERIOD," +
                "a.ABSENCE_STATUS," +
                "a.ABSENCE_TOKEN," +
                "a.ABSENCE_DETAIL," +
                "a.ABSENCE_DOCUMENT," +
                "a.CREATION_DATE," +
                "a.STATUS_APPROVE," +
                "a.DEPART_CODE," +
                "a.DIVI_CODE," +
                "a.SECT_CODE, " +
                "b.POSITION_NAME, " +
                "a.DATE_REVIEW, " +
                "a.DATE_APPROVE " +
                "FROM " +
                "sf_per_absence_moble_v a," +
                "sf_per_employees_v b " +
                "WHERE a.employee_code = b.employee_code " +
                "and a.absence_document = '" + id + "' " +
                "ORDER BY a.creation_date DESC";
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            LeavingViewModel leaving = new LeavingViewModel();
            while (reader.Read())
            {
                string imgUrl = "http://10.2.2.5/img/sfi/";

                imgUrl = imgUrl + Convert.ToString(reader["EMPLOYEE_CODE"]).Substring(0, 2) + "-" + Convert.ToString(reader["EMPLOYEE_CODE"]).Substring(2, 4) + ".jpg";

                ViewBag.imgUrl = imgUrl;

                System.Diagnostics.Debug.WriteLine(imgUrl);

                if (Convert.ToString(reader["ABSENCE_HOUR"]) == "0.3")
                {
                    leaving.ABSENCE_HOUR = "0.5";
                }
                else
                {
                    if (Convert.ToString(reader["ABSENCE_HOUR"]).Contains("."))
                    {
                        leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]).Split(".")[0] + ".5";
                    }
                    else
                    {
                        leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                    }

                }

                if (double.Parse(leaving.ABSENCE_HOUR) > 0)
                {
                    leaving.COUNT_DATE = "0";
                }
                else
                {
                    leaving.COUNT_DATE = Convert.ToString(reader["COUNT_DATE"]);
                }


                leaving.START_DATE = Convert.ToString(reader["START_DATE"]);
                leaving.END_DATE = Convert.ToString(reader["END_DATE"]);

                leaving.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                leaving.NAME = Convert.ToString(reader["NAME"]);
                leaving.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                leaving.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);

                leaving.DELETE_MARK = Convert.ToString(reader["DELETE_MARK"]);
                leaving.REVIEW = Convert.ToString(reader["REVIEW"]);
                leaving.APPROVE = Convert.ToString(reader["APPROVE"]);
                leaving.ABSENCE_PERIOD = Convert.ToString(reader["ABSENCE_PERIOD"]);
                leaving.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                leaving.ABSENCE_TOKEN = Convert.ToString(reader["ABSENCE_TOKEN"]);
                leaving.ABSENCE_DETAIL = Convert.ToString(reader["ABSENCE_DETAIL"]);
                leaving.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                leaving.CREATION_DATE = Convert.ToString(reader["CREATION_DATE"]);
                leaving.STATUS_APPROVE = Convert.ToString(reader["STATUS_APPROVE"]);
                leaving.DIVI_CODE = Convert.ToString(reader["DIVI_CODE"]);
                leaving.NAME = Convert.ToString(reader["NAME"]);
                leaving.DEPART_CODE = Convert.ToString(reader["DEPART_CODE"]);
                leaving.SECT_CODE = Convert.ToString(reader["SECT_CODE"]);
                leaving.POSITION_NAME = Convert.ToString(reader["POSITION_NAME"]);
                leaving.DATE_REVIEW = Convert.ToString(reader["DATE_REVIEW"]);
                leaving.DATE_APPROVE = Convert.ToString(reader["DATE_APPROVE"]);
            }
            _connection.Close();




            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username>>>>:" + username);

            OracleCommand command = _connection.CreateCommand();
            command.CommandText = "SELECT EMPLOYEE_CODE,position_group_code,depart_code,divi_code,sect_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";
            _connection.Open();
            OracleDataReader readeruser = command.ExecuteReader();

            string sectCode = "";
            string diviCode = "";
            string departCode = "";
            string positionGroupCode = "";
            while (readeruser.Read())
            {
                employeeCode = Convert.ToString(readeruser["EMPLOYEE_CODE"]);

                sectCode = Convert.ToString(readeruser["SECT_CODE"]);
                diviCode = Convert.ToString(readeruser["DIVI_CODE"]);
                departCode = Convert.ToString(readeruser["DEPART_CODE"]);
                positionGroupCode = Convert.ToString(readeruser["POSITION_GROUP_CODE"]);

            }
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
                ViewBag.sectCode = "5251";
            }

            System.Diagnostics.Debug.WriteLine("sectCode>>>>:" + sectCode);
            System.Diagnostics.Debug.WriteLine("DiviCode>>>>:" + diviCode);

            //admin chp
            if (sectCode == "5211" || sectCode == "5212" || diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }

            //admin bkk
            if (departCode == "7000" && positionGroupCode == "042")
            {
                ViewBag.AdminBkk = "Visable";
            }



            OracleCommand cmd2 = _connection.CreateCommand();
            cmd2.CommandText = "select a.document_id, " +
                "a.status_healthy," +
                "a.detail," +
                "a.last_update_date," +
                "b.name " +
                "from SF_PER_EMPLOYEE_HEALTHY a, sf_personal_healthy b  where b.id_personal_healthy = a.id_personal_healthy and a.DOCUMENT_ID = '" + id + "'";
            OracleDataReader reader2 = cmd2.ExecuteReader();
            HealthyModel healthyModel = new HealthyModel();
            while (reader2.Read())
            {
                healthyModel.DOCUMENT_ID = Convert.ToString(reader2["DOCUMENT_ID"]);
                healthyModel.DETAIL = Convert.ToString(reader2["DETAIL"]);
                healthyModel.STATUS_HEALTHY = Convert.ToString(reader2["STATUS_HEALTHY"]);
                healthyModel.LAST_UPDATE_DATE = Convert.ToString(reader2["LAST_UPDATE_DATE"]);
                healthyModel.NAME_PERSONAL_HEALTHY = Convert.ToString(reader2["NAME"]);

            }
            ViewBag.healthyModel = healthyModel;
            ViewBag.Page = receivedPage;

            System.Diagnostics.Debug.WriteLine("page>>>>:" + receivedPage);



            _connection.Close();
            System.Diagnostics.Debug.WriteLine("sectCode>>>>:" + sectCode);

            return View(leaving);


        }



        public IActionResult Delete(string? id)

        {
            //delete with api
            HttpResponseMessage response = _client2.GetAsync("sfi-hr/DelectLeavingCard.php?leavDocument=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                System.Diagnostics.Debug.WriteLine(result);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }


            ViewBag.message = "ลบเอกสารสำเร็จ";

            /* if (id == null)
             {
                 return NotFound();
             }
             else
             {
                 if (getStatusApprove(id))
                 {
                     ViewBag.message = "ไม่สามารถลบเอกสารได้ เนื่องจากเอกสารได้รับการอนุมัติแล้ว";

                     System.Diagnostics.Debug.WriteLine("ลบเอกสารสำเร็จ");

                     OracleCommand cmd = _connection.CreateCommand();

                     cmd.CommandText = "DELETE FROM sf_per_absence_mobile WHERE absence_document = '" + id + "'";
                     _connection.Open();
                     cmd.ExecuteNonQuery();
                     _connection.Close();


                 }
                 else
                 {
                     ViewBag.message = "ลบเอกสารสำเร็จ";
                     System.Diagnostics.Debug.WriteLine("ไม่สามารถลบเอกสารได้ เนื่องจากเอกสารได้รับการอนุมัติแล้ว");


                 }

             }*/

            return RedirectToAction("Index");
        }
        public IActionResult DeleteSick(string? id)

        {
            ViewBag.message = "ลบเอกสารสำเร็จ";

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                if (getStatusApprove(id))
                {
                    ViewBag.message = "ไม่สามารถลบเอกสารได้ เนื่องจากเอกสารได้รับการอนุมัติแล้ว";

                    System.Diagnostics.Debug.WriteLine("ลบเอกสารสำเร็จ");

                    OracleCommand cmd = _connection.CreateCommand();

                    cmd.CommandText = "DELETE FROM sf_per_absence_mobile WHERE absence_document = '" + id + "'";
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();


                }
                else
                {
                    ViewBag.message = "ลบเอกสารสำเร็จ";
                    System.Diagnostics.Debug.WriteLine("ไม่สามารถลบเอกสารได้ เนื่องจากเอกสารได้รับการอนุมัติแล้ว");


                }

            }

            return RedirectToAction("LeaveSickAll");
        }


        private bool getStatusApprove(string id)
        {
            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT a.STATUS_APPROVE FROM sf_per_absence_moble_v a WHERE a.absence_document = '" + id + "'";
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            string status = "";
            while (reader.Read())
            {
                status = Convert.ToString(reader["STATUS_APPROVE"]);
            }
            _connection.Close();

            if (status == "approved")
            {
                return false;
            }
            else
            {
                return true;
            }


        }



        public IActionResult PDF(string id)
        {

            System.Diagnostics.Debug.WriteLine("Document ID:" + id);


            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT " +
                "a.START_DATE,a.END_DATE," +
                "a.COUNT_DATE," +
                "b.first_name||' '||" +
                "b.last_name name," +
                "a.EMPLOYEE_CODE," +
                "a.ABSENCE_CODE,a.ABSENCE_DAY," +
                "a.ABSENCE_HOUR,a.DELETE_MARK," +
                "a.REVIEW," +
                "a.APPROVE," +
                "a.ABSENCE_PERIOD," +
                "a.ABSENCE_STATUS," +
                "a.ABSENCE_TOKEN," +
                "a.ABSENCE_DETAIL," +
                "a.ABSENCE_DOCUMENT," +
                "a.CREATION_DATE," +
                "a.STATUS_APPROVE," +
                "a.DEPART_CODE," +
                "a.DIVI_CODE," +
                "a.SECT_CODE, " +
                "b.POSITION_NAME, " +
                "a.DATE_REVIEW, " +
                "a.DATE_APPROVE " +
                "FROM " +
                "sf_per_absence_moble_v a," +
                "sf_per_employees_v b " +
                "WHERE a.employee_code = b.employee_code " +
                "and a.absence_document = '" + id + "' " +
                "ORDER BY a.creation_date DESC";
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            LeavingViewModel leaving = new LeavingViewModel();
            while (reader.Read())
            {
                string imgUrl = "http://10.2.2.5/img/sfi/";

                imgUrl = imgUrl + Convert.ToString(reader["EMPLOYEE_CODE"]).Substring(0, 2) + "-" + Convert.ToString(reader["EMPLOYEE_CODE"]).Substring(2, 4) + ".jpg";

                ViewBag.imgUrl = imgUrl;

                System.Diagnostics.Debug.WriteLine(imgUrl);

                if (Convert.ToString(reader["ABSENCE_HOUR"]) == "0.3")
                { leaving.ABSENCE_HOUR = "0.5"; }
                else
                {
                    leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                }

                if (double.Parse(leaving.ABSENCE_HOUR) > 0)
                {
                    leaving.COUNT_DATE = "0";
                }
                else
                {
                    leaving.COUNT_DATE = Convert.ToString(reader["COUNT_DATE"]);
                }


                leaving.START_DATE = Convert.ToString(reader["START_DATE"]);
                leaving.END_DATE = Convert.ToString(reader["END_DATE"]);

                leaving.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                leaving.NAME = Convert.ToString(reader["NAME"]);
                leaving.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                leaving.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);

                leaving.DELETE_MARK = Convert.ToString(reader["DELETE_MARK"]);
                leaving.REVIEW = Convert.ToString(reader["REVIEW"]);
                leaving.APPROVE = Convert.ToString(reader["APPROVE"]);
                leaving.ABSENCE_PERIOD = Convert.ToString(reader["ABSENCE_PERIOD"]);
                leaving.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                leaving.ABSENCE_TOKEN = Convert.ToString(reader["ABSENCE_TOKEN"]);
                leaving.ABSENCE_DETAIL = Convert.ToString(reader["ABSENCE_DETAIL"]);
                leaving.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                leaving.CREATION_DATE = Convert.ToString(reader["CREATION_DATE"]);
                leaving.STATUS_APPROVE = Convert.ToString(reader["STATUS_APPROVE"]);
                leaving.DIVI_CODE = Convert.ToString(reader["DIVI_CODE"]);
                leaving.NAME = Convert.ToString(reader["NAME"]);
                leaving.DEPART_CODE = Convert.ToString(reader["DEPART_CODE"]);
                leaving.SECT_CODE = Convert.ToString(reader["SECT_CODE"]);
                leaving.POSITION_NAME = Convert.ToString(reader["POSITION_NAME"]);
                leaving.DATE_REVIEW = Convert.ToString(reader["DATE_REVIEW"]);
                leaving.DATE_APPROVE = Convert.ToString(reader["DATE_APPROVE"]);
            }
            _connection.Close();




            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            System.Diagnostics.Debug.WriteLine("Username>>>>:" + username);

            OracleCommand command = _connection.CreateCommand();
            command.CommandText = "SELECT EMPLOYEE_CODE,position_group_code,depart_code,divi_code,sect_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";
            _connection.Open();
            OracleDataReader readeruser = command.ExecuteReader();

            string sectCode = "";
            while (readeruser.Read())
            {
                employeeCode = Convert.ToString(readeruser["EMPLOYEE_CODE"]);

                sectCode = Convert.ToString(readeruser["SECT_CODE"]);

            }

            OracleCommand cmd2 = _connection.CreateCommand();
            cmd2.CommandText = "select a.document_id, " +
                "a.status_healthy," +
                "a.detail," +
                "a.last_update_date," +
                "b.name " +
                "from SF_PER_EMPLOYEE_HEALTHY a, sf_personal_healthy b  where b.id_personal_healthy = a.id_personal_healthy and a.DOCUMENT_ID = '" + id + "'";
            OracleDataReader reader2 = cmd2.ExecuteReader();
            HealthyModel healthyModel = new HealthyModel();
            while (reader2.Read())
            {
                healthyModel.DOCUMENT_ID = Convert.ToString(reader2["DOCUMENT_ID"]);
                healthyModel.DETAIL = Convert.ToString(reader2["DETAIL"]);
                healthyModel.STATUS_HEALTHY = Convert.ToString(reader2["STATUS_HEALTHY"]);
                healthyModel.LAST_UPDATE_DATE = Convert.ToString(reader2["LAST_UPDATE_DATE"]);
                healthyModel.NAME_PERSONAL_HEALTHY = Convert.ToString(reader2["NAME"]);

            }
            ViewBag.healthyModel = healthyModel;

            var viewModel = new LeavingHealthyModel
            {
                leavingViewModel = leaving,
                healthyModel = healthyModel
            };

            _connection.Close();
            System.Diagnostics.Debug.WriteLine("sectCode>>>>:" + sectCode);

            if (sectCode == "5251")
            {
                ViewBag.sectCode = "5251";
            }

            /*	return new ViewAsPdf(viewModel);*/


            return new ViewAsPdf(viewModel)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = { Left = 10, Bottom = 0, Right = 10, Top = 10 },
                /* CustomSwitches = "--disable-smart-shrinking",*/
                FileName = "" + id + ".pdf",

            };
        }



        private List<LeavingViewModel> GetDataLeavingModel(string? id)
        {

            using (OracleCommand cmd = new OracleCommand())
            {

                List<LeavingViewModel> list = new List<LeavingViewModel>();
                cmd.Connection = _connection;
                cmd.CommandText = "SELECT " +
                 "a.START_DATE,a.END_DATE," +
                 "a.COUNT_DATE,b.first_name||' '||" +
                 "b.last_name name,a.EMPLOYEE_CODE," +
                 "a.ABSENCE_CODE,a.ABSENCE_DAY," +
                 "a.ABSENCE_HOUR,a.DELETE_MARK," +
                 "a.REVIEW," +
                 "a.APPROVE," +
                 "a.ABSENCE_PERIOD," +
                 "a.ABSENCE_STATUS," +
                 "a.ABSENCE_TOKEN," +
                 "a.ABSENCE_DETAIL," +
                 "a.ABSENCE_DOCUMENT," +
                 "a.CREATION_DATE," +
                 "a.STATUS_APPROVE," +
                 "a.DEPART_CODE," +
                 "a.DIVI_CODE," +
                 "a.SECT_CODE " +
                 "a.POSITION_NAME " +
                 "FROM " +
                 "sf_per_absence_moble_v a," +
                 "sf_per_employees_v b " +
                 "WHERE a.employee_code = b.employee_code " +
                 "and a.absence_document = '" + id + "' " +
                 "ORDER BY a.creation_date DESC";
                _connection.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LeavingViewModel leaving = new LeavingViewModel();
                    leaving.START_DATE = Convert.ToString(reader["START_DATE"]);
                    leaving.END_DATE = Convert.ToString(reader["END_DATE"]);
                    leaving.COUNT_DATE = Convert.ToString(reader["COUNT_DATE"]);
                    leaving.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                    leaving.NAME = Convert.ToString(reader["NAME"]);
                    leaving.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                    leaving.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);
                    leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                    leaving.DELETE_MARK = Convert.ToString(reader["DELETE_MARK"]);
                    leaving.REVIEW = Convert.ToString(reader["REVIEW"]);
                    leaving.APPROVE = Convert.ToString(reader["APPROVE"]);
                    leaving.ABSENCE_PERIOD = Convert.ToString(reader["ABSENCE_PERIOD"]);
                    leaving.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                    leaving.ABSENCE_TOKEN = Convert.ToString(reader["ABSENCE_TOKEN"]);
                    leaving.ABSENCE_DETAIL = Convert.ToString(reader["ABSENCE_DETAIL"]);
                    leaving.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                    leaving.CREATION_DATE = Convert.ToString(reader["CREATION_DATE"]);
                    leaving.STATUS_APPROVE = Convert.ToString(reader["STATUS_APPROVE"]);
                    leaving.DIVI_CODE = Convert.ToString(reader["DIVI_CODE"]);

                    list.Add(leaving);

                }
                _connection.Close();
                return list;
            }

        }

        public IActionResult Approveleaving()
        {
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            string alertMessage = TempData["AlertMessage"] as string;
            if (!string.IsNullOrEmpty(alertMessage))
            {
                ViewBag.AlertMessage = alertMessage;
            }


            ApprovePath approvePath = new ApprovePath(_connection);
            string columFilter = approvePath.GetPosition(username);
            

            string sectCode = approvePath.sectCode;
            string diviCode = approvePath.diviCode;
            string departCode = approvePath.departCode;
            string nameFilter = approvePath.nameFilter;
            string positionGroupCode = approvePath.positionGroupCode;


            if (sectCode == "5251") //แผนกพยาบาล
            {
                ViewBag.Health = "Visable";
            }

            if (sectCode == "5211" || sectCode == "5212" || diviCode == "5210") //HR chp
            {
                ViewBag.Admin = "Visable";
            }

            System.Diagnostics.Debug.WriteLine("Colum Filter:" + columFilter);
            System.Diagnostics.Debug.WriteLine("Name Filter:" + nameFilter);

            List<LeavingViewModel> list = new List<LeavingViewModel>();

            string cmdText = "SELECT " +
                "a.START_DATE,a.END_DATE," +
                "a.COUNT_DATE,b.first_name||' '||" +
                "b.last_name name,a.EMPLOYEE_CODE," +
                "a.ABSENCE_CODE,a.ABSENCE_DAY," +
                "a.ABSENCE_HOUR,a.DELETE_MARK," +
                "a.REVIEW," +
                "a.APPROVE," +
                "a.ABSENCE_PERIOD," +
                "a.ABSENCE_STATUS," +
                "a.ABSENCE_TOKEN," +
                "a.ABSENCE_DETAIL," +
                "a.ABSENCE_DOCUMENT," +
                "a.CREATION_DATE," +
                "a.STATUS_APPROVE," +
                "a.DEPART_CODE," +
                "a.DIVI_CODE," +
                "a.SECT_CODE " +
                "FROM " +
                "sf_per_absence_moble_v a," +
                "sf_per_employees_v b " +
                "WHERE a.employee_code = b.employee_code " +
                "and " + columFilter + " and a.ABSENCE_STATUS < 2 " +
                "and b.position_group_code < " + positionGroupCode + " " +
                "ORDER BY a.creation_date DESC";
              System.Diagnostics.Debug.WriteLine(cmdText);
            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = cmdText;
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                LeavingViewModel leaving = new LeavingViewModel();
                leaving.START_DATE = Convert.ToString(reader["START_DATE"]);
                leaving.END_DATE = Convert.ToString(reader["END_DATE"]);
                leaving.COUNT_DATE = Convert.ToString(reader["COUNT_DATE"]);
                leaving.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                leaving.NAME = Convert.ToString(reader["NAME"]);
                leaving.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                leaving.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);
                leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                leaving.DELETE_MARK = Convert.ToString(reader["DELETE_MARK"]);
                leaving.REVIEW = Convert.ToString(reader["REVIEW"]);
                leaving.APPROVE = Convert.ToString(reader["APPROVE"]);
                leaving.ABSENCE_PERIOD = Convert.ToString(reader["ABSENCE_PERIOD"]);
                leaving.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                leaving.ABSENCE_TOKEN = Convert.ToString(reader["ABSENCE_TOKEN"]);
                leaving.ABSENCE_DETAIL = Convert.ToString(reader["ABSENCE_DETAIL"]);
                leaving.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                leaving.CREATION_DATE = Convert.ToString(reader["CREATION_DATE"]);
                leaving.STATUS_APPROVE = Convert.ToString(reader["STATUS_APPROVE"]);
                leaving.DIVI_CODE = Convert.ToString(reader["DIVI_CODE"]);
                leaving.DEPART_CODE = Convert.ToString(reader["DEPART_CODE"]);
                leaving.SECT_CODE = Convert.ToString(reader["SECT_CODE"]);
                list.Add(leaving);

            }


            ViewBag.departName = nameFilter;


            _connection.Close();

            return View(list);

        }

        public IActionResult UpdateReviewApprove(string? id)
        {
            string username = User.Identity!.Name!.Split('\\')[1].ToUpper();
            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT employee_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";

            _connection.Open();
            OracleDataReader readeruser = cmd.ExecuteReader();
            string employeeCode = "";
            while (readeruser.Read())
            {
                employeeCode = Convert.ToString(readeruser["EMPLOYEE_CODE"]);
            }
            _connection.Close();


            System.Diagnostics.Debug.WriteLine("Username:" + employeeCode);
            System.Diagnostics.Debug.WriteLine("Document ID:" + id);
            OracleCommand cmdUpdataStatus = _connection.CreateCommand();
            cmdUpdataStatus.CommandText = "UPDATE sf_per_absence_mobile SET ABSENCE_REVIEW = " + employeeCode + ",DATE_REVIEW = SYSDATE,ABSENCE_STATUS='1',STATUS_APPROVE = 'approved' WHERE absence_document = '" + id + "'";
            _connection.Open();
            var statusExcute = cmdUpdataStatus.ExecuteNonQuery();
            if (statusExcute > 0)
            {
                System.Diagnostics.Debug.WriteLine("Update Status Review Success");

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Update Status Review Fail");
            }
            _connection.Close();


            return RedirectToAction("Approveleaving");

        }
        public IActionResult UpdateReviewReject(string? id)
        {
            string username = User.Identity!.Name!.Split('\\')[1].ToUpper();
            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT employee_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";

            _connection.Open();
            OracleDataReader readeruser = cmd.ExecuteReader();
            string employeeCode = "";
            while (readeruser.Read())
            {
                employeeCode = Convert.ToString(readeruser["EMPLOYEE_CODE"]);
            }
            _connection.Close();

            cmd.CommandText = "UPDATE sf_per_absence_mobile SET ABSENCE_REVIEW = " + employeeCode + ",DATE_REVIEW = SYSDATE,ABSENCE_STATUS='3',STATUS_APPROVE = 'disapprove' WHERE absence_document = '" + id + "'";
            _connection.Open();
            var statusExcute = cmd.ExecuteNonQuery();
            if (statusExcute == 1)
            {
                System.Diagnostics.Debug.WriteLine("Update Status Review Reject Success");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Update Status Review Reject Fail");
            }
            _connection.Close();
            return RedirectToAction("Approveleaving");


        }

        public IActionResult UpdateApprove(string? id)
        {
            string username = User.Identity!.Name!.Split('\\')[1].ToUpper();
            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT employee_code,position_group_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";

            _connection.Open();
            OracleDataReader readeruser = cmd.ExecuteReader();
            string employeeCode = "";
            string employeeCodeMe = "";
            string name = "";
            string sectCode = "";
            string diviCode = "";
            string departCode = "";
            string positionGroupCodeBoss = "";
            string positionGroupCode = "";
            while (readeruser.Read())
            {
                employeeCode = Convert.ToString(readeruser["EMPLOYEE_CODE"]);
                positionGroupCodeBoss = Convert.ToString(readeruser["position_group_code"]);
            }



            cmd.CommandText = "SELECT b.position_group_code,b.employee_code,'คุณ'||b.first_name||' '||b.last_name name,b.sect_code,b.divi_code,b.depart_code FROM sf_per_absence_mobile a,sf_per_employees_v b where a.employee_code = b.employee_code and a.absence_document = '" + id + "'";

            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                positionGroupCode = Convert.ToString(reader["position_group_code"]);
                employeeCodeMe = Convert.ToString(reader["employee_code"]);
                sectCode = Convert.ToString(reader["sect_code"]);
                diviCode = Convert.ToString(reader["divi_code"]);
                departCode = Convert.ToString(reader["depart_code"]);
                name = Convert.ToString(reader["name"]);

            }

            System.Diagnostics.Debug.WriteLine("Position Group Code Boss:" + positionGroupCodeBoss);
            System.Diagnostics.Debug.WriteLine("Position Group Code ME:" + positionGroupCode);

            if (positionGroupCodeBoss == "032" && (positionGroupCode == "021" || positionGroupCode == "022"))
            {
                System.Diagnostics.Debug.WriteLine("ไม่สามารถอนุมัติเอกสารได้ เนื่องจากตำแหน่งของคุณต่ำกว่าผู้อนุมัติ");
                TempData["AlertMessage"] = "N";

            }
            else
            {
                cmd.CommandText = "select absence_review from sf_per_absence_mobile where absence_document = '" + id + " and  ROWNUM = 1'";
                OracleDataReader reader2 = cmd.ExecuteReader();
                string absenceReview = "";
                while (reader2.Read())
                {
                    absenceReview = Convert.ToString(reader2["ABSENCE_REVIEW"]);
                }
                System.Diagnostics.Debug.WriteLine("absenceReview:" + absenceReview);

                if (absenceReview == "")
                {
                    cmd.CommandText = "UPDATE sf_per_absence_mobile SET ABSENCE_REVIEW = " + employeeCode + ",ABSENCE_APPROVE = " + employeeCode + ",DATE_REVIEW = SYSDATE,DATE_APPROVE = SYSDATE,ABSENCE_STATUS='2',STATUS_APPROVE = 'approved' WHERE absence_document = '" + id + "'";

                    var statusExcute = cmd.ExecuteNonQuery();
                    if (statusExcute == 1)
                    {
                        System.Diagnostics.Debug.WriteLine("Update Status Review Success");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Update Status Review Fail");
                    }

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("อนุมัติเอกสารสำเร็จ");
                    cmd.CommandText = "UPDATE sf_per_absence_mobile SET ABSENCE_APPROVE = " + employeeCode + ",DATE_APPROVE = SYSDATE,ABSENCE_STATUS='2',STATUS_APPROVE = 'approved' WHERE absence_document = '" + id + "'";

                    var statusExcute = cmd.ExecuteNonQuery();
                    if (statusExcute == 1)
                    {
                        System.Diagnostics.Debug.WriteLine("Update Status Review Success");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Update Status Review Fail");
                    }

                }
                _connection.Close();




                InsertAbsenceAPI(id);
                /*InsertAbsence(id);*/


                sendEmailToHR(id, employeeCodeMe);

                TempData["AlertMessage"] = "Y";



            }
            return RedirectToAction("Approveleaving");

        }
        public IActionResult UpdateReject(string? id)
        {

            string username = User.Identity!.Name!.Split('\\')[1].ToUpper();
            string alertMessage = TempData["AlertMessage"] as string;
            if (!string.IsNullOrEmpty(alertMessage))
            {
                ViewBag.AlertMessage = alertMessage;
            }

            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT employee_code,position_group_code FROM sf_per_employees_fnduser_v where User_name = '" + username + "'";

            _connection.Open();
            OracleDataReader readeruser = cmd.ExecuteReader();
            string employeeCode = "";
            string positionGroupCodeBoss = "";
            string positionGroupCode = "";

            while (readeruser.Read())
            {
                employeeCode = Convert.ToString(readeruser["EMPLOYEE_CODE"]);
                positionGroupCodeBoss = Convert.ToString(readeruser["position_group_code"]);
            }
            _connection.Close();

            if (positionGroupCodeBoss == "032" && (positionGroupCode == "021" || positionGroupCode == "022"))
            {
                System.Diagnostics.Debug.WriteLine("ไม่สามารถอนุมัติเอกสารได้ เนื่องจากตำแหน่งของคุณต่ำกว่าผู้อนุมัติ");
                TempData["AlertMessage"] = "N";
                return RedirectToAction("Approveleaving");


            }
            else
            {
                cmd.CommandText = "UPDATE sf_per_absence_mobile SET ABSENCE_APPROVE = " + employeeCode + ",DATE_APPROVE = SYSDATE,ABSENCE_STATUS='3',STATUS_APPROVE = 'disapprove' WHERE absence_document = '" + id + "'";
                _connection.Open();
                var statusExcute = cmd.ExecuteNonQuery();
                if (statusExcute == 1)
                {
                    System.Diagnostics.Debug.WriteLine("Update Status Review Reject Success");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Update Status Review Reject Fail");
                }
                _connection.Close();
                TempData["AlertMessage"] = "Y";
                return RedirectToAction("Approveleaving");
            }
        }

        public IActionResult Heatlthy(string id)
        {
            OracleCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM SF_PER_EMPLOYEE_HEALTHY a where a.ID_PERSONAL_HEALTHY = '" + id + "'";
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Document ID:" + id);
                ViewBag.documentId = id;
                return View();
            }

            /*
                        System.Diagnostics.Debug.WriteLine("Document ID:" + id);
                        ViewBag.documentId = id;

                        return View();*/
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Heatlthy(HealthyModel healthyModel)
        {
            string name = User.Identity!.Name!.Split('\\')[1].ToUpper();
            string detail = healthyModel.DETAIL;
            string statusHealthy = healthyModel.STATUS_HEALTHY;
            string documentId = healthyModel.DOCUMENT_ID;
            string cardRow = healthyModel.ID_PERSONAL_HEALTHY;

            System.Diagnostics.Debug.WriteLine("Name:" + name);
            System.Diagnostics.Debug.WriteLine("Detail:" + detail);
            System.Diagnostics.Debug.WriteLine("Status Healthy:" + statusHealthy);
            System.Diagnostics.Debug.WriteLine("Document ID:" + documentId);
            System.Diagnostics.Debug.WriteLine("Card Row:" + cardRow);


            OracleCommand cmd = _connection.CreateCommand();
            string sql = "INSERT INTO sf_per_employee_healthy (" +

                "DETAIL," +
                "STATUS_HEALTHY," +
                "DOCUMENT_ID," +
                "CREATE_DATE," +
                "CREATE_BY," +
                "LAST_UPDATE_DATE," +
                "LAST_UPDATE_BY," +
                "ID_PERSONAL_HEALTHY) " +
                "VALUES ('" + detail + "','" + statusHealthy + "','" + documentId + "',SYSDATE,'" + name + "',SYSDATE,'" + name + "','" + cardRow + "')";

            System.Diagnostics.Debug.WriteLine(sql);
            cmd.CommandText = sql;
            _connection.Open();
            var statusExcute = cmd.ExecuteNonQuery();
            if (statusExcute == 1)
            {
                System.Diagnostics.Debug.WriteLine("Insert Healthy Success");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Insert Healthy Fail");
            }




            return RedirectToAction("Details", new { id = documentId });
        }






    }


}
