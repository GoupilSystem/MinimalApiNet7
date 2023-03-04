using AutoMapper;
using MinimalAPI.Api.Data;
using MinimalAPI.Api.Dtos;
using MinimalAPI.Api.Entities;
using MinimalAPI.Api.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace MinimalAPI.Tests
{
    public class UnitTests
    {
        public UnitTests()
        {
            
        }

        [Fact]
        public async Task Customers_ReturnsListOfCustomerDto()
        {
            // Arrange
            var mockMapper = new Mock<IMapper>();
            var expectedDto = new CustomerDto();
            mockMapper.Setup(x => x.Map<Customer, CustomerDto>(It.IsAny<Customer>())).Returns(expectedDto);

            var contextFactory = new TestDbContextFactory();
            var context = contextFactory.CreateDbContext();
            var repository = new ApiRepository(context, mockMapper.Object);

            // Act
            var result = await repository.Customers();

            // Assert
            Assert.IsType<List<CustomerDto>>(result);
        }

        [Fact]
        public async Task Customer_ReturnsCustomerDtoWithCorrectCustomerId()
        {
            // Arrange
            Random rnd = new Random(); 
            int id = rnd.Next(1, 10);

            var mockMapper = new Mock<IMapper>();
            var expectedDto = new CustomerDto();
            mockMapper.Setup(x => x.Map<Customer, CustomerDto>(It.IsAny<Customer>())).Returns(expectedDto);

            var contextFactory = new TestDbContextFactory();
            var context = contextFactory.CreateDbContext();
            var repository = new ApiRepository(context, mockMapper.Object);

            // Act
            var result = await repository.Customer(id);

            // Assssert
            Assert.IsType<CustomerDto>(result);
            Assert.True(result.CustomerId == id);
        }

        [Fact]
        public async Task CustomersByCity_ReturnsListOfCustomerDtoWithCorrectCity()
        {
            // Arrange
            Random rnd = new Random(); 
            int rndInteger = rnd.Next(1, 5);
            string city = $"City{rndInteger}";

            var mockMapper = new Mock<IMapper>();
            var expectedDto = new CustomerDto();
            mockMapper.Setup(x => x.Map<Customer, CustomerDto>(It.IsAny<Customer>())).Returns(expectedDto);

            var contextFactory = new TestDbContextFactory();
            var context = contextFactory.CreateDbContext();
            var repository = new ApiRepository(context, mockMapper.Object);

            // Act
            var result = await repository.CustomersByField(new KeyValuePair<FilterKey, string>(FilterKey.City, city));

            // Assert
            Assert.IsType<List<CustomerDto>>(result);
            Assert.True(result.All(p => p.City == city));
        }
    }
}

public class TestDbContextFactory : IDbContextFactory<ApiContext>
{
    private DbContextOptions<ApiContext> _options;

    public TestDbContextFactory()
    {
        // Use of Guid to use a not already existing db
        _options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    public ApiContext CreateDbContext()
    {
        var context = new ApiContext(_options);

        // Populate TestDb
        for (var i = 1; i <= 10; i++)
        {
            context.Customer.Add(CreateCustomer(i));
        }
        context.SaveChanges();
        context.Database.EnsureCreated();

        return context;
    }

    public Customer CreateCustomer(int i)
    {
        return new Customer()
        {
            Id = i,
            // CustomerId = 1 lives in City1, CustomerId = 5 in City5, CustomerId = 6 lives in City1..
            City = $"City{(i - 1) % 5 + 1}"
        };
    }
}