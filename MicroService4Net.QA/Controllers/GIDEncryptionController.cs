using System;
using System.Linq;
using System.Collections.Generic;
using MicroService4Net.QA.Models;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MicroService4Net.QA.Controllers
{

    public class GIDEncryptionController : ApiController
    {
        [HttpGet]
        [Route("GIDEncrypt/{plaintext}")]
        public string GIDEncrypt(string plaintext)
        {
            return Encrypt(plaintext);
        }

        private byte[] Encrypt(byte[] byteData)
        {
            MemoryStream memoryStream = new MemoryStream();
            SymmetricAlgorithm symmetricAlgorithm = DES.Create();
            symmetricAlgorithm.Key= new byte[]
            {
                115,
                17,
                147,
                32,
                91,
                245,
                111,
                7
            };
            symmetricAlgorithm.IV= new byte[]
            {
                14,
                177,
                19,
                167,
                154,
                220,
                12,
                15
            };
            byte[] result;
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(byteData, 0, byteData.Length);
                cryptoStream.Close();
                byte[] array = memoryStream.ToArray();
                result = array;
            }
            return result;
        }

        private string Encrypt(string textString)
        {
            if (string.Empty.Equals(textString))
            {
                return string.Empty;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(textString);
            byte[] n = Encrypt(bytes);
            return ToHexString(n);
        }

        private static string ToHexString(byte[] n)
        {
            StringBuilder stringBuilder = new StringBuilder(n.Length * 2);
            for (int i = 0; i < n.Length; i++)
            {
                byte b = n[i];
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }
    }

}