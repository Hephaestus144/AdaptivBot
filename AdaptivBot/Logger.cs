using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;


namespace AdaptivBot
{
    // TODO: Convert this to a singleton.
    public class Logger
    {
        private string _text = "";
        public readonly RichTextBox RtbLogger;

        public void HorizontalLine(Brush color, char lineCharacter = '-', int lineLength = 97)
        {
            var horizontalLine = new Run(new string(lineCharacter, lineLength))
            {
                FontWeight = FontWeights.Bold,
                Foreground = color
            };

            var paragraph = new Paragraph();
            paragraph.Inlines.Add(horizontalLine);
            RtbLogger.Document.Blocks.Add(paragraph);
        }

        public void DashedHorizontalLine(Brush color)
        {
            var dashedLine = string.Concat(Enumerable.Repeat("-  ", 43));
            var horizontalLine = new Run(dashedLine)
            {
                FontWeight = FontWeights.Bold,
                Foreground = color
            };

            var paragraph = new Paragraph();
            paragraph.Inlines.Add(horizontalLine);
            RtbLogger.Document.Blocks.Add(paragraph);
        }

        public void NewProcess(string message)
        {
            DashedHorizontalLine(Brushes.LawnGreen);

            var timeStamp = new Run($"{DateTime.Now:hh:mm:ss}:  ")
            {
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LawnGreen,
                FontFamily = new FontFamily("Courier")
            };

            var messageRun = new Run($"{message}")
            {
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LawnGreen,
                FontFamily = new FontFamily("Courier")
            };

            var paragraph = new Paragraph();
            paragraph.Inlines.Clear();
            paragraph.Inlines.Add(timeStamp);
            paragraph.Inlines.Add(messageRun);
            RtbLogger.Document.Blocks.Add(paragraph);
            RtbLogger.ScrollToEnd();
        }

        public void NewExtraction(string message)
        {
            HorizontalLine(Brushes.LawnGreen);

            var timeStamp = new Run($"{DateTime.Now:hh:mm:ss}:  ")
            {
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LawnGreen,
                FontFamily = new FontFamily("Courier")
            };

            var messageRun = new Run($"{message}")
            {
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.LawnGreen,
                FontFamily = new FontFamily("Courier")
            };

            var paragraph = new Paragraph();
            paragraph.Inlines.Clear();
            paragraph.Inlines.Add(timeStamp);
            paragraph.Inlines.Add(messageRun);
            RtbLogger.Document.Blocks.Add(paragraph);
            RtbLogger.ScrollToEnd();
        }


        public void ExtractionComplete(string message)
        {
            DashedHorizontalLine(Brushes.LawnGreen);

            var completeRun = new Run($"\n>>>>>>>>  Complete : ")
            {
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LawnGreen,
                FontFamily = new FontFamily("Courier")
            };

            
            var messageRun = new Run($" {message} ")
            {
                Foreground = Brushes.LawnGreen,
                FontFamily = new FontFamily("Courier")
            };

            var chevrons = new Run("  <<<<<<<<")
            {
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LawnGreen,
                FontFamily = new FontFamily("Courier")
            };

            var paragraph = new Paragraph();
            paragraph.Inlines.Clear();
            paragraph.Inlines.Add(completeRun);
            paragraph.Inlines.Add(messageRun);
            paragraph.Inlines.Add(chevrons);
            RtbLogger.Document.Blocks.Add(paragraph);
            RtbLogger.ScrollToEnd();
        }

        public string WarningText
        {
            get => _text;
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
                RtbLogger.ScrollToEnd();
            }
        }


        public string ErrorText
        {
            get => _text;
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
                RtbLogger.Document.Blocks.Add(paragraph);
                RtbLogger.ScrollToEnd();
            }
        }


        public string DontPanicErrorText
        {
            get => _text;
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

                var dontPanic = new Run($"DON'T PANIC. This error has been 'gracefully' dealt with.")
                {
                    FontWeight = FontWeights.ExtraBold,
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
                paragraph.Inlines.Add(dontPanic);
                loggerText.Blocks.Add(paragraph);
                RtbLogger.Document.Blocks.Add(paragraph);
                RtbLogger.ScrollToEnd();

                paragraph = new Paragraph();
                paragraph.Inlines.Add(timeStamp);
                paragraph.Inlines.Add(message);
                loggerText.Blocks.Add(paragraph);
                RtbLogger.Document.Blocks.Add(paragraph);
                RtbLogger.ScrollToEnd();
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
                RtbLogger.ScrollToEnd();
            }
        }

        public string WarningTextWithoutTime
        {
            get => _text;
            set
            {
                var loggerText = new FlowDocument();
                _text += "\n" + value;

                var message = new Run($"{value}")
                {
                    Foreground = Brushes.Orange,
                    FontFamily = new FontFamily("Courier")
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(message);
                loggerText.Blocks.Add(paragraph);
                RtbLogger.Document.Blocks.Add(paragraph);
                RtbLogger.ScrollToEnd();
            }
        }


        public string ErrorTextWithoutTime
        {
            get => _text;
            set
            {
                var loggerText = new FlowDocument();
                _text += "\n" + value;
                
                var message = new Run($"{value}")
                {
                    Foreground = Brushes.Red,
                    FontFamily = new FontFamily("Courier")
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(message);
                loggerText.Blocks.Add(paragraph);
                RtbLogger.Document.Blocks.Add(paragraph);
                RtbLogger.ScrollToEnd();
            }
        }

        public string OkayTextWithoutTime
        {
            get => _text;
            set
            {
                var loggerText = new FlowDocument();
                _text += "\n" + value;

                var message = new Run($"{value}")
                {
                    Foreground = Brushes.LawnGreen,
                    FontFamily = new FontFamily("Courier")
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(message);
                loggerText.Blocks.Add(paragraph);
                RtbLogger.Document.Blocks.Add(paragraph);
                RtbLogger.ScrollToEnd();
            }
        }


        public Logger(RichTextBox rtbLogger)
        {
            RtbLogger = rtbLogger;
        }
    }
}
