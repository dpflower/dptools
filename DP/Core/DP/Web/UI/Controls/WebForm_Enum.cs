using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Web.UI.Controls
{
    public enum WebFormFieldType
    {
        TextBox = 0,
        DropDownList = 1,
        RadioButton = 2,
        CheckBox = 3,
        DataTimePicker = 5,
        RichTextBox = 10,
        DropDownListAndButton = 11,
        TextBoxAndButton = 12
    }

    public enum DataSourceType
    {
        String = 0,
        Enum = 1,
        Custom = 2
    }

    public enum FieldDataType
    {
        PositiveNumber = 0,
        Decimal = 1,
        String = 2
    }
}
