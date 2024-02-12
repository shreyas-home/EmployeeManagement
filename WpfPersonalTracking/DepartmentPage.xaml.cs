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

namespace WpfPersonalTracking
{
    /// <summary>
    /// Interaction logic for DepartmentPage.xaml
    /// </summary>
    public partial class DepartmentPage : Window
    {
        public Department dpt;
        public DepartmentPage()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(txtDepartmentName.Text))
            {
                MessageBox.Show("Please fill Department name..");
            }
            else
            {
                using(PersonalTrackingContext db = new PersonalTrackingContext())
                {
                    if (dpt != null && dpt.Id !=0)
                    {
                        Department updatedDepartment = new Department();
                        updatedDepartment.DepartmentName = txtDepartmentName.Text.Trim();
                        updatedDepartment.Id = dpt.Id;
                        db.Departments.Update(updatedDepartment);
                        db.SaveChanges();
                        txtDepartmentName.Clear();
                        MessageBox.Show("Department Updated Successfully");
                    }
                    else 
                    {
                        Department department = new Department();
                        department.DepartmentName = txtDepartmentName.Text.Trim().ToString();
                        db.Departments.Add(department);
                        db.SaveChanges();
                        txtDepartmentName.Clear();
                        MessageBox.Show("Department Added Successfully");
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(dpt != null && dpt.Id!=0)
            {
                txtDepartmentName.Text = dpt.DepartmentName;
            }
        }
    }
}
