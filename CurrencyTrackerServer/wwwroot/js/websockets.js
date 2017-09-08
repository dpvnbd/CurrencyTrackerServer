var uri = "ws://" + window.location.host + "/changeNotifications";
var table = document.getElementById("currenciesTable");
var firstTime = true;
var connected = false;
var socket;

function connect() {
    socket = new WebSocket(uri);
    socket.onopen = function(event) {
        connected = true;
        appendItems(table, { Type: 2, Message: "Соединено с сервером" });
        $("#connectionButton").text("Отключиться").attr("class", "btn btn-success");


    };
    socket.onclose = function(event) {
        connected = false;
        appendItems(table, { Type: 2, Message: "Разорвано соединение с сервером" });
        $("#connectionButton").text("Подключиться").attr("class", "btn btn-danger");
    };
    socket.onmessage = function(event) {
        appendItems(table, event.data);
    };
    socket.onerror = function(event) {
        console.log("error: " + event.data);
        appendItems(table, { Type: 1, Message: "Ошибка: " + event.data });

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
    if (typeof data === "string") {
        arr = JSON.parse(data);
    } else {
        arr = data;
    }

    arr = [].concat(arr);

    $("tr").removeClass();
    var speech = "";
    var isSpeaking = false;
    for (var i = 0; i < arr.length; i++) {
        var time;
        if (arr[i].Time !== undefined) {
            var date = new Date(arr[i].Time);
            time = timeFromDate(date);
        } else {
            time = timeFromDate(new Date);
        }


        if (arr[i].Type !== 0) {

            $("#currenciesTable").find('tbody')
                .append($('<tr>')
                    .append($('<td>')
                        .attr('colspan', '3')
                        .text(arr[i].Message)
                    )
                    .append($('<td>')
                        .attr('colspan', '1')
                        .text(time)
                    )
                );

        } else {
            isSpeaking = true;
            speech += arr[i].Currency + ", ";
            var currency = arr[i];
            //var message = currency.Currency + " " + currency.ChangePercentage.toFixed(0) + "%";
            var iconSrc = "";

            if (currency.ChangeSource === 1) {
                iconSrc = "/images/bittrexIcon.png";
            } else if (currency.ChangeSource === 2) {
                iconSrc = "/images/poloniexIcon.png";
            }

            $("#currenciesTable").find('tbody')
                .append($('<tr>')
                    .attr("class", firstTime ? "" : "success")
                    .append(($('<td>')
                        .html("<img class = 'source-icon' src = '" + iconSrc + "' />")))
                    .append($('<td>')
                        .html("<a href='https://bittrex.com/Market/Index?MarketName=BTC-" +
                            currency.Currency +
                            "'>" +
                            currency.Currency +
                            "</a>"))
                    .append(($('<td>')
                        .text(currency.Percentage.toFixed(0) + " %")))
                    .append(($('<td>')
                        .text(time)))
                );
        }
    }

    if (isSpeaking && !firstTime) {
        if (document.getElementById('speechCheckbox').checked) {
            responsiveVoice.speak(speech);
        }
    } else if (isSpeaking) {
        firstTime = false;
    }

    var n = $(document).height();
    $('html, body').animate({ scrollTop: n }, 50);
}