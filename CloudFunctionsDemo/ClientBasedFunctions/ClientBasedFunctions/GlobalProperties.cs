// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Research.SEAL;
using System.Collections.Generic;

namespace SEALAzureFuncClient
{
    /// <summary>
    /// Define properties and constants to be used in the Application
    /// </summary>
    static class GlobalProperties
    {
        /// <summary>
        /// Azure keys needed to execute functions in Azure. Keys will depend on a particular installation.
        /// </summary>
        internal struct Codes
        {
            public static readonly string Addition = "";
            public static readonly string Subtraction = "";
            public static readonly string MatrixVectorProduct = "";
            public static readonly string PublicKeysQuery = "";
            public static readonly string PublicKeysUpload = "";
            public static readonly string PublicKeysDelete = "";
        }

        /// <summary>
        /// Maximum size a matrix can have
        /// </summary>
        public static readonly int MatrixSizeMax = 16;

        /// <summary>
        /// The polynomial modulus parameter used by Microsoft SEAL BFV scheme
        /// </summary>
        public static readonly ulong PolyModulusDegree = 4096;

        /// <summary>
        /// Modulus for Plaintext. Since we use BatchEncoder in this demo, we
        /// need PlainModulus to be a prime number congruent to 2*PolyModulusDegree.
        /// The range of values the result matrix slots can contain is 
        /// [-127*128*MatrixSizeMax, 128*128*MatrixSizeMax], we need PlainModulus
        /// to be such that 128*128*MatrixSizeMax < PlainModulus / 2.
        /// </summary>
        public static readonly ulong PlainModulus = (1ul << 13) * 119 + 1;

        /// <summary>
        /// The decomposition bit count to be used with RelinKeys
        /// </summary>
        public static readonly int RelinKeysDBC = DefaultParams.DBCmax;

        /// <summary>
        /// The decomposition bit count to be used with GaloisKeys
        /// </summary>
        public static readonly int GaloisKeysDBC = DefaultParams.DBCmax / 2;

        private static SEALContext context_ = null;

        /// <summary>
        /// SEAL Context for general use in the application.
        /// </summary>
        public static SEALContext Context
        {
            get
            {
                if (null == context_)
                {
                    EncryptionParameters parms = new EncryptionParameters(SchemeType.BFV)
                    {
                        PolyModulusDegree = PolyModulusDegree,
                        CoeffModulus = DefaultParams.CoeffModulus128(polyModulusDegree: PolyModulusDegree),
                        PlainModulus = new SmallModulus(GlobalProperties.PlainModulus)
                    };
                    context_ = SEALContext.Create(parms);
                }

                return context_;
            }
        }
    }
}
