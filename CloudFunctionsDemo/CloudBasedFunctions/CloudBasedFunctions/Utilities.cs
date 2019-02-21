// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Research.SEALDemo
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Convert a Base64 string to a Ciphertext
        /// </summary>
        /// <param name="b64">Base 64 string</param>
        /// <param name="context">SEALContext to verify resulting Ciphertext is valid for the SEALContext</param>
        /// <returns>Decoded Ciphertext</returns>
        public static Ciphertext Base64ToCiphertext(string b64, SEALContext context)
        {
            Ciphertext result = new Ciphertext();
            byte[] bytes = Convert.FromBase64String(b64);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                result.Load(context, ms);
            }

            return result;
        }

        /// <summary>
        /// Convert a Base64 string to a vector of Ciphertexts
        /// </summary>
        /// <param name="b64">Base 64 string</param>
        /// <param name="context">SEALContext to verify resulting Ciphertexts are valid for the SEALContext</param>
        /// <returns>Decoded Ciphertext</returns>
        public static List<Ciphertext> Base64ToCiphertextList(string b64, SEALContext context)
        {
            List<Ciphertext> result = new List<Ciphertext>();
            byte[] bytes = Convert.FromBase64String(b64);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                while (true)
                {
                    Ciphertext currCipher = new Ciphertext();
                    try
                    {
                        currCipher.Load(context, ms);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    result.Add(currCipher);
                }
            }

            return result;
        }

        /// <summary>
        /// Convert a Ciphertext to a Base64 string
        /// </summary>
        /// <param name="cipher">Ciphertext to convert</param>
        /// <returns>Base64 string representing the Ciphertext</returns>
        public static string CiphertextToBase64(Ciphertext cipher)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                cipher.Save(ms);
                byte[] bytes = ms.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Convert an IEnumerable<Ciphertext> to a Base64 string
        /// </summary>
        /// <param name="cipher">Ciphertext to convert</param>
        /// <returns>Base64 string representing the Ciphertext</returns>
        public static string CiphertextToBase64(IEnumerable<Ciphertext> ciphers, out int size)
        {
            size = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                foreach (Ciphertext cipher in ciphers)
                {
                    cipher.Save(ms);
                    size++;
                }
                byte[] bytes = ms.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Convert a Base64 string to a RelinKeys object
        /// </summary>
        /// <param name="b64">Base 64 string</param>
        /// <param name="context">SEALContext to verify resulting RelinKeys is valid for the SEALContext</param>
        /// <returns>Decoded RelinKeys</returns>
        public static RelinKeys Base64ToRelinKeys(string b64, SEALContext context)
        {
            RelinKeys result = new RelinKeys();
            byte[] bytes = Convert.FromBase64String(b64);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                result.Load(context, ms);
            }

            return result;
        }

        /// <summary>
        /// Convert a Base64 string to a GaloisKeys object
        /// </summary>
        /// <param name="b64">Base 64 string</param>
        /// <param name="context">SEALContext to verify resulting GaloisKeys is valid for the SEALContext</param>
        /// <returns>Decoded GaloisKeys</returns>
        public static GaloisKeys Base64ToGaloisKeys(string b64, SEALContext context)
        {
            GaloisKeys result = new GaloisKeys();
            byte[] bytes = Convert.FromBase64String(b64);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                result.Load(context, ms);
            }

            return result;
        }
    }
}
