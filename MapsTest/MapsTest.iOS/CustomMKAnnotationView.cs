using MapKit;

namespace MapsTest.iOS
{
    public class CustomMKAnnotationView : MKAnnotationView
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public int NumMoments;

        public CustomMKAnnotationView(IMKAnnotation annotation, CustomPin pin) : base(annotation, pin.Id)
        {
            NumMoments = pin.NumMoments;
        }
    }
}
