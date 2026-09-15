using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ModbusExtendedTools.Desktop.Enums;

namespace ModbusExtendedTools.Desktop.Views.Windows;

public partial class NotifyWindow : Window
{
    public NotifyWindow(string message, NotificationType type = NotificationType.Info, string? title = null)
    {
        InitializeComponent();

        MessageText.Text = message;
        TitleText.Text = title ?? type.ToString();

        ConfigureByType(type);
    }

    private void ConfigureByType(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.Info:
                OkButton.Visibility = Visibility.Visible;
                OkButton.Focus();
                CancelButton.Visibility = Visibility.Collapsed;
                IconImage.Source = new BitmapImage(new Uri("/Resources/Assets/IconInfo.png", UriKind.Relative));
                TitleIcon.Source = IconImage.Source;
                SystemSounds.Asterisk.Play();
                break;
            case NotificationType.Confirmation:
                OkButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                IconImage.Source = new BitmapImage(new Uri("/Resources/Assets/IconConfirm.png", UriKind.Relative));
                TitleIcon.Source = IconImage.Source;
                SystemSounds.Question.Play();
                break;
            case NotificationType.Error:
                OkButton.Visibility = Visibility.Visible;
                OkButton.Focus();
                CancelButton.Visibility = Visibility.Collapsed;
                IconImage.Source = new BitmapImage(new Uri("/Resources/Assets/Icon_Error.png", UriKind.Relative));
                TitleIcon.Source = IconImage.Source;
                SystemSounds.Hand.Play();
                break;
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}