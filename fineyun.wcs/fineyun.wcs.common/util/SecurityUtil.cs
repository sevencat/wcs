using System.Security.Cryptography;
using System.Text;

namespace fineyun.wcs.common.util;

public class SecurityUtil
{
	[ThreadStatic]
	static SHA1 _sha;

	static readonly Encoding Utf8Enc = Encoding.UTF8;

	public static string Sha1(string orgstr)
	{
		return Sha1(orgstr, Utf8Enc);
	}

	public static string Sha1(string orgstr, Encoding enc)
	{
		_sha ??= SHA1.Create();
		var md5data = _sha.ComputeHash(enc.GetBytes(orgstr));
		var sb = new StringBuilder();
		for (var i = 0; i < md5data.Length; i++)
		{
			sb.Append(md5data[i].ToString("x").PadLeft(2, '0'));
		}

		return sb.ToString();
	}

	public static string Sha1(byte[] orgdata)
	{
		_sha ??= SHA1.Create();
		var md5data = _sha.ComputeHash(orgdata);
		var sb = new StringBuilder();
		for (var i = 0; i < md5data.Length; i++)
		{
			sb.Append(md5data[i].ToString("x").PadLeft(2, '0'));
		}

		return sb.ToString();
	}

	[ThreadStatic]
	static MD5 _md5;

	public static string Md5(string orgstr, Encoding enc)
	{
		_md5 ??= MD5.Create();
		var md5data = _md5.ComputeHash(enc.GetBytes(orgstr));
		var sb = new StringBuilder();
		for (var i = 0; i < md5data.Length; i++)
		{
			sb.Append(md5data[i].ToString("x").PadLeft(2, '0'));
		}

		return sb.ToString();
	}

	public static string Md5(string orgstr)
	{
		return Md5(orgstr, Utf8Enc);
	}

	public static byte[] Md5(byte[] orgdata, int offset, int cnt)
	{
		_md5 ??= MD5.Create();
		var md5data = _md5.ComputeHash(orgdata, offset, cnt);
		return md5data;
	}

	public static string Md5(byte[] orgdata)
	{
		_md5 ??= MD5.Create();
		var md5data = MD5.HashData(orgdata);
		var sb = new StringBuilder();
		for (var i = 0; i < md5data.Length; i++)
		{
			sb.Append(md5data[i].ToString("x").PadLeft(2, '0'));
		}

		return sb.ToString();
	}


	public static string AesEncrypt(string text, string key)
	{
		if (string.IsNullOrEmpty(key))
			throw new ArgumentException("Key must have valid value.", nameof(key));
		if (string.IsNullOrEmpty(text))
			throw new ArgumentException("The text must have valid value.", nameof(text));

		var buffer = Encoding.UTF8.GetBytes(text);
		var hash = SHA512.Create();
		var aesKey = new byte[24];
		Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

		using (var aes = Aes.Create())
		{
			if (aes == null)
				throw new ArgumentException("Parameter must not be null.", nameof(aes));

			aes.Key = aesKey;

			using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
			using (var resultStream = new MemoryStream())
			{
				using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
				using (var plainStream = new MemoryStream(buffer))
				{
					plainStream.CopyTo(aesStream);
				}

				var result = resultStream.ToArray();
				var combined = new byte[aes.IV.Length + result.Length];
				Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
				Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);
				return Convert.ToBase64String(combined);
			}
		}
	}

	public static string AesDecrypt(string encryptedText, string key)
	{
		if (string.IsNullOrEmpty(key))
			throw new ArgumentException("Key must have valid value.", nameof(key));
		if (string.IsNullOrEmpty(encryptedText))
			throw new ArgumentException("The encrypted text must have valid value.", nameof(encryptedText));

		var combined = Convert.FromBase64String(encryptedText);
		var buffer = new byte[combined.Length];
		var hash = SHA512.Create();
		var aesKey = new byte[24];
		Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

		using (var aes = Aes.Create())
		{
			if (aes == null)
				throw new ArgumentException("Parameter must not be null.", nameof(aes));

			aes.Key = aesKey;

			var iv = new byte[aes.IV.Length];
			var ciphertext = new byte[buffer.Length - iv.Length];
			Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);
			Array.ConstrainedCopy(combined, iv.Length, ciphertext, 0, ciphertext.Length);
			aes.IV = iv;
			using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
			{
				using (var resultStream = new MemoryStream())
				{
					using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
					using (var plainStream = new MemoryStream(ciphertext))
					{
						plainStream.CopyTo(aesStream);
					}

					return Encoding.UTF8.GetString(resultStream.ToArray());
				}
			}
		}
	}
}
