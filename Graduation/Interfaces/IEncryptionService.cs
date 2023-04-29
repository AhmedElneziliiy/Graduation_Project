namespace Graduation.Interfaces
{
    public interface IEncryptionService
    {
        Task<byte[]> Encrypt(string simpletext, byte[] key, byte[] iv);
        Task<string> Decrypt(byte[] cipheredtext, byte[] key, byte[] iv);
    }
}
