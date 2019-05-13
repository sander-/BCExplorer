using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BCExplorer.Web.Models;
using BCExplorer.Services;

namespace BCExplorer.Web.Controllers
{
    public class HomeController : Controller
    {
        public IBlockService BlockService { get; set; }

        public HomeController(IBlockService blockService)
        {
            BlockService = blockService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var lastBlock = await BlockService.GetLastBlock();
                var latestBlocks = await BlockService.GetLatestBlocks(20);
                var vm = new IndexViewModel
                {
                    LastBlock = lastBlock,
                    LatestBlocks = latestBlocks
                };
                return View(vm);
            }
            catch (Exception)
            {
                return View("_NotFound");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
