using PcBuilder.Database;
using PcBuilder.Pages;
using System.Windows;

namespace PcBuilder.Classes
{
    public class App : Application
    {
        [STAThread]
        public static void Main()
        {
            DatabaseInitializer dbInitializer = new DatabaseInitializer(Config.connectionString);
            // dbInitializer.CreateTables(); // если нужно

            var authWindow = new Auth();
            bool? authResult = authWindow.ShowDialog();

            if (authResult == true)
            {
                var mainWindow = new MainWindow(authWindow.autoCheckEnabled);
                var app = new App();
                app.Run(mainWindow);
            }
        }
    }
}
