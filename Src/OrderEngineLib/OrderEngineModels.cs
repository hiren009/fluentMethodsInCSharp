using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderEngineLib
{
	public class OrderEngine
	{
		internal ITaxCalculator TaxCalculator {  get; set; }
		internal OrderProcessor OrderProcessor { get; set; }
		public Order Order { get; internal set; }

		public List<Func<Order, bool>> ValidateFunctions { get; internal set; }

		public static OrderEngine Initialize()
		{
			// Instantiate dependencies
			var orderEngine = new OrderEngine
			{
				Order = new Order(),
				TaxCalculator = new TaxCalculator(),
				OrderProcessor = new OrderProcessor(),
				ValidateFunctions = new List<Func<Order, bool>>()
			};

			orderEngine.Order.OrderLineItems = new List<OrderLineItem>();

			// Add a simplistic null check
			orderEngine.ValidateFunctions.Add(o => o != null && o.OrderLineItems != null);

			return orderEngine;
		}
	}

	public static class OrderEngineExtensions {

		/// <summary>
		/// Add customer id
		/// </summary>
		public static OrderEngine Customer(this OrderEngine orderEngine, int customerId)
		{
			orderEngine.Order.CustomerId = customerId;

			return orderEngine;
		}

		/// <summary>
		/// Add line item
		/// </summary>
		public static OrderEngine AddLineItem(this OrderEngine orderEngine, OrderLineItem orderLineItem)
		{
			if(orderEngine.Order == null)
			{
				orderEngine.Order = new Order();
			}

			if(orderEngine.Order.OrderLineItems == null)
			{
				orderEngine.Order.OrderLineItems = new List<OrderLineItem>();
			}

			orderEngine.Order.OrderLineItems.Add(orderLineItem);

			return orderEngine;
		}

		/// <summary>
		/// Process Order
		/// </summary>
		public static OrderEngine Process(this OrderEngine orderEngine)
		{
			// Can't instantiate an order for processing; need an order with details.
			if(orderEngine == null || orderEngine.Order == null)
			{
				throw new InvalidOperationException("Processing not provided an Order.");
			}

			if(orderEngine.TaxCalculator != null && orderEngine.TaxCalculator.Apply(orderEngine.Order))
			{
				orderEngine.TaxCalculator.Calculate(orderEngine.Order);
			}

			// Run thru any validation checks
			orderEngine.Order.Valid = true;

			if(orderEngine.ValidateFunctions != null)
			{
				foreach (var validateFunction in orderEngine.ValidateFunctions)
				{
					orderEngine.Order.Valid = orderEngine.Order.Valid && validateFunction(orderEngine.Order);
				}
			}

			// Process the order
			if (orderEngine.Order.Valid)
			{
				orderEngine.Order.OrderNum = $"O{new Random().Next(100, 999).ToString()}";
				orderEngine.Order.isProcessed = true;
			}

			return orderEngine;
		}


		/// <summary>
		/// Allows developers to easily alter the default behaviour, by swapping out the TaxCalculator
		/// </summary>
		public static OrderEngine Using(this OrderEngine orderEngine, ITaxCalculator taxCalculator)
		{
			orderEngine.TaxCalculator = taxCalculator;

			return orderEngine;
		}


		/// <summary>
		/// Allows for incorporating additional custom validation checks when order processing
		/// </summary>
		public static OrderEngine AddValidateFunction(this OrderEngine orderEngine, Func<Order, bool> validationFunction)
		{
			if (validationFunction == null) throw new ArgumentNullException("validationFunction");

			if(orderEngine.ValidateFunctions == null)
			{
				orderEngine.ValidateFunctions = new List<Func<Order, bool>>();
			}

			orderEngine.ValidateFunctions.Add(validationFunction);

			return orderEngine;
		}
	}
}
