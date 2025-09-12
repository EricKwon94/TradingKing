using Microsoft.Maui.Controls;
using ViewModel.ViewModels;

namespace Host.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
