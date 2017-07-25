function Reset(parameters) {
    $.post("Home/Reset");
    $('#currenciesTable > tbody > tr').remove();

};

function timeFromDate(date) {
    var h = date.getHours();
    var m = date.getMinutes();
    var s = date.getSeconds();
    h = (h < 10 ? "0" : '') + h;
    m = (m < 10 ? "0" : "") + m;
    s = (s < 10 ? "0" : "") + s;
    return h + ':' + m + ':' + s;
}

function connectClick() {
    
}