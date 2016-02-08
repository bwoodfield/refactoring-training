using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Tusc
    {
        private static List<User> _users = new List<User>();
        private static List<Product> _products = new List<Product>();

        private static User uzr = new User();

        
        public static void Start()
        {
            LoadLists();
            // Write welcome message
            OutPutToConsole("Welcome to TUSC");
            OutPutToConsole("---------------");

            // Login
            bool loggedIn = false; // Is logged in?            

            // Validate Username
            bool valUsr = LogInUser(); // Is valid user?

            if (valUsr)
            {            
                loggedIn = true;

                // Show welcome message
                OutPutToConsole("Login successful! Welcome " + uzr.Name + "!",true, true, ConsoleColor.Green);
                                                
                // Show balance 
                OutPutToConsole("Your balance is " + uzr.Bal.ToString("C"), true);

                //storage variable for calculations
                var bal = uzr.Bal;
                           

                // Show product list
                while (true)
                {
                    // Prompt for user input
                    OutPutToConsole("What would you like to buy?", true);
                    for (int i = 0; i < 7; i++)
                    {
                        Product prod = _products[i];
                        OutPutToConsole(i + 1 + ": " + prod.Name + " (" + prod.Price.ToString("C") + ")");
                    }
                    OutPutToConsole(_products.Count + 1 + ": Exit");

                    // Prompt for user input
                    Console.WriteLine("Enter a number:");
                    string answer = Console.ReadLine();
                    int num = Convert.ToInt32(answer);
                    num = num - 1; /* Subtract 1 from number
                    num = num + 1 // Add 1 to number */

                    // Check if user entered number that equals product count
                    if (num == 7)
                    {
                        // Update balance
                        uzr.Bal = bal;
                                   
                        // Write out new balance
                        string json = JsonConvert.SerializeObject(_users, Formatting.Indented);
                        File.WriteAllText(@"Data/Users.json", json);

                        // Write out new quantities
                        string json2 = JsonConvert.SerializeObject(_products, Formatting.Indented);
                        File.WriteAllText(@"Data/Products.json", json2);


                        // Prevent console from closing
                        OutPutToConsole("Press Enter key to exit", true);
                        Console.ReadLine();
                        return;
                    }
                    else
                    {
                        OutPutToConsole("You want to buy: " + _products[num].Name, true);
                        OutPutToConsole("Your balance is " + bal.ToString("C"));

                        // Prompt for user input
                        OutPutToConsole("Enter amount to purchase:");
                        answer = Console.ReadLine();
                        int qty = Convert.ToInt32(answer);
                        var prod = _products[num];
                        //process the purchase request
                        ProcessPurchase(qty, prod, bal);
                       
                    }
                }
                    
                                
            }

            // Prevent console from closing
            OutPutToConsole("Press Enter key to exit", true);
            Console.ReadLine();
        }

        private static void OutPutToConsole(string outputText,bool newLine = false, bool clearScreen = false, ConsoleColor cColor = ConsoleColor.White)
        {
            if (clearScreen) Console.Clear();
            if (cColor != ConsoleColor.White) Console.ForegroundColor = cColor;
            Console.WriteLine();
            Console.WriteLine(outputText);
            Console.ResetColor();
        }

        private static bool LogInUser()
        {
            var isValid = true;
            var exit = false;
            
            while (!exit)
            {
                //request UserName
                OutPutToConsole("Enter Username:", true);
                string name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name))
                {
                    //Find user in list and prompt if not found
                    uzr = _users.Find(x => x.Name == name);

                    if (uzr == null)
                    {
                        OutPutToConsole("You entered an invalid user.", true, true, ConsoleColor.Red);
                        isValid = false;
                    }
                    else
                    {
                        OutPutToConsole("Enter Password:");
                        string pwd = Console.ReadLine();

                        //get user that matches username and password
                        uzr = _users.Find(x => (x.Name == name) && (x.Pwd == pwd));
                        if (uzr == null)
                        {
                            OutPutToConsole("You entered an invalid password.", true, true, ConsoleColor.Red);
                            isValid = false;
                        }
                        else
                        {
                            //valid user found exit login
                            exit = true;
                        }
                    }
                }
                else
                {
                    //exit login
                    isValid = false;
                    exit = true;
                }
            }
           
            return isValid;
        }

        private static void LoadLists()
        {
            // Load users from data file
            _users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(@"Data/Users.json"));

            // Load products from data file
            _products = JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText(@"Data/Products.json"));
        }

        private static void ProcessPurchase(int qty, Product prod, double bal)
        {
            // Check if balance - quantity * price is less than 0
            if (bal - prod.Price * qty < 0)
            {
                OutPutToConsole("You do not have enough money to buy that.", true, true, ConsoleColor.Red);
                return;
            }

            // Check if quantity is less than quantity
            if (prod.Qty <= qty)
            {
                OutPutToConsole("Sorry, " + prod.Name + " is out of stock", true, true, ConsoleColor.Red);
                return;
            }

            // Check if quantity is greater than zero
            if (qty > 0)
            {
                // Balance = Balance - Price * Quantity
                bal = bal - prod.Price * qty;

                // Quanity = Quantity - Quantity
                prod.Qty = prod.Qty - qty;

                OutPutToConsole("You bought " + qty + " " + prod.Name + Environment.NewLine + "Your new balance is " + bal.ToString("C"), true, true, ConsoleColor.Green);
            }
            else
            {
                // Quantity is less than zero
                OutPutToConsole("Purchase cancelled", true, true, ConsoleColor.Yellow);

            }
        }
    }

    
}
