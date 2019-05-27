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
        public async Task<IActionResult> Index(string id, int page = 1)
        {
            var address = await AddressService.GetAddress(id, page, ItemsOnPage);

            if (address == null)
                return View("_NotFound");

            var pageCount = (int)Math.Ceiling((decimal)address.TotalTransactions / ItemsOnPage);

            var viewModel = new AddressViewModel()
            {
                Address = address,
                CurrentPage = page,
                PageCount = pageCount,
                TotalReceived = address.TotalReceived,
                TotalSent = address.TotalSent,
            };

            return View(viewModel);
        }
    }
}