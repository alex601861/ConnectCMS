using System.Text;

namespace CMSTrain.Client.Models.Constants;

public static class StringCipher
{
    public static string Decrypt(string cipherText, string key)
    {
        var decryptedText = Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));
        
        var result = new StringBuilder();
        
        for (var i = 0; i < decryptedText.Length; i++)
        {
            var character = (char)(decryptedText[i] ^ key[i % key.Length]);
            
            result.Append(character);
        }
        
        return result.ToString();
    }
}
