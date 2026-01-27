using System.Security.Cryptography;

namespace AcademiaLounge.Security;

public static class PasswordHasher
{
    // Formato: base64(salt).base64(hash)
    public static string Hash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password: senha,
            salt: salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string senha, string stored)
    {
        var parts = stored.Split('.', 2);
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = Convert.FromBase64String(parts[1]);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password: senha,
            salt: salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }
}
