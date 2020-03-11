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
using System.Text.RegularExpressions;

namespace TransportGUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public class DateConverter
    {
        public object Convert(object value)
        {
            DateTime date = (DateTime)value;
            return date.ToShortDateString();
        }
    }

    public partial class MainWindow : Window
    {


        private ITransport transport;

        public MainWindow()
        {
            InitializeComponent();
            var datetime = DateTime.Now;
            DepartureDate.SelectedDate = DateTime.Now;
            txtDepartureTime.Text = datetime.ToString("HH:mm");
        }

        private void btnConSearch_Clicked(object sender, RoutedEventArgs e)
        {
            string from = txtFrom.Text.ToString();
            string to = txtTo.Text.ToString();
            DateTime selectedDate = (DateTime)DepartureDate.SelectedDate;
            string date = selectedDate.ToString("yyyy-MM-dd");
            string time = txtDepartureTime.Text.ToString();

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
            string searchTerm = txtStation.Text.ToString();
            var stationBoard = transport.GetStationBoard(searchTerm);
            StationBoardTable.ItemsSource = stationBoard.Entries;
        }


        private void btnStationInfo_Clicked(object sender, RoutedEventArgs e)
        {
            if (validateTextField())
            {
                string searchTerm = txtStationInfo.Text.ToString();
                Stations stations = getStations(searchTerm);
                if (stations.StationList.Count > 0)
                {
                    StationInfoTable.ItemsSource = stations.StationList;
                }
            }
        }

        private Stations getStations(string searchTerm)
        {
            transport = new Transport();
            {
                Stations stations = transport.GetStations(searchTerm);
                return stations;
            }
        }

        private bool validateTextField()
        {
            if (txtStationInfo.Text.Length > 0)
            {
                return true;
            }
            return false;
        }

        private void validateTimeField(string time)
        {
            if (time.Length == 1)
            {
                    time = "0" + time + ":00";
            }
            else if (time.Length == 2)
            {
                time = time+":00";
            }
            else if (time.Length == 3)
            {
                time = "0" + time;
                time = time.Insert(2, ":");
            }
            else
            {
                time = time.Insert(2, ":");
            }
            Regex regex = new Regex("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
            if (!regex.IsMatch(time))
            {
                txtDepartureTime.Text = DateTime.Now.ToString("HH:mm");
            } 
            else
            {
                txtDepartureTime.Text = time;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtDepartureTime_LostFocus(object sender, RoutedEventArgs e)
        {
            string time = txtDepartureTime.Text.ToString();
            validateTimeField(time);
        }

        private void selectTime(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("FOCUSED");
            TextBox tb = (sender as TextBox);
            tb.SelectAll();
        }

        private void getStationListSelect(object sender, TextChangedEventArgs e)
        {
            Stations stations;
            ComboBox cb = (sender as ComboBox);
            string cbText = cb.Text;
            if (cbText.Length > 2 && cb.Items.Count == 0)
            {
                cb.Items.Clear();
                stations = getStations(cbText);
                if (stations.StationList.Count > 0)
                {
                    Console.WriteLine(stations.StationList.Count);
                    foreach (Station s in stations.StationList)
                    {
                        cb.Items.Add(s.Name);
                    }
                    cb.IsDropDownOpen = true;        
                    var tb = (cb.Template.FindName("PART_EditableTextBox", cb) as TextBox);
                    if (tb != null)
                    {
                        tb.Focus();
                        tb.SelectionStart = tb.Text.Length;
                    }
                }
            }
            else if(cbText.Length < 3)
            {
                cb.Items.Clear();
                cb.IsDropDownOpen = false;
            }
        }

        private void cmbAutofillStation_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            if (!cb.Items.Contains(cb.Text) && cb.Items.Count>0)
            {
                cb.Text = cb.Items.GetItemAt(0).ToString();
            }
        }
    }
}
