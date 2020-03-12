using Microsoft.Maps.MapControl.WPF;
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
using System.Windows.Shapes;
using System.Device.Location;
using SwissTransport;

namespace TransportGUI
{
    /// <summary>
    /// Interaktionslogik für MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        private ITransport transport;

        // Constructor for MapWindow if User wants to see his position
        public MapWindow()
        {
            InitializeComponent();
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();

            // Do not suppress prompt, and wait 1000 milliseconds to start.
            watcher.TryStart(false, TimeSpan.FromMilliseconds(3000));

            GeoCoordinate coord = watcher.Position.Location;

            if (coord.IsUnknown != true)
            {
                Console.WriteLine("Lat: {0}, Long: {1}",
                    coord.Latitude,
                    coord.Longitude);
            }
            else
            {
                MessageBox.Show("Position konnte nicht ermittelt werden, Standort auf Bahnhof Luzern gesetzt.", "Fehler");
                coord.Latitude = 47.050174;
                coord.Longitude = 8.31024;
            }
            transport = new Transport();
            try
            {
                Stations stations = transport.GetStationsByCoordinates(coord.Latitude.ToString().Replace(",", "."), coord.Longitude.ToString().Replace(",", "."));
                Location initLocation = new Location(coord.Latitude, coord.Longitude);
                myMap.SetView(initLocation, 15);
                foreach (Station s in stations.StationList)
                {
                    Console.WriteLine(s.Distance);
                    Location loc = new Location(s.Coordinate.XCoordinate, s.Coordinate.YCoordinate);
                    Pushpin pushpin = new Pushpin();
                    pushpin.Location = loc;
                    pushpin.ToolTip = s.Name + " " + s.Distance + " m";
                    myMap.Children.Add(pushpin);
                }
            }
            catch
            {
                MessageBox.Show("Keine Verbindung zum Internet.", "Fehler");
            }
        }

        // Constructor for MapWindow if User wants to see the position of a station
        public MapWindow(double xCoord, double yCoord)
        {
            InitializeComponent();
            Location initLocation = new Location(xCoord, yCoord);
            myMap.SetView(initLocation, 15);
            Pushpin pushpin = new Pushpin();
            pushpin.Location = initLocation;
            myMap.Children.Add(pushpin);
        }
    }
}
