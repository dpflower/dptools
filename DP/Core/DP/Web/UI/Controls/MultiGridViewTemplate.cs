using System;
using System.Collections.Generic;

using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace DP.Web.UI.Controls
{
    public class MultiGridViewTemplate : ITemplate
    {
        //A variable to hold the type of ListItemType.
        ListItemType _templateType;

        //A variable to hold the column name.
        string _columnName;

        //A variable to hold the control Id.
        string _controlId;

        //Constructor where we define the template type and column name.
        public MultiGridViewTemplate(ListItemType type, string colname, string controlId)
        {
            _templateType = type;
            _columnName = colname;
            _controlId = controlId;
        }

        void ITemplate.InstantiateIn(System.Web.UI.Control container)
        {
            switch (_templateType)
            {
                case ListItemType.Header:
                    //HtmlInputCheckBox chkSelectAll = new HtmlInputCheckBox();
                    //chkSelectAll.Attributes.Add("onclick", string.Format("stopBubble(event);SelectAll('{0}');", _controlId));
                    //container.Controls.Add(chkSelectAll);
                    break;
                case ListItemType.Item:
                    HtmlInputCheckBox chkItem = new HtmlInputCheckBox();
                    chkItem.Attributes.Add("onclick", string.Format("stopBubble(event);SelectItem('{0}');", _controlId));
                    container.Controls.Add(chkItem);
                    break;
                case ListItemType.EditItem:
                    break;
                case ListItemType.Footer:
                    break;
            }
        }
    }

}
