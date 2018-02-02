//This code taken and modified from John Papa @ Microsoft
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Interactivity;
using System.ComponentModel;

namespace Aggravation.GameEngine.Behaviors
{
    public class Swivel : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty FrontElementNameProperty =
            DependencyProperty.Register("FrontElementName", typeof(string),
                                        typeof(Swivel), new PropertyMetadata(null));

        [Category("Swivel Properties")]
        public string FrontElementName { get; set; }

        public static readonly DependencyProperty BackElementNameProperty =
            DependencyProperty.Register("BackElementName", typeof(string),
                                        typeof(Swivel), new PropertyMetadata(null));

        [Category("Swivel Properties")]
        public string BackElementName { get; set; }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(Duration),
                                        typeof(Swivel), new PropertyMetadata(null));

        [Category("Animation Properties")]
        public Duration Duration { get; set; }

        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation", typeof(RotationDirection),
                                        typeof(Swivel), new PropertyMetadata(RotationDirection.LeftToRight));

        [Category("Animation Properties")]
        public RotationDirection Rotation { get; set; }

        private readonly Storyboard frontToBackStoryboard = new Storyboard();
        private readonly Storyboard backToFrontStoryboard = new Storyboard();
        private bool forward = true;

        protected override void Invoke(object parameter)
        {
            if (AssociatedObject == null) return;

            FrameworkElement parent = AssociatedObject; // as FrameworkElement;
            UIElement front = null;
            UIElement back = null;

            front = parent.FindName(FrontElementName) as UIElement;
            back = parent.FindName(BackElementName) as UIElement;
            if (front == null || back == null) return;

            InvokeSwivel(frontToBackStoryboard, backToFrontStoryboard, front, back, this.Rotation, this.Duration, ref this.forward);
        }

        public static void InvokeSwivel(Storyboard frontToBackStoryboard, Storyboard backToFrontStoryboard, UIElement front, UIElement back, RotationDirection rotation, Duration duration, ref Boolean forward)
        {
            if (front.Projection == null || back.Projection == null)
            {
                front.Projection = new PlaneProjection();
                front.RenderTransformOrigin = new Point(.5, .5);
                front.Visibility = Visibility.Visible;

                back.Projection = new PlaneProjection { CenterOfRotationY = .5, RotationY = 180.0 }; //, CenterOfRotationZ = this.CenterOfRotationZ };
                back.RenderTransformOrigin = new Point(.5, .5);
                back.Visibility = Visibility.Collapsed;

                RotationData showBackRotation = null;
                RotationData hideFrontRotation = null;
                RotationData showFrontRotation = null;
                RotationData hideBackRotation = null;

                var frontPP = new PlaneProjection(); // { CenterOfRotationZ = this.CenterOfRotationZ };
                var backPP = new PlaneProjection(); // { CenterOfRotationZ = this.CenterOfRotationZ };

                switch (rotation)
                {
                    case RotationDirection.LeftToRight:
                        backPP.CenterOfRotationY = frontPP.CenterOfRotationY = 0.5;
                        showBackRotation = new RotationData { FromDegrees = 180.0, MidDegrees = 90.0, ToDegrees = 0.0, RotationProperty = "RotationY", PlaneProjection = backPP, AnimationDuration = duration };
                        hideFrontRotation = new RotationData { FromDegrees = 0.0, MidDegrees = -90.0, ToDegrees = -180.0, RotationProperty = "RotationY", PlaneProjection = frontPP, AnimationDuration = duration };
                        showFrontRotation = new RotationData { FromDegrees = -180.0, MidDegrees = -90.0, ToDegrees = 0.0, RotationProperty = "RotationY", PlaneProjection = frontPP, AnimationDuration = duration };
                        hideBackRotation = new RotationData { FromDegrees = 0.0, MidDegrees = 90.0, ToDegrees = 180.0, RotationProperty = "RotationY", PlaneProjection = backPP, AnimationDuration = duration };
                        break;
                    case RotationDirection.RightToLeft:
                        backPP.CenterOfRotationY = frontPP.CenterOfRotationY = 0.5;
                        showBackRotation = new RotationData { FromDegrees = -180.0, MidDegrees = -90.0, ToDegrees = 0.0, RotationProperty = "RotationY", PlaneProjection = backPP, AnimationDuration = duration };
                        hideFrontRotation = new RotationData { FromDegrees = 0.0, MidDegrees = 90.0, ToDegrees = 180.0, RotationProperty = "RotationY", PlaneProjection = frontPP, AnimationDuration = duration };
                        showFrontRotation = new RotationData { FromDegrees = 180.0, MidDegrees = 90.0, ToDegrees = 0.0, RotationProperty = "RotationY", PlaneProjection = frontPP, AnimationDuration = duration };
                        hideBackRotation = new RotationData { FromDegrees = 0.0, MidDegrees = -90.0, ToDegrees = -180.0, RotationProperty = "RotationY", PlaneProjection = backPP, AnimationDuration = duration };
                        break;
                    case RotationDirection.BottomToTop:
                        backPP.CenterOfRotationX = frontPP.CenterOfRotationX = 0.5;
                        showBackRotation = new RotationData { FromDegrees = 180.0, MidDegrees = 90.0, ToDegrees = 0.0, RotationProperty = "RotationX", PlaneProjection = backPP, AnimationDuration = duration };
                        hideFrontRotation = new RotationData { FromDegrees = 0.0, MidDegrees = -90.0, ToDegrees = -180.0, RotationProperty = "RotationX", PlaneProjection = frontPP, AnimationDuration = duration };
                        showFrontRotation = new RotationData { FromDegrees = -180.0, MidDegrees = -90.0, ToDegrees = 0.0, RotationProperty = "RotationX", PlaneProjection = frontPP, AnimationDuration = duration };
                        hideBackRotation = new RotationData { FromDegrees = 0.0, MidDegrees = 90.0, ToDegrees = 180.0, RotationProperty = "RotationX", PlaneProjection = backPP, AnimationDuration = duration };
                        break;
                    case RotationDirection.TopToBottom:
                        backPP.CenterOfRotationX = frontPP.CenterOfRotationX = 0.5;
                        showBackRotation = new RotationData { FromDegrees = -180.0, MidDegrees = -90.0, ToDegrees = 0.0, RotationProperty = "RotationX", PlaneProjection = backPP, AnimationDuration = duration };
                        hideFrontRotation = new RotationData { FromDegrees = 0.0, MidDegrees = 90.0, ToDegrees = 180.0, RotationProperty = "RotationX", PlaneProjection = frontPP, AnimationDuration = duration };
                        showFrontRotation = new RotationData { FromDegrees = 180.0, MidDegrees = 90.0, ToDegrees = 0.0, RotationProperty = "RotationX", PlaneProjection = frontPP, AnimationDuration = duration };
                        hideBackRotation = new RotationData { FromDegrees = 0.0, MidDegrees = -90.0, ToDegrees = -180.0, RotationProperty = "RotationX", PlaneProjection = backPP, AnimationDuration = duration };
                        break;
                }

                front.RenderTransformOrigin = new Point(.5, .5);
                back.RenderTransformOrigin = new Point(.5, .5);

                front.Projection = frontPP;
                back.Projection = backPP;

                frontToBackStoryboard.Duration = duration;
                backToFrontStoryboard.Duration = duration;

                // Rotation
                frontToBackStoryboard.Children.Add(CreateRotationAnimation(showBackRotation));
                frontToBackStoryboard.Children.Add(CreateRotationAnimation(hideFrontRotation));
                backToFrontStoryboard.Children.Add(CreateRotationAnimation(hideBackRotation));
                backToFrontStoryboard.Children.Add(CreateRotationAnimation(showFrontRotation));


                // Visibility
                frontToBackStoryboard.Children.Add(CreateVisibilityAnimation(showBackRotation.AnimationDuration, front, false));
                frontToBackStoryboard.Children.Add(CreateVisibilityAnimation(hideFrontRotation.AnimationDuration, back, true));
                backToFrontStoryboard.Children.Add(CreateVisibilityAnimation(hideBackRotation.AnimationDuration, front, true));
                backToFrontStoryboard.Children.Add(CreateVisibilityAnimation(showFrontRotation.AnimationDuration, back, false));
            }

            if (forward)
            {
                frontToBackStoryboard.Begin();
                forward = false;
            }
            else
            {
                backToFrontStoryboard.Begin();
                forward = true;
            }
        }

        private static ObjectAnimationUsingKeyFrames CreateVisibilityAnimation(Duration duration, DependencyObject element, bool show)
        {
            var animation = new ObjectAnimationUsingKeyFrames();
            animation.BeginTime = new TimeSpan(0);
            animation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = new TimeSpan(0), Value = (show ? Visibility.Collapsed : Visibility.Visible) });
            animation.KeyFrames.Add(new DiscreteObjectKeyFrame { KeyTime = new TimeSpan(duration.TimeSpan.Ticks / 2), Value = (show ? Visibility.Visible : Visibility.Collapsed) });
            Storyboard.SetTargetProperty(animation, new PropertyPath("Visibility"));
            Storyboard.SetTarget(animation, element);
            return animation;
        }


        private static DoubleAnimationUsingKeyFrames CreateRotationAnimation(RotationData rd)
        {
            var animation = new DoubleAnimationUsingKeyFrames();
            animation.BeginTime = new TimeSpan(0);
            //animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = new TimeSpan(0), Value = rd.FromDegrees, EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn } });
            //animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = new TimeSpan(rd.AnimationDuration.TimeSpan.Ticks / 2), Value = rd.MidDegrees });
            //animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = new TimeSpan(rd.AnimationDuration.TimeSpan.Ticks), Value = rd.ToDegrees, EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut } });
            animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = new TimeSpan(0), Value = rd.FromDegrees, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn } });
            animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = new TimeSpan(rd.AnimationDuration.TimeSpan.Ticks / 2), Value = rd.MidDegrees });
            animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = new TimeSpan(rd.AnimationDuration.TimeSpan.Ticks), Value = rd.ToDegrees, EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } });
            Storyboard.SetTargetProperty(animation, new PropertyPath(rd.RotationProperty));
            Storyboard.SetTarget(animation, rd.PlaneProjection);
            return animation;
        }
    }

    public class RotationData
    {
        public double FromDegrees { get; set; }
        public double MidDegrees { get; set; }
        public double ToDegrees { get; set; }
        public string RotationProperty { get; set; }
        public PlaneProjection PlaneProjection { get; set; }
        public Duration AnimationDuration { get; set; }
    }

    public enum RotationDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }
}
