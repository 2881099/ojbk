using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

public static class Util
{
    //颜色列表，用于验证码、噪线、噪点 
    private static readonly Color[] _colors = new[] { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };

    /// <summary>
    /// 绘制验证码图片，返回图片的字节数组
    /// </summary>
    /// <param name="code"></param>
    /// <param name="length">验证码长度</param>
    /// <returns></returns>
    public static byte[] DrawVerifyCode(out string code, int length = 6)
    {
        code = Util.GenerratorCode(length);
        var bmp = new Bitmap(4 + 16 * code.Length, 40);
        var font = new Font("Times New Roman", 16);

        var r = new Random();

        var g = Graphics.FromImage(bmp);
        g.Clear(Color.White);
        //画噪线 
        for (var i = 0; i < 4; i++)
        {
            int x1 = r.Next(bmp.Width);
            int y1 = r.Next(bmp.Height);
            int x2 = r.Next(bmp.Width);
            int y2 = r.Next(bmp.Height);
            g.DrawLine(new Pen(_colors[_rnd.Value.Next(_colors.Length)]), x1, y1, x2, y2);
        }

        //画验证码字符串 
        for (int i = 0; i < code.Length; i++)
            g.DrawString(code[i].ToString(), font, new SolidBrush(_colors[_rnd.Value.Next(_colors.Length)]), (float)i * 16 + 2, 8);

        //将验证码图片写入内存流，并将其以 "image/Png" 格式输出 
        var ms = new MemoryStream();
        try
        {
            bmp.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            g.Dispose();
            bmp.Dispose();
        }
    }

    /// <summary>
    /// 绘制验证码图片，返回图片的Base64字符串
    /// </summary>
    /// <param name="code"></param>
    /// <param name="length">验证码长度</param>
    /// <returns></returns>
    public static string DrawVerifyCodeBase64String(out string code, int length = 6) =>
        $"data:image/png;base64,{DrawVerifyCode(out code, length)}";

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
