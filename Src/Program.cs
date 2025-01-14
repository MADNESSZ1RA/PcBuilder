using PcBuilder.Database;
using PcBuilder.Pages;
using System.Windows;
using System.Diagnostics;

namespace PcBuilder.Classes
{
    public class App : Application 
    {
        [STAThread]
        public static void Main()
        {

            DatabaseInitializer dbInitializer = new DatabaseInitializer(Config.connectionString);
            //dbInitializer.CreateTables(); // Для создания таблиц
            //dbInitializer.InsertData(); // Для заполнения таблиц
            //dbInitializer.DropDataTable(); // Для удаления таблиц


            var app = new App();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            app.Run(new MainWindow());


        }
    } 
}