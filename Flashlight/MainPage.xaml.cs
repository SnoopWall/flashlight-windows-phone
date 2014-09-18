using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Windows.Phone.Media.Capture;
using Flashlight.Resources;
using System.Windows.Media.Imaging;

namespace Flashlight {

    public partial class MainPage : PhoneApplicationPage {

        private AudioVideoCaptureDevice captureDevice;
        private bool flashOn, flashSupported, screenOn;
        private Image flashImage, screenImage;
        private const CameraSensorLocation sensorLocation = CameraSensorLocation.Back;

        // Constructor
        public MainPage() {
            InitializeComponent();

            ToggleFlash.ApplyTemplate();
            flashImage = FindVisualChildByType<Image>(ToggleFlash, "ToggleFlashImage");
            ToggleScreen.ApplyTemplate();
            screenImage = FindVisualChildByType<Image>(ToggleScreen, "ToggleScreenImage");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            InitializeCaptureDevice();
            flashOn = false;
            flashImage.Source = App.Current.Resources["flash"] as BitmapImage;
            if (screenOn) {
                ToggleScreen_Click(null, null);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            base.OnNavigatedFrom(e);
            captureDevice.Dispose();
        }

        private async void InitializeCaptureDevice() {
            captureDevice = await GetCaptureDevice();
            IReadOnlyList<object> supportedModes = AudioVideoCaptureDevice.GetSupportedPropertyValues(sensorLocation, KnownCameraAudioVideoProperties.VideoTorchMode);
            flashSupported = supportedModes.ToList().Contains((UInt32)VideoTorchMode.On);
        }

        private async Task<AudioVideoCaptureDevice> GetCaptureDevice() {
            return await AudioVideoCaptureDevice.OpenForVideoOnlyAsync(sensorLocation, AudioVideoCaptureDevice.GetAvailableCaptureResolutions(sensorLocation).First());
        }

        private void Website_Tap(object sender, System.Windows.Input.GestureEventArgs e) {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://www.snoopwall.com", UriKind.Absolute);
            webBrowserTask.Show();
        }

        private void ToggleFlash_Click(object sender, RoutedEventArgs e) {
            if (flashSupported) {
                if (flashOn) {
                    captureDevice.SetProperty(KnownCameraAudioVideoProperties.VideoTorchMode, VideoTorchMode.Off);
                    flashImage.Source = App.Current.Resources["flash"] as BitmapImage;
                } else {
                    captureDevice.SetProperty(KnownCameraAudioVideoProperties.VideoTorchMode, VideoTorchMode.On);
                    flashImage.Source = App.Current.Resources["flashPressed"] as BitmapImage;
                }
                flashOn = !flashOn;
            }
        }

        private void ToggleScreen_Click(object sender, RoutedEventArgs e) {
            if (screenOn) {
                BackGradient.Visibility = Visibility.Visible;
                screenImage.Source = App.Current.Resources["screen"] as BitmapImage;
                Title.Foreground = App.Current.Resources["White"] as SolidColorBrush;
                Website.Foreground = App.Current.Resources["White"] as SolidColorBrush;
            } else {
                BackGradient.Visibility = Visibility.Collapsed;
                screenImage.Source = App.Current.Resources["screenPressed"] as BitmapImage;
                Title.Foreground = App.Current.Resources["Black"] as SolidColorBrush;
                Website.Foreground = App.Current.Resources["Black"] as SolidColorBrush;
            }
            screenOn = !screenOn;
        }

        static T FindVisualChildByType<T>(DependencyObject element, String name) where T : DependencyObject {
            if (element is T && (element as FrameworkElement).Name == name)
                return element as T;
            int childcount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childcount; i++)
            {
                T childElement = FindVisualChildByType<T>(VisualTreeHelper.GetChild(element, i), name);
                if (childElement != null)
                    return childElement;
            }
            return null;
        }

    }
}