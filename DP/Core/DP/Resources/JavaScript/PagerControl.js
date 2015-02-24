function JumpPage(obj,object)
{    
    var text = document.getElementById(obj).value;  
    if(text == null || text == '')
    {
        alert('请输入要跳转的页数！');
        return false;
    }
    var url = object.href;
    var strUrl = String.format(url, text);
    window.location.href = strUrl;   
    
    return false;
}








