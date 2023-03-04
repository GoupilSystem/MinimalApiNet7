namespace MinimalAPI.Api.Dtos
{
    public class PurchaseDto
    {
        public int PurchaseId { get; set; }

        public int CustomerId { get; set; }

        public int Amount { get; set; }

        public int PurchaseType { get; set; }
    }
}
