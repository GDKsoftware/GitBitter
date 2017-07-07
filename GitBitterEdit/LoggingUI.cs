namespace GitBitterEdit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GitBitterLib;
    using System.Threading;
    using System.Windows;

    public class LoggingUI: IGitBitterLogging
    {
        private LoggingForm form = null;
        private List<string> collectedLogging = null;
        private TaskScheduler mainThreadScheduler = null;

        public LoggingUI(TaskScheduler AMainThreadScheduler)
        {
            collectedLogging = new List<string>();

            mainThreadScheduler = AMainThreadScheduler;

            form = new LoggingForm();

            Application.Current.MainWindow.Closed += (object sender, EventArgs e) =>
            {
                if (form != null) form.Close();
            };
        }

        private void ExecuteInMainContext(Action action)
        {
            Task task = new Task(action);
            task.Start(mainThreadScheduler);
        }

        public void Add(string AMessage, LoggingLevel ALevel, string AModule)
        {
            collectedLogging.Add("[" + AModule + "] " + AMessage.Trim());

            ExecuteInMainContext(() =>
            {
                if (!form.IsVisible)
                {
                    try
                    {
                        form.Show();
                    }
                    catch (Exception)
                    {
                        form = new LoggingForm();
                        form.Show();
                    }
                }

                form.SetList(collectedLogging);
            });
        }
    }
}
