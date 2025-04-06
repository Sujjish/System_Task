using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EmployeesData.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public EmployeesInfo employeesInfo = new EmployeesInfo();
        public string errorMessage = "";
        public string SuccessMessage = "";

        public void OnGet()
        {
            string id = Request.Query["EmployeeId"];

            try
            {
                // Get connection string from appsettings.json
                string connectionString = _configuration.GetConnectionString("EmployeeDbConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT e.EmployeeId, e.EmployeeName, e.Department, e.Sex, e.MaritalStatus, e.Address, s.Salary FROM Employees e LEFT JOIN SalaryInformation s ON e.EmployeeId = s.EmployeeId  WHERE e.EmployeeId = @EmployeeId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", Convert.ToInt32(id));
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                employeesInfo.EmployeeId = reader.GetInt32(0).ToString();
                                employeesInfo.EmployeeName = reader.GetString(1);
                                employeesInfo.Department = reader.GetString(2);
                                employeesInfo.Sex = reader.GetString(3);
                                employeesInfo.MaritalStatus = reader.GetString(4);
                                //employeesInfo.Salary = reader.GetDecimal(5).ToString();
                                //employeesInfo.Salary = reader.GetString(5);
                                employeesInfo.Address = reader.GetString(5);
                                employeesInfo.Salary = reader.GetString(6);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            employeesInfo.EmployeeId = Request.Form["EmployeeId"];
            employeesInfo.EmployeeName = Request.Form["EmployeeName"];
            employeesInfo.Department = Request.Form["Department"];
            employeesInfo.Sex = Request.Form["Sex"];
            employeesInfo.MaritalStatus = Request.Form["MaritalStatus"];
            employeesInfo.Salary = Request.Form["Salary"];
            employeesInfo.Address = Request.Form["Address"];

            if (employeesInfo.EmployeeId.Length == 0 || employeesInfo.EmployeeName.Length == 0 ||
                employeesInfo.Department.Length == 0 || employeesInfo.Sex.Length == 0 ||
                employeesInfo.MaritalStatus.Length == 0 || employeesInfo.Salary.Length == 0 ||
                employeesInfo.Address.Length == 0)
            {
                errorMessage = "All the Fields are required";
                return;
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("EmployeeDbConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"
                                    UPDATE Employees 
                                    SET EmployeeName = @EmployeeName, 
                                        Department = @Department, 
                                        Sex = @Sex, 
                                        MaritalStatus = @MaritalStatus, 
                                        Address = @Address 
                                    WHERE EmployeeId = @EmployeeId;

                                    UPDATE SalaryInformation 
                                    SET Salary = @Salary 
                                    WHERE EmployeeId = @EmployeeId;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", Convert.ToInt32(employeesInfo.EmployeeId));
                        command.Parameters.AddWithValue("@EmployeeName", employeesInfo.EmployeeName);
                        command.Parameters.AddWithValue("@Department", employeesInfo.Department);
                        command.Parameters.AddWithValue("@Sex", employeesInfo.Sex);
                        command.Parameters.AddWithValue("@MaritalStatus", employeesInfo.MaritalStatus);

                        command.Parameters.AddWithValue("@Salary", employeesInfo.Salary);
                        command.Parameters.AddWithValue("@Address", employeesInfo.Address);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Employees/Index");
        }

    }
}
