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
    /// Interaction logic for PositionPage.xaml
    /// </summary>
    public partial class PositionPage : Window
    {
        public PositionModel positionModel;
        public PositionPage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using(PersonalTrackingContext db = new PersonalTrackingContext())
            {
                var list = db.Departments.ToList().OrderBy(x => x.DepartmentName);
                cmbDepartment.ItemsSource = list;
                cmbDepartment.DisplayMemberPath = "DepartmentName";
                cmbDepartment.SelectedValuePath = "Id";
                cmbDepartment.SelectedIndex = -1;

                if(positionModel!= null && positionModel.ID != 0)
                {
                    cmbDepartment.SelectedValue = positionModel.DepartmentID;
                    txtPosition.Text = positionModel.PositionName;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtPosition.Text.Trim()) || cmbDepartment.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all areas");
            }
            else
            {
                if (positionModel != null && positionModel.ID != 0)
                {
                    Position pst = new Position();
                    pst.PositionName = txtPosition.Text;
                    pst.DepartmentId = (int)cmbDepartment.SelectedValue;
                    pst.Id = positionModel.ID;
                    using (PersonalTrackingContext db = new PersonalTrackingContext())
                    {
                        db.Positions.Update(pst);
                        db.SaveChanges();
                    }
                    cmbDepartment.SelectedIndex = -1;
                    txtPosition.Clear();
                    MessageBox.Show("Position Updated Succesfully");
                }
                else
                {
                    Position position = new Position();
                    position.PositionName = txtPosition.Text.Trim();
                    position.DepartmentId = Convert.ToInt32(cmbDepartment.SelectedValue);
                    using (PersonalTrackingContext db = new PersonalTrackingContext())
                    {
                        db.Positions.Add(position);
                        db.SaveChanges();
                    }
                    cmbDepartment.SelectedIndex = -1;
                    txtPosition.Clear();
                    MessageBox.Show("Position Added Succesfully");
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
