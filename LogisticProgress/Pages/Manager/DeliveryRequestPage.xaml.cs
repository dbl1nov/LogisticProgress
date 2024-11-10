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
    /// Interaction logic for DeliveryRequestPage.xaml
    /// </summary>
    public partial class DeliveryRequestPage : Page
    {
        public DeliveryRequestPage()
        {
            InitializeComponent();
            UpdateDevReq();
            UpdateLeft();
        }

        private void UpdateDevReq()
        {
            var devRequests = ProgressLogisticEntities.GetContext().DeliveryRequests.OrderByDescending(s=>s.ReqNum).ToList();
            dgDevReq.ItemsSource = devRequests;
        }

        private void UpdateLeft()
        {
            string needRequest = Classes.ABC_analysis.NeedRequest();
            string inProcess = Classes.ABC_analysis.InProcess();
            if (needRequest.Length == 0 && inProcess.Length == 0)
            {
                columnNeedRequest.MaxWidth = 0;
            }
            else
            {
                if (needRequest.Length > 0)
                {
                    tblNeedRequest.Text = needRequest;
                }
                else
                {
                    tblNeedRequest.Visibility = Visibility.Collapsed;
                    tblNeedRequestTitle.Visibility = Visibility.Collapsed;
                }

                if (inProcess.Length > 0)
                {
                    tblInProcess.Text = inProcess;
                }
                else
                {
                    tblInProcessTitle.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new AddLookDeliveryRequestPage(null));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var devReqForRemove = dgDevReq.SelectedItems.Cast<DeliveryRequest>().ToList();
            if (MessageBox.Show($"Вы уверены что хотите удалить следующее количество выбранных накладных: {devReqForRemove.Count}", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    ProgressLogisticEntities.GetContext().DeliveryRequests.RemoveRange(devReqForRemove);
                    ProgressLogisticEntities.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    UpdateDevReq();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnLook_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new AddLookDeliveryRequestPage((sender as Button).DataContext as DeliveryRequest));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateDevReq();
            UpdateLeft();
        }
    }
}
