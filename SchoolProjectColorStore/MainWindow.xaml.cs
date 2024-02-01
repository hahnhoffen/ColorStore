using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace SchoolProjectColorStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Databaseconnection dbConnection = new Databaseconnection("localhost", "SCHOOL_STORE", "root", // "DittLösenord");
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Information_button(object sender, RoutedEventArgs e)
        {
            Information informationWindow = new Information(dbConnection.GetConnectionString());
            informationWindow.Show();
        }

        private void Store_button(object sender, RoutedEventArgs e)
        {
            Store storeWindow = new Store(dbConnection.GetConnectionString());
            storeWindow.Show();
        }

        private void Users_button(object sender, RoutedEventArgs e)
        {
            // har inte börjat med users än, det är för ifall jag vill vidaruteckla programmet
        }

        private void Orders_button(object sender, RoutedEventArgs e)
        {
            Orders orderWindow = new Orders(dbConnection.GetConnectionString());
            orderWindow.Show();
        }

        private void Search_click(object sender, RoutedEventArgs e)
        {
            SearchWIndow searchWindow = new SearchWIndow(dbConnection.GetConnectionString());
            searchWindow.Show();
        }
    }
}
