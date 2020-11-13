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
                command.Parameters.AddWithValue("@TaxablePay",emp.TaxablePay);


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
        /// Adds the employee to payroll data base with thread with synchronization. UC3
        /// </summary>
        /// <param name="employeePayrollDataList">The employee payroll data list.</param>
        public bool AddEmployeesWithThreadsAndSynchronization(List<EmployeeDetails> employeeDetails)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            employeeDetails.ForEach(employeeData =>
            {
                Thread thread = new Thread(() =>
                {
                    //mutex waitone method is used
                    //this method does not allow to other threads to go in it, until current thread execution is complete
                    mutex.WaitOne();
                    AddEmployee(employeeData);
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
            return true;
        }
    }
}
