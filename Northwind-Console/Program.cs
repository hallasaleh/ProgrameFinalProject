using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NLog;
using NorthwindConsole.Models;

namespace NorthwindConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("11) Add Product");
                    Console.WriteLine("12) Display All Products Field");
                    Console.WriteLine("13) Edit a specified record from the Products table");
                    Console.WriteLine("14) Display the Product Name for discontinued Product");
                    Console.WriteLine("21) Add Category");
                    Console.WriteLine("22) Edit a specified record from the Category table");
                    Console.WriteLine("23) Display Categories");
                    Console.WriteLine("24) Display Category and related products");
                    Console.WriteLine("25) Display all Categories and their related products");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "11")
                    {
                        Product product = new Product();
                        Console.WriteLine("Enter Product Name:");
                        product.ProductName = Console.ReadLine();
                        Console.WriteLine("Enter the Product QuantityPerunit:");
                        product.QuantityPerUnit = Console.ReadLine();
                        //try code
                        product.UnitPrice = 100;
                        product.UnitsInStock = 50;
                        product.UnitsOnOrder = 10;
                        product.ReorderLevel = 20;
                        product.Discontinued = true;
                        product.CategoryId = 1;
                        product.SupplierId = 3;

                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Products.Any(c => c.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.AddProduct(product);
                                logger.Info("Product added - {ProductName}", product.ProductName);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "12")
                    {
                        var db = new NorthwindContext();
                        var query = db.Products.OrderBy(p => p.ProductName);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductName} - {item.QuantityPerUnit} - {item.UnitPrice} - {item.UnitsInStock} - {item.UnitsOnOrder} - {item.ReorderLevel} - {item.Discontinued}");
                        }
                    }
                    else if (choice == "13")
                    {
                        // Edit product
                        Console.WriteLine("Choose the product to edit:");
                        var db = new NorthwindContext();
                        var product = GetProduct(db);
                        if (product != null)
                        {
                            // input category
                            Product UpdatedProduct = InputProduct(db);
                            if (UpdatedProduct != null)
                            {
                                UpdatedProduct.ProductID = product.ProductID;
                                db.EditProduct(UpdatedProduct);
                                logger.Info("Product (id: {productid}) updated", UpdatedProduct.ProductID);
                            }
                        }
                    }
                    else if (choice == "14")
                    {
                        var db = new NorthwindContext();
                        var query = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductName);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductName}"); //- {item.QuantityPerUnit} - {item.UnitPrice} - {item.UnitsInStock} - {item.UnitsOnOrder} - {item.ReorderLevel} - {item.Discontinued}");
                        }
                    }

                    else if (choice == "21")
                    {
                        //Add Category
                        Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                // TODO: save category to db
                                db.AddCategory(category);
                                logger.Info("Category added - {CategoryName}", category.CategoryName);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "22")
                    {
                        // Edit Category
                        Console.WriteLine("Choose the category to edit:");
                        var db = new NorthwindContext();
                        var category = GetCategory(db);
                        if (category != null)
                        {
                            // input categoy
                            Category UpdatedCategory = InputCategory(db);
                            if (UpdatedCategory != null)
                            {
                                UpdatedCategory.CategoryId = category.CategoryId;
                                db.EditCategory(UpdatedCategory);
                                logger.Info("Category (id: {categoryid}) updated", UpdatedCategory.CategoryId);
                            }
                        }
                    }
                    else if (choice == "23")
                    {
                        //Display Category
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                    }
                    else if (choice == "24")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products.Where(p => p.Discontinued == false))
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "25")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    Console.WriteLine();


                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }
        //Try this part
        public static Product InputProduct(NorthwindContext db)
        {
            Product product = new Product();
            Console.WriteLine("Enter the Product name");
            product.ProductName = Console.ReadLine();
            Console.WriteLine("Enter the Product QuantityPerUnit");
            product.QuantityPerUnit = Console.ReadLine();

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                return product;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return null;
        }
        public static Category InputCategory(NorthwindContext db)
        {
            Category category = new Category();
            Console.WriteLine("Enter the Category name");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("Enter the Category Discription");
            category.Description = Console.ReadLine();

            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                return category;
            }
            else
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return null;
        }

        public static Product GetProduct(NorthwindContext db)
        {
            var products = db.Products.OrderBy(b => b.ProductName);
            /*foreach (Product b in products)
            {
                Console.WriteLine(b.ProductName);
                if (b.Products.Count() == 0)
                {
                    Console.WriteLine($"  <no products>");
                }
                else
                {
                    foreach (Product p in b.product)
                    {
                        Console.WriteLine($"{p.ProductID} - {p.ProductName}");
                    }
                }
            }*/
            if (int.TryParse(Console.ReadLine(), out int ProductID))
            {
                Product product = db.Products.FirstOrDefault(p => p.ProductID == ProductID);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }
        public static Category GetCategory(NorthwindContext db)
        {
            var categories = db.Categories.OrderBy(b => b.CategoryName);
            if (int.TryParse(Console.ReadLine(), out int CategoryId))
            {
                Category category = db.Categories.FirstOrDefault(p => p.CategoryId == CategoryId);
                if (category != null)
                {
                    return category;
                }
            }
            logger.Error("Invalid Category Id");
            return null;
        }
    }
}
    

