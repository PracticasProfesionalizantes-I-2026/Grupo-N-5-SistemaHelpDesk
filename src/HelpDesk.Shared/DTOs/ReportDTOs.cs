namespace HelpDesk.Shared.DTOs;

public record TicketsByStatusDTO(
    StatusResponseDTO Estado,
    int Count,
    double Percentage
);

public record TicketsByPriorityDTO(
    PriorityResponseDTO Prioridad,
    int Count,
    double Percentage
);

public record TicketsByTechnicianDTO(
    UserSummaryDTO Tecnico,
    int TotalAssigned,
    int Open,
    int InProgress,
    int Resolved,
    int Closed,
    double AvgResolutionHours
);

public record SLAComplianceDTO(
    PriorityResponseDTO Prioridad,
    int TotalTickets,
    int WithinSLA,
    int Overdue,
    double CompliancePercentage,
    double AvgResolutionHours
);

public record TechnicianWorkloadDTO(
    UserSummaryDTO Tecnico,
    int ActiveTickets,
    int OverdueTickets,
    double AvgResolutionHours,
    int TicketsResolvedThisMonth,
    WorkloadLevel WorkloadLevel
);

public enum WorkloadLevel
{
    Bajo = 1,
    Medio = 2,
    Alto = 3,
    Critico = 4
}

public record DashboardStatsDTO(
    int TotalTickets,
    int OpenTickets,
    int InProgressTickets,
    int ResolvedTickets,
    int ClosedTickets,
    int OverdueTickets,
    double AvgResolutionHours,
    int TicketsCreatedToday,
    int TicketsResolvedToday
);