//nowy poprawiony kod
using Android.Icu.Number;
using ISTQB_PL.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.IO;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.App.Assist.AssistStructure;

namespace ISTQB_PL.Views
{
    public partial class ZoomPanImageView : ContentPage
    {
        private SKCanvasView skCanvasView;
        private SKBitmap bitmap;
        private SKMatrix matrix = SKMatrix.CreateIdentity();
        private SKPoint pinchCenter;
        private SKPoint startPoint;

        public ZoomPanImageView(string imageUrl)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Brak dostępu do Internetu
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
            else
            {
                skCanvasView = new SKCanvasView();
                skCanvasView.PaintSurface += OnPaintSurface;
                skCanvasView.EnableTouchEvents = true;

                var pinchGesture = new PinchGestureRecognizer();
                pinchGesture.PinchUpdated += OnPinchUpdated;
                skCanvasView.GestureRecognizers.Add(pinchGesture);

                var panGesture = new PanGestureRecognizer();
                panGesture.PanUpdated += OnPanUpdated;
                skCanvasView.GestureRecognizers.Add(panGesture);

                imageUrl = imageUrl.Replace("http", "https");
                LoadImageAsync($"{imageUrl}");

                Content = skCanvasView;
            }

            Title = "Zdjęcie";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void LoadImageAsync(string imageUrl)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var imageStream = await httpClient.GetStreamAsync(new Uri(imageUrl));

                    using (var memoryStream = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();

                        bitmap = SKBitmap.Decode(imageBytes);
                        AdjustMatrix();
                        skCanvasView.InvalidateSurface();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        private void AdjustMatrix()
        {
            if (bitmap != null)
            {
                float scale = (float)skCanvasView.CanvasSize.Width / bitmap.Width;

                matrix = SKMatrix.CreateIdentity();
                matrix = SKMatrix.CreateScale(scale, scale);
                matrix = SKMatrix.CreateTranslation((skCanvasView.CanvasSize.Width - bitmap.Width * scale) / 2, 0);
            }
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            if (bitmap != null)
            {
                float scale = matrix.ScaleX;
                float scaledWidth = bitmap.Width * skCanvasView.CanvasSize.Width / bitmap.Width * scale;

                float scaledHeight = bitmap.Height * scale;

                float translateX = matrix.TransX;
                float translateY = (float)(skCanvasView.CanvasSize.Height / 2 - scaledHeight / 2);

                canvas.DrawBitmap(bitmap, new SKRect(translateX, translateY, translateX + scaledWidth, translateY + scaledHeight));
            }
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch(e.Status)
            {
                case GestureStatus.Started:
                    pinchCenter = new SKPoint(skCanvasView.CanvasSize.Width / 2, skCanvasView.CanvasSize.Height / 2);
                    break;
                case GestureStatus.Running:
                    float newScale = matrix.ScaleX * (float)e.Scale;
                    newScale = Math.Max(1, Math.Min(newScale, 3));

                    matrix = SKMatrix.CreateScale(newScale, newScale, pivotX: pinchCenter.X, pivotY: pinchCenter.Y);
                    skCanvasView.InvalidateSurface();
                    break;
                case GestureStatus.Completed:
                    break;
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    startPoint = new SKPoint((float)e.TotalX, (float)e.TotalY);
                    break;

                case GestureStatus.Running:
                    SKPoint panVector = new SKPoint((float)e.TotalX, (float)e.TotalY) - startPoint;
                    matrix.TransX += panVector.X;
                    matrix.TransY += panVector.Y;
                    startPoint = new SKPoint((float)e.TotalX, (float)e.TotalY);
                    skCanvasView.InvalidateSurface();
                    break;

                case GestureStatus.Completed:
                    break;
            }
        }
    }
}
