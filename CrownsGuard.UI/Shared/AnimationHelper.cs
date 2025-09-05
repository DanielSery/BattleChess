using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CrownsGuard.UI.Shared;

public static class AnimationHelper
{
    public static Func<Task> AppearAnimation(this FrameworkElement element, bool isVisible)
    {
        if (isVisible && element.Visibility != Visibility.Visible)
        {
            element.Opacity = 0;
            element.Visibility = Visibility.Visible;
            
            var tcs = new TaskCompletionSource();
            return () =>
            {
                var appearAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(400),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                appearAnimation.Completed += (_, _)  => tcs.TrySetResult();
                element.BeginAnimation(UIElement.OpacityProperty, appearAnimation);
                return tcs.Task;
            };
        }
        else if (!isVisible && element.Visibility != Visibility.Hidden)
        {
            element.Opacity = 1;
            element.Visibility = Visibility.Visible;

            var tcs = new TaskCompletionSource();
            return () =>
            {
                var disappearAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(400),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                disappearAnimation.Completed += SlideOutOnCompleted;
                
                element.BeginAnimation(UIElement.OpacityProperty, disappearAnimation);
                return tcs.Task;
                
                void SlideOutOnCompleted(object? o, EventArgs eventArgs)
                {
                    disappearAnimation.Completed -= SlideOutOnCompleted;
                    tcs.TrySetResult();
                    element.Visibility = Visibility.Hidden;
                }
            };
        }
        else return () => Task.CompletedTask;
    } 
    
    public static Func<Task> SlideInFromTopAnimation(this FrameworkElement element, FrameworkElement visibleParent, bool isVisible)
    {
        if (isVisible && element.Visibility != Visibility.Visible)
        {
            var height = element.ActualHeight == 0 ? visibleParent.ActualHeight : element.ActualHeight;
            if (element.RenderTransform is not TranslateTransform transform)
            {
                element.RenderTransform = new TranslateTransform{Y = height};
            }
            else
            {
                transform.Y = height;
            }
            element.Visibility = Visibility.Visible;

            var tcs = new TaskCompletionSource();
            return () =>
            {
                var slideInAnimation = new DoubleAnimation
                {
                    From = height, 
                    To = 0,     
                    Duration = TimeSpan.FromMilliseconds(400),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                slideInAnimation.Completed += (_, _)  => tcs.TrySetResult();
                
                (element.RenderTransform as TranslateTransform)!.BeginAnimation(TranslateTransform.YProperty, slideInAnimation);
                return tcs.Task;
            };
        }
        else if (!isVisible && element.Visibility != Visibility.Hidden)
        {
            var height = element.ActualHeight ==  0 ? visibleParent.ActualHeight : element.ActualHeight;
            if (element.RenderTransform is not TranslateTransform transform)
            {
                element.RenderTransform = new TranslateTransform{Y = 0};
            }
            else
            {
                transform.Y = 0;
            }
            element.Visibility = Visibility.Visible;

            var tcs = new TaskCompletionSource();
            return () =>
            {
                var slideOut = new DoubleAnimation
                {
                    From = 0,        // Start off-screen to the right
                    To = height,            // End at original position
                    Duration = TimeSpan.FromMilliseconds(400),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                slideOut.Completed += SlideOutOnCompleted;
                
                (element.RenderTransform as TranslateTransform)!.BeginAnimation(TranslateTransform.YProperty, slideOut);
                return tcs.Task;
                
                void SlideOutOnCompleted(object? o, EventArgs eventArgs)
                {
                    slideOut.Completed -= SlideOutOnCompleted;
                    tcs.TrySetResult();
                    element.Visibility = Visibility.Hidden;
                }
            };
        }
        else return () => Task.CompletedTask;
    } 
    
    public static Func<Task> SlideInFromRightAnimation(this FrameworkElement element, FrameworkElement visibleParent, bool isVisible)
    {
        if (isVisible && element.Visibility != Visibility.Visible)
        {
            var width = element.ActualWidth == 0 ? visibleParent.ActualWidth : element.ActualWidth;
            if (element.RenderTransform is not TranslateTransform transform)
            {
                element.RenderTransform = new TranslateTransform{X = width};
            }
            else
            {
                transform.X = width;
            }
            element.Visibility = Visibility.Visible;

            var tcs = new TaskCompletionSource();
            return () =>
            {
                var slideInAnimation = new DoubleAnimation
                {
                    From = width,        // Start off-screen to the right
                    To = 0,            // End at original position
                    Duration = TimeSpan.FromMilliseconds(400),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                slideInAnimation.Completed += (_, _)  => tcs.TrySetResult();
                
                (element.RenderTransform as TranslateTransform)!.BeginAnimation(TranslateTransform.XProperty, slideInAnimation);
                return tcs.Task;
            };
        }
        else if (!isVisible && element.Visibility != Visibility.Hidden)
        {
            var width = element.ActualWidth == 0 ? visibleParent.ActualWidth : element.ActualWidth;
            element.Visibility = Visibility.Visible;
                
            var tcs = new TaskCompletionSource();
            return () =>
            {
                var slideOut = new DoubleAnimation
                {
                    From = 0,        // Start off-screen to the right
                    To = width,            // End at original position
                    Duration = TimeSpan.FromMilliseconds(400),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                slideOut.Completed += SlideOutOnCompleted;
                
                (element.RenderTransform as TranslateTransform)!.BeginAnimation(TranslateTransform.XProperty, slideOut);
                return tcs.Task;
                
                void SlideOutOnCompleted(object? o, EventArgs eventArgs)
                {
                    slideOut.Completed -= SlideOutOnCompleted;
                    tcs.TrySetResult();
                    element.Visibility = Visibility.Hidden;
                }
            };
        }
        else return () => Task.CompletedTask;
    } 
}