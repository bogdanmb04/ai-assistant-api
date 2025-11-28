using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCI.AIAssistant.API.Services
{
    public interface IAppConfigurationService
    {
        public string? KeyVaultName { get; set; }
        public string? SecretsPrefix { get; set; }
        public string? IoTDeviceName { get; set; }
    }
}