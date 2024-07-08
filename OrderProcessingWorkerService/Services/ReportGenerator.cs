using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Models;
using System.Collections.Generic;

namespace OrderProcessingWorkerService.Services
{
    public class ReportGenerator : IReportGenerator
    {
        private List<Order> _generatedReport;

        public void GenerateReport(IEnumerable<Order> orders)
        {
            _generatedReport = orders.ToList();
        }

        public List<Order> GetGeneratedReport()
        {
            return _generatedReport;
        }
    }
}