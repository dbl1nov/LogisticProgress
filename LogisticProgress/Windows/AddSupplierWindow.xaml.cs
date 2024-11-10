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
using System.Windows.Shapes;

namespace LogisticProgress.Windows
{
    /// <summary>
    /// Interaction logic for AddSupplierWindow.xaml
    /// </summary>
    public partial class AddSupplierWindow : Window
    {
        private Supplier supplier = new Supplier();
        public AddSupplierWindow()
        {
            InitializeComponent();
            DataContext = supplier;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(supplier.Name))
                errors.AppendLine("Введите название!");
            if (string.IsNullOrWhiteSpace(supplier.Phone))
                errors.AppendLine("Введите телефон!");
            if (string.IsNullOrWhiteSpace(supplier.City))
                errors.AppendLine("Введите город!");
            if (string.IsNullOrWhiteSpace(supplier.Street))
                errors.AppendLine("Введите улицу!");
            if (string.IsNullOrWhiteSpace(tbHouse.Text))
                errors.AppendLine("Введите дом!");

            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            ProgressLogisticEntities.GetContext().Suppliers.Add(supplier);

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
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
