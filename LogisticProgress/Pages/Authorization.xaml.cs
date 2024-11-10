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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticProgress.Pages
{
    /// <summary>
    /// Interaction logic for Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(tbLogin.Text))
                errors.AppendLine("Введите логин!");
            if (string.IsNullOrWhiteSpace(tbPassword.Text) && string.IsNullOrWhiteSpace(pbPassword.Password))
                errors.AppendLine("Введите пароль!");

            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            var user = ProgressLogisticEntities.GetContext().Users.Where(p=>p.Login == tbLogin.Text).ToList();
            if(user.Count == 0 || (user[0].Password != pbPassword.Password && user[0].Password != tbPassword.Text))
            {
                MessageBox.Show("Неправильный логин или пароль!");
                return;
            }

            Classes.Manager.UserId = user[0].UserId;

            if(user[0].Role == "m")
            {
                Classes.Manager.MainFrame.Navigate(new ManagerMenu());
            }
            else if (user[0].Role == "s")
            {
                Classes.Manager.MainFrame.Navigate(new Seller(null));
                Classes.Manager.ButtonBackVisibility = false;
            }
        }

        private void chbShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if(chbShowPassword.IsChecked == true)
            {
                tbPassword.Text = pbPassword.Password;
                tbPassword.Visibility = Visibility.Visible;
                pbPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                pbPassword.Password = tbPassword.Text;
                tbPassword.Visibility = Visibility.Collapsed;
                pbPassword.Visibility = Visibility.Visible;
            }
        }
    }
}
