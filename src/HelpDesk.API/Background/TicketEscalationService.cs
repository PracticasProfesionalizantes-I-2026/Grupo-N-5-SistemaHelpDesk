using HelpDesk.BusinessLogic.Interfaces;

namespace HelpDesk.API.Background;

public class TicketEscalationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TicketEscalationService> _logger;

    public TicketEscalationService(
        IServiceScopeFactory scopeFactory,
        ILogger<TicketEscalationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();
                var count = await ticketService.EscalateOverdueTicketsAsync();

                if (count > 0)
                    _logger.LogInformation("CU-25: {Count} ticket(s) escalados por SLA", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en escalamiento automático de tickets");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}