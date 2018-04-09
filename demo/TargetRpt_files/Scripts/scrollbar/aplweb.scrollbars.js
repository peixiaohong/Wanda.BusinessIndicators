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
            var $scrollframe = $(this);
            //var $ = $('#g1_1');
            var $scrollwrap = $(this).find('table.collectTab');
            var $hscroller = $('.scrollable .hscroller');
            var $hscrollblock = $('.scrollable .scrollblock');
            var $hscrollarea = $('.scrollable .scrollarea');
            var $scrollleft = $('.scrollable .scrollleft');
            var $scrollright = $('.scrollable .scrollright');
            var wid = $("#navContainer").innerWidth() + $("#switchPoint").innerWidth() + 2;
            var switchPoint = $("#switchPoint");
            var navContainer = $("#navContainer");
            var invisible = navContainer.data("hide");
            //START

            $scrollleft.mousehold(100, function () {
                $scrollframe.scrollLeft($scrollframe.scrollLeft() - 40);
                updateScrollers();
            });

            $scrollright.mousehold(100, function () {
                $scrollframe.scrollLeft($scrollframe.scrollLeft() + 40);
                updateScrollers();
            });

            $("#switchPoint").unbind('click').removeAttr('onclick').click(ResizeScroll);
            
            function ResizeScroll() {
                var invisible1 = navContainer.data("hide");
                if (invisible1) {
                    switchPoint.removeClass("spread_handler").addClass("shrink_handler");
                    navContainer.show("fast", function () {
                        $scrollleft.css({ left: wid, width: "16px" });
                        $hscrollarea.css({ left: wid + 16, width: $hscroller.innerWidth() - wid - 16 * 2 });
                        
                        updateScrollers();
                    });
                    navContainer.data("hide", false);
                }
                else {
                    switchPoint.removeClass("shrink_handler").addClass("spread_handler");
                    navContainer.hide("fast", function () {
                        $scrollleft.css({ left: $("#switchPoint").innerWidth(),width:"16px" });
                        $hscrollarea.css({ left: 16 + $("#switchPoint").innerWidth(), width: $hscroller.width() - 16 * 2 - $("#switchPoint").innerWidth() });
                        
                        updateScrollers();
                    });
                    navContainer.data("hide", true);
                }
            }

            var scrollHandler = null;

            $hscrollblock.unbind("drag");

            $hscrollblock.drag("start", function (e, dd) {
                dd.origLeft = $(this).position().left;
            }).drag(function (e, dd) {
                var maxLeft = $(this).parent().innerWidth() - $(this).outerWidth();
                var newLeft = Math.max(0, Math.min(maxLeft, dd.origLeft + dd.deltaX));


                $(this).css({ left: newLeft });
                //console.log("maxLeft£º" + maxLeft + "; newLeft£º" + newLeft);

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
            //$(window).resize(scrollEvent);
            $(window).mousewheel(scrollEvent);
            $(".rank2").click(scrollEvent);
            $(".rank3").click(scrollEvent);

            window.resizeHandler = null;
            $(window).resize(function () {
                if (window.resizeHandler != null) clearTimeout(window.resizeHandler);
                window.resizeHandler = setTimeout(scrollEvent //function () { //head.width(head.parent().width() - 1);},
                                                , 200);

            });

            $hscrollarea.mousedown(function (e) {
                var maxLeft = $(this).innerWidth() - $hscrollblock.outerWidth();
                var newLeft = Math.max(0, Math.min(maxLeft, e.pageX - $(this).offset().left - ($hscrollblock.outerWidth() / 2.0)));

                $hscrollblock.css({ left: newLeft });

                $scrollframe.scrollLeft((newLeft / maxLeft) * ($scrollwrap.innerWidth() - $scrollframe.innerWidth()));
            });

            if (invisible) {
                $scrollleft.css({ left: $("#switchPoint").innerWidth(), width: "16px" });
                $hscrollarea.css({ left: 16 + $("#switchPoint").innerWidth(), width: $hscroller.width() - 16 * 2 - $("#switchPoint").innerWidth() });
                
            }
            else {
                $scrollleft.css({ left: wid, width: "16px" });
                $hscrollarea.css({ left: wid + 16, width: $hscroller.innerWidth() - wid - 16 * 2 });
            }

            updateScrollers();
            function updateScrollers() {
                var amountWidth = $scrollframe.innerWidth() / $scrollwrap.innerWidth();
                if (amountWidth < 1) {
                    $scrollable.show();
                    var hscrollWidth = amountWidth * $hscrollarea.innerWidth();
                    var availableWidth = $scrollwrap.innerWidth() - $scrollframe.innerWidth();
                    var amountLeft = $scrollframe.scrollLeft() / availableWidth;
                    var hscrollLeft = amountLeft * ($hscrollarea.innerWidth() - hscrollWidth);
                    $hscrollblock.css({ left: hscrollLeft, width: hscrollWidth });
                }
                else {
                    $scrollable.hide();
                }
            };
        });
    };
})(jQuery);

