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
using System.Windows.Shapes;
using WpfPersonalTracking.DB;
using WpfPersonalTracking.ViewModels;

namespace WpfPersonalTracking
{
    /// <summary>
    /// Interaction logic for SalaryPage.xaml
    /// </summary>
    public partial class SalaryPage : Window
    {
        public SalaryPage()
        {
            InitializeComponent();
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        int EmployeeId = 0;
        List<Position> positions = new List<Position>();
        List<Employee> employeeList = new List<Employee>();
        List<Month> monthList = new List<Month>();
        public SalaryDetailModel model;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            employeeList = db.Employees.ToList();
            gridEmployee.ItemsSource = employeeList;

            cmbDepartment.ItemsSource = db.Departments.ToList();
            cmbDepartment.SelectedValuePath = "Id";
            cmbDepartment.DisplayMemberPath = "DepartmentName";
            cmbDepartment.SelectedIndex = -1;

            positions = db.Positions.ToList();
            cmbPosition.ItemsSource = positions;
            cmbPosition.DisplayMemberPath = "PositionName";
            cmbPosition.SelectedValuePath = "Id";
            cmbPosition.SelectedIndex = -1;
            
            monthList = db.Months.ToList();
            cmbMonth.ItemsSource = monthList;
            cmbMonth.DisplayMemberPath = "MonthName";
            cmbMonth.SelectedValuePath= "Id";
            cmbMonth.SelectedIndex = -1;

            if (model != null && model.Id != 0)
            {
                txtUserNo.Text = model.UserNo.ToString();
                txtName.Text = model.Name;
                txtSurname.Text = model.Surname;
                txtSalary.Text = model.Amount.ToString();
                txtYear.Text = model.Year.ToString();
                EmployeeId = model.EmployeeId;
                cmbMonth.SelectedValue = model.MonthId;
            }
        }

        private void gridEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee employee = (Employee)gridEmployee.SelectedItem;
            txtUserNo.Text = employee.UserNo.ToString();
            txtName.Text = employee.Name;
            txtSurname.Text = employee.Surname;
            txtYear.Text = DateTime.Now.Year.ToString();
            txtSalary.Text = employee.Salary.ToString();
            EmployeeId = employee.Id;
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

            gridEmployee.ItemsSource=db.Employees.Where(x=>x.DepartmentId== departmentID).ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(txtSalary.Text.Trim()=="" || txtYear.Text.Trim()=="" || cmbMonth.SelectedIndex == -1)
            {
                MessageBox.Show("please fill all areas");
            }
            else
            {
                if(model!=null && model.Id != 0)
                {
                    Salary salary = db.Salaries.Find(model.Id);
                    int OldSalary = salary.Amount;
                    salary.Amount = Convert.ToInt32(txtSalary.Text);
                    salary.EmployeeId = EmployeeId;
                    salary.MonthId = Convert.ToInt32(cmbMonth.SelectedValue);
                    salary.Year = Convert.ToInt32(txtYear.Text);
                    db.SaveChanges();
                    if (OldSalary < salary.Amount)
                    {
                        Employee employee = db.Employees.Find(EmployeeId);
                        employee.Salary = salary.Amount;
                        db.SaveChanges();
                    }
                    MessageBox.Show("Salary updated successfully");
                }
                else
                {
                    if (EmployeeId == 0)
                    {
                        MessageBox.Show("Please select employee");
                    }
                    else
                    {
                        Salary salary = new Salary();
                        salary.EmployeeId = EmployeeId;
                        salary.Amount = Convert.ToInt32(txtSalary.Text);
                        salary.MonthId = Convert.ToInt32(cmbMonth.SelectedValue);
                        salary.Year = Convert.ToInt32(txtYear.Text);
                        db.Salaries.Add(salary);
                        db.SaveChanges();
                        MessageBox.Show("Salary Added Successfully");
                        EmployeeId = 0;
                        txtName.Clear();
                        txtSurname.Clear();
                        txtUserNo.Clear();
                        txtSalary.Clear();
                        txtYear.Clear();
                        cmbMonth.SelectedIndex = -1;
                        gridEmployee.ItemsSource = employeeList;
                        cmbDepartment.SelectedIndex = -1;
                        cmbPosition.ItemsSource = positions;
                        cmbPosition.SelectedIndex = -1;
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
