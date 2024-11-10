using LogisticProgress.Classes;
using LogisticProgress.DataBase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using Word = Microsoft.Office.Interop.Word;

namespace LogisticProgress.Pages.Manager
{
    /// <summary>
    /// Interaction logic for AddLookDeliveryRequestPage.xaml
    /// </summary>
    public partial class AddLookDeliveryRequestPage : Page
    {
        private DeliveryRequest deliveryRequest = new DeliveryRequest();
        public AddLookDeliveryRequestPage(DeliveryRequest selectedDeliveryRequest)
        {
            InitializeComponent();

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

                if(inProcess.Length > 0)
                {
                    tblInProcess.Text = inProcess;
                }
                else
                {
                    tblInProcessTitle.Visibility = Visibility.Collapsed;
                }
            }



            cbSupplier.ItemsSource = ProgressLogisticEntities.GetContext().Suppliers.OrderBy(s => s.Name).ToList();
            cbComplier.ItemsSource = ProgressLogisticEntities.GetContext().Users.OrderBy(s => s.LastName).ToList();

            if(selectedDeliveryRequest != null)
            {
                deliveryRequest = selectedDeliveryRequest;
                rowAddProduct.MaxHeight = 0;
                btnAddSupplier.Visibility = Visibility.Collapsed;
                tbInvNum.IsReadOnly = true;
                dpDateOfDelivery.IsEnabled = false;
                dpDateOfPreporation.IsEnabled = false;
                cbComplier.IsEnabled = false;
                cbSupplier.IsEnabled = false;
                dgColumnDelete.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Collapsed;
                btnCancel.Visibility = Visibility.Collapsed; 
                btnSavePDF.Visibility = Visibility.Visible;

                var products = ProgressLogisticEntities.GetContext().DevReq_Prod.Where(p => p.DevReqId == selectedDeliveryRequest.DevReqId).ToList();
                if (products.Count > 0)
                {
                    dgDevReqProducts.ItemsSource = products;
                }
            }

            DataContext = deliveryRequest;

            if (selectedDeliveryRequest == null)
            {
                deliveryRequest.DateOfDelivery = DateTime.Today;
                deliveryRequest.DateOfPreparation = DateTime.Today;
                dpDateOfPreporation.IsEnabled = false;
                cbProducts.SelectedIndex = 0;
                Classes.Manager.ButtonBackVisibility = false;
                btnSavePDF.Visibility = Visibility.Collapsed;
            }
        }

        private void btnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            Windows.AddSupplierWindow addSupplier = new Windows.AddSupplierWindow();
            addSupplier.ShowDialog();
            cbSupplier.ItemsSource = ProgressLogisticEntities.GetContext().Suppliers.ToList();
        }

        private void btnAddProductsToDevReq_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(tbInvNum.Text))
                errors.AppendLine("Введите номер договора!");
            if (dpDateOfDelivery.SelectedDate == null)
                errors.AppendLine("Выберите дату доставки!");
            if (dpDateOfDelivery.SelectedDate <= dpDateOfPreporation.SelectedDate)
                errors.AppendLine("Дата доставки выбрана неправильно!");
            if (cbComplier.SelectedItem == null)
                errors.AppendLine("Выберите составителя заявки!");
            if (cbSupplier.SelectedItem == null)
                errors.AppendLine("Выберите поставщика!");
            if (Validator.CheckNumbers(tbInvNum.Text) == false)
                errors.AppendLine("Номер договора должен быть указан положительным целым числом!");

            var reqNums = ProgressLogisticEntities.GetContext().DeliveryRequests.Where(p => p.ReqNum == deliveryRequest.ReqNum).ToList();
            if (reqNums.Count > 0)
                errors.AppendLine("Договор с таким номером уже существует!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            ProgressLogisticEntities.GetContext().DeliveryRequests.Add(deliveryRequest);

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
                btnAddProductsToDevReq.Visibility = Visibility.Hidden;
                grbAddProducts.Visibility = Visibility.Visible;
                tbInvNum.IsReadOnly = true;
                dpDateOfDelivery.IsEnabled = false;
                dpDateOfPreporation.IsEnabled = false;
                cbComplier.IsEnabled = false;
                cbSupplier.IsEnabled = false;
                btnAddSupplier.Visibility = Visibility.Collapsed;

                var devReq = ProgressLogisticEntities.GetContext().DeliveryRequests.Where(p => p.ReqNum == deliveryRequest.ReqNum).ToList();
                deliveryRequest.DevReqId = devReq[0].DevReqId;
                cbProducts.ItemsSource = ProgressLogisticEntities.GetContext().Products.OrderBy(s => s.Name).Where(p => p.SupplierId == deliveryRequest.Supplier.SupplierId).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddToReq_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(tbQty.Text))
                errors.AppendLine("Введите количество товара!");
            if (string.IsNullOrWhiteSpace(tbPrice.Text))
                errors.AppendLine("Введите цену закупки товара!");
            if (Validator.CheckNumbers(tbQty.Text) == false)
                errors.AppendLine("Количество товара должно быть положительным целым числом!");
            if (Validator.CheckPrice(tbPrice.Text) == false)
                errors.AppendLine("Цена товара должна быть положительным числом!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            DevReq_Prod devReqProduct = new DevReq_Prod();
            devReqProduct.Product = cbProducts.SelectedItem as Product;
            devReqProduct.DevReqId = deliveryRequest.DevReqId;
            devReqProduct.Price = Convert.ToInt32(tbPrice.Text);
            devReqProduct.Qty = Convert.ToInt32(tbQty.Text);

            var products = ProgressLogisticEntities.GetContext().DevReq_Prod.Where(p => p.DevReqId == deliveryRequest.DevReqId).Where(p => p.ProdId == devReqProduct.Product.ProdId).ToList();
            if (products.Count > 0)
            {
                MessageBox.Show("Вы уже добавили данный товар!");
                return;
            }

            ProgressLogisticEntities.GetContext().DevReq_Prod.Add(devReqProduct);

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
                dgDevReqProducts.ItemsSource = ProgressLogisticEntities.GetContext().DevReq_Prod.Where(p => p.DevReqId == deliveryRequest.DevReqId).ToList();
                tblSumm.Text = ProgressLogisticEntities.GetContext().DevReq_Prod.Where(p => p.DevReqId == deliveryRequest.DevReqId).Sum(s => s.Price * s.Qty).ToString("0.00");
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить выбранный товар?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
               MessageBoxResult.Yes)
            {
                ProgressLogisticEntities.GetContext().DevReq_Prod.Remove((sender as Button).DataContext as DevReq_Prod);
                try
                {
                    ProgressLogisticEntities.GetContext().SaveChanges();
                    dgDevReqProducts.ItemsSource = ProgressLogisticEntities.GetContext().DevReq_Prod.Where(p => p.DevReqId == deliveryRequest.DevReqId).ToList();
                    tblSumm.Text = ProgressLogisticEntities.GetContext().DevReq_Prod.Where(p => p.DevReqId == deliveryRequest.DevReqId).Sum(s => s.Price * s.Qty).ToString("0.00");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var devReqProducts = ProgressLogisticEntities.GetContext().DevReq_Prod.Where(p => p.DevReqId == deliveryRequest.DevReqId).ToList();
            var products = ProgressLogisticEntities.GetContext().Products.ToList();

            if (devReqProducts.Count == 0)
            {
                MessageBox.Show("Вы не внесли ни одного товара!");
                return;
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
            if (MessageBox.Show("Вы уверены, что не хотите сохранить данную заявку?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                var devReqForRemove = ProgressLogisticEntities.GetContext().DeliveryRequests.Where(p => p.DevReqId == deliveryRequest.DevReqId).ToList();
                if (devReqForRemove.Count > 0)
                {
                    ProgressLogisticEntities.GetContext().DeliveryRequests.RemoveRange(devReqForRemove);
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

        private void btnSavePDF_Click(object sender, RoutedEventArgs e)
        {
            var devReqProd = ProgressLogisticEntities.GetContext().DevReq_Prod.Where(p => p.DevReqId == deliveryRequest.DevReqId).ToList();

            var application = new Word.Application();

            Word.Document document = application.Documents.Add();
            application.Selection.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;

            application.Selection.Font.Size = 14;
            application.Selection.Font.Name = "Times New Roman";
            application.Selection.ParagraphFormat.LineSpacing = 11;
            application.Selection.ParagraphFormat.LineUnitBefore = 0;

            var date = (DateTime)deliveryRequest.DateOfPreparation;
            var date2 = date.ToString("dd.MM.yyyy");

            application.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
            application.Selection.ParagraphFormat.LineSpacing = 7;
            application.Selection.TypeText($"Кому: \"{deliveryRequest.Supplier.Name}\" \nОт ИП \"Блинова Татьяна Валерьевна\"\n г. Краснодар, ул. Северная, д. 23, оф. 5" +
               $"\nт. +7 (938) 530-29-77\n{date2} г.");
            application.Selection.TypeParagraph();
            application.Selection.TypeParagraph();

            application.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            application.Selection.ParagraphFormat.LineSpacing = 11;
            application.Selection.TypeText($"Заявка на поставку №{deliveryRequest.ReqNum}");
            application.Selection.TypeParagraph();


            var date3 = (DateTime)deliveryRequest.DateOfDelivery;
            var date4 = date3.ToString("dd.MM.yyyy");
            application.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            application.Selection.TypeText($"Прошу поставить до {date4} г. следующие наименования товара:");
            application.Selection.TypeParagraph();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table requestTable = document.Tables.Add(tableRange, devReqProd.Count()+1, 5);
            requestTable.Borders.InsideLineStyle = requestTable.Borders.OutsideLineStyle
                = Word.WdLineStyle.wdLineStyleSingle;
            requestTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            Word.Range cellRange;
            cellRange = requestTable.Cell(1, 1).Range;
            cellRange.Text = "№ п.п.";
            cellRange = requestTable.Cell(1, 2).Range;
            cellRange.Text = "Наименование";
            cellRange = requestTable.Cell(1, 3).Range;
            cellRange.Text = "Количество";
            cellRange = requestTable.Cell(1, 4).Range;
            cellRange.Text = "Цена за 1 ед.";
            cellRange = requestTable.Cell(1, 5).Range;
            cellRange.Text = "Общая стоимость";

            requestTable.Rows[1].Range.Bold = 1;
            requestTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            requestTable.Columns[1].SetWidth(51, Word.WdRulerStyle.wdAdjustNone);
            requestTable.Columns[2].SetWidth(357, Word.WdRulerStyle.wdAdjustNone);
            requestTable.Columns[3].SetWidth(100, Word.WdRulerStyle.wdAdjustNone);
            requestTable.Columns[4].SetWidth(111, Word.WdRulerStyle.wdAdjustNone);
            requestTable.Columns[5].SetWidth(111, Word.WdRulerStyle.wdAdjustNone);

            for (int i = 0; i < devReqProd.Count(); i++)
            {
                var currentProd = devReqProd[i];
                cellRange = requestTable.Cell(i + 2, 1).Range;
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                cellRange.Text = $"{i + 1}";

                cellRange = requestTable.Cell(i + 2, 2).Range;
                cellRange.Text = currentProd.Product.Name;

                cellRange = requestTable.Cell(i + 2, 3).Range;
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                cellRange.Text = $"{currentProd.Qty} шт.";

                cellRange = requestTable.Cell(i + 2, 4).Range;
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                cellRange.Text = currentProd.Price.ToString("F");

                cellRange = requestTable.Cell(i + 2, 5).Range;
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                cellRange.Text = currentProd.Summ.ToString("F");
            }


            Object oConst1 = Word.WdGoToItem.wdGoToLine;
            Object oConst2 = Word.WdGoToDirection.wdGoToLast;
            application.Selection.GoTo(ref oConst1, ref oConst2);
            application.Selection.ParagraphFormat.LineSpacing = 3;
            application.Selection.TypeParagraph();
            application.Selection.ParagraphFormat.LineSpacing = 11;

            application.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
            application.Selection.TypeText($"Итого: {deliveryRequest.Summ.ToString("C3", CultureInfo.CreateSpecificCulture("ru-RU"))}");
            application.Selection.TypeParagraph();

            application.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            application.Selection.TypeText($"Менеджер отдела продаж ИП \"Блинова Татьяна Валерьевна\"\t");
            application.Selection.Font.Italic = 1;
            application.Selection.TypeText($"{deliveryRequest.User.LastName}\t");
            application.Selection.Font.Italic = 0;
            application.Selection.TypeText(deliveryRequest.User.FullName);


            string desctopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = $"{desctopPath}\\Заявка на поставку товара №{deliveryRequest.ReqNum}.pdf";
            try
            {
                document.SaveAs2(fullPath, Word.WdExportFormat.wdExportFormatPDF);
                if (File.Exists(fullPath))
                {
                    System.Diagnostics.Process.Start(fullPath);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            document.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
            application.Quit();
        }
    }
}
