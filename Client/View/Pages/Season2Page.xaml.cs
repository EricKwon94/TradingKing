using ViewModel.ViewModels;

namespace View.Pages;

public partial class Season2Page : BasePage
{
    public Season2Page(Season2ViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}