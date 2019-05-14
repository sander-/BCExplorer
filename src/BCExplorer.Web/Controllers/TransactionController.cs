using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCExplorer.Services;
using BCExplorer.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BCExplorer.Web.Controllers
{
    public class TransactionController : Controller
    {
        readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Route("transaction/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            try
            {
                if (String.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index", "Home");
                }

                var transaction = await _transactionService.GetTransaction(id);

                var vm = new TransactionViewModel()
                {
                    Transaction = transaction
                };

                if (vm.Transaction == null)
                {
                    return View("_NotFound");
                }

                return View(vm);
            }
            catch (Exception)
            {
                return View("_NotFound");
            }
        }
    }
}