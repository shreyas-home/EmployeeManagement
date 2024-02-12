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
    /// Interaction logic for PositionList.xaml
    /// </summary>
    public partial class PositionList : UserControl
    {
        public PositionList()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void FillGrid()
        {
            using (PersonalTrackingContext db = new PersonalTrackingContext())
            {
                var list = db.Positions.Include(x => x.Department).Select(a => new
                {
                    positionId = a.Id,
                    positionName = a.PositionName,
                    departmentId = a.DepartmentId,
                    departmentName = a.Department.DepartmentName
                }).OrderBy(x => x.positionName).ToList();

                List<PositionModel> positionModels = new List<PositionModel>();

                foreach (var position in list)
                {
                    PositionModel positionModel = new PositionModel();
                    positionModel.ID = position.positionId;
                    positionModel.PositionName = position.positionName;
                    positionModel.DepartmentName = position.departmentName;
                    positionModel.DepartmentID = position.departmentId;

                    positionModels.Add(positionModel);
                }

                gridPosition.ItemsSource = positionModels;
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PositionPage page = new PositionPage();
            page.ShowDialog();

            FillGrid();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            PositionModel model = gridPosition.SelectedItem as PositionModel;
            if(model != null && model.ID != 0)
            {
                PositionPage page = new PositionPage();
                page.positionModel = model;
                page.ShowDialog();
            }

            FillGrid();
        }
    }
}
