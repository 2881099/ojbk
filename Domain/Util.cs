using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

public static class Util
{
    static ThreadLocal<Random> _rnd = new ThreadLocal<Random>(() => new Random());
    public static Random Random => _rnd.Value;

    /// <summary>
    ///  生成短信验证码
    /// </summary>
    /// <param name="len">验证码长度为4</param>
    /// <returns></returns>
    public static string GenerratorCode(int len = 4)
    {
        var code = string.Empty;
        for (int a = 0; a < len; a++) code += "0123456789"[Random.Next(0, 10)];

        return code;
    }

    public static object Hash_HMAC(string signatureString, string secretKey, bool raw_output = false)
    {
        HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey));
        hmac.Initialize();
        byte[] buffer = Encoding.UTF8.GetBytes(signatureString);
        if (raw_output) return hmac.ComputeHash(buffer);
        return BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
    }

    public static string SHA1(string text, Encoding encode)
    {
        try
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_in = encode.GetBytes(text);
            byte[] bytes_out = sha1.ComputeHash(bytes_in);
            sha1.Dispose();
            string result = BitConverter.ToString(bytes_out);
            result = result.Replace("-", "");
            return result.ToLower();
        }
        catch (Exception ex)
        {
            throw new Exception("SHA1加密出错：" + ex.Message);
        }
    }

    public static string MD5(string source)
    {
        byte[] sor = Encoding.UTF8.GetBytes(source);
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] result = md5.ComputeHash(sor);
        StringBuilder strbul = new StringBuilder(40);
        for (int i = 0; i < result.Length; i++)
            strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
        return strbul.ToString();
    }

    public static string AESEncrypt(string text, byte[] key, byte[] iv)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;
        rijndaelCipher.KeySize = 128;
        rijndaelCipher.BlockSize = 128;
        byte[] keyBytes = new byte[16];
        Array.Copy(key, keyBytes, Math.Min(keyBytes.Length, key.Length));
        rijndaelCipher.Key = keyBytes;
        byte[] ivBytes = new byte[16];
        Array.Copy(iv, ivBytes, Math.Min(ivBytes.Length, iv.Length));
        rijndaelCipher.IV = ivBytes;
        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
        byte[] plainText = Encoding.UTF8.GetBytes(text);
        byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
        return Convert.ToBase64String(cipherBytes);
    }
    public static string AESDecrypt(string base64_text, byte[] key, byte[] iv)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;
        rijndaelCipher.KeySize = 128;
        rijndaelCipher.BlockSize = 128;
        byte[] encryptedData = Convert.FromBase64String(base64_text);
        byte[] keyBytes = new byte[16];
        Array.Copy(key, keyBytes, Math.Min(keyBytes.Length, key.Length));
        rijndaelCipher.Key = keyBytes;
        byte[] ivBytes = new byte[16];
        Array.Copy(iv, ivBytes, Math.Min(ivBytes.Length, iv.Length));
        rijndaelCipher.IV = ivBytes;
        ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
        byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        return Encoding.UTF8.GetString(plainText);
    }

    public static string Mask(string str, int maskLeft, int maskRight)
    {
        if (string.IsNullOrEmpty(str)) return str;
        var chrs = str.ToCharArray();
        var mastEnd = maskLeft + (str.Length - maskRight);
        for (var a = maskLeft; a < mastEnd; a++)
        {
            if (a >= chrs.Length) break;
            if (a >= 0 && a < chrs.Length)
                chrs[a] = '*';
        }
        return new string(chrs);
    }
}
