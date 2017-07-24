var uri = "ws://" + window.location.host + "/notifications";
var table = document.getElementById("currenciesTable");

function connect() {
    var socket = new WebSocket(uri);
    socket.onopen = function(event) {
        console.log("opened connection to " + uri);
    };
    socket.onclose = function(event) {
        console.log("closed connection from " + uri);
    };
    socket.onmessage = function(event) {
        appendItems(table, event.data);
    };
    socket.onerror = function(event) {
        console.log("error: " + event.data);
    };
}

connect();

function Reset(parameters) {
    $.post("Home/Reset");
};


function appendItems(table, currencies) {

    var arr = JSON.parse(currencies.replace(/&quot;/g, '"'));

    if (arr.info != undefined) {
        var today = new Date();
        var h = today.getHours();
        var m = today.getMinutes();
        var s = today.getSeconds();
        arr.text += " Клиент:" + h + ":" + m + ":" + s;
        $("#currenciesTable").find('tbody')
            .append($('<tr>')
                .append($('<td>')
                    .attr('colspan', '3')
                    .text(arr.text)
                )
            );

    } else {
        for (var i = 0; i < arr.length; i++) {
            var currency = arr[i];
            //var message = currency.Currency + " " + currency.ChangePercentage.toFixed(0) + "%";
            var date = new Date(currency.LastNotifiedChange);
            var h = date.getHours();
            var m = date.getMinutes();
            var s = date.getSeconds(); 

            h = h < 10 ? '0' : '' + h;
            m = m < 10 ? '0' : '' + m;
            s = s < 10 ? '0' : '' + s;

            $("#currenciesTable").find('tbody')
                .append($('<tr>')
                    .append($('<td>')
                        .text(currency.Currency))
                    .append(($('<td>')
                        .text(currency.ChangePercentage.toFixed(0) + " %")))
                    .append(($('<td>')
                        .text(h + ':' + m + ':' + s)))
                );
        }
    }
}