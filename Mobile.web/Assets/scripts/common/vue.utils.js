var CommonProvider = {};
CommonProvider.install = function (Vue, ops) {
    CommonAjax = function (ele, options) {
        utils.block();
        ele.then(function (response) {
            if (response.status == 200) {
                return response.json();
            }
        })
           .then(function (data) {
               utils.unblock();
               //utils.log("登陆状态验证：" + data.IsAuth);
               if (options.success != undefined && typeof options.success === 'function') {
                   options.success(data);
               }
           })
           .catch(function (response) {
               utils.unblock();
               var msg = response.body;
               var callback = function (status, statusText, msg) { };
               if (options.error != undefined && typeof options.error === 'function') {
                   callback = options.error;
               }
               if (response.status == 409) {
                   utils.alertMessage(msg, function () {
                       callback(response.status, response.statusText, msg);
                   });
                   return;
               }
               if (response.status == 403) {
                   utils.alertMessage(msg, function () {
                       //callback(response.status, response.statusText, msg);
                       //window.location.href = 'wd_sso_logout?action=exit&url=' + encodeURIComponent(window.location.href);
                       window.location.href = '/Public/Login.aspx?returnUrl=' + encodeURIComponent(window.location.href);
                   });
                   return;
               }
               else {
                   callback(response.status, response.statusText, msg);
                   return;
               }

           })
    }
    CommonAjaxDelete = function (ele, options) {
        CommonAjax(ele.delete(options.url), options);
    }
    CommonAjaxGet = function (ele, options) {
        CommonAjax(ele.get(options.url, options.args), options);
    }
    CommonAjaxPost = function (ele, options) {
        CommonAjax(ele.post(options.url, options.args), options);
    }

    Vue.ajaxPost = function (opts) {
        CommonAjaxPost(Vue.http, opts);
    }
    Vue.ajaxGet = function (opts) {
        CommonAjaxGet(Vue.http, opts);
    }
    Vue.ajaxDelete = function (opts) {
        CommonAjaxDelete(Vue.http, opts);
    }
    Vue.ToThousands = function (num) {
        return (parseInt(num) || 0).toString().replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
    }
    Vue.Trim = function (str) {
        if (str.length) {
            return str.replace(/(?!(^\n|\n{2}))\n/g, "<br/>");
        } 
    }
    Vue.Height = function (str) {
        var num = Number(str.replace(/\%/g, ""));
        if (num < 100) {
            return true;
        } else {
            return false;
        }
    }
    /*
     * 异步post事件
     * options
     *      url 请求地址
     *      args 提交参数
     *      success 成功回调
     *      error 错误回调
     */
    Vue.prototype.ajaxPost = function (options) {
        CommonAjaxPost(this.$http, options);
    }
    Vue.prototype.ajaxGet = function (options) {
        CommonAjaxGet(this.$http, options);
    }
    Vue.prototype.ajaxDelete = function (options) {
        CommonAjaxDelete(this.$http, options);
    }
    Vue.filter('format-datetime', function (value) {
        return utils.ChangeDateFormat(value, true);
    });
    Vue.filter('format-date', function (value) {
        return utils.ChangeDateFormat(value, false);
    });
    Vue.filter('format-datediff', function (value) {
        return utils.DateDiffString(value);
    });
    Vue.filter('format-dateminute', function (value) {
        return utils.ChangeDateFormatToMinute(value);
    });
}
Vue.use(CommonProvider);

Vue.http.options.credentials = true;
Vue.config.devtools = true;

