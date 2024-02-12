using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for EmployeePage.xaml
    /// </summary>
    public partial class EmployeePage : Window
    {
        List<Position> positions = new List<Position>();
        public EmployeeDetailModel employeeDetailModel;
        public EmployeePage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
            }

            if (employeeDetailModel != null && employeeDetailModel.Id != 0)
            {
                cmbDepartment.SelectedValue = employeeDetailModel.DepartmentId;
                cmbPosition.SelectedValue = employeeDetailModel.PositionId;
                txtUserNo.Text = employeeDetailModel.UserNo.ToString();
                txtPassword.Text = employeeDetailModel.Password;
                txtName.Text = employeeDetailModel.Name;
                txtSurname.Text = employeeDetailModel.Surname;
                txtSalary.Text = employeeDetailModel.Salary.ToString();
                txtAddress.AppendText(employeeDetailModel.Address);
                datePicker.SelectedDate = employeeDetailModel.BirthDate;
                chIsAdmin.IsChecked = employeeDetailModel.IsAdmin;

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"Images/" + employeeDetailModel.ImagePath,UriKind.RelativeOrAbsolute);
                image.EndInit();
                EmployeeImage.Source = image;
            }

            if (!UserStatic.IsAdmin)
            {
                chIsAdmin.IsEnabled = false;
                txtUserNo.IsEnabled = false;
                txtSalary.IsEnabled = false;
                cmbDepartment.IsEnabled = false;
                cmbPosition.IsEnabled = false;
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

        OpenFileDialog dialog = new OpenFileDialog();
        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == true)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(dialog.FileName);
                image.EndInit();

                EmployeeImage.Source = image;
                txtImage.Text = dialog.FileName;
            }

        }

        private void txtUserNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtUserNo.Text.Trim() == "" || txtPassword.Text.Trim() == "" || txtName.Text.Trim() == "" || txtSurname.Text.Trim() == ""
                || txtSalary.Text.Trim() == "" || cmbDepartment.SelectedIndex == -1 || cmbPosition.SelectedIndex == -1)
            {
                MessageBox.Show("please fill all details");
            }
            else
            {
                if (employeeDetailModel != null && employeeDetailModel.Id != 0)
                {

                    using (PersonalTrackingContext db = new PersonalTrackingContext())
                    {
                        Employee employee = db.Employees.Find(employeeDetailModel.Id);

                        List<Employee> employeelist = db.Employees.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)
                            && x.Id != employee.Id).ToList();

                        if (employeelist.Count > 0)
                        {
                            MessageBox.Show("This User no is already exists");
                        }
                        else
                        {

                            if (txtImage.Text.Trim() != "")
                            {
                                if (File.Exists(@"Images//" + employee.ImagePath))
                                {
                                    File.Delete(@"Images//" + employee.ImagePath);
                                    string filename = "";
                                    string unique = Guid.NewGuid().ToString();
                                    filename += unique + System.IO.Path.GetFileName(txtImage.Text);
                                    File.Copy(txtImage.Text, @"Images//" + filename);
                                    employee.ImagePath = filename;
                                }
                            }
                            employee.UserNo = Convert.ToInt32(txtUserNo.Text);
                            employee.Name = txtName.Text;
                            employee.Surname = txtSurname.Text;
                            employee.Password = txtPassword.Text;
                            employee.Salary = Convert.ToInt32(txtSalary.Text);
                            employee.DepartmentId = Convert.ToInt32(cmbDepartment.SelectedValue);
                            employee.PositionId = Convert.ToInt32(cmbPosition.SelectedValue);
                            TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                            employee.Address = textRange.Text;
                            employee.BirthDate = datePicker.SelectedDate;
                            employee.IsAdmin = (bool)chIsAdmin.IsChecked;

                            db.Employees.Update(employee);
                            db.SaveChanges();
                            MessageBox.Show("Employee updated successfully");
                        }
                    }
                    

                }
                else
                {
                    if (IsUniqueEmployee())
                    {
                        Employee employee = new Employee();
                        employee.UserNo = Convert.ToInt32(txtUserNo.Text);
                        employee.Name = txtName.Text;
                        employee.Surname = txtSurname.Text;
                        employee.Password = txtPassword.Text;
                        employee.Salary = Convert.ToInt32(txtSalary.Text);
                        employee.DepartmentId = Convert.ToInt32(cmbDepartment.SelectedValue);
                        employee.PositionId = Convert.ToInt32(cmbPosition.SelectedValue);
                        TextRange textRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                        employee.Address = textRange.Text;
                        employee.BirthDate = datePicker.SelectedDate;
                        employee.IsAdmin = (bool)chIsAdmin.IsChecked;

                        string filename = "";
                        string unique = Guid.NewGuid().ToString();
                        filename += unique + dialog.SafeFileName;
                        employee.ImagePath = filename;

                        using (PersonalTrackingContext db = new PersonalTrackingContext())
                        {
                            db.Employees.Add(employee);
                            db.SaveChanges();
                        }
                        File.Copy(txtImage.Text, @"Images/" + filename);

                        MessageBox.Show("Employee added succesfully");
                    }
                    txtUserNo.Clear();
                    txtName.Clear();
                    txtPassword.Clear();
                    txtSurname.Clear();
                    txtSalary.Clear();
                    datePicker.SelectedDate = DateTime.Today;
                    cmbDepartment.SelectedIndex = -1;
                    cmbPosition.ItemsSource = positions;
                    cmbPosition.SelectedIndex = -1;
                    txtAddress.Document.Blocks.Clear();
                    chIsAdmin.IsChecked = false;
                    EmployeeImage.Source = new BitmapImage();
                    txtImage.Clear();

                }

            }
        }
    

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            IsUniqueEmployee();
        }

        private bool IsUniqueEmployee()
        {
            bool isUnique = false;

            using (PersonalTrackingContext db = new PersonalTrackingContext())
            {
                var list = db.Employees.Where(x => x.UserNo == (Convert.ToInt32(txtUserNo.Text))).ToList();
                if (list.Count > 0)
                {
                    MessageBox.Show("This user no is already used by another employee ");
                }
                else
                {
                    MessageBox.Show("This user no is available");
                    isUnique = true;
                }
            }

            return isUnique;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
