﻿using Npgsql;
using PcBuilder.Classes;
using PcBuilder.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PcBuilder.Pages
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string current_page = "";

        private string selectedCpu = "";
        private string selectedMotherboard = "";
        private string selectedPowerSupply = "";
        private string selectedCase = "";
        private string selectedVideoCard = "";
        private string selectedCpuCooler = "";
        private string selectedMemory = "";
        private string selectedInternalHardDrive = "";
        private string selectedOs = "";

        private static readonly Dictionary<string, string> columnTranslations = new Dictionary<string, string>
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

        public void cpu_click(object sender, EventArgs e)
        {
            DisplayTableData("cpu");
            current_page = "cpu";
            DescriptionTextBox.Text = "";
            //cpu_btn.Background = new SolidColorBrush(Colors.Green);
        }

        public void motherboard_click(object sender, EventArgs e)
        {
            DisplayTableData("motherboard");
            current_page = "motherboard";
            DescriptionTextBox.Text = "";
            //motherboard_btn.Background = new SolidColorBrush(Colors.Green);
        }

        public void power_supply_click(object sender, EventArgs e)
        {
            DisplayTableData("power_supply");
            current_page = "power_supply";
            DescriptionTextBox.Text = "";
            //power_supply_btn.Background = new SolidColorBrush(Colors.Green);
        }

        public void case_click(object sender, EventArgs e)
        {
            DisplayTableData("case");
            current_page = "case";
            DescriptionTextBox.Text = "";
            //case_btn.Background = new SolidColorBrush(Colors.Green);
        }

        public void video_card_click(object sender, EventArgs e)
        {
            DisplayTableData("video_card");
            current_page = "video_card";
            DescriptionTextBox.Text = "";
            //video_card_btn.Background = new SolidColorBrush(Colors.Green);
        }

        public void cpu_cooler_click(object sender, EventArgs e)
        {
            DisplayTableData("cpu_cooler");
            current_page = "cpu_cooler";
            DescriptionTextBox.Text = "";
            //cpu_cooler_btn.Background = new SolidColorBrush(Colors.Green);
        }

        public void memory_click(object sender, EventArgs e)
        {
            DisplayTableData("memory");
            current_page = "memory";
            DescriptionTextBox.Text = "";
            //memory_btn.Background = new SolidColorBrush(Colors.Green);
        }

        public void internal_hard_drive_click(object sender, EventArgs e)
        {
            DisplayTableData("internal_hard_drive");
            current_page = "internal_hard_drive";
            DescriptionTextBox.Text = "";
            //internal_hard_drive_btn.Background = new SolidColorBrush(Colors.Green);
        }

        public void os_click(object sender, EventArgs e)
        {
            DisplayTableData("os");
            current_page = "os";
            DescriptionTextBox.Text = "";
            //os_btn.Background = new SolidColorBrush(Colors.Green);
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
        public void open_cart(object sender, EventArgs e)
        {
            var Text = ComplectTextBox.Text;
            var CartWindow = new CartWindow();
            try
            {
                CartWindow.ShowCart(Text);
                CartWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        public void open_search(object sender, EventArgs args)
        {
            var parserWindow = new ParserWindow();
            try
            {
                if (ListTextBox.SelectedItem != null)
                {
                    string productName = ListTextBox.SelectedItem.ToString();
                    string itemName = CreateUrl(productName);

                    Uri url = new Uri(itemName);
                    parserWindow.OpenPage(url);
                    parserWindow.Show();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите элемент из списка.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        public string CreateUrl(string productName)
        {
            string baseUrl = "https://www.dns-shop.ru/search/";
            string searchUrl = $"{baseUrl}?q={Uri.EscapeDataString(productName.Trim())}";
            return searchUrl;
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

                                for (int i = 1; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    if (reader[i] != DBNull.Value && columnTranslations.ContainsKey(columnName))
                                    {
                                        description.AppendLine($"{columnTranslations[columnName]}: {reader[i].ToString()}");
                                    }
                                }

                                descriptions.Add(description.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }

            return (names.ToArray(), descriptions.ToArray());
        }

        private void ListTextBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListTextBox.SelectedItem != null)
            {
                int index = ListTextBox.SelectedIndex;

                var descriptions = GetTableData(current_page).descriptions;

                if (index >= 0 && index < descriptions.Length)
                {
                    DescriptionTextBox.Text = descriptions[index];
                }
                else
                {
                    MessageBox.Show("Описание для выбранного элемента отсутствует.");
                }
            }
        }
        private void add_to_tomplect_click(object sender, RoutedEventArgs e)
        {
            if (ListTextBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите товар из списка.");
                return;
            }

            string itemName = ListTextBox.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(itemName))
            {
                switch (current_page)
                {
                    case "cpu":
                        selectedCpu = itemName;
                        cpu_btn.Background = new SolidColorBrush(Colors.Green); // Изменение цвета кнопки
                        break;
                    case "motherboard":
                        selectedMotherboard = itemName;
                        motherboard_btn.Background = new SolidColorBrush(Colors.Green);
                        break;
                    case "power_supply":
                        selectedPowerSupply = itemName;
                        power_supply_btn.Background = new SolidColorBrush(Colors.Green);
                        break;
                    case "case":
                        selectedCase = itemName;
                        case_btn.Background = new SolidColorBrush(Colors.Green);
                        break;
                    case "video_card":
                        selectedVideoCard = itemName;
                        video_card_btn.Background = new SolidColorBrush(Colors.Green);
                        break;
                    case "cpu_cooler":
                        selectedCpuCooler = itemName;
                        cpu_cooler_btn.Background = new SolidColorBrush(Colors.Green);
                        break;
                    case "memory":
                        selectedMemory = itemName;
                        memory_btn.Background = new SolidColorBrush(Colors.Green);
                        break;
                    case "internal_hard_drive":
                        selectedInternalHardDrive = itemName;
                        internal_hard_drive_btn.Background = new SolidColorBrush(Colors.Green);
                        break;
                    case "os":
                        selectedOs = itemName;
                        os_btn.Background = new SolidColorBrush(Colors.Green);
                        break;
                    default:
                        MessageBox.Show("Неизвестная категория.");
                        return;
                }
                UpdateComplectTextBox();
            }
        }
        private void UpdateComplectTextBox()
        {
            ComplectTextBox.Clear();
            if (!string.IsNullOrEmpty(selectedCpu)) ComplectTextBox.AppendText("Процессор: " + selectedCpu + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedMotherboard)) ComplectTextBox.AppendText("Материнская плата: " + selectedMotherboard + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedPowerSupply)) ComplectTextBox.AppendText("Блок питания: " + selectedPowerSupply + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedCase)) ComplectTextBox.AppendText("Корпус: " + selectedCase + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedVideoCard)) ComplectTextBox.AppendText("Видеокарта: " + selectedVideoCard + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedCpuCooler)) ComplectTextBox.AppendText("Охлаждение: " + selectedCpuCooler + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedMemory)) ComplectTextBox.AppendText("Память: " + selectedMemory + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedInternalHardDrive)) ComplectTextBox.AppendText("Внутренний жесткий диск: " + selectedInternalHardDrive + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedOs)) ComplectTextBox.AppendText("ОС: " + selectedOs + Environment.NewLine);
        }
    }
}
