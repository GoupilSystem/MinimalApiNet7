using AutoMapper;
using MinimalAPI.Api.Dtos;
using MinimalAPI.Api.Enums;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Api.Entities;
using System;
using System.Runtime;
using System.Linq.Expressions;

namespace MinimalAPI.Api.Data
{
    public class ApiRepository : IApiRepository
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public ApiRepository(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CustomerDto>> Customers()
        {
            var customers = await _context.Customer
                .Select(c => new CustomerDto()
                {
                    CustomerId = c.Id,
                    LastName = c.LastName,
                    FirstName = c.FirstName,
                    Address = c.Address,
                    City = c.City,
                    DateCreated = c.DateCreated,
                    DateUpdated = c.DateUpdated,
                    Purchases = _context.Purchase
                            .Where(pu => pu.CustomerId == c.Id)
                            .Select(pu => _mapper.Map<PurchaseDto>(pu)).ToList()
                })
                .ToListAsync();

            return customers;
        }

        public async Task<CustomerDto> Customer(int id)
        {
            var customer = await _context.Customer
                .Where(c => c.Id == id)
                .Select(c => new CustomerDto()
                {
                    CustomerId = c.Id,
                    LastName = c.LastName,
                    FirstName = c.FirstName,
                    Address = c.Address,
                    City = c.City,
                    DateCreated = c.DateCreated,
                    DateUpdated = c.DateUpdated,
                    Purchases = _context.Purchase
                            .Where(pu => pu.CustomerId == c.Id)
                            .Select(pu => _mapper.Map<PurchaseDto>(pu)).ToList()
                })
                .FirstOrDefaultAsync();

            return customer;
        }

        public async Task<List<PurchaseDto>> Purchases()
        {
            var purchases = await _context.Purchase
                .Select(p => _mapper.Map<PurchaseDto>(p))
                .ToListAsync();

            return purchases;
        }

        public async Task PopulateDb(int amountOfCustomers)
        {
            List<Customer> customers = new();
            List<Purchase> purchases = new();
            int purchaseIdCount = 0;

            for (int i = 1; i <= amountOfCustomers; i++)
            {
                Customer customer = new()
                {
                    Id = i,
                    FirstName = $"FirstName {i}",
                    LastName = $"LastName {i}",
                    Address = $"Some street {i}",
                    City = i % 3 == 0 ? "Oslo" : i % 2 == 0 ? "Paris" : "Kyoto",
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                };
                customers.Add(customer);

                // Populate with at least one until 3 orders per customer
                for (int j = 1; j <= i % 4; j++)
                {
                    purchaseIdCount++;
                    Purchase purchase = new()
                    {
                        Id = 1000 + purchaseIdCount,
                        CustomerId = customer.Id,
                        // Orders can be type 0 or 1; if 3 orders for a customer two are type 1
                        PurchaseType = j / 2,
                        // Amount from 200 until 500
                        Amount = 100 + 100 * j,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now
                    };
                    purchases.Add(purchase);
                }
            }
            _context.AddRange(customers);
            _context.AddRange(purchases);
            _context.SaveChanges();
        }

        public async Task<List<CustomerDto>> CustomersByField(KeyValuePair<FilterKey, string> filter)
        {
            var validator = new FilterValidator();
            var result = validator.Validate(filter);

            if (!result.IsValid)
            {
                throw new ArgumentException(result.Errors.First().ErrorMessage);
            }

            var query = _context.Customer.Include(c => c.Purchases).AsQueryable();

            Expression<Func<Purchase, bool>> purchaseSelector = null;

            switch (filter.Key)
            {
                case FilterKey.PurchaseType:
                    if (int.TryParse(filter.Value, out int purchaseTypeValue))
                    {
                        purchaseSelector = person => person.PurchaseType == purchaseTypeValue;
                    }
                    break;
                case FilterKey.City:
                    query = query.Where(p => p.City.Trim().ToLower() == filter.Value.Trim().ToLower());
                    break;
                default:
                    break;
            }

            var customers = await (query
                .Select(c => new CustomerDto()
                {
                    CustomerId = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Address = c.Address,
                    City = c.City,
                    Purchases = purchaseSelector == null ? null : _context.Purchase
                        .Where(pu => pu.CustomerId == c.Id)
                        .AsQueryable()
                        .Where(purchaseSelector)
                        .Select(pu => _mapper.Map<PurchaseDto>(pu))
                        .ToList()
                })).ToListAsync();

            if (purchaseSelector != null)
            {
                customers = customers.Where(c => c.Purchases.Count > 0).ToList();
            }

            return customers;
        }
    }
}
