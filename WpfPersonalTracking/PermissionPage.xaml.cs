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
    /// Interaction logic for PermissionPage.xaml
    /// </summary>
    public partial class PermissionPage : Window
    {
        public PermissionPage()
        {
            InitializeComponent();
        }

        TimeSpan tsPermissionDay = new TimeSpan();
        PersonalTrackingContext db = new PersonalTrackingContext();
        public PermissionDetailModel model = new PermissionDetailModel();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUserNo.Text = UserStatic.UserNo.ToString();
            if(model!=null && model.Id != 0)
            {
                txtUserNo.Text = model.UserNo.ToString();
                txtDayAmount.Text = model.DayAmount.ToString();
                txtExplanation.Text = model.Explanation;
                dpEnd.SelectedDate = model.EndDate;
                dpStart.SelectedDate = model.StartDate;
            }
        }

        private void dpStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpEnd.SelectedDate != null)
            {
                tsPermissionDay = (TimeSpan)(dpEnd.SelectedDate - dpStart.SelectedDate);
                txtDayAmount.Text = tsPermissionDay.TotalDays.ToString();
            }
        }

        private void dpEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpStart.SelectedDate != null)
            {
                tsPermissionDay = (TimeSpan)(dpEnd.SelectedDate - dpStart.SelectedDate);
                txtDayAmount.Text = tsPermissionDay.TotalDays.ToString();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtDayAmount.Text.Trim() == "")
            {
                MessageBox.Show("Please select start date and end date");
            }
            else if(Convert.ToInt32(txtDayAmount.Text)<=0)
            {
                MessageBox.Show("Permission Day Amount must be greater than 0");
            }
            else if(txtExplanation.Text.Trim()=="")
            {
                MessageBox.Show("Please write Explanation");
            }
            else
            {
                if (model!=null && model.Id!=0)
                {
                    Permission permission = db.Permissions.Find(model.Id);
                    permission.PermissionStartDate = (DateTime)dpStart.SelectedDate;
                    permission.PermissionEndDate = (DateTime)dpEnd.SelectedDate;
                    permission.PermissionDay = Convert.ToInt32(txtDayAmount.Text);
                    permission.PermissionExplanation = txtExplanation.Text;
                    db.SaveChanges();
                    MessageBox.Show("Permission updated");
                }
                else {
                    Permission permission = new Permission();
                    permission.EmployeeId = UserStatic.EmployeeId;
                    permission.PermissionState = Definitions.PermissionStates.OnAdmin;
                    permission.PermissionStartDate = (DateTime)dpStart.SelectedDate;
                    permission.PermissionEndDate = (DateTime)dpEnd.SelectedDate;
                    permission.PermissionDay = Convert.ToInt32(txtDayAmount.Text);
                    permission.PermissionExplanation = txtExplanation.Text;
                    db.Permissions.Add(permission);
                    db.SaveChanges();
                    MessageBox.Show("Permission Added");
                    dpStart.SelectedDate = DateTime.Today;
                    dpEnd.SelectedDate = DateTime.Today;
                    txtDayAmount.Clear();
                    txtExplanation.Clear();
                }
                
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
