//  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
// TITLE: Currency Converter 
// NAME: Stacie Herda
// DATE: 09/03/2021
// EMAIL: stacie.herda@colorado.edu
// SUMMARY: Converts currency from one form to another using user-specified criteria,
//          database information or data received from a public API. User can also
//          make CRUD commands to modify the database.
//
//          Project is created with restrictions in place to keep all code within a
//          single class and containing the following public function:
//
//          public decimal ConvertCurrency(string currencyFrom, string currencyTo, decimal amount);
//
//  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  

//  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
// LIBRARIES
//  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //

// Import Library
using System.Data.SQLite;

// Declare Class for All Methods
class CurrencyConverter
{
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    // PUBLIC VARIABLES
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  

    // Define Public Static String for Database Connection String
    public static string ConnectionString = "Data Source=CurrencyDatabase.db;Version=3;";

    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    // MAIN
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    static void Main()
    {
        InitializeDatabase();   // Initializes SQLite Database
        TitleBanner();  // Displays Banner with Title
        MainPage(); // Displays Main Page
    }

    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    // MAIN METHODS SECTION
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  

    // Method to Set-Up SQLite Database
    static void InitializeDatabase()
    {
        CreateDatabaseAndTable();   // Create Database and Tables
        InitializeDatabaseWithDefaultValues();  // Insert Values
    }

    // Method for Banner 
    static void TitleBanner()
    {
        // Display Title
        Console.WriteLine("========================================================================\n");
        Console.WriteLine("PROGRAMMING EXERCISE: CURRENCY GENERATOR\n");
        Console.WriteLine("========================================================================\n");
    }

    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    // DATABASE INITIALIZATION SECTION
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  

    // Method to Create Database and Currency Table
    static void CreateDatabaseAndTable()
    {
        // If File Exists from Previous Run, Remove
        if (File.Exists("CurrencyDatabase.db"))
        {
            File.Delete("CurrencyDatabase.db");
        }
        
        // Use Connection String to Establish Connection
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();  // Open Connection to Database

            // Create Table
            using (var command = new SQLiteCommand(
                "CREATE TABLE CurrencyRates (CurrencyID INTEGER, CurrencyCode TEXT, ExchangeRate DOUBLE, PRIMARY KEY (CurrencyCode));",
                connection))
            {
                command.ExecuteNonQuery();  // Execute Command to Create Table
            }
        }
    }

    // Initialize Database Table
    static void InitializeDatabaseWithDefaultValues()
    {
        // Using Connection String Establish Connection
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();  // Open Connection

            // Count the Number of Rows in Database
            using (var commandCount = new SQLiteCommand(
                "SELECT COUNT(*) FROM CurrencyRates;",
                connection))
            {
                // Get Row Count
                int rowCount = Convert.ToInt32(commandCount.ExecuteScalar());

                // If Table is Empty
                if (rowCount == 0)
                {
                    // Insert Default Currency into Database Table
                    using (var commandInsert = new SQLiteCommand(
                        "INSERT INTO CurrencyRates (CurrencyID, CurrencyCode, ExchangeRate) VALUES " +
                        "(1, 'USD', 1.00)," +
                        "(2, 'PHP', 56.80)," +
                        "(3, 'EUR', 0.93)," +
                        "(4, 'GBP', 0.79)," +
                        "(5, 'JPY', 146.52)," +
                        "(6, 'CAD', 1.36)," +
                        "(7, 'AUD', 1.55)," +
                        "(8, 'CNY', 7.27)," +
                        "(9, 'INR', 82.78)," +
                        "(10, 'MXN', 17.18);",
                        connection))
                    {
                        commandInsert.ExecuteNonQuery();    // Execute Command
                    }
                }
            }
        }
    }

    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    // HELPER METHODS
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  

    // Currency Converter Method
    public static decimal ConvertCurrency(string currencyFrom, string currencyTo, decimal amount)
    {
        // Using Connection String Establish Connection
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();  // Open Connection

            // Get Exchange and Currency Rate In Row of Currency Code
            using (var command = new SQLiteCommand(
                "SELECT ExchangeRate FROM CurrencyRates WHERE CurrencyCode = @CurrencyCode;",
                connection))
            {
                // Execute Scalar to Get Currency From
                command.Parameters.AddWithValue("@CurrencyCode", currencyFrom);
                double exchangeRateFrom = Convert.ToDouble(command.ExecuteScalar());

                // Clear Out Parameter and Get Currency Desired
                command.Parameters.Clear(); 
                command.Parameters.AddWithValue("@CurrencyCode", currencyTo);
                double exchangeRateTo = Convert.ToDouble(command.ExecuteScalar());

                // If There Is an Exchange Rate To and From Execute Conversion
                if (exchangeRateFrom != 0 && exchangeRateTo != 0)
                {
                    decimal convertedAmount = amount * (decimal)(exchangeRateTo / exchangeRateFrom);    // Convert to Desired Currency
                    return Math.Round(convertedAmount, 2); // Return Rounded Result
                }

                // If Conversion Not Present
                else
                {
                    throw new Exception("Please Check Currencies or Exchange Rates.");  // Throw Exception if There Aren't Any Rates
                }
            }
        }
    }

    // Read User Input from Console
    static string ReadUserInput()
    {
        string? input = Console.ReadLine(); // Put Into Nullable Value
        return input?.ToUpper() ?? "";  // Return Value or Empty String
    }

    // Check if Currency Code is Valid
    static bool CurrencyCodeCheck(string currencyCode)
    {
        // Check if the currency code is valid
        if (currencyCode.Length != 3 || currencyCode.Any(char.IsDigit))
        {
            // Write Message About Invalid Currency Code
            Console.WriteLine("Invalid currency code.");
            Console.WriteLine("\n------------------------------------------------------------------------\n");

            return false;   // Return False if Fails Test
        }

        return true;    // Return True if Meets Criteria
    }


    // Method to Convert Currency from Database with User-Inpout
    static void ConvertCurrencyFromDatabase()
    {
        // Receive User Source Currency Code
        Console.Write("Enter the source currency code: ");
        string currencyFrom = ReadUserInput().ToUpper();

        // If Fails, Return to Main Page
        bool CheckFrom = CurrencyCodeCheck(currencyFrom);
        if (CheckFrom == false)
        {
            return;
        }

        // Receive User Target Currency Code
        Console.Write("Enter the target currency code: ");
        string currencyTo = ReadUserInput().ToUpper();

        // If Fails, Return to Main Page
        bool CheckTo = CurrencyCodeCheck(currencyTo);
        if (CheckTo == false)
        {
            return;
        }

        // Get User Amount for Conversion
        Console.Write("Enter the amount to convert: ");

        // Check if Decimal, Else Return to Main Page
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            Console.WriteLine("\n------------------------------------------------------------------------\n");
            return;
        }

        // Using Connection String, Read Data from Database
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            // Convert to Desired Currency
            decimal convertedAmount = ConvertCurrency(currencyFrom, currencyTo, amount);
            
            // Text to Display Result
            Console.WriteLine("\n========================================================================");
            Console.WriteLine("RESULT:");
            Console.WriteLine($"{Math.Round(amount, 2)} {currencyFrom} is currently valued at {Math.Round(convertedAmount, 2)} {currencyTo}");
            Console.WriteLine("from 09/04/2023 03:52:00 PM PT");
            Console.WriteLine("========================================================================\n");
        }
    }

    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    // MAIN PAGE
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  

    // Main Page
    static void MainPage()
    {
        Thread.Sleep(1500); // Lets User See Results Easily When Reloaded
        
        // Display Options
        Console.WriteLine("Please Enter a Corresponding Letter to Make a Selection.\n");
        Console.WriteLine("SELECTION:");
        Console.WriteLine("(D) Convert Currency with Data from Database?");
        Console.WriteLine("(A) Convert Currency with Data from an API?");
        Console.WriteLine("(M) Modify or Access Data in Database?");
        Console.WriteLine("(E) Exit?\n");
        Console.WriteLine("------------------------------------------------------------------------\n");
        Console.Write("Selection: ");

        // Obtain User Selection
        string choiceSource = ReadUserInput().ToUpper();

        // More Formatting
        Console.Write("\n");
        Console.WriteLine("------------------------------------------------------------------------\n");

        // Direct to Page Based on Input
        switch (choiceSource)
        {
            // Database Currency Conversion
            case "D":
            case "(D)":
                ConvertCurrencyFromDatabase(); 
                break;

            // API Currency Conversion
            case "A":
            case "(A)":
                ConvertCurrencyFromAPI().Wait();    // Wait for API Call to Complete
                break;

            // Modifications to Database
            case "M":
            case "(M)":
                DisplayCRUDPage(); 
                break;

            // Exit
            case "E":
            case "(E)":
                Environment.Exit(0);
                break;
            
             // Other, No Response
            default:
                break;
        }

        // Cycle Through Main Page
        MainPage();
    }


    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    // API METHODS
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  

    // Method to Convert Currency Through API Call
    static async Task ConvertCurrencyFromAPI()
    {
        // Receive User Source Currency Code
        Console.Write("Enter the source currency code: ");
        string currencyFrom = ReadUserInput().ToUpper();

        // If Fails, Return to Main Page
        bool CheckFrom = CurrencyCodeCheck(currencyFrom);
        if (CheckFrom == false)
        {
            return;
        }

        // Receive User Target Currency Code
        Console.Write("Enter the target currency code: ");
        string currencyTo = ReadUserInput().ToUpper();

        // If Fails, Return to Main Page
        bool CheckTo = CurrencyCodeCheck(currencyTo);
        if (CheckTo == false)
        {
            return;
        }

        // Ask for Amount to Be Converted
        Console.Write("Enter the amount to convert: ");
        
        // If Not A Number, Return to Main Page
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            Console.WriteLine("\n------------------------------------------------------------------------\n");
            return;
        }

        // Using HTTP Client for API
        using (var httpClient = new HttpClient())
        {
            try
            {
                // Get Timezone for Display
                TimeZoneInfo pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                DateTime pacificTime = TimeZoneInfo.ConvertTime(DateTime.Now, pacificTimeZone);

                // Display Time for API Call
                string APITimeStamp = pacificTime.ToString("MM/dd/yyyy hh:mm:ss tt");

                // Await HTTP Response
                HttpResponseMessage response = await httpClient.GetAsync("https://api.exchangerate.host/latest");

                // If Successful
                if (response.IsSuccessStatusCode)
                {
                    // Read JSON Content for the Exchange Rates
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    var exchangeRates = Newtonsoft.Json.JsonConvert.DeserializeObject<ExchangeRates>(jsonContent);

                    // If Currency To and From are Present in the Response
                    if (exchangeRates.Rates.ContainsKey(currencyFrom) && exchangeRates.Rates.ContainsKey(currencyTo))
                    {
                        // Calculate the Ratio for the Conversion
                        decimal exchangeRateFrom = exchangeRates.Rates[currencyFrom];
                        decimal exchangeRateTo = exchangeRates.Rates[currencyTo];

                        // Return the Conversion
                        decimal convertedAmount = amount * (exchangeRateTo / exchangeRateFrom);

                        // Display the Result
                        Console.WriteLine("\n========================================================================");
                        Console.WriteLine("RESULT:");
                        Console.WriteLine($"{Math.Round(amount, 2)} {currencyFrom} is currently valued at {Math.Round(convertedAmount, 2)} {currencyTo}");
                        Console.WriteLine("from " + APITimeStamp + " PT");
                        Console.WriteLine("========================================================================\n");
                    }

                    // If Unable to Find Currency Codes, Display Error
                    else
                    {
                        Console.WriteLine("Invalid Currency Code.");
                    }
                }

                // If Unsuccessful API Response, Display Error
                else
                {
                    Console.WriteLine("Unsuccessful API Connection.");
                }
            }

            // Display Other Exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // Exchange Rate Method
    class ExchangeRates
    {
        // Get the Rates from the JSON 
        public System.Collections.Generic.Dictionary<string, decimal>? Rates { get; set; }
    }

    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    // CRUD MENU METHOD
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  


    // Display CRUD Page Method
    static void DisplayCRUDPage()
    {
        Thread.Sleep(1500); // Lets User See Results Easily When Reloaded

        // Display CRUD Options
        Console.WriteLine("CRUD OPTIONS:");
        Console.WriteLine("(C) Create Currency Entry");
        Console.WriteLine("(R) Read Currency Data");
        Console.WriteLine("(U) Update Currency Entry");
        Console.WriteLine("(D) Delete Currency Entry");
        Console.WriteLine("(B) Back to Main Page\n");
        Console.WriteLine("------------------------------------------------------------------------\n");
        Console.Write("Selection: ");

        // Get User Input
        string choiceCRUD = ReadUserInput().ToUpper();

        // Formatting
        Console.WriteLine("\n------------------------------------------------------------------------\n");

        // Make Choice Depending on User Input
        switch (choiceCRUD)
        {
            // Create Currency With Associated Rate
            case "C":
            case "(C)":
                CreateCurrencyEntry();
                break;
            
            // Read Database Rates
            case "R":
            case "(R)":
                ReadCurrencyDataFromDatabase();
                break;
            
           // Update Currency Rate
            case "U":
            case "(U)":
                UpdateCurrencyRates();
                break;

            // Delete Currency from Table
            case "D":
            case "(D)":
                DeleteCurrencyEntry();
                break;

            // Break and Return to Main Page
            case "B":
            case "(B)":
                return;


            // Other, No Response
            default:
                break;
        }

        // Cycle Through CRUD Page
        DisplayCRUDPage();
    }

    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
    //  CRUD METHODS
    //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  

    // CRUD Entry Method
    static void CreateCurrencyEntry()
    {
        // Ask User for Input
        Console.Write("Enter the currency code: ");
        string currencyCode = ReadUserInput().ToUpper();

        // If Fails, Return to CRUD Page
        bool CheckFrom = CurrencyCodeCheck(currencyCode);
        if (CheckFrom == false)
        {
            return;
        }

        // Obtain Exchange Rate from User
        Console.Write("Enter the exchange rate: ");

        // Check if Double, Return if Invalid
        if (!double.TryParse(Console.ReadLine(), out double exchangeRate))
        {
            Console.WriteLine("Invalid exchange rate.");
            Console.WriteLine("\n------------------------------------------------------------------------\n");
            return;
        }

        // Using Connection String, Connect to Database
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();  // Open Connection

            // Insert Values into SQLite Database
            using (var command = new SQLiteCommand(
                "INSERT INTO CurrencyRates (CurrencyCode, ExchangeRate) VALUES (@CurrencyCode, @ExchangeRate);",
                connection))
            {
                // Declare SQL Parameters
                command.Parameters.AddWithValue("@CurrencyCode", currencyCode);
                command.Parameters.AddWithValue("@ExchangeRate", exchangeRate);

                // Formatting
                Console.WriteLine("\n========================================================================\n");

                // Check that Rows Were Inserted
                int rowsInserted = command.ExecuteNonQuery();

                // Sucessful Addition if Rows Added is 1
                if (rowsInserted == 1)
                {
                    // Display Message that Currency Code was Added
                    Console.WriteLine($"Currency {currencyCode} created.");
                }

                // If A New Row Isn't Added, Display Message
                else
                {
                    Console.WriteLine("Failed to add currency code.");
                }
            }
        }

        // Formatting
        Console.WriteLine("\n========================================================================\n");
    }

    // Read Currency Data from Database Method
    static void ReadCurrencyDataFromDatabase()
    {
        // Establish Connection to Database
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();  // Open Connection
            
            // Select Currency Codes and Exchange Rates from Table
            using (var command = new SQLiteCommand(
                "SELECT CurrencyCode, ExchangeRate FROM CurrencyRates;",
                connection))
            {
                using (var reader = command.ExecuteReader())    // Execute Command
                {
                    // Formatting for Display
                    Console.WriteLine("========================================================================");
                    Console.WriteLine("CURRENCY DATA FROM DATABASE:");

                    // Read from Table
                    while (reader.Read())
                    {
                        // Obtain Currency Code and Exchange Rate
                        string currencyCode = reader["CurrencyCode"]?.ToString() ?? "";
                        decimal exchangeRate = Convert.ToDecimal(reader["ExchangeRate"]);

                        // Write Lines for Each
                        Console.WriteLine($"{currencyCode}: {exchangeRate:F2}");    // F2 for Two Decimals
                    }

                    // Formatting
                    Console.WriteLine("\n========================================================================\n");
                }
            }
        }
    }

    // Method to Update the Currency Rates
    static void UpdateCurrencyRates()
    {
        // Ask User for Input on Currency Code to Update
        Console.Write("Enter the currency code to update: ");
        string currencyCode = ReadUserInput().ToUpper();    // Convert to Upper Case

        // If Fails, Return to CRUD Page
        bool CheckFrom = CurrencyCodeCheck(currencyCode);
        if (CheckFrom == false)
        {
            return;
        }

        // Ask User for New Exchange Rate
        Console.Write("Enter the new exchange rate: ");

        // Check if is a Double, Return to CRUD Page if Not
        if (!double.TryParse(Console.ReadLine(), out double newExchangeRate))
        {
            Console.WriteLine("Invalid exchange rate.");
            Console.WriteLine("\n========================================================================\n");
            return;
        }

        // Using Connection String, Connect to Database
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open(); // Open Connection

            // With SQL Command to Update Table
            using (var command = new SQLiteCommand(
                "UPDATE CurrencyRates SET ExchangeRate = @NewExchangeRate WHERE CurrencyCode = @CurrencyCode;",
                connection))
            {
                // Add Parameters for Updated Exchange Rate and Currency Code
                command.Parameters.AddWithValue("@NewExchangeRate", newExchangeRate);
                command.Parameters.AddWithValue("@CurrencyCode", currencyCode);

                // Formatting
                Console.WriteLine("\n========================================================================\n");

                // Check Rows Modified
                int rowsUpdated = command.ExecuteNonQuery();

                // If Rows Updated is One, Display Success
                if (rowsUpdated == 1)
                {
                    Console.WriteLine($"{currencyCode}'s exchange rate updated.");
                }

                // If Zero Rates Updated, Alert User that Row Not in Database
                else
                {
                    Console.WriteLine($"{currencyCode} not found.");
                }
            }
        }

        // Formatting
        Console.WriteLine("\n========================================================================\n");
    }

    // Method to Delete Currency
    static void DeleteCurrencyEntry()
    {
        // Ask User for Currency Code of Choice
        Console.Write("Enter the currency code to delete: ");
        string currencyCodeToDelete = ReadUserInput().ToUpper();

        // Connect to SQL Database Using Connection String
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();  // Open Connection

            // Delete from Currency Rate where Currency Code is User's Input
            using (var command = new SQLiteCommand(
                "DELETE FROM CurrencyRates WHERE CurrencyCode = @CurrencyCode;",
                connection))
            {
                command.Parameters.AddWithValue("@CurrencyCode", currencyCodeToDelete);

                // Formatting
                Console.WriteLine("\n========================================================================\n");

                // Get Row Count of Deleted Rows
                int rowsDeleted = command.ExecuteNonQuery();
                
                // If Rows Count Deleted is One, Display Success Message
                if (rowsDeleted == 1)
                {
                    Console.WriteLine($"{currencyCodeToDelete} deleted.");
                }

                // If Not Equal to One, Row Not Found in Database
                else
                {
                    Console.WriteLine($"{currencyCodeToDelete} not found.");
                }
            }
        }

        // Formatting
        Console.WriteLine("\n========================================================================\n");
    }
}
