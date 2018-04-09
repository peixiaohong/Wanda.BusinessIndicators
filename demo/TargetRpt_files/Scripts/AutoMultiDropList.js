
/*
多选下拉控件。 
Example
html: 
        <input type="text" id="autoMulti" />
JS:
            var options = {
                data: { "aaa": 0, "aaaa": 4, "aadaa": 5, "bbb": 10, "ccc": 1, "ddd": 2, '测试': 3 ,'a':6,'aa':7,'aaaaa':8}
            };
            $("#autoMulti").AutoMultiDropList(options);

     data为键值组合， 显示键， 保存值

*/
(function ($) {
    $.fn.extend({

        AutoMultiDropList: function (options) {


            var _setting = {
                id: "multiselect",
                wraperClass: "multiselect",
                width: 0,
                height: "auto",
                simple: false, //如果是simple模式， 则data是json 值键对 格式如{text:value}； 否则根据dataParser来决定
                data: {},  //  
                dataParser: {
                    valueField: "", // 保存的值
                    textFields: [],  // 显示的文字
                    textFieldsFormat: "{0}", // 显示的文字的格式
                    tipFields: []   //提示的字段

                },
                selectedValues: [], //值数组， 保存value值
                getText: function (data, dataParser) {
                    if (typeof (dataParser.textFields) == "string") {
                        return data[dataParser.textFields];
                    }

                    if ($.isArray(dataParser.textFields)) {
                        var items = $.map(dataParser.textFields, function (p) { return data[p]; });

                        return dataParser.textFieldsFormat.formatBy(items);
                    }

                    return "";
                },
                onItemClicked: function (selectedValues) { }, //单选选中的时候激发的事件
                onCollapsed: function (selectedValues) { } //收起的时候激发的事件
            };

            $.extend(_setting, options);


            return this.each(function () {
                var $this = $(this); //指向TextBox
                var conSelector = "#" + $this.attr("id");
                var $wraper = $(conSelector).wrapAll("<div><span></span></div>").parent().parent().addClass(_setting.wraperClass);
                var $choosenBar = $("<span></span>").prependTo($wraper);


                var $list = $('<div class="list"></div>').appendTo($wraper);
                if (_setting.width == 0) {
                    _setting.width = $(this).width();
                }
                $list.css({ "width": _setting.width, "height": _setting.height });
                ////控制弹出页面的显示与隐藏
                //$this.click(function (e) {
                //    // 关闭弹出框
                //    if ($list.is(":visible")) {
                //        $list.hide();
                //        if (_setting.onCollapsed && $.isFunction(_setting.onCollapsed)) {
                //            _setting.onCollapsed(_setting.selectedValues);
                //        }
                //    } 
                //    e.stopPropagation();
                //});

                // 初始化值
                if ($.isArray(_setting.selectedValues) && _setting.selectedValues.length > 0) {

                    $.each(_setting.selectedValues, function (i, val) {


                        var text = null;
                        if (_setting.simple) {
                            $.each(_setting.data, function (itemName, itemVal) {
                                if (itemVal == val) {
                                    text = itemName;
                                    return false;
                                }
                            });
                        }
                        else {
                            $.each(_setting.data, function (i, dataItem) {
                                if (dataItem[_setting.dataParser.valueField] == val) {
                                    //text = dataItem[_setting.dataParser.textField];
                                    text = _setting.getText(dataItem, dataParser);
                                    return false;
                                }
                            });
                        }


                        if (text == null) {
                            _setting.selectedValues = $.grep(_setting.selectedValues, function (item) {
                                return item != val;
                            });
                            return;
                        }

                        var value = val;

                        var $item = $("<span></span>");
                        $choosenBar.append($item);
                        $item.prepend('<span class="text">' + text + '</span><span style="display:none" name="val">' + value + '</span>');
                        $item.append("<a href='javascript:void(0)' class='btnX'></a>");

                        $item.find("a.btnX").click(function () {
                            $(this).parent().remove();

                            // 移除值
                            var delValue = $(this).parent().find("span[name=val]").text();
                            _setting.selectedValues = $.grep(_setting.selectedValues, function (item) {
                                return item != delValue;
                            });

                            //移除获得值
                            if (_setting.onCollapsed && $.isFunction(_setting.onCollapsed)) {
                                _setting.onCollapsed(_setting.selectedValues);
                            }

                        });
                    });
                }

                $this.keyup(function (e) {
                    if ($.trim($this.val()) == "") {
                        $list.html("");
                        return;
                    }

             

                    var items = {};
                    var text = $.trim($this.val().toLocaleLowerCase());
                    if (_setting.simple) {

                        $.each(_setting.data, function (key, value) {

                            if ($.inArray(value + "", _setting.selectedValues) >= 0) {
                                return;
                            }

                            if (key.toLocaleLowerCase().indexOf(text) >= 0) {
                                items[key] = value;
                            }
                        });
                    }
                    else {
                        $.each(_setting.data, function (i, dataItem) {
                            var dp = _setting.dataParser;
                            var value = dataItem[dp.valueField];
                            if ($.inArray(value + "", _setting.selectedValues) >= 0) {
                                return;
                            }

                            var found = false;

                            $.each(dp.tipFields, function (j, field) {
                                if (dataItem[field].toLocaleLowerCase().indexOf(text) >= 0) {
                                    found = true;
                                    return false;
                                }
                            });

                            if (found) {
                                //items[dataItem[dp.textField]] = value;
                                items[_setting.getText(dataItem, dp)] = value;
                            }
                        });
                    }

                    

                    BindItems($list, items);

                    if (e.keyCode == 13) {
                        $list.find("a:first").click();
                        return;
                    }

                    $list.css("position", "absolute");
                    $list.css("width", $this.width() + 4);
                    $list.css("left", $this.offset().left, "top", 0);
                    $list.show();
                });

                // 关闭
                $(document).click(function () {
                    if ($list.is(":visible")) {
                        $list.hide();
                        if (_setting.onCollapsed && $.isFunction(_setting.onCollapsed)) {
                            _setting.onCollapsed(_setting.selectedValues);
                        }
                    }
                });

                $list.click(function (e) {
                    e.stopPropagation();
                });



                //加载默认数据
                function BindItems($list, items) {
                    $list.html("");
                    $list.append('<ul></ul>');
                    var $ul = $list.find("ul");

                    // 单选项
                    for (var obj in items) {
                        var text = obj;
                        var value = items[obj];
                        $ul.append('<li> <a  style="display:block"><span class="text">' + text + '</span><span style="display:none" name="val">' + value + '</span></a></li>');

                    }
                    $("a", $ul).click(function () {
                        var $item = $("<span></span>");
                        $choosenBar.append($item);
                        $item.prepend($(this).html());
                        $item.append("<a href='javascript:void(0)' class='btnX'></a>");
                        $list.hide();
                        $this.val("").focus(); //清空输入

                        _setting.selectedValues.push($(this).find("span[name=val]").text());

                        //添加获得值
                        if (_setting.onCollapsed && $.isFunction(_setting.onCollapsed)) {
                            _setting.onCollapsed(_setting.selectedValues);
                        }

                        $item.find("a.btnX").click(function () {
                            $(this).parent().remove();

                            // 移除值
                            var delValue = $(this).parent().find("span[name=val]").text();
                            _setting.selectedValues = $.grep(_setting.selectedValues, function (item) {
                                return item != delValue;
                            });

                            //移除获得值
                            if (_setting.onCollapsed && $.isFunction(_setting.onCollapsed)) {
                                _setting.onCollapsed(_setting.selectedValues);
                            }

                        });
                    });

                }

            });
        }
    });
})(jQuery);