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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace SchoolProjectColorStore
{
    /// <summary>
    /// Interaction logic for SearchWIndow.xaml
    /// </summary>
    public partial class SearchWIndow : Window
    {
        private string connectionString;

        public SearchWIndow(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
        }

        private void SearchText_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchText.Text = string.Empty;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }
        private void PerformSearch()
        {
            string searchKeyword = SearchText.Text.Trim();
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                try
                {
                    connection.Open();
                    string searchQuery = @"
                SELECT
                    HexCode,
                    Name,
                    Description
                FROM
                    Color
                WHERE
                    Name LIKE @SearchKeyword;
            ";

                    using (MySqlCommand searchCommand = new MySqlCommand(searchQuery, connection))
                    {
                        searchCommand.Parameters.AddWithValue("@SearchKeyword", $"%{searchKeyword}%");
                        using (MySqlDataReader searchReader = searchCommand.ExecuteReader())
                        {
                            List<string> searchResults = new List<string>();
                            while (searchReader.Read())
                            {
                                string hexCode = searchReader.GetString("HexCode");
                                string colorName = searchReader.GetString("Name");
                                string description = searchReader.GetString("Description");
                                searchResults.Add($"HexCode: {hexCode}, Name: {colorName}, Description: {description}");
                            }
                            searchListBox.ItemsSource = searchResults;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error searching colors: " + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a search keyword.");
            }
        }
    }
}
