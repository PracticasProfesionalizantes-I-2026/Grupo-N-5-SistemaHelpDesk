using System.Security.Cryptography;

namespace HelpDesk.Shared.Security;

/// <summary>
/// Proporciona funciones seguras para el hashing y verificación de contraseñas utilizando PBKDF2 (Rfc2898DeriveBytes).
/// No almacena contraseñas en texto plano.
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 100_000;

    /// <summary>
    /// Genera un hash seguro para la contraseña con una sal aleatoria criptográfica.
    /// </summary>
    /// <param name="password">Contraseña en texto claro.</param>
    /// <returns>String codificado en Base64 que contiene la sal y el hash derivado.</returns>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña no puede estar vacía.", nameof(password));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifica si la contraseña proporcionada coincide con el hash almacenado.
    /// </summary>
    /// <param name="password">Contraseña en texto claro.</param>
    /// <param name="hashedPassword">Hash almacenado en Base64.</param>
    /// <returns>True si la contraseña coincide; de lo contrario, False.</returns>
    public static bool VerifyPassword(string password, string? hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            if (hashBytes.Length != SaltSize + HashSize)
                return false;

            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            // Comparación de tiempo constante para evitar ataques de temporización
            return CryptographicOperations.FixedTimeEquals(
                new ReadOnlySpan<byte>(hashBytes, SaltSize, HashSize),
                actualHash);
        }
        catch
        {
            return false;
        }
    }
}
