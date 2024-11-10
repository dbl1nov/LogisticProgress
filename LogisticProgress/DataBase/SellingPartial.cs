using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticProgress.DataBase
{
    public partial class Selling
    {
        public decimal Summ
        {
            get
            {
                decimal summ = 0;
                foreach (Selling_Prod selling_Prod in Selling_Prod)
                {
                    summ += selling_Prod.Price * selling_Prod.Qty;
                }
                return summ;
            }
        }
    }
}
