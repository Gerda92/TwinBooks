var id1 = null;
var selected = null;
var href = window.location.pathname;
var book1 = (/\/([0-9]+)\/([0-9]+)/g).exec(href)[1];
var book2 = (/\/([0-9]+)\/([0-9]+)/g).exec(href)[2];

$(document).ready(function () {

    $(".chapter").click(function (e) {
        e.preventDefault();
        if (id1 == null) {
            id1 = $(this).attr('href');
            id1 = id1.substring(1, id1.length);
        } else {
            id2 = $(this).attr('href');
            id2 = id2.substring(1, id2.length);
            addChapterBinding(id1, id2);
            id1 = null
        }
    });

    $(document).mousedown(function (e) {
        if (e.which == 3) {
            id1 = null;
            $('.chapter').attr('style', 'background: inherit;');
        }
    });

});

function addChapterBinding(id1, id2) {
    $.ajax({
        url: "../../Alignment/CreateChapter/" + id1 + "/" + id2,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (data) {
            alert(data);
        },
        error: function () {
            alert('Some kind of error');
        },
        complete: function () {

        }
    });
}