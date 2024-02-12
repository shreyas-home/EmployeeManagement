using Microsoft.EntityFrameworkCore;
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
using WpfPersonalTracking.ViewModels;
using Task = WpfPersonalTracking.DB.Task;

namespace WpfPersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for PositionList.xaml
    /// </summary>
    public partial class EmployeeList : UserControl
    {
        List<Position> positions = new List<Position>();
        List<EmployeeDetailModel> employeeDetailModels = new List<EmployeeDetailModel>();
        PersonalTrackingContext db = new PersonalTrackingContext();
        public EmployeeList()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            EmployeePage page = new EmployeePage();
            page.ShowDialog();
            FillDataGrid();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }

        private void FillDataGrid()
        {

            using (PersonalTrackingContext db = new PersonalTrackingContext())
            {
                cmbDepartment.ItemsSource = db.Departments.ToList();
                cmbDepartment.SelectedValuePath = "Id";
                cmbDepartment.DisplayMemberPath = "DepartmentName";
                cmbDepartment.SelectedIndex = -1;

                positions = db.Positions.ToList();
                cmbPosition.ItemsSource = positions;
                cmbPosition.DisplayMemberPath = "PositionName";
                cmbPosition.SelectedValuePath = "Id";
                cmbPosition.SelectedIndex = -1;

                employeeDetailModels = db.Employees.Include(x => x.Position).Include(x => x.Department).Select(x => new EmployeeDetailModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UserNo = x.UserNo,
                    Surname = x.Surname,
                    Password = x.Password,
                    Salary = x.Salary,
                    Address = x.Address,
                    BirthDate = (DateTime)x.BirthDate,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.DepartmentName,
                    IsAdmin = x.IsAdmin,
                    PositionId = x.PositionId,
                    PositionName = x.Position.PositionName,
                    ImagePath = x.ImagePath
                }).ToList();

                gridEmployee.ItemsSource = employeeDetailModels;
            }
        }

        private void cmbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int departmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
            if (cmbPosition.SelectedIndex != -1)
            {
                cmbPosition.ItemsSource = positions.Where(x => x.DepartmentId == departmentID).ToList();
                cmbPosition.DisplayMemberPath = "PositionName";
                cmbPosition.SelectedValuePath = "Id";
                cmbPosition.SelectedIndex = -1;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<EmployeeDetailModel> searchList = employeeDetailModels;
            if (txtUserNo.Text.Trim() != "")
            {
                searchList = searchList.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)).ToList();
            }
            if (txtName.Text.Trim() != "")
            {
                searchList = searchList.Where(x => x.Name.Contains(txtName.Text)).ToList();
            }
            if (txtSurname.Text.Trim() != "")
            {
                searchList = searchList.Where(x => x.Surname.Contains(txtSurname.Text)).ToList();
            }
            if(cmbDepartment.SelectedIndex != -1)
            {
                searchList = searchList.Where(x => x.DepartmentId == Convert.ToInt32(cmbDepartment.SelectedValue)).ToList();
            }
            if (cmbPosition.SelectedIndex != -1)
            {
                searchList = searchList.Where(x => x.PositionId == Convert.ToInt32(cmbPosition.SelectedValue)).ToList();
            }

            gridEmployee.ItemsSource = searchList;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            cmbDepartment.SelectedIndex = -1;
            cmbPosition.ItemsSource = positions;
            cmbPosition.SelectedIndex = -1;

            gridEmployee.ItemsSource = employeeDetailModels;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            EmployeeDetailModel employeeDetail = (EmployeeDetailModel)gridEmployee.SelectedItem;
            if (employeeDetail != null && employeeDetail.Id != 0)
            {
                EmployeePage page = new EmployeePage();
                page.employeeDetailModel = employeeDetail;
                page.ShowDialog();
                FillDataGrid();
            }
            else
            {
                MessageBox.Show("Please select employee from table");
            }
            
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            EmployeeDetailModel employeeDetail = (EmployeeDetailModel)gridEmployee.SelectedItem;
            if(employeeDetail!=null && employeeDetail.Id != 0)
            {
                if(MessageBox.Show("Are you sure to delete employee?","Question",MessageBoxButton.YesNo,MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    List<Task> tasks = db.Tasks.Where(x => x.EmployeeId == employeeDetail.Id).ToList();
                    foreach (var item in tasks)
                    {
                        db.Tasks.Remove(item);
                    }

                    List<Permission> permissions = db.Permissions.Where(x => x.EmployeeId == employeeDetail.Id).ToList();
                    foreach (var item in permissions)
                    {
                        db.Permissions.Remove(item);
                    }

                    List<Salary> salaries = db.Salaries.Where(x => x.EmployeeId == employeeDetail.Id).ToList();
                    foreach (var item in salaries)
                    {
                        db.Salaries.Remove(item);
                    }
                    db.SaveChanges();

                    Employee employee = db.Employees.Find(employeeDetail.Id);
                    db.Employees.Remove(employee);
                    db.SaveChanges();

                    MessageBox.Show("Employee deleted");
                    FillDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Please select employee from table");
            }

        }
    }
}
