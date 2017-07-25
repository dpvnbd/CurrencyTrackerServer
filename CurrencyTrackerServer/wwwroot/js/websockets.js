var uri = "ws://" + window.location.host + "/notifications";
var table = document.getElementById("currenciesTable");
var firstTime = true;
var connected = false;
var socket;

function connect() {
    socket = new WebSocket(uri);
    socket.onopen = function(event) {
        connected = true;
        appendItems(table, { info: true, text: "Соединено с сервером" });
        $("#connectionButton").text("Отключиться").attr("class", "btn btn-success");


    };
    socket.onclose = function(event) {
        connected = false;
        appendItems(table, { info: true, text: "Разорвано соединение с сервером" });
        $("#connectionButton").text("Подключиться").attr("class", "btn btn-danger");
    };
    socket.onmessage = function(event) {
        appendItems(table, event.data);
    };
    socket.onerror = function(event) {
        console.log("error: " + event.data);
        appendItems(table, { info: true, text: "Ошибка: " + event.data });

    };
}

connect();

function connectClick() {
    if (connected) {
        socket.close();
    } else {
        connect();
    }
}

function appendItems(table, data) {
    var arr;
    if (typeof data == "string") {
        arr = JSON.parse(data);
    } else {
        arr = data;
    }
    if (arr.info != undefined) {

        var time = timeFromDate(new Date());
        $("#currenciesTable").find('tbody')
            .append($('<tr>')
                .append($('<td>')
                    .attr('colspan', '2')
                    .text(arr.text)
                )
                .append($('<td>')
                    .text(time)
                )
            );

    } else {
        var speech = "";
        $("tr").removeClass();

        for (var i = 0; i < arr.length; i++) {
            speech += arr[i].Currency + ", ";
            var currency = arr[i];
            //var message = currency.Currency + " " + currency.ChangePercentage.toFixed(0) + "%";
            var date = new Date(currency.LastNotifiedChange);
            var time = timeFromDate(date);

            $("#currenciesTable").find('tbody')
                .append($('<tr>')
                    .attr("class", firstTime ? "" : "success")
                    .append($('<td>')
                        .html("<a href='https://bittrex.com/Market/Index?MarketName=BTC-" + currency.Currency + "'>" +
                            currency.Currency +
                            "</a>"))
                    .append(($('<td>')
                        .text(currency.ChangePercentage.toFixed(0) + " %")))
                    .append(($('<td>')
                        .text(time)))
                );
        }

        if (!firstTime) {
            if (document.getElementById('speechCheckbox').checked) {
                responsiveVoice.speak(speech);
            }
        } else {
            firstTime = false;
        }
    }
    var n = $(document).height();
    $('html, body').animate({ scrollTop: n }, 50);
}