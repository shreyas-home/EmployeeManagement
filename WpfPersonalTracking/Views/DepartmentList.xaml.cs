using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfPersonalTracking.DB;

namespace WpfPersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for DepartmentList.xaml
    /// </summary>
    public partial class DepartmentList : UserControl
    {
        public DepartmentList()
        {
            InitializeComponent();

            using(PersonalTrackingContext db = new PersonalTrackingContext())
            {
                List<Department> departments = db.Departments.OrderBy(x=>x.DepartmentName).ToList();
                gridDepartment.ItemsSource = departments;
            }

            
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DepartmentPage departmentPage = new DepartmentPage();
            departmentPage.ShowDialog();

            using (PersonalTrackingContext db = new PersonalTrackingContext())
            {
                List<Department> departments = db.Departments.OrderBy(x=>x.DepartmentName).ToList();
                gridDepartment.ItemsSource = departments;
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Department department = (Department)gridDepartment.SelectedItem;
            DepartmentPage page = new DepartmentPage();
            page.dpt = department;
            page.ShowDialog();
            using (PersonalTrackingContext db = new PersonalTrackingContext())
            {
                List<Department> departments = db.Departments.OrderBy(x => x.DepartmentName).ToList();
                gridDepartment.ItemsSource = departments;
            }
        }
    }
}
