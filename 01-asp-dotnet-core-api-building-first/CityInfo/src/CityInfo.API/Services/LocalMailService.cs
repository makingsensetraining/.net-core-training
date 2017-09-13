using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private ILogger<LocalMailService> _logger;

        private string fromAddress = Startup.Configuration["mailSettings:fromAddress"];
        private string toAddress = Startup.Configuration["mailSettings:toAddress"];

        public LocalMailService(ILogger<LocalMailService> logger)
        {
            _logger = logger;
        }

        public void Send(string subject, string message)
        {
            _logger.LogInformation($"From: {fromAddress} to {toAddress} using LocalMailService");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Message: {message}");
        }
    }
}
