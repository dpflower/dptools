//文本框只能输入数字代码(小数点也不能输入)
//onKeyUp
function isPositiveInteger(obj)
{
	obj.value=obj.value.replace(/[^\d]/g,'');
}

//
function isClipboardPositiveInteger()
{
	clipboardData.setData('text',clipboardData.getData('text').replace(/[^\d]/g,''));
}


//文本框只能输入正负号，数字(小数点也不能输入)
//onKeyUp
function isInteger(obj)
{
	if(!obj.value.match(/^[\+\-]?\d*?\d*?$/g))
	{
	    obj.value = (obj.t_value == null) ? "" : obj.t_value;
	}
	else
	{
	    obj.t_value = obj.value;
	}
}

//文本框只能输入正负号，数字和小数
//onKeyUp
function isDecimal(obj)
{
	if(!obj.value.match(/^[\+\-]?\d*?\.?\d*?$/g))
	{
	    obj.value = (obj.t_value == null) ? "" : obj.t_value;
	}
	else 
	{
	    obj.t_value=obj.value;
	}
}

//只能输入英文字母和数字,不能输入中文
//onKeyUp
function isEnglist(obj)
{
    obj.value=obj.value.replace(/[^a-zA-Z]/ig,'');
}

//只能输入英文字母和数字,不能输入中文
//onKeyUp
function isLetters(obj)
{
    obj.value=obj.value.replace(/[^\w]/ig,'');
}

function isChinese(obj)
{
    obj.value=obj.value.replace(/[^\u4E00-\u9FA5]/g,'');
}

function isFullWidthCharacters(obj)
{
    obj.value=obj.value.replace(/[^\uFF00-\uFFFF]/g,'');
}


//
function isMatch(obj, strformat)
{
    alert(strformat);
    var r = new RegExp(strformat);
    if(!r.test(obj.value))
    {        
	    obj.value = (obj.t_value == null) ? "" : obj.t_value;
    }
    else
    {    
	    obj.t_value=obj.value;
    }

}

//
function isMatchClipboard(strformat)
{
    var r = new RegExp(strformat);
    alert(r);
    if(!r.test(clipboardData.getData('text')))
    {        
	    clipboardData.setData('text', '');
    }
}

//只能输入汉字
//onkeypress
function onlychinese()
{
    if ((window.event.keyCode >=32) && (window.event.keyCode <= 126))
    {
        window.event.keyCode = 0;
    }
}


