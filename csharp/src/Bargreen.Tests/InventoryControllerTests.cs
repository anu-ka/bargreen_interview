using Bargreen.API.Controllers;
using Bargreen.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Bargreen.Tests
{
    public class InventoryControllerTests
    {
        private static void AssertInventoryBalance(InventoryBalance expected, InventoryBalance actual)
        {
            Assert.Equal(expected.ItemNumber, actual.ItemNumber);
            Assert.Equal(expected.PricePerItem, actual.PricePerItem);
            Assert.Equal(expected.QuantityOnHand, actual.QuantityOnHand);
            Assert.Equal(expected.WarehouseLocation, actual.WarehouseLocation);
        }

        private static void AssertAccountingBalance(AccountingBalance expected, AccountingBalance actual)
        {
            Assert.Equal(expected.ItemNumber, actual.ItemNumber);
            Assert.Equal(expected.TotalInventoryValue, actual.TotalInventoryValue);
        }
        [Fact]
        public async void InventoryController_Can_Return_Inventory_BalancesAsync()
        {
            var inventoryService = new Mock<IInventoryService>();
            var inventoryServiceUtility = new Mock<IInventoryServiceUtility>();

            var item1 = new InventoryBalance()
            {
                ItemNumber = "ABC123",
                PricePerItem = 7.5M,
                QuantityOnHand = 312,
                WarehouseLocation = "WLA1"
            };
            var item2 = new InventoryBalance()
            {
                ItemNumber = "ABC123",
                PricePerItem = 7.5M,
                QuantityOnHand = 146,
                WarehouseLocation = "WLA2"
            };
            var inventoryBalance = new List<InventoryBalance>() { item1, item2 };
            inventoryService.Setup((i) => i.GetInventoryBalances()).Returns(inventoryBalance);
            var controller = new InventoryController(inventoryService.Object, inventoryServiceUtility.Object);
            var result = (await controller.GetInventoryBalances()).ToList();

            Assert.Collection(result,
                item => AssertInventoryBalance(item1, item),
                item => AssertInventoryBalance(item2, item)
            );
        }

        [Fact]
        public async void InventoryController_Can_Return_Accounting_BalancesAsync()
        {
            var inventoryService = new Mock<IInventoryService>();
            var inventoryServiceUtility = new Mock<IInventoryServiceUtility>();

            var item1 = new AccountingBalance()
            {
                ItemNumber = "ABC123",
                TotalInventoryValue = 3435M
            };
            var item2 = new AccountingBalance()
            {
                ItemNumber = "ZZZ99",
                TotalInventoryValue = 1930.62M
            };
            var accountingBalance = new List<AccountingBalance>() { item1, item2 };
            inventoryService.Setup((i) => i.GetAccountingBalances()).Returns(accountingBalance);
            var controller = new InventoryController(inventoryService.Object, inventoryServiceUtility.Object);
            var result = (await controller.GetAccountingBalances()).ToList();

            Assert.Collection(result,
                item => AssertAccountingBalance(item1, item),
                item => AssertAccountingBalance(item2, item)
            );
        }

        [Fact]
        public void Controller_Methods_Are_Async()
        {
            var methods = typeof(InventoryController)
                .GetMethods()
                .Where(m => m.DeclaringType == typeof(InventoryController));

            Assert.All(methods, m =>
            {
                Type attType = typeof(AsyncStateMachineAttribute);
                var attrib = (AsyncStateMachineAttribute)m.GetCustomAttribute(attType);
                Assert.NotNull(attrib);
                Assert.Equal(typeof(Task), m.ReturnType.BaseType);
            });
        }
    }
}
