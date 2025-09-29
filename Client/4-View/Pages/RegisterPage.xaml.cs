using ViewModel.ViewModels;

namespace View.Pages;

public partial class RegisterPage : BasePage
{
    public RegisterPage(RegisterViewModel vm) : base(vm)
    {
        InitializeComponent();
    }
}