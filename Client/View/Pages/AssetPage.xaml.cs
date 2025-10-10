using ViewModel.ViewModels.Trade;

namespace View.Pages;

public partial class AssetPage : BasePage
{
    public AssetPage(AssetViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}