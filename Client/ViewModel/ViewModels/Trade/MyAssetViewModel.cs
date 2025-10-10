using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModel.ViewModels.Trade;

public partial class MyAsset : ObservableObject
{
    // 자산구분
    public string Code { get; }
    public string Name { get; }
    // 보유잔고
    public double TotalQuantity { get; }
    // 평균매수가
    public double AvgPrice { get; }
    // 매수금액
    public double TotalPrice { get; set; }
    // 평가금액
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EvaluationProfit), nameof(ProfitRate))]
    public double _evaluationPrice;
    // 평가손익
    public double EvaluationProfit => EvaluationPrice - TotalPrice;
    // 수익률
    public double ProfitRate => (EvaluationProfit / TotalPrice) * 100;

    public MyAsset(string code, string name, double totalQuantity, double avgPrice)
    {
        Code = code;
        Name = name;
        TotalQuantity = totalQuantity;
        AvgPrice = avgPrice;
    }
}
