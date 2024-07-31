using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using WebApplicationASP.Models;

namespace WebApplicationASP.Controllers
{
    public class UserMobileController : Controller
    {
        private readonly OracleConnection _connection;

        public UserMobileController(OracleConnection oracleConnection)
        {
            _connection = oracleConnection;
        }
        public IActionResult Index()
        {
            string[] name = User.Identity!.Name!.Split('\\');
            string username = name[1].ToUpper();
            List<UserMobileModel> UserMobiles = new List<UserMobileModel>();

            ApprovePath approvePath = new ApprovePath(_connection);
            approvePath.GetPosition(username);

            if (approvePath.sectCode == "5251")
            {
                ViewBag.Health = "Visable";
            }

            if (approvePath.sectCode == "5211" || approvePath.sectCode == "5212" || approvePath.diviCode == "5210")
            {
                ViewBag.Admin = "Visable";
            }


            using (OracleCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "select EMPCODE,NAME ,nvl(EMAIL,'No Email')email,CREATE_DATE from SF_EMP_MOBILE_TOKEN";
                _connection.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UserMobileModel userMobileModel = new UserMobileModel();
                    userMobileModel.EMPCODE = reader["EMPCODE"].ToString();
                    userMobileModel.NAME = reader["NAME"].ToString();
                    userMobileModel.EMAIL = reader["EMAIL"].ToString();
                    userMobileModel.CREATE_DATE = reader["CREATE_DATE"].ToString();
                    UserMobiles.Add(userMobileModel);
                }
                _connection.Close();
            }



           return View(UserMobiles);
        }
        public IActionResult Delete(string id)
        {
            using (OracleCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "delete from SF_EMP_MOBILE_TOKEN where EMPCODE = '" + id + "'";
                _connection.Open();
                cmd.ExecuteNonQuery();
                _connection.Close();
            }
            return RedirectToAction("Index");
        }
    }

    
}
