using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace MiComanderaApp.Presentation.Services
{
    /// <summary>
    /// Mueve el control en órbita elíptica alrededor de un centro.
    /// Al final de cada ciclo el radio se encoge (aterriza en el plato)
    /// y el control se desvanece para reaparecer al inicio del ciclo.
    /// Los iconos siempre quedan verticales (solo se posiciona, no se rota).
    /// El control debe vivir dentro de un Canvas.
    /// </summary>
    public class OrbitBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<double> CenterXProperty =
            AvaloniaProperty.Register<OrbitBehavior, double>(nameof(CenterX), 150.0);

        public static readonly StyledProperty<double> CenterYProperty =
            AvaloniaProperty.Register<OrbitBehavior, double>(nameof(CenterY), 150.0);

        public static readonly StyledProperty<double> RadiusXProperty =
            AvaloniaProperty.Register<OrbitBehavior, double>(nameof(RadiusX), 115.0);

        public static readonly StyledProperty<double> RadiusYProperty =
            AvaloniaProperty.Register<OrbitBehavior, double>(nameof(RadiusY), 115.0);

        public static readonly StyledProperty<double> CycleSecondsProperty =
            AvaloniaProperty.Register<OrbitBehavior, double>(nameof(CycleSeconds), 4.0);

        /// <summary>Desfase dentro del ciclo (0-1) para repartir los ingredientes.</summary>
        public static readonly StyledProperty<double> PhaseOffsetProperty =
            AvaloniaProperty.Register<OrbitBehavior, double>(nameof(PhaseOffset), 0.0);

        public double CenterX { get => GetValue(CenterXProperty); set => SetValue(CenterXProperty, value); }
        public double CenterY { get => GetValue(CenterYProperty); set => SetValue(CenterYProperty, value); }
        public double RadiusX { get => GetValue(RadiusXProperty); set => SetValue(RadiusXProperty, value); }
        public double RadiusY { get => GetValue(RadiusYProperty); set => SetValue(RadiusYProperty, value); }
        public double CycleSeconds { get => GetValue(CycleSecondsProperty); set => SetValue(CycleSecondsProperty, value); }
        public double PhaseOffset { get => GetValue(PhaseOffsetProperty); set => SetValue(PhaseOffsetProperty, value); }

        private DispatcherTimer? _timer;
        private DateTime _start;

        protected override void OnAttached()
        {
            base.OnAttached();
            _start = DateTime.UtcNow;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _timer.Tick += OnTick;
            _timer.Start();
        }

        protected override void OnDetaching()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= OnTick;
                _timer = null;
            }
            base.OnDetaching();
        }

        private void OnTick(object? sender, EventArgs e)
        {
            if (AssociatedObject == null || CycleSeconds <= 0) return;

            var elapsed = (DateTime.UtcNow - _start).TotalSeconds;
            var progress = ((elapsed / CycleSeconds) + PhaseOffset) % 1.0;

            // Ángulo en la elipse
            var angle = progress * Math.PI * 2.0;

            // Radio: completo hasta el 70% del ciclo, luego aterriza al centro
            double radiusFactor = 1.0;
            if (progress > 0.7)
            {
                var land = (progress - 0.7) / 0.3; // 0..1
                radiusFactor = 1.0 - land * 0.92;  // termina casi en el centro
            }

            // Opacidad: visible orbitando, se desvanece al aterrizar y reaparece
            double opacity = 1.0;
            if (progress > 0.85) opacity = 1.0 - (progress - 0.85) / 0.15;
            else if (progress < 0.05) opacity = progress / 0.05;

            var w = double.IsNaN(AssociatedObject.Width) ? 30.0 : AssociatedObject.Width;
            var h = double.IsNaN(AssociatedObject.Height) ? 30.0 : AssociatedObject.Height;

            var x = CenterX + Math.Cos(angle) * RadiusX * radiusFactor - w / 2.0;
            var y = CenterY + Math.Sin(angle) * RadiusY * radiusFactor - h / 2.0;

            AssociatedObject.SetValue(Canvas.LeftProperty, x);
            AssociatedObject.SetValue(Canvas.TopProperty, y);
            AssociatedObject.Opacity = opacity;
        }
    }
}
