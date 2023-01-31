using System;
using System.Collections.Generic;
using System.Text;

namespace Bargreen.Services
{
    public class InventoryReconciliationResult
    {
        public string ItemNumber { get; set; }
        public decimal TotalValueOnHandInInventory { get; set; }
        public decimal TotalValueInAccountingBalance { get; set; }
    }

    public class InventoryBalance
    {
        public string ItemNumber { get; set; }
        public string WarehouseLocation { get; set; }
        public int QuantityOnHand { get; set; }
        public decimal PricePerItem { get; set; }
    }

    public class AccountingBalance
    {
        public string ItemNumber { get; set; }
        public decimal TotalInventoryValue { get; set; }
    }

    internal class BalanceAmounts
    {
        public decimal ValueOnInventory { get; set; }
        public decimal ValueInAccounting { get; set; }
    }

    public interface IInventoryService
    {
        IEnumerable<InventoryBalance> GetInventoryBalances();

        IEnumerable<AccountingBalance> GetAccountingBalances();
    }

    public interface IInventoryServiceUtility
    {
        IEnumerable<InventoryReconciliationResult> ReconcileInventoryToAccounting( 
             IEnumerable<InventoryBalance> inventoryBalances, 
             IEnumerable<AccountingBalance> accountingBalances
            );
    }

    public class InventoryServiceUtility : IInventoryServiceUtility
    {
        public IEnumerable<InventoryReconciliationResult> ReconcileInventoryToAccounting(
                    IEnumerable<InventoryBalance> inventoryBalances, 
                    IEnumerable<AccountingBalance> accountingBalances)
        {
            // TODO-CHALLENGE: Compare inventory balances to accounting balances and find differences
            // The method should return information that indicates where inventory balances match or mismatch the accounting balances.

            IDictionary<string, BalanceAmounts> balances = new Dictionary<string, BalanceAmounts>();

            foreach (InventoryBalance item in inventoryBalances)
            {
                if (!balances.ContainsKey(item.ItemNumber))
                {
                    balances.Add(item.ItemNumber, new BalanceAmounts());
                }
                balances[item.ItemNumber].ValueOnInventory = item.QuantityOnHand * item.PricePerItem;
            }
            foreach (AccountingBalance item in accountingBalances)
            {
                if (!balances.ContainsKey(item.ItemNumber))
                {
                    balances.Add(item.ItemNumber, new BalanceAmounts());
                }
                balances[item.ItemNumber].ValueInAccounting = item.TotalInventoryValue;
            }

            IList<InventoryReconciliationResult> results = new List<InventoryReconciliationResult>();
            foreach (var balance in balances)
            {
                InventoryReconciliationResult res = new InventoryReconciliationResult
                {
                    ItemNumber = balance.Key,
                    TotalValueOnHandInInventory = balance.Value.ValueOnInventory,
                    TotalValueInAccountingBalance = balance.Value.ValueInAccounting
                };
                results.Add(res);
            }
            return results;
        }
    }

    public class InventoryService : IInventoryService
    {
        public IEnumerable<InventoryBalance> GetInventoryBalances()
        {
            return new List<InventoryBalance>()
            {
                new InventoryBalance()
                {
                     ItemNumber = "ABC123",
                     PricePerItem = 7.5M,
                     QuantityOnHand = 312,
                     WarehouseLocation = "WLA1"
                },
                new InventoryBalance()
                {
                     ItemNumber = "ABC123",
                     PricePerItem = 7.5M,
                     QuantityOnHand = 146,
                     WarehouseLocation = "WLA2"
                },
                new InventoryBalance()
                {
                     ItemNumber = "ZZZ99",
                     PricePerItem = 13.99M,
                     QuantityOnHand = 47,
                     WarehouseLocation = "WLA3"
                },
                new InventoryBalance()
                {
                     ItemNumber = "zzz99",
                     PricePerItem = 13.99M,
                     QuantityOnHand = 91,
                     WarehouseLocation = "WLA4"
                },
                new InventoryBalance()
                {
                     ItemNumber = "xxccM",
                     PricePerItem = 245.25M,
                     QuantityOnHand = 32,
                     WarehouseLocation = "WLA5"
                },
                new InventoryBalance()
                {
                     ItemNumber = "xxddM",
                     PricePerItem = 747.47M,
                     QuantityOnHand = 15,
                     WarehouseLocation = "WLA6"
                }
            };
        }

        public IEnumerable<AccountingBalance> GetAccountingBalances()
        {
            return new List<AccountingBalance>()
            {
                new AccountingBalance()
                {
                     ItemNumber = "ABC123",
                     TotalInventoryValue = 3435M
                },
                new AccountingBalance()
                {
                     ItemNumber = "ZZZ99",
                     TotalInventoryValue = 1930.62M
                },
                new AccountingBalance()
                {
                     ItemNumber = "xxccM",
                     TotalInventoryValue = 7602.75M
                },
                new AccountingBalance()
                {
                     ItemNumber = "fbr77",
                     TotalInventoryValue = 17.99M
                }
            };
        }
    }
}