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

namespace Microsoft.Research.SEALDemo
{
    /// <summary>
    /// Azure Function implementation of a Ciphertext addition
    /// </summary>
    public static class Addition
    {
        /// <summary>
        /// Execute Ciphertext Addition in an Azure Function
        /// </summary>
        /// <param name="req">Http request containing the matrices to add</param>
        /// <param name="log">Logger</param>
        /// <returns>Result of the operation</returns>
        [FunctionName("Addition")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing request: Addition");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string matrixa = data?.matrixa;
            string matrixb = data?.matrixb;

            if (matrixa == null || matrixb == null)
            {
                return new BadRequestObjectResult("matrixa or matrixb not present in request");
            }

            Ciphertext ciphera = null;
            Ciphertext cipherb = null;
            try
            {
                ciphera = Utilities.Base64ToCiphertext(matrixa, GlobalProperties.Context);
                cipherb = Utilities.Base64ToCiphertext(matrixb, GlobalProperties.Context);

            }
            catch (Exception e)
            {
                return new BadRequestObjectResult($"Error loading ciphertexts: {e.Message}");
            }

            Ciphertext result = new Ciphertext();
            try
            {
                GlobalProperties.Evaluator.Add(ciphera, cipherb, result);

                // Switch to smallest modulus so we save in communication
                GlobalProperties.Evaluator.ModSwitchToInplace(result, GlobalProperties.Context.LastParmsId);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult($"Evaluation error: {e.Message}");
            }

            string resultstr = $"{{ \"result\": \"{Utilities.CiphertextToBase64(result)}\" }}";
            return new OkObjectResult(resultstr);
        }
    }
}
