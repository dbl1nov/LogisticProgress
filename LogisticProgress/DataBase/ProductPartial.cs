using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticProgress.DataBase
{
    public partial class Product
    {
        public decimal Summ
        {
            get
            {
                int summ = (int)RefQty * Price;
                return summ;

            }
        }
        public decimal CumulativeCost { get; set; }
        public int CumulativeAmount { get; set; }
        public decimal PercentageOfCumCost { get; set; }
        public decimal PercentageOfCumAmount { get; set; }
    }
}
