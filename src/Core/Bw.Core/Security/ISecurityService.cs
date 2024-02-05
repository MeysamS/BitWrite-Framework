using System.Security.Cryptography;
using System.Text;

namespace Bw.Core.Security;

public interface ISecurityService
{
    string GetSha256Hash(string input);
    Guid CreateCryptographicallySecureGuid();
    string Base64Encode(string text);
    string Base64Decode(string base64);
}



public class SecurityService : ISecurityService
{
    private byte[] IV =
    {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };
    private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

    public string GetSha256Hash(string input)
    {
        var byteValue = Encoding.UTF8.GetBytes(input);
        var byteHash = SHA256.HashData(byteValue);
        return Convert.ToBase64String(byteHash);
    }

    public string Base64Encode(string text)
    {

        var textBytes = Encoding.UTF8.GetBytes(text);

        var output = Convert.ToBase64String(textBytes);
        output = output.Split('=')[0]; // Remove any trailing '='s
        output = output.Replace('+', '-'); // 62nd char of encoding
        output = output.Replace('/', '_'); // 63rd char of encoding

        return output;
    }
    public string Base64Decode(string input)
    {
        var output = input;
        output = output.Replace('-', '+'); // 62nd char of encoding
        output = output.Replace('_', '/'); // 63rd char of encoding
        switch (output.Length % 4) // Pad with trailing '='s
        {
            case 0: break; // No pad chars in this case
            case 2: output += "=="; break; // Two pad chars
            case 3: output += "="; break; // One pad char
            default: throw new Exception("Illegal base64url string!");
        }
        var converted = Convert.FromBase64String(output); // Standard base64 decoder

        return Encoding.UTF8.GetString(converted);
    }
    public Guid CreateCryptographicallySecureGuid()
    {
        var bytes = new byte[16];
        _rand.GetBytes(bytes);
        return new Guid(bytes);
    }

}
