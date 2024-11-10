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
    /// Interaction logic for PurchaceInvocePage.xaml
    /// </summary>
    public partial class PurchaceInvocePage : Page
    {
        public PurchaceInvocePage()
        {
            InitializeComponent();
            UpdatePurInv();
        }

        private void UpdatePurInv()
        {
            var purInvoices = ProgressLogisticEntities.GetContext().PurchaceInvoces.OrderByDescending(s=>s.InvNum).ToList();
            dgPurInv.ItemsSource = purInvoices;
        }

        private void btnLook_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new AddLookPurInvPage((sender as Button).DataContext as PurchaceInvoce));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new AddLookPurInvPage(null));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var purInvForRemove = dgPurInv.SelectedItems.Cast<PurchaceInvoce>().ToList();
            if (MessageBox.Show($"Вы уверены что хотите удалить следующее количество выбранных накладных: {purInvForRemove.Count()}", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    ProgressLogisticEntities.GetContext().PurchaceInvoces.RemoveRange(purInvForRemove);
                    ProgressLogisticEntities.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    UpdatePurInv();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdatePurInv();
        }
    }
}
