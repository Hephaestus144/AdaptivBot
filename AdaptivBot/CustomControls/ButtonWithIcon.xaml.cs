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
    /// Interaction logic for ButtonWithIcon.xaml
    /// </summary>
    public partial class ButtonWithIcon : Button
    {
        public ButtonWithIcon()
        {
            InitializeComponent();
            Label = "Label";
            Text = "Text";
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ButtonWithIcon),
                new FrameworkPropertyMetadata(LabelPropertyChangedCallback));

        private static void LabelPropertyChangedCallback(DependencyObject controlInstance, DependencyPropertyChangedEventArgs args)
        {
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ButtonWithIcon),
                new FrameworkPropertyMetadata(TextPropertyChangedCallback));

        private static void TextPropertyChangedCallback(DependencyObject controlInstance,
            DependencyPropertyChangedEventArgs args)
        {
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(ButtonWithIcon.TextProperty, value); }
        }
    }
}
