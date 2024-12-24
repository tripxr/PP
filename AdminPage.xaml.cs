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
                            Date = reader.GetDateTime(2) // ������ ���� ���������
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", $"�� ������� ��������� ���������: {ex.Message}", "OK");
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
            // �������� ��� ������ ���������, ���� �����
            var selectedViolation = (Violation)e.SelectedItem;
            Console.WriteLine($"������� ���������: {selectedViolation.Description}");
        }
    }

       public class Violation
    {
        public int ViolationId { get; set; }
        public int UserId { get; set; } // ��������� ��� ��������
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int SeverityLevel { get; set; }
        public int ViolationCount { get; set; }
    }
    //private void OnViolationSelected(object sender, SelectedItemChangedEventArgs e)
    //{
    //    if (e.SelectedItem is Violation selectedViolation)
    //    {
    //        // �������� � ��������� ����������
    //        DisplayAlert("������� ���������", $"��������: {selectedViolation.Description}", "OK");
    //    }

    //// ����� ���������
    //((ListView)sender).SelectedItem = null;
    //}
}