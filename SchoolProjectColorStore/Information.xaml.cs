using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Xml;

namespace SchoolProjectColorStore
{
    /// <summary>
    /// Interaction logic for Information.xaml
    /// </summary>
    public partial class Information : Window
    {

        private string connectionString;

        public Information(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            Loaded += Information_Loaded;
        }
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            nameTextBox.Visibility = Visibility.Visible;
            urlTextBox.Visibility = Visibility.Visible;
            locationTextBox.Visibility = Visibility.Visible;
            contactDetailsTextBox.Visibility = Visibility.Visible;
            nameLabel.Visibility = Visibility.Visible;
            urlLabel.Visibility = Visibility.Visible;
            locationLabel.Visibility = Visibility.Visible;
            contactDetailsLabel.Visibility = Visibility.Visible;
            updateButton.Visibility = Visibility.Visible;
        }

        private void Information_Loaded(object sender, RoutedEventArgs e)
        {
            Information_show(sender, e);
        }

        private void Information_show(object sender, RoutedEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                string query = "SELECT * FROM Webstore";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    Console.WriteLine("Executing SQL Query: " + query);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        StringBuilder resultStringBuilder = new StringBuilder();

                        while (reader.Read())
                        {
                            string webstoreID = reader["WebstoreID"].ToString();
                            string name = reader["Name"].ToString();
                            string url = reader["URL"].ToString();
                            string location = reader["Location"].ToString();
                            string contactDetails = reader["ContactDetails"].ToString();

                            resultStringBuilder.AppendLine($"WebstoreID: {webstoreID}, Name: {name}, URL: {url}, Location: {location}, ContactDetails: {contactDetails}");
                        }
                        informationresult.Text = resultStringBuilder.ToString();
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
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                string updateQuery = "UPDATE Webstore SET Name = @Name, URL = @URL, Location = @Location, ContactDetails = @ContactDetails WHERE WebstoreID = @WebstoreID";
                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Name", nameTextBox.Text);
                    updateCommand.Parameters.AddWithValue("@URL", urlTextBox.Text);
                    updateCommand.Parameters.AddWithValue("@Location", locationTextBox.Text);
                    updateCommand.Parameters.AddWithValue("@ContactDetails", contactDetailsTextBox.Text);
                    updateCommand.Parameters.AddWithValue("@WebstoreID", 1);
                    Console.WriteLine("Executing SQL Query: " + updateQuery);
                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Update successful!");
                        informationresult.Text = $"Name: {nameTextBox.Text}, URL: {urlTextBox.Text}, Location: {locationTextBox.Text}, ContactDetails: {contactDetailsTextBox.Text}";
                    }
                    else
                    {
                        MessageBox.Show("Update failed. No matching record found.");
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

    }
}

