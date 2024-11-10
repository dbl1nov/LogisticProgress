using LogisticProgress.DataBase;
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

namespace LogisticProgress.Pages.Manager
{
    /// <summary>
    /// Interaction logic for SellingsPage.xaml
    /// </summary>
    public partial class SellingsPage : Page
    {
        public SellingsPage()
        {
            InitializeComponent();
            UpdateSellings();
        }

        private void UpdateSellings()
        {
            var sellings = ProgressLogisticEntities.GetContext().Sellings.OrderByDescending(s=>s.SellingId).ToList();
            foreach(Selling selling in sellings)
            {
                if(selling.Selling_Prod.Count == 0)
                {
                    ProgressLogisticEntities.GetContext().Sellings.Remove(selling);
                }
            }
            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
                sellings = ProgressLogisticEntities.GetContext().Sellings.OrderByDescending(s => s.SellingId).ToList();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            dgPurInv.ItemsSource = sellings;
        }

        private void btnLook_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Seller((sender as Button).DataContext as Selling));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var sellingsForRemove = dgPurInv.SelectedItems.Cast<Selling>().ToList();
            if (MessageBox.Show($"Вы уверены что хотите удалить информацию о следующем количестве выбранных продаж: {sellingsForRemove.Count}", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    ProgressLogisticEntities.GetContext().Sellings.RemoveRange(sellingsForRemove);
                    ProgressLogisticEntities.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    UpdateSellings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
