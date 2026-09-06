using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CalculatorLibrary
{
    public class Calculator
    {

        JsonWriter writer;
        int timesUsed = 0;
        List<(string Operation, double Answer)> latestCalculations = new List<(string Operation, double Answer)>();



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

        public double DoOperation(double num1, double num2, string op)
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
                case "a":
                    result = num1 + num2;
                    writer.WriteValue("Add");
                    AddCalculation(num1, num2, "+", result);
                    break;
                case "s":
                    result = num1 - num2;
                    writer.WriteValue("Subtract");
                    AddCalculation(num1, num2, "-", result);
                    break;
                case "m":
                    result = num1 * num2;
                    writer.WriteValue("Multiply");
                    AddCalculation(num1, num2, "*", result);
                    break;
                case "d":
                    // Ask the user to enter a non-zero divisor.
                    if (num2 != 0)
                    {
                        result = num1 / num2;
                        AddCalculation(num1, num2, "/", result);
                    }
                    writer.WriteValue("Divide");
                    break;
                case "sr"://Square root
                    if (num1 >= 0)
                    {
                        result = Math.Sqrt(num1);
                        AddCalculation(num1, 0, "√", result);
                    }
                    writer.WriteValue("Square Root");
                    break;
                case "p"://Power
                    result = Math.Pow(num1, num2);
                    writer.WriteValue("Power");
                    AddCalculation(num1, num2, "^", result);
                    break;
                case "tn": //10x
                    result = num1 * 10;
                    writer.WriteValue("x10");
                    AddCalculation(num1, 0, "x10", result);
                    break;
                case "sine": //Sine
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
            latestCalculations.Add(($"{num1} {op} {num2}", result));
        }

        public void GetCalculations()
        {
            int index = 0;

            Console.WriteLine("\n Your latest calculations:");
            foreach (var calculation in latestCalculations)
            {
                Console.WriteLine($"Index: {index}; Calculation: {calculation.Operation} = {calculation.Answer}");
                index++;
            }
        }


        public void DeleteCalculation()
        {
            GetCalculations();

            Console.WriteLine("\n Please write the index you wish to delete:");

            int index = GetValidIndex();


            if (index >= 0 && index < latestCalculations.Count)
            {
                latestCalculations.RemoveAt(index);

                Console.WriteLine("Calculation deleted successfully.");
            }
        }

        //0: all
        //1: Just mathematical operations
        public string AskOption(int option)
        {

            switch (option)
            {
                case 0:
                    Console.WriteLine("Choose an operator from the following list:");
                    Console.WriteLine("\ta    - Add");
                    Console.WriteLine("\ts    - Subtract");
                    Console.WriteLine("\tm    - Multiply");
                    Console.WriteLine("\td    - Divide");
                    Console.WriteLine("\tsr   - Square Root");
                    Console.WriteLine("\tp    - Power");
                    Console.WriteLine("\ttn   - 10x");
                    Console.WriteLine("\tsine - Sine");
                    Console.WriteLine("\tl    - List Calculations");
                    Console.WriteLine("\tc    - Delete Calculations");
                    Console.WriteLine("\tpc   - Perform Calculations with two answer from previous calculations");
                    break;
                case 1:
                    Console.WriteLine("Choose an operator from the following list:");
                    Console.WriteLine("\ta    - Add");
                    Console.WriteLine("\ts    - Subtract");
                    Console.WriteLine("\tm    - Multiply");
                    Console.WriteLine("\td    - Divide");
                    Console.WriteLine("\tsr   - Square Root");
                    Console.WriteLine("\tp    - Power");
                    Console.WriteLine("\ttn   - 10x");
                    Console.WriteLine("\tsine - Sine");
                    break;
            }

            Console.Write("Your option? ");

            return Console.ReadLine();
        }

        public void Calculate(string op, string input1 = "", string input2 = "")
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
            if (numInput2 == "" && !Regex.IsMatch(op, "^(sr|tn|sine)$"))
            {
                Console.Write("Type another number, and then press Enter: ");
                numInput2 = Console.ReadLine();

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
            if (latestCalculations.Count < 2)
            {
                Console.WriteLine("Not enough previous calculations to perform this operation.");
                return;
            }
            GetCalculations();
            Console.WriteLine("\n Please write the index of the first calculation you wish to use:");
            int index1 = GetValidIndex();
            Console.WriteLine("\n Please write the index of the second calculation you wish to use:");
            int index2 = GetValidIndex();

            double num1 = latestCalculations[index1].Answer;
            double num2 = latestCalculations[index2].Answer;

            string op = AskOption(1);

            Calculate(op, num1.ToString(), num2.ToString());
        }

        public int GetValidIndex()
        {
            
            int index = -1;

            while(index == -1)
            {
                if (int.TryParse(Console.ReadLine(), out index))
                {
                    if (index < 0 || index >= latestCalculations.Count)
                    {
                        Console.WriteLine("Invalid index. Please try again.");
                        index = -1;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid index.");
                }
            }

            return index;

        }

    }


}
