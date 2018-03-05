/*
* A jQuery extension to provide scrollbars to a container
* 
* @author Andrew Lowndes
* @date 08/12/10
*
* @requires jquery.js
* @requires jquery.event.drag.js
* @requires jquery.resize.js
* @requires jquery.mousehold.js
* @requires jquery.mousewheel.js
*/
(function ($) {
    $.fn.scrollbars = function () {
        return $(this).each(function () {
            var $scrollable = $('.scrollable:first');
            var $scrollframe = $('#scrollTest');
            //var $ = $('#g1_1');
            var $scrollwrap = $('.scrollwrap');
            var $hscroller = $('.scrollable .hscroller');
            var $hscrollblock = $('.scrollable .scrollblock');
            var $hscrollarea = $('.scrollable .scrollarea');
            var $scrollleft = $('.scrollable .scrollleft');
            var $scrollright = $('.scrollable .scrollright');
            var wid = $("#navContainer").innerWidth() + $("#switchPoint").innerWidth() + 2;
            //START

            $scrollleft.mousehold(100, function () {
                $scrollframe.scrollLeft($scrollframe.scrollLeft() - 40);
                updateScrollers();
            });

            $scrollright.mousehold(100, function () {
                $scrollframe.scrollLeft($scrollframe.scrollLeft() + 40);
                updateScrollers();
            });

            $("#switchPoint").unbind('click').removeAttr('onclick').click(function () {
                var switchPoint = $("#switchPoint");
                var navContainer = $("#navContainer");
                var invisible = navContainer.data("hide");

                if (invisible) {
                    switchPoint.removeClass("spread_handler").addClass("shrink_handler");
                    navContainer.show("fast", function () {
                        $hscrollarea.css({ left: wid + $scrollleft.innerWidth(), width: $scrollable.innerWidth() - wid - $scrollleft.innerWidth() * 2 });
                        $scrollleft.css({ left: wid });
                        updateScrollers();
                    });
                    navContainer.data("hide", false);
                }
                else {
                    switchPoint.removeClass("shrink_handler").addClass("spread_handler");
                    navContainer.hide("fast", function () {
                        $hscrollarea.css({ left: $scrollleft.innerWidth() + $("#switchPoint").innerWidth(), width: $scrollable.width() - $scrollleft.innerWidth() * 2 - $("#switchPoint").innerWidth() });
                        $scrollleft.css({ left: $("#switchPoint").innerWidth() });
                        updateScrollers();
                    });
                    navContainer.data("hide", true);
                }
            });

            $hscrollblock.drag("start", function (e, dd) {
                dd.origLeft = $(this).position().left;
            }).drag(function (e, dd) {
                var maxLeft = $(this).parent().innerWidth() - $(this).outerWidth();
                var newLeft = Math.max(0, Math.min(maxLeft, dd.origLeft + dd.deltaX));

                $(this).css({ left: newLeft });
                $scrollframe.scrollLeft((newLeft / maxLeft) * ($scrollwrap.innerWidth() - $scrollframe.innerWidth()));
            });

            function scrollEvent() {
                updateScrollers();
                if ($scrollframe.offset().top + $scrollframe.innerHeight() > $(window).scrollTop() + $(window).height()) {
                    $scrollable.show();
                }
                else {
                    $scrollable.hide();
                }
            }

            $(window).bind("scroll", scrollEvent);
            $(window).resize(scrollEvent);
            $(window).mousewheel(scrollEvent);
            $(".sumrow1").click(scrollEvent);
            $(".sumrow2").click(scrollEvent);

            $hscrollarea.mousedown(function (e) {
                var maxLeft = $(this).innerWidth() - $hscrollblock.outerWidth();
                var newLeft = Math.max(0, Math.min(maxLeft, e.pageX - $(this).offset().left - ($hscrollblock.outerWidth() / 2.0)));

                $hscrollblock.css({ left: newLeft });
                $scrollframe.scrollLeft((newLeft / maxLeft) * ($scrollwrap.innerWidth() - $scrollframe.innerWidth()));
            });

            $hscrollarea.css({ left: wid + $scrollleft.innerWidth(), width: $scrollable.innerWidth() - wid - $scrollleft.innerWidth()*2 });
            $scrollleft.css({ left: wid });
            updateScrollers();
            function updateScrollers() {
                var amountWidth = $scrollframe.innerWidth() / $scrollwrap.innerWidth();
                if (amountWidth < 1) {
                    var hscrollWidth = amountWidth * $hscrollarea.innerWidth();
                    var availableWidth = $scrollwrap.innerWidth() - $scrollframe.innerWidth();
                    var amountLeft = $scrollframe.scrollLeft() / availableWidth;
                    var hscrollLeft = amountLeft * ($hscrollarea.innerWidth() - hscrollWidth);
                    $hscrollblock.css({ left: hscrollLeft, width: hscrollWidth });
                }
            };
        });
    };
})(jQuery);
