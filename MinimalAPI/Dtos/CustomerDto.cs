namespace MinimalAPI.Api.Dtos
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public List<PurchaseDto> Purchases { get; set; }
    }
}
