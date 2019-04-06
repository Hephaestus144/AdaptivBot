using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace AdaptivBot.CustomControls
{
    /// <summary>
    /// Interaction logic for PrimaryButtonWithIcon.xaml
    /// </summary>
    public partial class PrimaryButtonWithIcon : Button
    {
        public PrimaryButtonWithIcon()
        {
            InitializeComponent();
            this.FontSize = 10;
            this.IconForeground = (Brush)Application.Current.Resources["SecondaryAccentBrush"];
            this.IconKind = "Wifi";
            this.IconLeftMargin = 4;
            this.IconSize = 18;
            this.Text = "Text";
        }


        #region FontSize

        public new static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(PrimaryButtonWithIcon),
                new FrameworkPropertyMetadata(FontSizePropertyChangedCallback));

        private static void FontSizePropertyChangedCallback(
            DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
        }

        public new int FontSize
        {
            get => (int)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        #endregion FontSize


        #region IconForeground

        public static readonly DependencyProperty IconForegroundProperty =
            DependencyProperty.Register("IconForeground", typeof(Brush), typeof(PrimaryButtonWithIcon),
                new FrameworkPropertyMetadata(IconForegroundPropertyChangedCallback));

        private static void IconForegroundPropertyChangedCallback(
            DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
        }

        public Brush IconForeground
        {
            get => (Brush)GetValue(IconForegroundProperty);
            set => SetValue(IconForegroundProperty, value);
        }

        #endregion IconForeground


        #region IconKind

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register("IconKind", typeof(string), typeof(PrimaryButtonWithIcon),
                new FrameworkPropertyMetadata(IconKindPropertyChangedCallback));

        private static void IconKindPropertyChangedCallback(
            DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
        }

        public string IconKind
        {
            get => (string)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        #endregion IconKind


        #region IconSize

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(int), typeof(PrimaryButtonWithIcon),
                new FrameworkPropertyMetadata(IconSizePropertyChangedCallback));

        private static void IconSizePropertyChangedCallback(
            DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
        }

        public int IconSize
        {
            get => (int)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        #endregion IconSize


        #region IconLeftMargin

        public static readonly DependencyProperty IconLeftMarginProperty =
            DependencyProperty.Register("IconLeftMargin", typeof(int), typeof(PrimaryButtonWithIcon),
                new FrameworkPropertyMetadata(IconLeftMarginPropertyChangedCallback));

        private static void IconLeftMarginPropertyChangedCallback(
            DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
        }

        public int IconLeftMargin
        {
            get => (int)GetValue(IconLeftMarginProperty);
            set => SetValue(IconLeftMarginProperty, value);
        }

        #endregion IconLeftMargin


        #region Text

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PrimaryButtonWithIcon),
                new FrameworkPropertyMetadata(TextPropertyChangedCallback));

        private static void TextPropertyChangedCallback(DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
        }


        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(PrimaryButtonWithIcon.TextProperty, value);
        }

        #endregion Text
    }
}
