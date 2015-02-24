/* Array of all MultiDropDown controls in the page.
This array is used to hide all other dropdowns before opening the current control's dropdown */
var arrMDDs = new Array();
var searchPlaceHolder = 'Search...';

/* Adds the current control to the array
Parameters:
controlId - Name of the control to be added */
function AddToArray(controlId) {
    arrMDDs.push(controlId);
}

/* Shows the current control's dropdown
Parameters:
controlId - Name of the current control
toggle - true if dropdown is to be hidden if it is already visible, false otherwise. */
function ShowDropdown(controlId, toggle) {
    HideAllDropdowns(controlId);

    var divDropdownId = controlId + '_divDropdown';
    var divDropdown = document.getElementById(divDropdownId);
    if (divDropdown != null) {
        if (toggle) {
            divDropdown.style.display = (divDropdown.style.display == 'block') ? 'none' : 'block';
        } else {
            divDropdown.style.display = 'block';
        }
    }
}

/* Hides all other dropdowns except the current control's dropdown
Parameters:
controlId - Name of the current control */
function HideAllDropdowns(controlId) {
    for (var i = 0; i < arrMDDs.length; i++) {
        if (arrMDDs[i] != controlId) {
            HideDropdown(arrMDDs[i]);
        }
    }
}

/* Hides the current control's dropdown
Parameters:
controlId - Name of the current control */
function HideDropdown(controlId) {
    var divDropdownId = controlId + '_divDropdown';
    var divDropdown = document.getElementById(divDropdownId);
    divDropdown.style.display = 'none';
}

/* Select a row - called from the Table Row's (tr) onclick event
Parameters:
controlId - Name of the current control 
rowIndex - The row to select */
function SelectRow(controlId, rowIndex) {
    var tblGrid = document.getElementById(controlId + '_divDropdown').getElementsByTagName('table')[0];
    tblGrid.rows[rowIndex].cells[0].getElementsByTagName('input')[0].checked = !tblGrid.rows[rowIndex].cells[0].getElementsByTagName('input')[0].checked;
    SelectItem(controlId);
}

/* Selects items based on each item's check status - called from the Checkbox's onclick event
Parameters:
controlId - Name of the current control */
function SelectItem(controlId) {
    var tblGrid = document.getElementById(controlId + '_divDropdown').getElementsByTagName('table')[0];
    var chkSelectAll = document.getElementById(controlId + '_chkSelectAll');
    var itemList = '';
    var valueList = '';
    var countNoCheck = 0;
    for (var i = 0; i < tblGrid.rows.length; i++) {
        if (tblGrid.rows[i].cells[0].getElementsByTagName('input')[0].checked) {
            tblGrid.rows[i].className = 'selected';
            itemList += (tblGrid.rows[i].cells[1].innerHTML + ', ');
            valueList += (tblGrid.rows[i].cells[2].innerHTML + '|');
        } else {
            tblGrid.rows[i].className = 'unselected';
            countNoCheck++;
        }
    }
    chkSelectAll.checked = (countNoCheck == 0);

    itemList = chkSelectAll.checked ? "All" : itemList.substring(0, itemList.length - 2);
    valueList = valueList.substring(0, valueList.length - 1);

    var txtItemList = document.getElementById(controlId + '_txtItemList');
    txtItemList.value = itemList;

    var hdnValueList = document.getElementById(controlId + '_hdnValueList');
    hdnValueList.value = valueList;
}

/* Returns if any item is selected, used to determine if it is a postback
Parameters:
controlId - Name of the current control */
function IsItemSelected(controlId) {
    var tblGrid = document.getElementById(controlId + '_divDropdown').getElementsByTagName('table')[0];
    for (var i = 0; i < tblGrid.rows.length; i++) {
        if (tblGrid.rows[i].cells[0].getElementsByTagName('input')[0].checked) {
            return true;
        }
    }
    return false;
}

/* Selects all items
Parameters:
controlId - Name of the current control */
function SelectAll(controlId) {
    var tblGrid = document.getElementById(controlId + '_divDropdown').getElementsByTagName('table')[0];
    var chkSelectAll = document.getElementById(controlId + '_chkSelectAll');
    for (var i = 0; i < tblGrid.rows.length; i++) {
        tblGrid.rows[i].cells[0].getElementsByTagName('input')[0].checked = chkSelectAll.checked;
    }
    SelectItem(controlId);
}

function SearchFocus(controlId) {
    var txtSearch = document.getElementById(controlId + '_txtSearch');
    if (txtSearch.value == searchPlaceHolder) {
        txtSearch.value = '';
    }
    txtSearch.style.color = '#000000';
    ShowDropdown(controlId, false);
}

function SearchBlur(controlId) {
    var txtSearch = document.getElementById(controlId + '_txtSearch');
    if (txtSearch.value == '') {
        txtSearch.style.color = 'gray';
        txtSearch.value = searchPlaceHolder;
    } else if (txtSearch.value == searchPlaceHolder) {
        txtSearch.style.color = 'gray';
    }
}

function ClearSearch(controlId) {
    var txtSearch = document.getElementById(controlId + '_txtSearch');
    txtSearch.value = searchPlaceHolder;
    txtSearch.style.color = 'gray';
    FilterItems(controlId);
}

/* Filters items based on search text.
Parameters:
controlId - Name of the current control */
function FilterItems(controlId) {
    var txtSearch = document.getElementById(controlId + '_txtSearch');
    var tblGrid = document.getElementById(controlId + '_divDropdown').getElementsByTagName('table')[0];
    var searchText = txtSearch.value.toLowerCase();
    var searchLen = searchText.length;
    //Hide all items
    for (var i = 0; i < tblGrid.rows.length; i++) {
        tblGrid.rows[i].style.display = 'block';
    }
    if (txtSearch.value != searchPlaceHolder) {
        for (var i = 0; i < tblGrid.rows.length; i++) {
            if (tblGrid.rows[i].cells[1].innerHTML.substring(0, searchLen).toLowerCase() != searchText) {
                tblGrid.rows[i].style.display = 'none';
            }
        }
    }
}

/* This function is called from Textbox's onmouseover event and sets the tooltip
Parameters:
controlId - Name of the current control
showDropdown - true if DropdownOnMouseOver property is true, false otherwise.
*/
function TextBoxMouseOver(controlId, showDropdown) {
    var txtItemList = document.getElementById(controlId + '_txtItemList');
    if (txtItemList.value != '') {
        var tooltipClass = GetSetting(controlId, 'tooltipClass');
        tooltip.show(txtItemList.value, null, tooltipClass);
    }
    if (showDropdown) {
        ShowDropdown(controlId, false);
    } else {
        HideAllDropdowns(controlId);
    }
}

/* Initializes state of selected items of all controls after a full or partial postback */
function Initialize() {
    for (var i = 0; i < arrMDDs.length; i++) {
        var controlId = arrMDDs[i];
        var selectAllAtStartup = GetSetting(controlId, 'selectAllAtStartup');
        if (selectAllAtStartup == "1" && !IsItemSelected(controlId)) {
            var chkSelectAll = document.getElementById(controlId + '_chkSelectAll');
            chkSelectAll.checked = true;
            SelectAll(controlId);
        } else {
            SelectItem(controlId);
        }
        var txtSearch = document.getElementById(controlId + '_txtSearch');
        txtSearch.value = searchPlaceHolder;
        txtSearch.style.color = 'gray';
    }
}

function GetSetting(controlId, settingName) {
    var hdnSettings = document.getElementById(controlId + '_hdnSettings');
    var arrSettings = hdnSettings.value.split('|');
    var index = -1;
    switch (settingName) {
        case 'tooltipClass':
            index = 0;
            break;
        case 'selectAllAtStartup':
            index = 1;
            break;
    }
    return arrSettings[index];
}

/* Stops event bubbling to outer controls.
Called from checkbox's onclick to prevent firing tr's onclick. */
function stopBubble(e) {
    try {
        var ev = e || window.event;
        if (ev.stopPropagation) {
            ev.stopPropagation();
        } else if (ev.cancelBubble) {
            ev.cancelBubble = true;
        }
    } catch (e) { }
}
