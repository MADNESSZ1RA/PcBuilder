using System;
using System.Windows;
using Npgsql;
using PcBuilder.Classes;

namespace PcBuilder.Pages
{
    public partial class Auth : Window
    {
        // добавь это свойство, чтобы передать значение
        public bool autoCheckEnabled { get; private set; }

        public Auth()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string pass = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            try
            {
                using (var connection = new NpgsqlConnection(Config.connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE login = @l AND password = @p";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@l", login);
                        cmd.Parameters.AddWithValue("@p", pass);

                        long count = (long)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            var result = MessageBox.Show(
                                "Включить режим автоматической проверки совместимости?",
                                "Режим совместимости",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question);

                            autoCheckEnabled = (result == MessageBoxResult.Yes);

                            // установка DialogResult, окно закроется автоматически
                            DialogResult = true;
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при авторизации: " + ex.Message);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new Registration();
            regWindow.ShowDialog();
        }
    }
}
