using ViewModel.ViewModels;

namespace View.Pages;

public partial class TradePage : BasePage
{
    public TradePage(TradeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}