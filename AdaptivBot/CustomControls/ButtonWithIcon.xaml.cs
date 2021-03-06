﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace AdaptivBot.CustomControls
{
    /// <summary>
    /// Interaction logic for ButtonWithIcon.xaml
    /// </summary>
    public partial class ButtonWithIcon : Button
    {
        public ButtonWithIcon()
        {
            InitializeComponent();
            this.FontSize = 10;
            this.IconForeground = (Brush)Application.Current.Resources["SecondaryAccentBrush"];
            this.IconKind = "Wifi";
            this.IconLeftMargin = 4;
            this.IconSize = 18;
            this.Padding = new Thickness(5,0,5,0);
            this.Text = "Text";
        }


        #region FontSize

        public new static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(ButtonWithIcon),
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
            DependencyProperty.Register("IconForeground", typeof(Brush), typeof(ButtonWithIcon),
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
            DependencyProperty.Register("IconKind", typeof(string), typeof(ButtonWithIcon),
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
            DependencyProperty.Register("IconSize", typeof(int), typeof(ButtonWithIcon),
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
            DependencyProperty.Register("IconLeftMargin", typeof(int), typeof(ButtonWithIcon),
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


        #region Padding

        public new static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(ButtonWithIcon),
                new FrameworkPropertyMetadata(PaddingPropertyChangedCallback));

        private static void PaddingPropertyChangedCallback(
            DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
        }

        public new Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        #endregion Padding


        #region Text

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ButtonWithIcon),
                new FrameworkPropertyMetadata(TextPropertyChangedCallback));

        private static void TextPropertyChangedCallback(DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
        }


        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(ButtonWithIcon.TextProperty, value);
        }

        #endregion Text
    }
}
