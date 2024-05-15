using Xamarin.Forms.PancakeView;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using FFImageLoading.Forms;

namespace ISTQB_PL.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ZoomedImageViewPage : ContentPage
	{
        private CachedImage cachedImage;
        private PancakeView pancakeView;
        private double startScale = 1;
        //private double currentScale = 1.0;
        private double startX, startY;

        public ZoomedImageViewPage(string imageUrl)
        {
            InitializeComponent();

            cachedImage = new CachedImage
            {
                Source = ImageSource.FromUri(new Uri(imageUrl)),
                Aspect = Aspect.AspectFit,
                BitmapOptimizations = true,
                
            };
            pancakeView = new PancakeView
            {
                Content = cachedImage,
                BackgroundColor = (Color)Application.Current.Resources["JasneTlo"] // Opcjonalne: ustaw kolor tła
            };

            //var pinchGestureRecognizer = new PinchGestureRecognizer();
            //pinchGestureRecognizer.PinchUpdated += OnPinchUpdated;

            var panGestureRecognizer = new PanGestureRecognizer();
            panGestureRecognizer.PanUpdated += OnPanUpdated;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnTapped;

            //pancakeView.GestureRecognizers.Add(pinchGestureRecognizer);
            pancakeView.GestureRecognizers.Add(panGestureRecognizer);
            pancakeView.GestureRecognizers.Add(tapGestureRecognizer);


            //scrollView = new ScrollView
            //{
            //    Content = pancakeView,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    VerticalOptions = LayoutOptions.FillAndExpand,
            //    IsEnabled = true,
            //    BackgroundColor = (Color)Application.Current.Resources["JasneTlo"],
            //};

            Content = pancakeView;
            Title = "Zdjęcie";
        }

        private void OnTapped(object sender, EventArgs e)
        {
            // Przywróć zdjęcie do pierwotnych rozmiarów i położenia
            pancakeView.Scale = pancakeView.Scale == 1.0 ? 2.0 : 1.0;
            pancakeView.TranslationX = 0;
            pancakeView.TranslationY = 0;
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                startScale = pancakeView.Scale;
                pancakeView.AnchorX = e.ScaleOrigin.X;
                pancakeView.AnchorY = e.ScaleOrigin.Y;
            }

            if (e.Status == GestureStatus.Running)
            {
                double newScale = startScale * e.Scale;

                // Ogranicz skalę do odpowiednich zakresów
                newScale = Math.Max(1, Math.Min(newScale, 3));

                // Zastosuj transformację skali obrazu
                pancakeView.Scale = newScale;
            }

            if (e.Status == GestureStatus.Completed)
            {
                double newScale = startScale * e.Scale;
                newScale = Math.Max(1, Math.Min(newScale, 3));

                // Animacja zmiany skali po zakończeniu gestu pinch
                pancakeView.ScaleTo(newScale, 250, Easing.Linear);
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    startX = pancakeView.TranslationX;
                    startY = pancakeView.TranslationY;
                    pancakeView.BatchBegin();
                    break;
                case GestureStatus.Running:
                    // Przesuwaj obraz wraz z ruchem palca
                    pancakeView.TranslationX = startX + e.TotalX / pancakeView.Scale;
                    pancakeView.TranslationY = startY + e.TotalY / pancakeView.Scale;
                    break;
                case GestureStatus.Completed:
                    pancakeView.BatchCommit();
                    break;
            }
        }
    }
}