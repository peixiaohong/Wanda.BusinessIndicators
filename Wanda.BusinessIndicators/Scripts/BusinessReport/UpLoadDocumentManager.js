
var zzNodes = new Array();
var ObjValue = {};
var SelectAID = "";
var SelectYearList = "";
var NodeID;
var CompanyName;
//加载模版项
function loadTmpl(selector) {
    return WebUtil.loadTmpl("../SystemConfiguration/MonthReportConfig.html", selector);
}



//加载
$(document).ready(function () {

    //Ztree的初始化设置
    var setting = {
        view: {
            addHoverDom: addHoverDom,
            removeHoverDom: removeHoverDom,
            selectedMulti: false
        },
        callback: {
            onRemove: zTreeOnRemove,
            onClick: docTreeOnClick,
            beforeClick: docTreeBeforeClick,
            beforeRename: BeforeRename,
            onRename: docTreeOnRename,
            beforeRemove: docTreeBeforeRemove
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
            enable: true,
            showRenameBtn: true,
            showRemoveBtn: true
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




//通过节点获取，文档List
function GetDocAttachmentsList(TNodes) {
    var TNodesId;

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


    //加载附件List
    WebUtil.ajax({
        async: true,
        url: "/DocumentManagerControll/GetDocAttachmentsList",
        args: { TreeNodeID: TNodesId, SystemId: $("#ddlSystem").val(), Year: $("#FinsYear").val() },
        successReturn: function (ResultData) {

            ObjValue.DocManagerData = ResultData;

            $("#AttaTableThead").empty();
            $("#AttaTableTBody").empty();

            loadTmpl('#DocManageTHeadTmpl').tmpl().appendTo('#AttaTableThead');

            loadTmpl('#DocManageTBodyTmpl').tmpl(ObjValue).appendTo('#AttaTableTBody');

        }
    });
}






var newCount = 1;

var newDCount = 1;

//加载 编辑和删除图标
function addHoverDom(treeId, treeNode) {

    var sObj = $("#" + treeNode.tId + "_span");

    if (treeNode.editNameFlag || $("#addBtn_" + treeNode.tId).length > 0) return;

    var addStr = "<span class='button add' id='addBtn_" + treeNode.tId
        + "' title='add node' onfocus='this.blur();'></span>";

    sObj.after(addStr);

    var btn = $("#addBtn_" + treeNode.tId);

    if (btn) btn.bind("click", function () {

        var zTree = $.fn.zTree.getZTreeObj("DocTree");

        // 将数的节点数据添加到数据库中
        WebUtil.ajax({
            async: true,
            url: "/DocumentManagerControll/AddDocumentTreeNode",
            args: { TreeNodeID: treeNode.ID, SysID: $("#ddlSystem").val(), NodeNmae: "新的节点" + (newDCount++) },
            successReturn: function (newNodeID) {
                if (newNodeID != null) {   //添加节点

                    $("#hideTerrNodeId").val(newNodeID);

                    zTree.addNodes(treeNode, { ID: newNodeID, ParentID: treeNode.ID, TreeNodeName: "新的节点" + (newCount++) });
                }
            }
        });

        return false;
    });
};

//删除操作
function removeHoverDom(treeId, treeNode) {

    //alert("删除节点");
    $("#addBtn_" + treeNode.tId).unbind().remove();
};



//删除之前操作
function docTreeBeforeRemove(treeId, treeNode) {
    var ret = false;
    var doc = false;
    var zui = false;

    //首先判断树的节点
    WebUtil.ajax({
        async: false,
        url: "/DocumentManagerControll/GetDocTreeListByParentID",
        args: { TreeNodeID: treeNode.ID, SysID: $("#ddlSystem").val() },
        successReturn: function (result) {
            if (result.length > 0) {
                ret = false;
            } else {
                ret = true;
            }

            WebUtil.ajax({
                async: false,
                url: "/DocumentManagerControll/GetDocAttachmentsList",
                args: { TreeNodeID: treeNode.ID, SystemId: $("#ddlSystem").val() },
                successReturn: function (ResultData) {

                    if (ResultData.length > 0)
                        doc = false;
                    else
                        doc = true;


                    //判断节点下是否含有文档
                    if (ret == true && doc == true) {
                        zui = true;
                    } else {
                        alert("当前节点不能删除，因当前节点含有子节点或者是当前节点已存在文档!");
                        zui = false;
                    }

                }
            });

        }
    });

    return zui;

}




//删除节点操作
function zTreeOnRemove(event, treeId, treeNode) {

    WebUtil.ajax({
        async: true,
        url: "/DocumentManagerControll/DelDocumentTreeNode",
        args: { TreeNodeID: treeNode.ID, SysID: $("#ddlSystem").val(), NodeNmae: treeNode.TreeNodeName },
        successReturn: function (ResultData) {

        }
    });

    return false;
}


//修改Node名字
function docTreeOnRename(event, treeId, treeNode, isCancel) {
    WebUtil.ajax({
        async: true,
        url: "/DocumentManagerControll/UpdateDocumentTreeNode",
        args: { TreeNodeID: treeNode.ID, SysID: $("#ddlSystem").val(), NodeNmae: treeNode.TreeNodeName },
        successReturn: function (ResultData) {

        }
    });
}
//修改Node名字判断是否为空
function BeforeRename(event, treeId, treeNode, isCancel) {
    if (treeNode == "" || treeNode == null) {
        alert("节点名称不能为空");
        return false;
    }
    return true;
}

//点击Node节点之前
function docTreeBeforeClick(treeId, treeNode, clickFlag) {

    $("#hideTerrNodeId").val(treeNode.ID);


    return true;
}

//单击事件
function docTreeOnClick(event, treeId, treeNode) {
    SelectAID = "";
    var treeObj = $.fn.zTree.getZTreeObj("DocTree");
    var nodeName = treeObj.getSelectedNodes();

    NodeID = nodeName[0].ID;
    GetDocAttachmentsList(treeNode);

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


//搜索文档文件
function SearchData() {
    var _TNodeId = $("#hideTerrNodeId").val();

    var _FileName = $("#TxtDocSearch").val();

    WebUtil.ajax({
        async: true,
        url: "/DocumentManagerControll/GetAttachmentsBySearch",
        args: { BusinessID: _TNodeId, FileName: _FileName },
        successReturn: function (ResultData) {

            ObjValue.DocManagerData = ResultData;

            $("#AttaTableThead").empty();
            $("#AttaTableTBody").empty();

            loadTmpl('#DocManageTHeadTmpl').tmpl().appendTo('#AttaTableThead');

            loadTmpl('#DocManageTBodyTmpl').tmpl(ObjValue).appendTo('#AttaTableTBody');
        }
    });
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
