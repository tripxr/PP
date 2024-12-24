using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace MauiApp3;

public partial class AdminPage : ContentPage
{
    private ObservableCollection<Violation> violations;
    public AdminPage()
	{
		InitializeComponent();
        violations = new ObservableCollection<Violation>();
        violationsListView.ItemsSource = violations;
        LoadViolations();
    }

    private async void LoadViolations()
    {
        string connectionString = "Server=sql6033.site4now.net;Database=db_aafddd_kyrs;User Id=db_aafddd_kyrs_admin;Password=kyrsovik23";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT ViolationId, Description, Date FROM Violations";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    violations.Clear();
                    while (await reader.ReadAsync())
                    {
                        violations.Add(new Violation
                        {
                            ViolationId = reader.GetInt32(0),
                            Description = reader.GetString(1),
                            Date = reader.GetDateTime(2) // Теперь типы совпадают
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить нарушения: {ex.Message}", "OK");
        }
    }

    private async void OnAddViolationClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddViolationPage());
    }

    private async void OnViewAppealsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProcessAppealPage());
    }

    private void OnViolationSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            // Действия при выборе нарушения, если нужно
            var selectedViolation = (Violation)e.SelectedItem;
            Console.WriteLine($"Выбрано нарушение: {selectedViolation.Description}");
        }
    }

       public class Violation
    {
        public int ViolationId { get; set; }
        public int UserId { get; set; } // Добавляем это свойство
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int SeverityLevel { get; set; }
        public int ViolationCount { get; set; }
    }
    //private void OnViolationSelected(object sender, SelectedItemChangedEventArgs e)
    //{
    //    if (e.SelectedItem is Violation selectedViolation)
    //    {
    //        // Действия с выбранным нарушением
    //        DisplayAlert("Выбрано нарушение", $"Описание: {selectedViolation.Description}", "OK");
    //    }

    //// Снять выделение
    //((ListView)sender).SelectedItem = null;
    //}
}