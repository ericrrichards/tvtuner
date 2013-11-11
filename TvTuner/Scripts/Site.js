$(document).ready(function () {
    $("a.movieLink").hover(function (o) {
        //console.log(o.currentTarget);
        //console.log($(o.currentTarget).parent());
        //console.log($(o.currentTarget).parent().next());
        $(o.currentTarget).parent().next().removeClass("info");
        $(o.currentTarget).parent().next().addClass("infoShow");
        var y = $(o.currentTarget).offset().top + window.screenY;
        //console.log($(o.currentTarget));
        //console.log($($(o.currentTarget)[0]));
        var h = $($(o.currentTarget)[0]).children(":first-child").height() / 2;
        //console.log(h);
        $(o.currentTarget).parent().next().css("top", y + h + 2);


    },
    function (o) {
        $(o.currentTarget).parent().next().removeClass("infoShow");
        $(o.currentTarget).parent().next().addClass("info");
    });
});