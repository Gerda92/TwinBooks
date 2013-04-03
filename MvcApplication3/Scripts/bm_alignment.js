var id1 = null, id2 = null;
var selected = null;
var href = window.location.pathname;
var twin = (/\/([0-9]+)/g).exec(href)[1];
var bindings = null;

$(document).ready(function () {

    bindings = jQuery.parseJSON($(".alignments").text());
    draw_table(bindings);
    rebind();

});

function addBookmarkBinding(id1, id2) {
    //alert(id1 + " " + id2);
    $.ajax({
        url: "../../Alignment/CreateBookmark/" + twin + "/" + id1 + "/" + id2,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (data) {
            //alert("Ura! " + data.length);
            bindings = data;
            draw_table(bindings);
            rebind();
        },
        error: function () {
            alert('Some kind of error');
        },
        complete: function () {
            $('.sentence').attr('style', 'background: inherit;');
        }
    });
}

function contradicts(mark1, mark2) {
    return (mark1.Order1 - mark2.Order1) * (mark1.Order2 - mark2.Order2) <= 0;
}

function draw_table(marks) {
    $("table").html("");
    var lc = 0;
    
    var l = $("#rawtext .left-twin .sentence");
    while (l[lc].id != marks[0].BookmarkId1) lc++;

    var rc = 0;
    var r = $("#rawtext .right-twin .sentence");
    while (r[rc].id != marks[0].BookmarkId2) rc++;

    for (var i = 0; i < marks.length - 1; i++) {
        var left = "";
        for (; l[lc].id != marks[i + 1].BookmarkId1; lc++) {
            left += '<span class="sentence" id="sent-' + l[lc].id + '">' + $(l[lc]).html() + '</span>';
        }
        var right = "";
        for (; r[rc].id != marks[i + 1].BookmarkId2; rc++) {
            right += '<span class="sentence" id="sent-' + r[rc].id + '">' + $(r[rc]).html() + '</span>';
        }
        $("table").append("<tr>" +
            '<td class="left-twin">' + left + "</td>" +
            '<td class="right-twin">' + right + "</td>" +
        "</tr>");
    }
    var left = "";
    for (; lc < l.length; lc++) {
        left += '<span class="sentence" id="sent-' + l[lc].id + '">' + $(l[lc]).html() + '</span>';
    }
    var right = "";
    for (; rc < r.length; rc++) {
        right += '<span class="sentence" id="sent-' + r[rc].id + '">' + $(r[rc]).html() + '</span>';
    }
    $("table").append("<tr>" +
        '<td class="left-twin">' + left + "</td>" +
        '<td class="right-twin">' + right + "</td>" +
    "</tr>");
}

function rebind() {
    $(".sentence").click(function (e) {
        e.preventDefault();
        if (id1 == null) {
            id1 = (/sent-(.+)/g).exec(this.id)[1];
            selected = this;
            $(this).attr('style', 'background: #ffb7b7;');
        } else {
            id2 = (/sent-(.+)/g).exec(this.id)[1];
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
        /*
        var id = (/sent-(.+)/g).exec(this.id)[1];
        var sentences = $.grep(bindings, function (el, i) { return el.BookmarkId1 == id });
        if (sentences) {
            for (var i = 0; i < sentences.length; i++) {
                //alert(sentences[i].BookmarkId2)
                $('#' + sentences[i].BookmarkId2).attr('style', 'background: #ffb7b7;');
            }
        }
        */
    }, function () {
        if (id1 == null || (id1 != null && (/sent-(.+)/g).exec(this.id)[1] != id1))
            $(this).attr('style', 'background: inherit;');
    });

    $(document).mousedown(function (e) {
        if (e.which == 3) {
            id1 = null;
            $('.sentence').attr('style', 'background: inherit;');
        }
    });
}