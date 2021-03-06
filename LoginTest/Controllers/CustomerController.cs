﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginTest.Models;
using static LoginTest.DataAccess.Business_Logic.CustomerProcessor;

namespace LoginTest.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult ViewCustomers()
        {
            return View(LoadCustomers());
        }

        [HttpPost]
        public ActionResult ViewCustomers(string searchInput)
        {
            //By default, return a list of products with search query
            var data = CustomerSearch(searchInput);

            //However, if the search query is null or white space, return all products
            if (String.IsNullOrWhiteSpace(searchInput))
            { data = LoadCustomers(); }

            return View(data);
        }


        public ActionResult Manage()
        {
            return View();
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string submit, Customer customer)
        {
            //Parameters to check (the reason we do this is because the number is not generated by the user)
            List<string> stringsToCheck = new List<string>
                { "Name", "Address", "City", "State"};

            foreach (string item in stringsToCheck)
            {
                //If one field is wrong, we do not move forward
                if (!ModelState.IsValidField(item))
                {
                    return View();
                }
            }

            //Only add to database if physical button is clicked, no refreshes etc.
            if (submit == "Add Customer")
            {
                //SQL Create Product
                CreateCustomer(customer.Name, customer.Address, customer.City, customer.State);

                //After that, retrieve the product so that we can tell the user the product number information
                customer = GetByAllButNumber(customer.Name, customer.Address, customer.City, customer.State);
                ViewBag.State = "Success";
                ViewBag.CustomerID = customer.ID;
                ViewBag.ProductName = customer.Name;
                Customer emptyCustomer = new Customer();
                return View(emptyCustomer);
            }

            return View(customer);
        }

        [HttpPost]
        public ActionResult Manage(string submit, Customer customer)
        {
            //Button clicked state machine
            switch (submit)
            {
                //Search button clicked
                case "Search":
                    //Take the number input and grab product from DB
                    if (ModelState.IsValidField("Number"))
                    {
                        customer = GetCustomerByID(customer.ID);
                        //var _product = GetProductFromID(product.Number);

                        if (customer.Name != null)
                            ViewBag.Valid = "True";
                        if (customer.Name == null)
                            ViewBag.Message = "The number entered didn't match any records";

                    }
                    break;

                //Submit button clicked
                case "Submit Changes":

                    //Still show the table even upon reload
                    ViewBag.Valid = "True";
                    //This function will execute only if the modified details are valid
                    if (ModelState.IsValid)
                    {
                        //SQL MODIFY FUNCTION
                        int recordsAffected = ModifyCustomer(customer);

                        ViewBag.State = "Success";
                    }
                    break;
                default:
                    break;
            }
            return View(customer);
        }
    }
}