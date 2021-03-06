﻿using SportsSoft.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsSoft.Controllers
{
    public class CustomerController : Controller
    {
        private SportingContext context { get; set; }

        public CustomerController(SportingContext ctxt)
        {
            context = ctxt;
        }

        [Route("Customers")]
        public ViewResult Manager()
        {
            var customers = context.Customers
                .Include(c => c.Country)
                .OrderBy(c => c.FirstName)
                .ToList();
            return View(customers);
        }

        public ViewResult Details(int id)
        {
            var customer = context.Customers
                .Include(c => c.Country)
                .FirstOrDefault(i => i.CustomerId == id);
            return View(customer);
        }

        [HttpGet]
        public ViewResult Add()
        {
            ViewBag.Action = "Add";
            ViewBag.Value = "Submit";
            List<Country> CountryList= context.Countries.OrderBy(c => c.CountryName).ToList();
            ViewBag.Countries = new SelectList(CountryList, "CountryId", "CountryName");
            return View("Edit" , new Customer());
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            ViewBag.Action = "Edit";
            ViewBag.Value = "Apply";
            ViewBag.Countries = new SelectList(context.Countries.OrderBy(c => c.CountryName).ToList(), "CountryId", "CountryName");

            var customer = context.Customers
                .Include(c => c.Country)
                .FirstOrDefault(c => c.CustomerId == id);
            return View("Edit" , customer);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer)
        {
            string action = (customer.CustomerId == 0) ? "Add" : "Edit";

            if (ModelState.IsValid)
            {
                if (action == "Add")
                {
                    context.Customers.Add(customer);
                }
                else
                {
                    context.Customers.Update(customer);
                }
                context.SaveChanges();
                TempData["message"] = $"\"{customer.FullName}\" customer is {action}ed Successfully";
                return RedirectToAction("Manager");
            }
            else
            {
                ViewBag.Action = "Add";
                ViewBag.Countries = new SelectList(context.Countries.OrderBy(c => c.CountryName).ToList(), "CountryId", "CountryName");
                return View(customer);
            }
        }

        [HttpGet]
        public ViewResult Delete(int id)
        {
            var customer = context.Customers
                .Include(c => c.Country)
                .FirstOrDefault(c => c.CustomerId == id);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToActionResult Delete(Customer customer)
        {
            context.Remove(context.Customers.Single(c => c.CustomerId == customer.CustomerId));
            context.SaveChanges();
            TempData["message"] = $"\"{customer.FirstName} {customer.LastName}\" customer is deleted successfully";
            return RedirectToAction("Manager");
        }
    }
}
