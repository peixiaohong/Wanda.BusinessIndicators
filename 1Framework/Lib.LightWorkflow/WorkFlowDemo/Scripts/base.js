
//document.write('<script type="text/javascript" src="../scripts/jquery.cookie.js"></script>  ');

function loadHeader(options) {

    if (typeof (options) == "string") {
        options = { currentTabName: options };
    }

    var setting = { path: "../_pagelet/header.html", placeHolder: "headerContent", currentTabName: null };

    $.extend(setting, options);

    $.get(setting.path, function (data) {
        $("#" + setting.placeHolder).html(data);
       
    })
    // $("#headerContent").html(
}

function loadLeft(options) {
    var setting = { path: "../_pagelet/navigator.html", placeHolder: "leftContent" };
    $.extend(setting, options);
    $.get(setting.path, function (data) {
        $("#" + setting.placeHolder).html(data);

        $("#" + setting.placeHolder).find("dd").removeClass("font_color");

        var name = pageName();
        $("#" + setting.placeHolder).find("dd").has("a[href*='" + name + "'").addClass("font_color");
    })

    //取当前页面名称(带后缀名)
    function pageName() {
        var strUrl = location.href;
        var arrUrl = strUrl.split("/");
        var strPage = arrUrl[arrUrl.length - 1];
        if (strPage.indexOf("#") > 0) {
            strPage = strPage.substr(0, strPage.indexOf("#"));
        }

        return strPage;

    }
}

function loadFooter(options) {
    var setting = { path: "../_pagelet/footer.html", placeHolder: "footerContent" };
    $.extend(setting, options);
    $.get(setting.path, function (data) { $("#" + setting.placeHolder).html(data); })
}


function loadPagenation(options) {
    var setting = { path: "../_pagelet/pagenation.html", placeHolder: "pagerContent", nopager:false };
    $.extend(setting, options);
    $.get(setting.path, function (data) {
        $("#" + setting.placeHolder).html(data);
        if (setting.nopager) { //不显示分页， 只显示下载
            $("#" + setting.placeHolder).find(".pager").hide();
        }
        
    })
}

function loadBreadPath(options) {
    var setting = { placeHolder: "breadpathContent", paths: [], sitemap: "../_pagelet/navigator.html" };

    if ($.isArray(options)) {
        options = { paths: options };
    }
    $.extend(setting, options);
    // $("#" + setting.placeHolder).text("您当前所在位置： "+setting.paths.join(" >> "));



    $.get(setting.sitemap, function (data) {

        var navigators = $("#nav", data).find("a");

        var $span = $("<span class='nav_path'>您当前所在位置：</span>");


        $.each(setting.paths, function (key, obj) {
            //var matched = $("a:contains('" + obj + "')", navigators);

            var matched = null;
            $.each(navigators, function (i, a) {
                if ($(a).text() == obj) {
                    $(a).text($(a).text());
                    matched = a;
                    return false;
                }
            });
            if (matched != null) {
                $span.append(matched);
            }
            else {
                $span.append("<span>" + obj + "</span>");
            }
            $span.append("<img src='../images/btn08.png'/> ");
        });

        $("#" + setting.placeHolder).append($span);
    })

    //$.each(options, function (key, obj) {

    //});
}


function sceneToggle(showClass, hideClass) {
    $("." + hideClass).hide("fast");
    $("." + showClass).show(1000);
}

$(function () {
    $(".userCheck").click(function () {
        $(this).prev("input").val("王小明");
    });
    $(".calendar").click(function () {
        $(this).prev("input").val("2013-3-27");
    });
});

function InitTabs() {
    $('ul.info_tabs').each(function () {
        // For each set of tabs, we want to keep track of
        // which tab is active and it's associated content
        var $active, $content, $links = $(this).find('a');

        // If the location.hash matches one of the links, use that as the active tab.
        // If no match is found, use the first link as the initial active tab.
        $active = $($links.filter('[href="' + location.hash + '"]')[0] || $links[0]);
        $active.addClass('active');
        $content = $($active.attr('href'));

        // Hide the remaining content
        $links.not($active).each(function () {
            $($(this).attr('href')).hide();
        });


        $links.bind("click", null, function (e) {
            // Make the old tab inactive.
            $active.removeClass('active');
            $content.hide();

            // Update the variables with the new link and content
            $active = $(this);
            $content = $($(this).attr('href'));

            // Make the tab active.
            $active.addClass('active');
            $content.show();

            // Prevent the anchor's default click action
            e.preventDefault();
        });

        // Bind the click event handler
        //$(this).on('click', 'a', function (e) {

        //});
    });
};

function GetQueryString(name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result == null || result.length < 1) {
        return "";
    }
    return result[1];
}


Number.prototype.toFixed = function (len) {
    var add = 0;
    var s, temp;
    var s1 = this + "";
    var start = s1.indexOf(".");
    if (s1.substr(start + len + 1, 1) >= 5) add = 1;
    var temp = Math.pow(10, len);
    s = Math.floor(this * temp) + add;
    return formatCurrency(s / temp);
};

//用法 ： var str = "{0},{1}".formatBy("Hi", " Bond");
// str = "Hi, Bond"
String.prototype.formatBy = function () {

    if (arguments.length == 0) {
        return this;
    }
    for (var StringFormat_s = this, StringFormat_i = 0; StringFormat_i < arguments.length; StringFormat_i++) {
        StringFormat_s = StringFormat_s.replace(new RegExp("\\{" + StringFormat_i + "\\}", "g"), arguments[StringFormat_i]);
    }

    return StringFormat_s;

};

/**
 * 将数值四舍五入(保留2位小数)后格式化成金额形式
 *
 * @param num 数值(Number或者String)
 * @return 金额格式的字符串,如'1,234,567.45'
 * @type String
 */
function formatCurrency(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' +
        num.substring(num.length - (4 * i + 3));
    return (((sign) ? '' : '-') + num + '.' + cents);
}



/**
 * 将数值四舍五入(保留1位小数)后格式化成金额形式
 *
 * @param num 数值(Number或者String)
 * @return 金额格式的字符串,如'1,234,567.4'
 * @type String
 */
function formatCurrencyTenThou(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 10 + 0.50000000001);
    cents = num % 10;
    num = Math.floor(num / 10).toString();
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' +
        num.substring(num.length - (4 * i + 3));
    return (((sign) ? '' : '-') + num + '.' + cents);
}