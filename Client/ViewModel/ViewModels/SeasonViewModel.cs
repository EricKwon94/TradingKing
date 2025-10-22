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
    private readonly INavigationService _navigation;
    private readonly ISeasonApi _season;

    public ObservableCollection<SeasonModel> Seasons { get; } = [];

    public SeasonViewModel(
        ILogger<SeasonViewModel> logger, IAlertService alert, INavigationService navigation, ISeasonApi season)
    {
        _logger = logger;
        _alert = alert;
        _navigation = navigation;
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
            var model = new SeasonModel(seasons[i].Id, i + 1, seasons[i].StartedAt.ToLocalTime());
            model.OnClick += OnClick;
            Seasons.Add(model);
        }
    }

    private Task OnClick(int seasonId, CancellationToken ct)
    {
        Dictionary<string, object> param = new Dictionary<string, object>
        {
            ["seasonId"] = seasonId
        };
        return _navigation.GoToAsync("Season2", param, ct);
    }
}

public partial class SeasonModel
{
    private int _seasonId;
    public int Seq { get; }
    public DateTime StartedAt { get; }

    public event Func<int, CancellationToken, Task>? OnClick;

    public SeasonModel(int seasonId, int seq, DateTime startedAt)
    {
        _seasonId = seasonId;
        Seq = seq;
        StartedAt = startedAt;
    }

    [RelayCommand]
    public Task Click(CancellationToken ct)
    {
        return OnClick?.Invoke(_seasonId, ct) ?? Task.CompletedTask;
    }
}
