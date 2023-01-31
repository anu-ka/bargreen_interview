using Bargreen.Services;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Bargreen.Tests
{
    public class InventoryServiceTests
    {
        private void AssertInventoryReconciliationResult(InventoryReconciliationResult expected, InventoryReconciliationResult actual)
        {
            Assert.Equal(expected.ItemNumber, actual.ItemNumber);
            Assert.Equal(expected.TotalValueOnHandInInventory, actual.TotalValueOnHandInInventory);
            Assert.Equal(expected.TotalValueInAccountingBalance, actual.TotalValueInAccountingBalance);
        }
        [Fact]
        public void Inventory_Reconciliation_InventoryAndAccountHasSameItem_Performs_As_Expected()
        {
            var inventoryBalance = new List<InventoryBalance>()
            {
                new InventoryBalance()
                {
                    ItemNumber = "ABC123",
                    PricePerItem = 7.5M,
                    QuantityOnHand = 312,
                    WarehouseLocation = "WLA1"
                }
            };
            var accountingBalance = new List<AccountingBalance>()
            {
                new AccountingBalance()
                {
                    ItemNumber = "ABC123",
                    TotalInventoryValue = 3435M
                }
            };

            var expectedResult = new InventoryReconciliationResult()
            {
                ItemNumber = "ABC123",
                TotalValueOnHandInInventory = 7.5M * 312,
                TotalValueInAccountingBalance = 3435M
            };

            IInventoryServiceUtility util = new InventoryServiceUtility();
            var actual = util.ReconcileInventoryToAccounting(inventoryBalance, accountingBalance);
            Assert.Collection(actual, item => AssertInventoryReconciliationResult(expectedResult, item));
        }

        [Fact]
        public void Inventory_Reconciliation_InventoryAndAccountNoSameItem_Performs_As_Expected()
        {
            var inventoryBalance = new List<InventoryBalance>()
            {
                new InventoryBalance()
                {
                     ItemNumber = "xxddM",
                     PricePerItem = 747.47M,
                     QuantityOnHand = 15,
                     WarehouseLocation = "WLA6"
                }
            };
            var accountingBalance = new List<AccountingBalance>()
            {
                new AccountingBalance()
                {
                    ItemNumber = "ABC123",
                    TotalInventoryValue = 3435M
                }
            };

            var expectedResult = new List<InventoryReconciliationResult>()
            {
                new InventoryReconciliationResult()
                {
                ItemNumber = "xxddM",
                TotalValueOnHandInInventory = 747.47M * 15,
                TotalValueInAccountingBalance = 0
                },
                new InventoryReconciliationResult()
                {
                ItemNumber = "ABC123",
                TotalValueOnHandInInventory = 0,
                TotalValueInAccountingBalance = 3435M
                }
            };

            IInventoryServiceUtility util = new InventoryServiceUtility();
            IList<InventoryReconciliationResult> actual = ((IList<InventoryReconciliationResult>)util.ReconcileInventoryToAccounting(inventoryBalance, accountingBalance));
            Assert.Collection(actual,
                item => AssertInventoryReconciliationResult(expectedResult[0], item),
                item => AssertInventoryReconciliationResult(expectedResult[1], item));
        }
    }
}
