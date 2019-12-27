using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderEngineLib
{
	public class OrderLineItem
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
	}


	public class Order
	{
		public int CustomerId { get; set; }
		public List<OrderLineItem> OrderLineItems { get; set; }

		public decimal ApplicableTax { get; set; }

		public string OrderNum { get; set; }
		public bool isProcessed { get; set; } = false;
		public bool Valid { get; set; } = true;
	}


	public interface ITaxCalculator
	{
		bool Apply(Order order);
		void Calculate(Order order);
	}

	public class TaxCalculator : ITaxCalculator
	{
		public bool Apply(Order order)
		{
			var TotalPrice = order.OrderLineItems.Sum(x => x.Quantity * x.UnitPrice);
			return (TotalPrice > 1000);
		}

		public void Calculate(Order order)
		{
			var TotalPrice = order.OrderLineItems.Sum(x => x.Quantity * x.UnitPrice);
			order.ApplicableTax = TotalPrice * 10 / 100;
		}
	}

	public class TaxCalculatorV2 : ITaxCalculator
	{
		public bool Apply(Order order)
		{
			var TotalPrice = order.OrderLineItems.Sum(x => x.Quantity * x.UnitPrice);
			return (TotalPrice > 1000);
		}

		public void Calculate(Order order)
		{
			var TotalPrice = order.OrderLineItems.Sum(x => x.Quantity * x.UnitPrice);
			order.ApplicableTax = TotalPrice * 15 / 100;
		}
	}

	public class OrderProcessor
	{
		public void Process(Order order)
		{
			order.OrderNum = $"O{new Random().Next(100, 999).ToString()}";
			order.isProcessed = true;
		}
	}

}
