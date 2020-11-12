using System;
using System.Collections.Generic;
using System.Text;

namespace PayrollServiceThreading
{
    public class EmployeeDetails
    {
        public int empId { get; set; }
        public string empName { get; set; }
        public char gender { get; set; }
        public string phoneNumber { get; set; }
        public int payrollId { get; set; }
        public DateTime startDate { get; set; }
        public int BasePay { get; set; }
        public int Deductions { get; set; }
        public int TaxablePay { get; set; }
        public int IncomeTax { get; set; }
    }
}
