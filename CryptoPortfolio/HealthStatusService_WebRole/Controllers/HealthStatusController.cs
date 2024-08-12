using CryptoPortfolioService_Data.Entities;
using CryptoPortfolioService_Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthStatusService_WebRole.Controllers
{
    public class HealthStatusController : Controller
    {
        private readonly HealthCheckRepository _healthCheckRepository;

        public HealthStatusController()
        {
            _healthCheckRepository = new HealthCheckRepository();
        }

        public ActionResult Index()
        {
            // Get health checks from the past hour
            var lastHourHealthChecks = _healthCheckRepository.GetWebRoleHealthChecksForLastHour()
                .OrderByDescending(hc => hc.Timestamp)
                .ToList();

            // Get health checks from the last 24 hours
            var WebRoleHealthChecks = _healthCheckRepository.GetLatestHealthChecks("WebRole", "0")
                .OrderByDescending(hc => hc.Timestamp)
                .ToList();

            // Calculate uptime percentage for the last 24 hours
            var uptimePercentage = CalculateUptimePercentage(WebRoleHealthChecks);

            ViewBag.UptimePercentage = uptimePercentage;
            ViewBag.LastHourHealthChecks = lastHourHealthChecks;

            return View();
        }

        private double CalculateUptimePercentage(List<HealthCheck> healthChecks)
        {
            int totalChecks = healthChecks.Count();
            int successfulChecks = healthChecks.Count(hc => hc.Status == "OK");

            if (totalChecks == 0) return 0;

            return (successfulChecks / (double)totalChecks) * 100;
        }
    }
}