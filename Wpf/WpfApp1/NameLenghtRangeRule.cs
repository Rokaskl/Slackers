using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1
{
   public class NameLenghtRangeRule : ValidationRule
    {
    private int _min;
    private int _max;

    public NameLenghtRangeRule()
    {
    }

    public int Min
    {
        get { return _min; }
        set { _min = value; }
    }

    public int Max
    {
        get { return _max; }
        set { _max = value; }
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        int lenght = 0;

        try
        {
            if (((string)value).Length > 0)
                lenght = ((string)value).Length;
        }
        catch (Exception e)
        {
            return new ValidationResult(false, "SMTG whent wrong" + e.Message);
        }

        if ((lenght < Min) || (lenght > Max))
        {
            return new ValidationResult(false,
              "Please enter field in the range: " + Min + " - " + Max + ".");
        }
        else
        {
            return ValidationResult.ValidResult;
        }
    }
    }
}
