using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorLibrary.Model;
internal class Calculations
{
    internal int Id { get; set; }
    internal string Operation { get; set; }
    internal double Answer { get; set; }

    internal Calculations(int id,string operation, double answer)
    {
        Id = id;
        Operation = operation;
        Answer = answer;
    }
}

