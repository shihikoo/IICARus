$(document).ready(function () {

    $(function listFAQs() {
        var spreadsheetID = "18yEd1WfDmJk4Rb5G04LpY53Vpo0YZcXK5TUA9tStbbU";
        var url = "https://spreadsheets.google.com/feeds/list/" + spreadsheetID + "/od6/public/values?alt=json";
        $.getJSON(url,
            function (data) {
                $("div#faqList").append('<div id="faqItems" class="col-xs-11 col-xs-offset-1"> </div>');
                $.each(data.feed.entry, function (i, entry) {
                    var item = '<br/><br/><div class="row"> <div class="row"><h4 class="list-group-item-heading existingReviewTitle">' + entry.gsx$question.$t + '</h4></div>';

                    item += '<div class="col-xs-12 col-xs-offset-0"><p class="">' + entry.gsx$answer.$t + '</p></div></div>';

                    $("#faqItems").append(item);
                });
            });
    });
});

function SearchFAQ() {
    var spreadsheetID = "18yEd1WfDmJk4Rb5G04LpY53Vpo0YZcXK5TUA9tStbbU";
    var url = "https://spreadsheets.google.com/feeds/list/" + spreadsheetID + "/od6/public/values?alt=json";
    var search = document.getElementById("search").value.toLowerCase();
    clearBox("faqList");
    $.getJSON(url,
        function (data) {
            $("div#faqList").append('<ul id="faqItems"  class="col-xs-11 col-xs-offset-1" </ul>');
            $.each(data.feed.entry, function (i, entry) {
                if (entry.gsx$question.$t.toLowerCase().indexOf(search) > -1 ||
                   entry.gsx$answer.$t.toLowerCase().indexOf(search) > -1 
                    ) {
                    var item = '<br/><br/><div class="row"> <div class="row"><h4 class="list-group-item-heading existingReviewTitle">' + entry.gsx$question.$t + '</h4></div>';

                    item += '<div class="col-xs-12 col-xs-offset-0"><p class="">' + entry.gsx$answer.$t + '</p></div></div>';
                    $("#faqItems").append(item);
                }
            });
        });

}

function clearBox(elementID) {
    document.getElementById(elementID).innerHTML = "";
}

function handle(e) {
    if (e.keyCode === 13) {
        SearchFAQ();
    }

    return false;
}