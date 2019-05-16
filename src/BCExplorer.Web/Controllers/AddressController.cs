using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCExplorer.Services;
using BCExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BCExplorer.Web.Controllers
{
    public class AddressController : Controller
    {
        const int ItemsOnPage = 20;

        public IBlockService BlockService { get; set; }
        public IAddressService AddressService { get; set; }

        public AddressController(IBlockService blockService, IAddressService addressService)
        {
            BlockService = blockService;
            AddressService = addressService;
        }

        [Route("address/{id}")]
        public async Task<IActionResult> Index(string id, int page)
        {
            var address = await AddressService.GetAddress(id);

            if (address == null)
                return View("_NotFound");

            var offset = ItemsOnPage * page;
                        
            int max;
            if (offset < address.TotalTransactions && offset + ItemsOnPage < address.TotalTransactions)
            {
                max = offset + ItemsOnPage;
            }
            else
            {
                max = address.TotalTransactions;
            }

            var viewModel = new AddressViewModel()
            {
                Address = address,
                Count = (int)Math.Ceiling((decimal)address.TotalTransactions / ItemsOnPage),
                CurrentPage = page,
                OffSet = offset,
                Max = max
            };
            viewModel.CalculateTotals();
            return View(viewModel);
        }
    }
}