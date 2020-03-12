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
using System.Text.RegularExpressions;
using SwissTransport;
using System.Diagnostics;

namespace TransportGUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        // Variable to initialize Transport Class
        private ITransport transport;

        // Constructor of MainWindow, gets the actual date and time for default values of connection search.
        public MainWindow()
        {
            InitializeComponent();
            var datetime = DateTime.Now;
            DepartureDate.SelectedDate = DateTime.Now;
            txtDepartureTime.Text = datetime.ToString("HH:mm");
        }

        // If btnConSearch gets clicked it will search for connections
        private void btnConSearch_Clicked(object sender, RoutedEventArgs e)
        {
            searchConnections();
        }

        // Calls with an instance of transport class the api for connections and displays them.
        private void searchConnections()
        {
            transport = new Transport();
            string from = txtFrom.Text.ToString();
            string to = txtTo.Text.ToString();
            DateTime selectedDate = (DateTime)DepartureDate.SelectedDate;
            string date = selectedDate.ToString("yyyy-MM-dd");
            string time = txtDepartureTime.Text.ToString();
            if(txtFrom.Text.Length > 0 && txtTo.Text.Length > 0)
            {
                try
                {
                    var connections = transport.GetConnectionsByDateTime(from, to, date, time);
                    foreach (Connection c in connections.ConnectionList)
                    {
                        DateTime departureTime = DateTime.Parse(c.From.Departure);
                        c.From.Departure = departureTime.ToString("HH:mm");
                    }
                    ConnectionTable.ItemsSource = connections.ConnectionList;
                }
                catch
                {
                    MessageBox.Show("Keine Verbindung zum Internet.", "Fehler");
                }
            }      
        }

        // if btnStationInfo gets clicked it will search for stations with function getStations() and displays them
        private void btnStationInfo_Clicked(object sender, RoutedEventArgs e)
        {
            if (validateTextField())
            {
                string searchTerm = txtStationInfo.Text.ToString();
                try
                {
                    Stations stations = getStations(searchTerm);
                    if (stations.StationList.Count > 0)
                    {
                        StationInfoTable.ItemsSource = stations.StationList;
                    }
                }
                catch
                {
                    MessageBox.Show("Keine Verbindung zum Internet.", "Fehler");
                }
            }
        }

        // If bstStationBoard is clicked and txtStation is not empty call getTimeTable().
        private void btnStationBoard_Clicked(object sender, RoutedEventArgs e)
        {
            string searchTerm = txtStation.Text.ToString();
            if (searchTerm.Length > 0)
            {
                getTimeTable(searchTerm);
            }
        }

        // Gets the Data for StationBoardTable from api.
        private void getTimeTable(string searchTerm)
        {
            transport = new Transport();
            try
            {
                var stationBoard = transport.GetStationBoard(searchTerm);
                StationBoardTable.ItemsSource = stationBoard.Entries;
            }
            catch
            {
                MessageBox.Show("Keine Verbindung zum Internet.", "Fehler");
            }
        }

        // Gets the stations from api.
        private Stations getStations(string searchTerm)
        {
            transport = new Transport();
            {
                Stations stations = transport.GetStations(searchTerm);
                return stations;
            }
        }

        // validates the textfield, should not be empty
        private bool validateTextField()
        {
            if (txtStationInfo.Text.Length > 0)
            {
                return true;
            }
            return false;
        }

        // Converts a string to a valid time in format HH:mm.
        private void validateTimeField(string time)
        {
            if (time.Length == 1)
            {
                time = "0" + time + ":00";
            }
            else if (time.Length == 2)
            {
                time = time + ":00";
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

        // Checks if input is a number.
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // If txtDepartureTime loses focus it will check the text.
        private void txtDepartureTime_LostFocus(object sender, RoutedEventArgs e)
        {
            string time = txtDepartureTime.Text.ToString();
            validateTimeField(time);
        }

        // Selects text in textfield.
        private void selectTime(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            tb.SelectAll();
        }

        // Gets stations for the ComboBox if more then three chars are typed.
        private void getStationListSelect(object sender, TextChangedEventArgs e)
        {
            Stations stations;
            ComboBox cb = (sender as ComboBox);
            string cbText = cb.Text;
            if (cbText.Length > 2 && cb.Items.Count == 0)
            {
                cb.Items.Clear();
                try
                {
                    stations = getStations(cbText);
                    if (stations.StationList.Count > 0)
                    {
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
                catch
                {
                    MessageBox.Show("Keine Verbindung zum Internet.", "Fehler");
                }
            }
            else if (cbText.Length < 3)
            {
                cb.Items.Clear();
                cb.IsDropDownOpen = false;
            }
        }

        // If ComboBox loses focus it will take the first item if items exist.
        private void cmbAutofillStation_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cb = (sender as ComboBox);
            if (!cb.Items.Contains(cb.Text) && cb.Items.Count > 0)
            {
                cb.Text = cb.Items.GetItemAt(0).ToString();
            }
        }

        // If btnOpenMap gets clicked open new window with map.
        private void btnOpenMap_Click(object sender, RoutedEventArgs e)
        {
            var tag = ((Button)sender).Tag;
            Station station = (Station)tag;
            MapWindow map = new MapWindow(station.Coordinate.XCoordinate, station.Coordinate.YCoordinate);
            map.ShowDialog();
        }

        // If btnOpenMapNear gets clicked open new window with map.
        private void btnOpenMapNear_Click(object sender, RoutedEventArgs e)
        {
            MapWindow map = new MapWindow();
            map.ShowDialog();
        }

        // Set focus in info tab when selection changed.
        private void infoTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConnectionTab.IsSelected)
            {
                ComboBox cb = (txtFrom as ComboBox);
                var tb = (cb.Template.FindName("PART_EditableTextBox", cb) as TextBox);
                if (tb != null)
                {
                    tb.Focus();
                }
            }
        }

        // Set focus in info tab when loaded.
        private void infoTab_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cb = (txtFrom as ComboBox);
            var tb = (cb.Template.FindName("PART_EditableTextBox", cb) as TextBox);
            if (tb != null)
            {
                tb.Focus();
            }
        }

        // Sets body and subject for mail and opens default mail app.
        private void btnMail_Clicked(object sender, RoutedEventArgs e)
        {
            if (ConnectionTab.IsSelected)
            {
                if (ConnectionTable.Items.Count == 0)
                {
                    MessageBox.Show("Keine Ergebnise zu versenden", "Fehler");
                }
                else
                {
                    string body = "Von" + "%09%09" + "Nach" + "%09" + "Ab" + "%09" + "Dauer" + "%09%09" + "Gleis" + "%0A";
                    foreach (Connection cn in ConnectionTable.Items)
                    {
                        body += cn.From.Station.Name + "%09" + cn.To.Station.Name + "%09" + cn.From.Departure + "%09" + cn.Duration + "%09" + cn.From.Platform + "%0A";
                    }
                    string subject = "Verbindungen von " + txtFrom.Text + " nach" + txtTo.Text;
                    Process.Start("mailto:?subject=" + subject + "&body=" + body);
                }
            }
            if (StationBoardTab.IsSelected)
            {
                if (StationBoardTable.Items.Count == 0)
                {
                    MessageBox.Show("Keine Ergebnise zu versenden", "Fehler");
                }
                else
                {
                    string body = "Fahrt" + "%09" + "Nach" + "%09" + "Ab" + "%09" + "Gleis" + "%0A";
                    foreach (StationBoard sb in StationBoardTable.Items)
                    {
                        body += sb.Name + "%09" + sb.To + "%09" + sb.Stop.Departure + "%09" + sb.Stop.Platform + "%0A";
                    }
                    string subject = "Abfahrtstafel " + txtStation.Text;
                    Process.Start("mailto:?subject=" + subject + "&body=" + body);
                }
            }
            if (StationInfoTab.IsSelected)
            {
                if (StationInfoTable.Items.Count == 0)
                {
                    MessageBox.Show("Keine Ergebnise zu versenden", "Fehler");
                }
                else
                {
                    string body = "Name" + "%09" + "X-Koordinate" + "%09" + "Y-Koordinate" + "%0A";
                    foreach (Station st in StationInfoTable.Items)
                    {
                        body += st.Name + "%09" + st.Coordinate.XCoordinate + "%09" + st.Coordinate.YCoordinate + "%0A";
                    }
                    string subject = "Stationen ";
                    Process.Start("mailto:?subject=" + subject + "&body=" + body);
                }
            }
        }

        // If enter is pressed call search function of tab.
        private void doAction_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                if (ConnectionTab.IsSelected)
                {
                    if(txtFrom.Text.Length > 0 && txtTo.Text.Length > 0)
                    {
                        searchConnections();
                    }
                }
                if (StationBoardTab.IsSelected)
                {
                    if (txtStation.Text.Length>0)
                        getTimeTable(txtStation.Text);
                }
                if (StationInfoTab.IsSelected)
                {
                    if(txtStationInfo.Text.Length>0)
                    {
                        getStations(txtStationInfo.Text);
                    }
                }
            }
        }
    }
}
