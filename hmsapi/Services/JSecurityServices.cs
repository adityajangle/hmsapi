using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace hmsapi.Services
{
    public class JSecConst
    {

        public const string GENRSA = "GENRSA";
        public const string GENRSA2048 = "GENRSA2048";
        public const string GENAES = "GENAES";
        public const string SETAES = "SETAES";
        public const string vcap_request = "X-vcap-request";


    }

    public class JSecurityServices
	{
        public byte[]? m_AesKey;
        public byte[]? m_AesIv;

        public readonly string? ws_rsaPBK;
        public readonly string? ws_rsaKeyinfo;
        public readonly RSAParameters ws_rsaparam;




        public JSecurityServices(string Type)
        {
            switch (Type)
            {
                case JSecConst.GENRSA:
                    {

                        using RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(1024);
                        var rsaObject = rSACryptoServiceProvider.ExportParameters(true);
                        ws_rsaPBK = "<RSAKeyValue><Modulus>" + Convert.ToBase64String(rsaObject.Modulus!) + "</Modulus><Exponent>" + Convert.ToBase64String(rsaObject.Exponent!) + "</Exponent></RSAKeyValue>";
                        ws_rsaKeyinfo = ConvRSAtoString(rsaObject);
                    }
                    break;
                case JSecConst.GENRSA2048:
                    {
                        using RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(2048);
                        var rsaObject = rSACryptoServiceProvider.ExportParameters(true);
                        ws_rsaPBK = Convert.ToBase64String(rsaObject.Modulus!);
                        ws_rsaparam = rsaObject;
                    }
                    break;

                case JSecConst.GENAES:
                    {
                        using (Aes aes = Aes.Create())
                        {

                            aes.Mode = CipherMode.CBC;
                            aes.GenerateIV();
                            aes.GenerateKey();
                            m_AesKey = aes.Key;
                            m_AesIv = aes.IV;
                        };

                    }
                    break;



            }
        }


        public static string? AES_Encrypt(string input)
        {
            if (input == null) { return null; }

            using (Aes aes = Aes.Create())
            {
                using (SHA256 sha256 = SHA256.Create())
                {


                    string inpKey = getAssemblySign();

                    byte[] ib1 = Encoding.UTF8.GetBytes(inpKey[^10..]);
                    byte[] ib2 = Encoding.UTF8.GetBytes(inpKey[^10..]);
                    byte[] hs1 = sha256.ComputeHash(ib1);
                    byte[] hs2 = sha256.ComputeHash(ib2);
                    // Key is first 256 bits of the hash
                    byte[] key = new byte[32];
                    Array.Copy(hs1, key, 32);

                    // IV is next 128 bits of the hash
                    byte[] iv = new byte[16];
                    Array.Copy(hs2, iv, 16);

                    string encrypted = "";
                    aes.IV = iv;
                    aes.Key = key;
                    aes.Mode = CipherMode.CBC;
                    ICryptoTransform DESEncrypter = aes.CreateEncryptor();
                    byte[] Buffer = Encoding.UTF8.GetBytes(input);
                    encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
                    return encrypted;

                };

            };



        }

        private static string getAssemblySign()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the public key from the assembly
            byte[] publicKey = assembly.GetName().GetPublicKey();

            // Convert the public key to a hexadecimal string
            string publicKeyHex = BitConverter.ToString(publicKey).Replace("-", "");

            return publicKeyHex;
        }


        public static string? AES_Decrypt(string input)
        {
            if (input == null) { return null; }
            using (Aes aes = Aes.Create())
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    string inpKey = getAssemblySign();

                    byte[] ib1 = Encoding.UTF8.GetBytes(inpKey[^10..]);
                    byte[] ib2 = Encoding.UTF8.GetBytes(inpKey[^10..]);
                    byte[] hs1 = sha256.ComputeHash(ib1);
                    byte[] hs2 = sha256.ComputeHash(ib2);
                    // Key is first 256 bits of the hash
                    byte[] key = new byte[32];
                    Array.Copy(hs1, key, 32);

                    // IV is next 128 bits of the hash
                    byte[] iv = new byte[16];
                    Array.Copy(hs2, iv, 16);

                    aes.IV = iv;
                    aes.Key = key;
                    aes.Mode = CipherMode.CBC;
                    // aes.Padding = PaddingMode.PKCS7;
                    ICryptoTransform DESDecrypter = aes.CreateDecryptor();
                    byte[] Buffer = Convert.FromBase64String(input);
                    string decrypted = Encoding.UTF8.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
                    return decrypted;
                };

            };

        }


        public static string AES_Encrypt(string input, byte[]? m_AesKey, byte[]? m_AesIv)
        {
            using (Aes aes = Aes.Create())
            {
                string encrypted = "";
                aes.Key = m_AesKey!;
                aes.IV = m_AesIv!;
                aes.Mode = CipherMode.CBC;
                ICryptoTransform DESEncrypter = aes.CreateEncryptor();
                byte[] Buffer = Encoding.UTF8.GetBytes(input);
                encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
                return encrypted;
            };



        }
        public static string AES_Decrypt(string input, byte[]? m_AesKey, byte[]? m_AesIv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = m_AesKey!;
                aes.IV = m_AesIv!;
                aes.Mode = CipherMode.CBC;
                // aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform DESDecrypter = aes.CreateDecryptor();
                byte[] Buffer = Convert.FromBase64String(input);
                string decrypted = Encoding.UTF8.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
                return decrypted;
            };



        }
        public string ExportAESKey()
        {
            //Separator Alt+22
            string strIv = "";
            foreach (byte b in m_AesIv!)
            {
                strIv = strIv + Convert.ToString(b, 16).ToUpper();
            }
            //Global_asax.write_log("log", "DEBUG_INFO : " & strIv & vbNewLine)
            string strKey = "";
            foreach (byte b in m_AesKey!)
            {
                strKey = strKey + Convert.ToString(b, 16).ToUpper();
            }
            //Global_asax.write_log("log", "DEBUG_INFO : " & strKey & vbNewLine)
            string iv = Convert.ToBase64String(m_AesIv);
            string ivLen = iv.Length.ToString().PadLeft(2, '0');
            return iv + "" + Convert.ToBase64String(m_AesKey);
        }




        public static string RSAEncrypt(byte[] modulus, byte[] exponent, string textToEncrypt)
        {
            using (RSACryptoServiceProvider t_rsa = new RSACryptoServiceProvider())
            {
                RSAParameters rsaParameters = new RSAParameters();
                rsaParameters.Modulus = modulus;
                rsaParameters.Exponent = exponent;
                t_rsa.ImportParameters(rsaParameters);
                string encryptedString = Convert.ToBase64String(t_rsa.Encrypt(Encoding.ASCII.GetBytes(textToEncrypt), false));
                return encryptedString;
            };


        }
        public static string RSADecrypt(string textToDecrypt)
        {
            using (RSACryptoServiceProvider t_rsa = new RSACryptoServiceProvider())
            {
                byte[] encryptedBytes = Convert.FromBase64String(textToDecrypt);
                byte[] decryptedBytes = t_rsa.Decrypt(encryptedBytes, false);
                string decryptedString = Encoding.UTF8.GetString(decryptedBytes);

                return decryptedString;
            };

        }
        public static string RSADecrypt(RSACryptoServiceProvider rsaKeyInfo, string textToDecrypt)
        {
            byte[] encryptedBytes = Convert.FromBase64String(textToDecrypt);
            byte[] decryptedBytes = rsaKeyInfo.Decrypt(encryptedBytes, true);
            string decryptedString = Encoding.UTF8.GetString(decryptedBytes);

            return decryptedString;
        }
        public static string RSADecrypt(RSAParameters p_rsaKeyInfo, string textToDecrypt)
        {
            using (RSACryptoServiceProvider t_rsa = new RSACryptoServiceProvider())
            {
                t_rsa.ImportParameters(p_rsaKeyInfo);
                byte[] l_encryptedBytes = Convert.FromBase64String(textToDecrypt);
                byte[] l_decryptedBytes = t_rsa.Decrypt(l_encryptedBytes, false);
                string l_decryptedString = Encoding.UTF8.GetString(l_decryptedBytes);
                return l_decryptedString;
            };


        }
        public static string ConvRSAtoString(RSAParameters key)
        {
            var sw = new System.IO.StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, key);
            string strKey = sw.ToString();
            return strKey;
        }
        public static RSAParameters ConvStringtoRSA(string key)
        {
            var sr = new System.IO.StringReader(key);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            RSAParameters rsaKey = (RSAParameters)xs.Deserialize(sr)!;
            return rsaKey;
        }


        public static string ComputeSHA256(String plainText)
        {
            //SHA256CryptoServiceProvider.HashData()
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                return Convert.ToBase64String(bytes);
            }

        }

        public static string ComputeHMACSHA256(string key, string plainText)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                return Convert.ToBase64String(hashValue);
            }


        }

        public static string GenerateNonce()
        {
            byte[] bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        public static void write_log(string p_type, string l_msg, string id)
        {
        }
    }
}

