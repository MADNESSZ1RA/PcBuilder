using Npgsql;
using PcBuilder.Classes;
using PcBuilder.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PcBuilder.Pages
{
    public partial class MainWindow : Window
    {
        // Флаг, отвечающий за включение / выключение режима автопроверки.
        public bool _autoCheckEnabled;

        public MainWindow()
        {
            InitializeComponent();
            _autoCheckEnabled = false; // по умолчанию
        }

        public MainWindow(bool autoCheckEnabled)
        {
            InitializeComponent();
            _autoCheckEnabled = autoCheckEnabled;
        }

        public string current_page = "";

        // Выбранные комплектующие
        private string selectedCpu = "";
        private string selectedMotherboard = "";
        private string selectedPowerSupply = "";
        private string selectedCase = "";
        private string selectedVideoCard = "";
        private string selectedCpuCooler = "";
        private string selectedMemory = "";
        private string selectedInternalHardDrive = "";
        private string selectedOs = "";

        // Локальный кэш для уже извлечённой из БД информации (чтобы не дергать БД каждый раз)
        // Но для упрощения примера можно дергать напрямую.
        // В данном примере храним ничего не будем, просто каждый раз читаем БД.

        /// <summary>
        /// Словарь перевода названий столбцов на русский
        /// (только для отображения в DescriptionTextBox)
        /// </summary>
        private static readonly Dictionary<string, string> columnTranslations = new Dictionary<string, string>
        {
            { "price", "Цена" },
            { "chipset", "Чипсет" },
            { "memory", "Память (видеокарта)" },
            { "core_clock", "Частота ядра" },
            { "boost_clock", "Частота в турбо-режиме" },
            { "color", "Цвет" },
            { "length", "Длина" },
            { "core_count", "Количество ядер" },
            { "tdp", "Тепловыделение (TDP)" },
            { "graphics", "Встроенное граф. ядро" },
            { "smt", "Многопоточность" },
            { "socket", "Сокет" },
            { "form_factor", "Форм-фактор" },
            { "max_memory", "Максимальная память" },
            { "memory_slots", "Слотов для ОЗУ" },
            { "type", "Тип" },
            { "efficiency", "Сертификат" },
            { "wattage", "Мощность (W)" },
            { "modular", "Модульный" },
            { "rpm", "Обороты (RPM)" },
            { "noise_level", "Уровень шума" },
            { "size", "Габарит / Длина" },
            { "speed", "Скорость (ОЗУ)" },
            { "modules", "Модули (кол-во x объем)" },
            { "price_per_gb", "Цена за 1ГБ" },
            { "first_word_latency", "Тактовая частота" },
            { "cas_latency", "Количество тактов" },
            { "Mode", "Разрядность (ОС)" },
            { "side_panel", "Боковая панель"},
            { "capacity", "Объём (накопитель)" },
            { "cache", "Кэш (накопитель)" },
            { "interface", "Интерфейс (накопитель)" },
            { "psu", "PSU (в корпусе)" },
            { "external_volume", "Внешние отсеки" },
            { "internal_35_bays", "Внутренние 3.5\" отсеки" },
            { "mode", "Разрядность (ОС)" },
            //{ "max_memory", "Макс. память (ОС)" },
        };

        // -----------------------------------------------
        // Обработчики выбора таблицы (кнопок слева)
        // -----------------------------------------------

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

        // -----------------------------------------------
        // Основной метод отображения (с учётом автопроверки)
        // -----------------------------------------------
        private void DisplayTableData(string table)
        {
            var data = _autoCheckEnabled
                ? GetFilteredData(table)   // Возвращаем ТОЛЬКО совместимые элементы
                : GetTableData(table);     // Возвращаем все элементы

            ListTextBox.Items.Clear();

            foreach (var name in data.names)
            {
                ListTextBox.Items.Add(name);
            }
        }

        // Открытие корзины (просто пример, как у вас было)
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

        // Открытие окна парсинга (пример)
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

        // Формирует URL для DNS
        public string CreateUrl(string productName)
        {
            string baseUrl = "https://www.dns-shop.ru/search/";
            string searchUrl = $"{baseUrl}?q={Uri.EscapeDataString(productName.Trim())}";
            return searchUrl;
        }

        // -----------------------------------------------
        // Получение всех данных из таблицы (без фильтров)
        // -----------------------------------------------
        private (string[] names, string[] descriptions) GetTableData(string table)
        {
            // Вытаскиваем все записи через вспомогательный метод, 
            // а потом формируем из них (string[] names, string[] descriptions)
            var allRows = GetAllRawRecords(table);

            List<string> names = new List<string>();
            List<string> descriptions = new List<string>();

            foreach (var row in allRows)
            {
                string productName = row.ContainsKey("name") ? row["name"].ToString() : "???";
                names.Add(productName);

                // Формируем текстовое описание
                StringBuilder sb = new StringBuilder();
                // Начиная со 2-го столбца (если нужно) или просто по всем:
                foreach (var col in row)
                {
                    if (col.Key == "name") continue; // name уже используем как заголовок

                    if (columnTranslations.ContainsKey(col.Key) && col.Value != null)
                    {
                        sb.AppendLine($"{columnTranslations[col.Key]}: {col.Value}");
                    }
                }
                descriptions.Add(sb.ToString());
            }

            return (names.ToArray(), descriptions.ToArray());
        }

        /// <summary>
        /// Метод, который возвращает только "совместимые" записи из таблицы.
        /// Логика фильтрации зависит от того, какие комплектующие уже выбраны.
        /// </summary>
        private (string[] names, string[] descriptions) GetFilteredData(string table)
        {
            // 1) Сначала получаем ВСЕ записи таблицы (в виде List<Dictionary<string, object>>)
            var allRows = GetAllRawRecords(table);

            // 2) Применяем логику совместимости для каждой записи
            var filtered = new List<Dictionary<string, object>>();

            foreach (var row in allRows)
            {
                if (IsCompatible(row, table))
                {
                    filtered.Add(row);
                }
            }

            // 3) Формируем результат (names[], descriptions[])
            List<string> names = new List<string>();
            List<string> descriptions = new List<string>();

            foreach (var row in filtered)
            {
                string productName = row.ContainsKey("name") ? row["name"].ToString() : "???";
                names.Add(productName);

                StringBuilder sb = new StringBuilder();
                foreach (var col in row)
                {
                    if (col.Key == "name") continue;

                    if (columnTranslations.ContainsKey(col.Key) && col.Value != null)
                    {
                        sb.AppendLine($"{columnTranslations[col.Key]}: {col.Value}");
                    }
                }
                descriptions.Add(sb.ToString());
            }

            return (names.ToArray(), descriptions.ToArray());
        }

        /// <summary>
        /// Основная логика проверки: "row" - это одна строка таблицы "table",
        /// нужно решить, совместима ли она с уже выбранными деталями.
        /// </summary>
        private bool IsCompatible(Dictionary<string, object> row, string table)
        {
            // Пример: если таблица "motherboard", то проверяем,
            // подходит ли она к выбранному CPU по сокету, и т.д.

            switch (table)
            {
                case "motherboard":
                    // 1) Сокет матери должен совпадать с сокетом CPU (если CPU выбран)
                    if (!string.IsNullOrEmpty(selectedCpu))
                    {
                        // Получаем socket у выбранного CPU и у текущей row
                        string cpuSocket = GetCpuField("socket", selectedCpu);
                        string mbSocket = row.ContainsKey("socket") ? row["socket"]?.ToString() : "";
                        if (!string.IsNullOrEmpty(cpuSocket) && cpuSocket != mbSocket)
                            return false; // не совпали сокеты => не подходит
                    }

                    // 2) Если уже выбрана память, проверим, не превышает ли она max_memory матери
                    if (!string.IsNullOrEmpty(selectedMemory))
                    {
                        int totalMemGb = ParseMemoryCapacity(selectedMemory);
                        // max_memory (int) в матери
                        int mbMaxMem = 0;
                        if (row.ContainsKey("max_memory") && row["max_memory"] is int mm)
                            mbMaxMem = mm;
                        // Если вдруг строка - надо сконвертить, но в модели заявлено int
                        if (totalMemGb > 0 && mbMaxMem > 0 && totalMemGb > mbMaxMem)
                            return false; // памяти больше, чем поддерживает материнка
                    }

                    // 3) Если уже выбран корпус, проверим совместимость форм-фактора
                    if (!string.IsNullOrEmpty(selectedCase))
                    {
                        string mbFormFactor = row.ContainsKey("form_factor") ? row["form_factor"]?.ToString() : "";
                        // Получим type корпуса
                        string caseType = GetCaseField("type", selectedCase);
                        // Допустим, у нас мини-словарь совместимости
                        if (!CaseSupportsFormFactor(caseType, mbFormFactor))
                            return false;
                    }

                    // OS / PSU / GPU и т.п. обычно не влияют на выбор матери,
                    // разве что можно выдумать дополнительные проверки, но пока опустим.
                    return true;

                case "case":
                    // 1) Проверим, есть ли выбранная материнка, и совпадает ли форм-фактор
                    if (!string.IsNullOrEmpty(selectedMotherboard))
                    {
                        string mbFormFactor = GetMotherboardField("form_factor", selectedMotherboard);
                        string caseType = row.ContainsKey("type") ? row["type"]?.ToString() : "";
                        if (!CaseSupportsFormFactor(caseType, mbFormFactor))
                            return false;
                    }
                    return true;

                case "power_supply":
                    // Проверим, хватает ли мощности для CPU + некое значение на видеокарту
                    int neededPower = 0;
                    if (!string.IsNullOrEmpty(selectedCpu))
                    {
                        // Возьмем TDP CPU
                        int cpuTdp = GetCpuTdp(selectedCpu);
                        neededPower += cpuTdp; // хотя обычно берут запас
                    }

                    if (!string.IsNullOrEmpty(selectedVideoCard))
                    {
                        // Видеокарты TDP нет, сделаем "плюс 200W", чисто как пример
                        neededPower += 200;
                    }

                    // Возьмём wattage из row
                    int psuWattage = 0;
                    if (row.ContainsKey("wattage") && row["wattage"] is int w)
                        psuWattage = w;

                    // Пример: хотим запас, скажем + 50W
                    neededPower += 50;
                    // Если psuWattage < neededPower => не подходит
                    if (psuWattage < neededPower)
                        return false;

                    return true;

                case "memory":
                    // Проверяем, что если у нас уже выбрана материнка, 
                    // то память не превышает её max_memory.
                    if (!string.IsNullOrEmpty(selectedMotherboard))
                    {
                        int totalMemGb = ParseMemoryCapacity(row); // парсим у текущего варианта памяти
                        int mbMaxMem = GetMotherboardMaxMemory(selectedMotherboard);

                        if (mbMaxMem > 0 && totalMemGb > mbMaxMem)
                            return false;
                    }

                    // Также, если уже выбрана OS, то проверим, не превышаем ли max_memory ОС
                    if (!string.IsNullOrEmpty(selectedOs))
                    {
                        int totalMemGb = ParseMemoryCapacity(row);
                        int osMax = GetOsMaxMemory(selectedOs);
                        if (osMax > 0 && totalMemGb > osMax)
                            return false;
                    }

                    return true;

                case "os":
                    // Если уже выбрана память, и у ОС есть max_memory, 
                    // сравниваем
                    if (!string.IsNullOrEmpty(selectedMemory))
                    {
                        int totalMemGb = ParseMemoryCapacity(selectedMemory);
                        int osMax = 0;
                        if (row.ContainsKey("max_memory") && row["max_memory"] is int mm)
                            osMax = mm;

                        if (osMax > 0 && totalMemGb > osMax)
                            return false;
                    }
                    return true;

                // Для остальных таблиц сделаем заглушку:
                // cpu, video_card, cpu_cooler, internal_hard_drive — сейчас
                // никаких специальных проверок не делаем.
                default:
                    return true;
            }
        }

        // -----------------------------------------------
        // Методы-помощники для выборок и парсинга
        // -----------------------------------------------

        /// <summary>
        /// Возвращает список записей (каждая запись - словарь "columnName" -> object)
        /// для указанной таблицы.
        /// </summary>
        private List<Dictionary<string, object>> GetAllRawRecords(string table)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();
                    string tableName = (table == "case") ? "\"case\"" : table;
                    string query = $"SELECT * FROM {tableName}";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string colName = reader.GetName(i);
                                    object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    row[colName] = value;
                                }
                                result.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Узнать значение поля у CPU по имени процессора
        /// (например, socket, tdp и т.п.)
        /// </summary>
        private string GetCpuField(string fieldName, string cpuName)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT {fieldName} FROM cpu WHERE name = @name LIMIT 1";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", cpuName);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            return result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении поля CPU: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Возвращает целочисленный TDP у процессора (или 0, если не нашли / не смогли распарсить).
        /// </summary>
        private int GetCpuTdp(string cpuName)
        {
            var val = GetCpuField("tdp", cpuName);
            if (int.TryParse(val, out int tdp))
            {
                return tdp;
            }
            return 0;
        }

        /// <summary>
        /// Узнать значение поля у Motherboard по имени
        /// </summary>
        private string GetMotherboardField(string fieldName, string mbName)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT {fieldName} FROM motherboard WHERE name = @name LIMIT 1";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", mbName);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            return result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении поля MB: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Узнать max_memory (int) у выбранной материнки
        /// </summary>
        private int GetMotherboardMaxMemory(string mbName)
        {
            var val = GetMotherboardField("max_memory", mbName);
            if (int.TryParse(val, out int mm))
            {
                return mm;
            }
            return 0;
        }

        /// <summary>
        /// Узнать значение поля у Case по имени
        /// </summary>
        private string GetCaseField(string fieldName, string caseName)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT {fieldName} FROM \"case\" WHERE name = @name LIMIT 1";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", caseName);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            return result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении поля Case: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Проверяет, поддерживает ли данный тип корпуса (например, "Mid Tower")
        /// указанный форм-фактор материнки (например, "ATX").
        /// Тут можно завести словарь с вариантами, либо простое сопоставление по подстроке.
        /// </summary>
        private bool CaseSupportsFormFactor(string caseType, string mbFormFactor)
        {
            if (string.IsNullOrEmpty(caseType) || string.IsNullOrEmpty(mbFormFactor))
                return true; // если одно из полей пустое, пропускаем проверку

            // Простейший "словарь" или логика:
            // Допустим, если материнка = "ATX", то корпус type должен содержать "ATX", "Mid", "Full".
            // Если материнка = "Micro ATX", то корпус может быть "micro", "Mini Tower", и т.д.
            // Ниже чисто пример — реальные соответствия зависят от ваших данных.
            mbFormFactor = mbFormFactor.ToLower();
            caseType = caseType.ToLower();

            if (mbFormFactor.Contains("atx"))
            {
                // тогда caseType может содержать "atx", "mid", "full" и т.д.
                if (caseType.Contains("atx") || caseType.Contains("mid") || caseType.Contains("full"))
                    return true;
                return false;
            }
            if (mbFormFactor.Contains("micro"))
            {
                // тогда корпус должен содержать "micro", "mini" и т.п.
                if (caseType.Contains("micro") || caseType.Contains("mini") || caseType.Contains("atx") || caseType.Contains("mid") || caseType.Contains("full"))
                    return true;
                return false;
            }
            // ...и т.д.
            // Если не знаем, пропускаем
            return true;
        }

        /// <summary>
        /// Узнать у ОС поле max_memory (int)
        /// </summary>
        private int GetOsMaxMemory(string osName)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();
                    string query = $"SELECT max_memory FROM os WHERE name = @name LIMIT 1";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", osName);
                        var result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int maxMem))
                            return maxMem;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении max_memory для ОС: " + ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Парсим поле "modules" у памяти, чтобы понять общий объём (в ГБ).
        /// Если передано имя памяти, мы идём в БД, достаём строку "modules",
        /// а потом парсим.
        /// </summary>
        private int ParseMemoryCapacity(string memoryName)
        {
            // 1) Получаем "modules" из таблицы memory
            string modulesStr = "";
            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();
                    string query = "SELECT modules FROM memory WHERE name = @name LIMIT 1";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", memoryName);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            modulesStr = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении modules: " + ex.Message);
            }

            return ParseMemoryString(modulesStr);
        }

        /// <summary>
        /// Парсим "modules" у конкретной записи (из словаря row), когда делаем фильтр по памяти.
        /// Например, если row["modules"] = "2 x 8GB".
        /// </summary>
        private int ParseMemoryCapacity(Dictionary<string, object> memoryRow)
        {
            if (!memoryRow.ContainsKey("modules") || memoryRow["modules"] == null)
                return 0;

            string modulesStr = memoryRow["modules"].ToString();
            return ParseMemoryString(modulesStr);
        }

        /// <summary>
        /// Собственно разбор строки вида "2 x 8GB", "1 x 16 GB", и т.п.
        /// Возвращает суммарный объём в ГБ (int).
        /// </summary>
        private int ParseMemoryString(string modulesStr)
        {
            if (string.IsNullOrEmpty(modulesStr)) return 0;

            // Допустим, строка "2 x 8GB". 
            // Упрощённо: находим число перед "x" (кол-во модулей), и число перед "GB" (объём каждого модуля).
            // Примерный RegEx: @"(\d+)\s*x\s*(\d+)\s*GB"
            // Учтём, что может быть пробелы, может быть разный регистр и т.д.
            var regex = new Regex(@"(\d+)\s*x\s*(\d+)\s*GB", RegexOptions.IgnoreCase);
            var match = regex.Match(modulesStr);
            if (match.Success)
            {
                // match.Groups[1] - количество модулей
                // match.Groups[2] - размер в ГБ каждого
                if (int.TryParse(match.Groups[1].Value, out int count) &&
                    int.TryParse(match.Groups[2].Value, out int sizeEach))
                {
                    return count * sizeEach;
                }
            }

            // Если RegEx не сработал, можно попытаться найти одно число "16GB":
            // "1 x 16GB" или просто "16GB", и т.д.
            var regex2 = new Regex(@"(\d+)\s*GB", RegexOptions.IgnoreCase);
            var match2 = regex2.Match(modulesStr);
            if (match2.Success)
            {
                if (int.TryParse(match2.Groups[1].Value, out int size))
                {
                    return size; // считаем, что 1 модуль
                }
            }

            // Не смогли распарсить => 0
            return 0;
        }

        // -----------------------------------------------
        // События выбора элемента из списка
        // -----------------------------------------------
        private void ListTextBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListTextBox.SelectedItem != null)
            {
                int index = ListTextBox.SelectedIndex;
                var descriptions = _autoCheckEnabled
                    ? GetFilteredData(current_page).descriptions
                    : GetTableData(current_page).descriptions;

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

        // -----------------------------------------------
        // Добавление выбранного товара к сборке
        // -----------------------------------------------
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
                        cpu_btn.Background = new SolidColorBrush(Colors.Green);
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

        // Обновляем поле со сборкой
        private void UpdateComplectTextBox()
        {
            ComplectTextBox.Clear();
            if (!string.IsNullOrEmpty(selectedCpu))
                ComplectTextBox.AppendText("Процессор: " + selectedCpu + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedMotherboard))
                ComplectTextBox.AppendText("Материнская плата: " + selectedMotherboard + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedPowerSupply))
                ComplectTextBox.AppendText("Блок питания: " + selectedPowerSupply + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedCase))
                ComplectTextBox.AppendText("Корпус: " + selectedCase + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedVideoCard))
                ComplectTextBox.AppendText("Видеокарта: " + selectedVideoCard + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedCpuCooler))
                ComplectTextBox.AppendText("Охлаждение: " + selectedCpuCooler + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedMemory))
                ComplectTextBox.AppendText("Память: " + selectedMemory + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedInternalHardDrive))
                ComplectTextBox.AppendText("Внутренний жесткий диск: " + selectedInternalHardDrive + Environment.NewLine);
            if (!string.IsNullOrEmpty(selectedOs))
                ComplectTextBox.AppendText("ОС: " + selectedOs + Environment.NewLine);
        }
    }
}
