﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Coalesce.Web.Controllers
{
    public class TestController : Controller
    {
        // GET: /<controller>/
        public IActionResult Table()
        {
            return View();
        }

        public IActionResult Person()
        {
            return View();
        }
    }
}