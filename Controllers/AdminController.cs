using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Globalization;
using WebApplicationASP.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplicationASP.Controllers
{
    public class AdminController : Controller
    {
        private readonly OracleConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string? employeeCode;
        Uri baseAddressAPI = new Uri("http://10.2.2.5");
        private readonly HttpClient _client2;

        public AdminController(OracleConnection connection, IWebHostEnvironment hostingEnvironment)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
            _client2 = new HttpClient { BaseAddress = baseAddressAPI };
        }

        public IActionResult Index()
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
                columFilter = "b.depart_code = '" + departCode + "'";
                nameFilter = departCode;
            }
            else if (positionGroupCode == "042" || positionGroupCode == "041")
            {
                columFilter = "b.divi_code = '" + diviCode + "'";
                nameFilter = diviCode;

            }
            else if (positionGroupCode == "032" || positionGroupCode == "031")
            {
                columFilter = "b.sect_code = '" + sectCode + "'";
                nameFilter = sectCode;
            }
            else
            {
                columFilter = "b.sect_code = '" + sectCode + "'";
                nameFilter = sectCode;
            }



            System.Diagnostics.Debug.WriteLine("Employee Code:" + employeeCode);
            System.Diagnostics.Debug.WriteLine("Colum Filter:" + columFilter);
            System.Diagnostics.Debug.WriteLine("Name Filter:" + nameFilter);

            if (sectCode == "5212" || sectCode == "5211" || diviCode == "5210")
            {


                List<LeavingViewModel> list = new List<LeavingViewModel>();
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
                    "FROM " +
                    "sf_per_absence_moble_v a," +
                    "sf_per_employees_v b " +

                    "WHERE a.employee_code = b.employee_code " +
                    "and b.employee_place = 'CHP' " +
                    "ORDER BY a.creation_date DESC";
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
                        leaving.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
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
                    leaving.DEPART_CODE = Convert.ToString(reader["DEPART_CODE"]);
                    leaving.SECT_CODE = Convert.ToString(reader["SECT_CODE"]);

                    list.Add(leaving);

                }


                ViewBag.departName = nameFilter;
                TempData["dataToSend"] = "Admin";
                if (sectCode == "5251")
                {
                    ViewBag.Health = "Visable";
                }

                if (sectCode == "5211" || sectCode == "5212" || diviCode == "5210")
                {
                    ViewBag.Admin = "Visable";
                }


                _connection.Close();

                return View(list);
            }
            else
            {
                return RedirectToAction("ContactAdmin");
            }

        }

        public IActionResult ContactAdmin()
        {
            return View();
        }

        public IActionResult AbsenceList()
        {
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

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

            List<AbsenceModel> list = new List<AbsenceModel>();
            OracleCommand cmd1 = _connection.CreateCommand();

            string sql = "select " +
                "a.absence_document," +
                "a.ABSENCE_DATE," +
                "a.EMPLOYEE_CODE," +
                "a.ABSENCE_CODE," +
                "a.ABSENCE_DAY," +
                "a.ABSENCE_HOUR," +
                "a.ABSENCE_REVIEW," +
                "a.ABSENCE_APPROVE," +
                "a.absence_status " +
                "from sf_per_absence_mobile a " +
                "where a.absence_document not in(" +
                "Select a.absence_document " +
                "FROM sf_per_absence_mobile a," +
                "sf_per_absence b " +
                "where a.absence_document = b.absence_document and a.absence_status = '2')" +
                "and a.absence_status = '2' and (a.absence_date,a.employee_code) not in(select a.absence_date,a.employee_code from sf_per_absence)";

            cmd1.CommandText = sql;
            _connection.Open();
            OracleDataReader reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                AbsenceModel absence = new AbsenceModel();
                absence.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                absence.ABSENCE_DATE = Convert.ToString(reader["ABSENCE_DATE"]);
                absence.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                absence.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                absence.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);
                absence.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                absence.ABSENCE_REVIEW = Convert.ToString(reader["ABSENCE_REVIEW"]);
                absence.ABSENCE_APPROVE = Convert.ToString(reader["ABSENCE_APPROVE"]);
                absence.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                list.Add(absence);
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
            return View(list);
        }

        public IActionResult InsertAbsence(string id)
        {

            InsertAbsenceAPI(id);
            System.Diagnostics.Debug.WriteLine("ID:" + id);

            return RedirectToAction("AbsenceList");
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

        public IActionResult Edit(string id)
        {
            ApprovePath approvePath = new ApprovePath(_connection);
            approvePath.GetPosition(User.Identity!.Name!.Split('\\')[1].ToUpper());
            string departCode = approvePath.departCode;
            string diviCode = approvePath.diviCode;
            string sectCode = approvePath.sectCode;
            string positionGroupCode = approvePath.positionGroupCode;
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }
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



            List<AbsenceModel> list = new List<AbsenceModel>();
            OracleCommand cmd = _connection.CreateCommand();
            string sql = "select " +
                "a.absence_document," +
                "to_char(a.absence_date,'dd-MM-yyyy')absence_date," +
                "a.EMPLOYEE_CODE," +
                "a.ABSENCE_CODE," +
                "a.ABSENCE_DAY," +
                "a.ABSENCE_HOUR," +
                "a.ABSENCE_REVIEW," +
                "a.ABSENCE_APPROVE," +
                "a.absence_status " +
                "from sf_per_absence_mobile a " +
                "where a.absence_document = '" + id + "'";
            cmd.CommandText = sql;
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string absence_date = Convert.ToString(reader["ABSENCE_DATE"]).Split(" ")[0].Replace("/", "-");



                AbsenceModel absence = new AbsenceModel();
                absence.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                absence.ABSENCE_DATE = Convert.ToString(reader["ABSENCE_DATE"]).Replace("/", "-");
                absence.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                absence.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                absence.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);
                absence.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                absence.ABSENCE_REVIEW = Convert.ToString(reader["ABSENCE_REVIEW"]);
                absence.ABSENCE_APPROVE = Convert.ToString(reader["ABSENCE_APPROVE"]);
                absence.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                list.Add(absence);
            }
            System.Diagnostics.Debug.WriteLine("ID:" + id);
            _connection.Close();
            ViewBag.documentID = id;
            return View(list);
        }

        public IActionResult EditAbsence(string id, string date)
        {
            date = date.Split(" ")[0];
            DateTime _date = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string _dateString = _date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);


            ApprovePath approvePath = new ApprovePath(_connection);
            approvePath.GetPosition(User.Identity!.Name!.Split('\\')[1].ToUpper());
            string departCode = approvePath.departCode;
            string diviCode = approvePath.diviCode;
            string sectCode = approvePath.sectCode;
            string positionGroupCode = approvePath.positionGroupCode;
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

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


            System.Diagnostics.Trace.WriteLine("ID:" + id);
            System.Diagnostics.Trace.WriteLine("Date:" + _dateString);
            AbsenceModel absence = new AbsenceModel();
            using (OracleCommand cmd = _connection.CreateCommand())
            {


                string sql = "SELECT " +
                    "a.absence_document," +
                "to_char(a.absence_date,'dd-MM-yyyy')absence_date," +
                "a.EMPLOYEE_CODE," +
                "a.ABSENCE_CODE," +
                "a.ABSENCE_DAY," +
                "a.ABSENCE_HOUR," +
                "a.ABSENCE_REVIEW," +
                "a.ABSENCE_APPROVE," +
                "a.absence_status " +
                    "FROM sf_per_absence_mobile a WHERE ABSENCE_DATE = '" + _dateString + "' and a.absence_document = '" + id + "'";
                cmd.CommandText = sql;
                _connection.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    absence.ABSENCE_DOCUMENT = Convert.ToString(reader["ABSENCE_DOCUMENT"]);
                    absence.ABSENCE_DATE = Convert.ToString(reader["ABSENCE_DATE"]);
                    absence.EMPLOYEE_CODE = Convert.ToString(reader["EMPLOYEE_CODE"]);
                    absence.ABSENCE_CODE = Convert.ToString(reader["ABSENCE_CODE"]);
                    absence.ABSENCE_DAY = Convert.ToString(reader["ABSENCE_DAY"]);
                    absence.ABSENCE_HOUR = Convert.ToString(reader["ABSENCE_HOUR"]);
                    absence.ABSENCE_REVIEW = Convert.ToString(reader["ABSENCE_REVIEW"]);
                    absence.ABSENCE_APPROVE = Convert.ToString(reader["ABSENCE_APPROVE"]);
                    absence.ABSENCE_STATUS = Convert.ToString(reader["ABSENCE_STATUS"]);
                }
                _connection.Close();


            }



            return View(absence);
        }
        [HttpPost]
        public IActionResult EditAbsence(AbsenceModel absence, bool checkboxEditAbsenceStatus)
        {
            var date = absence.ABSENCE_DATE.Split(" ")[0];
            DateTime _date = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string _dateString = _date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);


            System.Diagnostics.Trace.WriteLine("ID:" + absence.ABSENCE_DOCUMENT);
            System.Diagnostics.Trace.WriteLine("Date:" + _dateString);
            System.Diagnostics.Trace.WriteLine("Code:" + absence.EMPLOYEE_CODE);
            System.Diagnostics.Trace.WriteLine("day:" + absence.ABSENCE_DAY);
            System.Diagnostics.Trace.WriteLine("hour:" + absence.ABSENCE_HOUR);


            if (checkboxEditAbsenceStatus)
            {

                System.Diagnostics.Trace.WriteLine("chang date");
                System.Diagnostics.Trace.WriteLine("Date_new:" + absence.ABSENCE_DATE_NEW);
                var _dateNew = absence.ABSENCE_DATE_NEW.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                System.Diagnostics.Trace.WriteLine("Date_new:" + _dateNew);

                using (OracleCommand cmd = _connection.CreateCommand())
                {
                    string sql = "UPDATE sf_per_absence_mobile a SET a.ABSENCE_DATE = '" + _dateNew + "',a.ABSENCE_DAY = '" + absence.ABSENCE_DAY + "',a.ABSENCE_HOUR = '" + absence.ABSENCE_HOUR + "',a.ABSENCE_CODE = '" + absence.ABSENCE_CODE + "' WHERE a.ABSENCE_DATE = '" + _dateString + "' and a.absence_document = '" + absence.ABSENCE_DOCUMENT + "'";
                    System.Diagnostics.Trace.WriteLine("SQL:" + sql);
                    cmd.CommandText = sql;
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();

                }

            }
            else
            {
                System.Diagnostics.Trace.WriteLine("No chang date");
                using (OracleCommand cmd = _connection.CreateCommand())
                {
                    string sql = "UPDATE sf_per_absence_mobile a SET a.ABSENCE_DAY = '" + absence.ABSENCE_DAY + "',a.ABSENCE_HOUR = '" + absence.ABSENCE_HOUR + "',a.ABSENCE_CODE = '" + absence.ABSENCE_CODE + "' WHERE a.ABSENCE_DATE = '" + _dateString + "' and a.absence_document = '" + absence.ABSENCE_DOCUMENT + "'";
                    System.Diagnostics.Trace.WriteLine("SQL:" + sql);
                    cmd.CommandText = sql;
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();

                }



            }

            return RedirectToAction("Edit", new { id = absence.ABSENCE_DOCUMENT });

        }

        public IActionResult Delete(string id, string date)
        {
            date = date.Split(" ")[0];
            DateTime _date = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string _dateString = _date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);


            OracleCommand cmd = _connection.CreateCommand();
            string sql = "delete from sf_per_absence_mobile where absence_document = '" + id + "' and ABSENCE_DATE = '" + _dateString + "'";
            cmd.CommandText = sql;
            _connection.Open();
            var result = cmd.ExecuteNonQuery();
            _connection.Close();

            return RedirectToAction("Index");

        }

		public IActionResult CheckAccessDoor()
		{
            ApprovePath approvePath = new ApprovePath(_connection);
            approvePath.GetPosition(User.Identity!.Name!.Split('\\')[1].ToUpper());
            string departCode = approvePath.departCode;
            string diviCode = approvePath.diviCode;
            string sectCode = approvePath.sectCode;
            string positionGroupCode = approvePath.positionGroupCode;
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }
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


            AccessDoorViewModelList accessDoorViewModelList = new AccessDoorViewModelList();
            accessDoorViewModelList.accessDoorModels = new List<AccessDoorModel>();
            accessDoorViewModelList.EmployeeCode = "";
			ViewBag.imgFile = "";
			ViewBag.urlImg = "";    

			return View(accessDoorViewModelList);
           
		}

		[HttpPost]
        public IActionResult CheckAccessDoor(AccessDoorViewModelList accessDoorView)
        {
            ApprovePath approvePath = new ApprovePath(_connection);
            approvePath.GetPosition(User.Identity!.Name!.Split('\\')[1].ToUpper());
            string departCode = approvePath.departCode;
            string diviCode = approvePath.diviCode;
            string sectCode = approvePath.sectCode;
            string positionGroupCode = approvePath.positionGroupCode;
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }
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

            System.Diagnostics.Debug.WriteLine("Employee Code:" + accessDoorView.EmployeeCode);
            string employeeCode = accessDoorView.EmployeeCode;
           List<AccessDoorModel> list = new List<AccessDoorModel>();
            OracleCommand cmd = _connection.CreateCommand();
            string sql = "select * from sf_per_access_transition_v where employee_code = '" + employeeCode + "'";
            cmd.CommandText = sql;
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
				AccessDoorModel accessDoor = new AccessDoorModel();
				accessDoor.name = Convert.ToString(reader["NAME"]);
				accessDoor.employee_code = Convert.ToString(reader["EMPLOYEE_CODE"]);
				accessDoor.machin_name = Convert.ToString(reader["MACHIN_NAME"]);
				accessDoor.machin_status = Convert.ToString(reader["MACHIN_STATUS"]);
				// time type is date time
				accessDoor.Time_stamp = Convert.ToDateTime(reader["TIME_STAMP"]);

				list.Add(accessDoor);
			}   
            _connection.Close();
            System.Diagnostics.Debug.WriteLine("List:" + list.Count);

           AccessDoorViewModelList listAccessDoor = new AccessDoorViewModelList();
            listAccessDoor.accessDoorModels = list;
            listAccessDoor.EmployeeCode = accessDoorView.EmployeeCode;
            string imgFile = accessDoorView.EmployeeCode.Substring(0, 2) + "-"+accessDoorView.EmployeeCode.Substring(2,4)+".jpg";

            string urlImg = "http://10.2.2.5/img/sfi/"+imgFile;
            ViewBag.imgFile = imgFile;
            System.Diagnostics.Debug.WriteLine("imgFile:" + urlImg);
            ViewBag.urlImg = urlImg;

            return View(listAccessDoor);
            

    
        }

		public IActionResult ContectPersonal() {
			ApprovePath approvePath = new ApprovePath(_connection);
			approvePath.GetPosition(User.Identity!.Name!.Split('\\')[1].ToUpper());
			string departCode = approvePath.departCode;
			string diviCode = approvePath.diviCode;
			string sectCode = approvePath.sectCode;
			string positionGroupCode = approvePath.positionGroupCode;
			if (sectCode == "5251")
			{
				ViewBag.Health = "Visable";
			}
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

			List<ContectPersonalModel> list = new List<ContectPersonalModel>();
            OracleCommand cmd = _connection.CreateCommand();
            string sql = "select * from SF_PER_PERSONAL_CONTECT";
            cmd.CommandText = sql;
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
				ContectPersonalModel contectPersonal = new ContectPersonalModel();
				contectPersonal.Id = Convert.ToInt32(reader["ID"]);
				contectPersonal.First_name = Convert.ToString(reader["FIRST_NAME"]);
				contectPersonal.Last_name = Convert.ToString(reader["LAST_NAME"]);
				contectPersonal.career = Convert.ToString(reader["CAREER"]);
                contectPersonal.Employee_code = Convert.ToString(reader["EMPLOYEE_CODE"]);
				contectPersonal.Card_raw = Convert.ToString(reader["CARD_RAW"]);
				list.Add(contectPersonal);
			}
            _connection.Close();
            return View(list);

        }

        public IActionResult CreateContectPersonal()
        {
            ApprovePath approvePath = new ApprovePath(_connection);
            approvePath.GetPosition(User.Identity!.Name!.Split('\\')[1].ToUpper());
            string departCode = approvePath.departCode;
            string diviCode = approvePath.diviCode;
            string sectCode = approvePath.sectCode;
            string positionGroupCode = approvePath.positionGroupCode;
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }
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

          
            

            return View();

           
        }
        [HttpPost]
        public IActionResult CreateContectPersonal(ContectPersonalModel contectPersonal)
        {
            System.Diagnostics.Debug.WriteLine("First Name:" + contectPersonal.First_name);
            System.Diagnostics.Debug.WriteLine("Last Name:" + contectPersonal.Last_name);
            System.Diagnostics.Debug.WriteLine("Career:" + contectPersonal.career);
            System.Diagnostics.Debug.WriteLine("Employee_code:" + contectPersonal.Employee_code);
            System.Diagnostics.Debug.WriteLine("Card Raw:" + contectPersonal.Card_raw);


            OracleCommand cmd = _connection.CreateCommand();
            string sql = "insert into SF_PER_PERSONAL_CONTECT (FIRST_NAME,LAST_NAME,CAREER,EMPLOYEE_CODE,CARD_RAW) values (:FIRST_NAME,:LAST_NAME,:CAREER,:EMPLOYEE_CODE,:CARD_RAW)";
            cmd.CommandText = sql;
            cmd.Parameters.Add(new OracleParameter("FIRST_NAME", contectPersonal.First_name));
            cmd.Parameters.Add(new OracleParameter("LAST_NAME", contectPersonal.Last_name));
            cmd.Parameters.Add(new OracleParameter("CAREER", contectPersonal.career));
            cmd.Parameters.Add(new OracleParameter("EMPLOYEE_CODE", contectPersonal.Employee_code));
            cmd.Parameters.Add(new OracleParameter("CARD_RAW", contectPersonal.Card_raw));
            _connection.Open();
            cmd.ExecuteNonQuery();
            _connection.Close();
            return RedirectToAction("ContectPersonal");
        }

     
        public IActionResult EditContectPersonal(int id)
        {
            ApprovePath approvePath = new ApprovePath(_connection);
            approvePath.GetPosition(User.Identity!.Name!.Split('\\')[1].ToUpper());
            string departCode = approvePath.departCode;
            string diviCode = approvePath.diviCode;
            string sectCode = approvePath.sectCode;
            string positionGroupCode = approvePath.positionGroupCode;
            if (sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }
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

            ContectPersonalModel contectPersonal = new ContectPersonalModel();
            OracleCommand cmd = _connection.CreateCommand();
            string sql = "select * from SF_PER_PERSONAL_CONTECT where ID = " + id;
            cmd.CommandText = sql;
            _connection.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contectPersonal.Id = Convert.ToInt32(reader["ID"]);
                contectPersonal.First_name = Convert.ToString(reader["FIRST_NAME"]);
                contectPersonal.Last_name = Convert.ToString(reader["LAST_NAME"]);
                contectPersonal.career = Convert.ToString(reader["CAREER"]);
                contectPersonal.Employee_code = Convert.ToString(reader["EMPLOYEE_CODE"]);
                contectPersonal.Card_raw = Convert.ToString(reader["CARD_RAW"]);
            }
            _connection.Close();
            return View(contectPersonal);
        }

        [HttpPost]
        public IActionResult EditContectPersonal(ContectPersonalModel contectPersonal)
        {
            System.Diagnostics.Debug.WriteLine("ID:" + contectPersonal.Id);
            System.Diagnostics.Debug.WriteLine("First Name:" + contectPersonal.First_name);
            System.Diagnostics.Debug.WriteLine("Last Name:" + contectPersonal.Last_name);
            System.Diagnostics.Debug.WriteLine("Career:" + contectPersonal.career);
            System.Diagnostics.Debug.WriteLine("Employee_code:" + contectPersonal.Employee_code);
            System.Diagnostics.Debug.WriteLine("Card Raw:" + contectPersonal.Card_raw);

            OracleCommand cmd = _connection.CreateCommand();
            string sql = "update SF_PER_PERSONAL_CONTECT set FIRST_NAME = :FIRST_NAME,LAST_NAME = :LAST_NAME,CAREER = :CAREER,EMPLOYEE_CODE = :EMPLOYEE_CODE,CARD_RAW = :CARD_RAW where ID = :ID";
            cmd.CommandText = sql;
            cmd.Parameters.Add(new OracleParameter("FIRST_NAME", contectPersonal.First_name));
            cmd.Parameters.Add(new OracleParameter("LAST_NAME", contectPersonal.Last_name));
            cmd.Parameters.Add(new OracleParameter("CAREER", contectPersonal.career));
            cmd.Parameters.Add(new OracleParameter("EMPLOYEE_CODE", contectPersonal.Employee_code));
            cmd.Parameters.Add(new OracleParameter("CARD_RAW", contectPersonal.Card_raw));
            cmd.Parameters.Add(new OracleParameter("ID", contectPersonal.Id));

            _connection.Open();
            cmd.ExecuteNonQuery();
            System.Diagnostics.Debug.WriteLine("SQL:"+sql);
            _connection.Close();
            return RedirectToAction("ContectPersonal");
        }
        
        public IActionResult DeleteContectPersonal(int id)
        {
            OracleCommand cmd = _connection.CreateCommand();
            string sql = "delete from SF_PER_PERSONAL_CONTECT where ID = " + id;
            cmd.CommandText = sql;
            _connection.Open();
            cmd.ExecuteNonQuery();
            _connection.Close();
            return RedirectToAction("ContectPersonal");
        }

    

    }

	
}
