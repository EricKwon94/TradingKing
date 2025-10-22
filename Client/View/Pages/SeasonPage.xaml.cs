using ViewModel.ViewModels;

namespace View.Pages;

public partial class SeasonPage : BasePage
{
    public SeasonPage(SeasonViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}