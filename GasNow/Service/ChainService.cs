using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System;
using GasNow.Module;
using GasNow.Dto;

namespace GasNow.Service
{
    public class ChainService
    {
        private readonly GasNowDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainService"/> class.
        /// </summary>
        /// <param name="context">The database context used to access the Chains.</param>
        /// <param name="mapper">The mapper used for converting between entities and DTOs.</param>
        public ChainService(GasNowDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all chains as a list of <see cref="ChainDto"/>.
        /// </summary>
        /// <returns>A list of <see cref="ChainDto"/> representing all chains.</returns>
        public List<ChainDto> GetAllChains()
        {
            return _mapper.Map<List<ChainDto>>(_context.Chains.ToList());
        }

        /// <summary>
        /// Retrieves a specific chain by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the chain.</param>
        /// <returns>
        /// A <see cref="ChainDto"/> representing the chain, or null if not found.
        /// </returns>
        public ChainDto GetChainById(int id)
        {
            var chain = _context.Chains.Find(id);
            return chain == null ? null : _mapper.Map<ChainDto>(chain);
        }

        /// <summary>
        /// Retrieves a specific chain by its name.
        /// </summary>
        /// <param name="chainName">The name of the chain to search for.</param>
        /// <returns>
        /// A <see cref="ChainDto"/> representing the chain, or null if not found.
        /// </returns>
        public ChainDto GetChainByName(string chainName)
        {
            var chain = _context.Chains.FirstOrDefault(c => c.ChainName.ToLower() == chainName.ToLower());
            return chain == null ? null : _mapper.Map<ChainDto>(chain);
        }

        /// <summary>
        /// Adds a new chain to the database.
        /// </summary>
        /// <param name="chainDto">The chain data transfer object to add.</param>
        public void AddChain(ChainDto chainDto)
        {
            var chain = _mapper.Map<Chain>(chainDto);
            _context.Chains.Add(chain);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates an existing chain in the database.
        /// </summary>
        /// <param name="chainDto">The chain data transfer object containing updated data.</param>
        public void UpdateChain(ChainDto chainDto)
        {
            var chain = _context.Chains.Find(chainDto.NetworkID);
            if (chain != null)
            {
                _mapper.Map(chainDto, chain);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a chain from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the chain to delete.</param>
        public void DeleteChain(int id)
        {
            var chain = _context.Chains.Find(id);
            if (chain != null)
            {
                _context.Chains.Remove(chain);
                _context.SaveChanges();
            }
        }
    }
}
