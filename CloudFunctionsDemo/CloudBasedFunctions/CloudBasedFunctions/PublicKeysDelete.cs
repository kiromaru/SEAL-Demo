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
    /// Azure Function implementation for deleting public keys
    /// </summary>
    public static class PublicKeysDelete
    {
        /// <summary>
        /// Delete keys of a given type for a given SID
        /// </summary>
        /// <param name="req">Http request containing the key type and SID</param>
        /// <param name="log">Logger</param>
        /// <returns>Result of the operation</returns>
        [FunctionName("PublicKeysDelete")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing request: PublicKeysDelete");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string sid = data?.sid;
            string keyType = data?.type;

            if (sid == null || sid.Equals(string.Empty))
            {
                return new BadRequestObjectResult("sid not present in request");
            }

            IActionResult result = null;
            if (keyType?.Equals("GaloisKeys") == true)
            {
                if (!GlobalProperties.GaloisKeys.ContainsKey(sid))
                {
                    result = GlobalProperties.GaloisKeys.Remove(sid) 
                        ? (IActionResult)new OkResult() : (IActionResult)new BadRequestResult();
                }
            }
            else if (keyType?.Equals("RelinKeys") == true)
            {
                if (!GlobalProperties.RelinKeys.ContainsKey(sid))
                {
                    result = GlobalProperties.RelinKeys.Remove(sid)
                        ? (IActionResult)new OkResult() : (IActionResult)new BadRequestResult();
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
