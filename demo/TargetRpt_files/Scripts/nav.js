// JavaScript Document
        $(function () {
            $(".nav li").click(function () {
                $("li[class='current']").removeAttr("class");
                $(this).addClass("current"); 
            });
            $(".nav li dl dd").click(function () {
                $("dd[class='current2']").removeAttr("class");
                $(this).addClass("current2"); 
            });
            $(".nav ul li dl dd .sanji p a").click(function () {
                $("a[class='current3']").removeAttr("class");
                $(this).addClass("current3"); 
            });			
			$("dd").click(function(){
				$("dd div").toggle()
								   });
        });