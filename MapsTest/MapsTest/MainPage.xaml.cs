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
                NumMoments = 1,
                Url = "http://xamarin.com/about/"
            };

            myMap.CustomPins.Add(pin);
            myMap.Pins.Add(pin);

            pin = new CustomPin()
            {
                Position = new Position(30.274054, -97.749074),
                Label = "Pin2",
                Type = PinType.Place,
                NumMoments = 2,
                Id = "SomeId",
                Url = "http://pronome.net"
            };

            myMap.CustomPins.Add(pin);
            myMap.Pins.Add(pin);

        }

        CustomPin _eventPin;

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (_eventPin == null)
            {
                _eventPin = new CustomPin()
				{
					Position = new Position(30.274054, -97.740074),
					Label = "pin3",
                    Type = PinType.Place,
					NumMoments = 1,
					Id = "Xamarin2"
				};
				
                myMap.CustomPins.Add(_eventPin);
                myMap.Pins.Add(_eventPin);
            }
            else
            {
                myMap.Pins.Remove(_eventPin);
                _eventPin.NumMoments++;
                myMap.Pins.Add(_eventPin);
            }
        }
    }
}
