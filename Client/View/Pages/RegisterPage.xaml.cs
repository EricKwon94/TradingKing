using ViewModel.ViewModels;

namespace View.Pages;

public partial class RegisterPage : BasePage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}