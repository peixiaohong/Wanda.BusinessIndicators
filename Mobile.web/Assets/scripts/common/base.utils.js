

function GUID() {
    this.date = new Date();

    /* 判断是否初始化过，如果初始化过以下代码，则以下代码将不再执行，实际中只执行一次 */
    if (typeof this.newGUID != 'function') {

        /* 生成GUID码 */
        GUID.prototype.newGUID = function () {
            this.date = new Date();
            var guidStr = '';
            sexadecimalDate = this.hexadecimal(this.getGUIDDate(), 16);
            sexadecimalTime = this.hexadecimal(this.getGUIDTime(), 16);
            for (var i = 0; i < 9; i++) {
                guidStr += Math.floor(Math.random() * 16).toString(16);
            }
            guidStr += sexadecimalDate;
            guidStr += sexadecimalTime;
            while (guidStr.length < 32) {
                guidStr += Math.floor(Math.random() * 16).toString(16);
            }
            return this.formatGUID(guidStr);
        }

        /*
         * 功能：获取当前日期的GUID格式，即8位数的日期：19700101
         * 返回值：返回GUID日期格式的字条串
         */
        GUID.prototype.getGUIDDate = function () {
            return this.date.getFullYear() + this.addZero(this.date.getMonth() + 1) + this.addZero(this.date.getDay());
        }

        /*
         * 功能：获取当前时间的GUID格式，即8位数的时间，包括毫秒，毫秒为2位数：12300933
         * 返回值：返回GUID日期格式的字条串
         */
        GUID.prototype.getGUIDTime = function () {
            return this.addZero(this.date.getHours()) + this.addZero(this.date.getMinutes()) + this.addZero(this.date.getSeconds()) + this.addZero(parseInt(this.date.getMilliseconds() / 10));
        }

        /*
        * 功能: 为一位数的正整数前面添加0，如果是可以转成非NaN数字的字符串也可以实现
         * 参数: 参数表示准备再前面添加0的数字或可以转换成数字的字符串
         * 返回值: 如果符合条件，返回添加0后的字条串类型，否则返回自身的字符串
         */
        GUID.prototype.addZero = function (num) {
            if (Number(num).toString() != 'NaN' && num >= 0 && num < 10) {
                return '0' + Math.floor(num);
            } else {
                return num.toString();
            }
        }

        /* 
         * 功能：将y进制的数值，转换为x进制的数值
         * 参数：第1个参数表示欲转换的数值；第2个参数表示欲转换的进制；第3个参数可选，表示当前的进制数，如不写则为10
         * 返回值：返回转换后的字符串
         */
        GUID.prototype.hexadecimal = function (num, x, y) {
            if (y != undefined) {
                return parseInt(num.toString(), y).toString(x);
            } else {
                return parseInt(num.toString()).toString(x);
            }
        }

        /*
         * 功能：格式化32位的字符串为GUID模式的字符串
         * 参数：第1个参数表示32位的字符串
         * 返回值：标准GUID格式的字符串
         */
        GUID.prototype.formatGUID = function (guidStr) {
            var str1 = guidStr.slice(0, 8) + '-',
                str2 = guidStr.slice(8, 12) + '-',
                str3 = guidStr.slice(12, 16) + '-',
                str4 = guidStr.slice(16, 20) + '-',
                str5 = guidStr.slice(20);
            return str1 + str2 + str3 + str4 + str5;
        }
    }
}
var utils = {
    isLoading: false,
    ChangeDateFormatToMinute: function (dateVal) {
        if (dateVal == null) {
            return "-";
        }
        try {
            //var date = new Date(parseInt(dateVal.replace("/Date(", "").replace(")/", ""), 10));
            var date = new Date(dateVal);
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var day = date.getDate();
            var hour = date.getHours();
            var minute = date.getMinutes();
            var second = date.getSeconds();

            return "{0}-{1}-{2} {3}:{4}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute));

        } catch (e) {
            return "";
        }

        function doubleDigit(n) { return n < 10 ? "0" + n : "" + n; }
    },
    DateDiffString: function (totalSecond) {
        if (totalSecond == 0) {
            return "-";
        }
        else if (totalSecond < 60) {
            return totalSecond + "秒";
        }
        else if (totalSecond < 60 * 60) {
            return parseInt(totalSecond / 60) + "分钟";
        }
        else if (totalSecond < 60 * 60 * 24) {
            return parseInt(totalSecond / 60 / 60) + "小时";
        }
        else {
            return parseInt(totalSecond / 60 / 60 / 24) + "天";
        }
    },
    ajax: function (options) {
        var setting = {
            async: true,
            type: 'GET',
            url: null,
            args: {},
            success: null,
            error: null
        };
        $.extend(setting, options);
        $.ajax({
            async: setting.async,
            type: setting.type,
            dataType: 'json',
            cache: false,
            url: setting.url,
            data: setting.args,
            success: function (result) {
                utils.unblock();
                if (setting.success != null && typeof setting.success === 'function') {
                    setting.success(result);
                }
            },
            error: function (XMLHttpRequest, textStatus) {
                utils.unblock();
                utils.alertMessage('提示:' + XMLHttpRequest.responseText, function () {
                    if (XMLHttpRequest.status === 403) {
                        //window.location.href = 'wd_sso_logout?action=exit&url=' + encodeURIComponent(window.location.href);
                        window.location.href = '/Public/Login.aspx?returnUrl=' + encodeURIComponent(window.location.href);
                    }
                });
            },
            timeout: function () {
                utils.unblock();
                utils.alertMessage('请求超时');
            },
            beforeSend: function () {
                utils.block();
            }
        });
    },
    postToWin: function (options) {
        var url = options.url;
        var args = options.args;
        var params = $.param(args).split('&');
        var temp_form = document.createElement("form");
        temp_form.action = url;
        temp_form.target = "_blank";
        temp_form.method = "post";
        temp_form.style.display = "none";
        $.each(params, function (i, d) {
            var pairs = d.split('=');
            var opt = document.createElement("textarea");
            opt.name = decodeURIComponent(pairs[0]);
            opt.value = decodeURIComponent(pairs[1]);
            temp_form.appendChild(opt);
        });
        document.body.appendChild(temp_form);
        temp_form.submit();
    },
    unblock: function () {
        $.unblockUI();
    },
    block: function () {
        $.blockUI();
    },
    log: function (msg) {
        if (window.console && console.log) {
            console.log(msg);
        }
    },
    loadTmpl: function (url, selector) {
        var result = null;
        var container = $("#tmplContainer__");
        if (container.length == 0) {
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
    },
    ToDayWeekString: function (dateVal) {
        var date = new Date(dateVal);
        return "星期" + "日一二三四五六".charAt(date.getDay());
    },
    ChangeDateFormat: function (dateVal, displayTime) {
        if (dateVal == null) {
            return "- -";
        }
        try {
            //var date = new Date(parseInt(dateVal.replace("/Date(", "").replace(")/", ""), 10));
            var date = new Date(dateVal);
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
    },
    //通过js获取queryString中的指定值
    getQueryString: function (name) {
        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    },
    //弹窗提醒,参数1：提醒消息 参数2：回调函数（无参数）
    alertMessage: function (message, callback) {
        if ($(".weui-mask") && $(".vue-message")) {
            $(".weui-mask").remove();
            $(".vue-message").remove();
        }
        var $msg = $("<div class='weui-mask'></div><div class='vue-message'><div class='dialog__hd'><span class='dialog__title'>"
            + "<img src='../../Assets/images/action-icon.png' width='18'>  提示</span>"
            + "<a href='javascript:;' class='btn_close'><img src='../../Assets/images/close-icon.png' width= '15'></a>"
            + "</div><div class='weui-dialog__bd dialog__bd' ><div>" + message
            + "</div></div ><div class='weui-dialog__ft dialog__ft'><a href='javascript:;' class='weui-dialog__btn weui-dialog__btn_primary'>确定</a></div></div>");
        $msg.fadeIn(1000, function () {
            if (callback != undefined && callback != null && typeof callback == 'function') {
                callback();
            }
            $(".weui-dialog__btn").click(function () {
                if (message == "提示:该流程已作废") {
                    setTarget();
                } else {
                    $msg.remove();
                }
            });
            $(".btn_close").click(function () {
                if (message == "提示:该流程已作废") {
                    setTarget();
                } else {
                    $msg.remove();
                }
            })
        });
        $("body").append($msg);
        function setTarget() {
            var isMobile = utils.mobileBrower();
            var isFromWeb = utils.getQueryString("IsFromWeb");
            if (isMobile) {
                $msg.remove();
                if (isFromWeb && isFromWeb > 0) {
                    window.location.href = '/Application/Task/TaskList.aspx';
                }
                else {
                    window.location.href = '/todoListMobile.html';
                }
            } else {
                $msg.remove();
                window.close();
            }
        }

    },
    //确认弹窗提醒,参数1：提醒消息 参数2：回调函数（参数 bool,是否确认）
    confirmMessage: function (message, callback) {
        var ret = confirm(message);
        if (callback != undefined && callback != null && typeof callback == 'function') {
            callback(ret);
        }
    },
    //判断当前是pc还是手机端
    mobileBrower: function () {
        var u = navigator.userAgent;
        return !!u.match(/AppleWebKit.*Mobile.*/);//是否为移动终端
    },
    // 初始指标样式
    initTarget: function (main, content, name, allow, length) {
        var elMain = $(".target-main");
        var elContent = $(".target-content");
        var width = $(main).width();
        var rate = "";
        if (length == 2) {
            rate = (1 / 2).toFixed(2);
        } else if (length == 3) {
            rate = (1 / 3).toFixed(2);
        } else if (length >= 4) {
            rate = (1 / 4).toFixed(2);
        }
        var tdWidth = width * rate;
        var tableWidth = tdWidth * length;
        $(main).find("th").css("width", tdWidth);
        $(main).find("td").css("width", tdWidth);
        $(content).css("width", tableWidth);
        var tdHeight = $(content).find("thead").height();
        $(name).find("th").css("height", tdHeight);
        var top = tdHeight / 2 - $(allow).height() / 2;
        $(allow).css("top", top);
        var num = 0;
        var marginLeft = 0;
        $(allow).off("click").on("click", function () {
            var numLength = parseInt((length - 1) / 3);
            var numRem = (length - 1) % 3
            var dataAllow = $(allow).attr("data-allow");
            if (dataAllow == "right") {
                num++;
            } else if (dataAllow == "left") {
                num--;
                if (num <= 0) {
                    num = 0;
                    $(allow).attr({
                        "src": "../Assets/images/arrow-right.png",
                        "data-allow": "right"
                    });
                }
            }
            if (num < numLength) {
                marginLeft = - num * 3 * tdWidth + "px";
                $(content).animate({ "margin-left": marginLeft }, 300);
            } else if (num == numLength) {
                marginLeft = -((numLength - 1) * 3 * tdWidth + numRem * tdWidth) + "px"
                $(content).animate({ "margin-left": marginLeft }, 300, function () {
                    if (dataAllow == "right") {
                        $(allow).attr({
                            "src": "../Assets/images/arrow-left.png",
                            "data-allow": "left"
                        });
                    }                 
                });
            }
        })
    }
};
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
var commons = {
    OpenWindow: function (url) {
        if (utils.mobileBrower()) {
            window.location.href = url;
        } else {
            window.open(url);
        }
    },
    OpenOWAPreview: function (businessId) {
        var url = '/Application/OWA/Preview.aspx?BusinessId=' + businessId;
        commons.OpenWindow(url);
        //window.location.href = '/Application/OWA/Preview.aspx?BusinessId=' + businessId;
    },
    DeleteAttachment: function (attachmentId, callback) {
        var url = api_url + 'api/attachments/delete/' + attachmentId;
        utils.ajax({
            type: 'DELETE',
            url: url,
            success: function (data) {
                if (callback != null && typeof callback === 'function') {
                    callback();
                }
            }
        });
    },
    DownloadAttachment: function (attachmentId) {
        url = api_url + 'api/attachments/download/' + attachmentId;
        commons.OpenWindow(url, null, null, null);
    },
    DownLoadTaskTemplate: function (taskId) {
        //template/download/preview/{taskId}
        //for owa
        //utils.ajax({
        //    type: 'GET',
        //    url: api_url + 'api/tasks/template/download/preview/' + taskId,
        //    //args: { BusinessID: businessID, TaskReportRemark: $(".tc-report-remark").val() },
        //    success: function (data) {
        //        //func();
        //        commons.OpenOWAPreview(data);
        //    }
        //});
        var url = api_url + 'api/tasks/template/download/' + taskId;
        commons.OpenWindow(url, null, null, null);
    },
    //预览模板
    PreviewTemplate: function (templateID) {
        //url = api_url + 'api/templates/preview/' + templateID;
        url = api_url + 'api/templates/download/' + templateID;// "../AjaxHander/DownLoadViewExt.ashx?type=DownloadCurrentTemplate&templateID=" + templateID;
        commons.OpenWindow(url, null, null, null);
    },
    //下载模板
    DownloadTemplate: function (templateID) {
        url = api_url + 'api/templates/download/' + templateID;// "../AjaxHander/DownLoadViewExt.ashx?type=DownloadCurrentTemplate&templateID=" + templateID;
        commons.OpenWindow(url, null, null, null);
    },
    OWA_PreviewAttachment: function (attachmentId) {

        //var url = api_url + 'api/attachments/getfile/' +attachmentId;
        //utils.ajax({
        //    type: 'POST',
        //    url: url,
        //    args: {
        //        attachmentId: attachmentId
        //    },
        //    success: function (data) {
        //        if (data.IsUseV1) {
        //            if (!data.LinkToV2) {
        //                commons.OpenOWAPreview(data.result);
        //            } else {
        //                window.open(data.result);
        //            }
        //        } else {
        //            commons.OpenWindow(data.result);
        //        }
        //        }
        //    });
        var url = api_url + 'api/attachments/download/' + attachmentId;
        commons.OpenWindow(url, null, null, null);


    },
    //打包相关附件
    DownloadTaskAttachments: function (taskID) {
        url = api_url + 'api/tasks/attachments/' + taskID;// "../AjaxHander/DownLoadViewExt.ashx?type=DownloadCurrentTemplate&templateID=" + templateID;
        commons.OpenWindow(url, null, null, null);
    },
    //打包源文件
    DownloadTaskFiles: function (taskID) {
        url = api_url + 'api/tasks/taskfile/' + taskID;// "../AjaxHander/DownLoadViewExt.ashx?type=DownloadCurrentTemplate&templateID=" + templateID;
        commons.OpenWindow(url, null, null, null);
    },
    //下载审批流程
    DownloadTaskApprovedList: function (taskID) {
        url = api_url + 'api/tasks/taskapprovedlist/' + taskID;// "../AjaxHander/DownLoadViewExt.ashx?type=DownloadCurrentTemplate&templateID=" + templateID;
        commons.OpenWindow(url, null, null, null);
    },
    //下载汇总文件
    DownloadSummary: function (taskID, chk1, chk2, chk3, chk4) {
        url = api_url + 'api/tasks/summary/' + taskID + '/' + chk1 + '/' + chk2 + '/' + chk3 + '/' + chk4;// "../AjaxHander/DownLoadViewExt.ashx?type=DownloadCurrentTemplate&templateID=" + templateID;
        commons.OpenWindow(url, null, null, null);
    },
    PreviewSummary: function (taskID, chk1, chk2, chk3, chk4) {
        //url = api_url + 'api/tasks/summarypreview/' + taskID + '/' + chk1 + '/' + chk2 + '/' + chk3 + '/' + chk4;
        url = api_url + 'api/tasks/summary/' + taskID + '/' + chk1 + '/' + chk2 + '/' + chk3 + '/' + chk4;// "../AjaxHander/DownLoadViewExt.ashx?type=DownloadCurrentTemplate&templateID=" + templateID;
        //commons.OpenWindow(url, null, null, null);
        utils.ajax({
            type: 'GET',
            url: url,
            success: function (data) {
                commons.OpenWindow(data);
            }
        });
    }
};
