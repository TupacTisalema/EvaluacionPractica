namespace OrderProcessingWorkerService.Config
{
    public class WorkerOptions
    {
        public string CronExpression { get; set; }
        public int MaxConcurrentOrders { get; set; }
        public int MaxConcurrentItems { get; set; }
    }
}