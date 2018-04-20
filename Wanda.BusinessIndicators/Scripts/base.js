




//-----------------------------------------------------------一上都是原来的，升级2.0版本的下面----------------------------------------------------------------------------------------

// 注意， 在给html 只读标记绑定值的时候， 一定要做此转换
function EC(str) { return $.type(str) == "string" ? WebUtil.htmlEncode(str) : str; }

var CommonUtil = {
    // Not Available
    isNA: function (obj) {
        if (typeof (obj) == "undefined") { return true; }
        if (typeof (obj) == "string" && obj === "") { return true; }
        if (obj == null) { return true; }
        return false;
    },
    clone: function (obj) {
        if (typeof (obj) != 'object')
            return obj;

        var re = {};
        if (obj.constructor == Array)
            re = [];

        for (var i in obj) {
            re[i] = util.clone(obj[i]);
        }

        return re;

    },
    numberLimit: function () {
        var k = window.event.keyCode;

        if ((k == 46) || (k == 8) || (k == 9) || (k == 189) || (k == 109)
            || (k == 190) || (k == 110) || (k >= 48 && k <= 57)
            || (k >= 96 && k <= 105) || (k >= 37 && k <= 40))
        { }
        else if (k == 13) { //回车=Tab
            window.event.keyCode = 9;
        }
        else {
            window.event.returnValue = false;
        }
        /*
        ascii码说明：
8：退格键
46：delete
37-40： 方向键
48-57：小键盘区的数字
96-105：主键盘区的数字
110、190：小键盘区和主键盘区的小数点
189、109：小键盘区和主键盘区的负号

13：回车
9： Tab 就是那个把焦点移到下一个文本框的东东。

 对了还有两个缺点：
1、没有验证多个小数点的情况。
2、没有验证多个负号，和符号必须在前的情
        */
    }
    ,      //改变json日期格式
    ChangeDateFormat: function (dateVal, displayTime) {
        try {
            var date = new Date(parseInt(dateVal.replace("/Date(", "").replace(")/", ""), 10));
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var day = date.getDate();
            var hour = date.getHours();
            var minute = date.getMinutes();
            var second = date.getSeconds();

            if (displayTime) {
                return "{0}-{1}-{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
            } else {
                return "{0}-{1}-{2}".formatBy(year, doubleDigit(month), doubleDigit(day));
            }



        } catch (e) {
            return "";
        }

        function doubleDigit(n) { return n < 10 ? "0" + n : "" + n; }
    }
    ,
    ShowJsonText: function (data) {
        var textarea = $("<pre  style='width:100%'></pre>");
        if ($.isPlainObject(data)) {
            if (JsonUti && JsonUti.convertToString)
            { textarea.text(JsonUti.convertToString(data)); }
            else {
                textarea.text(WebUtil.jsonToString(data));
            }
        }
        else {
            textarea.text(data);
        }

        $(textarea).OpenDiv({
            title: '数据查看',
            mode: 'alert',
            scrollY: true
        });
    }
,
    GetItemText: function (array, value) {
        if (!array) {
            return "";
        }
        var result = "";

        $.each(array, function (i, item) {
            if (item.Value == value) {
                result = item.Text;
                return false;
            }
        });

        return result;
    }

}

//田振建添加错误提示框实现 Start
    $.MsgBox = {
        Alert: function (title, sketchErrorMessage,msg) {
            GenerateHtml("alert", title, sketchErrorMessage, msg);
            btnOk();  //alert只是弹出消息，因此没必要用到回调函数callback
            btnNo();
        },
        Confirm: function (title, sketchErrorMessage, msg, callback) {
            GenerateHtml("confirm", title,sketchErrorMessage, msg);
            btnOk(callback);
            btnNo();
        }
    }
    //生成Html
var GenerateHtml = function (type, title, sketchErrorMessage, msg) {
        var _html = "";
        _html += '<div id="mb_box"></div><div id="mb_con"><span id="mb_tit"><img src="../Images/action-icon.png">  ' + title + ' <a id="mb_ico"><img src="../Images/close-icon.png"></a></span>';
        if (msg == "add") {
            _html += '<div id="mb_msg"><div style="margin-bottom: 15px;"><span>角色名称：</span><input type="text" id="CnName"/><p  class="CnNameAction"style="padding-left:10px;color: #cb5c61;display:none;">请输入角色名称<p></div><div><span>角色描述：</span><input type="text" id="Description"/></div></div><div id="mb_btnbox">';
        } else if (msg == "edit") {
            
            _html += '<div id="mb_msg"><div style="margin-bottom: 15px;"><span>角色名称：</span><input type="text" id="CnName" value="' + JSON.parse(sketchErrorMessage).CnName + '"/><p  class="CnNameAction"style="padding-left:10px;color: #cb5c61;display:none;">请输入角色名称<p></div><div><span>角色描述：</span><input type="text" id="Description" value="' + JSON.parse(sketchErrorMessage).Description + '"/></div></div><div id="mb_btnbox">';
        }
        else {
            _html += '<div id="se_msg">' + sketchErrorMessage + '</div><div id="mb_btnbox">';
        }
        
        if (type == "alert") {
            _html += '<input id="mb_btn_ok" type="button" value="确定" />';
        }
        if (type == "confirm") {
            _html += '<input id="mb_btn_ok" type="button" value="确定" />';
            _html += '<input id="mb_btn_no" type="button" value="取消" />';
        }
        _html += '</div></div>';
        //必须先将_html添加到body，再设置Css样式
        $("body").append(_html); GenerateCss();
    }
    //生成Css
    var GenerateCss = function () {
        $("#mb_box").css({
            width: '100%', height: '100%', zIndex: '99999', position: 'fixed',
            filter: 'Alpha(opacity=60)', backgroundColor: 'black', top: '0', left: '0', opacity: '0.6'
        });
        $("#mb_con").css({
            zIndex: '999999', width: '80%', position: 'fixed', maxWidth: "300px",
            backgroundColor: 'White', borderRadius: '3px'
        });
        $("#mb_tit").css({
            display: 'block', fontSize: '14px', color: '#666666', padding: '10px 15px',
        });
        $("#mb_tit img").css({
            width: "18px", height: "18px", verticalAlign: "middle"
        });
        $("#se_msg").css({
            padding: "0 1.6em .8em", minHeight: "50px", fontSize: "15px", marginTop: "15px", textAlign: "center",
            lineHeight: "1.3", wordWrap: "breakWord", wordBreak: "breakAll",borderBottom: "1px solid #ccc",
            color: "#999"
        });

        $("#mb_msg").css({
            padding: '15px', lineHeight: '20px', textAlign: "center",
            borderBottom: '1px solid #ccc', fontSize: '13px'
        });
        $("#mb_msg input").css({
            outline: 0,
        });
        $("#ErrorTextarea").css({
            width: '92%',
        height: '205px',
        padding: '5px 20px 5px 20px',
        border:'1px solid #ccc'
        });
        $("#mb_ico").css({
            width: '15px', height: '15px', cursor: 'pointer',float:"right",
        });
        $("#mb_ico img").css({
            width: '15px', height: '15px',
        })
        $("#mb_btnbox").css({ margin: '5px 0', textAlign: 'center' });
        $("#mb_btn_ok,#mb_btn_no").css({ width: '70px', height: '30px', color: 'white', border: '1px solid #cb5c61', borderRadius: "4px", outline: 0, cursor:"pointer" });
        $("#mb_btn_ok").css({ backgroundColor: '#d2322d' });
        $("#mb_btn_no").css({ marginLeft: '20px', color: "#cb5c61",backgroundColor: "#fff" });
        var _widht = document.documentElement.clientWidth;  //屏幕宽
        var _height = document.documentElement.clientHeight; //屏幕高
        var boxWidth = $("#mb_con").width();
        var boxHeight = $("#mb_con").height();
        //让提示框居中
        $("#mb_con").css({ top: (_height - boxHeight) / 2 + "px", left: (_widht - boxWidth) / 2 + "px" });
    }
    //确定按钮事件
    var btnOk = function (callback) {
        $("#mb_btn_ok").click(function () {
            if (typeof (callback) == 'function') {
                callback();
            } else {
                $("#mb_box,#mb_con").remove();
            }
        });
    }
    //取消按钮事件
    var btnNo = function () {
        $("#mb_btn_no,#mb_ico").click(function () {
            $("#mb_box,#mb_con").remove();
        });
    }
//田振建添加错误提示框实现 End

var WebUtil = {

    //将文本域换行符替换为HTML换行标签：
    TextAreaContextChangeHtml: function (textereaContext) {
        return textereaContext.replace(/\n|\r|(\r\n)|(\u0085)|(\u2028)|(\u2029)/g, "<br/>");
        //return textereaContext;
    },

    getQueryString: function (name) {
        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    },


    getHashString: function (name) {
        var result = location.hash.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    },

    alertInfo: function (messageHtml, callback) {
        var tmplFormat = ["<div style='padding:0px 0 20px 0;width:95%;min-height:180px;overflow:hidden'>",
          "<table border=0 >",
              "<tr>",
              "<td style='vertical-align:middle;word-break:break-all;height:100%; white-space:normal;font-size:14px;padding-left:20px; '>",
                  " {0} </td>",
              "</tr>",
              "</table>",
          "</div>"].join('');
        var setting = { title: '提示信息', scrollY: true, mode: 'alert', widthMode: "thin", other: "" };
        if (callback != undefined && typeof (callback) == "function") {
            setting.onSubmit = callback;
        }
        $(tmplFormat.formatBy(messageHtml)).OpenDiv(setting);
    },

    alertWarn: function (sketchErrorMessage, messageHtml) {
        $.MsgBox.Alert("错误消息", sketchErrorMessage,messageHtml);

        //window.alert(messageHtml);
        $.unblockUI();
        return;

        var tmplFormat = ["<div style='padding:0px 0 20px 0;width:95%;min-height:180px;overflow:hidden'>",
            "<table border=0 >",
                "<tr>",
                "<td style='padding-right:10px;vertical-align:top;width:30px'>",
                    "<img src='../Images/warning.png'/></td>",
                "<td style='vertical-align:top;padding-left:10px; '>",
                    "<div style='width:100%;word-break:break-all; white-space:normal'>{0}<div></td>",
                "</tr>",
                "</table>",
            "</div>"].join('');

        $(tmplFormat.formatBy(messageHtml)).OpenDiv({
            title: '提示信息',
            scrollY: true,
            mode: 'alert',
            btns: [{ name: "复制信息", onclick: function (context) { copyContent(context) } }]
        });
    },

    confirm: function (message, confirmFunc) {
        var tmplFormat = ["<div style='padding:0px 0 20px 0;width:100%;overflow:auto;height:120px'>",
           "<table border=0 style='width:100%'>",
               "<tr>",
               "<td style='vertical-align:top;padding-left:10px;font-size:14px;text-align:center'>",
                   "{0}</td>",
               "</tr>",
               "</table>",
           "</div>"].join('');

        $(tmplFormat.formatBy(message)).OpenDiv({
            title: '确认信息',
            widthMode: "thin",
            onSubmit: function () { if (confirmFunc) confirmFunc(); return true; }
        });

    },


    /*
Requirements: 
     jquery.js
     blockUI.js
     OpenDiv(inner)

     /images/ajax-loader.gif
     /Images/warning.png
*/
    ajax: function (options) {
        var setting = {
            async: false,
            asyncBlock: true,
            cache: true,
            type: "post", //"get",//"post",
            dataType: "json", //默认使用json， 除非指定为html
            url: "", // /{controller}/{action}.jx 
            args: {}, //*3 action arguments. It is suggested to use below 3 level json object. 
            before: function (beforeOptions) {
                // check data before ajax sending
                if (CommonUtil.isNA(beforeOptions.url)) {
                    return false;
                }

                if (beforeOptions.url.endWith(".jx") == false) {
                    beforeOptions.url += ".jx";//自动补充后缀
                }

                return true;
            },
            successReturn: function (result) { //*4
                // server handled data successfully, and return normal
                // result type is JSON when dataType='json'
                // ALWAYSE be override by invoker
                alert(result);
            },
            failureReturn: function (errorCode, errorMsg, errorResult) {
                // server is failed to handle data , but failure or exception has been encapsulated on server.
                // errorResult type is JSON when dataType='json'
                var code = errorCode.toString();
                if (typeof (setting.errorHandlers[code]) != "undefined" && $.isFunction(setting.errorHandlers[code])) {
                    setting.errorHandlers[code](errorMsg);
                }
                else {
                    var i = errorMsg.indexOf("\r\n");
                    var errormsg = "<p>" + errorMsg + "</p>";
                    if (i > 0) errormsg = "<p>" + errorMsg.substring(0, i) + "</p>";
                    errormsg = errormsg + "<p style='display:none' class='detail'>" + errorMsg + "</p>";
                    //errormsg = errormsg + "<p><a href='javascript:void(0)' onclick='copyContent(this)' class='btn_orange'>复制</a></p>";
                    WebUtil.alertWarn(errormsg,errormsg);
                }
            },
            errorHandlers: {
                "0": function (msg) {
                    WebUtil.confirm("没有登录或者会话已过期！\r\n点确定重新登录。", function () {
                        window.location = '/public/login.aspx?returnurl='
                                 + encodeURIComponent(window.location.pathname + window.location.search); //重新跳转, 系统会转到Login.aspx?ReturnUrl=...
                    });
                },
                "1": function (msg) { WebUtil.alertWarn("请求被拒绝！",""); },
                "2": function (msg) { WebUtil.alertWarn("没有权限访问此方法！",""); },
                "3": function (msg) { WebUtil.alertWarn("数据库操作失败！" ,msg); },
                "4": function (msg) { WebUtil.alertWarn("结果数据转化为JSON失败！",""); },
                "5": function (msg) { WebUtil.alertWarn("非法的Ajax Action！",msg); },
                "6": function (msg) { WebUtil.alertWarn("缺少某些参数！", msg); },
                "7": function (msg) { WebUtil.alertWarn("没找到匹配的数据！", ""); },
                "8": function (msg) {
                    WebUtil.alertWarn(msg, msg
                        );
                }
            }
        };
        $.extend(setting, options);

        if (typeof setting.before == 'function') {
            //setTimeout($.blockUI, 10, null);
            if (setting.async && setting.asyncBlock && $.blockUI) {
                $.blockUI({ message: "<div style='width:200px'><img src='../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
            }
            if (setting.before(setting) == false)
                return;
        }

        // 对参数做htmlEncode
        var args = {};
        for (var p in setting.args) {
            if ($.type(setting.args[p]) === "string") {
                args[p] = WebUtil.htmlEncode(setting.args[p]);
            }
            else {
                args[p] = setting.args[p];
            }
        }

        // 传递操作系统信息和浏览器信息
        $.extend(args, {
            __1osInfo: WebUtil.getOSInfo(),
            __2browserInfo: WebUtil.getBrowserInfo().description
        });

        jQuery.ajax({
            async: setting.async,
            type: setting.type,
            dataType: setting.dataType,
            cache: setting.cache,
            url: setting.url,
            data: args,
            success: function (result) {
                //setTimeout($.unblockUI, 10, null);

                if (setting.dataType == "html") {
                    if (typeof setting.successReturn == "function") {
                        setting.successReturn(result, setting);
                    }

                } else if (setting.dataType == "json") {


                    if (result == null) {
                        callAjaxFailed({ ErrorCode: -300, ErrorMessage: "Failed to get data.", callback: setting.failureReturn });
                    }
                    else {

                        if (result.ModelCode == 200) {
                            //log.info("action=" + setting.action + "; return success! ");
                            if (typeof setting.successReturn == "function") {
                                try {
                                    setting.successReturn(result.ResultData, setting);
                                } catch (e) {
                                    WebUtil.alertWarn(e.toString(), e.toString() + ":" + WebUtil.jsonToString(result.ResultData));
                                }
                            }
                        }
                        else {
                            //log.info("action=" + setting.action + "; return failure! ");
                            var msg = result.ErrorMessage;
                            if (result.ModelCode == -99 || msg == "") {
                                msg = "Failed to get data.";
                            }
                            callAjaxFailed({ ErrorResult: result, ErrorCode: result.ModelCode, ErrorMessage: msg, callback: setting.failureReturn });
                        }
                    }
                }
                if (setting.async && setting.asyncBlock && $.unblockUI) {
                   $.unblockUI();
                }
            },
            error: function (XMLHttpRequest, textStatus) {
                var msg = "Failed to get data.";
                if (!XMLHttpRequest) {
                    callAjaxFailed({ ErrorCode: -100, ErrorMessage: msg, callback: setting.failureReturn });
                }
                else {
                    if (XMLHttpRequest.status == 200) {
                        if (XMLHttpRequest.responseText.indexOf("loginPageTag") > 0
                            || XMLHttpRequest.responseText.indexOf("THISISSSOLOGINPAGE") > 0
                            || XMLHttpRequest.responseText.indexOf("无用户身份信息") > 0
                            ) {
                            callAjaxFailed({ ErrorCode: "0", ErrorMessage: "没有登录或者会话已过期!", callback: setting.failureReturn });
                        }
                    }
                    else {
                        var $response = $(XMLHttpRequest.responseText);
                        var exception = $response.find("code").text();
                        var title = $response.find("title").text();
                        callAjaxFailed({ ErrorCode: XMLHttpRequest.status, ErrorMessage: title + exception, callback: setting.failureReturn });
                    }
                }
            },
            timeout: function () {
                var msg = "Time out.";
                callAjaxFailed({ ErrorCode: -200, ErrorMessage: msg, callback: setting.failureReturn });
            }
        });

        // 处理正常返回结果时， 结果中的错误信息处理
        function callAjaxFailed(options) {
            if (typeof options.callback != 'function') {
                var errormsg = "error code:" + options.ErrorCode + "\r\n error msg:" + options.ErrorMessage;
                WebUtil.alertWarn("error code",errormsg);
            } else {
                options.callback(options.ErrorCode, options.ErrorMessage, options.ErrorResult); // failureReturn
            }
        };

    },

    // 将json对象转换为js string
    jsonToString: function (data, replacer, space) {// replacer and space are optional
        if ((typeof JSON == "undefined") || (typeof JSON.stringify == "undefined"))
            throw new Error("Cannot find JSON.stringify(). Some browsers (e.g., IE < 8) don't support it natively, but you can overcome this by adding a script reference to json2.js, downloadable from http://www.json.org/json2.js");
        return JSON.stringify(data, replacer, space);
    },

    // 将简单数组对象转换为js string
    // 注：未处理双引号
    arrayToString: function (array) {

        if (arguments.length == 0)
            throw new Error("no arguments, pass the object you want to convert.");

        if (!array || typeof array == "undefined" || array.length < 1) {
            return "";
        }


        return "[\"" + array.join("\",\"") + "\"]";
    }

    , loadTmpl: function (url, selector) {
        url = url + "?ver=1" ;
        var result = null;
        var container = $("#tmplContainer__");
        if (container.length == 0 || true) {
            $.ajax(
                {
                    url: url,
                    cache: true,
                    async: false,
                    dataType: "html",
                    type: "get",
                    success: function (html) {
                        $("#tmplContainer__").remove();
                        container = $("<div id='tmplContainer__'></div>");
                        container.appendTo("body");
                        container.append(html);
                    }
                });
        }

        result = container.find(selector);
        return result;
    }

    , htmlEncode: function (str) {
        if ($.type(str) != "string") return str;
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&/g, "&amp;");
        s = s.replace(/</g, "&lt;");
        s = s.replace(/>/g, "&gt;");
        s = s.replace(/ /g, "&nbsp;");
        s = s.replace(/\'/g, "&#39;");
        s = s.replace(/\"/g, "&quot;");
        return s;
    }
    , htmlDecode: function (str) {
        if ($.type(str) != "string") return str;
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&amp;/g, "&");
        s = s.replace(/&lt;/g, "<");
        s = s.replace(/&gt;/g, ">");
        s = s.replace(/&nbsp;/g, " ");
        s = s.replace(/&#39;/g, "\'");
        s = s.replace(/&quot;/g, "\"");
        return s;
    }

    , getOSInfo: function () {
        var platform = navigator.platform;
        var appVer = navigator.userAgent;
        var sysBit = "32位";

        if (platform == "Win32" || platform == "Windows") {
            if (appVer.indexOf("WOW64") > -1) {
                sysBit = "64位";
            }
            if (appVer.indexOf("Windows NT 6.0") > -1 || appVer.indexOf("Windows Vista") > -1) {
                return 'Windows_vista ' + sysBit;

            } else if (appVer.indexOf("Windows NT 6.1") > -1 || appVer.indexOf("Windows 7") > -1) {
                return "Windows_7 " + sysBit;

            } else if (appVer.indexOf("Windows NT 6.2") > -1 || appVer.indexOf("Windows 7") > -1) {
                return "Windows_8 " + sysBit;
            } else {
                try {
                    var _winName = Array('2000', 'XP', '2003');
                    var _ntNum = appVer.match(/Windows NT 5.\d/i).toString();
                    return 'Windows_' + _winName[_ntNum.replace(/Windows NT 5.(\d)/i, "$1")] + " " + sysBit;
                } catch (e) { return 'Windows'; }
            }
        } else if (platform == "Mac68K" || platform == "MacPPC" || platform == "Macintosh") {
            return "Mac";
        } else if (platform == "X11") {
            return "Unix";
        } else if (String(platform).indexOf("Linux") > -1) {
            return "Linux";
        } else {
            return "Unknow";
        }
    }

    // 取 result.ie/firefox/chrome/opera/safari, 然后 取result.description
    , getBrowserInfo: function () {
        return {};
        var result = {};
        var ua = navigator.userAgent.toLowerCase();
        window.ActiveXObject ? result.ie = ua.match(/msie ([\d.]+)/)[1] :
        document.getBoxObjectFor ? result.firefox = ua.match(/firefox\/([\d.]+)/)[1] :
        window.MessageEvent && !document.getBoxObjectFor ? result.chrome = ua.match(/chrome\/([\d.]+)/)[1] :
        window.opera ? result.opera = ua.match(/opr.([\d.]+)/)[1] :
        window.openDatabase ? result.safari = ua.match(/version\/([\d.]+)/)[1] : 0;

        if (result.ie) { result.description = 'IE: ' + result.ie }
        else if (result.firefox) { result.description = 'Firefox: ' + result.firefox }
        else if (result.chrome) { result.description = 'Chrome: ' + result.chrome }
        else if (result.opera) { result.description = 'Opera: ' + result.opera }
        else if (result.safari) { result.description = 'Safari: ' + result.safari }
        return result;
    }
    // 获得IE浏览器的版本
    , getIEVersion: function () {
        var v = 3, div = document.createElement('div'), all = div.getElementsByTagName('i');
        while (
            div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->',
            all[0]
        );
        return v > 4 ? v : false;
    }
    , exportExcel: function (options) {

        if ($.type(options) == "string") {
            options = { category: options }
        }
        var _setting = {
            category: "",
            args: {}
        }

        $.extend(_setting, options);

        if (_setting.category.startWith("/") == false) {
            _setting.category = "/" + _setting.category;
        }

        if (!_setting.category.endWith(".xlsh")) {
            _setting.category += ".xlsh";
        }

        var url = _setting.category + "?" + $.param(_setting.args);
        //alert(url);//

        window.open(url, "下载Excel", "height=0,width=0,top=0,left=0,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no", null);
    }
    , downloadFile: function (options) {

        if ($.type(options) == "string") {
            options = { filePath: options }
        }
        var p = "/download.aspx";
        var _setting = {
            filePath: "",
            fileName: ""
        };
        $.extend(_setting, options);
        var url = p + "?" + $.param(_setting);
        window.open(url, "下载文件", "height=0,width=0,top=0,left=0,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no", null);
    }

    , other: null
}

function copyContent(sender) {
    window.clipboardData.setData("text", $(sender).find(".detail").text());
    //alert(window.clipboardData.getData("text"));
}

function InitTabs() {
    $('ul.info_tabs').each(function () {
        // For each set of tabs, we want to keep track of
        // which tab is active and it's associated content
        var $active, $content, $links = $(this).find("li a");

        // If the location.hash matches one of the links, use that as the active tab.
        // If no match is found, use the first link as the initial active tab.
        $active = $($links.filter('[href="' + location.hash + '"]')[0] || $links[0]);
        $active.addClass('active');
        $content = $($active.attr('href'));

        $content.show();
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

// Date Extension
(function () {

    // 注意， 是直接修改了原日期
    Date.prototype.addDay = function (num) {
        this.setDate(this.getDate() + num);
        return this;
    };

    Date.prototype.addMonth = function (num) {
        var tempDate = this.getDate();
        this.setMonth(this.getMonth() + num);
        if (tempDate != this.getDate()) this.setDate(0);
        return this;
    };

    Date.prototype.addYear = function (num) {
        var tempDate = this.getDate();
        this.setYear(this.getYear() + num);
        if (tempDate != this.getDate()) this.setDate(0);
        return this;
    };

    Date.prototype.toDateString = function () {
        var result = this.getFullYear() + "/" + (this.getMonth() + 1) + "/" + this.getDate();
        return result;
    };


    Date.prototype.toDateTimeString = function () {
        var result = this.getFullYear() + "/" + (this.getMonth() + 1) + "/" + this.getDate() + " " +
          +this.getHours() + ":" + this.getMinutes() + ":" + this.getSeconds();
        return result;
    };


    Array.prototype.indexOf = function (val) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == val) return i;
        }
        return -1;
    };
    Array.prototype.remove = function (val) {
        var index = this.indexOf(val);
        if (index > -1) {
            this.splice(index, 1);
        }
    };
    Array.prototype.replace = function (oldVal, newVal) {
        this.remove(oldVal);
        this.push(newVal);
    };

    Array.prototype.contains = function (obj) {
        var result = false;
        for (var __xi = 0; __xi < this.length; __xi++) {
            if (WebUtil.jsonToString(this[__xi]) == WebUtil.jsonToString(obj)) {
                result = true;
                break;
            }
        }
        return result;
    }

    Array.prototype.clear = function () {
        this.length = 0;
    }
    Array.prototype.insertAt = function (index, obj) {
        this.splice(index, 0, obj);
    }
    Array.prototype.removeAt = function (index) {
        if (isNaN(index) || index > this.length) { return false; }
        for (var i = 0, n = 0; i < this.length; i++) {
            if (this[i] != this[index]) {
                this[n++] = this[i];
            }
        }
        this.length -= 1;
    }
})();

// Number Extension
(function () {


    // 转换为默认2位小数的， 带千分位的字符串
    Number.prototype.thousandize = function (options) {
        var _setting = {
            precision: 2,
            emptyWhenZero: false,
            emptyString: ""
        }
        $.extend(_setting, options);

        // s 表示绝对值， n表示小数后的精度位数， opt表示正负号
        var s = Math.abs(this), n = _setting.precision, opt = (this < 0) ? "-" : "";
        n = ((n >= 0) && (n <= 7) ? n : 2);


        // 注意, 特别小的小数会因为科学计数法， 如0.00000011=1.1e-7, 从而导致后面的字符替换结果出错
        if (s < Math.pow(0.1, (n + 2))) {  //1e(n+2)是为了微小数不至于影响转换
            if (_setting.emptyWhenZero) { //当s为特别小的小数时， 近似为0； 
                return _setting.emptyString;
            }
            else {
                s = Math.pow(0.1, (n + 2));
            }
        }
        s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";

        var l, r, t;
        if (n == 0) {
            l = s.split("").reverse();
            r = "";
        } else {
            l = s.split(".")[0].split("").reverse(),
            r = s.split(".")[1];
        }
        t = "";
        for (i = 0; i < l.length; i++) {
            t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
        }

        if (n == 0) {
            return opt + t.split("").reverse().join("");
        } else {
            return opt + t.split("").reverse().join("") + "." + r;

        }
    };
    // 转换为默认4位小数的， 带千分位的字符串
    Number.prototype.thousandize4 = function () {
        var _setting = {
            precision: 4
        }

        return this.thousandize(_setting);
    };

    // 转换为默认4位小数的， 带千分位的字符串
    Number.prototype.thousandize4OrEmpty = function () {
        var _setting = {
            precision: 4,
            emptyWhenZero: true,
            emptyString: "--"
        }

        return this.thousandize(_setting);
    };

    // 转换为默认1位小数的， 带千分位的字符串
    Number.prototype.thousandize1OrEmpty = function () {
        var _setting = {
            precision: 1,
            emptyWhenZero: true,
            emptyString: "--"
        }

        return this.thousandize(_setting);
    };


    // 转换为默认1位小数的， 带千分位的字符串
    Number.prototype.thousandize0OrEmpty = function () {
        var _setting = {
            precision: 0,
            emptyWhenZero: true,
            emptyString: "0"
        }

        return this.thousandize(_setting);
    };
})();

// String Extension
(function () {


    //用法 ： var str = "{0},{1}".formatBy("Hi", " Bond");
    // str = "Hi, Bond"
    String.prototype.formatBy = function () {
        if (arguments.length == 0) {
            return this;
        }

        var args = arguments;
        if ($.isArray(arguments[0])) {
            args = arguments[0];
        }

        for (var StringFormat_s = this, StringFormat_i = 0; StringFormat_i < args.length; StringFormat_i++) {
            StringFormat_s = StringFormat_s.replace(new RegExp("\\{" + StringFormat_i + "\\}", "g"), args[StringFormat_i]);
        }
        return StringFormat_s;
    };

    String.prototype.endWith = function (str) {
        if (str == null || str == "" || this.length == 0 || str.length > this.length)
            return false;
        if (this.substring(this.length - str.length) == str)
            return true;
        else
            return false;
        return true;
    }

    String.prototype.startWith = function (str) {
        if (str == null || str == "" || this.length == 0 || str.length > this.length)
            return false;
        if (this.substr(0, str.length) == str)
            return true;
        else
            return false;
        return true;
    }
    String.prototype.isNumber = function () {
        var oNum = this;
        if (!oNum) return false;
        var strP = /^(\-)?(\d)+(\.\d+)?$/;
        if (!strP.test(oNum.replaceAll(',', ''))) return false;
        try {
            if (parseFloat(oNum.replaceAll(',', '')) != oNum.replaceAll(',', '')) return false;
        }
        catch (ex) {
            return false;
        }
        return true;
    }
    String.prototype.replaceAll = function (reallyDo, replaceWith, ignoreCase) {
        if (!RegExp.prototype.isPrototypeOf(reallyDo)) {
            return this.replace(new RegExp(reallyDo, (ignoreCase ? "gi" : "g")), replaceWith);
        } else {
            return this.replace(reallyDo, replaceWith);
        }
    }

    String.prototype.validateNumberLen = function (options) {
        var _setting = {
            intLen: 7,
            mantissaLen: 4
        };

        $.extend(_setting, options);
        var totalLen = _setting.intLen + _setting.mantissaLen + 1;
        var oNum = this;
        if (oNum.isNumber()) {
            //规则 
            //1.不包含小数，长度为intLen
            //2.包含小数，小数部分最长为mantissaLen,整数部分最长为intLen
            return (oNum.indexOf(".") == -1 && oNum.length <= _setting.intLen) || (oNum.indexOf(".") > -1 && oNum.indexOf(".") <= _setting.intLen && oNum.length < totalLen + 1 && (oNum.length - oNum.indexOf(".") - 1) < _setting.mantissaLen + 1);
        }
        return false;
    }

    String.prototype.leftSub = function (len) {
        return this.substr(0, len);
    }

    // 将千分位的数字文本转换为浮点数
    String.prototype.toFloat = function () {
        var numText = this.replaceAll(",", "");
        var result = parseFloat(numText);
        if (isNaN(result)) {
            result = 0;
        }
        return result;
    }
    // 将千分位的数字文本转换为整数
    String.prototype.toInt = function () {
        var numText = this.replaceAll(",", "");
        var result = parseInt(numText, 10);
        if (isNaN(result)) {
            result = 0;
        }
        return result;

    }

})();

// 多选下拉框
(function ($) {
    $.fn.extend({

        MultDropList: function (options) {


            var _setting = {
                id: "multiselect",
                wraperClass: "multiselect",
                width: 0,
                height: "auto",
                data: {},  // data是json 值键对 格式如{text:value}
                selectedValues: "",
                onItemClicked: function (selectedValues) { }, //单选选中的时候激发的事件
                onCollapsed: function (selectedValues) { }, //收起的时候激发的事件
                innerClose: true
            };

            $.extend(_setting, options);


            return this.each(function () {
                var $this = $(this); //指向TextBox
                //var $hf = $(this).next(); //指向隐藏控件存
                var conSelector = "#" + $this.attr("id") //+ ",#" + $hf.attr("id");
                var $wraper = $(conSelector).wrapAll("<div style='overflow-x:hidden'><div></div></div>").parent().parent().addClass(_setting.wraperClass);

                var $list = $('<div class="list" ></div>').appendTo($wraper);
                if (_setting.width == 0) {
                    _setting.width = $(this).width();
                }
                $list.css({
                    "width": ("" + _setting.width).isNumber() ? _setting.width + 6 + "px" : _setting.width,
                    "height": ("" + _setting.width).isNumber() ? _setting.height + "px" : _setting.height
                });

                $list.click(function (e) {
                    e.stopPropagation();
                });
                //加载默认数据

                // 全选项
                $list.append('<ul><li><input type="checkbox" class="mckselectAll" id="' + _setting.id + 'mckall"/><label for="' + _setting.id + 'mckall">全部</label></li></ul>');
                var $ul = $list.find("ul");

                // 单选项
                for (var obj in _setting.data) {
                    var text = obj;
                    var value = _setting.data[obj];
                    var id = _setting.id + 'mck"' + value;
                    $ul.append('<li><input type="checkbox" value="' + value + '" id=' + id + '" /><label for=' + id + '">' + text + '</label></li>');

                }

                // 根据当前值， 选中复选框
                if ($.trim(_setting.selectedValues)) {

                    var vals = $.trim(_setting.selectedValues).split(",");
                    var texts = [];
                    if (vals[0].toLowerCase() == '[all]') { //如果是[all]则表示全选
                        $("li :checkbox", $ul).attr("checked", true);
                        texts.push("全部");

                        // 用真实的值替换[all]
                        _setting.selectedValues = $.map($("li :checkbox", $ul),
                            function (ctrl) { return $(ctrl).val(); }).join(',');


                    }
                    else {
                        $.each(vals, function (index, value) {
                            var chk = $("li input[value='" + value + "']", $ul);
                            chk.attr("checked", true);
                            texts.push(chk.next().text());
                        });
                    }
                    $this.val(texts.join(","));
                }

                //全部选择或全不选
                $("input[class='mckselectAll']", $ul).first().click(function () {
                    var cks = $("li input[class!='mckselectAll']", $ul);

                    var checked = $(this).attr("checked");
                    cks.attr("checked", checked);

                });
                //点击其它复选框时，更新隐藏控件值,文本框的值
                $("input:checkbox", $ul).click(function () {
                    var kArr = new Array();
                    var vArr = new Array();
                    $("input[class!='mckselectAll']:checked", $ul).each(function (index) {
                        kArr[index] = $(this).val();
                        vArr[index] = $(this).next().text();
                    });
                    _setting.selectedValues = kArr.join(",");
                    $this.val(vArr.join(","));
                    var $checkAll = $("input[class='mckselectAll']", $ul);

                    if (kArr.length == 0) {
                        $checkAll.attr("checked", false);
                        $checkAll[0].indeterminate = false;
                    }
                    else if (kArr.length == ($("input", $ul).length - 1)) {
                        $checkAll.attr("checked", true);
                        $checkAll[0].indeterminate = false;
                        $this.val("全部");
                    }
                    else {
                        $checkAll.attr("checked", true);
                        $checkAll[0].indeterminate = true;
                    }

                    if (_setting.onItemClicked && $.isFunction(_setting.onItemClicked)) {
                        _setting.onItemClicked(_setting.selectedValues);
                    }

                });


                //控制弹出页面的显示与隐藏
                $this.click(function (e) {
                    // 弹出或关闭
                    if ($list.is(":visible")) {
                        Collapse();

                    }
                    else {
                        $list.show();

                    }
                    e.stopPropagation();
                });

                if (_setting.innerClose) {
                    $list.append("<input type='button' value='确定' style='float:right; margin-right:10px; padding: 2px 4px'/>");
                    $(":button", $list).click(function () {
                        Collapse();
                    });
                } else {
                    // 关闭
                    $(document).click(function () {
                        if ($list.is(":visible")) {
                            Collapse();
                        }
                    });
                }


                function Collapse() {
                    $list.hide();
                    if (_setting.onCollapsed && $.isFunction(_setting.onCollapsed)) {
                        _setting.onCollapsed(_setting.selectedValues);
                    }
                }
            });
        }
        // 客户端分页
        , paginationex: function (options) {

            var context = $(this);
            var _setting = {
                current: 1, // 当前页
                pageSize: 10, // 每页多少行
                pageCount: 1, // 共多少页
                totalCount: 0, // 共多少条数据
                navTo: function (pageIndex, pageSize) { }, //跳转到第几页， 需要使用者自己实现： 1 获得绑定数据，刷新表格 2 刷新分页器
                //onPaging: function (opt) {
                //    var _default = {
                //        data: null,
                //        tmplHtml: "",

                //    };
                //},
                cols: "20",
                other: null
            };

            $.extend(_setting, options);
            if (_setting.pageSize > 0 && _setting.totalCount > 0) {
                _setting.pageCount = Math.ceil(_setting.totalCount / _setting.pageSize);
            }
            else {
                _setting.pageCount = 0;
                _setting.current = 0;
            }
            // class: pagination, pager, first, prev, next, last, currentPage, totalPage, go, totalItemCount, w20
            // variance: ${current}, ${pageCount}, ${totalCount}
            var pageTmpl = [
                //"<tr   class='pagination'>",
                //"    <td colspan='" + _setting.cols + "'>",
                "        <div >",
                "            <a href='javascript:void(0)' class='first' style='border:1px solid #ccc;border-radius:3px!important;'>首页</a>",
                "            <a href='javascript:void(0)' class='prev' style='border:1px solid #ccc;border-radius:3px!important;'>上一页</a>",
                "            <a href='javascript:void(0)' class='next' style='border:1px solid #ccc;border-radius:3px!important;'>下一页</a>",
                "            <a href='javascript:void(0)' class='last' style='border:1px solid #ccc;border-radius:3px!important;'>尾页</a>",
                "            <span>当前页：<span class='currentPage'>${current}</span>/<span class='totalPage'>${pageCount}</span></span>",
                "            <span>&nbsp;&nbsp;&nbsp;&nbsp;跳转至：第</span>",
                "            <input class='numBox targetIndex numOnly' name='targetIndex' type='text' value='${current}' maxlength=7 style='width:30px;border:1px #bebebe solid;position:relative;top:-1px;text-align:center;padding-top:3px;padding-bottom:2px;border-radius:0px!important;'/>",
                "            <span>页</span>",
                "            <input class='go' type='button' value='跳转' style='cursor:pointer;border:1px solid #ccc;border-radius:3px!important;position:relative;top:-1px;padding:2px 7px;padding-top:4px;'/>",
                "            <span>&nbsp;&nbsp;&nbsp;&nbsp;共有<span class='totalItemCount'>${totalCount}</span>条数据</span>",
                "        </div>",
                //"    </td>",
                //"</tr>"
            ];
            var pageTmplMarkups = pageTmpl.join("");
            var pageTemplate = $.template("pageTemplate", pageTmplMarkups);
            var pager = $.tmpl("pageTemplate", _setting); //分页控件
            pager.appendTo(context);

            //分面样式修改

            if (_setting.current == 0) {
                pager.find("a.first").attr("disabled", true);
                pager.find("a.prev").attr("disabled", true);
            } else {
                pager.find("a.first").attr("disabled", false);
                pager.find("a.prev").attr("disabled", false);
            }

            if (_setting.current == _setting.pageCount - 1) {
                pager.find("a.next").attr("disabled", true);
                pager.find("a.last").attr("disabled", true);

            } else {
                pager.find("a.next").attr("disabled", false);
                pager.find("a.last").attr("disabled", false);
            }
            //分页操作事件
            pager.find("a.first")
                .attr("title", "快捷键 HOME")
                .click(function () {
                    if (_setting.totalCount == 0) return;
                    _setting.navTo(1, _setting.pageSize);
                    //var btn = context.find("a.first");
                    //if (btn.length > 0) {
                    //    btn.focus();
                    //}

                });
            pager.find("a.prev")
                .attr("title", "快捷键 <-")
                .click(function () {
                    if (_setting.current > 1)
                        _setting.navTo(_setting.current - 1, _setting.pageSize);

                    if (_setting.current == 0) {
                        pager.find("a.first").attr("disabled", true);
                        pager.find("a.prev").attr("disabled", true);
                    } else {
                        pager.find("a.first").attr("disabled", false);
                        pager.find("a.prev").attr("disabled", false);
                    }
                    //var btn = context.find("a.prev");
                    //if (btn.length > 0) {
                    //    btn.focus();
                    //}

                });
            pager.find("a.next")
                .attr("title", "快捷键 ->")
                .click(function () {
                    if (_setting.current < _setting.pageCount)
                        _setting.navTo(_setting.current + 1, _setting.pageSize);
                    //var btn = context.find("a.next");
                    //if (btn.length > 0) {
                    //    btn.focus();
                    //}
                });
            pager.find("a.last")
                .attr("title", "快捷键 END")
                .click(function () {
                    _setting.navTo(_setting.pageCount, _setting.pageSize);
                    //var btn = context.find("a.last");
                    //if (btn.length > 0) {
                    //    btn.focus();
                    //}
                });
            pager.find("input.targetIndex").keyup(function () {
                if (event.keyCode == 13) { pager.find("input.go").click(); }
                var input = context.find("input.targetIndex");
                if (input.length > 0) {
                    input.focus();
                }

            });
            pager.find("input.go").click(function () {
                var targetIndex = pager.find("input.targetIndex").val().toInt();
                if (targetIndex < 1) {
                    targetIndex = 1;
                }
                else if (targetIndex > _setting.pageCount) {
                    targetIndex = _setting.pageCount;
                }
                _setting.navTo(targetIndex - 1, _setting.pageSize);

                //var input = context.find("input.targetIndex");
                //if (input.length > 0) {
                //    input.focus();
                //}
            });
            pager.keyup(function () {
                //  alert(event.keyCode);
                switch (event.keyCode) {
                    case 33: case 37: pager.find("a.prev").click(); break;/*page up*/
                    case 34: case 39: pager.find("a.next").click(); break;/*page down*/
                    case 35: pager.find("a.last").click(); break;/*end*/
                    case 36: pager.find("a.first").click(); break; /*home*/
                    default: break;
                }
                // alert(context.find("input.targetIndex").length);
                context.find("input.targetIndex").click();
                event.returnValue = false;
            })
        }

    });
})(jQuery);
jQuery.extend({
    handleError: function (s, xhr, status, e) {
        // If a local callback was specified, fire it
        if (s.error)
            s.error(xhr, status, e);
            // If we have some XML response text (e.g. from an AJAX call) then log it in the console
        else if (xhr.responseText)
            console.log(xhr.responseText);
    }
})
// 表头浮动
(function ($) {
    // 暂时只支持表头的浮动
    // 支持带水平的滚动条（必须是element的水平滚动条）
    // 支持窗体的resize

    $.fn.smartFloat = function (option) {

        var _setting = {
            mode: "fixed" // enum: fixed or float
            , headContainer: ".tableHeadContainer"
            , bodyContainer: ".tableBodyContainer"
            // ,supportOnly : "table"
        }

        $.extend(_setting, option);

        var position = function (element) {
            //var top = element.position().top, pos = element.css("position");
            if (element.length == 0) {
                return;
            }
            var head = element.find(_setting.headContainer);
            var body = element.find(_setting.bodyContainer);

            if (head.length == 0) {
                return;
            }
            var headerPos = head.position().top;
            var hd = null;
            $(window).scroll(headFloat);
            $(window).bind("navToggle", headFloat);

            $("#switchPoint").click(function () { setTimeout(headFloat, 200) });

            if (_setting.mode == "fixed") {
                // 处理窗口大小调整的情况

                window.resizeHandler = null;
                $(window).resize(function () {
                    if (window.resizeHandler != null) clearTimeout(window.resizeHandler);
                    window.resizeHandler = setTimeout(headFloat //function () { //head.width(head.parent().width() - 1);},
                                                    , 200);

                });

                // 处理水平滚动的情况
                element.scroll(function () {
                    if (head.css("position") == "fixed") {
                        head.scrollLeft(element.scrollLeft());

                    }
                });
            }
            else if (_setting.mode == "float") {

            }

            function headFloat() {
                var scrolls = $(window).scrollTop();

                if (_setting.mode == "fixed") {
                    if (scrolls > headerPos) {
                        head.css({
                            position: "fixed",
                            top: 0,
                            width: head.parent().innerWidth(),
                            "overflow-x": "hidden"

                        });
                        if (element.scrollLeft() > 0) {

                            // 恢复之前的水平滚动偏移 
                            head.scrollLeft(element.scrollLeft());
                        }
                    }
                    else {

                        head.css({
                            position: "static",
                            top: 0,
                            width: "",
                            "overflow-x": ""

                        });

                        if (head.scrollLeft() > 0) {
                            // 水平滚动偏移 
                            head.scrollLeft(0);
                        }


                    }
                    //   end--
                } else if (_setting.mode == "float") {
                    if (scrolls > headerPos) {

                        if (hd != null) clearTimeout(hd);
                        hd = setTimeout(function () {
                            head.animate({
                                top: (scrolls - headerPos) + "px"
                            }, 250);
                        }, 50);

                    } else {
                        head.css({ top: 0 });
                    }
                }
            }
        };

        return $(this).each(function () {
            //if ($(this).is("thead") || $(this).is("tr")) {
            if (true) {
                //if ($(this).is("table")) {
                position($(this));
            }
            else {
                alert('暂不支持');
            }
        });
    };
}(jQuery));



//弹出层
jQuery.fn.extend(
{
    OpenDiv: function (options) {
        var $self = $(this);
        var _setting = {
            title: "",
            level: 1, //弹出层级别
            onInit: function (context) { },  //在对话框弹出时， 初始化对话框内容

            onCancel: function (context) { $(context).ContextValidation(false); return true; }, //在对话框取消/关闭时， 触发的事件； 未返回为true，则无法关闭对话框
            onSubmit: function (context) { return true; }, //在对话框确定提交时， 触发的事件； 未返回为true，则无法关闭对话框
            //btns: [{ name: "test", cssClass: "", onclick: function (context) { } }], //自定义的按钮及事件
            mode: "confirm", //or "alert" or "info"
            widthMode: "standard", //thin/small= 400px, standard=700px, wide/large=1000px
            scrollY: false, //如果弹出内容很长或者需要增长， 则设为scrollY:true
            other: ""
        }

        if (!window.popupZindex) {
            window.popupZindex = 10000;
        }

        $.extend(_setting, options);


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
        if ($(".maskDiv").length == 1) {
            $("html").data("overflow", $("html").css("overflow"))
                    .css("overflow", "hidden");
        }
        // window.scrollTo($("#BigDiv").attr("divbox_scrollleft"), maskObj.attr("divbox_scrolltop"));

        if (_setting.level < 1) { _setting.level = 1 }
        if (_setting.level > 4) { _setting.level = 4 }

        var popupFrameHtml = [

      '<div class="popup context">',
      ' <div class="t_l"> ',
      ' </div>            ',
      ' <div class="t_m"> ',
      ' </div>            ',
      ' <div class="t_r"> ',
      ' </div>            ',
      ' <div class="m_l dnrHandler"> ',
      ' </div>            ',
      ' <div class="m_m"> ',
      '     <div class="pop_titile dnrHandler">',
      '         <span class="pop_icon pop_icon_0' + _setting.level + '"></span>',
      '         <div class="pop_txt">',
      '             <span class="pop_txt_title"></span>',
      //'             <img src="../images/popup/help.png" class="help" />',
      '             <img src="../images/popup/exit.png" class="exit" />',
      '         </div>',
      '     </div>',
      '     <!--pop_titile-->',
      '     <div class="padding_10  popup_content">', //class overflow
      '     </div>',
      '     <div class="pop_btn line_t" style="padding: 10px">',
      '         <div class="btn01 btn_fr btn_cancel"  ><a><span>取消</span></a></div>',
      '         <div class="btn_blue01 btn_fr btn_submit"><a><span>确定</span></a></div>',
      '     </div>',
      ' </div>',
      ' <div class="m_r dnrHandler">',
      ' </div>             ',
      ' <div class="b_l">  ',
      ' </div>             ',
      ' <div class="b_m dnrHandler">  ',
      ' </div>             ',
      ' <div class="b_r resizeHandler ">  ',
      ' <div class="resizeHandler"></div>  ',
      ' </div>             ',
      '</div>'
        ];
        var $popupFrame;//= $(popupFrameHtml.join(""));
        if ($self.parents(".popup").length > 0) {
            $popupFrame = $self.parents(".popup:first");
            $popupFrame.css({ zIndex: window.popupZindex++ });
        }
        else {
            $popupFrame = $(popupFrameHtml.join(""));
            $popupFrame.css({ zIndex: window.popupZindex++ });
            $popupFrame.appendTo($("body"));
            $popupFrame.find(".popup_content").append($self);  //将内容附属到弹出层上
            $popupFrame.find(".exit, .btn_cancel>a").click(function () {

                if (!!_setting.onCancel($popupFrame)) {
                    $self.ClearInputs();
                    $self.ClearFloatDiv($self);
                    $self.CloseDiv();
                }
            });
            $popupFrame.find(".help").click(function () {
                //alert("TODO 显示帮助内容");
            });
            $popupFrame.find(".btn_submit>a").click(function () {
                if (!!_setting.onSubmit($popupFrame)) {
                    $self.ClearInputs();
                    $self.ClearFloatDiv($self);
                    $self.CloseDiv();
                }
            });
            $popupFrame.bind("submitContext", function () { $popupFrame.find(".btn_submit>a").click(); });
            $popupFrame.bind("cancelContext", function () { $popupFrame.find(".exit").click(); });
            $popupFrame.bind("hideContext", function () { $popupFrame.hide(); });
            $popupFrame.bind("showContext", function () { $popupFrame.show(); });
            // 设置高度
            if (_setting.scrollY) {
                $popupFrame.find(".popup_content").addClass("overflow");
            }
            // 设置宽度
            if (_setting.widthMode) {
                switch (_setting.widthMode) {
                    case "small":
                    case "thin": $popupFrame.css({ width: 400 }); break;
                    case "large":
                    case "wide": $popupFrame.css({ width: 1000 }); break;
                    default: break;
                }
            }

            var btnContainer = $popupFrame.find(".pop_btn");
            if (_setting.mode) {
                if (_setting.mode == "info") {
                    btnContainer.find(".btn_submit").remove();
                    btnContainer.find(".btn_cancel").remove();
                }
                else if (_setting.mode == "alert") {
                    btnContainer.find(".btn_cancel").remove();
                }
            }

            // 额外的自定义按钮
            if (_setting.btns && $.isArray(_setting.btns)) {
                $.each(_setting.btns, function (i, btnDesc) {
                    btnDesc.cssClass = btnDesc.cssClass || "";
                    var btn = $('<div class="btn01 btn_fr ' + btnDesc.cssClass + '"  ><a><span>' + btnDesc.name + '</span></a></div>');
                    if (btnDesc.onclick && $.isFunction(btnDesc.onclick)) {
                        btn.find("a").click(function () {
                            var r = btnDesc.onclick($popupFrame);
                            if (r == true) {
                                $self.CloseDiv();
                            }
                        });
                    }
                    btn.appendTo(btnContainer);
                });
            }
        }
        $popupFrame.data("level", _setting.level);//缓存当前级别，如果需要多次弹出， 则需要取此值。
        $popupFrame
            .jqDrag($popupFrame.find(".dnrHandler"), $self.ClearFloatDiv) // 位置可拖拽, 第2个参数表示拖动时要“清理”的动作
            .jqResize($popupFrame.find(".resizeHandler"))      // 右下角拖拉可Resize
            .find(".pop_txt_title").text(_setting.title);
        $self.data("mask", maskObj); //关联遮罩
        if (_setting.onInit && $.isFunction(_setting.onInit)) {
            _setting.onInit($popupFrame);
        }



        var MyDiv_w = $popupFrame.width();
        var MyDiv_h = $popupFrame.height();
        MyDiv_w = parseInt(MyDiv_w, 10);
        MyDiv_h = parseInt(MyDiv_h, 10);
        var width = $.PageSize().Width;
        var height = $.PageSize().Height;
        //var left = 0;// $.ScrollPosition().Left;
        //var top = 0; //$.ScrollPosition().Top;
        var left = $.ScrollPosition().Left;
        var top = $.ScrollPosition().Top;
        var Div_topposition = top + (height / 2) - (3 * MyDiv_h / 4) + _setting.level * 18;
        var Div_leftposition = left + (width / 2) - (MyDiv_w / 2) + _setting.level * 26
        //var Div_topposition = (height / 2) - (3 * MyDiv_h / 4) + _setting.level * 18;
        //var Div_leftposition = (width / 2) - (MyDiv_w / 2) + _setting.level * 26
        $popupFrame.css("left", Div_leftposition + "px");
        $popupFrame.css("top", Div_topposition + "px");
        if ($.browser.mozilla) {
            $popupFrame.show();
            return;
        }
        $popupFrame.fadeIn("fast");
        return $self;
    }
    , CloseDiv: function () {
        var $self = $(this);
        var mask = $self.data("mask")
        var $popupFrame = $self.parents(".popup");
        var destroy = true;
        if (destroy) {
            $popupFrame.remove();

        } else {

            if ($.browser.mozilla) {
                $popupFrame.hide();

            } else {
                $popupFrame.fadeOut("fast");
            }
        }


        //$("#maskDiv").data("divbox_selectlist").show();
        if (typeof (mask) != 'undefined') {
            if ($(".maskDiv").length == 1) { //由于是多层弹出， 只有在最后关闭的时候才重新显示滚动
                $("html").css("overflow", $("html").data("overflow"));
                window.scrollTo(mask.data("divbox_scrollleft"),
                                mask.data("divbox_scrolltop"));

                myDebugger.log("$self.mask top" + mask.data("divbox_scrolltop"));
                myDebugger.log("$self.mask left" + mask.data("divbox_scrollleft"));
            }
            mask.remove();
        }


        // $(this).appendTo($("body"));
        // $popupFrame.remove(); //删除外框
    }
    , ClearInputs: function () {
        var container = $(this);
        container.find("table[name='toEmpty']").find("tbody").empty();
        container.find("input:text, textarea, select").val("");
        container.find(":checkbox, :radio").attr("checked", "");


    }
    , ClearFloatDiv: function (handler) {
        //return; // 怀疑与473bug有关
        var context = $(handler).parents(".popup:first");
        if ($.fn.tipsy) {
            // tipsy
            context.find("[title]").tipsy("hide");
            context.find("[original-title]").tipsy("hide");
        }
        // autocomplete
        $(".ac_results").hide();
    }
});

$.extend(
{
    PageSize: function () {
        var width = 0;
        var height = 0;
        width = window.innerWidth != null ? window.innerWidth : document.documentElement && document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body != null ? document.body.clientWidth : null;
        height = window.innerHeight != null ? window.innerHeight : document.documentElement && document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body != null ? document.body.clientHeight : null;
        return { Width: width, Height: height };
    }
    , ScrollPosition: function () {
        var top = 0, left = 0;
        if ($.browser.mozilla) {
            top = window.pageYOffset;
            left = window.pageXOffset;

        }
        else if ($.browser.msie) {
            top = document.documentElement.scrollTop;
            left = document.documentElement.scrollLeft;

        }
        else if (document.body) {
            top = document.body.scrollTop;
            left = document.body.scrollLeft;
        }
        return { Top: top, Left: left };

    }
    , Popup: function (options) {

        if ($.type(options) === "string") {
            options = { url: options };
        }

        var _setting = {
            url: "",
            target: "",  //窗口的名称， 控制窗口的唯一
            width: 0,
            height: 0,
            sizeMode: "standard", //small: 300*200 / standard:800*600 / large: 1024*768
            isModelDialog: "",
            other: "",
            resizable: false
        };
        $.extend(_setting, options);



        var left = 140;
        var top = 20;
        var width = 0;
        var height = 0;
        switch (_setting.sizeMode) {
            case "small": width = 300; height = 200; break;
            case "standard": width = 800; height = 600; break;
            case "large": width = 1024; height = 768; break;
            default: width = 800; height = 600; break;
        }
        width = _setting.width || width;
        height = _setting.height || height;
        left = $.ScrollPosition().Left + $.PageSize().Width / 2 - width / 2;
        top = $.ScrollPosition().Top + $.PageSize().Height / 2 - height / 2;


        window.open(_setting.url,
            _setting.target || "",
            //_setting.target||_setting.url,
            "height={0},width={1},top={2},left={3},toolbar=no,menubar=no,scrollbars=yes,resizable={4},location=no, status=no".formatBy(height, width, top, left, _setting.resizable ? "yes" : "no")
        );
    }
});

$.fn.extend({
    // 适合通过data-field来收集属性; 
    // this 为单一jq对象, 表示context
    // 支持 input(text, checkbox, hidden), textarea, select
    // 以及表格和列表绑定的数组属性。 
    // 表格支持table 和 tbody; 列表支持ol, ul
    // 除递归元素外， 如果父元素有data-field, 则子元素加data-field无效
    // 9.24 新增 支持data-field缓存
    ContextSerialize: function () {

        var inputs = "input[type=text],input[type=checkbox],input[type=hidden],textarea,select".split(","); // 编辑
        var labels = "label,a,p,b,i,span,div,em,h1,h2,h3,dl,dd".split(","); //只读 , 注意, 不能加在tr, li这些遍历载体上
        var tableIter = "table,tbody".split(",");
        var listIter = "ul,ol".split(",");

        var result = {};
        var keyAttr = "data-field";
        var keyValAttr = "data-fieldvalue";  //适合labels
        var selectOptionSuffix = "_optionText"; //select 控件option 取文本时的属性后缀


        getProperties(this, result);

        return result;

        function getProperties(context, obj) {
            if (context.is(":disabled")) {
                return;
            }
            $.each(context.children(), function (i, child) {
                var child = $(child);
                if (child.is(":disabled")) {
                    return;
                }
                var tagName = child[0].tagName;
                // 表示获得属性
                var digged = false;

                // 是否有直接属性: 编辑
                $.each(inputs, function (j, selector) {
                    if (child.is(selector + "[" + keyAttr + "]")) {
                        if (child.is("select")) {
                            obj[child.attr(keyAttr) + selectOptionSuffix] = child.find(":selected").text();
                        } //如果是select ，则同时记录option text

                        var field = child.attr(keyAttr);
                        var v = (child.val() || child.data(field) || ""); //

                        obj[field] = child.is(":checkbox") ? child.is(":checked") : (getValue(child, v));

                        digged = true;
                        return false; // 找到则返回
                    }
                });

                // 是否有直接属性: 只读
                if (!digged) {
                    $.each(labels, function (j, selector) {
                        if (child.is(selector + "[" + keyAttr + "]")) {
                            var field = child.attr(keyAttr);
                            var v = (child.attr(keyValAttr) || child.data(field) || "");

                            //i和span不能用在父级标签 暂定
                            if (v === "" && tagName.toLowerCase() != "i" && tagName.toLowerCase() != "span") {
                                obj[field] = {};
                                getProperties(child, obj[field]);
                            }
                            else {
                                //if ($.type(v) == "string" && v !== "" && !isNaN(Number(v.replaceAll(",", "")))) v = Number(v.replaceAll(",", ""));  //转换为数值（替换掉千分号）
                                obj[field] = getValue(child, v);
                            }
                            digged = true;
                            return false; // 找到则返回
                        }
                    });
                }

                // 是否有表格递归属性
                if (!digged) {
                    $.each(tableIter, function (j, selector) {
                        if (child.is(":disabled") || child.attr("disabled")) {
                            return;
                        }
                        if (child.is(selector + "[" + keyAttr + "]")) {
                            obj[child.attr(keyAttr)] = [];

                            getSubInTable(child, obj[child.attr(keyAttr)]);
                            digged = true;
                            return false;
                        }
                    });
                }

                // 是否有列表递归属性
                if (!digged) {
                    $.each(listIter, function (j, selector) {
                        if (child.is(":disabled") || child.attr("disabled")) {
                            return;
                        }
                        if (child.is(selector + "[" + keyAttr + "]")) {
                            obj[child.attr(keyAttr)] = []; //构建数组
                            getSubInList(child, obj[child.attr(keyAttr)]); //添加子对象
                            digged = true;
                            return false;
                        }
                    });
                }

                if (!digged) {
                    getProperties(child, obj)
                }

                return;
            });


        }

        function getValue(sender, v) {
            if ($.type(v) == "number") return v;
            if ($.type(v) == "string") v = $.trim(v);
            var isNum = isNumType(sender);
            if (isNum) {
                v = v.replaceAll(",", "");   //替换掉千分号
                v = Number(v);
                if (isNaN(v)) {
                    v = 0;
                }
                return v;
            }
            else {
                return v;
            }
        }



        function isNumType(sender) {
            var type = sender.attr("data-fieldtype");
            if (!type) {
                return false;
            }
            type = type.toLowerCase();
            if ("n,num,number".split(",").indexOf(type) < 0) {
                return false;
            }
            return true;

        }
        function getSubInTable(context, array) {

            getSubs(context, array, ">tr, >tbody>tr");
        }
        function getSubInList(context, array) {

            getSubs(context, array, "li");
        }
        function getSubs(context, array, subTag) {

            context.find(subTag).each(function (i, li) {
                li = $(li);
                if (li.is(":disabled") || li.attr("disabled")) {
                    return;
                }
                var sub = {};
                getProperties(li, sub);
                if ($.isEmptyObject(sub)) {
                    return;
                }
                array.push(sub);
            });



        }
    }

    // 通过ruleModel绑定的方法来给Context中的输入内容设置错误提示tips
    // 绑定的方法中带一个参数context, 表示上下文； 方法中的this表示输入控件
    // gravity 表示tooltip方向， 默认为向下； 如果是string类型， 则为默认方向 ； 如果是对象， 则表示对指定控件分别设置
    , ContextValidation: function (ruleModel, gravity) {
        var inputs = "input[type=text],input[type=checkbox],textarea,select".split(","); // 输入框
        var $context = $(this);

        if (arguments[0] == false || $.type(arguments[0]) == "undefined" || arguments[0] == null) {
            $context.find("input[data-field],textarea[data-field]").removeClass("validate_error");
            $context.find("[data-field]").each(function () { $(this).tipsy("hide") });
            return;
        }

        if ($.isPlainObject(ruleModel) == false) {
            ruleModel = {};
        }

        var result = true;

        // 
        if ($.type(gravity) == "string") {
            gravity = { base: gravity };
        }
        else { gravity = $.extend({ base: "w" }, gravity); }

        for (var property in ruleModel) {
            if (ruleModel.hasOwnProperty(property)) {

                var gv = gravity[property] || gravity.base; //设置tooltip方向

                var sender = $context.FindFieldCtrl(property);
                if (sender.length == 0) {
                    continue;
                }


                //var val = $context.val() || $context.attr["data-fieldvalue"] || "";
                if ($.isFunction(ruleModel[property])) {
                    var func = ruleModel[property];
                    if (!controlValidation($context, sender, func, gv)) { result = false; /**/ };
                }
                else if ($.isPlainObject(ruleModel[property])) { // 多个方法
                    for (var funcName in ruleModel[property]) {
                        var func = ruleModel[property][funcName];
                        if ($.isFunction(func)) {
                            if (!controlValidation($context, sender, func, gv)) { result = false; };//验证， 一旦有错， 退出
                        }
                    }
                }
                else if ($.isArray(ruleModel[property])) { // 多个方法数组
                    for (var i = 0; i < ruleModel[property].length; i++) {
                        var func = ruleModel[property][i];
                        if ($.isFunction(func)) {
                            if (!controlValidation($context, sender, func, gv)) { result = false; }; //验证， 一旦有错， 退出
                        }
                    }
                }
                // clear
                sender.each(function () {
                    $(this).data("errMsgList", null);
                });
            }
        }

        return result;

        function controlValidation(context, controls, validateFunc, gravity) {
            // var val = control.val() || control.attr["data-fieldvalue"] || "";
            var result = true;
            $(controls).each(function () {
                var control = $(this);
                $.each(inputs, function (i, selector) {
                    if (control.is(selector)) {
                        var errMsg = validateFunc.apply(control, [context]);
                        var tips = control.data("errMsgList");
                        if ($.isArray(tips) == false) {
                            tips = [];
                        }
                        if ($.type(errMsg) == "string" || errMsg != true) {
                            tips.push(errMsg);
                            control.data("errMsgList", tips);
                        }
                        //console.log(control.val() + tips);
                        if (tips.length > 0) { result = false; }
                        else {
                            $(control).tipsy("disable");
                            $(control).removeClass("validate_error");
                        }



                        if (tips.length > 0) { //显示tipsy 做成异步
                            var timer = control.data("tipsTimer");
                            if (timer) {
                                clearTimeout(timer);
                            }

                            timer = setTimeout(function () {
                                var tipsHtml = tips.join("</br>");
                                $(control).addClass("validate_error");
                                $(control).attr("title", tipsHtml)
                                    .tipsy({
                                        theme: "warnTheme",
                                        opacity: 0.95,
                                        trigger: "focus",
                                        html: true,
                                        gravity: gravity || "n", //w,n,s,e...
                                        fade: false
                                    })
                                    .tipsy("enable");
                                $(control).tipsy("show");

                                setTimeout(function () { $(control).tipsy("hide") }, 2000, null);

                            }, 100, null);
                        }

                        control.data("tipsTimer", timer);
                    }
                });
            });

            return result;
        }
    }

    // 目前暂未考虑多级的情况
    , FindFieldValue: function (field, defaultValue) {
        var context = $(this);
        var ctrl = context.find("[data-field='" + field + "']:first");
        if (ctrl.length == 0) {
            return defaultValue || "";
        }
        var result = ctrl.attr("data-fieldvalue") || "";
        return result;
    }

    // 目前暂未考虑多级的情况
    , FindFieldCtrl: function (field) {
        var context = $(this);
        var ctrl = context.find("[data-field='" + field + "']");
        return ctrl;
    }
    , SetFieldCtrlValue: function (field, value) {
        var inputs = "input[type=text],input[type=checkbox],input[type=hidden],textarea,select".split(","); // 编辑
        var labels = "label,a,p,b,i,span,div,em,h1,h2,h3,dl,dd".split(","); //只读 , 注意, 不能加在tr, li这些遍历载体上

        var context = $(this);
        var ctrl = context.find("[data-field='" + field + "']");

        return $.each(ctrl, function (i, c) {
            c = $(c);
            if (isInput(c)) {
                c.val(value);
            }
            if (isLabel(c)) {
                c.attr("data-fieldvalue", value);
            }
        });

        function isInput(c) {
            for (var i = 0; i < inputs.length; i++) {
                if (c.is(inputs[i])) { return true; }
            }
            return false;
        }


        function isLabel(c) {
            for (var i = 0; i < labels.length; i++) {
                if (c.is(labels[i])) { return true; }
            }
            return false;
        }
    }
});


(function ($) {

    $.fn.extend({
        // 表格折叠（树形）
        tableTree: function (options) {

            var _setting = {
                // 展开触发器
                expandedSelector: "i.icon-expanded",
                // 收缩触发器
                collapsedSelector: "i.icon-collapsed",
                // 缩进样式前缀
                levelClassPrefix: ".tr_level",
                // 收起操作
                collapse: function (row) { row.find("i.icon-expanded").removeClass("icon-expanded").addClass("icon-collapsed"); },
                // 展开操作
                expand: function (row) { row.find("i.icon-collapsed").removeClass("icon-collapsed").addClass("icon-expanded"); },
                // 获得行级别的方法
                getLevel: function (row) { return row.FindFieldValue("Level").toInt(); }, //获得row的级别
                // 初始化
                onInit: function () {
                    $(this).find(".tr_level1").show();
                },
                other: ""
            };

            $.extend(_setting, options);

            var context = $(this);
            var rows = context.find("tbody>tr");
            rows.find(_setting.collapsedSelector + "," + _setting.expandedSelector).bind("click", function () {
                var row = $(this).parents("tr:first");
                var level = _setting.getLevel(row);
                var nextRow = row.next("tr");
                if (nextRow.length == 0) {
                    return;
                }
                var nextRowLevel = _setting.getLevel(nextRow);

                if (nextRowLevel <= level) {
                    return;
                }

                var isCollapsed = nextRow.is(":hidden");

                var levelClass = _setting.levelClassPrefix + level;
                var upLevels = $.grep([1, 2, 3, 4, 5, 6], function (n, i) { return n <= level });
                var selector = $.map(upLevels, function (n) { return _setting.levelClassPrefix + n; }).join(",");

                if (isCollapsed) {
                    var subLevel = level + 1;
                    row.trigger("expand");
                    row.nextUntil(selector).filter(_setting.levelClassPrefix + subLevel)
                        .trigger("collapse")
                        .show();

                }
                else {
                    row.trigger("collapse")
                        .nextUntil(selector).hide();
                }

            });
            rows.bind("collapse", function () {
                var row = $(this);
                _setting.collapse(row);
            })
            .bind("expand", function () {
                var row = $(this);
                _setting.expand(row);
            })

            if (_setting.onInit) {
                _setting.onInit();
            }
        }

        // 客户端分页
        , pagination: function (options) {

            var context = $(this);
            var _setting = {
                current: 1, // 当前页
                pageSize: 10, // 每页多少行
                pageCount: 1, // 共多少页
                totalCount: 0, // 共多少条数据
                navTo: function (pageIndex, pageSize) { }, //跳转到第几页， 需要使用者自己实现： 1 获得绑定数据，刷新表格 2 刷新分页器
                //onPaging: function (opt) {
                //    var _default = {
                //        data: null,
                //        tmplHtml: "",

                //    };
                //},
                cols: "20",
                other: null
            };

            $.extend(_setting, options);
            if (_setting.pageSize > 0 && _setting.totalCount > 0) {
                _setting.pageCount = Math.ceil(_setting.totalCount / _setting.pageSize);
            }
            else {
                _setting.pageCount = 0;
                _setting.current = 0;
            }
            // class: pagination, pager, first, prev, next, last, currentPage, totalPage, go, totalItemCount, w20
            // variance: ${current}, ${pageCount}, ${totalCount}
            var pageTmpl = [
                "<tr   class='pagination'>",
                "    <td colspan='" + _setting.cols + "'>",
                "        <span>",
                "            <a href='javascript:void(0)' class='first'>首页</a>",
                "            <a href='javascript:void(0)' class='prev'>上一页</a>",
                "            <a href='javascript:void(0)' class='next'>下一页</a>",
                "            <a href='javascript:void(0)' class='last'>尾页</a>",
                "            <span>当前页：<span class='currentPage'>${current}</span>/<span class='totalPage'>${pageCount}</span></span>",
                "            <span>&nbsp;&nbsp;&nbsp;&nbsp;跳转至：第</span>",
                "            <input class='numBox targetIndex numOnly' name='targetIndex' type='text' value='${current}' maxlength=3/>",
                "            <span>页</span>",
                "            <input class='go' type='button' value='GO' />",
                "            <span>&nbsp;&nbsp;&nbsp;&nbsp;共有<span class='totalItemCount'>${totalCount}</span>条数据</span>",
                "        </span>",
                "    </td>",
                "</tr>"
            ];
            var pageTmplMarkups = pageTmpl.join("");
            var pageTemplate = $.template("pageTemplate", pageTmplMarkups);
            var pager = $.tmpl("pageTemplate", _setting); //分页控件
            pager.appendTo(context);

            //分面样式修改
            if (_setting.current == 1) {
                pager.find("a.first").attr("disabled", true);
                pager.find("a.prev").attr("disabled", true);
            } else {
                pager.find("a.first").attr("disabled", false);
                pager.find("a.prev").attr("disabled", false);
            }

            if (_setting.current == _setting.pageCount) {
                pager.find("a.next").attr("disabled", true);
                pager.find("a.last").attr("disabled", true);

            } else {
                pager.find("a.next").attr("disabled", false);
                pager.find("a.last").attr("disabled", false);
            }
            //分页操作事件
            pager.find("a.first")
                .attr("title", "快捷键 HOME")
                .click(function () {
                    if (_setting.totalCount == 0) return;
                    _setting.navTo(1, _setting.pageSize);
                    //var btn = context.find("a.first");
                    //if (btn.length > 0) {
                    //    btn.focus();
                    //}

                });
            pager.find("a.prev")
                .attr("title", "快捷键 <-")
                .click(function () {
                    if (_setting.current > 1)
                        _setting.navTo(_setting.current - 1, _setting.pageSize);
                    //var btn = context.find("a.prev");
                    //if (btn.length > 0) {
                    //    btn.focus();
                    //}

                });
            pager.find("a.next")
                .attr("title", "快捷键 ->")
                .click(function () {
                    if (_setting.current + 1 <= _setting.pageCount)
                        _setting.navTo(_setting.current + 1, _setting.pageSize);
                    //var btn = context.find("a.next");
                    //if (btn.length > 0) {
                    //    btn.focus();
                    //}
                });
            pager.find("a.last")
                .attr("title", "快捷键 END")
                .click(function () {
                    _setting.navTo(_setting.pageCount, _setting.pageSize);
                    //var btn = context.find("a.last");
                    //if (btn.length > 0) {
                    //    btn.focus();
                    //}
                });
            pager.find("input.targetIndex").keyup(function () {
                if (event.keyCode == 13) { pager.find("input.go").click(); }
                var input = context.find("input.targetIndex");
                if (input.length > 0) {
                    input.focus();
                }

            });
            pager.find("input.go").click(function () {
                var targetIndex = pager.find("input.targetIndex").val().toInt();
                if (targetIndex < 1) {
                    targetIndex = 1;
                }
                else if (targetIndex > _setting.pageCount) {
                    targetIndex = _setting.pageCount;
                }
                _setting.navTo(targetIndex, _setting.pageSize);

                //var input = context.find("input.targetIndex");
                //if (input.length > 0) {
                //    input.focus();
                //}
            });
            pager.keyup(function () {
                //  alert(event.keyCode);
                switch (event.keyCode) {
                    case 33: case 37: pager.find("a.prev").click(); break;/*page up*/
                    case 34: case 39: pager.find("a.next").click(); break;/*page down*/
                    case 35: pager.find("a.last").click(); break;/*end*/
                    case 36: pager.find("a.first").click(); break; /*home*/
                    default: break;
                }
                // alert(context.find("input.targetIndex").length);
                context.find("input.targetIndex").click();
                event.returnValue = false;
            })
        }


    });
})(jQuery);

// 自定义log记录器
var myDebugger = {
    log: function (str) {
        return;
        if (typeof (console) == "undefined") {
            return;
        }
        console.log(str);

    },

    warn: function (str) {
        if (typeof (console) == "undefined") {
            return;
        }
        console.warn(str);
    }
}

// 观察者模式

//var publisher = {
//    subscribers: {
//        any: [] // event type: subscribers
//    },
//    subscribe: function (fn, type) {
//        type = type || 'any';
//        if (typeof this.subscribers[type] === "undefined") {
//            this.subscribers[type] = [];
//        }
//        this.subscribers[type].push(fn);
//    },
//    unsubscribe: function (fn, type) {
//        this.visitSubscribers('unsubscribe', fn, type);
//    },
//    publish: function (publication, type) {
//        this.visitSubscribers('publish', publication, type);
//    },
//    visitSubscribers: function (action, arg, type) {
//        var pubtype = type || 'any',
//            subscribers = this.subscribers[pubtype],
//            i,
//            max = subscribers.length;

//        for (i = 0; i < max; i += 1) {
//            if (action === 'publish') {
//                subscribers[i](arg);
//            } else {
//                if (subscribers[i] === arg) {
//                    subscribers.splice(i, 1);
//                }
//            }
//        }
//    }
//};


$.extend({
    // 将数组转换成树  
    /* 
    * 注意， 数组中的对象必须具备levelTag指定的属性来表示层级， 如"Level", 且为数值类型
    *
    */
    ArrayToTree: function (array, levelTag) {
        var result = array[0];

        var last = array[0];
        var parent = last;
        for (var i = 1; i < array.length; i++) {
            var current = array[i];
            if (last[levelTag] < current[levelTag]) {
                parent = last;
            } else if (last[levelTag] > current[levelTag]) {
                parent = getParent(last, current[levelTag] - 1);
            }
            addSub(parent, current);
            last = current;
        }

        return result;

        // 添加树的子节点
        function addSub(p, c) {
            if (!p.children) {
                p.children = new Array();
            }
            p.children.push(c);
            c.parent = p;
        }

        // 获得指定级别节点的上级
        function getParent(c, level) {
            if (c[levelTag] == level) {
                return c;
            }
            return getParent(c.parent, level);
        }

    },
    // 将树节点转换为数组
    /*
    * treeNode, 表示树的根节点
    */
    TreeToArray: function (treeNode, tags) {

        var _tags = {
            level: "Level",
            subs: "SubItems"
        };
        $.extend(_tags, tags);

        var result = [];
        popTree(treeNode, 1);

        // 把树节点添加到数组中（深度优先遍历， 递归）
        function popTree(treeNode, level) {
            if (!treeNode) {
                return;
            }
            var i;

            treeNode[_tags.level] = level; // 增加level属性
            result.push(treeNode);

            var subNodes = treeNode[_tags.subs];

            if (!!subNodes && subNodes.length > 0) {
                for (i = 0; i < subNodes.length; i++) {
                    popTree(subNodes[i], level + 1);
                }
            }

        }
        return result;
    }



});

//以下实现拖拽与缩放
// h 表示句柄
// c 表示拖拽的时候清理浮动菜单的方法
// k 表示命令， 
(function ($) {
    $.fn.jqDrag = function (h, c) { return i(this, h, c, 'd'); };
    $.fn.jqResize = function (h) { return i(this, h, 'r'); };
    $.jqDnR = {
        dnr: {}, e: 0,
        drag: function (v) {

            if (M.k == 'd') E.css({ left: M.X + v.pageX - M.pX, top: M.Y + v.pageY - M.pY });
            else {
                E.css({ width: Math.max(v.pageX - M.pX + M.W, 0), height: Math.max(v.pageY - M.pY + M.H, 0) });
            }
            return false;
        },
        stop: function () { E.css('opacity', M.o); $("body").unbind('mousemove', J.drag).unbind('mouseup', J.stop); }
    };
    var J = $.jqDnR, M = J.dnr, E = J.e,
    i = function (e, h, c, k) {
        return e.each(function () {
            h = (h) ? $(h, e) : e;
            h.bind('mousedown', { e: e, k: k }, function (v) {
                if (c) { c(h); }
                var d = v.data, p = {}; E = d.e;
                // attempt utilization of dimensions plugin to fix IE issues
                if (E.css('position') != 'relative') { try { E.position(p); } catch (e) { } }
                M = { X: p.left || f('left') || 0, Y: p.top || f('top') || 0, W: f('width') || E[0].scrollWidth || 0, H: f('height') || E[0].scrollHeight || 0, pX: v.pageX, pY: v.pageY, k: d.k, o: E.css('opacity') };
                E.css({ opacity: 0.8 }); $("body").mousemove($.jqDnR.drag).mouseup($.jqDnR.stop);
                return false;
            });
        });
    },
    f = function (k) { return parseInt(E.css(k)) || false; };
})(jQuery);

(function ($) {

    // 分段(section)延迟处理数组的元素， 通常用来加载大量数据并将数据渲染到页面上的时候
    $.extend({
        sequentlyExecute: function (options) {

            var _setting = {
                array: [],
                arrayHandler: function (arr) { },  // 数组处理器
                sectionSize: 100,
                timeout: 200,//ms     每个分组的间隔时间
                initTimeout: 0, //ms , 第一次执行的延迟时间
                onStart: function () {
                    if ($.blockUI) {
                        $.blockUI({ message: "<div style='width:200px'><img src='../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据显示中...</span></div>" });
                    }
                }, //在开始时
                onCompleted: function () { if ($.unblockUI) { $.unblockUI(); } }, //在全部结束时调用， 可以用来提示
                other: null
            }
            $.extend(_setting, options);

            var section = 0;

            setTimeout(function () {
                if ($.type(_setting.onStart) == "function") {
                    _setting.onStart();
                }
            }, _setting.initTimeout, null);

            setTimeout(function () {
                var end = true;
                var offset = section * _setting.sectionSize;
                var subArray = _setting.array.slice(offset, offset + _setting.sectionSize);
                if (subArray.length > 0) {
                    _setting.arrayHandler(subArray);

                    end = (subArray.length < _setting.sectionSize);

                    if (!end) {
                        section++;
                        setTimeout(arguments.callee, _setting.timeout, null)
                    }
                }

                if (end) {
                    if ($.type(_setting.onCompleted) == "function") {
                        _setting.onCompleted();
                    }
                }

            }, _setting.initTimeout, null);
        }
    });
})(jQuery);

// 常用的验证方法的
var validator = {};
var $v = validator; // 值validator
var $jqV = {};  //控件validator
(function ($) {
    var vor = validator;
    $.extend(vor, {
        //是否是必选项
        isRequired: function (s) {
            if ($.type(s) == "undefined" || s == null) {
                return false;
            }
            if ($.type(s) == "string" && $.trim(s).length == 0) { return false; }

            return true;
        },

        //最大长度
        maxLength: function (s, l) {
            if ($.type(s) != "string") {
                return false;
            }

            if ($.trim(s).length > l) {
                return false;
            }
            return true;

        },

        //最小长度
        minLength: function (s, l) {
            if ($.type(s) != "string") {
                return false;
            }
            if ($.trim(s).length < l) {
                return false;
            }
            return true;

        },

        //邮箱验证
        isEmail: function (s) {
            var re = /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$/;
            if ($.type(s) != "string") {
                return false;
            }
            if (re.test($.trim(s))) {
                return true;
            }
            return false;

        },

        isNumber: function (s) {
            var re = /^(-[0-9]|[0-9])[0-9]{0,}((\.){0,1}[0-9]+|[0-9]*)$/;
            if (re.test($.trim(s))) {
                return true;
            } else {
                return false;
            }
        },

        isMobile: function (s) {
            var re = /^1[3|4|5|8][0-9]\d{8}$/;
            if (re.test($.trim(s))) {
                return true;
            } else {
                return false;
            }
        },

        //isFax: function (s) { return false; },

        isSame: function (s1, s2) {
            if (s1 === s2) {
                return true;
            } else {
                return false;
            }
        },

        isExist: function (s, serverCheckFunc) {

            return serverCheckFunc(s);
        },

        isDigitAlphaBeta: function (s) {
            var re = /^[a-zA-Z_][0-9a-zA-Z_]*$/g;
            if (re.test($.trim(s))) {
                return true;
            } else {
                return false;
            }
        },

        characterOnly: function (s) {
            //var re = /^[^0-9][\u4e00-\u9fa50-9a-zA-Z_。]*$/;
            //＋ － × ÷ （）; + - * / (), 
            var re = /^[^0-9][^＋－×÷（）;\+\-\*\/\(\),]*$/;
            if (re.test($.trim(s))) {
                return true;
            } else {
                return false;
            }
        },

        checkKeyWords: function (s) {
            var keys = '&,#,<,>,$,+,!,@,#,%,^,&,*,?,~'.split(','); var bool = true;
            $.each(keys, function (i, item) {
                if (s.indexOf(item) >= 0) {
                    bool = false;
                }
            });
            return bool;
        },

        //notScript: function (s) {
        //    var re = /^[a-zA-Z_][0-9a-zA-Z_]*$/g;
        //    if (re.test($.trim(s))) {
        //        return true;
        //    } else {
        //        return false;
        //    }
        //},
        other: function (s) { return false; }

    });

    $.extend($jqV, {
        isRequired: function () { if (!$(this).length) return true; var value = $(this).val(); if (!$v.isRequired(value)) { return "必填项！" } return true; },
        minLength2: function () { var value = $(this).val(); if (!$v.minLength(value, 2)) { return "请输入至少2个字符！" } return true; },
        minLength5: function () { var value = $(this).val(); if (!$v.minLength(value, 5)) { return "请输入至少5个字符！" } return true; },
        maxLength20: function () { var value = $(this).val(); if (!$v.maxLength(value, 20)) { return "请输入字符不要超过20个！" } return true; },
        minLength30: function () { var value = $(this).val(); if (!$v.minLength(value, 30)) { return "请输入至少30个字符！" } return true; },
        minLength100: function () { var value = $(this).val(); if (!$v.minLength(value, 100)) { return "请输入至少100个字符！" } return true; },
        minLength200: function () { var value = $(this).val(); if (!$v.minLength(value, 200)) { return "请输入至少200个字符！" } return true; },
        minLength256: function () { var value = $(this).val(); if (!$v.minLength(value, 256)) { return "请输入至少256个字符！" } return true; },
        maxLength15: function () { var value = $(this).val(); if (!$v.maxLength(value, 15)) { return "请输入字符不超过15个！" } return true; },
        maxLength500: function () { var value = $(this).val(); if (!$v.maxLength(value, 500)) { return "请输入字符不要超过500个！" } return true; },
        isNumber: function () { var value = $(this).val(); if (!$v.isNumber(value)) { return "请输入数字！" } return true; },
        isPositive: function () { var value = $(this).val(); if (value.toFloat() <= 0) { return "请输入正数！" } return true; },
        isPositiveOrZero: function () { var value = $(this).val(); if (!value.toFloat() < 0) { return "请输入非负数！" } return true; },
        checkKeyWords: function () { var value = $(this).val(); if (!$v.checkKeyWords(value)) { return "请勿输入敏感字符！" } return true; },
        characterOnly: function () { var value = $(this).val(); if (!$v.characterOnly(value)) { return "1 指标名称不能包含英文及中文的＋ － × ÷ （）; + - * / (), 以及$符号！<br/>2 不能是数字开头！<br/>3 不能为空！" } return true; }
    });
})(jQuery);
