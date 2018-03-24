<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocMutipleUpload.ascx.cs" Inherits="LJTH.BusinessIndicators.Web.UserControl.DocMutipleUpload" %>
<style type="text/css">
    #attaTable tbody td {
        text-align: center;
    }

    #ContentPlaceHolder1_DocMutipleUpload_AttaUpload {
        height: 18px!important;
    }
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
            'buttonText': '上传文档',
            'width': 77,
            'height': 25,
            'removeTimeout': 1,
            'fileTypeDesc': 'All Files',
            'fileTypeExts': '<%=FileFilter%>',
            'fileSizeLimit': '10240',
            //swf文件路径
            'swf': '<%=this.ResolveClientUrl("~/Scripts/Upload/uploadify.swf")%>',
            //后台处理页面
            'uploader': '<%=this.ResolveClientUrl("~/UserControl/DocUpLoad1.ashx")%>',
            'formData': { "action": "<%=AttachmentType%>", "businessID": $("#hideTerrNodeId").val(), "ValueA": '<%=this.ValueA%>', "ValueB": "<%=this.ValueB%>", "ValueC": "<%=this.ValueC%>", "ValueD": "<%=this.ValueD%>", "FileYear": "<%=this.FinValueYear%>" },
            'onUploadSuccess': function (file, data, response) {

            },
            'onUploadStart': function (obj) {
                
                //重新加载上传控件的方法（动态加载）
                var docSettings = {};
                docSettings.action ="<%=AttachmentType%>";
                docSettings.businessID =$("#hideTerrNodeId").val();
                $('#<%=this.AttaUpload.ClientID%>').uploadify('settings', 'formData', docSettings);
            },
            'onQueueComplete': function (queueData) {

                setTimeout(AttachmentsList_Show, 1000);
            }
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


    //显示list
    function AttachmentsList_Show() {
        //加载附件List
        WebUtil.ajax({
            async: true,
            url: "/DocumentManagerControll/GetDocAttachmentsList",
            args: { TreeNodeID: $("#hideTerrNodeId").val() },
            successReturn: function (ResultData) {

                ObjValue.DocManagerData = ResultData;

                $("#AttaTableThead").empty();
                $("#AttaTableTBody").empty();

                loadTmpl('#DocManageTHeadTmpl').tmpl().appendTo('#AttaTableThead');

                loadTmpl('#DocManageTBodyTmpl').tmpl(ObjValue).appendTo('#AttaTableTBody');

            }
        });
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

    //删除附件
    function Attachment_Delete(Id) {
        if (!confirm("确定删除该文档么?"))
            return;

        WebUtil.ajax({
            url: "/DocumentManagerControll/DelDocAttachments",
            args: { DocAttachmentsID: Id },
            successReturn: function (result) {

                //执行成功
                if (result == "Succeed") {
                    for (i = 0; i < ObjValue.DocManagerData.length; i++) {
                        if (ObjValue.DocManagerData[i].ID.toLowerCase() == Id.toLowerCase()) {
                            ObjValue.DocManagerData.remove(i);
                        }
                    }
                    $("#AttaTableThead").empty();
                    $("#AttaTableTBody").empty();

                    loadTmpl('#DocManageTHeadTmpl').tmpl().appendTo('#AttaTableThead');
                    loadTmpl('#DocManageTBodyTmpl').tmpl(ObjValue).appendTo('#AttaTableTBody');
                }
            }
        });
    }


    Array.prototype.remove = function (dx) {
        if (isNaN(dx) || dx > this.length) { return false; }
        for (var i = 0, n = 0; i < this.length; i++) {
            if (this[i] != this[dx]) {
                this[n++] = this[i]
            }
        }
        this.length -= 1
    }
</script>
<div id="propose_enclosure" class="launchProcess_cont">
    <div class="module padding_l10_r10 margin_t10" data-field="TmplPositionSetting">
        <div class="warp_title"  id="UploadButton" >
            <a><span id="lbTitle" runat="server">附件</span></a>
            <i id="lbIcon" runat="server" class="icon-bell" style="margin-left: 10px"></i><span id="lbNote" runat="server"></span>
            <label style="float: right; margin-right: 10px;">
                <input type="file" name="file_upload" runat="server" id="AttaUpload" />
            </label>
        </div>
        <div class="warp_border padding_10" style="text-align: center; min-height: 50px">
            <table class="tab_005" id="attaTable">
                <thead id="AttaTableThead"></thead>
                <tbody id="AttaTableTBody" data-field="Attachments">
                </tbody>
            </table>
        </div>
    </div>
</div>
