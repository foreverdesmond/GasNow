using System.Collections.Generic;
using System.Linq;
using GasNow.Module;
using GasNow.Dto;
using AutoMapper;

namespace GasNow.Service
{
    public class GasFeeService
    {
        private readonly GasNowDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GasFeeService"/> class.
        /// </summary>
        /// <param name="context">The database context used to access gas fee data.</param>
        /// <param name="mapper">The mapper used for converting between entities and DTOs.</param>
        public GasFeeService(GasNowDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all gas fees as a list of <see cref="GasFeeDto"/>.
        /// </summary>
        /// <returns>A list of <see cref="GasFeeDto"/> representing all gas fees.</returns>
        public List<GasFeeDto> GetAllGasFees()
        {
            return _mapper.Map<List<GasFeeDto>>(_context.GasFees.ToList());
        }

        /// <summary>
        /// Retrieves a specific gas fee by block number and network ID.
        /// </summary>
        /// <param name="blockNumber">The block number associated with the gas fee.</param>
        /// <param name="networkId">The network ID associated with the gas fee.</param>
        /// <returns>
        /// A <see cref="GasFeeDto"/> representing the gas fee, or null if not found.
        /// </returns>
        public GasFeeDto GetGasFeeByBlockNumberAndNetworkId(long blockNumber, int networkId)
        {
            var gasFee = _context.GasFees.Find(blockNumber, networkId);
            return gasFee == null ? null : _mapper.Map<GasFeeDto>(gasFee);
        }

        /// <summary>
        /// Adds a new gas fee to the database.
        /// </summary>
        /// <param name="gasFeeDto">The gas fee data transfer object to add.</param>
        public void AddGasFee(GasFeeDto gasFeeDto)
        {
            var gasFee = _mapper.Map<GasFee>(gasFeeDto);
            _context.GasFees.Add(gasFee);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates an existing gas fee in the database.
        /// </summary>
        /// <param name="gasFeeDto">The gas fee data transfer object containing updated data.</param>
        public void UpdateGasFee(GasFeeDto gasFeeDto)
        {
            var gasFee = _context.GasFees.Find(gasFeeDto.BlockNumber, gasFeeDto.NetworkID);
            if (gasFee != null)
            {
                _mapper.Map(gasFeeDto, gasFee);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a gas fee from the database by block number and network ID.
        /// </summary>
        /// <param name="blockNumber">The block number associated with the gas fee to delete.</param>
        /// <param name="networkId">The network ID associated with the gas fee to delete.</param>
        public void DeleteGasFee(long blockNumber, int networkId)
        {
            var gasFee = _context.GasFees.Find(blockNumber, networkId);
            if (gasFee != null)
            {
                _context.GasFees.Remove(gasFee);
                _context.SaveChanges();
            }
        }
    }
}
