using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;

namespace EmployeesData.Pages.Employees
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public EmployeesInfo employeesInfo = new EmployeesInfo();
        public string errorMessage = "";
        public string SuccessMessage = "";

        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            string action = Request.Form["action"];
            if (action == "View")
            {
                Response.Redirect("/Employees/Index");
                return;
            }
            employeesInfo.EmployeeName = Request.Form["EmployeeName"];
            employeesInfo.Department = Request.Form["Department"];
            employeesInfo.Sex = Request.Form["Sex"];
            employeesInfo.MaritalStatus = Request.Form["MaritalStatus"];
            employeesInfo.Address = Request.Form["Address"];
            employeesInfo.Salary = Request.Form["Salary"];

            
            if (string.IsNullOrWhiteSpace(employeesInfo.EmployeeName) ||
                string.IsNullOrWhiteSpace(employeesInfo.Department) ||
                string.IsNullOrWhiteSpace(employeesInfo.Sex) ||
                string.IsNullOrWhiteSpace(employeesInfo.MaritalStatus) ||
                string.IsNullOrWhiteSpace(employeesInfo.Address) 
                )
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

                    
                    string sqlEmp = "INSERT INTO Employees (EmployeeName, Department, Sex, MaritalStatus, Address) " +
                                    "VALUES (@EmployeeName, @Department, @Sex, @MaritalStatus, @Address); " +
                                    "SELECT CAST(scope_identity() AS int);";
                    int newEmployeeId;
                    using (SqlCommand command = new SqlCommand(sqlEmp, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeName", employeesInfo.EmployeeName);
                        command.Parameters.AddWithValue("@Department", employeesInfo.Department);
                        command.Parameters.AddWithValue("@Sex", employeesInfo.Sex);
                        command.Parameters.AddWithValue("@MaritalStatus", employeesInfo.MaritalStatus);
                        command.Parameters.AddWithValue("@Address", employeesInfo.Address);
                        newEmployeeId = (int)command.ExecuteScalar();
                    }

                    
                    string sqlSalary = "INSERT INTO SalaryInformation (EmployeeId, Salary) " +
                                       "VALUES (@EmployeeId, @Salary);";
                    using (SqlCommand command = new SqlCommand(sqlSalary, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", newEmployeeId);
                        command.Parameters.AddWithValue("@Salary", employeesInfo.Salary);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            SuccessMessage = "New Employee Added Successfully";
            Response.Redirect("/Employees/Index");
        }
    }

}


