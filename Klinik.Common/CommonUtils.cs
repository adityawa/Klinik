﻿using Klinik.Data.DataRepository;
using Klinik.Resources;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Klinik.Common
{
    /// <summary>
    /// Common class to provide common functionalities
    /// </summary>
    public static class CommonUtils
    {
        private const string _keyEncryptor = "Klinik2019";
        public static string KeyEncryptor { get { return _keyEncryptor; } }


        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="plainText">String to be encrypted</param>
        /// <param name="password">Password</param>
        public static string Encryptor(string plainText, string password)
        {
            if (plainText == null)
            {
                return null;
            }

            if (password == null)
            {
                password = String.Empty;
            }

            // Get the bytes of the string
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(plainText);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(bytesEncrypted);
        }

        /// <summary>
        /// Decrypt a string.
        /// </summary>
        /// <param name="encryptedText">String to be decrypted</param>
        /// <param name="password">Password used during encryption</param>
        /// <exception cref="FormatException"></exception>
        public static string Decryptor(string encryptedText, string password)
        {
            if (encryptedText == null)
            {
                return null;
            }

            if (password == null)
            {
                password = String.Empty;
            }
            try
            {
                var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
                var passwordBytes = Encoding.UTF8.GetBytes(password);

                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                var bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);

                return Encoding.UTF8.GetString(bytesDecrypted);
            }
            catch
            {
                return encryptedText;
            }
            // Get the bytes of the string

        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="bytesToBeEncrypted"></param>
        /// <param name="passwordBytes"></param>
        /// <returns></returns>
        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="bytesToBeDecrypted"></param>
        /// <param name="passwordBytes"></param>
        /// <returns></returns>
        private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public static string GetPatientAge(string birthDate)
        {
            DateTime dob;
            if (DateTime.TryParse(birthDate, out dob))
            {
                return GetPatientAge(dob);
            }

            return string.Empty;
        }

        public static string GetPatientAge(DateTime dob)
        {
            string result = string.Empty;

            int Years = new DateTime(DateTime.Now.Subtract(dob).Ticks).Year - 1;

            DateTime PastYearDate = dob.AddYears(Years);

            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == DateTime.Now)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= DateTime.Now)
                {
                    Months = i - 1;
                    break;
                }
            }

            if (Years > 0)
            {
                if (Months > 0)
                    result = Years.ToString() + " " + UIMessages.Years + " " + Months.ToString() + " " + UIMessages.Month;
                else
                    result = Years.ToString() + " " + UIMessages.Years;
            }
            else
            {
                result = Months.ToString() + " " + UIMessages.Month;
            }

            return result;
        }

        public static DateTime ConvertStringDate2Datetime(string strDate)
        {
            if (String.IsNullOrEmpty(strDate) || String.IsNullOrWhiteSpace(strDate))
                return Convert.ToDateTime("1900-01-01");
            string[] arrDates = strDate.Split('/');
            return new DateTime(Convert.ToInt16(arrDates[2]), Convert.ToInt16(arrDates[1]), Convert.ToInt16(arrDates[0]));
        }
    }
}