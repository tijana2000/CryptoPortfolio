using CryptoPortfolioService_Data.Entities;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    interface IEmailSender
    {
        [OperationContract]
        Task<bool> SendNotificationEmail(Alarm alarm);

        [OperationContract]
        Task<bool> SendAlertEmail(HealthCheck healthCheck);
    }
}
