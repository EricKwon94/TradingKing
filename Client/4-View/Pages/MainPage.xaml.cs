using ViewModel.ViewModels;

namespace View.Pages;

public partial class MainPage : BasePage
{
    public MainPage(MainViewModel vm) : base(vm)
    {
        InitializeComponent();
    }
}