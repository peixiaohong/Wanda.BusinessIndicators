
var selID = ""; // 选择的系统ID



//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/ComprehensiveReportForms.html", selector);
}


$(function () {

    InitSysTree(); //初始化数控件数据
    InitYear();
    InitddlTarget('');

    GetReportTime();
});


//初始化指标
function InitddlTarget(objstr) {

    WebUtil.ajax({
        async: false,
        url: "/TargetSimpleReportController/GetTargetList",
        args: { SysID: objstr },
        successReturn: function (dateresult) {
            $("#ddlTarget").empty();
            loadTmpl("#Tmpl_ddlTargetInfo").tmpl(dateresult).appendTo("#ddlTarget");
        }
    });


    $("#ddlTarget").change(function () {
    }).multipleSelect({
        width: 210,
        multiple: true,
        multipleWidth: 180,
        selectAllText: "全选",
        allSelected: ""
    });

}


function InitYear() {
    WebUtil.ajax({
        async: false,
        url: "/TargetSimpleReportController/GetYear",
        successReturn: function (dateresult) {
            loadTmpl("#Tmpl_ddlYearInfo").tmpl(dateresult).appendTo("#ddlYear");

            loadTmpl("#Tmpl_ddlYearInfo").tmpl(dateresult).appendTo("#ddlYears");
        }
    });

    $("#ddlYear").change(function () {
    }).multipleSelect({
        width: 100,
        multiple: true,
        multipleWidth: 80,
        maxHeight:270,
        selectAllText: "全选",
        allSelected: ""
        });
    
}

//初始化Ztree的数据
function InitSysTree() {
    //Ztree的初始化设置
    var setting = {
        view: {
            dblClickExpand: false
        },
        callback: {
            beforeClick: beforeClick,
            onCheck: onCheck
        },
        check: {
            enable: true,
            chkboxType: { "Y": "", "N": "" }
        },
        data: {
            key: { name: "TreeNodeName" },
            simpleData: {
                enable: true,
                idKey: "ID",
                pIdKey: "ParentID",
                rootPId: '99999999-9999-9999-9999-FFFFFFFFFFFF'
            }
        }
    };


    //对Json 做下处理
    for (var i = 0; i < TreeDataJson.length; i++) {
        if (TreeDataJson[i].Category == 0)
            TreeDataJson[i].nocheck = true;
    }


    $.fn.zTree.init($("#SysTree"), setting, TreeDataJson);

    var treeObj = $.fn.zTree.getZTreeObj("SysTree");
    var nodes = treeObj.getNodes();
    for (var i = 0; i < nodes.length; i++) { //设置节点展开
        treeObj.expandNode(nodes[i], true, false, true);
    }

}

function GetReportTime()
{
    WebUtil.ajax({
        async: false,
        url: "/TargetSimpleReportController/GetReportTime",
        successReturn: function (dateresult) {

            var Report = dateresult;
            var MMM = new Date(dateresult.ReportTime).getMonth() + 1;

            var YYY = new Date(dateresult.ReportTime).getFullYear();
            
            $("#ddlYear").multipleSelect("setSelects", [YYY]);

            $("#ddlYears").val(YYY);
            
            $("#ddlMonths").val(MMM);

        }
    });
}




// 树控件只能点击最末级子节点
function beforeClick(treeId, treeNode) {


    var zTree = $.fn.zTree.getZTreeObj("SysTree");
    zTree.checkNode(treeNode, !treeNode.checked, null, true);
    return false;
}

function onCheck(e, treeId, treeNode) {

    var zTree = $.fn.zTree.getZTreeObj("SysTree"),
        nodes = zTree.getCheckedNodes(true),
        v = "", vId = "";

    for (var i = 0, l = nodes.length; i < l; i++) {
        v += nodes[i].TreeNodeName + ",";
        vId += nodes[i].ID + ",";
    }
    if (v.length > 0) v = v.substring(0, v.length - 1);
    var cityObj = $("#TxtSystem");
    cityObj.attr("value", v);

    if (vId.length > 0) vId = vId.substring(0, vId.length - 1);
    selID = vId;


    InitddlTarget(selID);// 这里控制指标的个数

}

function showMenu() {
    var cityObj = $("#TxtSystem");
    var cityOffset = $("#TxtSystem").offset();
    $("#menuContent").css({ left: cityOffset.left + "px", top: cityOffset.top + cityObj.outerHeight() + "px" }).slideDown("fast");

    $("body").bind("mousedown", onBodyDown);
}

function hideMenu() {
    $("#menuContent").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "menuBtn" || event.target.id == "TxtSystem" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
        hideMenu();
    }
}


//查询数据
function SearchData() {
    var TargetStr = $('#ddlTarget').multipleSelect('getSelects', 'text');
    var FinYear = $('#ddlYear').multipleSelect('getSelects', 'text');

    var _M = WebUtil.jsonToString(FinYear);
    var _N = WebUtil.jsonToString(TargetStr);

    if (TargetStr.length == 0 && selID.length == 0)
    {
        alert("请先选择,系统名称和系统指标!");
        return false;
    }

    WebUtil.ajax({
        async: true,
        url: "/TargetSimpleReportController/GetComprehensiveReport",
        args: { SysIDs: selID, FinYears: _M, Targets: _N, DataType: $("#ddlDataType").val(), IsCurrent: $("#ddlCumulativ_Month").val() },
        successReturn: function (result) {

            $("#CR_body").empty();

            if (result != null && result != undefined) {
               
                loadTmpl("#Tmpl_ComprehensiveReportInfo").tmpl(result).appendTo("#CR_body");

            }
        }

    });









}

function DownExcelReport(sender) {
    var TargetStr = $('#ddlTarget').multipleSelect('getSelects', 'text');
    var FinYear = $('#ddlYear').multipleSelect('getSelects', 'text');
    var FinMonth = $("#ddlMonths").val();
    var G_FinYear = $("#ddlYears").val();

    var _M = WebUtil.jsonToString(FinYear);
    var _N = WebUtil.jsonToString(TargetStr);

    var _F = WebUtil.jsonToString([G_FinYear]);

    if (sender == 'DownExcel')
        window.open("/AjaxHander/DownLoadComprehensiveReportForms.ashx?_SysIDs=" + selID + "&_FinYears=" + _M + "&_Targets=" + _N + "&_DataType=" + $("#ddlDataType").val() + "&_IsCurrent=" + $("#ddlCumulativ_Month").val() + "&_RptType=DownExcel");
    else if (sender == 'Movie')
        window.open("/AjaxHander/DownLoadComprehensiveReportForms.ashx?_RptType=Movie&_FinYears=" + _F + "&_FinMonths=" + FinMonth);
    else if (sender == 'Children')
        window.open("/AjaxHander/DownLoadComprehensiveReportForms.ashx?_RptType=Children&_FinYears=" + _F + "&_FinMonths=" + FinMonth);
    else if (sender == 'Business')
        window.open("/AjaxHander/DownLoadComprehensiveReportForms.ashx?_RptType=Business&_FinYears=" + _F + "&_FinMonths=" + FinMonth);
    else if (sender == 'Culture')
        window.open("/AjaxHander/DownLoadComprehensiveReportForms.ashx?_RptType=Culture&_FinYears=" + _F + "&_FinMonths=" + FinMonth);
}