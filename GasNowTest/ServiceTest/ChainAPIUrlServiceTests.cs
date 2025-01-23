using AutoMapper;
using Microsoft.EntityFrameworkCore;
using GasNow.Module;
using GasNow.Dto;
using GasNow.Service;

namespace GasNow.Tests.Service
{
    public class ChainAPIUrlServiceTests : IDisposable
    {
        private readonly GasNowDbContext _context;
        private readonly IMapper _mapper;
        private readonly ChainAPIUrlService _chainAPIUrlService;

        public ChainAPIUrlServiceTests()
        {
            var options = new DbContextOptionsBuilder<GasNowDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new GasNowDbContext(options);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ChainAPIUrl, ChainAPIUrlDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            _chainAPIUrlService = new ChainAPIUrlService(_context, _mapper);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public void GetAllChainAPIUrls_ReturnsListOfChainAPIUrlDto()
        {
            // Arrange
            var apiUrls = new List<ChainAPIUrl>
            {
                new ChainAPIUrl { Guid = Guid.NewGuid(), APIName = "Api1", APIUrl="Url1", NetworkID = 1 },
                new ChainAPIUrl { Guid = Guid.NewGuid(), APIName = "Api2", APIUrl="Url2", NetworkID = 2 }
            };

            _context.ChainAPIUrls.AddRange(apiUrls);
            _context.SaveChanges();

            // Act
            var result = _chainAPIUrlService.GetAllChainAPIUrls();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetChainAPIUrlById_ReturnsChainAPIUrlDto_WhenExists()
        {
            // Arrange
            var apiUrl = new ChainAPIUrl { Guid = Guid.NewGuid(), APIName = "Api1", APIUrl = "Url1", NetworkID = 1 };
            _context.ChainAPIUrls.Add(apiUrl);
            _context.SaveChanges();

            // Act
            var result = _chainAPIUrlService.GetChainAPIUrlById(apiUrl.Guid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(apiUrl.APIName, result.APIName);
        }

        [Fact]
        public void GetChainAPIUrlById_ReturnsNull_WhenDoesNotExist()
        {
            // Act
            var result = _chainAPIUrlService.GetChainAPIUrlById(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetChainAPIUrlsByAPIName_ReturnsListOfChainAPIUrlDto_WhenExists()
        {
            // Arrange
            var apiUrl = new ChainAPIUrl { Guid = Guid.NewGuid(), APIName = "Api1", APIUrl = "Url1", NetworkID = 1 };
            var apiUrl2 = new ChainAPIUrl { Guid = Guid.NewGuid(), APIName = "Api2", APIUrl = "Url2", NetworkID = 2 };
            _context.ChainAPIUrls.AddRange(apiUrl, apiUrl2);
            _context.SaveChanges();

            // Act
            var result = _chainAPIUrlService.GetChainAPIUrlsByAPIName("Api1");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetChainAPIUrlsByAPIName_ReturnsEmptyList_WhenDoesNotExist()
        {
            // Act
            var result = _chainAPIUrlService.GetChainAPIUrlsByAPIName("NonExistentAPI");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetChainAPIUrlsByNetworkID_ReturnsListOfChainAPIUrlDto_WhenExists()
        {
            // Arrange
            var apiUrl = new ChainAPIUrl { Guid = Guid.NewGuid(), APIName = "Api1", APIUrl = "Url1", NetworkID = 1 };
            var apiUrl2 = new ChainAPIUrl { Guid = Guid.NewGuid(), APIName = "Api2", APIUrl = "Url2", NetworkID = 1 };
            _context.ChainAPIUrls.AddRange(apiUrl, apiUrl2);
            _context.SaveChanges();

            // Act
            var result = _chainAPIUrlService.GetChainAPIUrlsByNetworkID(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetChainAPIUrlsByNetworkID_ReturnsEmptyList_WhenDoesNotExist()
        {
            // Act
            var result = _chainAPIUrlService.GetChainAPIUrlsByNetworkID(999);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void AddChainAPIUrl_AddsChainAPIUrlToDatabase()
        {
            // Arrange
            var apiUrlDto = new ChainAPIUrlDto { GUID = Guid.NewGuid(), APIName = "Api1", APIUrl="Url1", NetworkID = 1 };

            // Act
            _chainAPIUrlService.AddChainAPIUrl(apiUrlDto);

            // Assert
            var apiUrlInDb = _context.ChainAPIUrls.Find(apiUrlDto.GUID);
            Assert.NotNull(apiUrlInDb);
            Assert.Equal("Api1", apiUrlInDb.APIName);
        }

        [Fact]
        public void UpdateChainAPIUrl_UpdatesExistingChainAPIUrl()
        {
            // Arrange
            var apiUrl = new ChainAPIUrl { Guid = Guid.NewGuid(), APIName = "Api1", APIUrl = "Url1", NetworkID = 1 };
            _context.ChainAPIUrls.Add(apiUrl);
            _context.SaveChanges();

            var apiUrlDto = new ChainAPIUrlDto { GUID = apiUrl.Guid, APIName = "UpdatedAPI", NetworkID = 1 };

            // Act
            _chainAPIUrlService.UpdateChainAPIUrl(apiUrlDto);

            // Assert
            var updatedApiUrl = _context.ChainAPIUrls.Find(apiUrl.Guid);
            Assert.NotNull(updatedApiUrl);
            Assert.Equal("UpdatedAPI", updatedApiUrl.APIName);
        }

        [Fact]
        public void UpdateChainAPIUrl_DoesNotUpdate_WhenChainAPIUrlDoesNotExist()
        {
            // Arrange
            var apiUrlDto = new ChainAPIUrlDto { GUID = Guid.NewGuid(), APIName = "NonExistentAPI", NetworkID = 1 };

            // Act
            var exception = Record.Exception(() => _chainAPIUrlService.UpdateChainAPIUrl(apiUrlDto));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteChainAPIUrl_RemovesChainAPIUrlFromDatabase()
        {
            // Arrange
            var apiUrl = new ChainAPIUrl { Guid = Guid.NewGuid(), APIName = "Api1", APIUrl = "Url1", NetworkID = 1 };
            _context.ChainAPIUrls.Add(apiUrl);
            _context.SaveChanges();

            // Act
            _chainAPIUrlService.DeleteChainAPIUrl(apiUrl.Guid);

            // Assert
            var deletedApiUrl = _context.ChainAPIUrls.Find(apiUrl.Guid);
            Assert.Null(deletedApiUrl);
        }

        [Fact]
        public void DeleteChainAPIUrl_DoesNotThrow_WhenChainAPIUrlDoesNotExist()
        {
            // Act
            var exception = Record.Exception(() => _chainAPIUrlService.DeleteChainAPIUrl(Guid.NewGuid()));

            // Assert
            Assert.Null(exception);
        }
    }
}