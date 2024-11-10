using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticProgress.DataBase
{
    public partial class PurchaceInvoce
    {
        public decimal Summ
        {
            get
            {
                decimal summ = 0;
                foreach(PurInv_Prod purInv_Prod in PurInv_Prod)
                {
                    summ += purInv_Prod.Price * purInv_Prod.Qty;
                }
                return summ;
            }
        }
    }
}
