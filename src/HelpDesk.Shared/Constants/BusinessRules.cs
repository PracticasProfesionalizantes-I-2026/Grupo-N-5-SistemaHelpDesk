namespace HelpDesk.Shared.Constants;

using HelpDesk.Shared.Enums;

public static class BusinessRules
{
    public const int MaxTitleLength = 200;
    public const int MaxDescriptionLength = 5000;
    public const int MaxCommentLength = 3000;
    public const int MaxCategoryNameLength = 100;
    public const int MaxCategoryDescriptionLength = 500;
    public const int MaxUserNameLength = 150;
    public const int MaxEmailLength = 256;
    public const int MaxTeamNameLength = 100;
    public const int MaxTeamDescriptionLength = 500;
    
    public static readonly Dictionary<TicketPriority, int> SLAHours = new()
    {
        { TicketPriority.Critica, 4 },
        { TicketPriority.Alta, 8 },
        { TicketPriority.Media, 24 },
        { TicketPriority.Baja, 72 }
    };
    
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
}

public static class ErrorMessages
{
    public const string TicketNotFound = "El ticket no fue encontrado";
    public const string UserNotFound = "El usuario no fue encontrado";
    public const string CategoryNotFound = "La categoría no fue encontrada";
    public const string PriorityNotFound = "La prioridad no fue encontrada";
    public const string StatusNotFound = "El estado no fue encontrado";
    public const string CommentNotFound = "El comentario no fue encontrado";
    public const string TeamNotFound = "El equipo no fue encontrado";
    
    public const string EmailAlreadyExists = "El email ya está registrado en el sistema";
    public const string CategoryHasTickets = "No se puede eliminar la categoría porque tiene tickets asociados";
    public const string UserHasOpenTickets = "No se puede desactivar el usuario porque tiene tickets abiertos asignados";
    public const string CannotModifyClosedTicket = "No se puede modificar un ticket en estado Cerrado";
    public const string CannotCommentOnClosedTicket = "No se puede agregar comentarios a un ticket cerrado";
    public const string OnlySupervisorCanAssign = "Solo un supervisor puede asignar técnicos";
    public const string OnlySupervisorCanReopen = "Solo un supervisor puede reabrir tickets";
    public const string TechnicianMustBeActive = "El técnico debe estar activo";
    public const string SupervisorCannotBeTechnician = "Un supervisor no puede ser asignado como técnico";
    public const string InvalidStatusTransition = "Transición de estado no válida";
    public const string TicketAlreadyAssigned = "El ticket ya tiene un técnico asignado";
    public const string UnauthorizedAccess = "No tiene permisos para realizar esta acción";
    public const string InvalidCredentials = "Credenciales inválidas";
    public const string TeamNameAlreadyExists = "Ya existe un equipo con ese nombre";
    public const string TeamMustHaveAtLeastOneTechnician = "Debe seleccionar al menos un técnico";
    public const string TechnicianNotFound = "El técnico no fue encontrado";
    public const string UserIsNotTechnician = "El usuario seleccionado no es un técnico";
    public const string TeamHasTickets = "No se puede eliminar el equipo porque tiene tickets asociados";
}