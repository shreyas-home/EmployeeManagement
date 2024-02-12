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
using Task = WpfPersonalTracking.DB.Task;

namespace WpfPersonalTracking
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : Window
    {
        List<Employee> employees = new List<Employee>();
        int employeeId;
        List<Position> positions = new List<Position>();
        public TaskDetailModel taskModel = new TaskDetailModel();
        public TaskPage()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using(PersonalTrackingContext db = new PersonalTrackingContext())
            {
                employees = db.Employees.OrderBy(x => x.Name).ToList();
                gridEmployee.ItemsSource = employees;

                cmbDepartment.ItemsSource = db.Departments.ToList();
                cmbDepartment.SelectedValuePath = "Id";
                cmbDepartment.DisplayMemberPath = "DepartmentName";
                cmbDepartment.SelectedIndex = -1;

                positions = db.Positions.ToList();
                cmbPosition.ItemsSource = positions;
                cmbPosition.DisplayMemberPath = "PositionName";
                cmbPosition.SelectedValuePath = "Id";
                cmbPosition.SelectedIndex = -1;

                if(taskModel!=null && taskModel.Id != 0)
                {
                    txtUserNo.Text = taskModel.UserNo.ToString();
                    txtName.Text = taskModel.Name;
                    txtSurname.Text = taskModel.Surname;
                    txtTitle.Text = taskModel.TaskTitle;
                    txtContent.Text = taskModel.TaskContent;
                }
            }
        }

        private void gridEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee employee = (Employee)gridEmployee.SelectedItem;
            txtUserNo.Text = employee.UserNo.ToString();
            txtName.Text = employee.Name;
            txtSurname.Text = employee.Surname;
            employeeId = employee.Id;
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(txtTitle.Text.Trim()=="" || txtContent.Text.Trim() == "")
            {
                MessageBox.Show("Please fill task details");
            }
            else
            {
                if (taskModel != null && taskModel.Id != 0)
                {
                    using(PersonalTrackingContext db = new PersonalTrackingContext())
                    {
                        Task task = db.Tasks.Find(taskModel.Id);
                        if (employeeId != 0) 
                        {
                            task.EmployeeId = employeeId;

                        }
                        task.TaskTitle = txtTitle.Text;
                        task.TaskContent = txtContent.Text;
                        db.SaveChanges();
                        MessageBox.Show("Task Updated Successfully");
                    }

                }
                else
                {
                    if (employeeId == 0)
                    {
                        MessageBox.Show("Please select employee from table");
                    }
                    else
                    {
                        Task task = new Task();
                        task.EmployeeId = employeeId;
                        task.TaskStartDate = DateTime.Now;
                        task.TaskTitle = txtTitle.Text;
                        task.TaskContent = txtContent.Text;
                        task.TaskState = Definitions.TaskStates.OnEmployee;

                        using (PersonalTrackingContext db = new PersonalTrackingContext())
                        {
                            db.Tasks.Add(task);
                            db.SaveChanges();
                        }
                        MessageBox.Show("Task added successfully");
                        txtName.Clear();
                        txtContent.Clear();
                        txtSurname.Clear();
                        txtTitle.Clear();
                        txtUserNo.Clear();
                        employeeId = 0;
                    }

                }
            }
        }
    }
}
