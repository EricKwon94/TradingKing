using Microsoft.Maui.Controls;
using View.Pages;

namespace View
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
