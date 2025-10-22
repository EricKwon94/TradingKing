using Application.Gateways;
using Application.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ViewModel.Contracts;

namespace ViewModel.ViewModels;

public class Season2ViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ILogger<Season2ViewModel> _logger;
    private readonly ISeasonApi _season;
    private readonly IAlertService _alert;

    public ObservableCollection<SeasonRankModel> Ranks { get; } = [];

    public Season2ViewModel(ILogger<Season2ViewModel> logger, ISeasonApi season, IAlertService alert)
    {
        _logger = logger;
        _season = season;
        _alert = alert;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        int seasonId = Convert.ToInt32(query["seasonId"]);
        IEnumerable<ISeasonApi.RankRes> ranks;
        try
        {
            ranks = await _season.GetRanksAsync(seasonId, default);
        }
        catch (Exception ex)
        {
            await _alert.DisplayAlertAsync("error", ex.Message, "ok", default);
            return;
        }

        foreach (var rank in ranks.OrderByDescending(e => e.Assets))
        {
            Ranks.Add(new SeasonRankModel(rank.UserId, rank.Assets));
        }
    }
}

public record SeasonRankModel(string UserId, double Assets);