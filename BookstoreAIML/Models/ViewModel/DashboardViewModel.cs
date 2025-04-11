namespace BookstoreAIML.Models.ViewModel
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }

        public List<RevenueChartData> MonthlyRevenue { get; set; }
    }

    public class RevenueChartData
    {
        public string Month { get; set; } // "04/2025"
        public decimal Revenue { get; set; }
    }
}
