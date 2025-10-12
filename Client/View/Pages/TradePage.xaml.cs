using ViewModel.ViewModels.Trade;

namespace View.Pages;

public partial class TradePage : BasePage
{
    public TradePage(TradeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}