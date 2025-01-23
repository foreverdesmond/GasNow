using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GasNow.Module;
using GasNow.Dto;
using GasNow.Service;
using Microsoft.EntityFrameworkCore.InMemory;

namespace GasNow.Tests.Service
{
    public class ChainServiceTests: IDisposable
    {
        private readonly GasNowDbContext _context;
        private readonly IMapper _mapper;
        private readonly ChainService _chainService;

        public ChainServiceTests()
        {
             var options = new DbContextOptionsBuilder<GasNowDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new GasNowDbContext(options);
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Chain, ChainDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            _chainService = new ChainService(_context, _mapper);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public void GetAllChains_ReturnsListOfChainDto()
        {
            // Arrange
            var chains = new List<Chain>
            {
                new Chain { NetworkID = 1, ChainName = "ETH Mainnet", Currency="ETH", Explorer="etherscan.io" },
                new Chain { NetworkID = 11155111, ChainName = "Sepolia Testnet", Currency="ETH", Explorer="sepolia.etherscan.io" }
            };

            _context.Chains.AddRange(chains);
            _context.SaveChanges();

            // Act
            var result = _chainService.GetAllChains();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetChainById_ReturnsChainDto_WhenChainExists()
        {
            // Arrange
            var chain = new Chain { NetworkID = 1, ChainName = "ETH Mainnet", Currency = "ETH", Explorer = "etherscan.io" };
                       _context.Chains.Add(chain);
            _context.SaveChanges();

            // Act
            var result = _chainService.GetChainById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ETH Mainnet", result.ChainName);
        }

        [Fact]
        public void GetChainById_ReturnsNull_WhenChainDoesNotExist()
        {
            // Act
            var result = _chainService.GetChainById(0);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public void GetChainByName_ReturnsChainDto_WhenChainExists()
        {
            // Arrange
            var chain = new Chain { NetworkID = 1, ChainName = "ETH Mainnet", Currency = "ETH", Explorer = "etherscan.io" };
            _context.Chains.Add(chain);
            _context.SaveChanges();

            // Act
            var result = _chainService.GetChainByName("ETH Mainnet");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.NetworkID);
        }

        [Fact]
        public void GetChainByName_ReturnsNull_WhenChainDoesNotExist()
        {
            // Act
            var result = _chainService.GetChainByName("ETH Mainnet"); ;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddChain_AddsChainToDatabase()
        {
            // Arrange
            var chainDto = new ChainDto { NetworkID = 1, ChainName = "ETH Mainnet", Currency = "ETH", Explorer = "etherscan.io" };

            // Act
            _chainService.AddChain(chainDto);

            // Assert
            var chainInDb = _context.Chains.Find(1);
            Assert.NotNull(chainInDb);
            Assert.Equal("ETH Mainnet", chainInDb.ChainName);
        }

        [Fact]
        public void UpdateChain_UpdatesExistingChain()
        {
            // Arrange
            var chain = new Chain { NetworkID = 1, ChainName = "ETH Mainnet", Currency = "ETH", Explorer = "etherscan.io" };
            
            _context.Chains.Add(chain);
            _context.SaveChanges();

            var chainDto = new ChainDto { NetworkID = 1, ChainName = "UpdatedChain", Currency = "ETH", Explorer = "etherscan.io"};

            // Act
            _chainService.UpdateChain(chainDto);

            // Assert
            var updatedChain = _context.Chains.Find(1);
            Assert.NotNull(updatedChain);
            Assert.Equal("UpdatedChain", updatedChain.ChainName);
        }

        [Fact]
        public void DeleteChain_RemovesChainFromDatabase()
        {
            // Arrange
            var chain = new Chain { NetworkID = 1, ChainName = "ETH Mainnet", Currency = "ETH", Explorer = "etherscan.io" };
            _context.Chains.Add(chain);
            _context.SaveChanges();

            // Act
            _chainService.DeleteChain(1);

            // Assert
            var deletedChain = _context.Chains.Find(1);
            Assert.Null(deletedChain);
        }
    }
}