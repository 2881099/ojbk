using System;
using System.IO;
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

    public static string Sha1(string text, Encoding encode)
    {
        try
        {
            SHA1 sha1 = SHA1.Create();
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

    public static string Md5(string source)
    {
        byte[] sor = Encoding.UTF8.GetBytes(source);
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] result = md5.ComputeHash(sor);
        StringBuilder strbul = new StringBuilder(40);
        for (int i = 0; i < result.Length; i++)
            strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
        return strbul.ToString();
    }

    public static string AesEncrypt(string text, byte[] key, byte[] iv)
    {
        if (text == null || text.Length <= 0) throw new ArgumentNullException("text");
        if (key == null || key.Length <= 0) throw new ArgumentNullException("key");
        if (iv == null || iv.Length <= 0) throw new ArgumentNullException("iv");

        byte[] encrypted;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                    swEncrypt.Write(text);
                encrypted = msEncrypt.ToArray();
                msEncrypt.Close();
            }
        }
        return Convert.ToBase64String(encrypted);
    }
    public static string AesDecrypt(string base64_text, byte[] key, byte[] iv)
    {
        byte[] encryptedData = Convert.FromBase64String(base64_text);
        if (encryptedData == null || encryptedData.Length <= 0) throw new ArgumentNullException("base64_text");
        if (key == null || key.Length <= 0) throw new ArgumentNullException("key");
        if (iv == null || iv.Length <= 0) throw new ArgumentNullException("iv");

        string plaintext = null;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (var msDecrypt = new MemoryStream(encryptedData))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    plaintext = srDecrypt.ReadToEnd();
                msDecrypt.Close();
            }
        }
        return plaintext;
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
