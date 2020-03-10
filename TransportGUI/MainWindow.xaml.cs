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
using SwissTransport;

namespace TransportGUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ITransport transport;
        public MainWindow()
        {
            InitializeComponent();

            var datetime = DateTime.Today;
            DepartureDate.SelectedDate = DateTime.Today;
            DepartureTime.Text = datetime.ToString("hh:mm");


        }

        private void getConnections(object sender, RoutedEventArgs e)
        {
            string from = txtFrom.Text.ToString();
            string to = txtTo.Text.ToString();
            DateTime selectedDate = (DateTime)DepartureDate.SelectedDate;
            string date = selectedDate.ToString("yyyy-MM-dd");
            string time = DepartureTime.Text.ToString();
            searchConnections(from, to, date, time);
        }

        private void searchConnections(string from, string to, string date, string time)
        {
            //MessageBox.Show(from + to + date + time);
            transport = new Transport();
            var connections = transport.GetConnectionsByDateTime(from, to, date, time);
            ConnectionTable.ItemsSource = connections.ConnectionList;
        }

        private void getTimeTable(object sender, RoutedEventArgs e)
        {
            transport = new Transport();
            var stationBoard = transport.GetStationBoard("Luzern", "8505000");
            StationBoardTable.ItemsSource = stationBoard.Entries;
        }

        private void getStations(object sender, RoutedEventArgs e)
        {
            transport = new Transport();
            if (validateTextfield())
            {
                var searchTerm = txtStationInfo.Text.ToString();
                var stationInfo = transport.GetStations(searchTerm);
                StationInfoTable.ItemsSource = stationInfo.StationList;
            }
        }

        private bool validateTextfield()
        {
            if (txtStationInfo.Text.Length > 0)
            {
                return true;
            }
            return false;
        }
    }
}
