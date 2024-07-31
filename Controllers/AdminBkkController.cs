using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Org.BouncyCastle.Asn1.X509;
using WebApplicationASP.Models;

namespace WebApplicationASP.Controllers
{
    public class AdminBkkController : Controller
    {
        private readonly OracleConnection _connection;

        public AdminBkkController(OracleConnection connection)
        {
            _connection = connection;
        }
        public IActionResult Index()
        {
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();

            ApprovePath approvePath = new ApprovePath(_connection);
            approvePath.GetPosition(username);

            List<LeavingViewModel> list = new List<LeavingViewModel>();

            if (approvePath.departCode == "7000" && approvePath.positionGroupCode == "042")
            {
               
                using(OracleCommand command = _connection.CreateCommand())
                {
                    string sql = "SELECT " +
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
                    "and b.employee_place = 'BKK' " +
                    "ORDER BY a.creation_date DESC";


                    command.CommandText = sql;
                    _connection.Open();
                    using(OracleDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            LeavingViewModel leavingViewModel = new LeavingViewModel();
                            leavingViewModel.START_DATE = reader["START_DATE"].ToString();
                            leavingViewModel.END_DATE = reader["END_DATE"].ToString();
                            leavingViewModel.COUNT_DATE = reader["COUNT_DATE"].ToString();
                            leavingViewModel.NAME = reader["NAME"].ToString();
                            leavingViewModel.EMPLOYEE_CODE = reader["EMPLOYEE_CODE"].ToString();
                            leavingViewModel.ABSENCE_CODE = reader["ABSENCE_CODE"].ToString();
                            leavingViewModel.ABSENCE_DAY = reader["ABSENCE_DAY"].ToString();
                            leavingViewModel.ABSENCE_HOUR = reader["ABSENCE_HOUR"].ToString();
                            leavingViewModel.DELETE_MARK = reader["DELETE_MARK"].ToString();
                            leavingViewModel.REVIEW = reader["REVIEW"].ToString();
                            leavingViewModel.APPROVE = reader["APPROVE"].ToString();
                            leavingViewModel.ABSENCE_PERIOD = reader["ABSENCE_PERIOD"].ToString();
                            leavingViewModel.ABSENCE_STATUS = reader["ABSENCE_STATUS"].ToString();
                            leavingViewModel.ABSENCE_TOKEN = reader["ABSENCE_TOKEN"].ToString();
                            leavingViewModel.ABSENCE_DETAIL = reader["ABSENCE_DETAIL"].ToString();
                            leavingViewModel.ABSENCE_DOCUMENT = reader["ABSENCE_DOCUMENT"].ToString();
                            leavingViewModel.CREATION_DATE = reader["CREATION_DATE"].ToString();
                            leavingViewModel.STATUS_APPROVE = reader["STATUS_APPROVE"].ToString();
                            leavingViewModel.DEPART_CODE = reader["DEPART_CODE"].ToString();
                            leavingViewModel.DIVI_CODE = reader["DIVI_CODE"].ToString();
                            leavingViewModel.SECT_CODE = reader["SECT_CODE"].ToString();
                            list.Add(leavingViewModel);
                        }
                    }

                   
                   
                }   


                System.Console.WriteLine("7000");
                ViewBag.AdminBkk = "Visable";
              
            }
            return View(list);
        }
    }
}
