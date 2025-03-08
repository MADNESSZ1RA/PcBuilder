using Npgsql;
using PcBuilder.Classes;
using System;
using System.IO;
using System.Windows;

namespace PcBuilder.Database
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void CreateTables()
        {
            string[] tableCreationScripts =
            {
                PcBuilder.Classes.Models.CpuTable,
                PcBuilder.Classes.Models.MotherboardTable,
                PcBuilder.Classes.Models.MemoryTable,
                PcBuilder.Classes.Models.PowerSupplyTable,
                PcBuilder.Classes.Models.VideoCardTable,
                PcBuilder.Classes.Models.CaseTable,
                PcBuilder.Classes.Models.InternalHardDriveTable,
                PcBuilder.Classes.Models.CpuCoolerTable,
                PcBuilder.Classes.Models.OsTable,
                PcBuilder.Classes.Models.Users
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var script in tableCreationScripts)
                        {
                            using (var command = new NpgsqlCommand(script, connection, transaction))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Все таблицы успешно созданы.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при создании таблиц: {ex.Message}");
                    }
                }
                connection.Close();
            }
        }
        
        public void DropDataTable()
        {
            string[] tables = { "cpu", "motherboard", "memory", "power_supply", "video_card", "case", "internal_hard_drive", "cpu_cooler", "os" };
            foreach (string table in tables)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        connection.Open();
                        string query = $"DROP TABLE \"{table}\"";
                        using (var cmd = new NpgsqlCommand(query, connection))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    MessageBox.Show(reader.GetString(0));
                                }
                            }
                        }
                        connection.Close();
                    }
                } 
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message );
                }
            }
        }
    }
}
