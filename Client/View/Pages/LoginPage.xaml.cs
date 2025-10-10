using ViewModel.ViewModels;

namespace View.Pages;

public partial class LoginPage : BasePage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
