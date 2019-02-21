// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Research.SEAL;
using System.Collections.Generic;

namespace Microsoft.Research.SEALDemo
{
    /// <summary>
    /// Global constants and properties for the Application
    /// </summary>
    public static class GlobalProperties
    {
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

        private static SEALContext context_ = null;

        /// <summary>
        /// SEALContext to use in the application
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

        private static Evaluator evaluator_ = null;

        /// <summary>
        /// Evaluator to be used by the Azure Functions
        /// </summary>
        public static Evaluator Evaluator
        {
            get
            {
                if (null == evaluator_)
                {
                    evaluator_ = new Evaluator(Context);
                }

                return evaluator_;
            }
        }

        private static Dictionary<string, RelinKeys> rlks_ = null;

        /// <summary>
        /// Relinearization keys for a given SID
        /// </summary>
        public static Dictionary<string, RelinKeys> RelinKeys
        {
            get
            {
                if (null == rlks_)
                {
                    rlks_ = new Dictionary<string, RelinKeys>();
                }

                return rlks_;
            }
        }

        private static Dictionary<string, GaloisKeys> galks_ = null;

        /// <summary>
        /// Galois Keys for a given SID
        /// </summary>
        public static Dictionary<string, GaloisKeys> GaloisKeys
        {
            get
            {
                if (null == galks_)
                {
                    galks_ = new Dictionary<string, GaloisKeys>();
                }

                return galks_;
            }
        }
    }
}
