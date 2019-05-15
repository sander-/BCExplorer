using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BCExplorer.Services
{
    public interface ISearchService
    {
        Task<SearchResult> Search(string id);
    }

    public class SearchService : ISearchService
    {
        readonly IBlockService _blockService;
        readonly IAddressService _addressService;
        readonly ITransactionService _transactionService;

        public SearchService(IBlockService blockService,
            IAddressService addressService,
            ITransactionService transactionService)
        {
            _blockService = blockService;
            _addressService = addressService;
            _transactionService = transactionService;
        }

        public async Task<SearchResult> Search(string id)
        {
            var block = await _blockService.GetBlock(id);

            if (block != null)
                return SearchResult.Block;

            var transaction = await _transactionService.GetTransaction(id);
            if (transaction != null)
                return SearchResult.Transaction;

            var address = await _addressService.GetAddress(id);
            if (address != null)
                return SearchResult.Address;

            return SearchResult.NotFound;
        }
    }
}
