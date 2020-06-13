using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Models
{
    public class DashboardModel
    {
        public int SortOrder { get; set; }
        public string InstanceName { get; set; }
        public string SubDomain { get; set; }
        public int Departmentid { get; set; }
        public string DepartmentName { get; set; }

        public long UserCount { get; set; }

        public int? ElectronicFilePending { get; set; }
        public int? PhysicalFilePending { get; set; }
        public int? PendingTotal { get; set; }

        public int? ElectronicFileCreated { get; set; }
        public int? PhysicalFileCreated { get; set; }
        public int? CreatedTotal { get; set; }

        public int? ElectronicFileClosed { get; set; }
        public int? PhysicalFileClosed { get; set; }
        public int? ClosedTotal { get; set; }

        public int? ElectronicReceiptCreated { get; set; }
        public int? PhysicalReceiptCreated { get; set; }
        public int? ReceiptsCreatedTotal { get; set; }


        public string EmployeeName { get; set; }
    }
}