using Application.Gateways;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModel.ViewModels;

public partial class SeasonViewModel : BaseViewModel
{
    private readonly ILogger<SeasonViewModel> _logger;
    private readonly ISeasonApi _season;

    public SeasonViewModel(ILogger<SeasonViewModel> logger, ISeasonApi season)
    {
        _logger = logger;
        _season = season;
    }

    public override async Task LoadAsync(CancellationToken ct)
    {
        var v = await _season.GetSeasonsAsync(ct);
    }
}
