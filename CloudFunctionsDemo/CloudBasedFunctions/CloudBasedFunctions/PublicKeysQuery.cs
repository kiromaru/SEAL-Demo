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
    /// Azure Function implementation for querying the presence of public keys
    /// </summary>
    public static class PublicKeysQuery
    {
        /// <summary>
        /// Query if a key of a given type exists for a given SID
        /// </summary>
        /// <param name="req">Http request that contains the key type and SID</param>
        /// <param name="log">Logger</param>
        /// <returns>Result of the operation</returns>
        [FunctionName("PublicKeysQuery")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing request: PublicKeysQuery");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string sid = data?.sid;
            string keyType = data?.type;

            if (sid == null)
            {
                return new BadRequestObjectResult("sid not present in request");
            }

            IActionResult result = null;
            if (keyType?.Equals("GaloisKeys") == true)
            {
                if (!GlobalProperties.GaloisKeys.ContainsKey(sid))
                {
                    result = GlobalProperties.GaloisKeys.ContainsKey(sid)
                        ? (IActionResult)new OkResult() : (IActionResult)new NotFoundResult();
                }
            }
            else if (keyType?.Equals("RelinKeys") == true)
            {
                if (!GlobalProperties.RelinKeys.ContainsKey(sid))
                {
                    result = GlobalProperties.RelinKeys.ContainsKey(sid)
                        ? (IActionResult)new OkResult() : (IActionResult)new NotFoundResult();
                }
            }
            else
            {
                result = new BadRequestObjectResult("Bad key type");
            }

            return result;
        }
    }
}
