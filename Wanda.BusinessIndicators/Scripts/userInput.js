/*
Author: Hu wei zheng
Usage: 实现用户的输入框选择

依赖： jquery 1.4.3及以上
       autocomplete.js
       base.js
       /userinfocontroller/getUserInfo 根据参数获得用户信息
*/

$.fn.extend({
    userInput: function (options) {
        var self = $(this);
        var requesetUrl = "/userinfocontroller/GetUserInfo";
        //在check方式下，实现getUser;
        var _setting = {
            op: "init", //op= init or check
            getUser: function (ctrl, rowData, value) {
                //alert(value);
            }, //选中人后，取选中值
            userWindow: true,  //如果有userWindow, 则没有chekUser
            textLength: 200,
            checkUser: function () {

            }, //点击输入框头像， 检查人员是否存在
            allowMulti:false
        };
        $.extend(_setting, options);

        if (_setting.op === "check") { //通过程序赋值后， 可以手动检查
            return $(this).each(function () {
                initUser($(this));
            });

        }

        return $(this).each(function () {
            var $ctrl = $(this);
            $ctrl.nextAll(".icon-user").remove();
            $ctrl.css({ width: $ctrl.width() - 16 });
            var btnCheck = $("<i class='icon icon-user curser userinput'></i>");
            btnCheck.css({
                backgroundColor: $ctrl.css("background-color"),
                height: $ctrl.height() - 1,
                cursor: "hand"
            });
            btnCheck.insertAfter($ctrl);
            if ($ctrl.attr("data-wduserid") != "") {
                var v = $ctrl.attr("data-wduserid");
                var hd = $("<input type='hidden userInputAppendix' class='hidden userInputAppendix'/>");
                hd.val(v).insertAfter($ctrl);
            }
            if (_setting.userWindow) {
                if (btnCheck.userWindow) btnCheck.userWindow({
                    allowMulti: _setting.allowMulti,
                    showOne: false,
                    onSubmit: function (context, users) {
                        //return _setting.onSubmit(context, users);
                        if (users && users.length > 0) {
                            initUser2($ctrl, users[0].data);
                            return true;
                        }
                    }
                });
            }
            else {
                if ($.type(_setting.checkUser) == "function") {
                    btnCheck.bind("click", _setting.checkUser);
                } else {
                    btnCheck.hide();
                }
            }

            var requesetUrl2 = "/AjaxHander/GetUserInfoHandler.ashx";
            $ctrl.autocomplete(requesetUrl2, {
                matchContains: true,
                minChars: 1,
                max: 20,
                width: 330,
                scrollHeight: 480,
                autoFill: false,
                mustChoose: true,
                mustMatch: false,
                parse: function (datas1) {
                    var datas = JSON.parse(datas1);
                    return $.map(datas, function (row) {
                        return {
                            data: row,
                            value: row.UserID,
                            result: formatResult("{0}".formatBy(row.Name))
                        }
                    })
                },
                formatItem: function (row, i, max) {
                    //  return "{0}{1}({2})".formatBy(row.Name,   row.LoginName, row.DeptName);
                    return ["<span style='display:block;font:bold'><i class='icon icon-user'></i>{0}{1}</span>",
                        "<span style='display:block;width:100%;height:16px;overflow:hidden'>{2}</span>"
                    ].join("").formatBy(row.Name, row.LoginName, row.DeptName);
                },
                formatMatch: function (row, i, max) {
                    return "{0}-{1}-{2}".formatBy(row.Name, row.LoginName, row.UserID);
                },
                formatResult: function (row) {
                    var result = "{0}".formatBy(row.Name);
                    return formatResult(result);
                }
            }).result(
            function (event, row, formatted) {
                if (!row) {
                    afterChoose($ctrl, 0);
                    _setting.getUser($ctrl, {}, 0);

                } else {
                    afterChoose($ctrl, row.UserID);
                    // if (row.UserID) { //用户被清空， 
                    _setting.getUser($ctrl, row, row.UserID);
                    //}
                }
            });
            $ctrl.change(function (e) {
                initUser($(e.currentTarget));
            });

        });
        function formatResult(result) {
            if (result.length > _setting.textLength - 3 && _setting.textLength > 3) {
                result = result.leftSub(_setting.textLength - 3) + "...";
            }
            return result;
        }
        function afterChoose(sender, v) {
            var sender = $(sender);
            sender.nextAll(".userInputAppendix").remove();

            var field = sender.attr("data-field") || sender.data("data-field");
            if (field) {
                var hd = $("<i class='hidden userInputAppendix'  data-fieldtype='n'></i>");
                sender.data("data-field", field);
                sender.removeAttr("data-field");
                hd.attr("data-field", field).attr("data-fieldvalue", v).insertAfter(sender);
            }
            else {
                var hd = $("<input type='hidden userInputAppendix' class='hidden userInputAppendix'/>");
                hd.val(v).insertAfter(sender);
            }
        }

        function initUser(ctrl) {
            WebUtil.ajax({
                url: requesetUrl,
                async: false,
                args: { term: ctrl.val(), count: 1 },
                successReturn: function (result) {
                    if (result.length > 0) {
                        var rowItem = result[0];
                        ctrl.val("{0}".formatBy(rowItem.Name));

                        afterChoose(ctrl, rowItem.UserID);
                        if ($.type(_setting.getUser) == "function") {
                            //_setting.getUser(null, rowItem, rowItem.UserID);
                            _setting.getUser(ctrl, rowItem, rowItem.UserID);
                        }
                    }
                }
            });
        }
        function initUser2(ctrl, UserInfo) {
            var rowItem = UserInfo;
            ctrl.val("{0}".formatBy($.trim(rowItem.UserName)));

            afterChoose(ctrl, rowItem.UserId);
            if ($.type(_setting.getUser) == "function") {
                //_setting.getUser(null, rowItem, rowItem.UserID);
                _setting.getUser(ctrl, rowItem, rowItem.UserId);
            }

        }
    }
});

