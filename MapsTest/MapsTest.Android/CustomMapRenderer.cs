using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using MapsTest;
using MapsTest.Droid;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace MapsTest.Droid
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        List<CustomPin> customPins;

        public CustomMapRenderer(Context c) : base(c) {}

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            // calls the MapView.GetMapAsync method, which gets the underlying GoogleMap that is tied to the view.
            // Once the GoogleMap instance is available, the OnMapReady override will be invoked.
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
            }
            
        }

        /// <summary>
        /// This method registers an event handler for the InfoWindowClick event,
        /// which fires when the info window is clicked, and is unsubscribed from
        /// only when the element the renderer is attached to changes (stop memory leak).
        /// </summary>
        /// <param name="map"></param>
        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.InfoWindowClick += OnInfoWindowClick;
            // specify that the CustomMapRenderer class will provide the methods to customize the info window
            NativeMap.SetInfoWindowAdapter(this);

            // Add a marker that has the info window showing by default
            //var marker = new MarkerOptions();
            //marker.SetPosition(new LatLng(30.274054, -97.749074));
            //marker.SetTitle("TEST");
            //marker.SetSnippet("TEST");
            //marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueOrange));
            //
            //var m = NativeMap.AddMarker(marker);
            //m.ShowInfoWindow();
        }

        // We implement the GoogleMap.IInfoWindowAdapter in order to customize the info winow.
        // this interface specifies we need these two methods:

        // only the window content is customized and so the GetInfoWindow method returns null
        public Android.Views.View GetInfoWindow(Marker marker) => null;

        /// <summary>
        /// This method returns a View containing the contents of the info window. This is accomplished as follows:
        /// - A LayoutInflater instance is retrieved. This is used to instantiate a layout XML file into its corresponding View.
        /// - The GetCustomPin method is called to return the custom pin data for the info window.
        /// - The XamarinMapInfoWindow layout is inflated if the CustomPin.Id property equals Xamarin. Otherwise,
        /// the MapInfoWindow layout is inflated. This allows for scenarios where different info window layouts can
        /// be displayed for different markers.
        /// - The InfoWindowTitle and InfoWindowSubtitle resources are retrieved from the inflated layout, and
        /// their Text properties are set the corresponding data from the Marker instance, provided that the resources
        /// are not null.
        /// - The View instance is returned for display on the map.
        /// 
        /// An info window is not a live View. Instead, Android will convert the View to a static bitmap 
        /// and display that as an image. This means that while an info window can respond to a 
        /// click event, it cannot respond to any touch events or gestures, and the individual 
        /// controls in the info window cannot respond to their own click events.
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                if (customPin.Id == "Xamarin")
                {
                    view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);
                }
                else
                {
                    view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
                }

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = marker.Title;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = marker.Snippet;
                }

                return view;
            }

            return null;
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            CustomPin customPin = GetCustomPin(pin);

            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);

            marker.SetIcon(BitmapDescriptorFactory.FromAsset($"m{customPin.NumMoments}.png"));
            //marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueMagenta));//.FromResource(Resource.Drawable.pin));
            return marker;
        }

        /// <summary>
        /// When the user clicks on the info window, the InfoWindowClick event fires, 
        /// which in turn executes the OnInfoWindowClick method
        /// 
        /// This method opens a web browser and navigates to the address stored in 
        /// the Url property of the retrieved CustomPin instance for the Marker. 
        /// Note that the address was defined when creating the CustomPin collection in the PCL project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnInfoWindowClick (object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            if (!string.IsNullOrWhiteSpace (customPin.Url))
            {
                var url = Android.Net.Uri.Parse(customPin.Url);
                var intent = new Intent(Intent.ActionView, url);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }

        CustomPin GetCustomPin(Pin pin)
        {
            return customPins.Where(x => x.Position.Latitude == pin.Position.Latitude && x.Position.Longitude == pin.Position.Longitude).FirstOrDefault();
        }
    }
}