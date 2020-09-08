using GFT_API_DB.Helper;
using GFT_API_DB.Models;
using System;
using System.Collections.Generic;

namespace GFT_API_DB.Rules
{
    public class OrderRules
    {
        //1. You must enter time of day as “morning” or “night”
        //2. You must enter a comma delimited list of dish types with at least one selection
        //3. The output must print food in the following order: entrée, side, drink, dessert
        //4. There is no dessert for morning meals
        //5. Input is not case sensitive
        //6. If invalid selection is encountered, display valid selections up to the error, then print error
        //7. In the morning, you can order multiple cups of coffee
        //8. At night, you can have multiple orders of potatoes
        //9. Except for the above rules, you can only order 1 of each dish type

        public static void ProcessRules(Order order)
        {
            order.OrderId = 0;
            order.OrderContents = ReadAndProcessInput(order);
        }

        private static string ReadAndProcessInput(Order order)
        {
            string output = "error";

            if (CheckInputIsValid(order))
            {
                GenerateOutput(order);
                return order.OrderContents;
            }

            return output;
        }

        private static bool CheckInputIsValid(Order order)
        {
            if (!string.IsNullOrWhiteSpace(order.OrderContents))
            {
                //2. You must enter a comma delimited list of dish types with at least one selection
                string[] values = SplitOrderContentsForPeriod(order.OrderContents);
                if (values != null && values.Length > 1 && CheckPeriod(values[0], order))
                {
                    return true;
                }
            }
            return false;
        }

        private static string[] SplitOrderContentsForPeriod(string input)
        {
            //2. You must enter a comma delimited list of dish types with at least one selection
            return input.Trim().Split(",");
        }

        private static string[] SplitOrderContentsForOrder(string input)
        {
            //2. You must enter a comma delimited list of dish types with at least one selection
            string[] splittedInput = input.Split(",");
            List<string> orderValues = new List<string>();
            for (int inputLenghtAux = 1; inputLenghtAux < splittedInput.Length; inputLenghtAux++)
            {
                orderValues.Add(splittedInput[inputLenghtAux]);
            }
            return orderValues.ToArray();
        }

        private static bool CheckPeriod(string period, Order order)
        {
            //1. You must enter time of day as “morning” or “night”
            //5. Input is not case sensitive
            bool isMorning = IsMorning(period);
            bool isNight = IsNight(period);
            if (isMorning)
            {
                order.IsMorning = true;
                return true;
            }
            if (isNight)
            {
                order.IsMorning = false;
                return true;
            }
            return false;
        }

        private static bool IsMorning(string period)
        {
            int morning = string.Compare(period, "morning", true);
            if (morning == 0)
            {
                return true;
            }
            return false;
        }

        private static bool IsNight(string period)
        {
            int night = string.Compare(period, "night", true);
            if (night == 0)
            {
                return true;
            }
            return false;
        }

        private static void GenerateOutput(Order order)
        {
            //6. If invalid selection is encountered, display valid selections up to the error, then print error
            //9. You can only order 1 of each dish type, there are exceptions for each period
            if (order.IsMorning)
            {
                ProcessMorningOrder(order);
            }
            else
            {
                ProcessNightOrder(order);
            }
        }

        private static void ProcessMorningOrder(Order order)
        {
            //3 The output must print food in the following order: entrée, side, drink, dessert. TODO
            //4. There is no dessert for morning meals
            //7. In the morning, you can order multiple cups of coffee(xTimes)

            string[] morningOrderInput = SplitOrderContentsForOrder(order.OrderContents);
            string entrée = "";
            string side = "";
            string drink = "";
            string error = "";
            string[] morningOrderOutput;
            bool errorFound = false;
            int coffeeMultiplier = 1;
            foreach (string dish in morningOrderInput)
            {
                switch (dish.Trim())
                {
                    case "1":
                        if ((string.IsNullOrWhiteSpace(entrée) || !entrée.Contains(Enum.GetName(typeof(Menu.MorningMenu), Menu.MorningMenu.eggs))))
                        {
                            entrée = (Enum.GetName(typeof(Menu.MorningMenu), Menu.MorningMenu.eggs));
                        }
                        else
                        {
                            error = "error";
                            errorFound = true;
                        }
                        break;
                    case "2":
                        if ((string.IsNullOrWhiteSpace(side) || !side.Contains(Enum.GetName(typeof(Menu.MorningMenu), Menu.MorningMenu.Toast))))
                        {
                            side = Enum.GetName(typeof(Menu.MorningMenu), Menu.MorningMenu.Toast);
                        }
                        else
                        {
                            error = "error";
                            errorFound = true;
                        }
                        break;
                    case "3":
                        if ((string.IsNullOrWhiteSpace(drink) || !drink.Contains(Enum.GetName(typeof(Menu.MorningMenu), Menu.MorningMenu.coffee))))
                        {
                            drink = Enum.GetName(typeof(Menu.MorningMenu), Menu.MorningMenu.coffee);
                        }
                        else
                        {
                            coffeeMultiplier++;
                            drink = string.Concat(Enum.GetName(typeof(Menu.MorningMenu), Menu.MorningMenu.coffee), "(x", coffeeMultiplier, ")");
                        }
                        break;
                    default:
                        error = "error";
                        errorFound = true;
                        break;
                }
                if (errorFound) break;
            }
            morningOrderOutput = new string[] { entrée, side, drink, error };
            order.OrderContents = UniteDishes(morningOrderOutput);
        }

        private static void ProcessNightOrder(Order order)
        {
            //3 The output must print food in the following order: entrée, side, drink, dessert.
            string[] nightOrderInput = SplitOrderContentsForOrder(order.OrderContents);
            string entrée = "";
            string side = "";
            string drink = "";
            string dessert = "";
            string error = "";
            string[] nightOrderOutput;
            bool errorFound = false;
            int potatoMultiplier = 1;
            foreach (string dish in nightOrderInput)
            {
                switch (dish.Trim())
                {
                    case "1":
                        if (string.IsNullOrWhiteSpace(entrée) || !entrée.Contains(Enum.GetName(typeof(Menu.NightMenu), Menu.NightMenu.steak)))
                        {
                            entrée = (Enum.GetName(typeof(Menu.NightMenu), Menu.NightMenu.steak));
                        }
                        else
                        {
                            error = "error";
                            errorFound = true;
                        }
                        break;
                    case "2":
                        //8. At night, you can have multiple orders of potatoes (xTimes)
                        if (string.IsNullOrWhiteSpace(side) || !side.Contains(Enum.GetName(typeof(Menu.NightMenu), Menu.NightMenu.potato)))
                        {
                            side = Enum.GetName(typeof(Menu.NightMenu), Menu.NightMenu.potato);
                        }
                        else
                        {
                            potatoMultiplier++;
                            side = (string.Concat(Enum.GetName(typeof(Menu.NightMenu), Menu.NightMenu.potato), "(x", potatoMultiplier, ")"));
                        }
                        break;
                    case "3":
                        if (string.IsNullOrWhiteSpace(side) || !drink.Contains(Enum.GetName(typeof(Menu.NightMenu), Menu.NightMenu.wine)))
                        {
                            drink = Enum.GetName(typeof(Menu.NightMenu), Menu.NightMenu.wine);
                        }
                        else
                        {
                            error = "error";
                            errorFound = true;
                        }
                        break;
                    case "4":
                        if (string.IsNullOrWhiteSpace(dessert) || !dessert.Contains(Enum.GetName(typeof(Menu.NightMenu), Menu.NightMenu.cake)))
                        {
                            dessert = Enum.GetName(typeof(Menu.NightMenu), Menu.NightMenu.cake);
                        }
                        else
                        {
                            error = "error";
                            errorFound = true;
                        }
                        break;
                    default:
                        error = "error";
                        errorFound = true;
                        break;
                }
                if (errorFound) break;
            }
            nightOrderOutput = new string[] { entrée, side, drink, dessert, error };
            order.OrderContents = UniteDishes(nightOrderOutput);
        }

        private static string UniteDishes(string[] dishes)
        {
            List<string> orderedDishesOutput = new List<string>();
            for (int dishesLenghtAux = 0; dishesLenghtAux < dishes.Length; dishesLenghtAux++)
            {
                if (!string.IsNullOrWhiteSpace(dishes[dishesLenghtAux]))
                {
                    orderedDishesOutput.Add(dishes[dishesLenghtAux]);
                }
            }
            return string.Join(", ", orderedDishesOutput.ToArray());
        }
    }
}
