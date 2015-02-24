using System;
using System.Collections.Generic;
using System.Text;

namespace DP.Web.UI.Controls
{
    [Serializable]
    public class FieldDataOptions
    {
        public FieldDataOptions(string value, string text)
        {
            _text = text;
            _value = value;
        }
        public FieldDataOptions()
            : this("", "")
        {

        }

        string _value = string.Empty;
        string _text = string.Empty;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

    }
}
