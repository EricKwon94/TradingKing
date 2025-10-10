using ViewModel.ViewModels;

namespace View.Pages;

public partial class RankingPage : BasePage
{
    public RankingPage(RankingViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}