using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MapsTest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            myMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(30.274054, -97.744074), Distance.FromMiles(1)));

            var pin = new CustomPin()
            {
                Position = new Position(30.274054, -97.744074),
                Label = "Hello!",
                Type = PinType.Place,
                Address = "TEST",
                Id = "Xamarin",
                Url = "http://xamarin.com/about/"
            };

            myMap.CustomPins.Add(pin);
            myMap.Pins.Add(pin);

            pin = new CustomPin()
            {
                Position = new Position(30.274054, -97.749074),
                Label = "Pin2",
                Type = PinType.Place,
                NumMoments = 2
            };

            myMap.CustomPins.Add(pin);
            myMap.Pins.Add(pin);

        }
    }
}
