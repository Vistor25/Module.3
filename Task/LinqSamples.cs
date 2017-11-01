// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
	[Title("LINQ Module")]
	[Prefix("Linq")]
	public class LinqSamples : SampleHarness
	{

		private DataSource dataSource = new DataSource();

		[Category("Restriction Operators")]
		[Title("Where - Task 1")]
		[Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
		public void Linq1()
		{
			int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

			var lowNums =
				from num in numbers
				where num < 5
				select num;

			Console.WriteLine("Numbers < 5:");
			foreach (var x in lowNums)
			{
				Console.WriteLine(x);
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 2")]
		[Description("This sample return return all presented in market products")]

		public void Linq2()
		{
			var products =
				from p in dataSource.Products
				where p.UnitsInStock > 0
				select p;

			foreach (var p in products)
			{
				ObjectDumper.Write(p);
			}
		}

        [Category("Restriction Operators")]
        [Title("Where - Task 1")]
        [Description("This sample return return all presented in market products")]
        public void Linq001()
        {
            var clients = dataSource.Customers.Where(customer => customer.Orders?.Sum(order => order.Total) > 1000);
            foreach(var p in clients)
            {
                ObjectDumper.Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 2")]
        [Description("This sample return return all presented in market products")]
        public void Linq002()
        {
            var suppliers =
                from c in dataSource.Customers
                from s in dataSource.Suppliers
                where c.Country == s.Country && c.City == s.City
                select c.CompanyName;
            //var customers = dataSource.Customers
            //   .Select(customer => new {
            //       Customer = customer,
            //       Suppliers = dataSource.Suppliers
            //   .Where(supplier => supplier.Country == customer.Country && supplier.City == customer.City)
            //   });
            foreach (var s in suppliers)
            {
                ObjectDumper.Write(s);
            }
                       
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 3")]
        [Description("This sample return return all presented in market products")]
        public void Linq003()
        {
            var customers = dataSource.Customers.Where(customer => customer.Orders?.DefaultIfEmpty(new Order()).Max(order => order.Total) > 10000);
            foreach (var s in customers)
            {
                ObjectDumper.Write(s);
            }

        }

        [Category("Restriction Operators")]
        [Title("Where - Task 4")]
        [Description("This sample return return all presented in market products")]
        public void Linq004()
        {
            var customers = dataSource.Customers.Where(customer => customer.Orders.Count() != 0)
                .Select(customer => new { customer = customer, Year = customer.Orders?.Min(order => order.OrderDate.Year),
                    Month = customer.Orders?.Min(order => order.OrderDate.Month)});
            foreach (var s in customers)
            {
                ObjectDumper.Write(s);
            }

        }

        [Category("Restriction Operators")]
        [Title("Where - Task 5")]
        [Description("This sample return return all presented in market products")]
        public void Linq005()
        {
            var customers = dataSource.Customers.Where(customer => customer.Orders.Count() != 0)
                .Select(customer => new {
                    customer = customer,
                    Year = customer.Orders?.Min(order => order.OrderDate.Year),
                    Month = customer.Orders?.Min(order => order.OrderDate.Month)
                }).OrderBy(customer => customer.Year).ThenBy(customer=> customer.Month)
                .ThenByDescending(customer => customer.customer.Orders?.Sum(order => order.Total))
                .ThenBy(customer => customer.customer.CompanyName);
            foreach (var s in customers)
            {
                ObjectDumper.Write(s);
            }

        }

        [Category("Restriction Operators")]
        [Title("Where - Task 6")]
        [Description("This sample return return all presented in market products")]
        public void Linq006()
        {
            var customers = dataSource.Customers.
                Where(customer => customer.PostalCode?.All(symbol => Char.IsDigit(symbol))!=true 
                || String.IsNullOrEmpty(customer.Region) 
                || customer.Phone.StartsWith("("));
            foreach (var s in customers)
            {
                ObjectDumper.Write(s);
            }

        }

        [Category("Restriction Operators")]
        [Title("Where - Task 7")]
        [Description("This sample return return all presented in market products")]
        public void Linq007()
        {
            var numberGroups = from p in dataSource.Products
                               group p by new { p.Category, p.UnitsInStock, p.UnitPrice } into g
                               orderby g.Key.UnitPrice
                               select new { g.Key, g };
            //var products =
            //    dataSource.Products.GroupBy(
            //        product =>
            //            new
            //            {
            //                Category = product.Category,
            //                //UnitsInStock = product.UnitsInStock,
            //                //Price = product.UnitPrice
            //            }).OrderBy(product => product.Key.Price);
            var products = dataSource.Products.GroupBy(product => product.Category)
                .Select(group => new {Group = group.Key, InStock = group.GroupBy(g => g.UnitsInStock)
                .Select(g => new {Key = g.Key, Cost = g.OrderBy(x => x.UnitPrice)})});
            foreach (var product in products)
            {
                ObjectDumper.Write(product);
            }

        }

        [Category("Restriction Operators")]
        [Title("Where - Task 8")]
        [Description("This sample return return all presented in market products")]
        public void Linq008()
        {
            decimal LowCostUpperBound = 15;
            decimal AverageCostUpperBound = 30;
            var products = dataSource.Products.Select(product => new
                {
                    Name = product.ProductName,
                    Cost = product.UnitPrice,
                    CostCategory =
                    product.UnitPrice <= LowCostUpperBound
                        ? "Low"
                        : product.UnitPrice > LowCostUpperBound && product.UnitPrice <= AverageCostUpperBound
                            ? "Average"
                            : "High"
                })
                .OrderBy(c => c.Cost)
                .GroupBy(x => x.CostCategory);
            foreach (var p in products)
            {
                ObjectDumper.Write(p);
            }

        }

        [Category("Restriction Operators")]
        [Title("Where - Task 9")]
        [Description("This sample return return all presented in market products")]
        public void Linq009()
        {
            var customers = dataSource.Customers.Select(customer => customer.City).Distinct()
                .Select(city => new
                {
                    city = city,
                    AverageIncome = dataSource.Customers
                        .Where(customer => customer.City == city)
                        .Average(customer => customer.Orders?.DefaultIfEmpty(new Order()).Average(order => order.Total)),
                    AverageIntensivity = dataSource.Customers
                        .Where(customer => customer.City == city).Average(customer => customer.Orders?.Length)
                });
            foreach (var c in customers)
            {
                ObjectDumper.Write(c);
            }

        }

        [Category("Restriction Operators")]
        [Title("Where - Task 10")]
        [Description("This sample return return all presented in market products")]
        public void Linq010()
        {
            var months = dataSource.Customers.SelectMany(customer => customer.Orders)
                .GroupBy(order => order.OrderDate.Month)
                .Select(month => new{ Month = month.Key, Orders = month.Select(x => x.OrderID).Count()});
            var years = dataSource.Customers.SelectMany(customer => customer.Orders)
                .GroupBy(order => order.OrderDate.Year)
                .Select(year => new { Year = year.Key, Orders = year.Select(x => x.OrderID).Count() });
            var yearsAndmonths = dataSource.Customers.SelectMany(customer => customer.Orders)
                .GroupBy(order => new { Year = order.OrderDate.Year, Months = order.OrderDate.Month} )
                .Select(monthandyear => new { Year = monthandyear.Key.Year, Month = monthandyear.Key.Months, Orders = monthandyear.Select(x => x.OrderID).Count() }).OrderBy(x => x.Year).ThenBy(x => x.Month);
            foreach (var c in yearsAndmonths)
            {
                ObjectDumper.Write(c);
            }

        }
    }
}
