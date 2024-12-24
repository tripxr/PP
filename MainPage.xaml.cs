
using System;

using System.Data.SqlClient;

namespace MauiApp3
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = usernameEntry.Text;
            string password = passwordEntry.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка", "Пожалуйста, введите имя пользователя и пароль.", "OK");
                return;
            }

            string role = await AuthenticateUser(username, password);

            if (role != null)
            {
                // Переход на соответствующую форму в зависимости от роли
                if (role == "Courier")
                {
                    // Переход на форму для курьера
                    await Navigation.PushAsync(new CourierPage()); // Страница для курьера
                }
                else if (role == "Admin")
                {
                    // Переход на форму для администратора
                    await Navigation.PushAsync(new AdminPage()); // Страница для администратора
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Неверное имя пользователя или пароль.", "OK");
            }
        }

        // Метод для аутентификации пользователя с учетом роли
        private async Task<string> AuthenticateUser(string username, string password)
        {
            try
            {
                string connectionString = "Server=sql6033.site4now.net;Database=db_aafddd_kyrs;User Id=db_aafddd_kyrs_admin;Password=kyrsovik23";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Запрос для получения роли пользователя
                    string query = "SELECT role FROM Users WHERE username = @username AND password = @password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        var result = await command.ExecuteScalarAsync();

                        // Если роль найдена, возвращаем ее
                        if (result != null)
                        {
                            return result.ToString(); // Возвращаем роль (Courier или Admin)
                        }
                        else
                        {
                            return null; // Если пользователя с такими данными нет
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", "Ошибка подключения к базе данных: " + ex.Message, "OK");
                return null;
            }
        }
    }

}
