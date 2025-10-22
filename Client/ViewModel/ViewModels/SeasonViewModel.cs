using Application.Gateways;
using Application.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModel.ViewModels;

public partial class SeasonViewModel : BaseViewModel
{
    private readonly ILogger<SeasonViewModel> _logger;
    private readonly IAlertService _alert;
    private readonly ISeasonApi _season;

    public ObservableCollection<SeasonModel> Seasons { get; } = [];

    public SeasonViewModel(ILogger<SeasonViewModel> logger, IAlertService alert, ISeasonApi season)
    {
        _logger = logger;
        _alert = alert;
        _season = season;
    }

    public override async Task LoadAsync(CancellationToken ct)
    {
        List<ISeasonApi.SeasonRes> seasons;
        try
        {
            seasons = [.. await _season.GetSeasonsAsync(ct)];
        }
        catch (Exception ex)
        {
            await _alert.DisplayAlertAsync("error", ex.Message, "ok", ct);
            return;
        }

        for (int i = 0; i < seasons.Count - 1; i++)
        {
            Seasons.Add(new SeasonModel(i + 1, seasons[i].StartedAt.ToLocalTime()));
        }
    }
}

public partial class SeasonModel
{
    public int Seq { get; }
    public DateTime StartedAt { get; }

    public SeasonModel(int seq, DateTime startedAt)
    {
        Seq = seq;
        StartedAt = startedAt;
    }

    [RelayCommand]
    public async Task Click(CancellationToken ct)
    {

    }
}
