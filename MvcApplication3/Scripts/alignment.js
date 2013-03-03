var id1 = null;
var selected = null;
var href = window.location.pathname;
var book1 = (/\/([0-9]+)\/([0-9]+)/g).exec(href)[1];
var book2 = (/\/([0-9]+)\/([0-9]+)/g).exec(href)[2];
var bindings = null;

getBindings();

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

    $(".sentence").click(function (e) {
        e.preventDefault();
        if (id1 == null) {
            id1 = $(this).attr('id');
            selected = this;
            $(this).attr('style', 'background: #ffb7b7;');
        } else {
            id2 = $(this).attr('id');
            $(this).attr('style', 'background: #ffb7b7;');
            if ($(selected).parents('.right-twin').attr('class') != undefined) {
                var c = id2; id2 = id1; id1 = c;
            }
            addBookmarkBinding(id1, id2);
            id1 = null
            selected = null;
        }
    });

    $('.sentence').hover(function () {
        $(this).attr('style', 'background: #ffb7b7;');
        var id = $(this).attr('id');
        var sentences = $.grep(bindings, function (el, i) { return el.BookmarkId1 == id });
        if (sentences) {
            for (var i = 0; i < sentences.length; i++) {
                //alert(sentences[i].BookmarkId2)
                $('#' + sentences[i].BookmarkId2).attr('style', 'background: #ffb7b7;');
            }
        }
    }, function () {
        if (this != selected) $('.sentence').attr('style', 'background: inherit;');
    });

    $(document).mousedown(function (e) {
        if (e.which == 3) {
            id1 = null;
            $('.chapter').attr('style', 'background: inherit;');
            $('.sentence').attr('style', 'background: inherit;');
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

function addBookmarkBinding(id1, id2) {
    $.ajax({
        url: "../../Alignment/CreateBookmark/" + book1 + "/" + book2 + "/" + id1 + "/" + id2,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (data) {
            alert("Ura! " + data.BookmarkId1 + " + " + data.BookmarkId2);
            getBindings();
        },
        error: function () {
            alert('Some kind of error');
        },
        complete: function () {
            $('.sentence').attr('style', 'background: inherit;');
        }
    });
}

function getBindings() {
    $.ajax({
        url: "../../GetBookmarks/" + book1 + "/" + book2,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (data) {
            //alert("Ura! " + data.size);
            bindings = data;
        },
        error: function () {
            alert('Some kind of error');
        },
        complete: function () {
        }
    });
}