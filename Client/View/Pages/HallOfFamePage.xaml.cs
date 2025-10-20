using ViewModel.ViewModels;

namespace View.Pages;

public partial class HallOfFamePage : BasePage
{
    public HallOfFamePage(HallOfFameViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}