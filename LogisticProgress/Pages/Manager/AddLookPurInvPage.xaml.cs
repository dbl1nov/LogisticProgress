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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticProgress.Pages.Manager
{
    /// <summary>
    /// Interaction logic for AddLookPurInvPage.xaml
    /// </summary>
    public partial class AddLookPurInvPage : Page
    {
        private PurchaceInvoce PurchaceInvoce = new PurchaceInvoce();
        public AddLookPurInvPage(PurchaceInvoce selectedPurchaceInvoce)
        {
            InitializeComponent();

            cbSupplier.ItemsSource = ProgressLogisticEntities.GetContext().Suppliers.OrderBy(s=>s.Name).ToList();
            cbWhoAccepted.ItemsSource = ProgressLogisticEntities.GetContext().Users.OrderBy(s=>s.LastName).ToList();

            if (selectedPurchaceInvoce != null)
            {
                PurchaceInvoce = selectedPurchaceInvoce;
                rowAddProduct.MaxHeight = 0;
                rowButtonSave.MaxHeight = 0;
                btnAddSupplier.Visibility = Visibility.Collapsed;
                tbInvNum.IsReadOnly = true;
                dpReceoptDate.IsEnabled = false;
                cbWhoAccepted.IsEnabled = false;
                cbSupplier.IsEnabled = false;
                dgColumnDelete.Visibility = Visibility.Collapsed;

                var products = ProgressLogisticEntities.GetContext().PurInv_Prod.Where(p => p.PurInvId == PurchaceInvoce.PurInvId).ToList();

                if (products.Count > 0)
                {
                    dgPurInvProducts.ItemsSource = products;
                }
            }


            if (selectedPurchaceInvoce == null)
            {
                PurchaceInvoce.ReceiptDate = DateTime.Today;
                cbProducts.SelectedIndex = 0;
                Classes.Manager.ButtonBackVisibility = false;
            }

            DataContext = PurchaceInvoce;
        }

        private void btnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            Windows.AddSupplierWindow addSupplier = new Windows.AddSupplierWindow();
            addSupplier.ShowDialog();
            cbSupplier.ItemsSource = ProgressLogisticEntities.GetContext().Suppliers.ToList();
        }

        private void btnAddToInv_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(tbQty.Text))
                errors.AppendLine("Введите количество товара!");
            if (string.IsNullOrWhiteSpace(tbPrice.Text))
                errors.AppendLine("Введите цену закупки товара!");
            if (dpReceoptDate.SelectedDate == null)
                errors.AppendLine("Выберите дату!");
            if (Validator.CheckNumbers(tbQty.Text) == false)
                errors.AppendLine("Количество товара должно быть положительным целым числом!");
            if (Validator.CheckPrice(tbPrice.Text) == false)
                errors.AppendLine("Цена товара должна быть положительным числом!");
            

            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            PurInv_Prod purInvProduct = new PurInv_Prod();
            purInvProduct.Product = cbProducts.SelectedItem as Product;
            purInvProduct.PurInvId = PurchaceInvoce.PurInvId;
            purInvProduct.Price = Convert.ToInt32(tbPrice.Text);
            purInvProduct.Qty = Convert.ToInt32(tbQty.Text);

            if(purInvProduct.Price > purInvProduct.Product.Price)
            {
                MessageBox.Show("Покупная цена товара не может превышать цену продажи!");
                return;
            }

            var products = ProgressLogisticEntities.GetContext().PurInv_Prod.Where(p => p.PurInvId == PurchaceInvoce.PurInvId).Where(p => p.ProdId == purInvProduct.Product.ProdId).ToList();
            if(products.Count > 0)
            {
                MessageBox.Show("Вы уже добавили данный товар!");
                return;
            }

            ProgressLogisticEntities.GetContext().PurInv_Prod.Add(purInvProduct);

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
                dgPurInvProducts.ItemsSource = ProgressLogisticEntities.GetContext().PurInv_Prod.Where(p => p.PurInvId == PurchaceInvoce.PurInvId).ToList();
                tblSumm.Text = ProgressLogisticEntities.GetContext().PurInv_Prod.Where(p => p.PurInvId == PurchaceInvoce.PurInvId).Sum(s => s.Price * s.Qty).ToString("0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            Windows.AddProductWindow addProduct = new Windows.AddProductWindow(null);
            addProduct.Title = "Добавление товара";
            addProduct.rowQty.MaxHeight = 0;
            addProduct.tbQty.Text = "0";

            addProduct.ShowDialog();
            cbProducts.ItemsSource = ProgressLogisticEntities.GetContext().Products.ToList();
        }

        private void btnAddProductsToPurInv_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(tbInvNum.Text))
                errors.AppendLine("Введите номер договора!");
            if (cbWhoAccepted.SelectedItem == null)
                errors.AppendLine("Выберите кто принял товар!");
            if (cbSupplier.SelectedItem == null)
                errors.AppendLine("Выберите поставщика!");
            if (Validator.CheckNumbers(tbInvNum.Text) == false)
                errors.AppendLine("Номер договора должен быть указан положительным целым числом!");

            var invNums = ProgressLogisticEntities.GetContext().PurchaceInvoces.Where(p => p.InvNum == PurchaceInvoce.InvNum).ToList();
            if (invNums.Count > 0)
                errors.AppendLine("Договор с таким номером уже существует!");

            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            ProgressLogisticEntities.GetContext().PurchaceInvoces.Add(PurchaceInvoce);

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
                btnAddProductsToPurInv.Visibility = Visibility.Hidden;
                grbAddProducts.Visibility = Visibility.Visible;
                tbInvNum.IsReadOnly = true;
                dpReceoptDate.IsEnabled = false;
                cbWhoAccepted.IsEnabled = false;
                cbSupplier.IsEnabled = false;
                btnAddSupplier.Visibility = Visibility.Collapsed;

                var purInv = ProgressLogisticEntities.GetContext().PurchaceInvoces.Where(p => p.InvNum == PurchaceInvoce.InvNum).ToList();
                PurchaceInvoce.PurInvId = purInv[0].PurInvId;
                cbProducts.ItemsSource = ProgressLogisticEntities.GetContext().Products.OrderBy(s => s.Name).Where(p => p.SupplierId == PurchaceInvoce.Supplier.SupplierId).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var purInvProducts = ProgressLogisticEntities.GetContext().PurInv_Prod.Where(p => p.PurInvId == PurchaceInvoce.PurInvId).ToList();
            var products = ProgressLogisticEntities.GetContext().Products.ToList();

            if(purInvProducts.Count == 0)
            {
                MessageBox.Show("Вы не внесли ни одного товара!");
                return;
            }

            foreach(PurInv_Prod purInv_Prod in purInvProducts)
            {
                var currentProduct = products.Where(p => p.ProdId == purInv_Prod.ProdId).ToList();
                currentProduct[0].Qty += purInv_Prod.Qty;
            }

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Classes.Manager.ButtonBackVisibility = true;
            Classes.Manager.MainFrame.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что не хотите сохранить данную накладную?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                var purInvForRemove = ProgressLogisticEntities.GetContext().PurchaceInvoces.Where(p => p.PurInvId == PurchaceInvoce.PurInvId).ToList();
                if (purInvForRemove.Count > 0)
                {
                    ProgressLogisticEntities.GetContext().PurchaceInvoces.RemoveRange(purInvForRemove);
                }

                try
                {
                    ProgressLogisticEntities.GetContext().SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                Classes.Manager.ButtonBackVisibility = true;
                Classes.Manager.MainFrame.GoBack();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Вы уверены, что хотите удалить выбранный товар?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question)==
                MessageBoxResult.Yes)
            {
                ProgressLogisticEntities.GetContext().PurInv_Prod.Remove((sender as Button).DataContext as PurInv_Prod);
                try
                {
                    ProgressLogisticEntities.GetContext().SaveChanges();
                    dgPurInvProducts.ItemsSource = ProgressLogisticEntities.GetContext().PurInv_Prod.Where(p => p.PurInvId == PurchaceInvoce.PurInvId).ToList();
                    tblSumm.Text = ProgressLogisticEntities.GetContext().PurInv_Prod.Where(p => p.PurInvId == PurchaceInvoce.PurInvId).Sum(s => s.Price * s.Qty).ToString("0.00");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
