using System;
using System.Collections.Generic;
using System.Text;
using static CalculatorLibrary.Enums.Enums;

namespace CalculatorLibraryl;

internal static class Menus
{
    internal static readonly MenuOption[] Basic =
    {
        MenuOption.Addition, MenuOption.Substraction, MenuOption.Multiply,
        MenuOption.Divide, MenuOption.SquareRoot, MenuOption.Power,
        MenuOption.MultiplyX10, MenuOption.Sin
    };

    internal static readonly MenuOption[] Complete =
        Enum.GetValues<MenuOption>(); // all of them
}

