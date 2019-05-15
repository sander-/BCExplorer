using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCExplorer.Services;
using BCExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BCExplorer.Web.Controllers
{
    public class BlockController : Controller
    {
        const int ItemsOnPage = 20;

        public IBlockService BlockService { get; set; }

        public BlockController(IBlockService blockService)
        {
            BlockService = blockService;
        }

        [Route("block/{id}")]
        public async Task<IActionResult> Index(string id, int page = 0)
        {
            var block = await BlockService.GetBlock(id);
            if (block == null)
            {
                return View("_NotFound");
            }

            var offset = ItemsOnPage * page;
            int max;
            if (offset < block.TotalTransactions && offset + ItemsOnPage < block.TotalTransactions)
            {
                max = offset + ItemsOnPage;
            }
            else
            {
                max = block.TotalTransactions;
            }

            var viewModel = new BlockViewModel()
            {
                Block = block,
                Count = (int)Math.Ceiling((decimal)block.TotalTransactions / ItemsOnPage),
                CurrentPage = page,
                OffSet = offset,
                Max = max
            };

            return View(viewModel);
        }
    }
}