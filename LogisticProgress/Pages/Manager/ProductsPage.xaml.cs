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
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            
            cbxCategory.Items.Add("Все товары");
            cbxCategory.Items.Add("Категория А");
            cbxCategory.Items.Add("Категория В");
            cbxCategory.Items.Add("Категория С");

            cbxCategory.SelectedIndex = 1;
            UpdateProducts();
        }

        private void UpdateProducts()
        {
            var products = ProgressLogisticEntities.GetContext().Products.OrderBy(s => s.CategoryId).ThenBy(s => s.Name).ToList();
            if(cbxCategory.SelectedIndex == 1)
            {
                products = products.Where(p => p.CategoryId == 1).ToList();
            }
            if (cbxCategory.SelectedIndex == 2)
            {
                products = products.Where(p => p.CategoryId == 2).ToList();
            }
            if (cbxCategory.SelectedIndex == 3)
            {
                products = products.Where(p => p.CategoryId == 3).ToList();
            }

            dgProducts.ItemsSource = products;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Windows.AddProductWindow addProduct = new Windows.AddProductWindow((sender as Button).DataContext as Product);
            addProduct.Title = "Редактирование товара";
            addProduct.ShowDialog();

            UpdateProducts();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Windows.AddProductWindow addProduct = new Windows.AddProductWindow(null);
            addProduct.Title = "Добавление товара";
            addProduct.ShowDialog();

            UpdateProducts();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var productsForRemove = dgProducts.SelectedItems.Cast<Product>().ToList();
            if(MessageBox.Show($"Вы уверены что хотите удалить следующее количество выбранных товаров: {productsForRemove.Count}", "Внимание!", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    ProgressLogisticEntities.GetContext().Products.RemoveRange(productsForRemove);
                    ProgressLogisticEntities.GetContext().SaveChanges();
                    Classes.ABC_analysis.GiveABCCategory();
                    MessageBox.Show("Данные удалены!");
                    UpdateProducts();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnCheckStockPoints_Click(object sender, RoutedEventArgs e)
        {
            var products = ProgressLogisticEntities.GetContext().Products.ToList();
            StringBuilder needRequest = new StringBuilder();

            foreach (Product product in products)
            {
                decimal percentCount = Convert.ToDecimal(product.RefQty) / 100 * product.Category.StockPoint;
                if (product.Qty < percentCount)
                    needRequest.AppendLine($"• {product.Name}");
            }

            if (needRequest.Length > 0)
            {
                if (MessageBox.Show($"Недостаточно на складе:\n{needRequest.ToString()}\nПерейти к составлению заявки на закупку?", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    Classes.Manager.MainFrame.Navigate(new AddLookDeliveryRequestPage(null));
                }
            }
            else
            {
                MessageBox.Show("Товара на складе достаточно!");
            }
        }

        private void cbxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }
    }
}
