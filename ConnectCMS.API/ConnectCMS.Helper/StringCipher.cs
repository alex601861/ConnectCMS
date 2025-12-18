using System.Text;

namespace CMSTrain.Helper;

public static class StringCipher
{
    public static string Encrypt(string plainText, string key)
    {
        var result = new StringBuilder();

        for (var i = 0; i < plainText.Length; i++)
        {
            var character = (char)(plainText[i] ^ key[i % key.Length]);

            result.Append(character);
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(result.ToString()));
    }
}