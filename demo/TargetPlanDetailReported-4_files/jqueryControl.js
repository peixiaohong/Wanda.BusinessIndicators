(function ($) {
    $.fn.extend({
        //isDefaultAll ture:默认选择全部
        MultDropList: function (e, defaultid, callback, isDefaultAll) {
            $("#" + e).hide();

            var opsdata = "";
            var totalOpsLength = 0;
            var ops = $("#" + e + " option");
            for (i = 0; i < ops.length; i++) {
                var v = ops.eq(i).text();
                var k = v;
                if (ops.eq(i).val() != null && ops.eq(i).val() != undefined && ops.eq(i).val() != "") {
                    k = ops.eq(i).val();
                }
                if (k.length > 0 && v.length > 0) {
                    var vs = '{k:"' + k + '",v:"' + v + '"}';
                    if (opsdata.indexOf(vs) <= 0) {
                        if (opsdata.length > 0) {
                            opsdata += " | "
                        }
                        opsdata += vs;
                        totalOpsLength++;
                    }
                }
            }

            var op = $.extend({ wraperClass: "multiselect", width: $("#" + e).width(), height: $("#" + e).height(), data: "", selected: "" }, { data: opsdata });
            return this.each(function () {
                var $this = $(this); //指向TextBox
                var $hf = $(this).next(); //指向隐藏控件存
                var conSelector = "#" + $this.attr("id") + ",#" + $hf.attr("id");
                var $wraper = $(conSelector).wrapAll("<div><div></div></div>").parent().parent().addClass(op.wraperClass);

                var $list = $('<div class="list"></div>').appendTo($wraper);
                $list.css({ "width": op.width, "height": op.height });
                //控制弹出页面的显示与隐藏
                $this.click(function (e) {
                    $(".list").hide();
                    var lis = $("li", $ul);
                    $.each(lis, function (i, l) {
                        $(l).show();
                    });
                    $list.toggle();
                    e.stopPropagation();
                });

                $this.keyup(function (e) {
                    var lis = $("li", $ul);
                    $.each(lis, function (i, l) {
                        if ($(l).text().indexOf($this.val()) < 0) {
                            $(l).hide();
                        }
                        else {
                            $(l).show();
                        }
                    });
                });

                $this.blur(function (e) {
                    if (document.getElementById(defaultid + "mckall").checked) {
                        $this.val("全部");
                    } else {
                        var kArr = new Array();
                        var vArr = new Array();
                        $("input[class!='mckselectAll']:checked", $ul).each(function (index) {
                            kArr[index] = $(this).val();
                            vArr[index] = $(this).next().text();
                        });
                        $hf.val(kArr.join(","));
                        $this.val(vArr.join(","));
                    }
                });

                $(document).click(function () {
                    $list.hide();
                });

                $list.filter("*").click(function (e) {
                    e.stopPropagation();
                });
                //加载默认数据
                $list.append('<ul><li><input type="checkbox" class="mckselectAll" id="' + defaultid + 'mckall"/><label for="' + defaultid + 'mckall">全部</label></li></ul>');
                var $ul = $list.find("ul");

                //加载json数据
                var listArr = op.data.split("|");
                var jsonData;
                if (listArr == "")
                    listArr.length = 0;
                for (var i = 0; i < listArr.length; i++) {
                    jsonData = eval("(" + listArr[i] + ")");
                    $ul.append('<li><input type="checkbox" value="' + jsonData.k + '" id="' + defaultid + 'mck' + jsonData.k + '" /><label for="' + defaultid + 'mck' + jsonData.k + '">' + jsonData.v + '</label></li>');
                }

                //加载勾选项
                var seledArr;
                if (op.selected.length > 0) {
                    seledArr = selected.split(",");
                }
                else {
                    seledArr = $hf.val().split(",");
                }

                $.each(seledArr, function (index) {
                    $("li input[value='" + seledArr[index] + "']", $ul).attr("checked", "checked");

                    var vArr = new Array();
                    $("input[class!='mckselectAll']:checked", $ul).each(function (index) {
                        vArr[index] = $(this).next().text();
                    });
                    $this.val(vArr.join(","));
                });

                //默认全选 add by wangyc bgn
                if (isDefaultAll) {
                    var cks = $("li input", $ul);
                    for (i = 0; i < cks.length; i++) {
                        cks[i].checked = true;
                    }
                    var kArr = new Array();
                    var vArr = new Array();
                    $("input[class!='mckselectAll']:checked", $ul).each(function (index) {
                        kArr[index] = $(this).val();
                        vArr[index] = $(this).next().text();
                    });
                    $hf.val(kArr.join(","));
                    $this.val(vArr.join(","));
                    var lis = $("li", $ul);
                    $.each(lis, function (i, l) {
                        $(l).show();
                    });
                }
                //如果选择全部,文本框显示"全部"
                if (document.getElementById(defaultid + "mckall").checked) {
                    $this.val("全部");
                }
                //默认全选 add by wangyc end          

                //全部选择或全不选
                $("input[class='mckselectAll']", $ul).first().click(function () {
                    var cks = $("li input[class!='mckselectAll']", $ul);
                    if (document.getElementById(defaultid + "mckall").checked) {
                        for (i = 0; i < cks.length; i++) {
                            cks[i].checked = true;
                        }

                    }
                    else {
                        for (i = 0; i < cks.length; i++) {
                            cks[i].checked = false;
                        }
                    }
                });
                //点击其它复选框时，更新隐藏控件值,文本框的值
                $("input", $ul).click(function () {
                    //如果有未选中元素 则全选按钮置为未选
                    //add by wangyc 20130618 bgn
                    var vUncheckArr = $("input[class!='mckselectAll']:not(:checked)", $ul);
                    //有元素未被选中
                    if (vUncheckArr.length > 0) {
                        document.getElementById(defaultid + "mckall").checked = false;
                    }
                    else {
                        document.getElementById(defaultid + "mckall").checked = true;
                    }
                    //add by wangyc 20130618 end
                    var kArr = new Array();
                    var vArr = new Array();
                    $("input[class!='mckselectAll']:checked", $ul).each(function (index) {
                        kArr[index] = $(this).val();
                        vArr[index] = $(this).next().text();
                    });
                    $hf.val(kArr.join(","));
                    $this.val(vArr.join(","));
                    eval(callback);
                    var lis = $("li", $ul);
                    $.each(lis, function (i, l) {
                        $(l).show();
                    });
                    if (isDefaultAll) {
                        //如果选择全部,文本框显示"全部"
                        if (document.getElementById(defaultid + "mckall").checked) {
                            $this.val("全部");
                        }
                    }

                });

            });
        },
    });
})(jQuery);
