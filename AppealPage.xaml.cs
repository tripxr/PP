using Microsoft.Maui.Controls;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MauiApp3;

public partial class AppealPage : ContentPage
{
    private int violationId;
    private string violationDescription;
    public AppealPage(int violationId, string violationDescription)
    {
        InitializeComponent();
        this.violationId = violationId;
        this.violationDescription = violationDescription;
        violationDescriptionLabel.Text = violationDescription;
    }
    private async void OnSubmitAppealClicked(object sender, EventArgs e)
    {
        string appealText = appealEditor.Text;

        if (string.IsNullOrEmpty(appealText))
        {
            await DisplayAlert("������", "����������, �������� ����� ���������.", "OK");
            return;
        }

        string connectionString = "Server=sql6033.site4now.net;Database=db_aafddd_kyrs;User Id=db_aafddd_kyrs_admin;Password=kyrsovik23";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();

                // ��������� ��������� � ������� Appeals
                string query = "INSERT INTO Appeals (ViolationId, AppealText, Status, DateSubmitted) VALUES (@ViolationId, @AppealText, @Status, @DateSubmitted)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ViolationId", violationId);
                    command.Parameters.AddWithValue("@AppealText", appealText);
                    command.Parameters.AddWithValue("@Status", "� ��������");  // ������ �� ���������
                    command.Parameters.AddWithValue("@DateSubmitted", DateTime.Now);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        await DisplayAlert("�����", "��������� ������� ����������.", "OK");
                        await Navigation.PopAsync();  // ������������ �� ���������� ��������
                    }
                    else
                    {
                        await DisplayAlert("������", "������ ��� �������� ���������.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("������", "��������� ������ ��� ����������� � ���� ������: " + ex.Message, "OK");
            }
        }
    }
}