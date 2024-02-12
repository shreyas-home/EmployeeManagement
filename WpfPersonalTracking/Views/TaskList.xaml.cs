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
    /// Interaction logic for TaskList.xaml
    /// </summary>
    public partial class TaskList : UserControl
    {
        List<TaskDetailModel> taskList = new List<TaskDetailModel>();
        List<TaskDetailModel> searchList = new List<TaskDetailModel>();
        PersonalTrackingContext db = new PersonalTrackingContext();
        List<Position> positions = new List<Position>();
        List<TaskState> taskStates = new List<TaskState>();
        TaskDetailModel taskModel = new TaskDetailModel();

        public TaskList()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            TaskPage page = new TaskPage();
            page.ShowDialog();
            FillDataGrid();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
            if (!UserStatic.IsAdmin)
            {
                btnAdd.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;
                btnApprove.SetValue(Grid.ColumnProperty, 1);
                btnApprove.Content = "Delivery";
            }
        }

        private void FillDataGrid()
        {
            taskList = db.Tasks.Include(x => x.TaskStateNavigation).Include(x => x.Employee).ThenInclude(x => x.Department).ThenInclude(x => x.Positions)
                .Select(x => new TaskDetailModel()
                {
                    Id = x.Id,
                    EmployeeId = (int)x.EmployeeId,
                    Name = x.Employee.Name,
                    StateName = x.TaskStateNavigation.StateName,
                    Surname = x.Employee.Surname,
                    TaskContent = x.TaskContent,
                    TaskDeliveryDate = x.TaskDeliveryDate,
                    TaskStartDate = (DateTime)x.TaskStartDate,
                    TaskState = (int)x.TaskState,
                    TaskTitle = x.TaskTitle,
                    UserNo = x.Employee.UserNo,
                    DepartmentId = x.Employee.DepartmentId,
                    PositionId = x.Employee.PositionId
                }).ToList();

            if (!UserStatic.IsAdmin)
            {
                taskList = taskList.Where(x => x.EmployeeId == UserStatic.EmployeeId).ToList();
                txtUserNo.IsEnabled = false;
                txtName.IsEnabled = false;
                txtSurname.IsEnabled = false;
                cmbDepartment.IsEnabled = false;
                cmbPosition.IsEnabled = false;
            }
            gridTask.ItemsSource = taskList;
            searchList = taskList;

            cmbDepartment.ItemsSource = db.Departments.ToList();
            cmbDepartment.SelectedValuePath = "Id";
            cmbDepartment.DisplayMemberPath = "DepartmentName";
            cmbDepartment.SelectedIndex = -1;

            positions = db.Positions.ToList();
            cmbPosition.ItemsSource = positions;
            cmbPosition.DisplayMemberPath = "PositionName";
            cmbPosition.SelectedValuePath = "Id";
            cmbPosition.SelectedIndex = -1;

            taskStates = db.TaskStates.ToList();
            cmbState.ItemsSource = taskStates;
            cmbState.DisplayMemberPath = "StateName";
            cmbState.SelectedValuePath = "Id";
            cmbState.SelectedIndex = -1;

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<TaskDetailModel> search = searchList;
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
            if (cmbState.SelectedIndex != -1)
            {
                search = search.Where(x => x.TaskState == Convert.ToInt32(cmbState.SelectedValue)).ToList();
            }
            if (rbStart.IsChecked == true)
            {
                search = search.Where(x => x.TaskStartDate > dpStart.SelectedDate && x.TaskStartDate < dpDelivery.SelectedDate).ToList();
            }
            if (rbDelivery.IsChecked == true)
            {
                search = search.Where(x => x.TaskDeliveryDate > dpStart.SelectedDate && x.TaskDeliveryDate < dpDelivery.SelectedDate).ToList();
            }

            gridTask.ItemsSource = search;
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
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            dpDelivery.SelectedDate = DateTime.Today;
            dpStart.SelectedDate = DateTime.Today;
            cmbDepartment.SelectedIndex = -1;
            cmbState.SelectedIndex = -1;
            cmbPosition.ItemsSource = positions;
            cmbPosition.SelectedIndex = -1;
            rbDelivery.IsChecked = false;
            rbStart.IsChecked = false;
            gridTask.ItemsSource = taskList;
        }

        private void gridTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            taskModel = (TaskDetailModel)gridTask.SelectedItem;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            TaskPage page = new TaskPage();
            page.taskModel = taskModel;
            page.ShowDialog();
            FillDataGrid();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (taskModel.Id != 0)
                {
                    TaskDetailModel detailModel = (TaskDetailModel)gridTask.SelectedItem;
                    Task task = db.Tasks.Find(detailModel.Id);
                    db.Tasks.Remove(task);
                    db.SaveChanges();
                    MessageBox.Show("Task deleted");
                    FillDataGrid();
                }
            }
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if(taskModel!=null && taskModel.Id != 0)
            {
                if(UserStatic.IsAdmin && taskModel.TaskState == Definitions.TaskStates.OnEmployee)
                {
                    MessageBox.Show("Before approve a task , task must be delivered");
                }
                else if(UserStatic.IsAdmin && taskModel.TaskState == Definitions.TaskStates.Approved)
                {
                    MessageBox.Show("Task is already approved");
                }
                else if(!UserStatic.IsAdmin && taskModel.TaskState == Definitions.TaskStates.Delivered)
                {
                    MessageBox.Show("Task is already Delivered");
                }
                else if (!UserStatic.IsAdmin && taskModel.TaskState == Definitions.TaskStates.Approved)
                {
                    MessageBox.Show("Task is already approved");
                }
                else
                {
                    Task task = db.Tasks.Find(taskModel.Id);
                    if (UserStatic.IsAdmin)
                    {
                        task.TaskState = Definitions.TaskStates.Approved;
                    }
                    else
                    {
                        task.TaskState = Definitions.TaskStates.Delivered;
                    }
                    db.SaveChanges();
                    MessageBox.Show("Task updated ");
                    FillDataGrid();
                }
            }
        }
    }
}
