using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PcBuilder;
using PcBuilder.Classes;
using System.Windows.Documents;

namespace PcBuilder.Pages
{
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
        }

        public void ShowCart(string Content)
        {
            CartTextBox.Text = Content;
        }

        public void check_compatibility(object sender, EventArgs args)
        {
            // Для выборки названий компонентов из корзины
            string _cpu = GetComponentName("Процессор");
            string _motherboard = GetComponentName("Материнская плата");
            string _power = GetComponentName("Блок питания");
            string _case = GetComponentName("Корпус");
            string _gpu = GetComponentName("Видеокарта");
            string _cooler = GetComponentName("Охлаждение");
            string _ram = GetComponentName("Память");
            string _hdd = GetComponentName("Внутренний жесткий диск");
            string _os = GetComponentName("ОС");

            if (string.IsNullOrWhiteSpace(_cpu) ||
                string.IsNullOrWhiteSpace(_motherboard) ||
                string.IsNullOrWhiteSpace(_power) ||
                string.IsNullOrWhiteSpace(_case) ||
                string.IsNullOrWhiteSpace(_gpu) ||
                string.IsNullOrWhiteSpace(_cooler) ||
                string.IsNullOrWhiteSpace(_ram) ||
                string.IsNullOrWhiteSpace(_hdd) ||
                string.IsNullOrWhiteSpace(_os))
            {
                MessageBox.Show("Вы выбрали не все компоненты!");
            }
            else
            {
                try
                {
                    // Для поиска необходимых параметров в БД
                    string cpu_socket = GetParamFromDatabase("socket", "cpu", _cpu);
                    string motherboard_socket = GetParamFromDatabase("socket", "motherboard", _motherboard);
                    string case_type = GetParamFromDatabase("type", "case", _case);    
                    string motherboard_type = GetParamFromDatabase("type", "motherboard", _motherboard);
                    //string gpu_memory = GetParamFromDatabase("memory", "video_card", _gpu);
                    //int wattage = int.Parse(GetParamFromDatabase("wattage", "power_supply", _power));

                    // Проверка совместимости сокетов
                    if (cpu_socket != motherboard_socket)
                    {
                        ErrorsTextBox.Text += "Сокеты процессора и материнской платы отличаются.\n";
                    }
                    if (motherboard_type != case_type)
                    {
                        ErrorsTextBox.Text += "Формфактор блока питания и корпуса не совпадают.\n";
                    }


                    //// Проверка мощности блока питания
                    //var gpuRequirements = new Dictionary<int, int>
                    //    {
                    //        { 4, 450 },
                    //        { 6, 550 },
                    //        { 8, 650 },
                    //        { 12, 750 },
                    //        { 16, 850 },
                    //        { 18, 950 },
                    //        { 24, 1000 }
                    //    };

                    //if (gpuRequirements.ContainsKey(int.Parse(gpu_memory)) && wattage < gpuRequirements[int.Parse(gpu_memory)])
                    //{
                    //    ErrorsTextBox.Text += $"Мощность блока питания слишком маленькая для видеокарты с {int.Parse(gpu_memory)} ГБ памяти.\n";
                    //}
                }
                catch (FormatException ex)
                {
                    MessageBox.Show($"Ошибка формата данных: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
            }
        }
        public string GetParamFromDatabase(string param, string table, string product)
        {
            using (var connection = new NpgsqlConnection(Config.connectionString))
            {
                connection.Open();

                string query = $@"SELECT {param} FROM ""{table}"" WHERE name = @Product";

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Product", product);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string GetComponentName(string componentType)
        {
            string cartContent = CartTextBox.Text;

            string searchPattern = $@"{componentType}:(.*)";

            var match = System.Text.RegularExpressions.Regex.Match(cartContent, searchPattern);

            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return string.Empty;
        }
    }
}
