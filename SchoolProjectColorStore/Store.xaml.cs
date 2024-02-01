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
using System.Windows.Shapes;

namespace SchoolProjectColorStore
{
    /// <summary>
    /// Interaction logic for Store.xaml
    /// </summary>
    public partial class Store : Window
    {
        private string connectionString;
        private List<string> colorList;

        public Store(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            colorList = new List<string>();
        }
        private void Showpaint_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                string query = "SELECT HexCode, Name, Description FROM Color";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> colorList = new List<string>();
                        while (reader.Read())
                        {
                            string hexCode = reader["HexCode"].ToString();
                            string name = reader["Name"].ToString();
                            string description = reader["Description"].ToString();
                            colorList.Add($"HexCode: {hexCode}, Name: {name}, Description: {description}");
                        }
                        colorListBox.ItemsSource = colorList;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void Addpaint_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                string hexCode = Hexcode.Text;
                string colorName = Colorname.Text;
                string description = Description.Text;
                string hexCodeCheckQuery = $"SELECT COUNT(*) FROM Color WHERE HexCode = '{hexCode}'";
                using (MySqlCommand hexCodeCheckCommand = new MySqlCommand(hexCodeCheckQuery, connection))
                {
                    long count = (long)hexCodeCheckCommand.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Error: HexCode already exists.");
                        return;
                    }
                }
                string insertColorQuery = $"INSERT INTO Color (HexCode, Name, Description) VALUES ('{hexCode}', '{colorName}', '{description}')";
                using (MySqlCommand insertColorCommand = new MySqlCommand(insertColorQuery, connection))
                {
                    insertColorCommand.ExecuteNonQuery();
                }
                string insertPaintQuery = $"INSERT INTO Paint (HexCode, OrderInPaint, WebstoreID) VALUES ('{hexCode}', 1, 1)";
                using (MySqlCommand insertPaintCommand = new MySqlCommand(insertPaintQuery, connection))
                {
                    insertPaintCommand.ExecuteNonQuery();
                }
                Showpaint_Click(sender, e);
                Hexcode.Text = "";
                Colorname.Text = "";
                Description.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private string GenerateRandomHexCode()
        {
            Random random = new Random();
            return $"#{random.Next(0x1000000):X6}";
        }
        private void Gethexcode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hexcode.Text = GenerateRandomHexCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (colorListBox.SelectedItem != null)
            {
                string selectedHexCode = GetHexCodeFromColorInfo(colorListBox.SelectedItem.ToString());
                RemoveColorFromDatabase(selectedHexCode);
                if (colorList != null)
                {
                    colorList.Remove(colorListBox.SelectedItem.ToString());
                    colorListBox.ItemsSource = null;
                    colorListBox.ItemsSource = colorList;
                }
                Showpaint_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Select a color");
            }
        }
        private void RemoveColorFromDatabase(string hexCode)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                string deleteQuery = $"DELETE FROM Color WHERE HexCode = '{hexCode}'";
                using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                {
                    deleteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting color from database: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private string GetHexCodeFromColorInfo(string colorInfo)
        {
            int startIndex = colorInfo.IndexOf("#");
            int endIndex = colorInfo.IndexOf(",", startIndex);

            if (startIndex != -1 && endIndex != -1)
            {
                return colorInfo.Substring(startIndex, endIndex - startIndex).Trim();
            }
            return null;
        }
    }
}
