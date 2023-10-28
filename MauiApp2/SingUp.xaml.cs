using System;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;


namespace MauiApp2
{
    public partial class SingUp : ContentPage
    {
        private DatabaseServiceUser _databaseService;
        public static string CurrentUserEmail { get;  set; }
        public static string CurrentUserPassword { get;  set; }
        public SingUp()
        {
            InitializeComponent();
            //string databasePath = @"C:\Users\����� ��������\source\repos\MauiApp2\MauiApp2\user.db";
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db");
            _databaseService = new DatabaseServiceUser(databasePath);
        }

        private async void OnComeClicked(object sender, EventArgs e)
        {
            string password1 = EntryPassword1.Text;
            string email = EntryMail.Text;
            email = email.ToLower();
            // �������� �� ������� �����
            if (string.IsNullOrEmpty(password1) || string.IsNullOrEmpty(email))
            {
                await DisplayAlert("������", "�� ��� ���� ���������", "OK");
                return;
            }

            // �������� ����������� � ���� ������
            bool isAuthenticated = AuthenticateUser(email, password1);
            if (isAuthenticated)
            {

                User user = _databaseService.GetUserByEmail(email);
                await DisplayAlert("�����", "�� ��������������", "OK");
                BasicsPage basicsPage = new BasicsPage();
                user.StatusAccount = "On";
                _databaseService.UpdateUser(user); // ���������� ������ ������������ � ���� ������
                await Navigation.PushModalAsync(basicsPage);
                basicsPage.Unfocus();
            }
            else
            {
                await DisplayAlert("������", "������������ email ��� ������", "OK");
            }
        }

        private bool AuthenticateUser(string email, string password)
        {
            User user = _databaseService.GetUserByEmail(email);
            if (user != null)
            {
                string salt = email.Split('@')[0];
                string hashedPassword = HashPassword(password, salt);

                CurrentUserEmail = email;
                CurrentUserPassword = hashedPassword;
                return user.Password == hashedPassword;
            }
            return false;
        }
        private string HashPassword(string password, string salt)
        {
            string saltedPassword = password + salt;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(saltedPassword);

                byte[] hashedBytes = sha256.ComputeHash(bytes);

                string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();


                return hashedPassword;
            }
        }


        private async void OnGoBackTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}