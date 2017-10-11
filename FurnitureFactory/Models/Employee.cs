using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FurnitureFactory.DBAgent;

namespace FurnitureFactory.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string name { get; set; }
        public int workgroupID { get; set; }
        public float modificator { get; set; }

        public string workgroup { get; set; }
    }

    public class Workgroup
    {
        public int ID { get; set; }
        public string name { get; set; }
    }

    public class Wage
    {
        public Employee employee { get; set; }
        public Workgroup workgroup { get; set; }
        public DateTime date { get; set; }
        public float wage { get; set; }

        public Wage(int EmployeID, int? WorkgroupID, float wage)
        {
            using (DBAgent.DBAgent agent = new DBAgent.DBAgent())
            {
                employee = agent.getEmployee(EmployeID);
                workgroup = agent.getWorkgroup(employee.workgroupID);
                this.wage = wage;
            }
        }
    }
}