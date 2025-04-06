using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace EmployeesData.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<EmployeesInfo> listEmployees = new List<EmployeesInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("EmployeeDbConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT e.EmployeeId, e.EmployeeName, e.Department, e.Sex, e.MaritalStatus, e.Address, s.Salary
                        FROM Employees e
                        LEFT JOIN SalaryInformation s ON e.EmployeeId = s.EmployeeId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeesInfo info = new EmployeesInfo();
                                info.EmployeeId = reader.GetInt32(0).ToString();
                                info.EmployeeName = reader.GetString(1);
                                info.Department = reader.GetString(2);
                                info.Sex = reader.GetString(3);
                                info.MaritalStatus = reader.GetString(4);
                                info.Address = reader.GetString(5);
                                //info.Salary = reader.IsDBNull(6) ? "0.00" : reader.GetDecimal(6).ToString("F2");
                                info.Salary = reader.GetString(6);
                                listEmployees.Add(info);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.ToString());
            }
        }
    }

    public class EmployeesInfo
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Sex { get; set; }
        public string MaritalStatus { get; set; }
        public string Address { get; set; }
        public String  Salary { get; set; }
    }
}

