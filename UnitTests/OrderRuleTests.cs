using GFT_API_DB.Models;
using GFT_API_DB.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit.Sdk;

namespace UnitTests
{
    [TestClass]
    public class OrderRulesTests
    {
        #region Failure Tests Cases

        [TestMethod]
        public void TestFailureOrderContentEmpty()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = ""
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("error", order.OrderContents);
        }

        [TestMethod]
        public void TestFailureOrderContentNull()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = null
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("error", order.OrderContents);
        }

        [TestMethod]
        public void TestFailureOrderContentInvalidPeriod()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "bleh"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("error", order.OrderContents);
        }

        [TestMethod]
        public void TestFailureOrderContentNoComma()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Night123"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("error", order.OrderContents);
        }

        [TestMethod]
        public void TestFailureOrderContentCommaInvalid()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "bleh,nhjaks"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("error", order.OrderContents);
        }

        [TestMethod]
        public void TestFailureOrderContentNoCommaCase2()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Night.1;2/3'4"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("error", order.OrderContents);
        }

        #region Failure Morning Menu

        [TestMethod]
        public void TestFailureOrderContentBadMorningOrderDessert()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Morning,1,2,3,4"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("eggs, Toast, coffee, error", order.OrderContents);
        }

        [TestMethod]
        public void TestFailureOrderContentBadMorningOrderMultipleInvalidEntrée()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Morning,1,1,2"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("eggs, error", order.OrderContents);
        }

        [TestMethod]
        public void TestFailureOrderContentBadMorningOrderMultipleInvalidSide()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Morning,1,2,2"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("eggs, Toast, error", order.OrderContents);
        }

        #endregion

        #region Failure Night Menu

        [TestMethod]
        public void TestFailureOrderContentBadNightOrderMultipleInvalidEntrée()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Night,1,1,2"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("steak, error", order.OrderContents);
        }

        [TestMethod]
        public void TestFailureOrderContentBadMorningOrderMultipleInvalidDrink()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Night,1,2,3,3"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("steak, potato, wine, error", order.OrderContents);
        }

        [TestMethod]
        public void TestFailureOrderContentBadMorningOrderMultipleInvalidDessert()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Morning,1,2,3,4,4"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("eggs, Toast, coffee, error", order.OrderContents);
        }

        #endregion

        #endregion

        #region Positive Test Cases

        [TestMethod]
        public void TestSuccessMorningCaseBasic()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Morning,1,2,3"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("eggs, Toast, coffee", order.OrderContents);
        }

        [TestMethod]
        public void TestSuccessMorningCaseMultipleDrinks()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "morning,1,2,3,3"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("eggs, Toast, coffee(x2)", order.OrderContents);
        }

        [TestMethod]
        public void TestSuccessMorningCases()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "MORNING,1,2,3"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("eggs, Toast, coffee", order.OrderContents);

            Order order2 = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "MoRnInG, 2, 1, 3"
            };
            OrderRules.ProcessRules(order2);

            Assert.AreEqual("eggs, Toast, coffee", order2.OrderContents);

            Order order3 = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "morning,1,2,3,4"
            };
            OrderRules.ProcessRules(order3);

            Assert.AreEqual("eggs, Toast, coffee, error", order3.OrderContents);

            Order order4 = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "morning,1,2,3,3,3"
            };
            OrderRules.ProcessRules(order4);

            Assert.AreEqual("eggs, Toast, coffee(x3)", order4.OrderContents);
        }


        [TestMethod]
        public void TestSuccessNightCaseBasic()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "night,1,2,3,4"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("steak, potato, wine, cake", order.OrderContents);
        }

        [TestMethod]
        public void TestSuccessNightCaseMultipleSides()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "night,1,2,2,3,4"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("steak, potato(x2), wine, cake", order.OrderContents);
        }

        [TestMethod]
        public void TestSuccessNightCases()
        {
            Order order = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "night,1,2,3,4"
            };
            OrderRules.ProcessRules(order);

            Assert.AreEqual("steak, potato, wine, cake", order.OrderContents);

            Order order2 = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Night,1,2,2,4"
            };
            OrderRules.ProcessRules(order2);

            Assert.AreEqual("steak, potato(x2), cake", order2.OrderContents);

            Order order3 = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "NiGhT,1,2,3,5"
            };
            OrderRules.ProcessRules(order3);

            Assert.AreEqual("steak, potato, wine, error", order3.OrderContents);

            Order order4 = new Order
            {
                OrderId = 0,
                IsMorning = false,
                OrderContents = "Night, 1, 1, 2, 3, 5"
            };
            OrderRules.ProcessRules(order4);

            Assert.AreEqual("steak, error", order4.OrderContents);
        }

        #endregion
    }
}
