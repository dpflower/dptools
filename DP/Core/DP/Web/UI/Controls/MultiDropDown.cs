using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.ComponentModel;

namespace DP.Web.UI.Controls
{

    /// <summary>
    /// The MultiDropDown Server Control class.
    /// </summary>
    //[ToolboxBitmap(typeof(MultiDropDown), "ToolBoxBitmap.bmp")]
    public class MultiDropDown : WebControl, INamingContainer {

        //These component controls maintain state automatically
        HtmlTable tblMain = new HtmlTable();
        HtmlTableRow row1 = new HtmlTableRow();
        HtmlTableCell cell11 = new HtmlTableCell();
        HtmlTableCell cell12 = new HtmlTableCell();
        HtmlTableRow row2 = new HtmlTableRow();
        HtmlTableCell cell21 = new HtmlTableCell();
        HtmlInputHidden hdnValueList = new HtmlInputHidden();
        HtmlInputHidden hdnSettings = new HtmlInputHidden();
        HtmlInputText txtItemList = new HtmlInputText();
        HtmlInputCheckBox chkSelectAll = new HtmlInputCheckBox();
        HtmlInputText txtSearch = new HtmlInputText();
        HtmlImage imgClearSearch = new HtmlImage();
        HtmlGenericControl divImageButton = new HtmlGenericControl("div");
        HtmlGenericControl divDropdown = new HtmlGenericControl("div");
        GridView grdDropdown= new GridView();
        UpdatePanel updatePanel = new UpdatePanel();
        
        // The default CSS classes
        private const string DefaultTooltipClass = "MddTooltip";
        private const string DefaultTextBoxClass = "MddTextBox";
        private const string DefaultDropdownClass = "MddDropDown";

        public MultiDropDown() {
            DropdownOnMouseOver = false;
            DropdownOnFocus = true;
            SelectAllAtStartup = false;
        }

        /// <summary>
        /// Returns the comma separated list of selected items.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the comma separated list of selected items")]
        public string SelectedItems {
            get {
                return txtItemList.Value;
            }
        }

        /// <summary>
        /// Returns the pipe separated list of selected values.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the pipe separated list of selected values")]
        public string SelectedValues {
            get {
                return hdnValueList.Value;
            }
        }

        /// <summary>
        /// Gets or sets the data source for the dropdown items.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the data source for the dropdown items.")]
        public object DataSource { get; set; }

        /// <summary>
        /// Gets or sets the field name for the text of the dropdown items.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the field name for the text of the dropdown items.")]
        public string DataTextField { get; set; }

        /// <summary>
        /// Gets or sets the field name for the value of the dropdown items.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the field name for the value of the dropdown items.")]
        public string DataValueField { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the textbox.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the CSS class for the textbox.")]
        public string TextBoxClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the image button.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the CSS class for the image button.")]
        public string ImageButtonClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the dropdown.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the CSS class for the dropdown.")]
        public string DropdownClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the tooltip.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets the CSS class for the tooltip.")]
        public string TooltipClass { get; set; }

        /// <summary>
        /// Gets or sets if all items are selected at startup.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets if all items are selected at startup.")]
        public bool SelectAllAtStartup { get; set; }

        /// <summary>
        /// Gets or sets if dropdown should be shown when the textbox receives focus.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets if dropdown should be shown when the textbox receives focus.")]
        public bool DropdownOnFocus { get; set; }

        /// <summary>
        /// Gets or sets if dropdown should be shown when mouse hovers over the textbox.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets if dropdown should be shown when mouse hovers over the textbox.")]
        public bool DropdownOnMouseOver { get; set; }


        protected override void OnInit(EventArgs e) {

            //Inject JavaScript
            InjectScripts();

            //Initialize controls
            InitializeUpdatePanel();
            InitializeGridView();
            InitializeDropdownDiv();
            InitializeTextBox();
            InitializeHiddenFields();
            InitializeImageButton();
            InitializeSearchBox();

            //Set up the table
            tblMain = new HtmlTable();
            tblMain.CellPadding = 0;
            tblMain.CellSpacing = 0;

            //cell1.Style.Add(HtmlTextWriterStyle.VerticalAlign, "center");
            cell11.Controls.Add(hdnValueList);
            cell11.Controls.Add(hdnSettings);
            cell11.Controls.Add(txtItemList);
            cell12.Controls.Add(divImageButton);

            cell21.ColSpan = 2;
            //cell2.Style.Add(HtmlTextWriterStyle.VerticalAlign, "center");
            cell21.Controls.Add(divDropdown);

            //divDropdown.Controls.Add(updatePanel);
            //updatePanel.ContentTemplateContainer.Controls.Add(chkSelectAll);
            //updatePanel.ContentTemplateContainer.Controls.Add(txtSearch);
            //updatePanel.ContentTemplateContainer.Controls.Add(imgClearSearch);
            //updatePanel.ContentTemplateContainer.Controls.Add(grdDropdown);

            divDropdown.Controls.Add(chkSelectAll);
            divDropdown.Controls.Add(txtSearch);
            divDropdown.Controls.Add(imgClearSearch);
            divDropdown.Controls.Add(grdDropdown);

            row1.Cells.Add(cell11);
            row1.Cells.Add(cell12);
            row2.Cells.Add(cell21);

            tblMain.Rows.Add(row1);
            tblMain.Rows.Add(row2);
            
            base.OnInit(e);
        }

        protected override void CreateChildControls() {
            Controls.Add(tblMain);

            if (!Page.IsPostBack) {
                grdDropdown.DataSource = DataSource;
                ((BoundField)grdDropdown.Columns[1]).DataField = DataTextField;
                ((BoundField)grdDropdown.Columns[2]).DataField = DataValueField;
                grdDropdown.DataBind();
            }
        }

        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
            string includeTemplate = "<link rel='stylesheet' text='text/css' href='{0}' />";
            string includeLocation = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.CSS.MultiDropDown.css");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MultiDropDownStyle", String.Format(includeTemplate, includeLocation));
            //HtmlLink linkCSS = new HtmlLink();
            //linkCSS.Href = Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.CSS.MultiDropDown.css");
            //linkCSS.Attributes.Add("rel", "stylesheet");
            //linkCSS.Attributes.Add("type", "text/css");
            //Page.Header.Controls.Add(linkCSS);
        }

        private void InitializeUpdatePanel()
        {
            updatePanel.ID = "updatePanel0";
        }

        private void InitializeHiddenFields() {
            hdnValueList.ID = "hdnValueList";

            hdnSettings.ID = "hdnSettings";
            hdnSettings.Value = string.Format("{0}|{1}"
                                    , !string.IsNullOrEmpty(TooltipClass) ? TooltipClass : DefaultTooltipClass
                                    , SelectAllAtStartup? "1" : "0"
                                    );
        }

        private void InitializeSearchBox() {
            chkSelectAll.ID = "chkSelectAll";
            chkSelectAll.Attributes.Add("onclick", string.Format("stopBubble(event);SelectAll('{0}');", this.ClientID));

            txtSearch.ID = "txtSearch";
            txtSearch.Style.Add(HtmlTextWriterStyle.Width, "70%");
            txtSearch.Attributes.Add("autocomplete", "off");
            txtSearch.Attributes.Add("onkeyup", string.Format("FilterItems('{0}');", this.ClientID));
            txtSearch.Attributes.Add("onfocus", string.Format("SearchFocus('{0}');", this.ClientID));
            txtSearch.Attributes.Add("onblur", string.Format("SearchBlur('{0}');", this.ClientID));
            txtSearch.Attributes.Add("onclick", string.Format("stopBubble(event);ShowDropdown('{0}', false);", this.ClientID));

            imgClearSearch.ID = "imgClearSearch";
            imgClearSearch.Attributes.Add("onclick", string.Format("stopBubble(event);ClearSearch('{0}');", this.ClientID));
            string urlImage = Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.Images.CloseSearch.gif");
            imgClearSearch.Src = urlImage;
        }

        private void InitializeTextBox() {
            txtItemList.ID = "txtItemList";
            txtItemList.Attributes.Add("readonly", "true");
            string showDropDown = DropdownOnMouseOver ? "true" : "false";
            txtItemList.Attributes.Add("class", !string.IsNullOrEmpty(TextBoxClass) ? TextBoxClass : DefaultTextBoxClass);
            txtItemList.Attributes.Add("onmouseover", string.Format("TextBoxMouseOver('{0}', {1});", this.ClientID, showDropDown));
            txtItemList.Attributes.Add("onclick", string.Format("stopBubble(event);ShowDropdown('{0}', false);", this.ClientID));
            txtItemList.Attributes.Add("onmouseout", "tooltip.hide();");
            if (DropdownOnFocus) {
                txtItemList.Attributes.Add("onfocus", string.Format("ShowDropdown('{0}', false);", this.ClientID));
            }
        }

        private void InitializeImageButton() {
            divImageButton.Attributes.Add("onclick", string.Format("stopBubble(event);ShowDropdown('{0}', true);", this.ClientID));
            if (!string.IsNullOrEmpty(ImageButtonClass)) {
                divImageButton.Attributes.Add("class", ImageButtonClass);
            } else {
                string urlImage = Page.ClientScript.GetWebResourceUrl(this.GetType(), "DP.Resources.Images.Button.gif");
                divImageButton.Style.Add("background-image", string.Format("url({0})", urlImage));
                divImageButton.Style.Add("background-repeat", "no-repeat");
                divImageButton.Style.Add("height", "18px");
                divImageButton.Style.Add("width", "21px");
            }
        }

        private void InitializeDropdownDiv() {
            divDropdown.ID = "divDropdown";
            string gridClass = !string.IsNullOrEmpty(DropdownClass) ? DropdownClass : DefaultDropdownClass;
            divDropdown.Style.Add(HtmlTextWriterStyle.Position, "absolute");
            divDropdown.Style.Add(HtmlTextWriterStyle.Display, "none");
            divDropdown.Attributes.Add("class", gridClass);
            divDropdown.Attributes.Add("onmouseover", string.Format("ShowDropdown('{0}', false);", this.ClientID));
            divDropdown.Attributes.Add("onmouseout", string.Format("HideDropdown('{0}');", this.ClientID));
        }

        private void InitializeGridView() {
            grdDropdown.AutoGenerateColumns = false;
            grdDropdown.GridLines = GridLines.None;
            grdDropdown.ShowHeader = false;
            grdDropdown.Width = new Unit(100, UnitType.Percentage);

            grdDropdown.Columns.Add(new TemplateField() {
                ItemTemplate = new MultiGridViewTemplate(ListItemType.Item, "Col1", this.ClientID)
            });
            grdDropdown.Columns[0].ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            grdDropdown.Columns[0].ItemStyle.Width = new Unit(1, UnitType.Pixel);

            grdDropdown.Columns.Add(new BoundField() { DataField = DataTextField });
            grdDropdown.Columns[1].ItemStyle.HorizontalAlign = HorizontalAlign.Left;

            grdDropdown.Columns.Add(new BoundField() { DataField = DataValueField });
            
            grdDropdown.RowCreated += new GridViewRowEventHandler(
                                            (object sender, GridViewRowEventArgs e) => {
                                                e.Row.Attributes.Add("onclick", string.Format("stopBubble(event);SelectRow('{0}', {1});", this.ClientID, e.Row.RowIndex));
                                                e.Row.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                                                e.Row.Style.Add(HtmlTextWriterStyle.Width, "100%");
                                                e.Row.Cells[1].Style.Add(HtmlTextWriterStyle.Width, "100%");
                                                e.Row.Cells[2].Style.Add(HtmlTextWriterStyle.Display, "none");
                                            }
                                        );
        }

        private void InjectScripts() {
            ClientScriptManager clientScript = Page.ClientScript;

            string jsResMain = "DP.Resources.JavaScript.MultiDropDown.js";
            clientScript.RegisterClientScriptResource(this.GetType(), jsResMain);

            string jsResInit = "DP.Resources.JavaScript.MddInitialize.js";
            clientScript.RegisterClientScriptResource(this.GetType(), jsResInit);

            string jsResTooltip = "DP.Resources.JavaScript.MddTooltip.js";
            clientScript.RegisterClientScriptResource(this.GetType(), jsResTooltip);

            string startupKey = string.Format("MDDStartupScript-{0}", this.ClientID);
            //if (!clientScript.IsStartupScriptRegistered(startupKey)) {
            //    string startupScript = string.Format("AddToArray('{0}');", this.ClientID);
            //    clientScript.RegisterStartupScript(this.GetType(), startupKey, startupScript, true);
            //}
        }

    }

}
