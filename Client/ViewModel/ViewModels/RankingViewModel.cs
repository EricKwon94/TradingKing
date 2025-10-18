using Application.Gateways.Hubs;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModel.ViewModels;

public partial class RankingViewModel : BaseViewModel
{
    private readonly ILogger<RankingViewModel> _logger;
    private readonly IRankingApi _ranking;

    public RankingViewModel(ILogger<RankingViewModel> logger, IRankingApi ranking)
    {
        _logger = logger;
        _ranking = ranking;
    }

    public override async Task OnAppearingAsync(CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
            return;

        await _ranking.StartAsync(ct);
        try
        {
            await foreach (var ranks in _ranking.GetRank(0, -1, ct))
            {
                foreach (var rank in ranks)
                {
                    _logger.LogInformation("{id}: {score}", rank.Key, rank.Value);
                }
            }
        }
        catch (OperationCanceledException) { }
    }

    public override async void OnDisappearing()
    {
        await _ranking.DisposeAsync();
    }

    [RelayCommand]
    public Task InvokeAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
