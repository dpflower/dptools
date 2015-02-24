//Full postback
if (window.addEventListener) {
    window.addEventListener('load', Initialize, false);
    window.addEventListener('click', HideAllDropdowns, false);
} else if (window.attachEvent) {
    window.attachEvent('onload', Initialize);
    window.attachEvent('onclick', HideAllDropdowns);
}

//Partial postback - the predefined method signature 'pageLoad(sender, args)'
//is called by the ASP.NET AJAX runtime after every AJAX request.
function pageLoad(sender, args) {
    Initialize();
}
