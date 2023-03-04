using MinimalAPI.Api.Dtos;
using MinimalAPI.Api.Entities;
using MinimalAPI.Api.Enums;

namespace MinimalAPI.Api.Data
{
    public interface IApiRepository
    {
        Task<List<CustomerDto>> Customers();
        Task<CustomerDto> Customer(int id);
        Task<List<PurchaseDto>> Purchases();
        Task PopulateDb(int amountOfPersons);
        Task<List<CustomerDto>> CustomersByField(KeyValuePair<FilterKey, string> filter);
    }
}
