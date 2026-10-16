using System.Security.Cryptography;
using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.DataAccess.Entities;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.Shared.DTOs;
using HelpDesk.Shared.Exceptions;
using HelpDesk.Shared.Security;

namespace HelpDesk.BusinessLogic.Services;

public class PasswordRecoveryService : IPasswordRecoveryService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;

    public PasswordRecoveryService(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
    }

    public async Task<PasswordResetResponseDTO> ForgotPasswordAsync(ForgotPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ValidationException("Email", "El correo electrónico es obligatorio");

        var emailNormalized = dto.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(emailNormalized);

        // Por seguridad, si el usuario no existe no revelamos detalles
        if (user == null || !user.Activo)
        {
            return new PasswordResetResponseDTO(
                Exitoso: true,
                Mensaje: "Si el correo está registrado y activo, se generó un código de recuperación."
            );
        }

        // Invalidar tokens previos del usuario
        await _tokenRepository.InvalidateUserTokensAsync(user.Id);

        // Generar código numérico seguro de 6 dígitos
        var codigo = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Codigo = codigo,
            ExpiraEn = DateTime.UtcNow.AddMinutes(15),
            Usado = false,
            FechaCreacion = DateTime.UtcNow
        };

        await _tokenRepository.CreateAsync(resetToken);

        return new PasswordResetResponseDTO(
            Exitoso: true,
            Mensaje: "Se generó el código de recuperación exitosamente.",
            Codigo: codigo
        );
    }

    public async Task<PasswordResetResponseDTO> ResetPasswordAsync(ResetPasswordDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ValidationException("Email", "El correo electrónico es obligatorio");

        if (string.IsNullOrWhiteSpace(dto.Codigo))
            throw new ValidationException("Codigo", "El código de recuperación es obligatorio");

        if (string.IsNullOrWhiteSpace(dto.NuevaPassword) || dto.NuevaPassword.Length < 6)
            throw new ValidationException("NuevaPassword", "La nueva contraseña debe tener al menos 6 caracteres");

        var emailNormalized = dto.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(emailNormalized, asNoTracking: false);

        if (user == null || !user.Activo)
            throw new NotFoundException("Usuario no encontrado o inactivo");

        var token = await _tokenRepository.GetValidTokenAsync(user.Id, dto.Codigo.Trim());
        if (token == null)
            throw new BusinessRuleException("El código de recuperación es inválido o ha expirado");

        // Marcar token como usado
        token.Usado = true;
        await _tokenRepository.UpdateAsync(token);

        // Actualizar contraseña con hash
        user.PasswordHash = PasswordHasher.HashPassword(dto.NuevaPassword);
        await _userRepository.UpdateAsync(user);

        return new PasswordResetResponseDTO(
            Exitoso: true,
            Mensaje: "La contraseña ha sido restablecida correctamente."
        );
    }
}
