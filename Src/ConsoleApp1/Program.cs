
using OrderEngineLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	/// <summary>
	/// Based on brilliant article on fluent api in C# at https://www.red-gate.com/simple-talk/dotnet/net-framework/fluent-code-in-c
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{

			// Invoke NonFluidMethod First
			// NonFluidMethod();

			// Invoke Fluid Method
			FluidMethod();


			// Wait termination till user input
			Console.ReadLine();
		}

		/// <summary>
		/// Non Fluid Methods
		/// </summary>
		public static void NonFluidMethod()
		{
			// Create the order
			var orderLines = new List<OrderLineItem>
			{
				new OrderLineItem{ ProductId = 123, Quantity = 252, UnitPrice = 2.99M },
				new OrderLineItem{ ProductId = 234, Quantity = 125, UnitPrice = 3.99M }
			};

			var order = new Order
			{
				CustomerId = 93834,
				OrderLineItems = orderLines
			};

			// Apply taxes
			var taxCalculator = new TaxCalculator();
			if (taxCalculator.Apply(order))
			{
				taxCalculator.Calculate(order);
			}

			// Validate and process the order
			if (order != null && order.OrderLineItems != null)
			{
				var orderProcessor = new OrderProcessor();
				orderProcessor.Process(order);
			}

			// Get yaml serialize version, and log to console
			var outputString = YamlSerialize(order);
			Console.WriteLine(outputString);
		}

		/// <summary>
		/// Fluid method
		/// </summary>
		public static void FluidMethod()
		{
			OrderEngine orderEngineTaxing = OrderEngine
				.Initialize()
				.Customer(56789)
				.Using(new TaxCalculatorV2())
				.AddValidateFunction(x => x.ApplicableTax > 0)
				.AddLineItem(new OrderLineItem { ProductId = 123, Quantity = 8, UnitPrice = 143 })
				.AddLineItem(new OrderLineItem { ProductId = 456, Quantity = 25, UnitPrice = 373 })
				.Process();

			var order = orderEngineTaxing.Order;

			// Get yaml serialize version, and log to console
			var outputString = YamlSerialize(order);
			Console.WriteLine(outputString);
		}

		/// <summary>
		/// Convert input object to Yaml representation
		/// </summary>
		public static string YamlSerialize(object objInput)
		{
			// Console Log Output
			YamlDotNet.Serialization.Serializer serializer = new YamlDotNet.Serialization.Serializer();
			return serializer.Serialize(objInput);
		}

	}
}