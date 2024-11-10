using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticProgress.DataBase
{
    public partial class Selling_Prod
    {
        public decimal Summ
        {
            get
            {
                var summ = Qty * Price;
                return summ;
            }
        }
    }
}
