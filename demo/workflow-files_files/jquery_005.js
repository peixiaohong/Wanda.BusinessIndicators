


jQuery.fn.extend(
{
    ajaxupload: function (options) {
        var _setting = {
            //formData: { "action": 'UploadTaskData' },
            paramName: "FileData",
            //url: '../AjaxHander/Upload.ashx?businessID=' + businessID,
            dataType: 'json',
            done: function (e, data) {
                if (options.finish)
                    options.finish(data.result.Result);
            },
            stop: function () {
                $(this).css({ fontSize: $(this).data("fs") });
                utils.unblock();
                //$.unblockUI();
            },
            error: function (e, data) {
                utils.alertMessage("文件上传失败：" + e.responseText, function () {
                    utils.unblock();
                });
            },
            start: function () {
                $(this).data("fs", $(this).css("fontSize")).css({ fontSize: "0px" });
                utils.block();
                //$.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>请稍等…</span></div>" });
            }
        };

        $.extend(_setting, options, true);
        $(this).fileupload(_setting);

    }
})





jQuery.fn.extend(
{
    ImportUser: function (options) {
        /**
         * 默认参数配置
         * businessid
         * callback
         * currentlist 当前人员，仅在任务发起页面需要使用到 因为页面的操作追加人没入库 
         * 
         * callback为点击后的回调函数
         * result包含以下属性
             * status 状态 0读取成功，1读取失败
             * importlength 总计导入数量
             * successlength 导入成功数量
             * errorlength 导入失败数量
             * repeatlength 重复导入数量
             * successuserlist 导入成功用户集合(UserLoginID,UserOrgPathName,UserJobName,UserName)
             * errordatalist 导入失败数据集合 (content,errormsg)
             * repeatuserlist 导入重复用户集合(UserLoginID,UserOrgPathName,UserJobName,UserName)
             * 
         */
        var _setting = {
            formData: { "action": 'UploadTaskUsers' },
            paramName: "FileData",
            url: api_url + 'api/usernodes/import/' + options.businessid,
            error: function (data) {
                utils.alertMessage('文件导入失败，请确定：\n要导入的数据存放在第一个工作表中\n每行一个用户账号（CTX）', function () {
                    utils.unblock();
                });
            },
            finish: function (data) {
                if (data.status == 0) {
                    _setting.error(data);
                    return;
                }
                var msg = "共计解析" + data.importlength + "条\n成功" + data.successlength + "条\n失败" + data.errorlength + "条\n重复" + data.repeatlength + "条";
                if (data.successlength == 0) {
                    utils.alertMessage(msg, function () {

                    });
                    return;
                }
                msg += "\n是否确定进行导入操作？";
                utils.confirmMessage(msg, function (ret) {
                    if (!ret) {
                        return;
                    }
                    if (typeof (options.callback) == "function") {
                        options.callback(data);
                    }
                });
            }
        };
        $(this).ajaxupload(_setting);
    }
})