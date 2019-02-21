// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Research.SEAL;
using System.Threading.Tasks;

namespace Microsoft.Research.SEALDemo
{
    /// <summary>
    /// Azure Function implementation for uploading public keys
    /// </summary>
    public static class PublicKeysUpload
    {
        /// <summary>
        /// Upload a key of the given type and associate it to the given SID
        /// </summary>
        /// <param name="req">Http request containing the key type, SID and key</param>
        /// <param name="log">Logger</param>
        /// <returns>Result of the operation</returns>
        [FunctionName("PublicKeysUpload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing request: PublicKeysUpload");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string sid = data?.sid;
            string keyType = data?.type;

            if (sid == null || sid.Equals(string.Empty))
            {
                return new BadRequestObjectResult("sid not present in request");
            }

            if (keyType?.Equals("GaloisKeys") == true)
            {
                if (GlobalProperties.GaloisKeys.ContainsKey(sid))
                {
                    return new BadRequestObjectResult($"{keyType} for given sid already present");
                }

                string keystr = data?.key;
                if (keystr == null)
                {
                    return new BadRequestObjectResult($"{keyType} not present in request");
                }

                GaloisKeys galk = Utilities.Base64ToGaloisKeys(keystr, GlobalProperties.Context);
                GlobalProperties.GaloisKeys.Add(sid, galk);
            }
            else if (keyType?.Equals("RelinKeys") == true)
            {
                if (GlobalProperties.RelinKeys.ContainsKey(sid))
                {
                    return new BadRequestObjectResult($"{keyType} for given sid already present");
                }

                string keystr = data?.key;
                if (keystr == null)
                {
                    return new BadRequestObjectResult($"{keyType} not present in request");
                }

                RelinKeys rlk = Utilities.Base64ToRelinKeys(keystr, GlobalProperties.Context);
                GlobalProperties.RelinKeys.Add(sid, rlk);
            }
            else
            {
                return new BadRequestObjectResult("Bad key type");
            }

            return new OkResult();
        }
    }
}
