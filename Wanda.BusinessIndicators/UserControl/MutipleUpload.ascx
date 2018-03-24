<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MutipleUpload.ascx.cs" Inherits="LJTH.Fiscal.Budget.Web.Control.MutipleUpload" %>
<style type="text/css">
    #attaTable tbody td {
        text-align: center;
    }

   a.blue:link {color: blue;}
   a.blue:visited {color:blue;}
    a.blue:hover {color:red;}

</style>
<script type="text/javascript">
    var <%=this.ClientID%>_Attachments = new Array();
    var <%=this.ClientID%>_Attachment = {
        ID: "",
        FileName: "",
        Url: "",
        CreatorName: "",
        CreateTime: ""
    }

    $(function () {
        $('#<%=this.AttaUpload.ClientID%>').uploadify({
            'buttonText': '上传附件',
            'width': 100,
            'height': 25,
            'removeTimeout': 1,
            'fileTypeDesc': 'office file',
            'fileTypeExts': '<%=FileFilter%>',
            'fileSizeLimit': '51200',
            //swf文件路径
            'swf': '<%=this.ResolveClientUrl("~/Scripts/Upload/uploadify.swf")%>',
            //后台处理页面
            'uploader': '<%=this.ResolveClientUrl("~/UserControl/Upload1.ashx")%>',
            'formData': { "action": "<%=AttachmentType%>", "BusinessID": $("#hideMonthReportID").val() },
            'onUploadSuccess': function (file, data, response) {
                var res = eval("res = " + data.substr(data.indexOf("{"), data.length - data.indexOf("{")) + ";");

             
                <%=this.ClientID%>_Attachments[<%=this.ClientID%>_Attachments.length] = {
                    ID: res.ID,
                    FileName: res.FileName,
                    FileSize: res.Size,
                    Url: res.Url,
                    CreatorName: res.CreatorName,
                    CreateTime: res.CreateTime
                };
            },
            'onQueueComplete': function (queueData) { setTimeout(<%=this.ClientID%>_Show, 1000); },
        });
    });

    //让指定的DIV始终显示在屏幕正中间
    function <%=this.ClientID%>_letDivCenter() {
        var top = ($(window).height() - $(".uploadify-queue").height()) / 2;
        var left = ($(window).width() - $(".uploadify-queue").width()) / 2;
        var scrollTop = $(document).scrollTop();
        var scrollLeft = $(document).scrollLeft();
        $(".uploadify-queue").css({ position: 'absolute', 'top': top + scrollTop, left: left + scrollLeft }).show();
    }

    function <%=this.ClientID%>_Show() {


        var bHtml = "";
        for (i = 0; i <<%=this.ClientID%>_Attachments.length; i++) {
            bHtml += "<li "
            //if (i % 2 == 1) {
            //    sHtml += " class=\"colortr\"";
            //}
            bHtml += ">";
            bHtml += "<a  class='blue' style='line-height:20px;'  href='/AjaxHander/DownLoadFile.ashx?path=";
            bHtml += encodeURIComponent(<%=this.ClientID%>_Attachments[i].Url);
            bHtml += "&FileName=" + encodeURIComponent(<%=this.ClientID%>_Attachments[i].FileName) + "' target='_blank' >";
            bHtml += <%=this.ClientID%>_Attachments[i].FileName + "(" +<%=this.ClientID%>_Attachments[i].FileSize + ")";
            bHtml += "</a>";

            bHtml += "<img src='/images/del.png' alt='删除附件' title='删除附件' style='width: 15px; height: 15px; cursor: pointer;vertical-align:text-bottom; ' onclick='javascript:<%=this.ClientID%>_Delete(\"";
            bHtml += <%=this.ClientID%>_Attachments[i].ID;
            bHtml += "\");' />";
            if (i != <%=this.ClientID%>_Attachments.length - 1) {
                bHtml += "<span style='width: 15px; padding-left: 15px; font-weight:700; color: #ccc'>|</span>";
            }
            bHtml += "</li>";

        }

        $("#attaView_UL").html(bHtml);

    }

    function <%=this.ClientID%>_GetIDs() {
        var IDs = "";
        for (i = 0; i <<%=this.ClientID%>_Attachments.length; i++) {
            if (i > 0) {
                IDs += ",";
            }
            IDs += <%=this.ClientID%>_Attachments[i].ID;
        }
        return IDs;
    }

    function <%=this.ClientID%>_Delete(Id, fromServer) {
        if (confirm("您确定删除此文件吗？")) {

            WebUtil.ajax({
                url: "/TargetReportedControll/Delete",
                args: { data: Id },
                successReturn: function (result) {
                    if (result == "t") {
                        for (i = 0; i <<%=this.ClientID%>_Attachments.length; i++) {
                            if ( <%=this.ClientID%>_Attachments[i].ID.toLowerCase() == Id.toLowerCase()) {
                              
                              ArrayRemove(<%=this.ClientID%>_Attachments, i);
                            }
                        }
                        <%=this.ClientID%>_Show();
                    }
                    else {
                        alert(result);
                    }
                }
            });
        }
    }

    //Array.prototype.remove = function (dx) {
    //    if (isNaN(dx) || dx > this.length) { return false; }
    //    for (var i = 0, n = 0; i < this.length; i++) {
    //        if (this[i] != this[dx]) {
    //            this[n++] = this[i]
    //        }
    //    }
    //    this.length -= 1
    //}

    //这里不得不换成该方法，上面的扩展属性，与工作流平台的脚本冲突
    function ArrayRemove(ArrayData, dx) {
        if (isNaN(dx) || dx > ArrayData.length) { return false; }
        for (var i = 0, n = 0; i < ArrayData.length; i++) {
            if (ArrayData[i] != ArrayData[dx]) {
                ArrayData[n++] = ArrayData[i]
            }
        }
        ArrayData.length -= 1
    }


</script>
<div id="propose_enclosure" class="launchProcess_cont">

    <div class="DownExcelDiv"style="height: 35px;">
        <span class="DownExcelspan">
            <span>
                <input type="file" name="file_upload" runat="server" id="AttaUpload" serverdataid="" /></span>
        </span>
    </div>
    <div style="border: 1px solid #ccc; min-height: 80px; margin-top: 10px;">
        <table class="form_table" id="attaTable" style="width: 90%; display: none;">
            <thead>
                <tr class="tab_5_row_alt">
                    <th style="width: 450px;">附件名称</th>

                    <th>操作</th>
                </tr>
            </thead>
            <tbody id="attaView" runat="server">
            </tbody>
        </table>

        <ul class="tabs" id="attaView_UL" style="border-bottom-color: #FFF; margin-top: 10px; height: auto; margin-bottom: 0px;">
        </ul>
    </div>

    <div>
    </div>
</div>
