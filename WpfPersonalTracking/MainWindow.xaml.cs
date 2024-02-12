using System.Text;
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
using WpfPersonalTracking.ViewModels;

namespace WpfPersonalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lblWindowName.Content = "Task List";
            DataContext = new TaskViewModel();
        }

        private void btnDepartment_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Department List";
            DataContext = new DepartmentViewModel();
        }

        private void btnPosition_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Position List";
            DataContext = new PositionViewModel();

        }

        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
           
            if (!UserStatic.IsAdmin)
            {
                PersonalTrackingContext db = new PersonalTrackingContext();
                Employee employee = db.Employees.Find(UserStatic.EmployeeId);
                EmployeeDetailModel model = new EmployeeDetailModel();
                model.BirthDate = (DateTime)employee.BirthDate;
                model.Address = employee.Address;
                model.Id = employee.Id;
                model.DepartmentId = employee.DepartmentId;
                model.PositionId = employee.PositionId;
                model.ImagePath = employee.ImagePath;
                model.IsAdmin = employee.IsAdmin;
                model.UserNo = employee.UserNo;
                model.Password = employee.Password;
                model.Name = employee.Name;
                model.Surname = employee.Surname;
                model.Salary = employee.Salary;

                EmployeePage page = new EmployeePage();
                page.employeeDetailModel = model;
                page.ShowDialog();
            }
            else
            {
                lblWindowName.Content = "Employee List";
                DataContext = new EmployeeViewModel();
            }
        }

        private void btnTask_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Task List";
            DataContext = new TaskViewModel();
        }

        private void btnSalary_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Salary List";
            DataContext = new SalaryViewModel();
        }

        private void btnPermission_Click(object sender, RoutedEventArgs e)
        {
            lblWindowName.Content = "Permission List";
            DataContext = new PermissionViewModel();
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PersonalMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserStatic.IsAdmin)
            {
                stackDepartment.Visibility = Visibility.Hidden;
                stackPosition.Visibility = Visibility.Hidden;
                stackLogOut.SetValue(Grid.RowProperty, 5);
                stackExit.SetValue(Grid.RowProperty, 6);
            }
        }
    }
}