using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPersonalTracking.ViewModels
{
    public class PositionModel
    {
        public int ID { get; set; }
        public string PositionName { get; set; }
        public string DepartmentName { get; set; }

        public int DepartmentID { get; set; }
    }
}
