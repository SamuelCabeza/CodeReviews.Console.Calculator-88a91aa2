using CalculatorLibrary.Interfaces;
using CalculatorLibrary.Model;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorLibrary.Controller;

internal class CalculationsController : BaseController, IBaseController
{
    public void ViewItems()
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);

        table.AddColumn("[yellow]ID[/]");
        table.AddColumn("[yellow]Operation[/]");
        table.AddColumn("[yellow]Answer[/]");

        // Filtering only items of the book type
        var books = MockDatabase.CalculationItems;

        foreach (var book in books)
        {
            table.AddRow(
                book.Id.ToString(),
                $"[cyan]{book.Operation}[/]",
                $"[cyan]{book.Answer}[/]"
                );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("Press Any Key to Continue.");
        Console.ReadKey();
    }

    public void AddItem()
    {
        throw new NotImplementedException();
    }

    public void AddItem(string operation, double answer) 
    {
        var newCalculatoin = new Calculations(MockDatabase.CalculationItems.Count + 1, operation, answer);

        MockDatabase.CalculationItems.Add(newCalculatoin);
    }

    public void DeleteItem()
    {
        var calculations = MockDatabase.CalculationItems.ToList();

        if (calculations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No calculation available to delete.[/]");
            Console.ReadKey();
            return;
        }

        var calculationToDelete = AnsiConsole.Prompt(
            new SelectionPrompt<Calculations>()
                .Title("Select a [red]operation[/] to delete:")
                .UseConverter(c => $"{c.Operation} by {c.Answer}")
                .AddChoices(calculations));

        if (ConfirmDeletion(calculationToDelete.Operation))
        {
            if (MockDatabase.CalculationItems.Remove(calculationToDelete))
            {
                DisplayMessage("Calculation deleted successfully!", "red");
            }
            else
            {
                DisplayMessage("Calculation not found.", "red");
            }
        }
        else
        {
            DisplayMessage("Deletion canceled.", "yellow");
        }

        DisplayMessage("Press Any Key to Continue.", "green");
        Console.ReadKey();
    }

    public Calculations SelectItem()
    {
        var calculations = MockDatabase.CalculationItems.ToList();

        var calculation = AnsiConsole.Prompt(
            new SelectionPrompt<Calculations>()
                .Title("Select a [green]operation[/]:")
                .UseConverter(c => $"{c.Operation} by {c.Answer}")
                .AddChoices(calculations));

        return calculation;
    }



}

