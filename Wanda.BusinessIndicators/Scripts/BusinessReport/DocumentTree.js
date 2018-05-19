
var zzNodes = new Array();
var ObjValue = {};

var NodeID;
var ValueA;
var ValueB;
var ValueC;
var ValueD;
var YearValue;
//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/MonthReportConfig.html", selector);
}





//加载
$(document).ready(function () {


    $("#UploadButton").hide();
    //Ztree的初始化设置
    var setting = {
        view: {
            selectedMulti: false
        },
        callback: {
            onClick: docTreeOnClick
        },
        check: {
            enable: false
        },

        data: {

            key: { name: "TreeNodeName" },
            simpleData: {
                enable: true,
                idKey: "ID",
                pIdKey: "ParentID",
                rootPId: '99999999-9999-9999-9999-FFFFFFFFFFFF'
            }
        },
        edit: {
            enable: false
        }
    };

    //初始化页面    
    WebUtil.ajax({
        async: true,
        url: "/DocumentManagerControll/GetDocumentTreeList",

        args: { SysID: $("#ddlSystem").val() },

        successReturn: function (ResultData) {
            zzNodes = ResultData;

            $.fn.zTree.init($("#DocTree"), setting, zzNodes);

            GetDocAttachmentsList(zzNodes);
        }
    });


});



function GetYear() {
    $("#DivYear").html("请选择年份");
    WebUtil.ajax({
        async: false,
        url: "/DocumentManagerControll/GetFinYear",
        successReturn: function (ResultData) {
            loadTmpl('#DocSelectYearTmpl').tmpl(ResultData).appendTo('#DivYear');
        }
    });

}

var TNodesId;
//通过节点获取，文档List
function GetDocAttachmentsList(TNodes) {
  
    TNodesId = $("#hideTerrNodeId").val();
    var zTree = $.fn.zTree.getZTreeObj("DocTree");
    if (TNodesId == "" || TNodesId == undefined) {
        var nodes = zTree.getNodes();
        if (nodes[0].children)
            TNodesId = nodes[0].children[0].ID; //获取第一个子节点
        else
            TNodesId = nodes[0].ID;
    }
   
    var node = zTree.getNodeByParam("ID", TNodesId);
    zTree.selectNode(node);

    SelectData();


}






var newCount = 1;

var newDCount = 1;










//单击事件
function docTreeOnClick(event, treeId, treeNode) {

    var treeObj = $.fn.zTree.getZTreeObj("DocTree");
    var nodeName = treeObj.getSelectedNodes();
    $("#hideTerrNodeId").val(treeNode.ID);
    TNodesId = treeNode.ID;
    SelectData();
    return true;
}



var objdata;


//编辑文档文件备注
function Attachment_Edit(Id) {
    for (i = 0; i < ObjValue.DocManagerData.length; i++) {
        if (ObjValue.DocManagerData[i].ID.toLowerCase() == Id.toLowerCase()) {
            objdata = ObjValue.DocManagerData[i];
        }
    }

    if (objdata != null) {

        $("#content_edit").empty();
        loadTmpl("#DocManage_info_tmpl").tmpl(objdata).appendTo($("#content_edit"));

        art.dialog({
            content: $("#divDocManageRemark").html(),
            lock: true,
            id: 'divDocManageRemark',
            title: '<span>文档备注--编辑</span>'
        });
    }
}

//保存文档备注
function SaveDocManage() {
    if (objdata != null) {
        objdata.Remark = $("#txtDocManageRemark").val();

        WebUtil.ajax({
            async: true,
            url: "/DocumentManagerControll/UpdateDocManageByRemark",
            args: { Data: WebUtil.jsonToString(objdata) },
            successReturn: function (ResultData) {
                if (ResultData == "Succeed") {
                    if (ObjValue != undefined) {
                        $("#AttaTableThead").empty();
                        $("#AttaTableTBody").empty();

                        loadTmpl('#DocManageTHeadTmpl').tmpl().appendTo('#AttaTableThead');
                        loadTmpl('#DocManageTBodyTmpl').tmpl(ObjValue).appendTo('#AttaTableTBody');
                    }
                }
            }
        });


    }

    art.dialog({ id: 'divDocManageRemark' }).close();
}






//格式化日期类型
function FormatDate(obj, displayTime, local) {
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
                    return "{0}年{1}月{2} {3}:{4}:{5}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                } else {
                    return "{0}-{1}-{2} {3}:{4}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute), doubleDigit(second));
                }
            }
        } else {
            if (obj == "1900/1/1 0:00:00" || obj == "1970/1/1 0:00:00" || obj == null) {
                return "---";
            } else {
                if (local == "CN") { //如果是CN的显示中文年月
                    return "{1}月{2}".formatBy(year, month, doubleDigit(day));
                } else {
                    return "{0}-{1}-{2} {3}:{4}".formatBy(year, doubleDigit(month), doubleDigit(day), doubleDigit(hour), doubleDigit(minute));
                }
            }
        }


    } catch (e) {
        return "";
    }

    function doubleDigit(n) { return n < 10 ? "0" + n : "" + n; }

}

function SelectData() {

    var FileName = $("#Text1").val();
    if (FileName == "输入文档名称") {
        FileName = "";
    }
    WebUtil.ajax({
        async: false,
        url: "/DocumentManagerControll/GetAttachmentsByName",
        args: { Value: FileName, Year: $("#FinsYear").val(), BusinessID: TNodesId, SystemID: $("#ddlSystem").val() },
        successReturn: function (ResultData) {
            ObjValue.DocManagerData = ResultData;
            $("#AttaTableThead").empty();
            $("#AttaTableTBody").empty();
            loadTmpl('#DocManageTHeadTmpl').tmpl().appendTo('#AttaTableThead');
            loadTmpl('#DocManageTBodyTmpl').tmpl(ObjValue).appendTo('#AttaTableTBody');

        }
    });

}


function HideZTree() {
    $("#HideTree").hide();
    $("#ShowTree").show();
    $("#TreeTD").fadeToggle();
}
function ShowZTree() {
    $("#ShowTree").hide();
    $("#HideTree").show();
    $("#TreeTD").fadeToggle();
}
