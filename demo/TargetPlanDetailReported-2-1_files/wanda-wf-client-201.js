//http://localhost/WebApp/Wanda.Platform.WorkFlow.Controls.Handler/
var isCc = true;
var wanda_wf = {
    settings: {
        resoucehost: 'http://wfuat.wanda-dev.cn/WebApp/Wanda.Platform.WorkFlow.Controls.Handler/',
        //resoucehost: 'http://wf.LJTH.cn/WebApp/Wanda.Platform.WorkFlow.Controls.Handler/',
        ProcessDataKey: "processInfo",//"MCS_WF_ProcessJSON",
        title: '由业务系统自定义',
        PostDataKey: "MCS_WF_OperationJSON",
        navhost: "workflowtd",
        opinContainer: "OpinionTable",
        buttonHost: "buttontd",
        defaultdom: "",
        currentOpinhostID: "wf_control_currentOpin",
        opinContentHelper: "wf_control_defaultOpinContent",
        referesh: false,
        disableReadyRun: true,
        submitUrl: undefined,
        submitFun: function () {
            document.getElementById(wanda_wf.settings.ProcessDataKey).form.submit();
        },
        template: function () {
            return "<div id='wanda_wf_content'>" +
                "<table style='width: 100%; border: none;'>" +
                "<tr>" +
                    "<td>" +
                        "<table style='border: 1px solid #cccccc; width: 100%;'>" +
                            "<tr>" +
                                "<td style='border-bottom: 1px dashed #cccccc;'>" +
                                    "<table style='border: none; width: 100%; height: 100%;'>" +
                                        "<tr style='border-bottom: 1px dashed #cccccc;'>" +
                                            "<td style='width: 90px; text-align: right; padding-top: 5px; vertical-align: top; padding-right: 4px;'>" +
                                                "<span style='font-size: 14px;' class='wanda_wf_filed'>审批流程：</span>" +
                                            "</td>" +
                                            "<td id='workflowtd'></td>" +
                                        "</tr>" +
                                        "<tr style='display:none;'>" +
                                            "<td style='width: 90px; text-align: right;'>" +
                                                "<span style='font-size: 14px;' class='wanda_wf_filed'>抄送：</span>" +
                                            "</td>" +
                                            "<td id='wanda_wf_ccRange' ></td>" +
                                        "</tr>" +
                                    "</table>" +
                                "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td style='border-bottom: 1px dashed #cccccc; min-height: 136px;' id='currentOpin'>" +
                                    "<table style='border: none; width: 100%;'>" +
                                        "<tr>" +
                                            "<td style='width: 90px; text-align: right; padding-right: 4px; font-size: 14px;' class='filed' >签字意见：</td>" +
                                            "<td>" +
                                                "<select style='width: 100%;' id='wf_control_defaultOpinContent'>" +
                                                    "<option value='-1'>--常用批示语--</option>" +
                                                    "<option value=''>同意</option>" +
                                                    "<option value=''>不同意</option>" +
                                                    "<option value=''>收到</option>" +
                "<option value=''>请领导审批</option>" +
                                                "</select>" +
                                            "</td>" +
                                            "<td style='width: 5px;'></td>" +
                                        "</tr>" +
                                        "<tr>" +
                                            "<td style='width: 90px; text-align: right; padding-right: 4px;'></td>" +
                                            "<td style='padding-right: 4px;'>" +
                                                "<textarea id='wf_control_currentOpin' style='border: 1px solid #cccccc; height: 120px; width: 99%;margin:2px;'></textarea>" +
                                            "</td>" +
                                            "<td style='width: 5px; vertical-align: middle;'>" +
                                                "<span style='color: red;'>*</span>" +
                                            "</td>" +
                                        "</tr>" +
                                    "</table>" +
                                "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td id='buttontd' style='min-height: 60px; text-align: center; vertical-align: middle;'></td>" +
                            "</tr>" +
                        "</table>" +
                    "</td>" +
                "</tr>" +
                "<tr class='wanda-wf-approval'>" +
                    "<td>" +
                       "<img src='" + wanda_wf.settings.resoucehost + "Images/Opinion_Title_Bg.png' alt='' style='margin-bottom: -8px; margin-left: -1px;' ></img>" +
                        "<span style='margin-left: -170px;' class='filed'>审批日志</span>" +
                    "</td>" +
            "</tr>" +
            "<tr class='wanda-wf-approval'>" +
                "<td class='td'>" +
                    "<table id='OpinionTable' class='table'>" +
                        "<tr class='table_title_tr'>" +
                            "<td class='table_title_td' style='width: 180px;'>节点</td>" +
                            "<td class='table_title_td'>审批意见</td>" +
                            "<td class='table_title_td' style='width: 150px;'>审批人</td>" +
                            "<td class='table_title_td' style='width: 150px;'>审批时间</td>" +
                            "<td class='table_title_td' style='width: 120px; border-right: none;'>操作" +
                            "</td>" +
                        "</tr>" +
                    "</table>" +
                "</td>" +
            "</tr>" +
        "</table>" +
    "</div>";
        },
        controlTemplate: {
            ActivityAdd: "<div> <table border='0' id='addactTool' style='margin: 0 auto; min-width: 340px;'>" +
            "<tr>" +
                "<td style='text-align: right'>加签<span style='color: red;'>*</span>:</td>" +
                "<td colspan='2' id='wanda_wf_addactUsers'>" +
                    "<a id='wanda_wf_addact_userSelector' style='color: #3776f7;cursor: pointer;'>请选择</a>" +
                "</td>" +
            "</tr>" +
            "<tr>" +
                "<td style='width: 80px; text-align: right'>节点:</td>" +
                "<td>" +
                    "<select id='wanda_wf_avtlist' style='width: 95%'></select>" +
                "</td>" +
                "<td id='wanda_wf_relation' style='width: 150px;'></td>" +
            "</tr>" +
            "<tr>" +
                "<td style='text-align: right'>审批类型:</td>" +
                "<td style='width: 200px;' id='wanda_wf_addtype' colspan='2'></td>" +
            "</tr>" +
            "<tr>" +
                "<td style='text-align: right'>加签意见<span style='color: red;'>*</span>:</td>" +
                "<td colspan='2'>" +
                    "<textarea id='wanda_wf_addActContent' style='width: 98%; height: 40px;'></textarea>" +
                "</td>" +
            "</tr>" +
        "</table></div>",
            Forward: "<div><div>" +
            "<table class='noboder_tab' style='width:650px;'>" +
                "<tr>" +
                    "<td style='width: 80px;'>接收人<span style='color: red;'>*</span>:</td>" +
                    "<td style='text-algin:left;'>" +
                        "<a id='wanda_wf_transferSelecor' style='color: #3776f7;cursor: pointer;'>请选择</a>" +
                    "</td>" +
                "</tr>" +
                "<td>转发意见<span style='color: red;'>*</span>:</td>" +
                "<td>" +
                    "<textarea id='wanda_wf_transferContent' style='width: 98%; height: 40px;'></textarea>" +
                "</td>" +
            "</table>" +
        "</div></div>"
        }
    },
    events: {
        onsummit: function (args) { },
        onbeforeaddactivity: function (args) { },
        onafteraddactivity: function (args) { }
    },
    debug: null,
    UserSelector: function (fn, multiple) {
        /// <summary>选人控件</summary>
        /// <param name="fn" type="function">callback函数</param>
        /// <param name="multiple" type="boolean">多选？</param>
    },
    init: function () {
        function MaskWinSubmit() {
            return;
            if (!window.popupZindex) {
                window.popupZindex = 10000;
            }
            var sWidth, sHeight;
            sWidth = window.screen.availWidth;
            if (window.screen.availHeight > document.body.scrollHeight) {
                sHeight = window.screen.availHeight;

            } else {
                sHeight = document.body.scrollHeight + 20;
            }
            var maskObj = $("<div  class='maskDiv'></div>");
            maskObj.css({ width: sWidth, height: sHeight, zIndex: window.popupZindex++ });
            maskObj.appendTo($("body"));
            $("body").attr("scroll", "no");

            //$("#BigDiv").data("divbox_selectlist", $("select:visible"));
            //$("select:visible").hide();
            maskObj.data("divbox_scrolltop", $.ScrollPosition().Top);
            maskObj.data("divbox_scrollleft", $.ScrollPosition().Left);
            //$("<img src='" + wanda_wf.settings.resoucehost + "images\07.gif'/><img>").appendTo(body)
            if ($(".maskDiv").length == 1) {
                $("html").data("overflow", $("html").css("overflow"))
                        .css("overflow", "hidden").append($("<center><img src='" + wanda_wf.settings.resoucehost + "images/07.gif'/><img></center>"));
            }
            client.controls.Mask = maskObj;
        }
        var client = {
            process: null,
            controls: {
                nav: null,
                Cc: null,
                Mask: null
            }
        };
        if (wanda_wf.debug != null)
            client = wanda_wf.debug;

        $(document).ready(function () {
            /// <summary>MCS.WF脚本库</summary>
            var wfLibrary = {
                init: function () {
                    if (wfLibrary.inited) {
                        return;
                    }
                    /// <summary>加载</summary>
                    Array.prototype.contains = function (obj) {
                        /// <param name="obj" type="function">每个项是否匹配的方法</param>           
                        /// <summary>是否匹配</summary>
                        var comparer;
                        switch (typeof (obj)) {
                            case "string":
                            case "number":
                            case "object":
                                comparer = function (i) { return i == obj; };
                                break;
                            case "function":
                                comparer = obj;
                                break;
                            default:
                        }
                        for (var i = 0; i < this.length; i++) {
                            if (comparer(this[i]))
                                return true;
                        }
                        return false;
                    };

                    Array.prototype.select = function (fileds) {
                        /// <param name="fileds" type="String">字段名称或者一个fun自己选择</param>
                        /// <summary>选择器</summary>
                        var fn;
                        switch (typeof (fileds)) {
                            case "string":
                                fn = function (obj) { return obj[fileds]; }
                                break;
                            case "object":
                                fn = function (obj) {
                                    var r = new Object();
                                    for (var i = 0; i < fileds.length; i++) {
                                        r[fileds[i]] = obj[fileds[i]];
                                    }
                                    return r;
                                }
                                break;
                            case "function":
                                fn = fileds;
                                break;
                            default:

                        }
                        var r = new Array();
                        for (var i = 0; i < this.length; i++) {
                            r.push(fn(this[i]));
                        }
                        return r;
                    };

                    Array.prototype.where = function (fn) {
                        /// <param name="fn" type="Function">匹配方法</param>
                        /// <summary>条件选择</summary>
                        var newArr = new Array();
                        for (var i = 0; i < this.length; i++) {
                            if (fn(this[i], i)) {
                                newArr.push(this[i]);
                            }
                        }
                        return newArr;
                    };

                    Array.prototype.foreach = function (fn) {
                        for (var i = 0; i < this.length; i++) {
                            if (fn(this[i], i) == true) { break; };
                        }
                    };

                    Array.prototype.all = function (fn) {
                        /// <param name="fn" type="Function">匹配方法</param>
                        /// <summary>是否全部满足</summary>
                        var r = true;
                        this.foreach(function (i) {
                            if (!fn(i)) {
                                r = false;
                                return true;
                            }
                            return false;
                        });
                        return r;
                    };

                    Array.prototype.any = function (fn) {
                        var r = false;
                        this.foreach(function (i) {
                            if (fn(i)) {
                                r = true;
                                return true;
                            }
                            return false;
                        });
                        return r;
                    };

                    Array.prototype.sum = function (fn/*fn:为sum项的选择器*/) {
                        var sum = 0;
                        var fun = fn;
                        if (fn != undefined) {
                            var t = typeof (fn);
                            switch (t) {
                                case "string":
                                    fun = function (i) { return i[fn]; };
                                    break;
                                default:
                            }
                        } else {
                            fun = function (i) { return i; };
                        }
                        this.foreach(function (i) {
                            sum += parseFloat(fun(i));
                        });
                        return sum;
                    };

                    Array.prototype.groupBy = function (fn/*Key selector*/) {
                        var igrouping = new Array();
                        var tempObj = new Object();
                        var keys = new Array();
                        this.foreach(function (i) {
                            var key = fn(i);
                            if (tempObj[key] == undefined) {
                                tempObj[key] = new Array();
                                keys.push(key);
                            }
                            tempObj[key].push(i);
                            return false;
                        });
                        keys.foreach(function (i) {
                            tempObj[i].Key = i;
                            igrouping.push(tempObj[i]);
                            return false;
                        });
                        return igrouping;
                    };

                    Array.prototype.valueCompare = function (arrayB) {
                        if (Object.prototype.toString.call(arrayB) == "[object Array]") {
                            if (this.length != arrayB.length) {
                                return false;
                            }
                            for (var i = 0; i < arrayB.length; i++) {
                                if (!this.contains(arrayB[i], true)) {
                                    return false;
                                }
                            }
                            return true;
                        }
                        return false;
                    };

                    Array.prototype.replaceItem = function (fn, isMachAll, obj) {
                        for (var i = 0; i < this.length; i++) {
                            if (fn(this[i])) {
                                this[i] = obj;
                                if (!isMachAll) {
                                    return;
                                }
                            }
                        }
                    };

                    Array.prototype.remove = function (fn) {
                        for (var i = this.length - 1; i > -1; i--) {
                            if (typeof (fn) == "function") {
                                if (fn(this[i])) {
                                    this.splice(i, 1);
                                }
                            }
                            else {
                                if (this[i] == fn)
                                    this.splice(i, 1);
                                if (typeof (fn) == "Object")
                                    break;;
                            }
                        }
                    };

                    Array.prototype.skip = function (count) {
                        var arr = new Array();
                        for (var i = 0; i < this.length; i++) {
                            if (i + 1 > count) {
                                arr.push(this[i]);
                            }
                        }
                        return arr;
                    };

                    Array.prototype.compareFun = function (x, y, fileds) {
                        /// <summary>arr.sort(function(x,y){ return arr.compareFun(x,y,[filed1,filed2]};)</summary>
                        for (var i = 0; i < fileds.length; i++) {
                            for (var i = 0; i < fileds.length; i++) {
                                if (x[fileds[i]] == y[fileds[i]]) {
                                    if (fileds.length > 1) {
                                        return this.compareFun(x, y, fileds.skip(1));
                                    } else {
                                        return 0;
                                    }
                                }
                                else if (x[fileds[i]] > y[fileds[i]]) {
                                    return 1;
                                } else {
                                    return -1;
                                }
                            }
                        }
                    };

                    if (Array.prototype.indexOf == undefined)
                        Array.prototype.indexOf = function (item) {
                            for (var i = 0; i < this.length; i++) {
                                if (this[i] == item)
                                    return i;
                            }
                            return -1;
                        }

                    Object.clone = function (obj) {
                        //Object.prototype.clone会导致Jquer.css({})出错
                        if (obj == null) {
                            return null;
                        }

                        var newobj;
                        if (Object.prototype.toString.call(obj) == "[object Array]") {
                            newobj = new Array();
                        }
                        else if (Object.prototype.toString.call(obj) == "[object Date]") {
                            obj.getTime = Date.prototype.getTime;
                            newobj = new Date(obj.getTime());
                            delete obj.getTime;
                        }
                        else {
                            newobj = new Object();
                        }
                        for (var i in obj) {
                            if (!isNaN(i)) { //为数组中的项
                                if (typeof (obj[i]) == "object") {
                                    newobj[i] = this.clone(obj[i]);
                                }
                            }
                            if (typeof (obj[i]) == "function") {
                                continue;
                            }
                            else if (typeof (obj[i]) == "object") {
                                newobj[i] = this.clone(obj[i]);
                            } else {
                                newobj[i] = obj[i];
                            }
                        }
                        return newobj;
                    };

                    Date.prototype.format = function (format) {
                        var o = {
                            "M+": this.getMonth() + 1, //month 
                            "d+": this.getDate(), //day 
                            "h+": this.getHours(), //hour 
                            "m+": this.getMinutes(), //minute 
                            "s+": this.getSeconds(), //second 
                            "q+": Math.floor((this.getMonth() + 3) / 3), //quarter 
                            "S": this.getMilliseconds() //millisecond 
                        }

                        if (/(y+)/.test(format)) {
                            format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
                        }

                        for (var k in o) {
                            if (new RegExp("(" + k + ")").test(format)) {
                                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
                            }
                        }
                        return format;
                    }

                    if (String.prototype.trim == undefined) {
                        String.prototype.trim = function () {
                            return this.replace(/(^\s*)|(\s*$)/g, "");
                        }
                    }
                    wfLibrary.inited = true;
                }
            };
            if (client.controls.Mask != null)
                client.controls.Mask.remove();
            wanda_wf.settings.referesh = false;
            wfLibrary.init();
            if ($("#" + wanda_wf.settings.ProcessDataKey).val().trim() == "")
                return;
            initData();
            initControls();
            wanda_wf.debug = client;
        });

        //Update by 2015-8-18
        function ChangeDateFormat(time) {
            if (typeof (time) == 'string') {

                if (time.indexOf("Date") >= 0) {
                    time = new Date(parseInt(time.replace("/Date(", "").replace(")/", ""), 10));
                    return time.format('yyyy-MM-dd hh:mm:ss')
                } else {

                    time = time.replace(/-/g, "/");
                    var date = new Date(time);
                    time = WF_Change_FormatDate(date, true, "");
                    return time;
                }
            }
            
        }


        //格式化日期类型
        function WF_Change_FormatDate(obj, displayTime, local) {
            try {
                var date = new Date(obj);
                var year = date.getFullYear();
                var month = date.getMonth() + 1;
                var day = date.getDate();
                var hour = date.getHours();
                var minute = date.getMinutes();
                var second = date.getSeconds();

                if (displayTime) {

                    if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null) {
                        return "---";
                    } else {
                        if (local == "CN") {
                            return "{0}年{1}月{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit_Time(hour), doubleDigit_Time(minute), doubleDigit_Time(second));
                        } else {
                            return "{0}-{1}-{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit_Time(hour), doubleDigit_Time(minute), doubleDigit_Time(second));
                        }
                    }
                } else {
                    if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null) {
                        return "---";
                    } else {
                        if (local == "CN") { //如果是CN的显示中文年月
                            return "{1}月{2}".formatBy(year, month, doubleDigit(day));
                        } else {
                            return "{0}-{1}-{2}".formatBy(year, doubleDigit(month), doubleDigit(day));
                        }
                    }
                }


            } catch (e) {
                return "";
            }

            function doubleDigit(n) { return n < 10 ? "0" + n : "" + n; }

            function doubleDigit_Time(n) { return n < 10 ? "0" + n : "" + n; }

        }


        function opTransfer(op) {

            var buttonName = "";
            switch (op) {
                case 0:
                    buttonName = "保 存";
                    break;
                case 1:
                    //client.process.na
                    var navInfo = client.process.na.where(function (i) {
                        return i.a == client.process.ra.i;
                    })[0];
                    //debugger;
                    if (client.process.ra.i == client.controls.nav[0].nav.a || (client.process.ra.r != 1 && client.process.ra.r != 2) || navInfo.t == 4 || navInfo.t == 3)
                        buttonName = "提 交";
                    else
                        buttonName = "批 准";
                    break;
                case 2:
                    buttonName = "转 发";
                    break;
                case 3:
                case 4:
                case 5:
                    buttonName = "加 签";
                    break;
                case 6:
                    buttonName = "退 回";
                    break;
                case 7:
                    buttonName = "撤 回";
                    break;
                case 8:
                    buttonName = "抄 送";
                case 9:
                    buttonName = "作 废";
                    break;
            }
            return buttonName;
        }

        function UserSelector(fn, Multiple) {
            function ajaxSetup() {
                // Ajax Setup
                $.ajaxSetup({
                    url: resourcehost + "AjaxPostHandler.ashx",
                    dataType: "json",
                    type: "POST",
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("ajax 返回出错:" + errorThrown.toString());
                    }
                });
            }
            ajaxSetup();
            if (Multiple) {
                this.userWindow({
                    allowMulti: false,
                    onSubmit: function (context, users) {
                        if (users && users.length > 0) {
                            var result = users.select('data').select(function (i) {
                                return {
                                    UserID: i["UserID"] == undefined ? i["Wd_UserID"] : i["UserID"],
                                    LoginName: i["LoginName"],
                                    Name: i["Name"],

                                    DeptName: i["DeptName"] == undefined ? i["OrgName"] : i["DeptName"]
                                };
                            });
                            fn.call(result, context);
                            return true;
                        }
                    }
                });
            } else {
                var setting = {
                    title: "选择人员",
                    users: [],
                    onInit: function (context) {
                        $(context).find("#Dealer").userInput1(setting);
                    },
                    onSubmit: function (context) {
                        if (setting.users.length == 0)
                            return;
                        //setting.users[0];
                        setting.users = setting.users.select(function (i) {
                            return {
                                UserID: i["UserID"] == undefined ? i["Wd_UserID"] : i["UserID"],
                                LoginName: i["LoginName"],
                                Name: i["Name"],

                                DeptName: i["DeptName"] == undefined ? i["OrgName"] : i["DeptName"]
                            };
                        });
                        fn.call(setting.users, context);
                        return true;
                    },
                    sizeMode: "large" //small: 300*200 / standard:800*600 / large: 1024*768"
                };
                setting.win = InitTemplate().tmpl().OpenDiv(setting);
                //this.bind("click", function () {
                //    setting.win = InitTemplate().tmpl().OpenDiv(setting);
                //});
                function InitTemplate() {
                    return $("<script id='tmplSendOutPersonnel' type='text/x-jquery-tmpl'>" + controlTemplate.userSelector + "</script>");
                }
            }
        }

        wanda_wf.UserSelector = UserSelector;
        function initData() {
            client.process = JSON.parse($("#" + wanda_wf.settings.ProcessDataKey).val());
            client.process.na = JSON.parse(client.process.na);
            client.process.na.foreach(function (n, index) {
                n.c = JSON.parse(n.c);
                n.o = JSON.parse(n.o);
            });
            client.process.o = JSON.parse(client.process.o);
            //干掉转发
            client.process.o.remove(2);
            client.process.o.remove(7);
            //不要加签
            client.process.o.remove(3);
            client.process.o.remove(4);
            client.process.o.remove(5);
        }

        function initNav() {
            function selectuser() {
                var srcElem = this;
                UserSelector(function (users) {
                    srcElem.options.length = 0;
                    this.foreach(function (i) {
                        srcElem.options.add(new Option(i.Name, i.UserID));
                    });
                    $(srcElem).unbind('click');
                })
            }
            var navhost = $("#" + wanda_wf.settings.navhost);

           
            var navobj = new Array();
            function navItem(simple) {
                var obj = {
                    nav: simple.nav,
                    id: simple.id,
                    name: simple.name,
                    dom: $("<span class='workflow_span wanda-wf-float'></span>"),
                    users: simple.users,
                    split: $("<img style='display:block;float:left; padding-top:2px; margin-left:4px;margin-right:8px;' src=\"" + wanda_wf.settings.resoucehost + "Images/arrow.png\" alt=''/>"),//$("<img style=' margin-left:4px;margin-right:8px; height:13px;width:15px;' src=\"../images/arrowright16.png\" alt='' />"),//$("<span style=' margin-left:4px;margin-right:8px;font-size:14px; font-weight:bold;color:green;' >→</span>"),//$("<img style=' margin-left:4px;margin-right:8px;' src=\"" + wanda_wf.settings.resoucehost + "Images/flow.png\" alt='' />"),
                    acttype: simple.acttype
                }
                obj.appendto = function (split) {
                    this.dom.appendTo(navhost);
                    this.split.appendTo(navhost);
                };
                obj.ondelete = function () {

                }
                obj.render = function () {
                    if (obj.nav.t == 3 || obj.nav.t == 4) {
                        var this_index = navobj.indexOf(obj);
                        if (this_index + 1 < navobj.length) {
                            var next = navobj[this_index + 1].nav.t;
                            if (next == 3 || next == 4) {
                                obj.split = $("<span style='float:left';>,</span>");
                            }
                        }
                    }
                    var resource;
                    if (this.nav.r == 1 || this.nav.r == 2 || this.nav.r == 4) {
                        this.dom = $("<span class='workflow_span wanda-wf-float' style='font-weight:600;'></span>");
                    }
                    //this.dom.addClass('nav-item');
                    function oneResource() {
                        if (this.users.length == 1 || this.acttype == 1) {
                            if (this.users.length == 0) return;
                            resource = $(this.dom[0].outerHTML).removeClass('wanda-wf-float').text('【' + this.users[0].n + '】');

                        }
                    }
                    function moreResouce() {
                        if (this.users.length > 1) {
                            if (this.nav.r != 0 || this.acttype != 1) {
                                var text = this.users.select(function (u) {
                                    if (u.c && (obj.nav.r == 1 || obj.nav.r == 2))
                                        return u.n + "<img style='padding-top:1px;' src=\"" + wanda_wf.settings.resoucehost + "Images/ok.png\" alt=''></img>";
                                    else
                                        return u.n;
                                }).join(',');
                                resource = $(this.dom[0].outerHTML).removeClass('wanda-wf-float');
                                if (text != '')
                                    resource.html("【" + text + "】");
                            }
                        }
                    }
                    if (this.nav.r == 0) {//not nun
                        oneResource.call(this, arguments);
                        moreResouce.call(this, arguments);
                        unRunNoOrMoreResource.call(this, arguments);
                        function unRunNoOrMoreResource() {
                            if (this.users.length != 1 && this.acttype == 1) {
                                if (client.process.readonly) {
                                    var text = this.users.select('n').join(',');

                                    resource = $(this.dom[0].outerHTML).removeClass('wanda-wf-float');
                                    if (text != '')
                                        resource.text("【" + text + "】");
                                } else {
                                    resource = $("<select style='margin-left:4px;width:auto;'><option value=''>请选择</option></select>");
                                    if (this.users.length == 0) {
                                        resource.click(function () {
					    isCc = false;
                                            selectuser.call(this, arguments);
                                        });
                                    } else {
                                        resource[0].options.length = 0;
                                        this.users.foreach(function (u) {
                                            resource[0].options.add(new Option(u.n, u.i));
                                        });
                                    }
                                }
                            }
                        }
                    } else {
                        oneResource.call(this, arguments);
                        moreResouce.call(this, arguments);
                    }
                    this.dom.text(this.name);
                    this.dom.append(resource);

                    if (this.clientadded) {
                        var obj_ref = this;
                        //var delbtn = $("<img style='cursor:pointer;' alt='删除'></img>")
                        var delbtn = $("<span style='cursor:pointer; color:red;'>X</span>")
                            .data("wf_obj", this)
                            .click(function () {
                                obj.ondelete();
                            });
                        this.dom.append(delbtn);
                    }
                    this.appendto();
                    //if (this.users.length < 2)
                    //    this.name = this.dom.text();
                    if (navobj.indexOf(obj) == navobj.length - 1) {
                        this.split.hide();
                    }
                    if (this.nav.r == 3 || this.nav.r == 4) {
                        this.dom.append($("<img style='display:block;float:right; padding-top:1px;' src=\"" + wanda_wf.settings.resoucehost + "Images/ok.png\" alt=''></img>"));
                        //this.dom.append($("<span style='color:green; font-weight:900'>√</span>"));
                    }
                }
                return obj;
            };
            client.process.na.foreach(function (n, index) {
                if (n.t == 4) return;
                var item = {
                    nav: n,
                    id: n.a,
                    name: n.n,
                    users: n.c,
                    acttype: n.t
                };
                item = navItem(item);
                //item.render();
                navobj.push(item);
            });
            navobj.render = function () {
                navobj.foreach(function (i) {
                    setTimeout(function () {
                        i.render();
                    }, 5);
                });
            }();
            navobj.wf_insert = function (hostid, user, type, addContent) {
                var index = 0;
                for (var i = 0; i < this.length; i++) {
                    if (this[i].id == hostid) {
                        index = i;
                        break;
                    }
                }
                if (type == 3) {
                    index = (index - 1) < 0 ? 0 : (index - 1);
                }
                var item = {
                    nav: { t: type == 5 ? 2 : 1 },
                    id: navobj.watch,
                    users: user,
                    name: type == 5 ? '会签' : '',
                    acttype: type == 5 ? 2 : 1 //如果是会签,否则就是审批
                }
                navobj.watch++;
                item = navItem(item);
                var Actname = "";
                if (client.process.ra.i == client.controls.nav[0].nav.a) {
                    Actname = "发起人加";
                } else {
                    Actname = client.process.CurrentUser + "加";
                }
                if (type == 5)
                    Actname += "会签"
                else
                    Actname += "签";
                item.name = Actname;
                item.AddContent = addContent;
                item.t = type == 5 ? 2 : 1;
                item.addtype = type;
                item.clientadded = true;
                item.hostid = hostid;
                var hostobj = navobj[index];
                item.appendto = function () {
                    hostobj.split.show();
                    this.split.insertAfter(hostobj.split);
                    this.dom.insertAfter(hostobj.split);
                };
                item.remove = function () {
                    setTimeout(function (obj) {
                        item.split.remove();
                        item.dom[0].innerHTML = "";
                        item.dom.remove();
                        delete item;
                    }, 10, this);
                };
                item.ondelete = function () {
                    navobj.where(function (i) {
                        return i.clientadded == true && i.hostid == item.id;
                    }).foreach(function (i) { i.hostid = item.hostid });
                    navobj.splice(navobj.indexOf(item), 1);
                    item.remove();
                    reSort();
                };
                navobj.splice(index + 1, 0, item);
                item.render();
                function reSort() {
                    var preNav = navobj[0];
                    //处理前加签                    
                    for (var i = 1; i < navobj.length; i++) {
                        if (navobj[i].clientadded == true) {
                            if (navobj[i].addtype == 3)
                                continue;
                            navobj[i].hostid = preNav.id;
                        }
                        preNav = navobj[i];
                    }
                    //后加签
                    preNav = navobj[navobj.length - 1];
                    for (var i = navobj.length - 1; i >= 0; i--) {
                        if (navobj[i].clientadded == true) {
                            if (navobj[i].addtype == 3)
                                navobj[i].hostid = preNav.id;
                        }
                        preNav = navobj[i];
                    }
                }
                reSort();
            }
            navobj.watch = navobj.length;

          //  var aa = WebUtil.jsonToString(navobj);

            return navobj;
        }

        function InitCcControl() {
            /// <summary>处理抄送列表</summary>
            var Cc = {
                dom: null
            };
            var ccControl = $("#wanda_wf_ccRange").html("");
            client.process.p.cc = JSON.parse(client.process.p.cc);
            function ccTransfer() {
                return client.process.p.cc.select(function (i) {
                    return { UserID: i.i, LoginName: i.l, Name: i.n, DeptName: i.d };
                });
            }
            Cc.AppendUsers = function (row, showDel, dom) {
                var item = $("<span></span>");
                item.text(row.Name).attr("title", row.LoginName);
                var split = $("<span>;<span>");
                if (showDel == true) {
                    var del = $("<span style='color:red;cursor:pointer;'> X </span>").click(function () {
                        //var self = $(this);
                        setTimeout(function () {
                            Cc.Users.remove(row);
                            item.remove();
                            split.remove();
                            del.remove();
                        }, 10)
                    })
                    item.insertBefore(dom);
                    del.insertBefore(dom);
                    split.insertBefore(dom);
                    item.parent().find('div').remove();
                } else {
                    item.appendTo(Cc.bar);
                    split.appendTo(Cc.bar);
                }
            };
            Cc.bar = ccControl;
            Cc.Users = [];
            var firstAct = client.process.na[0];
            if (firstAct != undefined && firstAct.r == 1 && firstAct.a == client.process.ra.i) {
                showCcUser();
                ccTransfer().foreach(function (i) {
		    isCc = true;
                    Cc.Users.push(i);
                    Cc.AppendUsers(i, true, Cc.dom);
                });

            } else {
                var hasCc = false;
                ccTransfer().foreach(function (i) {
                    hasCc = true;
                    Cc.AppendUsers(i, false, null);
                });
                if (!hasCc)
                    ccControl.html('无');
            }
            function showCcUser() {
                var userSelector = $("<a href='#' onclick='onclickCc()' style='color:#3776f7;cursor: pointer;'>请选择</a>")
                UserSelector.call(userSelector, function (context) {
                    this.foreach(function (row) {
                        Cc.Users.push(row);
                        Cc.AppendUsers(row, true, userSelector);
                    });
                }, true);
                Cc.dom = userSelector;
                ccControl.append(userSelector);
            }
            ccControl.find('div').remove();
            return Cc;
        }

        function InitButton() {
            var buttonhost = $("#" + wanda_wf.settings.buttonHost);
            if (buttonhost.length == 0)
                return;
            if (client.process.o.length == 0) {
                buttonhost.hide();
                return;
            }
            function initUIbtnList() {
                client.process.o = client.process.o.sort();
                var obj = new Object();
                client.process.o.foreach(function (i) {
                    obj[opTransfer(i)] = i;
                });
                return obj;
            }
            var thtml = "";
            var html = "<table style='border:none;width:100%;'><tr>";
            html += "<td style='text-align:center;vertical-align:middle;'>&nbsp;</td>";
            var uibindObj = initUIbtnList();
            
            for (var i in uibindObj) {
                if (typeof (uibindObj[i]) == 'number') {
                    if (uibindObj[i] == 0) continue;
                    if (uibindObj[i] == 1 || uibindObj[i] == 6) {
                        html += "<td style='width:100px; height:40px;text-align:center;vertical-align:middle;'><div class='workflow_button_div filed' style='font-size:18px; padding-bottom:3px;line-height:35px;' opType='" + uibindObj[i] + "'>" + i + "</div></td>";
                    }
                    else {
                        thtml += "<td style='width:100px; height:40px;text-align:center;vertical-align:middle;'><div class='workflow_button_div filed' style='font-size:18px; padding-bottom:3px;line-height:35px;' opType='" + uibindObj[i] + "'>" + i + "</div></td>";

                    }
                }
            }
            html += thtml;
            html += "<td style=\"text-align:center;vertical-align:middle;\">&nbsp;</td>";
            html += "</tr></table>";
            buttonhost.html(html);
            setTimeout(function () {
                var mask = null;
                function setdisable(disable) {
                    setTimeout(function () {
                        $('.workflow_button_div').each(function (i, item) {
                            item.disabled = disable;
                        });
                        var doc = $(document);
                        if (disable) {
                            if (mask != null)
                                mask.height(doc.height()).width(doc.width()).show();
                            else {
                                var loader = $("<div class='loader'></div>");
                                mask = $("<div class='mask-div'></div> ").height(doc.height()).width(doc.width()).appendTo(document.body);
                                loader.appendTo(mask);
                            }
                        } else {
                            if (mask != null)
                                mask.hide();
                        }
                    }, 10);
                }
                $('.workflow_button_div').click(function () {
		    isCc = false;
                    var op = $(this).attr("opType");
                    if (isNaN(op))
                        return;
                    op = parseInt(op);
                    if (op == 3)
                        if (!confirm('确定要作废表单?')) return;
                    //添加提交时对选人部分验证是否有人
                    if (op == 1) {
                        var selects = document.getElementById("workflowtd").getElementsByTagName("SELECT");
                        for (var i = 0; i < selects.length; i++) {

                            if (selects[i].value == "") {

                                alert("请选择【" + selects[i].parentElement.childNodes['0'].nodeValue + "】节点候选人");
                                return;
                            }
                        }

                    }
                    if (op == 6) {
                        if (!confirm('确定要退回流程?')) return;
                    }
                    clientclick.call(this, op, submitOperation);
                });
            }, 5);
            function submitOperation() {
                /// <summary>提交</summary>
                wanda_wf.settings.submitFun();
            }
        }

        function initOpin() {
            var helper = $("#" + wanda_wf.settings.opinContentHelper);
            var currentoOpin = $("#" + wanda_wf.settings.currentOpinhostID);
            helper.hide();
            currentoOpin.parent().parent().parent().parent().parent().hide();
            var host = document.getElementById(wanda_wf.settings.opinContainer);
            var opinList = new Array();
            
            function attachCurrentOpin(item) {
                helper.change(function () {
                    if (this.value == '-1')
                        return;
                    //var value = currentoOpin.val();
                    //if (value.trim() != '') {
                    //    value += "\n";
                    //}
                    //value += helper[0].options[helper[0].selectedIndex].text;
                    //currentoOpin.val(value);
                    currentoOpin.val(helper[0].options[helper[0].selectedIndex].text);
                }).show();
                currentoOpin.parent().parent().parent().parent().parent().show();
                currentoOpin.show();
                $("#" + wanda_wf.settings.currentOpinhostID).text(decodeURI(item.c));
                //if (currentoOpin.val() == "") {
                //    currentoOpin.val(helper[0].options[helper[0].selectedIndex].text);
                //}
            }
            function transferItem(src) {
                var obj = {
                    obj: src.obj,
                    dom: $("<tr></tr>"),
                    actid: src.actid,
                    appendTo: function () {
                        if (host != null) {
                            setTimeout(function (arg) {

                                obj.dom.appendTo(host);

                            }, 5, this);
                        }
                    },
                    render: function () {
                        var columns = [
                            function (obj) { return obj['an'] == '' ? '审批' : obj['an']; },
                            function (obj) { return decodeURI(obj['c']).replace(/\r/ig, "").replace(/\n/ig, "<br />");; },
                            'u',
                            function (obj) { return ChangeDateFormat(obj['d']); },
                            function (obj) { return obj["r"]; }
                        ];
                        for (var index = 0; index < columns.length; index++) {
                            var fn = columns[index];
                            if (typeof (columns[index]) == "string") {
                                fn = function (obj) { return obj[columns[index]]; };
                            }
                            var container = $("<td class='table_td'></td>");
                            var inner = $("<div class='table_td_div'  style='text-align:center;font-weight:normal'></div>").text(fn(this.obj));
                            if (index == 0 || index == 3 || index == 2) {
                                container.css("min-width", "150px");
                            } else if (index == 1) {
                                //inner = $("<textarea class='readonly' style='width:98%;text-align:left;padding-top:15px;' disabled='disabled' ></textarea>")
				            var inner = $("<div style='text-align:left;font-weight:normal;font-size: 12px;color: gray;'></div>").text(fn(this.obj));
                                container.css("min-width", "250px");
                            } else if (index == 4) {
                                container.css("min-width", "100px");
                            }
                            inner.html(fn(this.obj));
                            inner.appendTo(container);
                            container.appendTo(this.dom);
                            this.appendTo();
                        }
                    }
                };
                return obj;
            }
            var showPanel = false;
            for (var index = client.process.na.length - 1; index >= 0; index--) {
                //client.process.na[index].o = JSON.parse(p.na[index].o);
                client.process.na[index].o.foreach(function (i) {
                    if (i.ce) {
                        attachCurrentOpin(i);
                        return;
                    }
                    showPanel = true;
                    var item = {
                        obj: i,
                        actid:null
                            //client.process.na[index].a
                    };
                    item = transferItem(item);
                    opinList.push(item);
                });
            }

            ///这是新增的  Update by  2015-8-18

            var Request = new Object();
            Request = GetRequest();
            var _ProType = Request["ProType"];
            var _bid = Request["BusinessID"];

           // _bid = "38662280-7009-4964-B554-EC9189ECC498";

            //这是在流程合并后的标记
            if (_ProType != null && _ProType != undefined && _ProType == "Batch") {
                //通过标记获取批次的实体
                WebUtil.ajax({
                    async: false,
                    url: "/TargetReportedControll/GetSystemBatchByID",
                    args: { BusinessID: _bid },
                    successReturn: function (result) {
                        var BatchData = {};
                        BatchData = eval(result.Opinions);
                        if (BatchData != null)
                        {
                            //首先先排序
                            BatchData = JsonSort(BatchData, 'd');
                            for (var i = 0; i < BatchData.length; i++) {

                                //将时间类型的格式转换
                                var da = BatchData[i].obj.d;
                                var daStr = da.replace("/", "-").replace("/", "-");
                                BatchData[i].obj.d = daStr;
                                opinList.push(transferItem(BatchData[i]));
                            }
                                
                            JsonSort(opinList, 'd');
                            var aa = opinList;

                        }
                    }
                });
            } else { //没有批次的流程，同时对经营系统可能有所干涉
                //通过业务ID，获取批次的实体


                var addUrl = location.pathname;


                if (addUrl.indexOf('TargetProReported.aspx') >= 0 || addUrl.indexOf('ProTargetApprove.aspx') >= 0)
                {
                    if (_BusinessID != null && _BusinessID != undefined && _bid == undefined)
                    {
                        _bid = _BusinessID;
                    }

                    WebUtil.ajax({
                        async: false,
                        url: "/TargetReportedControll/GetSysBatchByMonthReportID",
                        args: { BusinessID: _bid },
                        successReturn: function (result) {
                            var BatchData = {};
                            BatchData = eval(result.Batch_Opinions);

                            if (BatchData != null)
                            {
                                //首先先排序
                                BatchData = JsonSort(BatchData, 'd');
                                for (var i = 0; i < BatchData.length; i++) {

                                   //将时间类型的格式转换
                                    var da = BatchData[i].obj.d;
                                    var daStr= da.replace("/", "-").replace("/", "-");
                                    BatchData[i].obj.d = daStr;

                                    opinList.push(transferItem(BatchData[i]));
                                }
                            }
                        }
                    });

                }
            }


            //排序---Update by 2015-8-18 单独写的排序
            function JsonSort(json, key) {
                
                //var dataY = y.obj.d.replace(new RegExp("-", "gm"), "").replace(new RegExp(":", "gm"), "").replace(new RegExp(" ", "gm"), "");

                //var dataX = x.obj.d.replace(new RegExp("-", "gm"), "").replace(new RegExp(":", "gm"), "").replace(new RegExp(" ", "gm"), "");

                for (var j = 1, jl = json.length; j < jl; j++) {
                    var temp = json[j],
                        val = temp.obj.d.replace(new RegExp("/", "gm"), "").replace(new RegExp("-", "gm"), "").replace(new RegExp(":", "gm"), "").replace(new RegExp(" ", "gm"), ""),
                        i = j - 1;
                    while (i >= 0 && json[i].obj.d.replace(new RegExp("/", "gm"), "").replace(new RegExp("-", "gm"), "").replace(new RegExp(":", "gm"), "").replace(new RegExp(" ", "gm"), "") < val) {
                        json[i + 1] = json[i];
                        i = i - 1;
                    }
                    json[i + 1] = temp;
                }
                return json;
            }


            //格式化日期类型
            function WF_FormatDate(obj, displayTime, local) {
                try {
                    var date = new Date(obj);
                    var year = date.getFullYear();
                    var month = date.getMonth() + 1;
                    var day = date.getDate();
                    var hour = date.getHours();
                    var minute = date.getMinutes();
                    var second = date.getSeconds();

                    if (displayTime) {

                        if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null) {
                            return "---";
                        } else {
                            if (local == "CN") {
                                return "{0}年{1}月{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit_Time(hour), doubleDigit_Time(minute), doubleDigit_Time(second));
                            } else {
                                return "{0}-{1}-{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit_Time(hour), doubleDigit_Time(minute), doubleDigit_Time(second));
                            }
                        }
                    } else {
                        if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null) {
                            return "---";
                        } else {
                            if (local == "CN") { //如果是CN的显示中文年月
                                return "{1}月{2}".formatBy(year, month, doubleDigit(day));
                            } else {
                                return "{0}-{1}-{2}".formatBy(year, doubleDigit(month), doubleDigit(day));
                            }
                        }
                    }


                } catch (e) {
                    return "";
                }

                function doubleDigit(n) { return n < 10 ? "0" + n : "" + n; }

                function doubleDigit_Time(n) { return n < 10 ? "0" + n : "" + n; }

            }


            //对json日期字段转化,Update by 2015-8-18
            opinList.foreach(function (i) {

                if (i.obj.d.indexOf("Date") >= 0)
                {
                    //将时间转换成 yyyy-MM-dd hh:mm:ss
                    var re = /-?\d+/;
                    var m = re.exec(i.obj.d);
                    var d = new Date(parseInt(m[0]));
                    var ti = WF_FormatDate(d, true, "");
                    i.obj.d = ti;
                    //i.obj.d = new Date(parseInt(i.obj.d.replace("/Date(", "").replace(")/", ""), 10));
                } 
            });

            //排序
            opinList.sort(function (x, y) {

                var dataY = y.obj.d.replace(new RegExp("-", "gm"), "").replace(new RegExp(":", "gm"), "").replace(new RegExp(" ", "gm"), "");
                var dataX = x.obj.d.replace(new RegExp("-", "gm"), "").replace(new RegExp(":", "gm"), "").replace(new RegExp(" ", "gm"), "");
                var aa =dataY -dataX;
                return aa;
            });

            opinList.foreach(function (i) {
                setTimeout(function () {
                    i.render();
                }, 15);
            });

            opinList.appendItem = function (content, op) {
                var first = this[0];
                var item = {
                    obj: {
                        c: decodeURI(content),
                        an: p.ra.i,
                        user: p.cu,
                        d: new Date(),
                        r: op
                    },
                    actid: first.actid
                }
                item = transferItem(item);
                item.appendTo = function () {
                    this.dom.insertBefore(first.dom);
                }
                item.cliendadd = true;
                item.render();
            }
            if (!showPanel) {
                setTimeout(function () {
                    $('.wanda-wf-approval').hide();
                }, 15);
            }

            return opinList;
        }

        function clientclick(op, nextfn) {
            var args = { operationType: op, cancel: false };
            var result;
            if (wanda_wf.settings.referesh) {
                alert('流程已改变,需刷新流程');
                args.operationType = 0;
            }
            //debugger;
            switch (args.operationType) {
                case 2://转发
                    Forward.call(this, nextfn);
                    return false;
                case 9://作废                        
                case 7://撤回,                        
                case 0://保存
                case 1://发送
                case 6://退回
                    var result;
                    if (args.operationType != 0) {
                        result = wanda_wf.events.onsummit.call(args);
                        args.operationType = op;
                    }
                    result = initOperationInfo.call(this, args);
                    break;
                case 3://前加签
                case 4://后加签
                case 5://会签                    
                    result = addactivity.call(this);
                    return false;
                default:
                    return false;
            }
            result = result == undefined ? true :
                        (typeof (result) == "boolean" ? result : false);
            args.cancel = !result;
            if (args.cancel == false) {
                MaskWinSubmit();
                nextfn();
            }
            //return args.cancel;
            function Forward(fn) {
                var self = this;
                var setting = {
                    users: [],
                    title: "转发",
                    onInit: function (context) {
                        var addcontrol_user = context.find("#wanda_wf_transferSelecor");
                        UserSelector.call(addcontrol_user, function (context) {
                            this.foreach(function (row) {
                                setting.users.push(row);
                                var item = $("<span></span>").text(row.Name).attr("title", row.LoginName);
                                var split = $("<span>;<span>");
                                var del = $("<span style='color:red;cursor:pointer;'> X </span>").click(function () {
                                    setTimeout(function () {
                                        setting.users.remove(row);
                                        item.remove();
                                        split.remove();
                                        del.remove();
                                    }, 10)
                                })
                                item.insertBefore(addcontrol_user);
                                del.insertBefore(addcontrol_user);
                                split.insertBefore(addcontrol_user);
                                item.parent().find('div').remove();
                            });
                        }, true);
                    },
                    onSubmit: function (context) {
                        if (setting.users.length == 0) {
                            alert('请选择接收人');
                            return false;
                        }
                        var ForwardContent = context.find("#wanda_wf_transferContent").val();
                        if (ForwardContent.trim() == '') {
                            alert('请填写转发意见');
                            return false;
                        }
                        setTimeout(function () {
                            args.ForwardContent = ForwardContent;
                            args.ForwardUsers = setting.users.select('LoginName');
                            initOperationInfo.call(self, args);
                            MaskWinSubmit();
                            fn();
                        }, 10);
                        return true;
                    },
                    sizeMode: "large"
                };
                $(wanda_wf.settings.controlTemplate.Forward).tmpl().OpenDiv(setting);
            }
            function addactivity() {
                wanda_wf.events.onbeforeaddactivity.call(this, arguments);
                showaddactool.call(this);
                wanda_wf.events.onafteraddactivity.call(this, arguments);
                return false;
                function showaddactool() {
                    var setting = {
                        controls: {
                            add_panel: null,
                            act_sel: null,
                            wanda_wf_addactUsers: null,
                            users: []
                        },
                        title: "加签",
                        onInit: function (context) {
                            //set the controls hand
                            if (client.process.ra.i == client.controls.nav[0].id) {
                                context.find("#wanda_wf_addActContent").parent().parent().hide();
                            } else {
                                context.find("#wanda_wf_addActContent").show;
                            }
                            setting.controls.act_sel = context.find("#wanda_wf_avtlist")[0];
                            setting.controls.add_panel = context.find("#wanda_wf_addtype")[0];
                            setting.controls.wanda_wf_addactUsers = $("#wanda_wf_addactUsers");
                            context.find("#wanda_wf_addact_userSelector").unbind("click");
                            var addcontrol_user = context.find("#wanda_wf_addact_userSelector");
                            UserSelector.call(addcontrol_user, function (context) {
                                this.foreach(function (row) {
                                    setting.controls.users.push(row);
                                    var item = $("<span></span>").text(row.Name).attr("title", row.LoginName);
                                    var split = $("<span>;<span>");
                                    var del = $("<span style='color:red;cursor:pointer;'> X </span>").click(function () {
                                        setTimeout(function () {
                                            setting.controls.users.remove(row);
                                            item.remove();
                                            split.remove();
                                            del.remove();
                                        }, 10)
                                    })
                                    item.insertBefore(addcontrol_user);
                                    del.insertBefore(addcontrol_user);
                                    split.insertBefore(addcontrol_user);
                                    item.parent().find('div').remove();
                                });
                            }, true);

                            setting.controls.act_sel.options.length = 0;
                            var wanda_wf_relation = context.find("#wanda_wf_relation")[0];
                            client.controls.nav.where(function (i) {
                                if (client.process.ra.i != client.controls.nav[0].id) {
                                    return i.id == client.process.ra.i;
                                }
                                return i.nav.r != 3;
                            }).foreach(function (i, index) {
                                var name = i.dom.text();
                                if (i.nav.t == '2' || i.nav.t == '3') {
                                    name = i.name;
                                } else {
                                    if (i.users.length > 1)
                                        name = i.name;
                                }
                                if (name.indexOf('X') != -1) {
                                    name = name.replace("X", '');
                                }
                                setting.controls.act_sel.options.add(new Option(name, i.id));
                            });
                            function actChange() {
                                for (var i = 0; i < client.controls.nav.length; i++) {
                                    if (client.controls.nav[i].id == $(this).val()) {
                                        wanda_wf_relation.innerHTML = "";
                                        setting.controls.add_panel = context.find("#wanda_wf_addtype")[0];
                                        var relation = getRelationList(client.controls.nav[i]);
                                        if (relation.length > 0) {
                                            var dom = $("<input type='radio' name='wanda_wf_relation_radio'/>");
                                            var first_dom = null;
                                            relation.foreach(function (ri, ri_index) {

                                                var id = wanda_wf_relation + "_" + ri.value;
                                                var dom_clone = dom.clone().val(ri.value).attr("id", id).appendTo(wanda_wf_relation).change(function () {
                                                    if (this.checked) {
                                                        bindaddtype.call(this);
                                                    }
                                                });
                                                var lab = $("<label></label>").text(ri.text).attr("for", id).appendTo(wanda_wf_relation);
                                                if (ri_index == 0)
                                                    first_dom = dom_clone;
                                            });
                                            if (first_dom != null) {
                                                first_dom.click();
                                                first_dom.change();
                                            }
                                        }
                                    }
                                }
                            }
                            $(setting.controls.act_sel).change(actChange).change();

                            setting.controls.wanda_wf_addactUsers.find("input").val('');
                            function bindaddtype() { //绑定审批类型
                                setting.controls.add_panel.innerHTML = "";
                                var source = getbindData.call(this);
                                if (client.process.ra.i == client.controls.nav[0].id) {
                                    context.find("#wanda_wf_addActContent").parent().parent().hide();
                                } else {
                                    if ($(this).val() == '1')
                                        context.find("#wanda_wf_addActContent").parent().parent().hide();
                                    else
                                        context.find("#wanda_wf_addActContent").parent().parent().show();
                                }
                                if (source.length > 0) {
                                    source.foreach(function (i, index) {
                                        var id = "wanda_wf_addtype" + "_" + i.value;
                                        var radioItem = $('<input type="radio" id="' + id +
                                            '" name="wanda_wf_addtype" value="' + i.value +
                                            '"/><label for="' + id + '">' + i.text +
                                            '</label>').appendTo(setting.controls.add_panel);
                                        if (index == 0)
                                            radioItem[0].checked = true;
                                    });
                                }
                                function getbindData() {
                                    var result = new Array();
                                    if (this.value == "0")
                                        result = client.process.o.where(function (i) {
                                            return i == 3;
                                        });
                                    else
                                        result = client.process.o.where(function (i) {
                                            return [4, 5].contains(i);
                                        });
                                    return result.select(function (i) {
                                        return { text: transferaddact(i), value: i };
                                    });
                                }
                            }
                        },
                        onSubmit: function (context) {
                            if (setting.controls.users.length == 0) {
                                alert('请选择处理人');
                                return false;
                            }
                            var hostid = setting.controls.act_sel.options[setting.controls.act_sel.selectedIndex].value;
                            var addtype;
                            $("input[name='wanda_wf_addtype']").each(function (i, item) {
                                if (item.checked)
                                    addtype = item.value;
                            });
                            var addContent = context.find("#wanda_wf_addActContent").val();
                            var submitInfo = false;
                            if (client.process.ra.i != client.controls.nav[0].id && addtype == 3) {
                                if (addContent.trim() == '') {
                                    alert('请填写加签意见');
                                    return false;
                                }
                                if (window.confirm('流程将提交至' + setting.controls.users[0].Name + "\n确定提交?")) {
                                    args.operationType = 3;
                                    submitInfo = true;
                                } else {
                                    return true;
                                }
                            }
                            var users = setting.controls.users.select(function (u) {
                                return {
                                    i: u.UserID,
                                    n: u.Name,
                                    loginName: u.LoginName,
                                    t: u.DeptName
                                };
                            });
                            if (addtype == 5) {
                                client.controls.nav.wf_insert(hostid, users, addtype, addContent);
                            }
                            else {
                                var virtualobj = null;
                                if (addtype == 3) {
                                    for (var ui = 0; ui < users.length; ui++) {
                                        if (virtualobj != null)
                                            hostid = virtualobj.id;
                                        virtualobj = client.controls.nav.wf_insert(hostid, [users[ui]], addtype, addContent);
                                    }
                                }
                                else {
                                    for (var ui = users.length - 1; ui >= 0; ui--) {
                                        if (virtualobj != null)
                                            hostid = virtualobj.id;
                                        virtualobj = client.controls.nav.wf_insert(hostid, [users[ui]], addtype, addContent);
                                    }
                                }
                            }
                            if (submitInfo)
                                setTimeout(function () {
                                    initOperationInfo.call(self, args);
                                    MaskWinSubmit();
                                    nextfn();
                                }, 25);
                            return true;
                        },
                        sizeMode: "large" //small: 300*200 / standard:800*600 / large: 1024*768"
                    };
                    $(wanda_wf.settings.controlTemplate.ActivityAdd).tmpl().OpenDiv(setting);
                    //$("#addmask").show();
                }//sel.options.add
                function getaddtypelist() {
                    return p.o.where(function (i) {
                        if (i == 3 || i == 4 || i == 5) {
                            return true;
                        }
                    });
                }
                function transferaddact(t) {
                    switch (t) {
                        case 3: return "顺序审批";
                        case 4: return "顺序审批";
                        case 5: return "同时审批";
                        default:
                    }
                }
                function getRelationList(selectedObj) {
                    var result = new Array();
                    if (client.process.o.contains(4) || p.o.contains(5))
                        result.push({ value: 1, text: '之后' });
                    if (client.process.o.contains(3)) {
                        if (selectedObj.acttype == 0)
                            return result;
                        if (selectedObj.acttype != 2 && (selectedObj.nav.r != 1 || selectedObj.nav.r != 2))
                            result.push({ value: 0, text: '之前' });
                        //if (selectedObj.acttype != 2 && selectedObj.nav.r != 0)
                    }
                    return result;
                }
            }
            function initOperationInfo(args) {
		//debugger;
                /// <summary>准备Operation信息参数</summary>
                var opInfo = $("#" + wanda_wf.settings.PostDataKey);
                var obj = { OperationType: args.operationType };
                var content = $("#" + wanda_wf.settings.currentOpinhostID);
                if (content.length == 0) return false;
	
                obj.OpinContent = content.val();
                if (obj.OperationType == 1) {
                    if (obj.OpinContent.trim().length == 0) {
                        alert('请填写意见');
                        return false;
                    }
                }
                if (opInfo.length == 0) {

                    opInfo = $("<input type='hidden'/>")
                        .attr("id", wanda_wf.settings.PostDataKey)
                        .attr("name", wanda_wf.settings.PostDataKey).insertBefore($("#" + wanda_wf.settings.ProcessDataKey));
                }
                if ([0, 2, 1, 3, 6].contains(obj.OperationType)) {
                    var resouceSeleced = new Array();
                    client.controls.nav.foreach(function (i) {
                        var selectControl = i.dom.find("select");
                        if ((i.cliendadd == undefined || i.cliendadd == false) && selectControl.length == 1) {
                            if (selectControl.val() == '') return;
                            resouceSeleced.push({ ActivityID: i.id, UserID: selectControl.val() });
                        }
                    });
                    obj.CCCandidates = client.controls.Cc.Users.select('UserID');//抄送人             
                    obj.ActivityResource = resouceSeleced;
                    obj.ForwardUsers = args.ForwardUsers;
                    obj.ForwardContent = args.ForwardContent;
                    obj.AddInfo = client.controls.nav.where(function (i) {
                        return i.clientadded == true;
                    }).select(function (i) {
                        var name = i.name;
                        if (name.indexOf('X') != -1) {
                            name = name.replace("X", '');
                        }
                        return {
                            ClientID: i.id,
                            HostActivityID: i.hostid,
                            OperationType: i.addtype,
                            Users: i.users.select('loginName'),
                            Name: name,
                            AddContent: i.AddContent
                        };
                    });
                    //if (obj.AddInfo.length > 0) {
                    //    var start = obj.AddInfo[0];

                    //}                        
                }
                //debugger;
                opInfo.val(JSON.stringify(obj));
                return true;
            }
        }

        function bindSelect(elem, source) {
            elem.options.length = 0;
            source.foreach(function (i) {
                elem.options.add(new Option(i.text, i.value));
            });
        }

        function initControls() {
            $("#wanda_wf_nav_opin_content").html(wanda_wf.settings.template());
            setTimeout(function () {
                client.controls.nav = initNav();
                client.controls.Cc = InitCcControl();
                InitButton();
                client.controls.Opin = initOpin();
            }, 10);
        };


        //$("#OpinionTable").html("");
    }
};
function onclickCc() {
    isCc = true;
}
if (!wanda_wf.settings.disableReadyRun)
    wanda_wf.init();


function GetRequest() {

    var url = location.search; //获取url中"?"符后的字串
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = (strs[i].split("=")[1]);
        }
    }
    return theRequest;
}