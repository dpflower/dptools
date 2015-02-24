var _activeComboBoxTextBox = null;
var _enableAutoComplete = true;

function SetActiveComboBoxTextBox(txtBx){_activeComboBoxTextBox=txtBx;}

function findPos(obj) 
{
    var curleft = curtop = 0;
    if (obj.offsetParent)
    {
        curleft = obj.offsetLeft
        curtop = obj.offsetTop
        while (obj = obj.offsetParent)
        {
	        curleft += obj.offsetLeft
	        curtop += obj.offsetTop
        }
    }
    return [curleft,curtop];
}

function ComboBox_TextBoxBlur(txtBx,lstBxWrapper)
{
    if(lstBxWrapper.style.visibility == "visible")
    {
        lstBxWrapper.all[0].focus();
    }
}

function ComboBox_ListBoxHide(lstBxWrapper)
{
    lstBxWrapper.style.visibility = "hidden";
    lstBxWrapper.style.width = "0px";    
}

function ComboBox_ListBoxShow(txtBxWrapper, lstBxWrapper)
{
    if(lstBxWrapper.style.visibility == "visible")
    {
        ComboBox_ListBoxHide(lstBxWrapper);
        return;
    }
    var pos = findPos(txtBxWrapper);
    var top = pos[1];
    var left = pos[0];
    
    lstBxWrapper.style.left = left+1;
    lstBxWrapper.style.top = top+txtBxWrapper.offsetHeight;
    lstBxWrapper.style.visibility = "visible";
    lstBxWrapper.all[0].focus();
    lstBxWrapper.all[0].style.width = txtBxWrapper.offsetWidth;
}

function ComboBox_ListBoxDisplaySelectedText(lstBx,textBx)
{
    textChanged = false;
    if(lstBx.selectedIndex >= 0)
    {
        oldVal = textBx.value;
        textBx.value = lstBx.options[lstBx.selectedIndex].text;
        if(oldVal != textBx.value)
            textChanged = true;
    }
    if(textChanged)//Fire onchange event of the TextBox, if AutoPostBack is enabled this will tell the form to PostBack
        textBx.fireEvent("onchange");
    textBx.focus();   
    ComboBox_ListBoxHide(lstBx.offsetParent);
    
}

function ComboBox_ListBoxDisplaySelectedTextNoHide(lstBx,textBx)
{
    textChanged = false;
    if(lstBx.selectedIndex >= 0)
    {
        oldVal = textBx.value;
        textBx.value = lstBx.options[lstBx.selectedIndex].text;
        if(oldVal != textBx.value)
            textChanged = true;
    }
    if(textBx.fireEvent)
    {
        if(textChanged)//Fire onchange event of the TextBox, if AutoPostBack is enabled this will tell the form to PostBack
            textBx.fireEvent("onchange");
    }
}

function ComboBox_ListBoxKeyup(lstBx,txtBx,e)
{
    switch(e.keyCode)
    {
        case 13:
            if(lstBx.fireEvent)
                lstBx.fireEvent("onclick");
            break;
        case 27:
            ComboBox_ListBoxHide(lstBx.offsetParent);
            break;
        case 38:
        case 40:
            ComboBox_ListBoxDisplaySelectedTextNoHide(lstBx, txtBx);
            break;
        default:
            break;
    }
}
function ComboBox_TextBoxKeyup(txtBx, lstBx, e)
{
    if(e.keyCode == 38 && lstBx.selectedIndex > 0)
    {
        lstBx.selectedIndex--;
        ComboBox_ListBoxDisplaySelectedTextNoHide(lstBx, txtBx);        
        return;
    }
    else if (e.keyCode == 40 && lstBx.selectedIndex < lstBx.options.length-1)
    {
        lstBx.selectedIndex++;
        ComboBox_ListBoxDisplaySelectedTextNoHide(lstBx, txtBx);
        return;
    }
    else if (e.keyCode == 27)
    {
        ComboBox_ListBoxHide(lstBx.offsetParent);
        return false;
    }
    else if (e.keyCode == 9) return false;
    
    if(_enableAutoComplete)
    {
        var part = txtBx.value.toLowerCase();
        var exp = new RegExp(part);
        for (i=0; i<lstBx.options.length; i++)
        {
            var text = lstBx.options[i].text.toLowerCase();
            if(text == "")
                break;
                
            var match = exp.exec(text);
            if(match != null && match.index == 0)
            {
                lstBx.selectedIndex = i;            
                return;
            }
        }
        lstBx.selectedIndex  = -1;
    }
}