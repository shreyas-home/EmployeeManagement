using System;
using System.Collections.Generic;
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

namespace WpfPersonalTracking
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        PersonalTrackingContext db = new PersonalTrackingContext();
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(txtUserNo.Text.Trim()=="" || txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Please fill User No and Password");
            }
            else
            {
                Employee employee = db.Employees.FirstOrDefault(x=>x.UserNo == Convert.ToInt32(txtUserNo.Text) && 
                                                                    x.Password.Equals(txtPassword.Text));
                if(employee!= null && employee.Id!=0)
                {
                    UserStatic.EmployeeId = employee.Id;
                    UserStatic.UserNo = employee.UserNo;
                    UserStatic.Name = employee.Name;
                    UserStatic.Surname = employee.Surname;
                    UserStatic.IsAdmin = employee.IsAdmin;

                    this.Visibility = Visibility.Collapsed;
                    MainWindow main = new MainWindow();
                    main.ShowDialog();
                    txtUserNo.Clear();
                    txtPassword.Clear();
                    this.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Incorrect User No or Password");
                }

            }
        }

        private void txtUserNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}
