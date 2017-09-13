using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private ILogger<CloudMailService> _logger;

        private string fromAddress = Startup.Configuration["mailSettings:fromAddress"];
        private string toAddress = Startup.Configuration["mailSettings:toAddress"];

        public CloudMailService(ILogger<CloudMailService> logger)
        {
            _logger = logger;
        }

        public void Send(string subject, string message)
        {
            _logger.LogInformation($"From: {fromAddress} to {toAddress} using CloudMailService");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Message: {message}");
        }
    }
}
