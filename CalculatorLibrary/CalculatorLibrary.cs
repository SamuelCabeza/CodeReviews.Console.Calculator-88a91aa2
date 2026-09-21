using CalculatorLibrary.Controller;
using CalculatorLibrary.Model;
using CalculatorLibraryl;
using Newtonsoft.Json;
using Spectre.Console;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static CalculatorLibrary.Enums.Enums;

namespace CalculatorLibrary;

public class Calculator
{

    private readonly CalculationsController _calculatoinController = new();
    JsonWriter writer;
    int timesUsed = 0;


    public Calculator()
    {

        StreamWriter logFile = File.CreateText("calculatorlog.json");
        logFile.AutoFlush = true;
        writer = new JsonTextWriter(logFile);
        writer.Formatting = Formatting.Indented;
        writer.WriteStartObject();
        writer.WritePropertyName("Operations");
        writer.WriteStartArray();
    }


    public void MainMenu()
    {
        bool endApp = false;
        // Display title as the C# console calculator app.
        Console.WriteLine("Console Calculator in C#\r");
        Console.WriteLine("------------------------\n");

        while (!endApp)
        {
            //Times used
            Console.WriteLine("Times used: " + GetTimesUsed());

            // Ask the user to choose an operator.
            object? op = AskOption(0);

            // Validate input is not null, and matches the pattern


            switch (op)
            {
                case MenuOption.Addition:
                case MenuOption.Substraction:
                case MenuOption.Multiply:
                case MenuOption.Divide:
                case MenuOption.SquareRoot:
                case MenuOption.Power:
                case MenuOption.MultiplyX10:
                case MenuOption.Sin:
                    Calculate(op);
                    break;
                case MenuOption.ListCalculations:
                    GetCalculations();
                    break;
                case MenuOption.DeleteCalculation:
                    DeleteCalculation();
                    break;
                case MenuOption.PerformCalculationsWithPreviousAnswered:
                    CalculateWithPreviousCalculations();
                    break;
            }


                
            Console.WriteLine("------------------------\n");

            // Wait for the user to respond before closing.
            Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");
            if (Console.ReadLine() == "n") endApp = true;

            Console.WriteLine("\n"); // Friendly linespacing.
        }

        Finish();
        return;
    }

    public double DoOperation(double num1, double num2, object? op)
    {
        double result = double.NaN; // Default value is "not-a-number" if an operation, such as division, could result in an error.


        writer.WriteStartObject();
        writer.WritePropertyName("Operand1");
        writer.WriteValue(num1);
        writer.WritePropertyName("Operand2");
        writer.WriteValue(num2);
        writer.WritePropertyName("Operation");

        // Use a switch statement to do the math.
        switch (op)
        {
            
            case MenuOption.Addition:
                result = num1 + num2;
                writer.WriteValue("Add");
                AddCalculation(num1, num2, "+", result);
                break;
            case MenuOption.Substraction:
                result = num1 - num2;
                writer.WriteValue("Subtract");
                AddCalculation(num1, num2, "-", result);
                break;
            case MenuOption.Multiply:
                result = num1 * num2;
                writer.WriteValue("Multiply");
                AddCalculation(num1, num2, "*", result);
                break;
            case MenuOption.Divide:
                // Ask the user to enter a non-zero divisor.
                if (num2 != 0)
                {
                    result = num1 / num2;
                    AddCalculation(num1, num2, "/", result);
                }
                writer.WriteValue("Divide");
                break;
            case MenuOption.SquareRoot://Square root
                if (num1 >= 0)
                {
                    result = Math.Sqrt(num1);
                    AddCalculation(num1, 0, "√", result);
                }
                writer.WriteValue("Square Root");
                break;
            case MenuOption.Power://Power
                result = Math.Pow(num1, num2);
                writer.WriteValue("Power");
                AddCalculation(num1, num2, "^", result);
                break;
            case MenuOption.MultiplyX10: //10x
                result = num1 * 10;
                writer.WriteValue("x10");
                AddCalculation(num1, 0, "x10", result);
                break;
            case MenuOption.Sin: //Sine
                double radians = num1 * Math.PI / 180.0;
                result = Math.Sin(radians);
                writer.WriteValue("x10");
                AddCalculation(num1, 0, "sine", result);
                break;

            // Return text for an incorrect option entry.
            default:
                break;
        }

        writer.WritePropertyName("Result");
        writer.WriteValue(result);
        writer.WriteEndObject();
        timesUsed++;

        return result;
    }


    public void Finish()
    {
        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.Close();
    }

    // Get the number of times the calculator has been used
    public int GetTimesUsed()
    {
        return timesUsed;
    }

    //Save the operation and result in a list
    public void AddCalculation(double num1, double num2, string op, double result)
    {
  

        _calculatoinController.AddItem($"{num1} {op} {num2}", result);
    }

    public void GetCalculations()
    {
        _calculatoinController.ViewItems();
    }


    public void DeleteCalculation()
    {
        _calculatoinController.DeleteItem();
    }

    //0: all
    //1: Just mathematical operations
    public object AskOption(int option)
    {

        var prompt = new SelectionPrompt<MenuOption>()
            .Title("Choose an operator from the following list:");

        if (option == 0)
        {
            prompt.AddChoices(Menus.Complete);
        }
        else
        {
            prompt.AddChoices(Menus.Basic);
        }

        var menuChoice = AnsiConsole.Prompt(prompt);

        return menuChoice;

    }

    public void Calculate(object? op, string input1 = "", string input2 = "")
    {
        double result = 0;

        // Declare variables and set to empty.
        // Use Nullable types (with ?) to match type of System.Console.ReadLine
        string? numInput1 = input1;
        string? numInput2 = input2;


        // Ask the user to type the first number.
            
        if(numInput1 == "")
        {
            Console.Write("Type a number, and then press Enter: ");
            numInput1 = Console.ReadLine();
        }
                

        double cleanNum1 = 0;
        while (!double.TryParse(numInput1, out cleanNum1))
        {
            Console.Write("This is not valid input. Please enter a numeric value: ");
            numInput1 = Console.ReadLine();
        }

        double cleanNum2 = 0;
        // Ask the user to type the second number.
        if (numInput2 == "" && !Regex.IsMatch(op.ToString(), "^(SquareRoot|MultiplyX10|Sin)$"))
        {
            Console.Write("Type another number, and then press Enter: ");
            numInput2 = Console.ReadLine();

            while (!double.TryParse(numInput2, out cleanNum2))
            {
                Console.Write("This is not valid input. Please enter a numeric value: ");
                numInput2 = Console.ReadLine();
            }
        }else if (numInput2 != "")
        {
            while (!double.TryParse(numInput2, out cleanNum2))
            {
                Console.Write("This is not valid input. Please enter a numeric value: ");
                numInput2 = Console.ReadLine();
            }
        }
                
        try
        {
            result = DoOperation(cleanNum1, cleanNum2, op);
            if (double.IsNaN(result))
            {
                Console.WriteLine("This operation will result in a mathematical error.\n");
            }
            else Console.WriteLine("Your result: {0:0.##}\n", result);
        }
        catch (Exception e)
        {
            Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
        }
    }

    public void CalculateWithPreviousCalculations()
    {

        var latestCalculations = MockDatabase.CalculationItems;

        if (latestCalculations.Count < 2)
        {
            Console.WriteLine("Not enough previous calculations to perform this operation.");
            return;
        }

        var item1 = _calculatoinController.SelectItem();
        double num1 = item1.Answer;


        var item2 = _calculatoinController.SelectItem();
        double num2 = item2.Answer;


        object op = AskOption(1);


        Calculate(op, num1.ToString(), num2.ToString());
    }


}

