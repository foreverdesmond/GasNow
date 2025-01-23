using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GasNow.Module;
using GasNow.Dto;
using GasNow.Service;

namespace GasNow.Tests.Service
{
    public class GasFeeServiceTests : IDisposable
    {
        private readonly GasNowDbContext _context;
        private readonly IMapper _mapper;
        private readonly GasFeeService _gasFeeService;

        public GasFeeServiceTests()
        {
            var options = new DbContextOptionsBuilder<GasNowDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new GasNowDbContext(options);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GasFee, GasFeeDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            _gasFeeService = new GasFeeService(_context, _mapper);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public void GetAllGasFees_ReturnsListOfGasFeeDto()
        {
            // Arrange
            var gasFees = new List<GasFee>
            {
                new GasFee { NetworkID = 1, BlockNumber = 100, SlowMaxFee = 10.0m },
                new GasFee { NetworkID = 2, BlockNumber = 200, SlowMaxFee = 20.0m }
            };

            _context.GasFees.AddRange(gasFees);
            _context.SaveChanges();

            // Act
            var result = _gasFeeService.GetAllGasFees();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetGasFeeById_ReturnsGasFeeDto_WhenExists()
        {
            // Arrange
            var gasFee = new GasFee { NetworkID = 1, BlockNumber = 100, SlowMaxFee = 10.0m };
            _context.GasFees.Add(gasFee);
            _context.SaveChanges();

            // Act
            var result = _gasFeeService.GetGasFeeByBlockNumberAndNetworkId(100,1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gasFee.NetworkID, result.NetworkID);
        }

        [Fact]
        public void GetGasFeeById_ReturnsNull_WhenDoesNotExist()
        {
            // Act
            var result = _gasFeeService.GetGasFeeByBlockNumberAndNetworkId(999,0);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddGasFee_AddsGasFeeToDatabase()
        {
            // Arrange
            var gasFeeDto = new GasFeeDto { NetworkID = 1, BlockNumber = 100, SlowMaxFee = 10.0m };

            // Act
            _gasFeeService.AddGasFee(gasFeeDto);

            // Assert
            var gasFeeInDb = _context.GasFees
                .FirstOrDefault(g => g.BlockNumber == 100 && g.NetworkID == 1);
            Assert.NotNull(gasFeeInDb);
            Assert.Equal(gasFeeDto.NetworkID, gasFeeInDb.NetworkID);
        }

        [Fact]
        public void UpdateGasFee_UpdatesExistingGasFee()
        {
            // Arrange
            var gasFee = new GasFee { NetworkID = 1, BlockNumber = 100, SlowMaxFee = 10.0m };
            _context.GasFees.Add(gasFee);
            _context.SaveChanges();

            var gasFeeDto = new GasFeeDto { NetworkID = 1, BlockNumber = 100, SlowMaxFee = 15.0m };

            // Act
            _gasFeeService.UpdateGasFee(gasFeeDto);

            // Assert
            var updatedGasFee = _context.GasFees
                .FirstOrDefault(g => g.BlockNumber == 100 && g.NetworkID == 1);
            Assert.NotNull(updatedGasFee);
            Assert.Equal(15.0m, updatedGasFee.SlowMaxFee);
        }

        [Fact]
        public void UpdateGasFee_DoesNotUpdate_WhenGasFeeDoesNotExist()
        {
            // Arrange
            var gasFeeDto = new GasFeeDto { NetworkID = 999, BlockNumber = 150, SlowMaxFee = 15.0m };

            // Act
            var exception = Record.Exception(() => _gasFeeService.UpdateGasFee(gasFeeDto));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteGasFee_RemovesGasFeeFromDatabase()
        {
            // Arrange
            var gasFee = new GasFee { NetworkID = 1, BlockNumber = 100, SlowMaxFee = 10.0m };
            _context.GasFees.Add(gasFee);
            _context.SaveChanges();

            // Act
            _gasFeeService.DeleteGasFee(100,1);

            // Assert
            var deletedGasFee = _context.GasFees
                .FirstOrDefault(g => g.BlockNumber == 100 && g.NetworkID == 1);
            Assert.Null(deletedGasFee);
        }

        [Fact]
        public void DeleteGasFee_DoesNotThrow_WhenGasFeeDoesNotExist()
        {
            // Act
            var exception = Record.Exception(() => _gasFeeService.DeleteGasFee(999,0));

            // Assert
            Assert.Null(exception);
        }
    }
}