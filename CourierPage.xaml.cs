using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
namespace MauiApp3;

public partial class CourierPage : ContentPage
{
	public CourierPage()
	{
		InitializeComponent();
        LoadViolations();
    }
    private List<Violation> violationsList = new List<Violation>();

    // Загрузка нарушений из базы данных
    private async void LoadViolations()
    {
        string connectionString = "Server=sql6033.site4now.net;Database=db_aafddd_kyrs;User Id=db_aafddd_kyrs_admin;Password=kyrsovik23";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = "SELECT ViolationId, Description, Date, SeverityLevel FROM Violations WHERE UserID = @UserID";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", 1); // Тут можно заменить на динамическое значение UserID

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    violationsList.Add(new Violation
                    {
                        ViolationId = reader.GetInt32(0),
                        Description = reader.GetString(1),
                        Date = reader.GetDateTime(2).ToString("dd-MM-yyyy"),
                        SeverityLevel = reader.GetInt32(3)
                    });
                }

                violationsListView.ItemsSource = violationsList;
            }
        }
    }

    // Обработчик нажатия на кнопку "Выговор"
    private async void OnPunishClicked(object sender, EventArgs e)
    {
        if (violationsListView.SelectedItem is Violation selectedViolation)
        {
            string connectionString = "Server=sql6033.site4now.net;Database=db_aafddd_kyrs;User Id=db_aafddd_kyrs_admin;Password=kyrsovik23";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT PunishmentDescription FROM Punishments WHERE SeverityLevel = @SeverityLevel";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SeverityLevel", selectedViolation.SeverityLevel);

                    var punishmentDescription = await command.ExecuteScalarAsync();

                    if (punishmentDescription != null)
                    {
                        await DisplayAlert("Выговор", punishmentDescription.ToString(), "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Штраф для этого уровня не найден.", "OK");
                    }
                }
            }
        }
        else
        {
            await DisplayAlert("Ошибка", "Пожалуйста, выберите нарушение.", "OK");
        }
    }

    // Обработчик нажатия на кнопку "Обжаловать"
    private async void OnAppealClicked(object sender, EventArgs e)
    {
        if (violationsListView.SelectedItem is Violation selectedViolation)
        {
            // Переход на форму обжалования с передачей ViolationId и Description
            await Navigation.PushAsync(new AppealPage(selectedViolation.ViolationId, selectedViolation.Description));
        }
        else
        {
            await DisplayAlert("Ошибка", "Пожалуйста, выберите нарушение для обжалования.", "OK");
        }
    }

    // Обработчик для выбора нарушения
    private void OnViolationSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Можно добавить логику для подсветки или получения данных о выбранном нарушении
    }
}

// Класс для хранения данных нарушения
public class Violation
{
    public int ViolationId { get; set; }
    public string Description { get; set; }
    public string Date { get; set; }
    public int SeverityLevel { get; set; }
}

