using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Orders.xaml
    /// </summary>
    public partial class Orders : Window
    {
        private string connectionString;
        public Orders(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
        }

        private class Order
        {
            public int OrderID { get; set; }
            public int CustomerID { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; }

            public override string ToString()
            {
                return $"OrderID: {OrderID}, CustomerID: {CustomerID}, OrderDate: {OrderDate}, TotalAmount: {TotalAmount}, Status: {Status}";
            }
        }
        private List<Order> ordersList = new List<Order>();
        private void Orders_click(object sender, RoutedEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                string orderQuery = "SELECT OrderID, CustomerID, OrderDate, TotalAmount, Status FROM Orders";
                using (MySqlCommand orderCommand = new MySqlCommand(orderQuery, connection))
                {
                    using (MySqlDataReader orderReader = orderCommand.ExecuteReader())
                    {
                        ordersList.Clear();
                        while (orderReader.Read())
                        {
                            Order order = new Order
                            {
                                OrderID = orderReader.GetInt32("OrderID"),
                                CustomerID = orderReader.GetInt32("CustomerID"),
                                OrderDate = orderReader.GetDateTime("OrderDate"),
                                TotalAmount = orderReader.GetDecimal("TotalAmount"),
                                Status = orderReader.GetString("Status")
                            };
                            ordersList.Add(order);
                        }
                        ordersListBox.ItemsSource = ordersList.Select(o => o.ToString()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching orders: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void DeleteOrder(int orderID)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Orders WHERE OrderID = @OrderID";
                using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@OrderID", orderID);
                    deleteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting order: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ordersListBox.SelectedItem != null && ordersListBox.SelectedItem is string selectedOrder)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this order?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Match match = Regex.Match(selectedOrder, @"OrderID: (\d+),.*");
                    if (match.Success)
                    {
                        int orderID = int.Parse(match.Groups[1].Value);
                        Order orderToRemove = ordersList.Find(order => order.OrderID == orderID);
                        ordersList.Remove(orderToRemove);
                        ordersListBox.ItemsSource = ordersList.Select(o => o.ToString()).ToList();
                        DeleteOrder(orderID);
                    }
                    else
                    {
                        MessageBox.Show("Unable to extract order details for deletion.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an order to delete.");
            }
        }
        private void ShowOrders()
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                string ordersQuery = @"
            SELECT
                OrderID,
                OrderDate,
                TotalAmount,
                Status
            FROM
                Orders;
        ";

                using (MySqlCommand ordersCommand = new MySqlCommand(ordersQuery, connection))
                {
                    using (MySqlDataReader ordersReader = ordersCommand.ExecuteReader())
                    {
                        List<string> orderList = new List<string>();
                        while (ordersReader.Read())
                        {
                            int orderID = ordersReader.GetInt32("OrderID");
                            DateTime orderDate = ordersReader.GetDateTime("OrderDate");
                            decimal totalAmount = ordersReader.GetDecimal("TotalAmount");
                            string status = ordersReader.GetString("Status");
                            orderList.Add($"OrderID: {orderID}, OrderDate: {orderDate}, TotalAmount: {totalAmount}, Status: {status}");
                        }
                        ordersListBox.ItemsSource = orderList;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching orders: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        private void Change_click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(OrderIDtext.Text, out int orderID))
            {
                if (decimal.TryParse(Totalamounttext.Text, out decimal updatedTotalAmount))
                {
                    string updatedOrderDate = Orderdatetext.Text;
                    string updatedStatus = Statustext.Text;
                    int index = -1;
                    for (int i = 0; i < ordersListBox.Items.Count; i++)
                    {
                        if (ordersListBox.Items[i] is string orderInfo && orderInfo.Contains($"OrderID: {orderID}"))
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index != -1)
                    {
                        Match match = Regex.Match((string)ordersListBox.Items[index], @"CustomerID: (?<CustomerID>\d+),.*");
                        if (match.Success)
                        {
                            string existingCustomerID = match.Groups["CustomerID"].Value;
                            ((List<string>)ordersListBox.ItemsSource)[index] = $"OrderID: {orderID}, CustomerID: {existingCustomerID}, OrderDate: {updatedOrderDate}, TotalAmount: {updatedTotalAmount}, Status: {updatedStatus}";
                            ordersListBox.Items.Refresh();
                            UpdateOrder(orderID, updatedOrderDate, updatedTotalAmount, updatedStatus);
                        }
                        else
                        {
                            MessageBox.Show($"Unable to find existing CustomerID for OrderID {orderID}.");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Order with OrderID {orderID} not found.");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid Total Amount (a decimal number) in Totalamounttext.");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid OrderID (a number) in OrderIDtext.");
            }
        }
        private void UpdateOrder(int orderID, string newOrderDate, decimal newTotalAmount, string newStatus)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                string updateQuery = @"
                                    UPDATE Orders
                                    SET OrderDate = @NewOrderDate,
                                    TotalAmount = @NewTotalAmount,
                                    Status = @NewStatus
                                    WHERE OrderID = @OrderID;";
                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@NewOrderDate", newOrderDate);
                    updateCommand.Parameters.AddWithValue("@NewTotalAmount", newTotalAmount);
                    updateCommand.Parameters.AddWithValue("@NewStatus", newStatus);
                    updateCommand.Parameters.AddWithValue("@OrderID", orderID);

                    updateCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating order: " + ex.Message);
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
