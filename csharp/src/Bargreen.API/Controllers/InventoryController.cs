using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bargreen.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bargreen.API.Controllers
{
    //TODO-CHALLENGE: Make the methods in this controller follow the async/await pattern
    //TODO-CHALLENGE: Use dotnet core dependency injection to inject the InventoryService
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService inventoryService;
        private readonly IInventoryServiceUtility inventoryServiceUtility;

        public InventoryController(IInventoryService service, IInventoryServiceUtility serviceUtility)
        {
            this.inventoryService = service;
            this.inventoryServiceUtility = serviceUtility;
        }
        [Route("InventoryBalances")]
        [HttpGet]
        public async Task<IEnumerable<InventoryBalance>> GetInventoryBalances()
        {
            return await Task.Run(() => this.inventoryService.GetInventoryBalances());
        }

        [Route("AccountingBalances")]
        [HttpGet]
        public async Task<IEnumerable<AccountingBalance>> GetAccountingBalances()
        {
            return await Task.Run(() => this.inventoryService.GetAccountingBalances());
        }

        [Route("InventoryReconciliation")]
        [HttpGet]
        public async Task<IEnumerable<InventoryReconciliationResult>> GetReconciliation()
        {
            return await Task.Run(() => this.inventoryServiceUtility.ReconcileInventoryToAccounting(
                this.inventoryService.GetInventoryBalances(),
                this.inventoryService.GetAccountingBalances())
            );
        }
    }
}