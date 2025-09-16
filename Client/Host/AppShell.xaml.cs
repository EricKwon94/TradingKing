using Host.Pages;
using Microsoft.Maui.Controls;

namespace Host
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("login/register", typeof(RegisterPage));
        }
    }
}
