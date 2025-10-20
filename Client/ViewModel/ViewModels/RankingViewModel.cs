using Application.Gateways.Hubs;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModel.ViewModels;

public partial class RankingViewModel : BaseViewModel
{
    private readonly ILogger<RankingViewModel> _logger;
    private readonly IAlertService _alertService;
    private readonly IRankingApi _ranking;

    private readonly HashSet<RankModel> _users = new(new RankModel.Comparer());
    private readonly List<RankModel> _ranks = [];

    public ObservableCollection<RankModel> Ranks { get; } = [];

    public RankingViewModel(ILogger<RankingViewModel> logger, IAlertService alert, IRankingApi ranking)
    {
        _logger = logger;
        _alertService = alert;
        _ranking = ranking;
    }

    public override async Task OnAppearingAsync(CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
            return;

        try
        {
            await _ranking.StartAsync(ct);
            await foreach (var ranks in _ranking.GetRank(0, -1, ct))
            {
                foreach (var rank in ranks)
                {
                    var model = new RankModel { UserId = rank.Key, Asset = rank.Value, };

                    if (_users.Add(model))
                    {
                        Ranks.Add(model);
                        _ranks.Add(model);
                    }
                    else if (_users.TryGetValue(model, out RankModel? user))
                    {
                        user.Asset = model.Asset;
                    }
                }

                _ranks.Sort(new RankModel.Comparer());
                for (int i = 0; i < _ranks.Count; i++)
                {
                    if (Ranks[i].UserId != _ranks[i].UserId)
                    {
                        Ranks.Move(Ranks.IndexOf(_ranks[i]), i);
                    }
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            await _alertService.DisplayAlertAsync("ERROR", ex.ToString(), "ok", ct);
        }
    }

    public override async void OnDisappearing()
    {
        await _ranking.DisposeAsync();
    }
}

public partial class RankModel : ObservableObject
{
    [ObservableProperty]
    private string _userId = "";
    [ObservableProperty]
    private double _asset;

    public class Comparer : IComparer<RankModel>, IEqualityComparer<RankModel>
    {
        public int Compare(RankModel? x, RankModel? y)
        {
            return -1 * Comparer<double>.Default.Compare(x?.Asset ?? 0, y?.Asset ?? 0);
        }

        public bool Equals(RankModel? x, RankModel? y)
        {
            return x?.UserId == y?.UserId;
        }

        public int GetHashCode([DisallowNull] RankModel obj)
        {
            return obj.UserId.GetHashCode();
        }
    }
}
