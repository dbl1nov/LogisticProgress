using LogisticProgress.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogisticProgress.Classes
{
    public class ABC_analysis
    {
        public static void GiveABCCategory()
        {

            var products = ProgressLogisticEntities.GetContext().Products.OrderByDescending(s => s.Price * s.RefQty).ThenBy(s => s.Name).ToList();

            decimal cumulativeCost = 0;
            int cumulativeAmount = 0;

            foreach (Product product in products)
            {
                cumulativeCost += product.Summ;
                cumulativeAmount += (int)product.RefQty;

                product.CumulativeCost = cumulativeCost;
                product.CumulativeAmount = cumulativeAmount;
            }

            foreach (Product product in products)
            {
                product.PercentageOfCumAmount = Convert.ToDecimal(product.CumulativeAmount) / cumulativeAmount * 100;
                product.PercentageOfCumCost = product.CumulativeCost / cumulativeCost * 100;

                if(product.PercentageOfCumCost < 80)
                {
                    product.CategoryId = 1;
                }else if(product.PercentageOfCumCost < 95)
                {
                    product.CategoryId = 2;
                }
                else
                {
                    product.CategoryId = 3;
                }
            }

            try
            {
                ProgressLogisticEntities.GetContext().SaveChanges();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public static string NeedRequest()
        {
            var suppliers = ProgressLogisticEntities.GetContext().Suppliers.OrderBy(s=> s.Name).ToList();
            StringBuilder needRequest = new StringBuilder();

            foreach(Supplier supplier in suppliers)
            {
                var products = ProgressLogisticEntities.GetContext().Products.Where(p=>p.SupplierId == supplier.SupplierId).ToList();
                string[] needProduct = new string[products.Count];
                int count = 0;
                foreach (Product product in products)
                {
                    decimal percentCount = Convert.ToDecimal(product.RefQty) / 100 * product.Category.StockPoint;

                    if (product.Qty < percentCount)
                    {
                        needProduct[count] = $"        • {product.Name}\n        В количестве {product.RefQty - product.Qty} шт.";
                        count++;
                    }
                }

                if (count > 0)
                {
                    needRequest.AppendLine($"   {supplier.Name}");
                    foreach (string product in needProduct)
                        needRequest.AppendLine(product);
                }
            }

            return needRequest.ToString();
        }

        public static string InProcess()
        {
            StringBuilder inProcess = new StringBuilder();

            var requests = ProgressLogisticEntities.GetContext().DeliveryRequests.Where(p => p.DateOfDelivery > DateTime.Today).ToList();
            List<DevReq_Prod> products = new List<DevReq_Prod>();

            foreach (DeliveryRequest request in requests)
            {
                var reqProducts = ProgressLogisticEntities.GetContext().DevReq_Prod.Where(p => p.DevReqId == request.DevReqId).ToList();
                products.AddRange(reqProducts);
            }

            string[] inProcessProducts = new string[products.Count];
            int count = 0;

            foreach (DevReq_Prod req_Prod in products)
            {
                inProcessProducts[count] = $"        • {req_Prod.Product.Name}\n        В количестве {req_Prod.Qty} шт.";
                count++;
            }

            if (count > 0)
            {
                foreach (string product in inProcessProducts)
                    inProcess.AppendLine(product);
            }

            return inProcess.ToString();
        }
    }
}
