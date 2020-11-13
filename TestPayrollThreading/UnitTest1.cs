using Microsoft.VisualStudio.TestTools.UnitTesting;
using PayrollServiceThreading;
using System.Collections.Generic;

namespace TestPayrollThreading
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AddMultipleEmployees()
        {
            //Arrange
            PayrollRepository payrollRepository = new PayrollRepository();
            List<EmployeeDetails> employeelist = new List<EmployeeDetails>();
            //Act
            employeelist.Add(new EmployeeDetails
            {
                empName = "Vijay",
                gender = 'M',
                phoneNumber = "2345167868",
                startDate = new System.DateTime(2019, 08, 24),
                payrollId=4,
                BasePay = 36000,
                Deductions = 1000,
                IncomeTax = 2000,
                TaxablePay = 4000
            });
            employeelist.Add(new EmployeeDetails
            {
                empName = "Sankar",
                gender = 'M',
                phoneNumber = "2345167456",
                startDate = new System.DateTime(2019, 11, 09),
                payrollId=5,
                BasePay = 40000,
                Deductions = 1500,
                IncomeTax = 2000,
                TaxablePay = 4000
            });
         
            bool actual= payrollRepository.AddMultipleEmployee(employeelist);
            bool expected = true;
            ///Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AddMultipleEmployeesWithThreading()
        {
            //Arrange
            PayrollRepository payrollRepository = new PayrollRepository();
            List<EmployeeDetails> employeelist = new List<EmployeeDetails>();
            //Act
            employeelist.Add(new EmployeeDetails
            {
                empName = "Manish",
                gender = 'M',
                phoneNumber = "2345167898",
                startDate = new System.DateTime(2019, 08, 24),
                payrollId = 6,
                BasePay = 80000,
                Deductions = 1000,
                IncomeTax = 4000,
                TaxablePay = 5000
            });
            employeelist.Add(new EmployeeDetails
            {
                empName = "Smriti",
                gender = 'F',
                phoneNumber = "2345167437",
                startDate = new System.DateTime(2019, 11, 09),
                payrollId = 7,
                BasePay = 40000,
                Deductions = 1500,
                IncomeTax = 2000,
                TaxablePay = 4000
            });

            bool actual = payrollRepository.AddMultipleEmployeeWithThreads(employeelist);
            bool expected = true;
            ///Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void AddMultipleEmployeesWithThreadingAndSynchronisation()
        {
            //Arrange
            PayrollRepository payrollRepository = new PayrollRepository();
            List<EmployeeDetails> employeelist = new List<EmployeeDetails>();
            //Act
            employeelist.Add(new EmployeeDetails
            {
                empName = "Manish",
                gender = 'M',
                phoneNumber = "2345167898",
                startDate = new System.DateTime(2019, 08, 24),
                payrollId = 6,
                BasePay = 80000,
                Deductions = 1000,
                IncomeTax = 4000,
                TaxablePay = 5000
            });
            employeelist.Add(new EmployeeDetails
            {
                empName = "Smriti",
                gender = 'F',
                phoneNumber = "2345167437",
                startDate = new System.DateTime(2019, 11, 09),
                payrollId = 7,
                BasePay = 40000,
                Deductions = 1500,
                IncomeTax = 2000,
                TaxablePay = 4000
            });

            bool actual = payrollRepository.AddEmployeesWithThreadsAndSynchronization(employeelist);
            bool expected = true;
            ///Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
