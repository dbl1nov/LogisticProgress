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
    /// Interaction logic for EditStockPointsWindow.xaml
    /// </summary>
    public partial class EditStockPointsWindow : Window
    {
        public EditStockPointsWindow()
        {
            InitializeComponent();
            var category = ProgressLogisticEntities.GetContext().Categories.ToList();
            tbCategoryA.Text = category[0].StockPoint.ToString();
            tbCategoryB.Text = category[1].StockPoint.ToString();
            tbCategoryC.Text = category[2].StockPoint.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(tbCategoryA.Text))
                errors.AppendLine("Введите точку запаса для категории А");
            if (string.IsNullOrWhiteSpace(tbCategoryB.Text))
                errors.AppendLine("Введите точку запаса для категории B");
            if (string.IsNullOrWhiteSpace(tbCategoryC.Text))
                errors.AppendLine("Введите точку запаса для категории C");
            if (Validator.CheckNumbers(tbCategoryA.Text)==false || Validator.CheckNumbers(tbCategoryB.Text) == false || 
                Validator.CheckNumbers(tbCategoryC.Text) == false)
                errors.AppendLine("Точка запаса может быть только положительным числом, состоящим из одной или двух цифр!");

            var category = ProgressLogisticEntities.GetContext().Categories.ToList();
            category[0].StockPoint = Convert.ToInt32(tbCategoryA.Text);
            category[1].StockPoint = Convert.ToInt32(tbCategoryB.Text);
            category[2].StockPoint = Convert.ToInt32(tbCategoryC.Text);

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
