using Npgsql;
using PcBuilder.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PcBuilder.Pages
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string current_page = "";

        public void cpu_click(object sender, EventArgs e)
        {
            DisplayTableData("cpu");
            current_page = "cpu";
            DescriptionTextBox.Text = "";
        }

        public void motherboard_click(object sender, EventArgs e)
        {
            DisplayTableData("motherboard");
            current_page = "motherboard";
            DescriptionTextBox.Text = "";
        }

        public void power_supply_click(object sender, EventArgs e)
        {
            DisplayTableData("power_supply");
            current_page = "power_supply";
            DescriptionTextBox.Text = "";
        }

        public void case_click(object sender, EventArgs e)
        {
            DisplayTableData("case");
            current_page = "case";
            DescriptionTextBox.Text = "";
        }

        public void video_card_click(object sender, EventArgs e)
        {
            DisplayTableData("video_card");
            current_page = "video_card";
            DescriptionTextBox.Text = "";
        }

        public void cpu_cooler_click(object sender, EventArgs e)
        {
            DisplayTableData("cpu_cooler");
            current_page = "cpu_cooler";
            DescriptionTextBox.Text = "";
        }

        public void memory_click(object sender, EventArgs e)
        {
            DisplayTableData("memory");
            current_page = "memory";
            DescriptionTextBox.Text = "";
        }

        public void internal_hard_drive_click(object sender, EventArgs e)
        {
            DisplayTableData("internal_hard_drive");
            current_page = "internal_hard_drive";
            DescriptionTextBox.Text = "";
        }

        public void os_click(object sender, EventArgs e)
        {
            DisplayTableData("os");
            current_page = "os";
            DescriptionTextBox.Text = "";
        }

        private void DisplayTableData(string table)
        {
            var data = GetTableData(table);
            ListTextBox.Items.Clear();

            foreach (var name in data.names)
            {
                ListTextBox.Items.Add(name);
            }
        }

        private (string[] names, string[] descriptions) GetTableData(string table)
        {
            List<string> names = new List<string>();
            List<string> descriptions = new List<string>();

            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();

                    string tableName = table == "case" ? "\"case\"" : table;

                    string query = $"SELECT * FROM {tableName}";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                names.Add(reader["name"].ToString());
                                StringBuilder description = new StringBuilder();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (reader.GetName(i) != "name")
                                    {
                                        description.Append($"{reader.GetName(i)}: {reader[i]} ");
                                    }
                                }

                                descriptions.Add(description.ToString().Trim());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                names.Clear();
                descriptions.Clear();
                names.Add($"Ошибка: {ex.Message}");
                descriptions.Add("");
            }

            return (names.ToArray(), descriptions.ToArray());
        }

        private void ListTextBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListTextBox.SelectedItem != null)
            {
                string selectedName = ListTextBox.SelectedItem.ToString();
                var description = GetProductDescription(selectedName);
                DescriptionTextBox.Text = description;
            }
        }
        private string GetProductDescription(string productName)
        {
            string description = "";

            var columnTranslations = new Dictionary<string, string>
    {
        { "price", "Цена" },
        { "chipset", "Чипсет" },
        { "memory", "Память" },
        { "core_clock", "Частота ядра" },
        { "boost_clock", "Частота в турбо-режиме" },
        { "color", "Цвет" },
        { "length", "Длина" },
        { "core_count", "Количество ядер" },
        { "tdp", "Тепловыделение" },
        { "graphics", "Встроенная графическое ядро" },
        { "smt", "Многопоточность" },
        { "socket", "Сокет" },
        { "form_factor", "Форм-фактор" },
        { "max_memory", "Максимальное количество памяти" },
        { "memory_slots", "Слотов для оперативной памяти" },
        { "type", "Тип" },
        { "efficiency", "Сертификат" },
        { "wattage", "Мощность" },
        { "modular", "Модульный" },
        { "rpm", "Число оборотов" },
        { "noise_level", "Уровень шума" },
        { "size", "Длина" },
        { "speed", "Скорость" },
        { "modules", "Количество в комплекте" },
        { "price_per_gb", "Цена за 1ГБ" },
        { "first_word_latency", "Тактовая частота" },
        { "cas_latency", "Количество тактов" },
        { "Mode", "Разрядность" },
        { "side_panel", "Боковая панель"},
    };

            // Фильтры, которые не нужно выводить
            var excludedColumns = new List<string> { "id", "name", "internal_35_bays", "external_volume", "side_panel", "psu" };

            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT * FROM \"{current_page}\" WHERE name = @name";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", productName);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                StringBuilder descriptionBuilder = new StringBuilder();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);

                                    if (!excludedColumns.Contains(columnName))
                                    {
                                        string translatedColumnName = columnTranslations.ContainsKey(columnName) ? columnTranslations[columnName] : columnName;

                                        descriptionBuilder.Append($"{translatedColumnName}: {reader[i]} \n");
                                    }
                                }

                                description = descriptionBuilder.ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                description = $"Ошибка при получении данных: {ex.Message}";
            }

            return description;
        }
    }
}
