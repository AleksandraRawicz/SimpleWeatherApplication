using System;
using System.Collections.Generic;
using SkiaSharp.Views;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp.Views.Forms;
using Weather.ViewModels;
using System.Threading.Tasks;


namespace Weather.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        int sweepAngleSun = 1;
        int sweepAngleHumidity = 1;
        double humidity=1;
        double humidityAngle = 1;
        int windDeg = 0;
        bool windOn = false;
        string sunset;
        string sunrise;
        double sunHeight=0;
        int sunsetS; int sunriseS;
        bool eventEnabled = true;

        public CurrentWeatherPage()
        {
            InitializeComponent();
            LoadInfo();
        }

        void LoadInfo()
        {
            double.TryParse(humidityValue.Text, out humidity);
            sunrise = sunriseString.Text;
            sunset = sunsetString.Text;
            int.TryParse(sunsetSec.Text, out sunsetS);
            int.TryParse(sunriseSec.Text, out sunriseS);
            try
            {
                sunHeight = (DateTime.Now).Subtract(new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(sunriseS)).TotalSeconds /
                    (new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(sunsetS)).Subtract(new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(sunriseS)).TotalSeconds;
            }
            catch (Exception ex)
            {
                sunHeight = 0;
            }
            sunHeight = Convert.ToInt32(sunHeight * 180);
            humidityAngle = Convert.ToInt32(270 * humidity / 100);
        }

        void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
        {
            if (e.ScrollY > humidityView.Y)
            {
                ActivateWeatherEvents();
            }
        }
        async void ActivateWeatherEvents()
        {
            if (eventEnabled)
            {
                LoadInfo();
                eventEnabled = false;
                windOn = !windOn;
                var sun = OnSunTriggered();
                var humidity = OnHumidityTrigerred();
                var wind = OnWindTrigerred();
                var tasks = new List<Task> { sun, humidity};
                await Task.WhenAll(tasks);
                //eventEnabled = true;
            }
        }

        async Task OnSunTriggered()
        {            
            if (sunHeight<0)
            {
                sweepAngleSun = 1;
            }
            else if(sunHeight > 180)
            {
                sweepAngleSun = 180;
            }
            else
            {
                for (int i = 1; i <= sunHeight; i++)
                {
                    sweepAngleSun = i;
                    sunView.InvalidateSurface();
                    await Task.Delay(5);
                }
            }
        }
        async Task OnHumidityTrigerred()
        {
            for (int i = 1; i <= humidityAngle; i++)
            {
                sweepAngleHumidity = i;
                humidityView.InvalidateSurface();
                await Task.Delay(5);
            }
        }
        async Task OnWindTrigerred()
        {
            windDeg = 0;
            while(windOn)
            {
                if (windDeg < 359)
                    windDeg++;
                else
                    windDeg=0;
                windView.InvalidateSurface();
                await Task.Delay(5);
            }
        }
        private void humidityView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            SKImageInfo info = e.Info;

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.LightSlateGray.ToSKColor(),
                StrokeWidth = 15,
                StrokeCap = SKStrokeCap.Round
        };

            SKPaint arcPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.White.ToSKColor(),
                StrokeWidth = 15,
                StrokeCap = SKStrokeCap.Round
            };

            float margin = 30;

            SKPaint textPaint = new SKPaint
            {
                TextSize = margin,
                IsAntialias = true,
                Color = SKColors.White,
            };
            SKPaint textPaint2 = new SKPaint
            {
                TextSize = 80,
                IsAntialias = true,
                Color = SKColors.White,
            };

            canvas.Clear(SKColors.Transparent);

            SKRect rect = new SKRect(margin, margin, info.Width-margin-1, info.Height-margin-1);

            float startAngle = 135;
            float sweepAngle = 270;

            using (SKPath path = new SKPath())
            {
                path.AddArc(rect, startAngle, sweepAngle);
                canvas.DrawPath(path, paint);
            }

            using (SKPath path = new SKPath())
            {                
                path.AddArc(rect, startAngle, sweepAngleHumidity);
                canvas.DrawPath(path, arcPaint);
            }

            canvas.DrawText("0%", rect.Left + margin, rect.Bottom, textPaint);
            canvas.DrawText("100%", rect.Right-2*margin, rect.Bottom, textPaint);
            canvas.DrawText(humidity+ "%", rect.Left + rect.Width/2-80, rect.Top + rect.Height/2, textPaint2);
        }
        private void sunView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            SKImageInfo info = e.Info;

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                StrokeWidth = 5,
                StrokeCap = SKStrokeCap.Round,
                PathEffect = SKPathEffect.CreateDash(new float[] {30,10 }, 20)
            };

            SKPaint sunDPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Yellow,
                StrokeWidth = 5,
            };
            SKPaint sunLPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.LightGoldenrodYellow,
                StrokeWidth = 5,
            };

            int width = info.Width;
            int height = info.Height;
            canvas.Translate(width / 2, height / 2);

            SKPaint textPaint = new SKPaint
            {
                TextSize = 30,
                IsAntialias = true,
                Color = SKColors.White,
            };

            canvas.Clear(SKColors.Transparent);
            float startAngle = 180;

            SKRect rect = new SKRect(-300,-100,300,250);
            SKPath path1 = new SKPath();
            path1.AddArc( rect, startAngle, 180);
            canvas.DrawPath(path1, paint);

            using (SKPath path = new SKPath())
            {
                path.AddArc(rect, startAngle, sweepAngleSun);
                canvas.DrawPath(path, paint);
                SKPoint p = path.GetPoint(path.PointCount - 1);
                canvas.DrawCircle(p, 30, sunLPaint);
                canvas.DrawCircle(p, 15, sunDPaint);
            }

            float textPlace = rect.Top + rect.Height / 2;
            canvas.DrawText(sunrise, rect.Left-30, textPlace+ 80, textPaint);
            canvas.DrawText(sunset, rect.Right-30, textPlace +80, textPaint);
        }
        private void windView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            SKImageInfo info = e.Info;

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = SKColors.White,
                StrokeWidth = 5,
                StrokeCap = SKStrokeCap.Round
            };

            canvas.Clear(SKColors.Transparent);

            int width = info.Width;
            int height = info.Height;

            canvas.Translate(width / 2, height / 2);
            canvas.DrawLine(0,-50,0,150,paint);
            canvas.DrawLine(90, -20, 90, 140, paint);
            canvas.DrawLine(-110, -10, -110, 130, paint);

            DrawWindTurbine(canvas, 0,-50,70,paint);
            DrawWindTurbine(canvas, 90, -20, 50, paint);
            DrawWindTurbine(canvas, -110, -10, 50, paint);
        }
        private void DrawWindTurbine(SKCanvas canvas, double x0, double y0, double radius, SKPaint paint)
        {
            canvas.Save();
            canvas.RotateDegrees(windDeg, (float)(x0),(float)(y0));
            canvas.DrawCircle((float)(x0), (float)(y0), 5, paint);
            DrawWing(canvas, x0,y0, radius, 210);
            DrawWing(canvas, x0, y0, radius, 90);
            DrawWing(canvas, x0, y0, radius, 330);
            canvas.Restore();
        }
        private void DrawWing(SKCanvas canvas, double x0, double y0, double radius, double alfa)
        {
            double[] p1 = GetPointFromCircle(radius, x0, y0, alfa);
            double[] p2 = GetPointFromCircle(radius*0.4, x0, y0, alfa-10);
            double[] p3 = GetPointFromCircle(radius*0.6, x0, y0, alfa+10);
            SKPoint[] points = new SKPoint[] { new SKPoint((float)(x0), (float)(y0)), new SKPoint((float)(p2[0]), (float)(p2[1])), new SKPoint((float)(p1[0]), (float)(p1[1])), new SKPoint((float)(p3[0]), (float)(p3[1])), new SKPoint((float)(x0), (float)(y0)) };

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = SKColors.White,
                StrokeWidth = 5,
                StrokeCap = SKStrokeCap.Round
            };

            var path = new SKPath();
            path.MoveTo(points[0]);
            for (var i = 1; i < points.Length; i++)
            {
                path.LineTo(points[i]);
            }
            path.LineTo(points[0]);
            path.Close();
            canvas.DrawPath(path, paint);
        }
        double[] GetPointFromCircle(double r, double x0, double y0, double alfa)
        {
            alfa = 360 - alfa;
            return new double[] { x0 + r*Math.Cos(alfa/180*Math.PI), y0 + r*Math.Sin(alfa/180*Math.PI)};
        }
    }
}