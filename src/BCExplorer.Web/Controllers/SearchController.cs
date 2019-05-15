using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCExplorer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BCExplorer.Web.Controllers
{
    public class SearchController : Controller
    {
        readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [Route("search/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var res = await _searchService.Search(id);

            switch (res)
            {
                case SearchResult.Block:
                    return RedirectToAction("Index", "Block", new { id = id });
                case SearchResult.Transaction:
                    return RedirectToAction("Index", "Transaction", new { id = id });
                case SearchResult.Address:
                    return RedirectToAction("Index", "Address", new { id = id });
            }

            return View("_NotFound");
        }
    }
}