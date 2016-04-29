var word;

$('#searchBox').keyup(function () {
    word = $(this).val();
    if (word.length != 0) {
        callWebMethod();
    } else {
        $('#results').empty();
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
            $('#results').empty();
            var data = eval(msg);
            for (var key in data.d) {
                var result = data.d[key];
                $("#results").append("<p class='suggestion' id='" + result + "'>" + result + "</p>");
            }
        },
        error: function (msg) {
            console.log("error");
            console.log(msg);
        }
    });
}

$("#results").on("click", ".suggestion", function () {
    word = $(this).attr("id");
    $("#searchBox").val(word);
    callWebMethod();
});