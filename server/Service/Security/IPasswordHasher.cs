namespace Service.Security;

public interface IPasswordHasher
{
    (string Hash, string Salt) HashingPassword(string password);
    bool VerifyPassword(string password, string hash, string salt);
}