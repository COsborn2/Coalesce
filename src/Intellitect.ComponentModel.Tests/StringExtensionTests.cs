﻿using Intellitect.ComponentModel.DataAnnotations;
using Intellitect.ComponentModel.Utilities;
using Intellitect.ComponentModel.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Intellitect.ComponentModel.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void AddUnique()
        {
            var list = new List<string>();
            list.Add("test");
            Assert.True(list.Count() == 1);
            list.AddUnique("test");
            Assert.True(list.Count() == 1);
            list.AddUnique("best");
            Assert.True(list.Count() == 2);

            var items = new List<string>();
            items.Add("test");
            items.Add("rest");
            list.AddUnique(items);
            Assert.True(list.Count() == 3);
        }
    }
}
