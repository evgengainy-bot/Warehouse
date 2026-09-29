using System;
using System.Collections.Generic;
using System.Numerics;

namespace WarehouseData.Models
{
    public enum OrderType
    {
        Income = 1,   
        Outcome = 2   
    }

    public class Order
    {
        public static BigInteger OrderCounter = BigInteger.Zero;

        public BigInteger OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderType OrderType { get; set; }
        public BigInteger WarehouseId { get; set; }
        public string Comment { get; set; } = string.Empty;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public Warehouse? Warehouse { get; set; }

        public Order() { }

        public Order(OrderType type, BigInteger warehouseId, string comment = "")
        {
            OrderId = ++OrderCounter;
            OrderDate = DateTime.Now;
            OrderType = type;
            WarehouseId = warehouseId;
            Comment = comment;
        }

        public string OrderTypeDisplay =>
            OrderType == OrderType.Income ? "Поступление" : "Списание";

        public decimal TotalSum
        {
            get
            {
                decimal sum = 0;
                foreach (var item in Items)
                    sum += item.PriceAtMoment * item.Quantity;
                return sum;
            }
        }
    }
}