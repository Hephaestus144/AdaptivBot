using AutoIt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace AdaptivBot
{
    public static class JavaScriptUtils
    {
        private static readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;

        public static void JavaScriptErrorDialogFound()
        {
            for (var i = 0; i < 15; i++)
            {
                AutoItX.Sleep(100);
                if (AutoItX.WinExists("Script Error") != 0)
                {
                    _window.Dispatcher.Invoke((() =>
                    {
                        _window.Logger.DontPanicErrorText = $"JavaScript error caught, restarting extraction...";
                    }));
                    AutoItX.WinActivate("Script Error");
                    AutoItX.Send("!y");
                    throw new Exception();
                }
            }
        }
    }
}
