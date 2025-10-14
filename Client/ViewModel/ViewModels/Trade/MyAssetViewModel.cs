using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModel.ViewModels.Trade;

public partial class MyAssetViewModel : ObservableObject
{
    private double _totalBuyPrice;
    private double _totalSellPrice;

    /// <summary>
    /// 자산구분
    /// </summary>
    public string Code { get; }
    public string Name { get; }

    /// <summary>
    /// 보유잔고
    /// </summary>
    public double TotalQuantity { get; }

    /// <summary>
    /// 실현손익
    /// </summary>
    public double TotalPrice { get; set; }

    /// <summary>
    /// 평가금액
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EvaluationProfit), nameof(ProfitRate))]
    public double _evaluationPrice;

    /// <summary>
    /// 평가손익
    /// </summary>
    public double EvaluationProfit => EvaluationPrice + TotalPrice;

    /// <summary>
    /// 수익률
    /// </summary>
    public double ProfitRate => ((_totalSellPrice + EvaluationPrice) / _totalBuyPrice - 1) * 100;

    public MyAssetViewModel(string code, string name, double totalQuantity, double totalBuyPrice, double totalSellPrice)
    {
        Code = code;
        Name = name;
        TotalQuantity = totalQuantity;
        _totalBuyPrice = totalBuyPrice;
        _totalSellPrice = totalSellPrice;
    }
}
