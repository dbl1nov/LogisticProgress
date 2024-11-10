using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace LogisticProgress.Classes
{
    public class Validator
    {
        public static bool CheckNumbers(string number)
        {
            bool valid = true;
            if (number != null)
            {
                foreach (char c in number)
                {
                    if (Regex.IsMatch(c.ToString(), @"[0-9]"))
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = false;
                        break;
                    }
                }
            }
            return valid;
        }

        public static bool CheckPrice(string price)
        {
            bool valid = true;
            if (price != null)
            {
                foreach (char c in price)
                {
                    if (Regex.IsMatch(c.ToString(), @"[0-9 .]"))
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = false;
                        break;
                    }
                }
            }
            return valid;
        }
    }
}
