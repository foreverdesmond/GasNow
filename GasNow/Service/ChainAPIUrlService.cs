using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GasNow.Module;
using GasNow.Dto;

namespace GasNow.Service
{
    public class ChainAPIUrlService
    {
        private readonly GasNowDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainAPIUrlService"/> class.
        /// </summary>
        /// <param name="context">The database context used to access the ChainAPIUrls.</param>
        /// <param name="mapper">The mapper used for converting between entities and DTOs.</param>
        public ChainAPIUrlService(GasNowDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all ChainAPIUrl entries as a list of <see cref="ChainAPIUrlDto"/>.
        /// </summary>
        /// <returns>A list of <see cref="ChainAPIUrlDto"/> representing all API URLs.</returns>
        public List<ChainAPIUrlDto> GetAllChainAPIUrls()
        {
            return _mapper.Map<List<ChainAPIUrlDto>>(_context.ChainAPIUrls.ToList());
        }

        /// <summary>
        /// Retrieves a specific ChainAPIUrl by its unique identifier.
        /// </summary>
        /// <param name="guid">The unique identifier of the API URL.</param>
        /// <returns>
        /// A <see cref="ChainAPIUrlDto"/> representing the API URL, or null if not found.
        /// </returns>
        public ChainAPIUrlDto GetChainAPIUrlById(Guid guid)
        {
            var apiUrl = _context.ChainAPIUrls.Find(guid);
            return apiUrl == null ? null : _mapper.Map<ChainAPIUrlDto>(apiUrl);
        }

        /// <summary>
        /// Retrieves a list of ChainAPIUrls that match the specified API name.
        /// </summary>
        /// <param name="apiName">The name of the API to search for.</param>
        /// <returns>A list of <see cref="ChainAPIUrlDto"/> matching the specified API name.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during the retrieval process.</exception>
        public List<ChainAPIUrlDto> GetChainAPIUrlsByAPIName(string apiName)
        {
            try
            {
                var apiUrls = _context.ChainAPIUrls
                    .Where(c => c.APIName.ToLower() == apiName.ToLower())
                    .ToList();
                return _mapper.Map<List<ChainAPIUrlDto>>(apiUrls);
            }
            catch (Exception ex)
            {
                throw; // Rethrow the exception for higher-level handling
            }
        }

        /// <summary>
        /// Retrieves a list of ChainAPIUrls that match the specified network ID.
        /// </summary>
        /// <param name="networkId">The network ID to filter the API URLs.</param>
        /// <returns>A list of <see cref="ChainAPIUrlDto"/> matching the specified network ID.</returns>
        public List<ChainAPIUrlDto> GetChainAPIUrlsByNetworkID(int networkId)
        {
            var apiUrls = _context.ChainAPIUrls
                .Where(c => c.NetworkID == networkId)
                .ToList();
            return _mapper.Map<List<ChainAPIUrlDto>>(apiUrls);
        }

        /// <summary>
        /// Adds a new ChainAPIUrl entry to the database.
        /// </summary>
        /// <param name="apiUrlDto">The API URL data transfer object to add.</param>
        public void AddChainAPIUrl(ChainAPIUrlDto apiUrlDto)
        {
            var apiUrl = _mapper.Map<ChainAPIUrl>(apiUrlDto);
            _context.ChainAPIUrls.Add(apiUrl);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates an existing ChainAPIUrl entry in the database.
        /// </summary>
        /// <param name="apiUrlDto">The API URL data transfer object containing updated data.</param>
        public void UpdateChainAPIUrl(ChainAPIUrlDto apiUrlDto)
        {
            var apiUrl = _context.ChainAPIUrls.Find(apiUrlDto.GUID);
            if (apiUrl != null)
            {
                _mapper.Map(apiUrlDto, apiUrl);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a ChainAPIUrl entry from the database by its unique identifier.
        /// </summary>
        /// <param name="guid">The unique identifier of the API URL to delete.</param>
        public void DeleteChainAPIUrl(Guid guid)
        {
            var apiUrl = _context.ChainAPIUrls.Find(guid);
            if (apiUrl != null)
            {
                _context.ChainAPIUrls.Remove(apiUrl);
                _context.SaveChanges();
            }
        }
    }
}
