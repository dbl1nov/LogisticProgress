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
    /// Interaction logic for ABC_analysisPage.xaml
    /// </summary>
    public partial class ABC_analysisPage : Page
    {
        public ABC_analysisPage()
        {
            InitializeComponent();

            var products = ProgressLogisticEntities.GetContext().Products.OrderByDescending(s => s.Price * s.RefQty).ThenBy(s => s.Name).ToList();

            decimal cumulativeCost = 0;
            int cumulativeAmount = 0;

            foreach(Product product in products)
            {
                cumulativeCost += product.Summ;
                cumulativeAmount += (int)product.RefQty;

                product.CumulativeCost = cumulativeCost;
                product.CumulativeAmount = cumulativeAmount;
            }

            foreach(Product product in products)
            {
                product.PercentageOfCumAmount = Convert.ToDecimal(product.CumulativeAmount) / cumulativeAmount * 100;
                product.PercentageOfCumCost = product.CumulativeCost / cumulativeCost * 100;
            }

            dgABCanalysis.ItemsSource = products;
        }

        private void btnEditStockPoint_Click(object sender, RoutedEventArgs e)
        {
            Windows.EditStockPointsWindow editStockPoints = new Windows.EditStockPointsWindow();
            editStockPoints.ShowDialog();
        }
    }
}
