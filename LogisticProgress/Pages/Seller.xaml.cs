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

namespace LogisticProgress.Pages
{
    /// <summary>
    /// Interaction logic for Seller.xaml
    /// </summary>
    public partial class Seller : Page
    {
        private Selling selling = new Selling();
        public Seller(Selling selectedSelling)
        {
            InitializeComponent();

            cbProducts.ItemsSource = ProgressLogisticEntities.GetContext().Products.OrderBy(s => s.Name).ToList();
            cbClients.ItemsSource = ProgressLogisticEntities.GetContext().Clients.OrderBy(s => s.Name).ToList();

            if (selectedSelling != null)
            {
                selling = selectedSelling;
                grbAddProducts.Visibility = Visibility.Hidden;
                grbAboutSelling.Visibility = Visibility.Visible;
                dgColumnDelete.Visibility = Visibility.Collapsed;
                rowButtonSave.MaxHeight = 0;

                var products = ProgressLogisticEntities.GetContext().Selling_Prod.Where(p => p.SellingId == selectedSelling.SellingId).ToList();

                if (products.Count > 0)
                {
                    dgPurInvProducts.ItemsSource = products;
                }
            }
            else
            {
                selling.DateTime = DateTime.Now;
                selling.SellerId = Classes.Manager.UserId;
                ProgressLogisticEntities.GetContext().Sellings.Add(selling);
                cbProducts.SelectedIndex = 0;
                try
                {
                    ProgressLogisticEntities.GetContext().SaveChanges();
                    var sellings = ProgressLogisticEntities.GetContext().Sellings.Last();
                    selling.SellingId = sellings.SellingId;
                }
                catch(Exception ex)
                {
                    
                }
            }

            DataContext = selling;
        }

        private void btnAddToInv_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(tbQty.Text))
                errors.AppendLine("Введите количество товара!");
            if (Classes.Validator.CheckNumbers(tbQty.Text) == false)
                errors.AppendLine("Количество товара должно быть положительным целым числом!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            Selling_Prod sellingProduct = new Selling_Prod();
            sellingProduct.Product = cbProducts.SelectedItem as Product;
            sellingProduct.SellingId = selling.SellingId;
            sellingProduct.Price = sellingProduct.Product.Price;
            sellingProduct.Qty = Convert.ToInt32(tbQty.Text);

            var products = ProgressLogisticEntities.GetContext().Selling_Prod.Where(p => p.SellingId == selling.SellingId).Where(p => p.ProdId == sellingProduct.Product.ProdId).ToList();
            if (products.Count > 0)
            {
                MessageBox.Show("Вы уже добавили данный товар!");
                return;
            }
            var productes = ProgressLogisticEntities.GetContext().Products.Where(p => p.ProdId == sellingProduct.Product.ProdId).ToList();
            if (productes[0].Qty < sellingProduct.Qty)
            {
                MessageBox.Show("Недостаточно товара на складе!");
                return;
            }

            ProgressLogisticEntities.GetContext().Selling_Prod.Add(sellingProduct);

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
                dgPurInvProducts.ItemsSource = ProgressLogisticEntities.GetContext().Selling_Prod.Where(p => p.SellingId == selling.SellingId).ToList();
                tblSumm.Text = ProgressLogisticEntities.GetContext().Selling_Prod.Where(p => p.SellingId == selling.SellingId).Sum(s => s.Price * s.Qty).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var sellingProducts = ProgressLogisticEntities.GetContext().Selling_Prod.Where(p => p.SellingId == selling.SellingId).ToList();
            var products = ProgressLogisticEntities.GetContext().Products.ToList();

            if (sellingProducts.Count == 0)
            {
                MessageBox.Show("Вы не внесли ни одного товара!");
                return;
            }

            foreach (Selling_Prod selling_Prod in sellingProducts)
            {
                var currentProduct = products.Where(p => p.ProdId == selling_Prod.ProdId).ToList();
                currentProduct[0].Qty -= selling_Prod.Qty;
            }
            var sell = ProgressLogisticEntities.GetContext().Sellings.Where(p => p.SellingId == selling.SellingId).ToList();
            sell[0].DateTime = DateTime.Now;
            sell[0].Client = cbClients.SelectedItem as Client;

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Classes.Manager.MainFrame.Navigate(new Seller(null));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что не хотите сохранить данные о продаже?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                var sellingsForRemove = ProgressLogisticEntities.GetContext().Sellings.Where(p => p.SellingId == selling.SellingId).ToList();
                if (sellingsForRemove.Count > 0)
                {
                    ProgressLogisticEntities.GetContext().Sellings.RemoveRange(sellingsForRemove);
                }

                try
                {
                    ProgressLogisticEntities.GetContext().SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                Classes.Manager.MainFrame.Navigate(new Seller(null));
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить выбранный товар?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes)
            {
                ProgressLogisticEntities.GetContext().Selling_Prod.Remove((sender as Button).DataContext as Selling_Prod);
                try
                {
                    ProgressLogisticEntities.GetContext().SaveChanges();
                    var sellProds = ProgressLogisticEntities.GetContext().Selling_Prod.Where(p => p.SellingId == selling.SellingId).ToList();
                    if (sellProds.Count == 0)
                    {
                        tblSumm.Text = "0.00";
                        dgPurInvProducts.ItemsSource = null;
                    }
                    else
                    {
                        dgPurInvProducts.ItemsSource = ProgressLogisticEntities.GetContext().Selling_Prod.Where(p => p.SellingId == selling.SellingId).ToList();
                        tblSumm.Text = ProgressLogisticEntities.GetContext().Selling_Prod.Where(p => p.SellingId == selling.SellingId).Sum(s => s.Price * s.Qty).ToString("0.00");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что не хотите сохранить данные о продаже?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question)
               == MessageBoxResult.Yes)
            {
                var sellingsForRemove = ProgressLogisticEntities.GetContext().Sellings.Where(p => p.SellingId == selling.SellingId).ToList();
                if (sellingsForRemove.Count > 0)
                {
                    ProgressLogisticEntities.GetContext().Sellings.RemoveRange(sellingsForRemove);
                }

                try
                {
                    ProgressLogisticEntities.GetContext().SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                Application.Current.Shutdown();
            }
        }
    }
}
