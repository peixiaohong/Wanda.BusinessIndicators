var alljson;
var SystemJson;
var CompanyProperty = [];
var arr = [];
var arrlist = [];
var table = '';
var SysID;
var XML;
var TargetList;
var ATargetID;
var SystemModel;

$(document).ready(function () {


    if ($("#SysID").val() != "") {
        $("#ddlSystem").val($("#SysID").val());
    }
    reload();
    CheckCompanyTmpl(TargetList[0].ID);
    var pathname = "/SystemConfiguration/CompanyList.aspx";
    if (location.pathname == pathname) {
        $("#sitmap").html('您当前所在的位置：系统管理<img src="../images/btn08.png">公司属性管理<img src="../images/btn08.png">公司属性管理');
        $("#jMenu").find("li").each(function () {
            var text = $(this).find("span")[0];
            $(this).removeClass("current first");
            if (text && text.innerHTML == "系统管理") {
                $(this).addClass("current first");
            }
        })
    }
});

//加载动画开始
function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}

function GetSystemModel() {
    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetSystem",
        args: { SysID: SysID },
        successReturn: function (result) {
            SystemModel = result;
        }
    });
}


function loadTmpl(selector) {
    return WebUtil.loadTmpl("../BusinessReport/CompanyTmpl.html", selector);
}
//加载动画开始
function reload() {
    //SystemJson = $.parseJSON($("#HiddenSystemJson").val());//获取XML的数据
    Load();
    SysID = $("#ddlSystem").val();
    GetSystemModel();
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetCompanyLists",
        args: { SysID: SysID },
        successReturn: function (ComList) {
            alljson = ComList;
        }
    });
    //获取指标加载tab页
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/GetTargetList",
        args: { SysID: SysID },
        successReturn: function (tarlist) {
            TargetList = tarlist;
            $("#tabhead").empty();
            for (var i = 0; i < tarlist.length; i++) {
                BangTargetList(tarlist[i]);

            }
        }
    });
    WebUtil.ajax({
        async: false,
        url: "/CompanyController/CompanyXMLList",
        args: { SysID: SysID },
        successReturn: function (xml) {
            if (xml != null) {
                XML = xml;
                SystemJson = $.parseJSON(xml);
                BangSelect(SystemJson);
            }
        }
    });
    BangTable(alljson, SystemJson, TargetList[0].ID);


    Fake();
}




//取出xml中属性的值
var CompanyProperty1 = new Array();
var CompanyProperty2 = new Array();
var CompanyProperty3 = new Array();
var CompanyProperty4 = new Array();
var CompanyProperty5 = new Array();
var CompanyProperty6 = new Array();
var CompanyProperty7 = new Array();
var CompanyProperty8 = new Array();
var CompanyProperty9 = new Array();
function BangSelect(SystemJson) {
    $("#TCompanyProperty1").empty();
    $("#TCompanyProperty2").empty();
    $("#TCompanyProperty3").empty();
    $("#TCompanyProperty4").empty();
    $("#TCompanyProperty5").empty();
    $("#TCompanyProperty6").empty();
    $("#TCompanyProperty7").empty();
    $("#TCompanyProperty8").empty();
    $("#TCompanyProperty9").empty();
    $("#TBCompanyProperty1").empty();
    $("#TBCompanyProperty2").empty();
    $("#TBCompanyProperty3").empty();
    $("#TBCompanyProperty4").empty();
    $("#TBCompanyProperty5").empty();
    $("#TBCompanyProperty6").empty();
    $("#TBCompanyProperty7").empty();
    $("#TBCompanyProperty8").empty();
    $("#TBCompanyProperty9").empty();
    if (SystemModel.Category != 2) {
        $("#TCompanyProperty1").prepend("<option value=''>空</option>")
        $("#TCompanyProperty2").prepend("<option value=''>空</option>")
        $("#TCompanyProperty3").prepend("<option value=''>空</option>")
        $("#TCompanyProperty4").prepend("<option value=''>空</option>")
        $("#TCompanyProperty5").prepend("<option value=''>空</option>")
        $("#TCompanyProperty6").prepend("<option value=''>空</option>")
        $("#TCompanyProperty7").prepend("<option value=''>空</option>")
        $("#TCompanyProperty8").prepend("<option value=''>空</option>")
        $("#TCompanyProperty9").prepend("<option value=''>空</option>")
        $("#TBCompanyProperty1").prepend("<option value=''>空</option>")
        $("#TBCompanyProperty2").prepend("<option value=''>空</option>")
        $("#TBCompanyProperty3").prepend("<option value=''>空</option>")
        $("#TBCompanyProperty4").prepend("<option value=''>空</option>")
        $("#TBCompanyProperty5").prepend("<option value=''>空</option>")
        $("#TBCompanyProperty6").prepend("<option value=''>空</option>")
        $("#TBCompanyProperty7").prepend("<option value=''>空</option>")
        $("#TBCompanyProperty8").prepend("<option value=''>空</option>")
        $("#TBCompanyProperty9").prepend("<option value=''>空</option>")
    }
 

    for (var a = 0; a < SystemJson.length; a++) {
        if (SystemJson[a].CompanyProperty["@ColumnName"] == "CompanyProperty1") {
            CompanyProperty1 = SystemJson[a].CompanyProperty.ItemProperty;
            for (var i = 0; i < CompanyProperty1.length; i++) {

                $("#TCompanyProperty1").append("<option value='" + CompanyProperty1[i]["@ItemPropertyValue"] + "'>" + CompanyProperty1[i]["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty1").append("<option value='" + CompanyProperty1[i]["@ItemPropertyValue"] + "'>" + CompanyProperty1[i]["@ItemPropertyName"] + "</option>");

            }
            if (CompanyProperty1.length == undefined && CompanyProperty1["@ItemPropertyValue"] == "尾盘") {
                $("#TCompanyProperty1").append("<option value='" + CompanyProperty1["@ItemPropertyValue"] + "'>是</option>");
                $("#TCompanyProperty1").append("<option value=''>否</option>");
                $("#TBCompanyProperty1").append("<option value='" + CompanyProperty1["@ItemPropertyValue"] + "'>是</option>");
                $("#TBCompanyProperty1").append("<option value=''>否</option>");
            }
            else if (CompanyProperty1["@ItemPropertyValue"] == "旅行社") {
                $("#TCompanyProperty1").append("<option value='" + CompanyProperty1["@ItemPropertyValue"] + "'>" + CompanyProperty1["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty1").append("<option value='" + CompanyProperty1["@ItemPropertyValue"] + "'>" + CompanyProperty1["@ItemPropertyName"] + "</option>");

            }
        }
        else if (SystemJson[a].CompanyProperty["@ColumnName"] == "CompanyProperty2") {
            CompanyProperty2 = SystemJson[a].CompanyProperty.ItemProperty;
            for (var i = 0; i < CompanyProperty2.length; i++) {
                $("#TCompanyProperty2").append("<option value='" + CompanyProperty2[i]["@ItemPropertyValue"] + "'>" + CompanyProperty2[i]["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty2").append("<option value='" + CompanyProperty2[i]["@ItemPropertyValue"] + "'>" + CompanyProperty2[i]["@ItemPropertyName"] + "</option>");

            }
        }
        else if (SystemJson[a].CompanyProperty["@ColumnName"] == "CompanyProperty3") {
            CompanyProperty3 = SystemJson[a].CompanyProperty.ItemProperty;
            for (var i = 0; i < CompanyProperty3.length; i++) {
                $("#TCompanyProperty3").append("<option value='" + CompanyProperty3[i]["@ItemPropertyValue"] + "'>" + CompanyProperty3[i]["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty3").append("<option value='" + CompanyProperty3[i]["@ItemPropertyValue"] + "'>" + CompanyProperty3[i]["@ItemPropertyName"] + "</option>");

            }
        }
        else if (SystemJson[a].CompanyProperty["@ColumnName"] == "CompanyProperty4") {
            CompanyProperty4 = SystemJson[a].CompanyProperty.ItemProperty;
            for (var i = 0; i < CompanyProperty4.length; i++) {
                $("#TCompanyProperty4").append("<option value='" + CompanyProperty4[i]["@ItemPropertyValue"] + "'>" + CompanyProperty4[i]["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty4").append("<option value='" + CompanyProperty4[i]["@ItemPropertyValue"] + "'>" + CompanyProperty4[i]["@ItemPropertyName"] + "</option>");

            }
        }
        else if (SystemJson[a].CompanyProperty["@ColumnName"] == "CompanyProperty5") {
            CompanyProperty5 = SystemJson[a].CompanyProperty.ItemProperty;
            for (var i = 0; i < CompanyProperty5.length; i++) {
                $("#TCompanyProperty5").append("<option value='" + CompanyProperty5[i]["@ItemPropertyValue"] + "'>" + CompanyProperty5[i]["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty5").append("<option value='" + CompanyProperty5[i]["@ItemPropertyValue"] + "'>" + CompanyProperty5[i]["@ItemPropertyName"] + "</option>");

            }
        }
        else if (SystemJson[a].CompanyProperty["@ColumnName"] == "CompanyProperty6") {
            CompanyProperty6 = SystemJson[a].CompanyProperty.ItemProperty;
            for (var i = 0; i < CompanyProperty6.length; i++) {
                $("#TCompanyProperty6").append("<option value='" + CompanyProperty6[i]["@ItemPropertyValue"] + "'>" + CompanyProperty6[i]["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty6").append("<option value='" + CompanyProperty6[i]["@ItemPropertyValue"] + "'>" + CompanyProperty6[i]["@ItemPropertyName"] + "</option>");

            }
        }
        else if (SystemJson[a].CompanyProperty["@ColumnName"] == "CompanyProperty7") {
            CompanyProperty7 = SystemJson[a].CompanyProperty.ItemProperty;
            for (var i = 0; i < CompanyProperty7.length; i++) {
                $("#TCompanyProperty7").append("<option value='" + CompanyProperty7[i]["@ItemPropertyValue"] + "'>" + CompanyProperty7[i]["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty7").append("<option value='" + CompanyProperty7[i]["@ItemPropertyValue"] + "'>" + CompanyProperty7[i]["@ItemPropertyName"] + "</option>");

            }
        }
        else if (SystemJson[a].CompanyProperty["@ColumnName"] == "CompanyProperty8") {
            CompanyProperty8 = SystemJson[a].CompanyProperty.ItemProperty;
            for (var i = 0; i < CompanyProperty8.length; i++) {
                $("#TCompanyProperty8").append("<option value='" + CompanyProperty8[i]["@ItemPropertyValue"] + "'>" + CompanyProperty8[i]["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty8").append("<option value='" + CompanyProperty8[i]["@ItemPropertyValue"] + "'>" + CompanyProperty8[i]["@ItemPropertyName"] + "</option>");

            }
        }
        else {
            CompanyProperty9 = SystemJson[a].CompanyProperty.ItemProperty;
            for (var i = 0; i < CompanyProperty9.length; i++) {
                $("#TCompanyProperty9").append("<option value='" + CompanyProperty9[i]["@ItemPropertyValue"] + "'>" + CompanyProperty9[i]["@ItemPropertyName"] + "</option>");
                $("#TBCompanyProperty9").append("<option value='" + CompanyProperty9[i]["@ItemPropertyValue"] + "'>" + CompanyProperty9[i]["@ItemPropertyName"] + "</option>");

            }
        }
    }
}

function Load() {
    $.blockUI({ message: "<div style='width:200px'><img src='../../images/ajax-loader.gif' alt='waiting'><span style='font-size:20px;padding-left:20px;color:#3f3f3f'>数据请求中...</span></div>" });
}
//加载动画结束
function Fake() {
    $.unblockUI();
}


function BangTargetList(tarlist) {


    loadTmpl('#CompanyTargetHead').tmpl(tarlist).appendTo('#tabhead');
}


function BangTable(alljson, SystemJson, id) {
    document.getElementById('tab').innerHTML = "";
    table = "";
    if (alljson != null) {


        for (var j = 0; j < alljson.length; j++) {

            if (id == TargetList[j].ID) {

            
            //如果XML中没有数据,则判定该系统没有属性,所以仅有序号和公司名称
            if (SystemJson == '' || SystemJson == null) {
                table += '<table class="tab_005" id="tb' + TargetList[j].ID + '" style="table-layout: fixed"> '; 
                table += ' <thead id="CompanyHead" style="width: 100%">';
                table += '<tr> <th style="width: 10%">序号</th><th style="width: 60%">公司名称</th><th style="width: 15%">开业时间</th>';
                table += '<th style="width:15%">排序值</th></tr></thead> <tbody id="CompanyRows">';
                for (var a = 0; a < alljson[j].length; a++) {
                    var sum = parseInt(a) + parseInt(1);
                    table += '<tr><td class="Td_Left" style="text-align: center !important; vertical-align: middle;width: 10%">' + sum + '</td>';

                    if (alljson[j][a].Sequence > 0) {
                        table += '<td  class="Td_Left" style="width: 60%;text-align:center !important">';
                        table += '<a style="text-decoration:underline;color:blue;font-size:13px;;cursor:pointer"  onclick="change(\'' + alljson[j][a].ID + '\',\'' + j + '\')">' + alljson[j][a].CompanyName + '<a></td>';
                    }
                    else {
                        table += '<td  class="Td_Left" style="text-align: center !important;width: 60%;">' + alljson[j][a].CompanyName + '</td>';
                    }
                    if (alljson[j][a].OpeningTime == "" || alljson[j][a].OpeningTime == "0001/1/1 0:00:00") {
                        table += '<td class="Td_Left" style="text-align: center !important; vertical-align: middle;width: 15%">--</td>';
                    }
                    else {
                        table += '<td class="Td_Left" style="text-align: center !important;width: 15%; vertical-align: middle">' + SetTime(alljson[j][a].OpeningTime) + '</td>';
                    }
                    table += '<td class="Td_Left" style="text-align:center !important;width:15%">' + alljson[j][a].Sequence + '</td></tr> ';

                }
                
            }
            else {
                //创建表头
                table += '<table class="tab_005" id="tb' + TargetList[j].ID + '" style="table-layout: fixed;"><thead id="CompanyHead" style="width: 100%"><tr> <th style="width: 10%">序号</th><th style="width: 20%">公司名称</th>';
                for (var i = 0; i < SystemJson.length; i++) {
                    table += '<th>' + SystemJson[i].CompanyProperty["@PropertyName"] + '</th>';
                   
                }
                if (SystemModel.Category == '2') {
                    table += '<th>小计</th>';
                }
                table += '<th style="width: 15%">开业时间</th><th style="width:15">排序值</th></tr></thead><tbody id="CompanyRows">';
                //循环该系统下的所有公司
                for (var i = 0; i < alljson[j].length; i++) {
                    var sumon = parseInt(i) + parseInt(1);
                    
                    table += '<tr><td class="Td_Left" style="text-align: center !important; vertical-align: middle;width: 10px">' + sumon + '</td>';
                    if (alljson[j][i].Sequence > 0) {
                        table += '<td  class="Td_Left" style="width: 20%"><a style="text-decoration:underline;color:blue;font-size:13px ;cursor:pointer" onclick="change(\'' + alljson[j][i].ID + '\',\'' + j + '\')">' + alljson[j][i].CompanyName + '<a></td>';
                    }
                    else {
                        table += '<td  class="Td_Left" style="width: 20%">' + alljson[j][i].CompanyName + '</td>';
                    }
                    //根据XML中存在的属性插入表中的属性   如果XML仅存在CompanyProperty1,那么仅显示表中CompanyProperty1的数据
                    for (var a = 0; a < SystemJson.length; a++) {
                        table += '<td  class="Td_Left" style="text-align: center !important;" id="' + alljson[j][i].ID + '' + SystemJson[a].CompanyProperty["@ColumnName"] + '">' + alljson[j][i][SystemJson[a].CompanyProperty["@ColumnName"]] + '</td>';
                    }
                    if (SystemModel.Category=='2') {
                        table += '<td  class="Td_Left" style="text-align: center !important;" id="' + alljson[j][i].ID + 'CompanyProperty2">' + alljson[j][i].CompanyProperty2 + '</td>';
                    }
                    if (alljson[j][i].OpeningTime != "" && alljson[j][i].OpeningTime != "0001/1/1 0:00:00") {

                        table += '<td class="Td_Left" style="text-align: center !important; vertical-align: middle">' + SetTime(alljson[j][i].OpeningTime) + '</td>';
                    }
                    else {
                        table += '<td class="Td_Left" style="text-align: center !important; vertical-align: middle">--</td>';
                    }
                    table += '<td class="Td_Left" style="text-align:center !important;width:15">' + alljson[j][i].Sequence + '</td></tr>';

                }
            }
            table += '</tbody></table>';
            }
        }
        var floatTab = '<table class="tab_005" id="tabFloate"><thead ></thead></table >'
        document.getElementById('tab').innerHTML = floatTab + table;
        var obj = $("#tabFloate");
        var head = $("#CompanyHead");
        obj.find("thead").html(head.html());
        var tab = $("#CompanyRows");
        FloatHeader(obj, tab);
    }
}



function CheckCompanyTmpl(ID) {
    for (var i = 0; i < TargetList.length; i++) {
        document.getElementById('tab' + TargetList[i].ID + '').className = "active3";
    }

    document.getElementById('tab' + ID + '').className = "active3 active_sub3";
    ATargetID = ID;
    BangTable(alljson, SystemJson, ID);
    DeleteSelect();
}


function Delete(CompanyID) {
    arr = [];
    brr = [];
    for (var i = 0; i < alljson.length; i++) {
        if (alljson[i].ID == CompanyID) {
            alljson[i].IsDeleted = true;//更改isdeleted变为删除
            arr.push(alljson[i]);
        }
        else {
            brr.push(alljson[i]);
        }

    }//循环出要删除的元素,把isdeleted至为true,并放入arr内

    WebUtil.ajax({
        async: true,
        url: "/CompanyController/UpdateCompany",
        args: { info: WebUtil.jsonToString(arr[0]) },
        successReturn: function () {
            alljson = brr;
            alert("删除成功！");
            BangTable(alljson, SystemJson);

        }
    });
}
function change(CompanyID, adj) {
    DeleteSelect();
    arr = [];
    for (var i = 0; i < alljson[adj].length; i++) {
        if (alljson[adj][i].ID == CompanyID) {
            arr.push(alljson[adj][i]);//得到该公司json
        }
    }
    $("#EditName").val(arr[0].CompanyName);//将公司名称插入
    $("#ThSeq").val(arr[0].Sequence);
    if (arr[0].OpeningTime != '0001/1/1 0:00:00') {
        $("#Selecttime").val(SetTime(arr[0].OpeningTime));
    }
    else {
        $("#Selecttime").val("");
    }


    for (var a = 0; a < SystemJson.length; a++) {
        var columnname = SystemJson[a].CompanyProperty["@ColumnName"];
        var CName = SystemJson[a].CompanyProperty["@PropertyName"];
        $("#NB" + columnname + "").html(CName + "：");//将区域属性的名称插入DIV
        $("#TB" + columnname + "").val(arr[0][columnname]);//将该公司的区域属性值插入输入框
        $("#HB" + columnname + "").show();//将该区域属性显示
    }

    if (SystemModel.Category == '2') {
         $("#NBCompanyProperty2").html("小计：")
        $("#TBCompanyProperty2").hide();
        $("#BCompanyProperty2input").val(arr[0].CompanyProperty2);
        $("#BCompanyProperty2input").show();
        $("#HBCompanyProperty2").show();
    }




    art.dialog({
        content: $("#EditDetail").html(),
        lock: true,
        id: 'EditDetail',
        title: '<span>编辑公司</span>'
    });
    BangTargetCheck(CompanyID,"edit");
}
function SaveCompany() {
    $("#ThTar").val("");
    $("#TargetList").val("");
    //没有选中的
    var checkboxlist = "";
    $("#tbEdit input[type='checkbox']").not("input:checked").each(function () {

        checkboxlist += $(this).val() + "|";
    });
    var check = new Array();
    $("#tbEdit input[name='TargetBox']").not("input:checked").each(function () {

        check.push($(this).val());
    });
    checkboxlist = checkboxlist.substring(0, checkboxlist.length - 1);

    var sel_chk = "";
    //选中的复选框
    $("#tbEdit input[type='checkbox']:checked").each(function () {
        sel_chk += $(this).val() + "|";
    });
    sel_chk = sel_chk.substring(0, sel_chk.length - 1);


    if ($("#ThSeq").val() == null || $("#ThSeq").val() == "" || $("#ThSeq").val() == 0) {
        alert("排序不能为空!");
    }
    else if ($("#EditName").val() == null || $("#EditName").val() == "") {
        alert("公司名称不能为空!");
    }
    else if (check.length == TargetList.length) {
        alert("请选择指标!");
    }
    else {

        if (arr != null || arr != "") {
            for (var a = 0; a < SystemJson.length; a++) {
                var columnname = SystemJson[a].CompanyProperty["@ColumnName"];
                arr[0][columnname] = $("#TB" + columnname + "").val();
            }
            arr[0].OpeningTime = $("#Selecttime").val();
            arr[0].CompanyName = $("#EditName").val();
            if (SystemModel.Category == '2') {
                arr[0].CompanyProperty2 = $("#BCompanyProperty2input").val();
            }


            arr[0].Sequence = $("#ThSeq").val();
            WebUtil.ajax({
                async: false,
                url: "/CompanyController/UpdateCompany",
                args: { info: WebUtil.jsonToString(arr[0]), TargetIDList: checkboxlist, SelTargetIDList: sel_chk },
                successReturn: function (result) {
                    reload();
                    alert("保存成功！");
                }
            });
            art.dialog({ id: 'EditDetail' }).close();
        }


    }
    
    CheckCompanyTmpl(ATargetID);
}
function AddCompany() {//添加的方法
    //按钮切换
    DeleteSelect();
    $("#savetrue").hide();
    $("#addtrue").show();
    $("#seq").val("");
    $("#addname").val("");
    $("#rpt_info_CommitDate").val("");
    $("#TargetList").val("");
    $("#ThTar").val("");

    AddSelect();
    for (var a = 0; a < SystemJson.length; a++) {
        var columnname = SystemJson[a].CompanyProperty["@ColumnName"];
        var CName = SystemJson[a].CompanyProperty["@PropertyName"];
        if (SystemModel.Category == '2') {
            $("#N" + columnname + "").html( "是否尾盘：");//将区域属性的名称插入
        }
        else {
            $("#N" + columnname + "").html(CName + "：");//将区域属性的名称插入
        }
     
        $("#T" + columnname + "").val("0");
        $("#H" + columnname + "").show();//将该区域属性显示
    }
    BangTargetCheck("","add");
    if (SystemModel.Category == '2') {
        $("#NCompanyProperty2").html("小计：")
        $("#TCompanyProperty2").hide();
        $("#CompanyProperty2input").show();
        $("#HCompanyProperty2").show();
    }
    art.dialog({
        content: $("#divDetail").html(),
        lock: true,
        id: 'divDetail',
        title: '<span>添加公司</span>'
    });
}
function AddsCompany() {//保存添加的方法
    var checkboxlist = new Array();
    $("#tbDetail input[type='checkbox']").not("input:checked").each(function () {

        checkboxlist.push($(this).val());
    });

    var sel_chk = "";
    //选中的复选框
    $("#tbDetail input[type='checkbox']:checked").each(function () {

        if ($(this).prop("checked"))
        {
            sel_chk += $(this).val() + "|";
        }
        
    });
    sel_chk = sel_chk.substring(0, sel_chk.length - 1);


    var info = "";
    info += 'ExTargetList:' + checkboxlist + '|';
    for (var a = 0; a < SystemJson.length; a++) {
        var columnname = SystemJson[a].CompanyProperty["@ColumnName"];
        if ($("#T" + columnname + "").val() != "0") {
            info += SystemJson[a].CompanyProperty["@ColumnName"];
            info += ':';
            info += $("#T" + columnname + "").val();
            info += '|';
        }
        else {
            alert("请选择公司的区域属性!");
        }

    }
    info += 'SystemID:' + SysID + '|';

    for (var s = 0; s < alljson.length; s++) {
        if ($("#seq").val() == alljson[s].Sequence) {
            h = alljson[s].CompanyName;
        }
    }
    if ($("#rpt_info_CommitDate").val() != "" && $("#rpt_info_CommitDate").val() != null) {
        info += 'OpeningTime:' + $("#rpt_info_CommitDate").val() + '|';
    }
    if ($("#addname").val() == null || $("#addname").val() == "") {
        alert("公司名称不能为空!");
    }
    if (SystemModel.Category == '2') {
        var CompanyProperty2xiaoji = $("#CompanyProperty2input").val();
        if (CompanyProperty2xiaoji!="") {
            info += 'CompanyProperty2:' + CompanyProperty2xiaoji + '|';
        }
    }
    if (checkboxlist.length == TargetList.length) {
        alert("请选择指标!");
    }
    else if ($("#seq").val() == null || $("#seq").val() == "" || $("#seq").val() == 0) {

        alert("排序值不能为空或为0!");
        reload();
    }
   
    else {
        info += 'Sequence:' + $("#seq").val() + '|'

        info += 'CompanyName:' + $("#addname").val() + '';

        WebUtil.ajax({
            async: false,
            url: "/CompanyController/AddCompany",
            args: { info: info, SysID: SystemModel.ID, SelTargetIDList: sel_chk },
            successReturn: function () {
                reload();
                alert("添加成功");
            }
        });
        art.dialog({ id: 'divDetail' }).close();

    }

    CheckCompanyTmpl(ATargetID);
}

function AddSelect() {



    $("#TCompanyProperty1").prepend("<option value='0'>--请选择--</option>")
    $("#TCompanyProperty2").prepend("<option value='0'>--请选择--</option>")
    $("#TCompanyProperty3").prepend("<option value='0'>--请选择--</option>")
    $("#TCompanyProperty4").prepend("<option value='0'>--请选择--</option>")
    $("#TCompanyProperty5").prepend("<option value='0'>--请选择--</option>")
    $("#TCompanyProperty6").prepend("<option value='0'>--请选择--</option>")
    $("#TCompanyProperty7").prepend("<option value='0'>--请选择--</option>")
    $("#TCompanyProperty8").prepend("<option value='0'>--请选择--</option>")
    $("#TCompanyProperty9").prepend("<option value='0'>--请选择--</option>")
    $("#TBCompanyProperty1").prepend("<option value='0'>--请选择--</option>")
    $("#TBCompanyProperty2").prepend("<option value='0'>--请选择--</option>")
    $("#TBCompanyProperty3").prepend("<option value='0'>--请选择--</option>")
    $("#TBCompanyProperty4").prepend("<option value='0'>--请选择--</option>")
    $("#TBCompanyProperty5").prepend("<option value='0'>--请选择--</option>")
    $("#TBCompanyProperty6").prepend("<option value='0'>--请选择--</option>")
    $("#TBCompanyProperty7").prepend("<option value='0'>--请选择--</option>")
    $("#TBCompanyProperty8").prepend("<option value='0'>--请选择--</option>")
    $("#TBCompanyProperty9").prepend("<option value='0'>--请选择--</option>")
}
function DeleteSelect() {
    $("#TCompanyProperty1 option[value='0']").remove();
    $("#TCompanyProperty2 option[value='0']").remove();
    $("#TCompanyProperty3 option[value='0']").remove();
    $("#TCompanyProperty4 option[value='0']").remove();
    $("#TCompanyProperty5 option[value='0']").remove();
    $("#TCompanyProperty6 option[value='0']").remove();
    $("#TCompanyProperty7 option[value='0']").remove();
    $("#TCompanyProperty8 option[value='0']").remove();
    $("#TCompanyProperty9 option[value='0']").remove();
    $("#TBCompanyProperty1 option[value='0']").remove();
    $("#TBCompanyProperty2 option[value='0']").remove();
    $("#TBCompanyProperty3 option[value='0']").remove();
    $("#TBCompanyProperty4 option[value='0']").remove();
    $("#TBCompanyProperty5 option[value='0']").remove();
    $("#TBCompanyProperty6 option[value='0']").remove();
    $("#TBCompanyProperty7 option[value='0']").remove();
    $("#TBCompanyProperty8 option[value='0']").remove();
    $("#TBCompanyProperty9 option[value='0']").remove();

}
$(function () {
    var error = 0;
    $('#file1').uploadify({
        'buttonText': '导入数据',
        'width': 80,
        'height': 25,
        'fileTypeDesc': 'office file',
        'fileTypeExts': '*.doc; *.docx; *.xls;*.xlsx;',
        'fileSizeLimit': '10240',
        //swf文件路径
        'swf': '../Scripts/UpLoad/uploadify.swf',
        //后台处理页面
        'uploader': '/AjaxHander/UpLoadCompany.ashx?SysId=' + SysID + '',
        'onUploadSuccess': function (file, data, response) {
            error = data;

            alert(data);
            reload();
            CheckCompanyTmpl(TargetList[0].ID)
        },

        'onUploadError': function (file, data, response) {
            alert("上传失败，程序出错！");
            reload();
        }
    })
})
function SetTime(time) {
    var date = new Date(time);
    var NewYear = date.getFullYear();
    var NewMonth = date.getMonth() + 1;
    var NewDay = date.getDate();
    var Newtime = NewYear + "-" + NewMonth + "-" + NewDay;
    return Newtime;
}

function DownExcel() {
    window.open("/AjaxHander/DownExcelCompany.ashx?SysId=" + SysID + "");
}


function BangTargetCheck(companyid, ADJ) {

    $("#TargetList").html("");
    $("#ThTar").html("");

    WebUtil.ajax({
        async: false,
        url: "/TargetController/GetExctargetListByComList",
        args: { CompanyID: companyid, SystemID: SysID },
        successReturn: function (result) {
            if (ADJ=="add") {
                loadTmpl('#CheckBoxTarget').tmpl(result).appendTo('#TargetList');

            }
            else {
                loadTmpl('#CheckBoxTarget').tmpl(result).appendTo('#ThTar');
            }

        }
    });

}


function TargetConfig() {
    window.open("../SystemConfiguration/ExceptionTarget.aspx?ID=" + SysID + "&ComeFrom=CompanyList");

}