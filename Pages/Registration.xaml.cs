using System;
using System.Windows;
using Npgsql;
using PcBuilder.Classes;

namespace PcBuilder.Pages
{
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        // Нажали "Зарегистрироваться"
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string pass = PasswordBox.Password.Trim();
            string passRepeat = PasswordBoxRepeat.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }
            if (pass != passRepeat)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();

                    // Проверим, нет ли уже пользователя с таким логином
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE login = @l";
                    using (var checkCmd = new NpgsqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@l", login);
                        long count = (long)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует!");
                            return;
                        }
                    }

                    // Добавим новую запись
                    string insertQuery = "INSERT INTO users (login, password) VALUES (@l, @p)";
                    using (var cmd = new NpgsqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@l", login);
                        cmd.Parameters.AddWithValue("@p", pass);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Регистрация успешно выполнена!");
                this.Close(); // Закрываем окно регистрации
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при регистрации: " + ex.Message);
            }
        }
    }
}
