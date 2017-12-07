using System;
using System.Collections.Generic;
using MapsTest;
using Xamarin.Forms;
using MapsTest.iOS;
using UIKit;
using CoreGraphics;
using MapKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace MapsTest.iOS
{
    public class CustomMapRenderer : MapRenderer
    {
        UIView customPinView;
        List<CustomPin> customPins;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.GetViewForAnnotation = null;
                //nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                customPins = formsMap.CustomPins;

                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                //nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
            }
        }

        /// <summary>
        /// This method is called when the annotation(pin) location becoms visible on the map
        /// </summary>
        /// <returns>The view for annotation.</returns>
        /// <param name="mapView">Map view.</param>
        /// <param name="annotation">Annotation.</param>
        MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;

            if (annotation is MKUserLocation)
            {
                return null;
            }

            var customPin = GetCustomPin(annotation as MKPointAnnotation);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            annotationView = mapView.DequeueReusableAnnotation(customPin.Id);
            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, customPin);
				annotationView.Image = UIImage.FromFile($"m{customPin.NumMoments.ToString()}.png");
                annotationView.CalloutOffset = new CGPoint(0, 0);
                ((CustomMKAnnotationView)annotationView).Id = customPin.Id;
                ((CustomMKAnnotationView)annotationView).Url = customPin.Url;
            }
            else if (((CustomMKAnnotationView)annotationView).NumMoments != customPin.NumMoments)
            {
                annotationView.Image = UIImage.FromFile($"m{customPin.NumMoments.ToString()}.png");
            }

            annotationView.CanShowCallout = true;

            return annotationView;
        }

        void OnDidSelectAnnotationView (object sender, MKAnnotationViewEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;
            customPinView = new UIView();

            if (customView.Id == "Xamarin")
            {
                customPinView.Frame = new CGRect(0, 0, 200, 84);
                customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
                e.View.AddSubview(customPinView);
            }
        }

        void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            if (!e.View.Selected)
            {
                customPinView.RemoveFromSuperview();
                customPinView.Dispose();
                customPinView = null;
            }
        }

        CustomPin GetCustomPin(MKPointAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }

            return null;
        }
    }
}
