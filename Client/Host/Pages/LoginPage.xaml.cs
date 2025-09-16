using ViewModel.ViewModels;

namespace Host.Pages
{
    public partial class LoginPage : BasePage
    {
        public LoginPage(LoginViewModel vm) : base(vm)
        {
            InitializeComponent();
        }
    }
}
