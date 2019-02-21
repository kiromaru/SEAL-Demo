// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Research.SEAL;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Microsoft.Research.SEALDemo
{
    /// <summary>
    /// Azure Function implementation of a Ciphertext matrix product
    /// </summary>
    public static class MatrixProduct
    {
        /// <summary>
        /// Execute Ciphertext matrix product in an Azure Function. This function assumes
        /// the matrix is a square matrix with power-of-two number of rows and columns.
        /// </summary>
        /// <param name="req">Http request containing the matrix and vector to multiply</param>
        /// <param name="log">Logger</param>
        /// <returns>Result of the operation</returns>
        [FunctionName("MatrixProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing request: Matrix Product");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string sid = data?.sid;
            string matrixaStr = data?.matrixa;
            string matrixbStr = data?.matrixb;

            if (sid == null)
            {
                return new BadRequestObjectResult("sid not present in request");
            }
            if (matrixaStr == null || matrixbStr == null)
            {
                return new BadRequestObjectResult("matrixa or matrixb not present in request");
            }

            bool rlkFound = GlobalProperties.RelinKeys.TryGetValue(sid, out RelinKeys rlk);
            if (!rlkFound)
            {
                return new BadRequestObjectResult("RelinKeys for given sid not found");
            }
            bool galkFound = GlobalProperties.GaloisKeys.TryGetValue(sid, out GaloisKeys galk);
            if (!galkFound)
            {
                return new BadRequestObjectResult("GaloisKeys for given sid not found");
            }

            List<Ciphertext> matrixa = null;
            Ciphertext matrixb = null;

            try
            {
                matrixa = Utilities.Base64ToCiphertextList(matrixaStr, GlobalProperties.Context);
                matrixb = Utilities.Base64ToCiphertext(matrixbStr, GlobalProperties.Context);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult($"Error loading ciphertexts: {e.Message}");
            }

            int dimension = 2 * matrixa.Count;
            if ((dimension & (dimension - 1)) != 0)
            {
                return new BadRequestObjectResult("dimension is not a power of two");
            }

            Ciphertext result = new Ciphertext();
            try
            {
                int batchRowSize = (int)GlobalProperties.PolyModulusDegree / 2;
                int eltSeparation = (dimension > 2) ? (batchRowSize / dimension) : 0;

                List<Ciphertext> tempResult = new List<Ciphertext>();
                foreach (Ciphertext matrixaDiag in matrixa)
                {
                    Ciphertext currProduct = new Ciphertext();
                    GlobalProperties.Evaluator.Multiply(matrixaDiag, matrixb, currProduct);
                    tempResult.Add(currProduct);
                    GlobalProperties.Evaluator.RotateRowsInplace(matrixb, 2 * eltSeparation, galk);
                }

                GlobalProperties.Evaluator.AddMany(tempResult, result);
                GlobalProperties.Evaluator.RelinearizeInplace(result, rlk);
                Ciphertext tempCipher = new Ciphertext();
                GlobalProperties.Evaluator.RotateColumns(result, galk, tempCipher);
                GlobalProperties.Evaluator.AddInplace(result, tempCipher);
                GlobalProperties.Evaluator.ModSwitchToInplace(result, GlobalProperties.Context.LastParmsId);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult($"Error computing matrix-vector product: {e.Message}");
            }

            string b64result = Utilities.CiphertextToBase64(result);
            string resultstr = $"{{ \"sid\": \"{sid}\", \"result\": \"{b64result}\" }}";
            return new OkObjectResult(resultstr);
        }
    }
}
