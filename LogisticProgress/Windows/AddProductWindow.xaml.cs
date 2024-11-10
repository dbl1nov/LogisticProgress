using LogisticProgress.DataBase;
using LogisticProgress.Classes;
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

namespace LogisticProgress.Windows
{
    /// <summary>
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private Product product = new Product();
        public AddProductWindow(Product selectedProduct)
        {
            InitializeComponent();

            cbSupplier.ItemsSource = ProgressLogisticEntities.GetContext().Suppliers.ToList();
            if (selectedProduct != null)
            {
                product = selectedProduct;
            }

            DataContext = product;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(product.Name))
                errors.AppendLine("Введите название!");
            if (string.IsNullOrWhiteSpace(tbPrice.Text))
                errors.AppendLine("Введите цену!");
            if (string.IsNullOrWhiteSpace(tbQty.Text))
                errors.AppendLine("Введите количество на складе!");
            if (string.IsNullOrWhiteSpace(tbRefQty.Text))
                errors.AppendLine("Введите эталонное количество!");
            if (cbSupplier.SelectedItem == null)
                errors.AppendLine("Выберите поставщика!");
            if (product.Price <= 0)
                errors.AppendLine("Цена должна быть положительной!");
            if (product.RefQty <= 0)
                errors.AppendLine("Эталонное количество должно быть больше 0!");
            if (product.Qty < 0 || product.Qty > 10000)
                errors.AppendLine("Укажите правильное количество товара!");
            if (product.RefQty < 0 || product.RefQty > 10000)
                errors.AppendLine("Укажите правильное эталонное количество товара!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (product.ProdId == 0)
                ProgressLogisticEntities.GetContext().Products.Add(product);
            product.CategoryId = 1;

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
                Classes.ABC_analysis.GiveABCCategory();
                MessageBox.Show("Данные сохранены!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
