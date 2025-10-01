using ViewModel.ViewModels;

namespace View.Pages;

public partial class TickerPage : BasePage
{
    public TickerPage(TickerViewModel vm) : base(vm)
    {
        InitializeComponent();
    }
}