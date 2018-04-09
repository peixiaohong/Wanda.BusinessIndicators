var JsonList = [];


var SYear ;
var SMonth ;
var SystemID;


//加载动画开始
function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}



$(document).ready(
    function () {

        InitSysTree(); //初始化数控件数据


        //默认选中根节点 , 
        var treeObj = $.fn.zTree.getZTreeObj("SysTree");
        var node = treeObj.getNodes()[0]; //获取根节点
        treeObj.selectNode(node, false);
        $("#TxtSystem").val(node.TreeNodeName);
        SystemID = node.ID;


        GetTreeData(); // TreeGrid


        // Getlist();
    });


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
        }
    };


    $.fn.zTree.init($("#SysTree"), setting, TreeDataJson);

    var treeObj = $.fn.zTree.getZTreeObj("SysTree");
    var nodes = treeObj.getNodes();
    for (var i = 0; i < nodes.length; i++) { //设置节点展开
        treeObj.expandNode(nodes[i], true, false, true);
    }

}


// 树控件只能点击最末级子节点
function beforeClick(treeId, treeNode) {


    $("#TxtSystem").val(treeNode.TreeNodeName);

    //SYear = $("#ddlYear").val();
    //SMonth = $("#ddlMonth").val();
    SystemID = treeNode.ID;
    
    hideMenu();

    //var check = (treeNode && !treeNode.isParent);

    //if (check) {
    //    $("#TxtSystem").val(treeNode.TreeNodeName);

    //    if (treeNode.Category == 1) {
    //        window.location.href = "TargetRpt.aspx?_sysid=" + treeNode.ID + "&_finYear=" + $("#ddlYear").val() + "&_finMonth=" + $("#ddlMonth").val() + "&IsLastestVersion=" + IsLatestVersion;
    //    }
    //    if (treeNode.Category == 2) {
    //        window.location.href = "ProMonthReport.aspx?_sysid=" + treeNode.ID + "&_finYear=" + $("#ddlYear").val() + "&_finMonth=" + $("#ddlMonth").val() + "&IsLastestVersion=" + IsLatestVersion;
    //    } else if (treeNode.Category == 3) {
    //        window.location.href = "TargetGroupRpt.aspx?_sysid=" + treeNode.ID + "&_finYear=" + $("#ddlYear").val() + "&_finMonth=" + $("#ddlMonth").val() + "&IsLastestVersion=" + IsLatestVersion;
    //    } else if (treeNode.Category == 4) {
    //        window.location.href = "TargetDirectlyRpt.aspx?_sysid=" + treeNode.ID + "&_finYear=" + $("#ddlYear").val() + "&_finMonth=" + $("#ddlMonth").val() + "&IsLastestVersion=" + IsLatestVersion;
    //    }
    //}
}

function onCheck(e, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("SysTree"),
        nodes = zTree.getCheckedNodes(true),
        v = "";
    for (var i = 0, l = nodes.length; i < l; i++) {
        v += nodes[i].name + ",";
    }
    if (v.length > 0) v = v.substring(0, v.length - 1);
    var cityObj = $("#SysTree");
    cityObj.attr("value", v);
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







//现在用TreeGrid 展示的
function GetTreeData()
{
    SYear = $("#ddlYear").val();
    SMonth = $("#ddlMonth").val();

    WebUtil.ajax({
        async: true,
        url: "/TargetController/GetTreeDataJosn",
        args: { SysID: SystemID, FinYear: SYear, FinMonth: SMonth},
        successReturn: function (List) {
            config.data = List;

            $("#div_TreeGrid").empty();

            var treeGrid = new TreeGrid(config);
            treeGrid.show()

        }
    });
}


// 原来获取数据的
function Getlist() {
    Load();

     SYear = $("#ddlYear").val();
     SMonth = $("#ddlMonth").val();
     SystemID = $("#ddlSystem").val();

    WebUtil.ajax({
        async: true,
        url: "/TargetController/GetAllReportApprove",
        args: { SystemID: SystemID, FinYear: SYear, FinMonth: SMonth },
        successReturn: function (List) {
            BangList(List);
            Fake();

        }
    });


    //var obj = $("#Head");
    //var tab = $("#rows");
    //FloatHeader(obj, tab);
}

function BangList(List) {
    var row = "";
    var index = 0;
    if (List.length > 0) {
        for (var i = 0; i < List.length; i++) {
            var th = "";//整行数据
            var td = "";//列数据(审批列)
            var sysid = $.base64.btoa(List[i].SystemID);//base64加密
            var finmonth = $.base64.btoa($("#ddlMonth").val());//base64加密
            var finyear = $.base64.btoa($("#ddlYear").val());//base64加密
            //首先判断数据库字段是否有值。无值则不显示。
            if (List[i].ReportApprove)
            {
                if (List[i].Group > 0)// 根据Group判断需要向下合并几个TD。
                {
                    index++;
                    th += "<tr><td class=\"Td_Center\" rowspan=" + List[i].Group + " ><input name=\"ChecBoxs\" type=\"checkbox\" value=" + List[i].BusinessID + " /></td><td class=\"Td_Center\" rowspan=" + List[i].Group + " >" + index + "</td>";
                    if (List[i].Category == 1) {
                        th += "<td class=\"Td_Center\" rowspan="+ List[i].Group+"><a target=\"_blank\" href=\"../BusinessReport/TargetRpt.aspx?sysID=" + sysid + "&FinMonths=" + finmonth + "&FinYears=" + finyear + " \">" + List[i].SystemName + "</a></td>";
                    }
                    else if (List[i].Category == 2) {
                        th += "<td class=\"Td_Center\" rowspan=" + List[i].Group + "><a target=\"_blank\" href=\"../BusinessReport/ProMonthReport.aspx?sysID=" + sysid + "&FinMonths=" + finmonth + "&FinYears=" + finyear + " \">" + List[i].SystemName + "</a></td>";
                    }
                    else if (List[i].Category == 3) {
                        th += "<td class=\"Td_Center\" rowspan=" + List[i].Group + "><a target=\"_blank\" href=\"../BusinessReport/TargetGroupRpt.aspx?sysID=" + sysid + "&FinMonths=" + finmonth + "&FinYears=" + finyear + " \">" + List[i].SystemName + "</a></td>";
                    }
                    else if (List[i].Category == 4) {
                        th += "<td class=\"Td_Center\" rowspan=" + List[i].Group + "><a target=\"_blank\" href=\"../BusinessReport/TargetDirectlyRpt.aspx?sysID=" + sysid + "&FinMonths=" + finmonth + "&FinYears=" + finyear + " \">" + List[i].SystemName + "</a></td>";
                    } else {
                        th += "<td class=\"Td_Center\" rowspan=" + List[i].Group + ">" + List[i].SystemName + "</td>";
                    }

                    th += "<td class=\"Td_Center\">" + List[i].CreatTime + "</td><td class=\"Td_TrueLeft\">";
                    for (var n = 0; n < List[i].list.length; n++) {
                        td += CreatList(List[i].list[n]);
                    }
                    th += td;
                    th += "</td>";
                    if (List[i].WFStatus == "Approved") {
                        th += "<td class=\"Td_Center\">审批通过</td>";
                    } else if (List[i].WFStatus == "Progress") {
                        th += "<td class=\"Td_Center\"  style=\"color:red\">审批中</td>";
                    } else if (List[i].WFStatus == "Draft") {
                        th += "<td class=\"Td_Center\"  style=\"color:red\">退回</td>";
                    } else if (List[i].WFStatus == "Cancel") {
                        th += "<td class=\"Td_Center\"  style=\"color:red\">作废</td>";
                    }
                    th += "</tr>";
                }
                else if (List[i].Group == 0)//除每个系统第一个需要合并的Td外其余本系统Group皆为0
                {
                    th += "<td class=\"Td_Center\">" + List[i].CreatTime + "</td><td class=\"Td_TrueLeft\">";
                    for (var n = 0; n < List[i].list.length; n++) {
                        td += CreatList(List[i].list[n]);
                    }
                    th += td;
                    th += "</td>";
                    if (List[i].WFStatus == "Approved") {
                        th += "<td class=\"Td_Center\">审批通过</td>";
                    } else if (List[i].WFStatus == "Progress") {
                        th += "<td class=\"Td_Center\"  style=\"color:red\">审批中</td>";
                    } else if (List[i].WFStatus == "Draft") {
                        th += "<td class=\"Td_Center\"  style=\"color:red\">退回</td>";
                    } else if (List[i].WFStatus == "Cancel") {
                        th += "<td class=\"Td_Center\"  style=\"color:red\">作废</td>";
                    }
                    th += "</tr>";
                }
                else if (List[i].Group < 0)//!=0时 正常展示全部数据
                {
                    if (List[i].Category == 1) {
                        th += "<tr> <td class=\"Td_Center\"><a target=\"_blank\" href=\"../BusinessReport/TargetRpt.aspx?sysID=" + sysid + "&FinMonths=" + finmonth + "&FinYears=" + finyear + " \">" + List[i].SystemName + "</a></td>";
                    }
                    else if (List[i].Category == 2) {
                        th += "<tr> <td class=\"Td_Center\"><a target=\"_blank\" href=\"../BusinessReport/ProMonthReport.aspx?sysID=" + sysid + "&FinMonths=" + finmonth + "&FinYears=" + finyear + " \">" + List[i].SystemName + "</a></td>";
                    }
                    else if (List[i].Category == 3) {
                        th += "<tr> <td class=\"Td_Center\"><a target=\"_blank\" href=\"../BusinessReport/TargetGroupRpt.aspx?sysID=" + sysid + "&FinMonths=" + finmonth + "&FinYears=" + finyear + " \">" + List[i].SystemName + "</a></td>";
                    }
                    else if (List[i].Category == 4) {
                        th += "<tr> <td class=\"Td_Center\"><a target=\"_blank\" href=\"../BusinessReport/TargetDirectlyRpt.aspx?sysID=" + sysid + "&FinMonths=" + finmonth + "&FinYears=" + finyear + " \">" + List[i].SystemName + "</a></td>";
                    }
                    th += "<td class=\"Td_Center\">" + List[i].CreatTime + "</td><td class=\"Td_TrueLeft\">";
                    for (var n = 0; n < List[i].list.length; n++) {
                        td += CreatList(List[i].list[n]);
                    }
                    th += td;
                    th += "</td>";
                    if (List[i].WFStatus == "Approved") {
                        th += "<td class=\"Td_Center\">审批通过</td>";
                    } else if (List[i].WFStatus == "Progress") {
                        th += "<td class=\"Td_Center\"  style=\"color:red\">审批中</td>";
                    } else if (List[i].WFStatus == "Draft") {
                        th += "<td class=\"Td_Center\"  style=\"color:red\">退回</td>";
                    } else if (List[i].WFStatus == "Cancel") {
                        th += "<td class=\"Td_Center\"  style=\"color:red\">作废</td>";
                    }
                    th += "</tr>";
                }               
            }
            //当审批流程为空时,将整列数据变空
            if (td == "") {
                th = "";
            }
            row += th;

        }
    }
    $("#rows").html(row);
}

function CreatList(list) {
    var tds = "";
    for (var i = 0; i < list.length; i++) {
        if ($.trim(list[i].ActivityName) != "通知" && $.trim(list[i].ActivityName) != "抄送") {
            if (list[i].RunningStatus == 3 && $.trim(list[i].ActivityName) == "发起人") {//发起人
                if (list[i].Candidates.length > 0) {


                    tds += "<span class=\"workflow_span wanda-wf-float\"><span class=\"workflow_span\">【 ";
                    var spans = "";
                    for (var n = 0; n < list[i].Candidates.length; n++) {
                        spans += "" + list[i].Candidates[n].Name + ",";
                    }
                    tds += Deletelast(spans);

                    tds += "】</span> <img style=\"PADDING-TOP: 1px;\" alt=\"\" src=\"../Images/icon/icon27.png\" /></span>";
                    // tds += " <img alt=\"\" src=\"../Images/icon/03-14.png\" />";

                }
            }
            else if (list[i].RunningStatus != 3 && $.trim(list[i].ActivityName) == '发起人') {//发起人
                if (list[i].Candidates.length > 0) {
                    //" + list[i].ActivityName + "
                    tds += "<span class=\"workflow_span wanda-wf-float\"><span class=\"workflow_span\">【 ";
                    var spans = "";
                    for (var n = 0; n < list[i].Candidates.length; n++) {
                        spans += "" + list[i].Candidates[n].Name + ",";
                    }
                    tds += Deletelast(spans);

                    tds += "】</span></span>";
                }
            }
            else if (list[i].RunningStatus == 0 && $.trim(list[i].ActivityName) != "发起人" && $.trim(list[i].ActivityName) != "等待汇总审批") {
                if (i != 0) {
                    if (tds != "") {
                        tds += " <img alt=\"\" src=\"../Images/icon/03-14.png\" />";
                    }
                }
                if (list[i].Candidates.length > 0) {
                    tds += "<span class=\"workflow_span wanda-wf-float\"><span class=\"workflow_span\">【 ";
                    var spans = "";
                    for (var n = 0; n < list[i].Candidates.length; n++) {
                        spans += "" + list[i].Candidates[n].Name + ",";
                    }
                    tds += Deletelast(spans);

                    tds += "】</span></span>";
                }
                //  tds += " <img alt=\"\" src=\"../Images/icon/03-14.png\" />";
            }
            else if (list[i].RunningStatus == 3 && $.trim(list[i].ActivityName) != "发起人" && $.trim(list[i].ActivityName) != "等待汇总审批") {

                if (list[i].Candidates.length > 0) {
                    if (i != 0) {
                        if (tds != "") {
                            tds += " <img alt=\"\" src=\"../Images/icon/03-14.png\" />";
                        }
                    }
                    if (list[i].Candidates.length == 1) {
                        tds += "<span class=\"workflow_span wanda-wf-float\">【";

                        tds += "<span class=\"workflow_span\">" + list[i].Candidates[0].Name + "</span>】";

                        tds += " <img style=\"PADDING-TOP: 1px;\" alt=\"\" src=\"../Images/icon/icon27.png\" />";

                        tds += "</span>";
                    }
                    else {
                        tds += "<span class=\"workflow_span wanda-wf-float\"><span class=\"workflow_span\">【 ";
                        var spans = "";
                        for (var n = 0; n < list[i].Candidates.length; n++) {
                            spans += "<span class=\"workflow_span\" >" + list[i].Candidates[n].Name + "</span>";
                            spans += " <img style=\"PADDING-TOP: 1px;\" alt=\"\" src=\"../Images/icon/icon27.png\" />,";

                        }
                        tds += Deletelast(spans);

                        tds += "】</span></span>";
                    }
                }
            }
            else if (list[i].RunningStatus == 1 && $.trim(list[i].ActivityName) != "发起人" && $.trim(list[i].ActivityName) != "等待汇总审批") {//正在进行中的流程


                if (list[i].Candidates.length > 0) {
                    if (i != 0) {
                        if (tds != "") {
                            tds += " <img alt=\"\" src=\"../Images/icon/03-14.png\" />";
                        }


                    }
                    tds += "<span class=\"workflow_span wanda-wf-float\" style=\"FONT-WEIGHT: 600\">【";
                    var spans = "";
                    if (list[i].Candidates.length > 1) {
                        for (var s = 0; s < list[i].Candidates.length; s++) {
                            if (list[i].Candidates[s].Completed == 'true') {
                                spans += "<span class=\"workflow_span\" style=\"FONT-WEIGHT: 600\">" + list[i].Candidates[s].Name + "</span>";
                                spans += " <img style=\"PADDING-TOP: 1px;\" alt=\"\" src=\"../Images/icon/icon27.png\" />,";
                            }
                            else {
                                spans += "<span class=\"workflow_span\" style=\"FONT-WEIGHT: 600\">" + list[i].Candidates[s].Name + "</span><img style=\"PADDING-TOP: 1px;\" alt=\"\" src=\"../Images/ico/light_red.png\" />,";
                            }
                        }
                        tds += Deletelast(spans);
                        tds += "】</span>";
                    }
                    else {

                        spans += "<span class=\"workflow_span\" style=\"FONT-WEIGHT: 600\">" + list[i].Candidates[0].Name + "</span>";
                        tds += spans;

                        tds += "】<img style=\"PADDING-TOP: 1px;\" alt=\"\" src=\"../Images/ico/light_red.png\" /></span>";
                    }
                }
            }
            else if (list[i].RunningStatus == 2 && list[i].ActivityType == 3 && $.trim(list[i].ActivityName) != "等待汇总审批") {

                if (list[i].Candidates.length > 0) {
                    if (i != 0) {
                        if (tds != "") {
                            tds += " <img alt=\"\" src=\"../Images/icon/03-14.png\" />";
                        }
                    }

                    tds += "<span class=\"workflow_span wanda-wf-float\" style=\"FONT-WEIGHT: 600\">" + list[i].ActivityName + "【";
                    var spans = "";
                    if (list[i].Candidates.length > 1) {
                        for (var s = 0; s < list[i].Candidates.length; s++) {
                            if (list[i].Candidates[s].Completed == true) {
                                spans += "<span class=\"workflow_span\" >" + list[i].Candidates[s].Name + "</span>";
                                spans += " <img style=\"PADDING-TOP: 1px;\" alt=\"\" src=\"../Images/icon/icon27.png\" />,";
                            }
                            else {
                                spans += "<span class=\"workflow_span\" >" + list[i].Candidates[s].Name + "</span>,";
                            }
                        }
                        tds += Deletelast(spans);
                        tds += "】</span>";
                    }
                    else {

                        spans += "<span class=\"workflow_span\" >" + list[i].Candidates[0].Name + "</span>";
                        tds += spans;


                        tds += "】</span>";
                    }
                }
            }
            else if (list[i].RunningStatus == 4) {
                tds = "";
                break;

            }
            if (i < list.length - 1) {
                if (list[i + 1].Candidates.length > 0) {
                }

            }
            if (i == list.length - 1) {
                tds += "<br /> ";
            }
        }
    }
    return tds;
}

function Deletelast(str) {
    var newstr = str.substring(0, str.length - 1);
    return newstr;
}

function DownExcel() {
    var BusID = "";
    $('input[name="ChecBoxs"]:checked').each(function () {
        BusID += $(this).val() + ",";
    });
    if (BusID.length > 0) {
        BusID = BusID.substring(0, BusID.length - 1);
    }
    window.open("/AjaxHander/DownFileList.ashx?BusinessID=" + BusID + "&BusinessType=" + "系统生成Excel"&"SystemID="+BusID+"&IsMonth=" + 1);
}



// TreeGrid的初始化参数 //月报的数据
var config = {
    id: "TreeGrid",
    width: "100%",
    renderTo: "div_TreeGrid",
    headerAlign: "left",
    headerHeight: "30",
    dataAlign: "left",
    indentation: "20",
    folderOpenIcon: "../Scripts/TreeGrid/images/folderOpen.png",
    folderCloseIcon: "../Scripts/TreeGrid/images/folderClose.png",
    defaultLeafIcon: "../Scripts/TreeGrid/images/defaultLeaf.gif",
    hoverRowBackground: "false",
    folderColumnIndex: "0",
    itemClick: "itemClickEvent",
    columns: [
    {
        headerText: "系统名称",
        dataField: "Name",
        headerAlign: "center",
        width: "20%",
        handler: "customMonthSysName"  
    },
    {
        headerText: "系统ID",
        dataField: "SysID",
        headerAlign: "center",
        dataAlign: "center",
        width: "100",
        hidden: true
    },
    {
        headerText: "提交时间",
        dataField: "RptTime",
        headerAlign: "center",
        dataAlign: "center",
        width: "150"
    },
    {
        headerText: "审批情况",
        dataField: "ReportApprove",
        headerAlign: "center",
        dataAlign: "left",
        width: "60%"
    },
    {
        headerText: "审批状态",
        dataField: "WFStause",
        headerAlign: "center",
        dataAlign: "center",
        width: "150"
    }],
    data: []
};


//去除换行，空格
function _TrimStr(obj) {
    var str;
    str = obj.replace(new RegExp('^(\s*\\n)*|(\\n\s*)*$'), '');
    ws = /\n/,
        i = str.length;
    while (ws.test(str.charAt(--i)));
    return str.slice(0, i + 1);
}


//判断是否是12月31日，以便前面判断是否隐藏
function IsTimeShow(obj) {
    try {
        var date = new Date(obj);
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();

        if (month == 12 && day == 31) {
            return true;
        } else {
            return false
        }
    } catch (e) {
        return false;
    }
}