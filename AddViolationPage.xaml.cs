using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
namespace MauiApp3;

public partial class AddViolationPage : ContentPage
{
    private Dictionary<int, string> users;
    public AddViolationPage()
	{
		InitializeComponent();
        LoadUsers();
        violationDatePicker.Date = DateTime.Today;
    }
    private async void LoadUsers()
    {
        users = new Dictionary<int, string>();
        string connectionString = "Server=sql6033.site4now.net;Database=db_aafddd_kyrs;User Id=db_aafddd_kyrs_admin;Password=kyrsovik23";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "SELECT UserId, Username FROM Users";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int userId = reader.GetInt32(0);
                        string username = reader.GetString(1);
                        users.Add(userId, username);
                    }
                }

                foreach (var user in users)
                {
                    userPicker.Items.Add(user.Value);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить пользователей: {ex.Message}", "OK");
            }
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (userPicker.SelectedIndex == -1 || string.IsNullOrEmpty(descriptionEditor.Text) || string.IsNullOrEmpty(severityEntry.Text))
        {
            await DisplayAlert("Ошибка", "Пожалуйста, заполните все поля.", "OK");
            return;
        }

        string description = descriptionEditor.Text;
        int severity = int.Parse(severityEntry.Text);
        DateTime date = violationDatePicker.Date;
        int userId = users.FirstOrDefault(x => x.Value == userPicker.SelectedItem.ToString()).Key;

        string connectionString = "Server=sql6033.site4now.net;Database=db_aafddd_kyrs;User Id=db_aafddd_kyrs_admin;Password=kyrsovik23";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Violations (UserId, Description, Date, SeverityLevel, ViolationCount) VALUES (@UserId, @Description, @Date, @SeverityLevel, 0)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@SeverityLevel", severity);

                    await command.ExecuteNonQueryAsync();
                }

                await DisplayAlert("Успех", "Нарушение успешно добавлено.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось сохранить нарушение: {ex.Message}", "OK");
            }
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

}
