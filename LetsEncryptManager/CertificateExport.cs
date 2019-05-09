using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace LetsEncryptManager
{
	public static class CertificateExport
	{
		/// <summary>
		/// Converts a PFX certificate to .crt (public key) and .key (private key) files that are usable by nginx.  If no exceptions are thrown, you can assume the files are written when this method returns.
		/// </summary>
		/// <param name="pfxPath">Pfx file path.</param>
		/// <param name="pfxPassword">Password for the pfx file.</param>
		/// <param name="crtPath">Path to write the public key to (recommended extension: .crt)</param>
		/// <param name="keyPath">Path to write the private key to (recommended extension: .key)</param>
		public static void PfxToCrtKey(string pfxPath, string pfxPassword, string crtPath, string keyPath)
		{
			X509Certificate2 pfx = new X509Certificate2(pfxPath, pfxPassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

			Directory.CreateDirectory(new FileInfo(crtPath).Directory.FullName);
			Directory.CreateDirectory(new FileInfo(keyPath).Directory.FullName);

			// Public Key
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("-----BEGIN CERTIFICATE-----");
			sb.AppendLine(Base64(pfx.Export(X509ContentType.Cert), 64));
			sb.AppendLine("-----END CERTIFICATE-----");
			File.WriteAllText(crtPath, sb.ToString(), Encoding.ASCII);

			// Private key
			using (RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)pfx.PrivateKey)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (TextWriter streamWriter = new StreamWriter(memoryStream))
					{
						PemWriter pemWriter = new PemWriter(streamWriter);
						AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetRsaKeyPair(rsa);
						pemWriter.WriteObject(keyPair.Private);
						streamWriter.Flush();
					}
					string output = Encoding.ASCII.GetString(memoryStream.ToArray()).Trim();
					File.WriteAllText(keyPath, output, Encoding.ASCII);
				}
			}
		}
		private static string Base64(byte[] data, int charsPerLine)
		{
			string str = Convert.ToBase64String(data);
			StringBuilder sb = new StringBuilder(str.Length + (str.Length / 64));
			for (int i = 0; i < str.Length; i++)
			{
				if (i > 0 && i % charsPerLine == 0)
					sb.Append('\n');
				sb.Append(str[i]);
			}
			return sb.ToString();
		}
	}
}
