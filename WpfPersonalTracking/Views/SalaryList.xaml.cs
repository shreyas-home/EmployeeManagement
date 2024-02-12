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

namespace WpfPersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for SalaryList.xaml
    /// </summary>
    public partial class SalaryList : UserControl
    {
        public SalaryList()
        {
            InitializeComponent();
        }
        List<SalaryDetailModel> salaryDetailModels = new List<SalaryDetailModel>();
        SalaryDetailModel model = new SalaryDetailModel();
        PersonalTrackingContext db = new PersonalTrackingContext();
        List<Position> positions = new List<Position>();
        List<Month> monthList = new List<Month>();
        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            SalaryPage page = new SalaryPage();
            page.ShowDialog();
            FillDataGrid();
        }

        private void FillDataGrid()
        {
            salaryDetailModels = db.Salaries.Include(x => x.Employee).Include(x => x.Month).Select(x => new SalaryDetailModel()
            {
                Id = x.Id,
                UserNo = x.Employee.UserNo,
                Name = x.Employee.Name,
                Surname = x.Employee.Surname,
                EmployeeId = x.EmployeeId,
                Amount = x.Amount,
                MonthId = x.MonthId,
                MonthName = x.Month.MonthName,
                Year = x.Year,
                DepartmentId=x.Employee.DepartmentId,
                PositionId=x.Employee.PositionId
            }).OrderByDescending(x=>x.Year).OrderByDescending(x=>x.MonthId).ToList();

            if (!UserStatic.IsAdmin)
            {
                btnAdd.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;

                txtName.IsEnabled = false;
                txtUserNo.IsEnabled = false;
                txtSurname.IsEnabled = false;
                cmbDepartment.IsEnabled = false;
                cmbPosition.IsEnabled = false;

                salaryDetailModels = salaryDetailModels.Where(x => x.EmployeeId == UserStatic.EmployeeId).ToList();
            }
            gridSalary.ItemsSource = salaryDetailModels;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
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
            cmbMonth.SelectedValuePath = "Id";
            cmbMonth.SelectedIndex = -1;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<SalaryDetailModel> search = salaryDetailModels;
            if (txtUserNo.Text.Trim() != "")
            {
                search = search.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)).ToList();
            }
            if (txtName.Text.Trim() != "")
            {
                search = search.Where(x => x.Name.Contains(txtName.Text)).ToList();
            }
            if (txtSurname.Text.Trim() != "")
            {
                search = search.Where(x => x.Surname.Contains(txtSurname.Text)).ToList();
            }
            if (cmbDepartment.SelectedIndex != -1)
            {
                search = search.Where(x => x.DepartmentId == Convert.ToInt32(cmbDepartment.SelectedValue)).ToList();
            }
            if (cmbPosition.SelectedIndex != -1)
            {
                search = search.Where(x => x.PositionId == Convert.ToInt32(cmbPosition.SelectedValue)).ToList();
            }
            if (cmbMonth.SelectedIndex != -1)
            {
                search = search.Where(x => x.MonthId == Convert.ToInt32(cmbMonth.SelectedValue)).ToList();
            }
            if (txtYear.Text.Trim() != "")
            {
                search = search.Where(x => x.Year == Convert.ToInt32(txtYear.Text)).ToList();
            }
            if (txtSalary.Text.Trim() != "")
            {
                if (rbMore.IsChecked == true)
                {
                    search = search.Where(x => x.Amount > Convert.ToInt32(txtSalary.Text)).ToList();
                }
                else if (rbLess.IsChecked == true)
                {
                    search = search.Where(x => x.Amount < Convert.ToInt32(txtSalary.Text)).ToList();
                }
                else
                {
                    search = search.Where(x => x.Amount == Convert.ToInt32(txtSalary.Text)).ToList();
                }
            }

            gridSalary.ItemsSource = search;
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtSalary.Clear();
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            txtYear.Clear();

            cmbDepartment.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
            cmbPosition.ItemsSource = positions;
            cmbMonth.SelectedIndex = -1;

            rbMore.IsChecked = false;
            rbLess.IsChecked = false;
            rbEquals.IsChecked = false;

            gridSalary.ItemsSource = salaryDetailModels;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            SalaryPage page = new SalaryPage();
            page.model = model;
            page.ShowDialog();
            FillDataGrid();
        }

        private void gridSalary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           model = (SalaryDetailModel)gridSalary.SelectedItem;         
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure to delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (model.Id != 0)
                {
                    SalaryDetailModel detailModel = (SalaryDetailModel)gridSalary.SelectedItem;
                    Salary salary = db.Salaries.Find(detailModel.Id);
                    db.Salaries.Remove(salary);
                    db.SaveChanges();
                    MessageBox.Show("Salary deleted");
                    FillDataGrid();
                }
            }
        }
    }
}
