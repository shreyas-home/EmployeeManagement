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
    /// Interaction logic for PermissionList.xaml
    /// </summary>
    public partial class PermissionList : UserControl
    {
        PersonalTrackingContext db = new PersonalTrackingContext();
        PermissionDetailModel model = new PermissionDetailModel();
        List<PermissionDetailModel> permissions = new List<PermissionDetailModel>();
        List<Position> positions = new List<Position>();

        public PermissionList()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PermissionPage page = new PermissionPage();
            page.ShowDialog();
            FillDataGrid();
        }

        private void FillDataGrid()
        {
            permissions = db.Permissions.Include(x => x.Employee).Include(x => x.PermissionStateNavigation).Select(x => new PermissionDetailModel() {
             Id=x.Id,
             Name=x.Employee.Name,
             Surname=x.Employee.Surname,
             PermissionState=x.PermissionState,
             DayAmount=x.PermissionDay,
             EmployeeId=x.EmployeeId,
             DepartmentId=x.Employee.DepartmentId,
             PositionId=x.Employee.PositionId,
             EndDate=x.PermissionEndDate,
             StartDate=x.PermissionStartDate,
             Explanation=x.PermissionExplanation,
             StateName=x.PermissionStateNavigation.StateName,
             UserNo=x.Employee.UserNo
            }).OrderByDescending(x=>x.StartDate).ToList();

            if (!UserStatic.IsAdmin)
            {
                permissions = permissions.Where(x => x.EmployeeId == UserStatic.EmployeeId).ToList();
                txtUserNo.IsEnabled = false;
                txtName.IsEnabled = false;
                txtSurname.IsEnabled = false;
                cmbDepartment.IsEnabled = false;
                cmbPosition.IsEnabled = false;
                btnApprove.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;
                btnDisapprove.Visibility = Visibility.Hidden;
                btnAdd.SetValue(Grid.ColumnProperty, 1);
            }

            gridPermission.ItemsSource = permissions;

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

            List<PermissionState> states = db.PermissionStates.ToList();
            cmbState.ItemsSource = states;
            cmbState.DisplayMemberPath = "StateName";
            cmbState.SelectedValuePath = "Id";
            cmbState.SelectedIndex = -1;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<PermissionDetailModel> search = permissions;
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
            if (rbStart.IsChecked == true)
            {
                search = search.Where(x => x.StartDate > dpStart.SelectedDate && x.StartDate < dpEnd.SelectedDate).ToList();
            }
            if (rbEnd.IsChecked == true)
            {
                search = search.Where(x => x.EndDate > dpStart.SelectedDate && x.EndDate < dpEnd.SelectedDate).ToList();
            }
            if (cmbState.SelectedIndex != -1)
            {
                search = search.Where(x => x.PermissionState == Convert.ToInt32(cmbState.SelectedValue)).ToList();
            }
            if (txtDayAmount.Text.Trim() != "")
            {
                search = search.Where(x => x.DayAmount == Convert.ToInt32(txtDayAmount.Text)).ToList();
            }
            gridPermission.ItemsSource = search;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtDayAmount.Clear();
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            dpStart.SelectedDate = DateTime.Today;
            dpEnd.SelectedDate = DateTime.Today;
            rbEnd.IsChecked = false;
            rbStart.IsChecked = false;
            cmbDepartment.SelectedIndex = -1;
            cmbPosition.SelectedIndex = -1;
            cmbPosition.ItemsSource = positions;
            cmbState.SelectedIndex = -1;
            gridPermission.ItemsSource = permissions;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (model != null && model.Id != 0)
            {
                PermissionPage page = new PermissionPage();
                page.model = model;
                page.ShowDialog();
                FillDataGrid();
            }
            else
            {
                MessageBox.Show("Please select permission from table");
            }
        }

        private void gridPermission_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model = (PermissionDetailModel)gridPermission.SelectedItem;
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if(model!=null && model.Id!=0 && model.PermissionState == Definitions.PermissionStates.OnAdmin)
            {
                Permission permission = db.Permissions.Find(model.Id);
                permission.PermissionState = Definitions.PermissionStates.Approved;
                db.SaveChanges();
                MessageBox.Show("Permission Approved");
                FillDataGrid();
            }
        }

        private void btnDisapprove_Click(object sender, RoutedEventArgs e)
        {
            if (model != null && model.Id != 0 && model.PermissionState == Definitions.PermissionStates.OnAdmin)
            {
                Permission permission = db.Permissions.Find(model.Id);
                permission.PermissionState = Definitions.PermissionStates.Disapproved;
                db.SaveChanges();
                MessageBox.Show("Permission Disapproved");
                FillDataGrid();
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(model!=null && model.Id != 0)
            {
                if(MessageBox.Show("Are you sure to delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Permission permission = db.Permissions.Find(model.Id);
                    db.Permissions.Remove(permission);
                    db.SaveChanges();
                    MessageBox.Show("Permission Deleted");
                    FillDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Please select permission from table");
            }
        }
    }
}
