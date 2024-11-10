using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticProgress.DataBase
{
    public partial class DeliveryRequest
    {
        public decimal Summ
        {
            get
            {
                decimal summ = 0;
                foreach (DevReq_Prod devReq_Prod in DevReq_Prod)
                {
                    summ += devReq_Prod.Price * devReq_Prod.Qty;
                }
                return summ;
            }
        }
    }
}
