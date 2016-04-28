var word;

$('#searchBox').keyup(function () {
    word = $(this).val();
    $('#results').empty();
    if (word.length != 0) {
        //word = replaceUnderscore(word);
        callWebMethod();
    }
});

function callWebMethod() {
    $.ajax({
        type: "POST",
        url: "WebService1.asmx/searchTrie",
        data: JSON.stringify({
            "search": word
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var data = eval(msg);
            for (var key in data.d) {
                $("#results").append(data.d[key] + "<br>");
            }
        },
        error: function (msg) {
            console.log("error");
            console.log(msg);
        }
    });
}

function replaceUnderscore(check) {
    for (var ch in check) {
        if (ch == ' ') {
            check($index) = '_';
        }
    }
    return check;
}