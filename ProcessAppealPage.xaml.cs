using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Microsoft.Maui.Controls;


namespace MauiApp3;

public partial class ProcessAppealPage : ContentPage
{
    private ObservableCollection<Appeal> appeals = new ObservableCollection<Appeal>();
    private const string connectionString = "Server=sql6033.site4now.net;Database=db_aafddd_kyrs;User Id=db_aafddd_kyrs_admin;Password=kyrsovik23";
    public ProcessAppealPage()
    {
		InitializeComponent();
        LoadAppeals();
        appealsListView.ItemsSource = appeals;
    }
    private async void LoadAppeals()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT AppealId, ViolationId, AppealText, Status, DateSubmitted FROM Appeals";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    appeals.Clear();
                    while (await reader.ReadAsync())
                    {
                        appeals.Add(new Appeal
                        {
                            AppealId = reader.GetInt32(0),
                            ViolationId = reader.GetInt32(1),
                            AppealText = reader.GetString(2),
                            Status = reader.GetString(3),
                            DateSubmitted = reader.GetDateTime(4)
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить апелляции: {ex.Message}", "OK");
        }
    }

    private async void OnApproveClicked(object sender, EventArgs e)
    {
        if (appealsListView.SelectedItem is Appeal selectedAppeal)
        {
            await UpdateAppealStatus(selectedAppeal.AppealId, "Approved");
        }
        else
        {
            await DisplayAlert("Ошибка", "Выберите апелляцию для одобрения.", "OK");
        }
    }

    private async void OnRejectClicked(object sender, EventArgs e)
    {
        if (appealsListView.SelectedItem is Appeal selectedAppeal)
        {
            await UpdateAppealStatus(selectedAppeal.AppealId, "Rejected");
        }
        else
        {
            await DisplayAlert("Ошибка", "Выберите апелляцию для отклонения.", "OK");
        }
    }

    private async Task UpdateAppealStatus(int appealId, string status)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE Appeals SET Status = @Status WHERE AppealId = @AppealId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@AppealId", appealId);

                    await command.ExecuteNonQueryAsync();
                }
            }

            await DisplayAlert("Успех", $"Статус апелляции обновлен на \"{status}\".", "OK");
            LoadAppeals(); // Обновляем список после изменения
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось обновить статус: {ex.Message}", "OK");
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}

public class Appeal
{
    public int AppealId { get; set; }
    public int ViolationId { get; set; }
    public string AppealText { get; set; }
    public string Status { get; set; }
    public DateTime DateSubmitted { get; set; }
}

