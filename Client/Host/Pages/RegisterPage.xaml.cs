using ViewModel.ViewModels;

namespace Host.Pages;

public partial class RegisterPage : BasePage
{
    public RegisterPage(RegisterViewModel vm) : base(vm)
    {
        InitializeComponent();
    }
}