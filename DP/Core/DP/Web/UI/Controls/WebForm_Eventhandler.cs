using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DP.Web.UI.Controls
{
    
    public delegate List<FieldDataOptions> DataSourceBindingEventhandler(object sender, WebFormField field);
    public delegate DataTable RelationEventhandler(object sender, string relationString);
    public delegate void WebFormFieldChangedEventHandler(object sender, WebFormField field, WebFormFieldType type, string name, string value);
    public delegate void DebugMessage(object sender, string message);

}
