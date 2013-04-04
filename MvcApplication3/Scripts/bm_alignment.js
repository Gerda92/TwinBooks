var id1 = null, id2 = null;
var mark_id1 = null, mark_id2 = null;
var selected = null;
var href = window.location.pathname;
var twin = (/\/([0-9]+)/g).exec(href)[1];
var bindings = null;

var highlight = "rgb(160, 222, 238)";
var hover_color = "rgb(209, 237, 245)";

$(document).ready(function () {
    
    bindings = jQuery.parseJSON($(".alignments").text());
    draw_table(bindings);
    rebind();

});

function addBookmarkBinding(id1, id2) {
    //alert(id1 + " " + id2);
    $.ajax({
        url: "http://localhost:1600/Alignment/CreateBookmark/" + twin + "/" + id1 + "/" + id2,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        success: function (data) {
            //alert("Ura! " + data.length);
            bindings = data;
            draw_table(bindings);
            rebind();
            var mark = $.grep(bindings, function (el, i) { return el.BookmarkId1 == id1 })[0];

            $("#mark-" + mark.Id + " td").css("background-color", "rgb(240, 105, 115)");
            $("#mark-" + mark.Id + " td").animate({ backgroundColor: '#FFFFFF' }, 1200);
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
        $("table").append(stringTableRaw(marks[i], left, right));
    }
    var left = "";
    for (; lc < l.length; lc++) {
        left += '<span class="sentence" id="sent-' + l[lc].id + '">' + $(l[lc]).html() + '</span>';
    }
    var right = "";
    for (; rc < r.length; rc++) {
        right += '<span class="sentence" id="sent-' + r[rc].id + '">' + $(r[rc]).html() + '</span>';
    }
    $("table").append(stringTableRaw(marks[marks.length - 1], left, right));
}

function rebind() {
    $(".sentence").click(function (e) {
        e.preventDefault();
        if (id1 == null) {
            id1 = (/sent-(.+)/g).exec(this.id)[1];
            mark_id1 = (/mark-(.+)/g).exec($(this).parents(".mark").attr('id'))[1];
            selected = this;
            $(this).attr('style', 'background:' + highlight + ';');
        } else {
            id2 = (/sent-(.+)/g).exec(this.id)[1];
            mark_id2 = (/mark-(.+)/g).exec($(this).parents(".mark").attr('id'))[1];
            $(this).attr('style', 'background:' + highlight + ';');
            if ($(selected).parents('.right-twin').attr('class') != undefined) {
                var c = id2; id2 = id1; id1 = c;
                c = mark_id2; mark_id2 = mark_id1; mark_id1 = c;
            }
            //alert(mark_id1 + " " + mark_id2);
            
            //previewRealign(id1, id2, mark_id1, mark_id2);

            addBookmarkBinding(id1, id2);
            id1 = null
            selected = null;
        }
    });

    $('.sentence').hover(function () {
        $(this).attr('style', 'background: ' + hover_color + ';');

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

function stringTableRaw(mark, left, right) {
    return '<tr id="mark-' + mark.Id + '" class="mark">' +
        '<td class="left-twin">' + left + "</td>" +
        '<td class="right-twin">' + right + "</td>" +
        '<td class="mark-info ' + (mark.Type == 1 ? "user" : "aligner") + '-made">' +
        (mark.Type == 1 ? "User" : "Aligner") + "</td>" +
    '</tr>';
}

function previewRealign(id1, id2, mark_id1, mark_id2) {
    var ll = 0;
    while (bindings[ll].Id != mark_id1) ll++;
    var rr = 0;
    while (bindings[rr].Id != mark_id2) rr++;

    var j = (ll < rr ? ll : rr) + 1;
    var min_j = j - 2;
    var max_j = (ll > rr ? ll : rr) + 1;

    var mark = { Id: 'new', Type: 1 };
    $("#mark-" + bindings[min_j].Id)
        .after(stringTableRaw(mark, $("#sent-" + id1).html(), $("#sent-" + id2).html()));

    var mark2 = { Id: 'after', Type: 0 };
    $("#mark-" + bindings[max_j].Id)
        .after(stringTableRaw(mark2, '', ''));

    $("#mark-" + bindings[max_j - 1].Id).remove();
    $("#mark-" + bindings[max_j].Id).remove();

    while (j < max_j - 1) {
        if (ll < rr) {
            $("#mark-" + bindings[min_j].Id + " .right-twin")
                .append($("#mark-" + bindings[j].Id + " .right-twin").html());
            $("#mark-after .left-twin")
                .append($("#mark-" + bindings[j].Id + " .left-twin").html());
        }
        if (ll > rr) {
            $("#mark-" + bindings[min_j].Id + " .left-twin")
                .append($("#mark-" + bindings[j].Id + " .left-twin").html());
            $("#mark-after .right-twin")
                .append($("#mark-" + bindings[j].Id + " .right-twin").html());
        }
        $("#mark-" + bindings[j].Id).remove();
        j++;
    }


    var toUp = $("#mark-" + bindings[min_j + 1].Id + " #sent-" + bindings[min_j + 1].BookmarkId1)
        .nextUntil("#sent-" + id1).andSelf();

    $("#mark-" + bindings[min_j].Id + " .right-twin")
                .append($(toUp).html());


    $("#mark-" + bindings[min_j + 1].Id).remove();

}