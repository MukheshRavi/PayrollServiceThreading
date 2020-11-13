using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace PayrollServiceThreading
{
    public class PayrollRepository
    {
        public static string connectionString = @"Server=MUKESH\SQLEXPRESS; Initial Catalog =payroll_service;;Integrated Security=True;Connect Timeout=30;
Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection connection = new SqlConnection(connectionString);
        Mutex mutex = new Mutex();
        public bool AddEmployee(EmployeeDetails emp)
        {
            // open connection and create transaction
            connection.Open();
            try
            {
                // create a new command in transaction
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                // Execute command
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.AddNewEmployee";
                command.Parameters.AddWithValue("@EmpName", emp.empName);
                command.Parameters.AddWithValue("@gender", emp.gender);
                command.Parameters.AddWithValue("@PhoneNumber", emp.phoneNumber);
                command.Parameters.AddWithValue("@start_date", emp.startDate);
                command.Parameters.AddWithValue("@payrollId", emp.payrollId);
                command.Parameters.AddWithValue("@BasePay", emp.BasePay);
                command.Parameters.AddWithValue("@Deductions", emp.Deductions);
                command.Parameters.AddWithValue("@Incometax", emp.IncomeTax);
                command.Parameters.AddWithValue("@TaxablePay", emp.TaxablePay);


                var result = command.ExecuteNonQuery();
                if (result != 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        // <summary>
        /// UC 1 Adds the employee.
        /// </summary>
        /// <param name="employeeDetails">The employee details.</param>
        /// <returns></returns>
        public bool AddMultipleEmployee(List<EmployeeDetails> employeeDetails)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (EmployeeDetails employee in employeeDetails)
            {
                bool result = AddEmployee(employee);
                if (result == false)
                    return false;
            }
            stopwatch.Stop();
            Console.WriteLine("Time taken without threads is :{0} ", stopwatch.ElapsedMilliseconds);
            return true;
        }
        /// <summary>
        /// UC 2 Adds the employee with threads.
        /// </summary>
        /// <param name="employeeDetails">The employee details.</param>
        /// <returns></returns>
        public bool AddMultipleEmployeeWithThreads(List<EmployeeDetails> employeeDetails)
        {
            bool result = false;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread[] thread = new Thread[employeeDetails.Count];
            int i = 0;
            foreach (EmployeeDetails employee in employeeDetails)
            {
                // Store all the threads
                thread[i++] = new Thread(() =>
                {
                    result = AddEmployee(employee);
                    Console.WriteLine("Current Thread Id" + Thread.CurrentThread.ManagedThreadId);
                });
            }
            // Start all the threads
            for (i = 0; i < thread.Length; i++)
            {
                thread[i].Start();
                thread[i].Join();
            }
            stopwatch.Stop();
            Console.WriteLine("Time taken with threads is :{0} ", stopwatch.ElapsedMilliseconds);
            return result;
        }
        /// <summary>
        /// UC3
        /// Adds the employee to payroll data base with thread with synchronization
        /// </summary>
        /// <param name="employeePayrollDataList">The employee payroll data list.</param>
        public bool AddEmployeesWithThreadsAndSynchronization(List<EmployeeDetails> employeeDetails)
        {
            Stopwatch stopwatch = new Stopwatch();
            bool result = false;
            stopwatch.Start();
            employeeDetails.ForEach(employeeData =>
            {
                Thread thread = new Thread(() =>
                {
                    //mutex waitone method is used
                    //this method does not allow to other threads to go in it, until current thread execution is complete
                    mutex.WaitOne();
                    result=AddEmployee(employeeData);
                    Console.WriteLine("Employee added" + employeeData.empName);
                    Console.WriteLine("Current Thread Id" + Thread.CurrentThread.ManagedThreadId);
                    //mut realease mutex is used, which releases current thread and allows new thread to be used.
                    mutex.ReleaseMutex();
                });
                // Start all the threads
                thread.Start();
                thread.Join();
            });
            stopwatch.Stop();
            Console.WriteLine("Time taken with threads is :{0} ", stopwatch.ElapsedMilliseconds);
            return result;
        }
        /// <summary>
        /// This method is used to update database by taking employee name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateEmployee(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                using (connection)
                {
                    string query = @"update  PayrollDetails set PayrollDetails.Basepay=" + 300000 + "from PayrollDetails p inner join Employee e on e.PayrollId=p.payrollId where e.EmpName" +
                        "= '" + name + "'";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    if (result != 0)
                        return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        /// <summary>
        /// UC4
        /// This method updates employees with multi threading and synchronization
        /// </summary>
        /// <param name="employeeNames"></param>
        /// <returns></returns>
        public bool UpdateEmployeesWithThreadsAndSynchronization(List<string> employeeNames)
        {
            Stopwatch stopwatch = new Stopwatch();
            bool result=false;
            stopwatch.Start();
            
            employeeNames.ForEach(name =>
            {
                Thread thread = new Thread(() =>
                {
                    //mutex waitone method is used
                    //this method does not allow to other threads to go in it, until current thread execution is complete
                    mutex.WaitOne();
                    result=UpdateEmployee(name);
                    Console.WriteLine("Employee with name "+name+" updated");
                    Console.WriteLine("Current Thread Id" + Thread.CurrentThread.ManagedThreadId);
                    //mut realease mutex is used, which releases current thread and allows new thread to be used.
                    mutex.ReleaseMutex();
                });
                // Start all the threads
                thread.Start();
                thread.Join();
            });
            stopwatch.Stop();
            Console.WriteLine("Time taken with threads is :{0} ", stopwatch.ElapsedMilliseconds);
            return result;
        }
    }
}
