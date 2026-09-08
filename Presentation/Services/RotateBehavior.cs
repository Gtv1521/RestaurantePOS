using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;

namespace MiComanderaApp.Presentation.Services
{
    public class RotateBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<double> DurationProperty =
       AvaloniaProperty.Register<RotateBehavior, double>(
           nameof(Duration),
           1.3);

        public double Duration
        {
            get => GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject == null)
                return;

            // Crear la transformación de rotación
            AssociatedObject.RenderTransform = new RotateTransform(0);

            // El centro de rotación será el centro del control
            AssociatedObject.RenderTransformOrigin = new RelativePoint(
                0.5,
                0.5,
                RelativeUnit.Relative);

            // Iniciar la animación
            _ = IniciarAnimacion();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.RenderTransform = null;
            }
        }

        private async Task IniciarAnimacion()
        {
            if (AssociatedObject == null)
                return;

            var transform = AssociatedObject.RenderTransform as RotateTransform;

            if (transform == null)
                return;

            var animation = new Animation
            {
                Duration = TimeSpan.FromSeconds(Duration),
                IterationCount = IterationCount.Infinite,
                Easing = new LinearEasing(),

                Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(
                            RotateTransform.AngleProperty,
                            0.0)
                    }
                },

                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(
                            RotateTransform.AngleProperty,
                            360.0)
                    }
                }
            }
            };

            await animation.RunAsync(AssociatedObject);
        }

    }
}