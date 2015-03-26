var tvtuner = tvtuner || {};
tvtuner.spinner = new Spinner();
tvtuner.getBaseURL = function () {
    //http://www.gotknowhow.com/articles/how-to-get-the-base-url-with-javascript
    var url = location.href;  // entire url including querystring - also: window.location.href;
    var baseURL = url.substring(0, url.indexOf('/', 14));


    if (baseURL.indexOf('http://localhost') != -1) {
        // Base Url for localhost
        url = location.href; // window.location.href;
        var pathname = location.pathname;  // window.location.pathname;
        var index1 = url.indexOf(pathname);
        var index2 = url.indexOf("/", index1 + 1);
        var baseLocalUrl = url.substr(0, index2);

        return baseLocalUrl + "/";
    }
    else {
        // Root Url for domain name
        return baseURL + "/";
    }

}

tvtuner.getUrlVars = function(){
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for(var i = 0; i < hashes.length; i++)
    {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

tvtuner.GetShowInformationFromRepository = function (showName, failure) {
    $.get(tvtuner.getBaseURL() + "ShowInformation/" + showName,
        function (data2) {
            if (typeof data2 === "string") {
                failure();
                return;
            } else {
                tvtuner.spinner.stop();
                $("#modalShowName").val(data2.Name);
                $("#modalShowSummary").text(data2.Summary);
                $("#modalShowBanner").attr("src", tvtuner.getBaseURL() + data2.BannerPath);
                $("#btnSaveShow").show();
            }
        }
    );
}

tvtuner.GetShowInformation = function (id) {
    $.get(tvtuner.getBaseURL() + "tvdb/ShowInformation/" + id,
        function (data2) {
            tvtuner.spinner.stop();
            $("#modalShowName").val(data2.Name);
            $("#modalShowSummary").text(data2.Summary);
            $("#modalShowBanner").attr("src", tvtuner.getBaseURL() + data2.BannerPath);
            $("#btnSaveShow").show();
        }
    );
}
tvtuner.LookupShow = function () {
    var search = $("#modalShowName").val();
    tvtuner.spinner.spin(document.getElementsByClassName('modal-body')[0]);
    var lb = $("#modalPickShow");
    lb.html('');
    $.get(tvtuner.getBaseURL() + "tvdb/lookupshow/",
        {
            search: search
        }, function (data) {
            if (typeof data === "string") {
                tvtuner.spinner.stop();
                alert(data);
                return;
            }

            if (data.length == 1) {
                tvtuner.GetShowInformation(data[0].Item2);
            } else if (data.length > 1) {
                lb.show();
                $(data).each(function (i, elem) {
                    lb.append("<option value='" + elem.Item2 + "'>" + elem.Item1 + "</option>");
                });
                tvtuner.spinner.stop();
            }
        }
    );
}
tvtuner.GetEpisodeInformationFromRepository = function(id, failure){
    $.get(tvtuner.getBaseURL() + "EpisodeInformation/" + id,
        function (data2) {
            if (typeof data2 === "string") {
                failure();
                return;
            } else {
                console.log(data2);
                $("#modalEpisodeSeason").val(data2.SeasonNumber);
                $("#modalEpisodeNum").val(data2.EpisodeNumber);
                $("#modalEpisodeTitle").val(data2.Title);
                $("#modalEpisodeSummary").val(data2.Summary);
                $("#modalEpisodeThumb").attr("src", data2.ThumbsFilePath);
            }
        }
    );
}

tvtuner.ClearModal = function () {
    $("#modalShowName").val('');
    $("#modalShowBanner").attr('src', '');
    $("#modalShowSummary").text('');
    $("#btnSaveShow").hide();
    $("#modalEpisodeSeason").val('');
    $("#modalEpisodeNum").val('');
    $("#modalPickShow").html('').hide();
    $("#modalEpisodeSummary").val('');
    $("#modalEpisodeTitle").val('');
    $("#modalEpisodeThumb").attr("src", '');
}

tvtuner.LookupEpisode = function (showname, seasonNum, episodeNum) {
    if ("#episodeList:visible") {
        $("#episodeList").slideUp();
    }
    $("#modalEpisodeSeason").val(seasonNum);
    $("#modalEpisodeNum").val(episodeNum);

    $.get(
        tvtuner.getBaseURL() + "tvdb/LookupEpisode",
        {
            showname:showname,
            seasonNum:seasonNum,
            episodeNum:episodeNum
        },
        function (data) {
            console.log(typeof data);
            if (typeof data === "string") {
                alert(data);
                return;
            } else if (data.length > 1) {
                var tbody = $("#episodeList tbody");

                $(data).each(function(i, elem) {
                    console.log(elem);
                    tbody.append(
                        "<tr>" +
                        "<td>" + elem.title + "</td>" +
                        "<td>" + elem.season + "</td>" +
                        "<td>" + elem.episodeNumber + "</td>" +
                        "<td><a href='#' onclick='tvtuner.LookupEpisode(\"" + showname + "\", " + elem.season + "," + elem.episodeNumber + ");'><span class='glyphicon glyphicon-check'></span></a></td>" +
                        "</tr>");
                });
                $("#episodeList").slideDown();
            } else {
                $("#modalEpisodeTitle").val(data.title);
                $("#modalEpisodeSummary").val(data.summary);
                $("#modalEpisodeThumb").attr("src", tvtuner.getBaseURL() + data.imgDlPath);
            }
        }
    );
}