using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticProgress.DataBase
{
    public partial class User
    {
        public string FullName
        {
            get
            {
                var fullName = $"{LastName} {FirstName}";

                if (string.IsNullOrWhiteSpace(Patronymic) == false)
                {
                    fullName += $" {Patronymic}";
                }
                return fullName;
            }
        }

    }
}
