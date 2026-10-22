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
    public class ScaleBehavior:Behavior<Control>
    {
        public static readonly StyledProperty<double> DurationProperty =
            AvaloniaProperty.Register<ScaleBehavior, double>(
                nameof(Duration), 1.0);

        public static readonly StyledProperty<double> FromProperty =
            AvaloniaProperty.Register<ScaleBehavior, double>(
                nameof(From), 0.0);

        public static readonly StyledProperty<double> ToProperty =
            AvaloniaProperty.Register<ScaleBehavior, double>(
                nameof(To), 1.0);

        public static readonly StyledProperty<string> EasingProperty =
            AvaloniaProperty.Register<ScaleBehavior, string>(
                nameof(Easing), "Linear");

        public static readonly StyledProperty<double> DelaySecondsProperty =
            AvaloniaProperty.Register<ScaleBehavior, double>(
                nameof(DelaySeconds), 0.0);

        public static readonly StyledProperty<bool> RunOnceProperty =
            AvaloniaProperty.Register<ScaleBehavior, bool>(
                nameof(RunOnce), false);

        public static readonly StyledProperty<bool> FadeInProperty =
            AvaloniaProperty.Register<ScaleBehavior, bool>(
                nameof(FadeIn), false);

        public double DelaySeconds
        {
            get => GetValue(DelaySecondsProperty);
            set => SetValue(DelaySecondsProperty, value);
        }

        public bool RunOnce
        {
            get => GetValue(RunOnceProperty);
            set => SetValue(RunOnceProperty, value);
        }

        public bool FadeIn
        {
            get => GetValue(FadeInProperty);
            set => SetValue(FadeInProperty, value);
        }

        public double Duration
        {
            get => GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public double From
        {
            get => GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public double To
        {
            get => GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public string Easing
        {
            get => GetValue(EasingProperty);
            set => SetValue(EasingProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _ = IniciarAnimacion();
        }

        private async Task IniciarAnimacion()
        {
            if (AssociatedObject == null) return;

            if (FadeIn)
                AssociatedObject.Opacity = 0;

            if (DelaySeconds > 0)
                await Task.Delay(TimeSpan.FromSeconds(DelaySeconds));

            if (AssociatedObject == null) return;

            var fromFrame = new KeyFrame { Cue = new Cue(0) };
            fromFrame.Setters.Add(new Setter(Control.RenderTransformProperty, new ScaleTransform(From, From)));
            fromFrame.Setters.Add(new Setter(Control.RenderTransformOriginProperty, new RelativePoint(0.5, 0.5, RelativeUnit.Relative)));

            var toFrame = new KeyFrame { Cue = new Cue(1) };
            toFrame.Setters.Add(new Setter(Control.RenderTransformProperty, new ScaleTransform(To, To)));

            if (FadeIn)
            {
                fromFrame.Setters.Add(new Setter(Control.OpacityProperty, 0.0));
                toFrame.Setters.Add(new Setter(Control.OpacityProperty, 1.0));
            }

            var animation = new Animation
            {
                Duration = TimeSpan.FromSeconds(Duration),
                IterationCount = RunOnce ? new IterationCount(1) : IterationCount.Infinite,
                FillMode = RunOnce ? FillMode.Forward : FillMode.None,
                // Easing = GetEasing(Easing),
                Children = { fromFrame, toFrame }
            };

            await animation.RunAsync(AssociatedObject);
        }

        // private IEasing GetEasing(string easingName)
        // {
        //     return easingName switch
        //     {
        //         "ElasticEaseOut" => new ElasticEase { EasingMode = EasingMode.EaseOut },
        //         "BounceEaseOut" => new BounceEase { EasingMode = EasingMode.EaseOut },
        //         "BackEaseOut" => new BackEase { EasingMode = EasingMode.EaseOut },
        //         _ => new LinearEasing()
        //     };
        // }
    }
}