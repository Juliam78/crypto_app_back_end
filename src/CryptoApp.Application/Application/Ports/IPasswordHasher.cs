namespace CryptoAppBackEnd.Application.Ports
{
    /// <summary>
    /// Puerto de salida para el hashing de contraseñas.
    /// El adaptador concreto (SHA-256, bcrypt, etc.) vive en Infrastructure.
    /// </summary>
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
