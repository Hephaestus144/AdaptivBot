using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using System.Threading;
using System.Windows.Threading;

namespace AdaptivBot
{
    // TODO: Convert this to a singleton.
    public class Logger
    {
        private string _text = "";
        public readonly RichTextBox RtbLogger;


        public string RunningText
        {
            get { return _text; }
            set
            {
                var loggerText = new FlowDocument();
                _text += "\n" + value;
                var timeStamp = new Run($"{DateTime.Now:hh:mm:ss}:  ")
                {
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Orange,
                    FontFamily = new FontFamily("Courier")
                };

                var message = new Run($"{value}")
                {
                    Foreground = Brushes.Orange,
                    FontFamily = new FontFamily("Courier")
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(timeStamp);
                paragraph.Inlines.Add(message);
                loggerText.Blocks.Add(paragraph);
                RtbLogger.Document.Blocks.Add(paragraph);
            }
        }


        public string ErrorText
        {
            get { return _text; }
            set
            {
                var loggerText = new FlowDocument();
                _text += "\n" + value;
                var timeStamp = new Run($"{DateTime.Now:hh:mm:ss}:  ")
                {
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Red,
                    FontFamily = new FontFamily("Courier")
                };

                var message = new Run($"{value}")
                {
                    Foreground = Brushes.Red,
                    FontFamily = new FontFamily("Courier")
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(timeStamp);
                paragraph.Inlines.Add(message);
                loggerText.Blocks.Add(paragraph);
            }
        }

        public string OkayText
        {
            get => _text;
            set
            {
                var loggerText = new FlowDocument();
                _text += "\n" + value;
                var timeStamp = new Run($"{DateTime.Now:hh:mm:ss}:  ")
                {
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.LawnGreen,
                    FontFamily = new FontFamily("Courier")
                };

                var message = new Run($"{value}")
                {
                    Foreground = Brushes.LawnGreen,
                    FontFamily = new FontFamily("Courier")
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(timeStamp);
                paragraph.Inlines.Add(message);
                loggerText.Blocks.Add(paragraph);
                RtbLogger.Document.Blocks.Add(paragraph);
            }
        }


        public Logger(System.Windows.Controls.RichTextBox rtbLogger)
        {
            RtbLogger = rtbLogger;
        }
    }
}
