// 选项卡切换
        $().ready(

        function () {
            $(".scrolldoorFrame").each(
            function () {
                var liarray = $(this).find("li");
                var labcontentarray = $(this).find(".c01");
                liarray.each(
                    function (index) {
                        $(this).click(
                            function () {
                                //                                alert(index);
                                labcontentarray.hide();
                                $(labcontentarray[index]).show();

                                liarray.removeClass("select");
                                $(liarray[index]).addClass("select");
                            }
                        )
                    }
                );

            }
            );
            //function(){$(this).find("li").click(alert($(this).text())});

        }
        );
