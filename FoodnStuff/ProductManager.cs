﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace FoodnStuff
{
    public class ProductManager
    {
        private static ProductManager instance = null;
        // Main list that contains everything available
        public List<Product> Inventory { get; set; } = new List<Product>();
        // Use this dictionary to create a Listbox or something similar and use the keys as category name then grab the list with that key and unpack all products
        public Dictionary<string, List<Product>> CategoryDictionary { get; set; } = new Dictionary<string, List<Product>>();
        public List<string> SuggestionList = new List<string>();
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Transport> Transports { get; set; } = new List<Transport>();
        public  List<Transport> availableTransports = new List<Transport>();
        // IDManager gives the product unique IDs
        private static int productID = 0;
        public int ProductID
        {
            get
            {
                return productID++;
            }
            set
            {
                if (productID == 0)
                {
                    productID = value;
                }
            }
        }
        private ProductManager()
        {

        }
        public void SendTransport(Transport transport)
        {
            transport.Delivered();
        }
        public void CartToOrder(Cart cart, string address, string name)
        {
            if (cart == null) { return; }
            if (cart.ProductsInCart.Count() <= 0) { return; }
            Orders.Add(new Order(cart.ProductsInCart, address, name));
        }

        public bool LoadTansport(Transport transport, Order order)
        {
            int quantity = order.CheckTotalOrderQuantity();

            if (transport.Capacity + quantity <= transport.MaxCapacity)
            {
                transport.OrdersOutOnDelivery.Add(order);
                transport.Capacity += quantity;
                return true;
            }
            else
            {
                MessageBox.Show("This order does not fit this transport");
                return false;
            }
        }
        private bool InventoryCheck(Order order)
        {
            bool exist = false;

            // Checks inventory
            var itemList = order.InCart;
            foreach (var item in itemList)
            {
                foreach (var product in Inventory)
                {
                    if (item.ID == product.ID)
                    {
                        if (item.Quantity <= product.Quantity)
                        {
                            exist = true;

                        }
                    }
                }
                if (!exist)
                {
                    return false;
                }
            }
            return true;
        }
        public List<Transport> CheckTransportAvailability(Order order)
        {
            availableTransports.Clear();

            foreach (Transport transport in Transports)
            {
                int quantity = order.CheckTotalOrderQuantity();

                if (transport.Available && transport.Capacity >= quantity && !availableTransports.Contains(transport))
                {
                    availableTransports.Add(transport);
                }
            }
            return availableTransports;
        }
        public static ProductManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ProductManager();
                instance.Transports.Add(new Transport(1));
                instance.Transports.Add(new Transport(2));
                instance.Transports.Add(new Transport(3));
            }
            return instance;
        }

        // Function that adds the product to cart
        public void AddToCart(Cart _myCart, Product _product, int _quantity)
        {
            if (_product.Quantity >= _quantity)
            {
                Product cartProduct = new Product(_product, _quantity);

                _myCart.AddProduct(cartProduct);
                _product.Quantity -= _quantity;
            }
            else
            {
                MessageBox.Show("Not enough quantity available");
            }
        }
            // Function doesnt require login and therefor works for both logged in and anonoymous users 
            public bool AddOrder(Cart _checkoutCart, string _myAddress, string _myName)
        {
            // Fail checking
            if (_checkoutCart == null || _checkoutCart.ProductsInCart.Count == 0) return false;
            //This is a list of our products in the order
            
            // Creates new Order and adds it to the Order list
            Orders.Add(new Order(_checkoutCart.ProductsInCart, _myAddress, _myName));
            // Returns true if successfully added order otherwise false
            return true;
        }

        // Function goes through Inventory and sorts them into our dictionary

        public void CategorySorter()
        {
            CategoryDictionary.Clear();
            SuggestionList.Clear();
            // Create a list of products that have the same category
            List<Product> CategoryList = new List<Product>();
            string currentKey = string.Empty;
            
            // Makes sure we have an inventory
            if (Inventory.Count <= 0) { return; }

            // This is now redudant code... maybe
            //// Go through every item in our inventory and creates a dictionary using their category as key
            //for (int i = 0; i < Inventory.Count; i++)
            //{
            //    // If the Dictionary does not contain the key then we add it
            //    if (!CategoryDictionary.ContainsKey(Inventory[i].Category))
            //    {
            //        CategoryDictionary.Add(Inventory[i].Category, CategoryList);
            //    }
            //}

            // Go through every item in our inventory and adds it to the dictionary list depending on category
            foreach (Product product in Inventory)
            {
                //MessageBox.Show(product.Category);
                // If we find the category of product then we take the list and add product to it
                if (CategoryDictionary.TryGetValue(product.Category, value: out var myList))
                {
                    // Adds product to the List
                    if (!myList.Contains(product))
                    {
                        SuggestionList.Add(product.Name);
                        myList.Add(product);
                    }
                }
                else
                {
                    // If the category/key does not exist then we create it.
                    myList = new List<Product> {product};
                    SuggestionList.Add(product.Name);
                    CategoryDictionary.Add(product.Category, myList);
                }
            }
        }
        public void CreateProduct(string _category, string _name, int _price, int _quantity)
        {
            // Increment ID
            int productID = ProductID;
            // This creates a new product and adds it to the inventory
            Inventory.Add(new Product(_category, _name, _price, _quantity, productID));
            MessageBox.Show($"{Inventory.Count} has been added");
            // Updates categories
            CategorySorter();
        }

        public void RemoveFromCart(Cart _myCart, Product _product, int _quantity)
        {
            bool FoundItem = false;
            foreach(Product product in Inventory)
            {
                if (product.ID == _product.ID)
                {
                    FoundItem = true;
                    product.Quantity += _quantity;
                    break;
                }
            }
            if (!FoundItem)
            {
                Inventory.Add(_product);
            }
            _myCart.RemoveProduct(_product);
        }

        public void UpdateProduct(string _category, string _name, int _price, int _quantity)
        {

        }
    }
}
