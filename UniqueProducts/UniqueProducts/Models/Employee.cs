using System;
using System.Collections.Generic;

namespace UniqueProducts.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Orders = new HashSet<Order>();
        }

        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeSurname { get; set; }
        public string? EmployeeMidname { get; set; }
        public string? EmployeePosition { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
