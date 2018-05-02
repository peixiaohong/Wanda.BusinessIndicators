<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMasterPage/MainMasterApprove.Master" AutoEventWireup="true" CodeBehind="ReportMonthApprove.aspx.cs" Inherits="Mobile.web.Report.ReportMonthApprove" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script src="<%=ResolveUrl("~/Assets/scripts/Report/ReportApprove.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <section id="ApproveContent">
        <div class="container" style="padding-top:0;">
            <div class="row">

                <div id="taskCollectionViewMobile" class="white-bg clear">
                    <div class="white-wrap col-md-12 col-sm-12">
                        <div class="collection-top text-center">
                            <h1 class="collection-t">房地产事业部2018年02月度报告</h1>
                        </div>
                        <div class="collection-cont col-md-12 col-sm-12">
                            <ul>
                                <li class="active">
                                    <h3>基本信息<span class="collection-updown-icon"></span></h3>
                                    <!--<div class="collection-result">-->
                                    <div>
                                        <table class="from-table">
                                            <tbody>
                                                <tr>
                                                    <td width="35%">上报系统</td>
                                                    <td>房地产事业部</td>
                                                </tr>
                                                <tr>
                                                    <td>上报年份</td>
                                                    <td>2018年</td>

                                                </tr>
                                                <tr>
                                                    <td>上报月份</td>
                                                    <td>2月</td>
                                                </tr>
                                                <tr>
                                                    <td>上报说明</td>
                                                    <td>
                                                        <p>1、销售收入：计划17,800万元，实际完成 17,655万元，累计完成率99.2%</p>
                                                        <p>2、回款收入：计划35,600万元，实际完成35,488万元，累计完成率99.7%</p>
                                                        <p>3、利润额收入：计划8,900万元，实际完成8,893万元，累计完成率99.7%</p>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </li>
                                <li class="active">
                                    <h3>月度经营报告（本月）<span class="collection-updown-icon"></span></h3>
                                    <!--<div class="collection-result">-->
                                    <div>
                                        <table class="from-table alignCenter">
                                            <thead>
                                                <tr>
                                                    <th>项目</th>
                                                    <th>本月计划（万元）</th>
                                                    <th>本月实际（万元）</th>
                                                    <th>完成率</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>销售</td>
                                                    <td>8,900</td>
                                                    <td>8,861</td>
                                                    <td>99.6%</td>
                                                </tr>
                                                <tr>
                                                    <td>回款</td>
                                                    <td>17,800</td>
                                                    <td>17,758</td>
                                                    <td>99.8%</td>
                                                </tr>
                                                <tr>
                                                    <td>利润额</td>
                                                    <td>4,450</td>
                                                    <td>4,448</td>
                                                    <td>100%</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </li>
                                <li class="active">
                                    <h3>月度经营报告（累计）<span class="collection-updown-icon"></span></h3>
                                    <!--<div class="collection-result">-->
                                    <div>
                                        <table class="from-table alignCenter">
                                            <thead>
                                                <tr>
                                                    <th>项目</th>
                                                    <th>本月计划（万元）</th>
                                                    <th>本月实际（万元）</th>
                                                    <th>完成率</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>销售</td>
                                                    <td>17,800</td>
                                                    <td>17,655</td>
                                                    <td>99.2%</td>
                                                </tr>
                                                <tr>
                                                    <td>回款</td>
                                                    <td>35,600</td>
                                                    <td>35,488</td>
                                                    <td>99.7%</td>
                                                </tr>
                                                <tr>
                                                    <td>利润额</td>
                                                    <td>8,900</td>
                                                    <td>8,893</td>
                                                    <td>99.7%</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>

                <div id="SJSJ_LC">
                    <%--<div class="inset_wrapper">
                        <div class="bpf_workflow_approve_result">
                            <div class="bpf_workflow_approve_center">
                                <a id="bpf_workflow_approve_top" href="http://192.168.50.72/Application/Task/TaskCollectionView.aspx?businessId=6ff06a0c-fd22-4b91-b0a6-79bd1b4ab848#" class="bpf_workflow_approve_top" style="bottom: 50px;"></a>
                                <div class="bpf_workflow_approve_container" style="padding-bottom: 50px;">
                                    <ul class="bpf_workflow_result_list">
                                        <li class="bpf_workflow_result_active">
                                            <h3>审批流程                                    <span class="bpf_workflow_hide_default" name="1"></span><span class="bpf_workflow_updown_icon" name="up"></span></h3>
                                            <div class="bpf_workflow_result_cont">
                                                <ul class="bpf_workflow_approve_list">
                                                    <li class="tag_first">
                                                        <div class="bpf_workflow_approve_node">
                                                            <span class="bpf_workflow_approve_arrow"></span><span style="color: rgb(51, 51, 51);">填报人</span>
                                                            <div class="bpf_workflow_tag bpf_workflow_tag_complete">
                                                                <span><span>【</span><span>郑桂 </span><span>】</span></span> <span class="bpf_workflow_tag_icon"></span>
                                                                <!---->
                                                                <!---->
                                                            </div>
                                                            <!---->
                                                        </div>
                                                    </li>
                                                    <li>
                                                        <div class="bpf_workflow_approve_node bpf_workflow_ongoing">
                                                            <span class="bpf_workflow_approve_arrow"></span><span style="color: rgb(51, 51, 51);" class="isDefault">审批</span>
                                                            <div class="bpf_workflow_tag">
                                                                <!---->
                                                                <!---->
                                                                <span><span><span class="isDefault">【</span><span>范冰 </span><span class="isDefault">】</span></span>
                                                                </span><span class="bpf_workflow_tag_icon"></span>
                                                            </div>
                                                            <!---->
                                                        </div>
                                                    </li>
                                                </ul>
                                            </div>
                                        </li>
                                        <!---->
                                        <li class="bpf_workflow_result_active">
                                            <h3>审批意见<span class="bpf_workflow_updown_icon" name="up"></span></h3>
                                            <div class="bpf_workflow_result_cont">
                                                <div class="bpf_workflow_quick_views">
                                                    <p><span>快捷意见：</span> <span class="bpf_workflow_approve_tag bpf_workflow_blue">同意</span> <span class="bpf_workflow_approve_tag bpf_workflow_blue">不同意</span> <span class="bpf_workflow_approve_tag bpf_workflow_blue">收到</span></p>
                                                </div>
                                                <div class="bpf_workflow_approve_views">
                                                    <textarea name="approveViews" id="" cols="30" rows="10" class="bpf_workflow_modal_textarea"></textarea></div>
                                            </div>
                                        </li>
                                        <li class="bpf_workflow_result_active">
                                            <h3>审批记录<span class="bpf_workflow_updown_icon" name="up"></span></h3>
                                            <div class="bpf_workflow_result_cont bpf_workflow_opinions">
                                                <ul>
                                                    <li>
                                                        <div class="bpf_workflow_opinion_info">
                                                            <div class="bpf_workflow_opinion_name">
                                                                <p class="bpf_workflow_opinion_t">郑桂 </p>
                                                                <p class="bpf_workflow_time">5分钟前</p>
                                                                <span class="bpf_workflow_icon_default bpf_workflow_icon_init"></span>
                                                            </div>
                                                            <div class="bpf_workflow_opinion_body">
                                                                <div class="bpf_workflow_opinion_bg">
                                                                    <h4>经营指标</h4>
                                                                    <div class="bpf_workflow_opinion_cont">
                                                                        <p>开始</p>
                                                                    </div>
                                                                    <p class="bpf_workflow_date">2018-04-08 17:36:54</p>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </li>
                                                </ul>
                                                <div class="bpf_workflow_view_more down" onclick="viewMoreBtn(down)">
                                                    <p><span class="">查看更多</span></p>
                                                </div>
                                                <div class="bpf_workflow_view_more up">
                                                    <p><span class="bpf_workflow_view_active" onclick="viewMoreBtn(up)">收起</span></p>
                                                </div>
                                                <!---->
                                            </div>
                                        </li>
                                    </ul>
                                </div>
                                <div class="bpf_workflow_approve_actions divert_actions_defalut">
                                    <span class="bpf_workflow_btn bpf_workflow_modal_sure bpf_workflow_modal_cancel" onclick="sumbit(true)">提交</span><span class="bpf_workflow_btn bpf_workflow_modal_sure" onclick="sumbit(false)">退回</span> <span class="bpf_workflow_more_action"></span>
                                    <div class="bpf_workflow_approve_more_bg"><span>加签</span><span>转发</span></div>
                                </div>
                                <div class="bpf_workflow_approve_actions divert_actions">
                                    <span class="bpf_workflow_btn bpf_workflow_modal_sure">转发</span>
                                </div>
                            </div>
                            <div class="bpf_workflow_modal bpf_workflow_personal_modal" style="display: none;">
                                <div class="bpf_workflow_modal_bg">
                                    <h2>（）</h2>
                                    <span class="bpf_workflow_modal_close"></span>
                                    <div class="bpf_workflow_modal_body">
                                        <ul class="bpf_workflow_modal_body_info">
                                            <li><span>部门：</span></li>
                                            <li><span>职位：</span></li>
                                        </ul>
                                        <!---->
                                        <div class="bpf_workflow_modal_actions"><span class="bpf_workflow_btn_remove" style="display: none;">移除</span> <span class="bpf_workflow_btn_close" style="width: 94%;">关闭</span></div>
                                    </div>
                                </div>
                            </div>
                            <div class="bpf_workflow_modal" style="display: none;">
                                <div class="bpf_workflow_modal_bg">
                                    <h2>退回</h2>
                                    <span class="bpf_workflow_modal_close"></span>
                                    <div class="bpf_workflow_modal_body">
                                        <div class="bpf_workflow_modal_form">
                                            <label for="" class="bpf_workflow_modal_label"><span class="bpf_workflow_red">*</span>退回到</label>
                                            <div class="bpf_workflow_received_list">
                                                <select name="select_name"></select>
                                            </div>
                                        </div>
                                        <div class="bpf_workflow_modal_form">
                                            <label for="" class="bpf_workflow_modal_label"><span class="bpf_workflow_red">*</span>退回意见</label>
                                            <div class="bpf_workflow_received_list">
                                                <textarea name="" id="" cols="30" rows="10" class="bpf_workflow_modal_textarea"></textarea></div>
                                        </div>
                                        <div class="bpf_workflow_modal_btns"><span class="bpf_workflow_btn bpf_workflow_modal_sure">确定</span> <span class="bpf_workflow_btn bpf_workflow_modal_cancel">取消</span></div>
                                    </div>
                                </div>
                            </div>

                            <div class="bpf_workflow_modal" style="display: none;">
                                <div class="bpf_workflow_modal_bg">
                                    <h2>加签</h2>
                                    <span class="bpf_workflow_modal_close"></span>
                                    <div class="bpf_workflow_modal_body">
                                        <div class="bpf_workflow_modal_form">
                                            <div>
                                                <label for="" class="bpf_workflow_modal_label"><span class="bpf_workflow_red">*</span>加签</label></div>
                                            <div class="bpf_workflow_received_list add_tag">
                                                <p class="bpf_workflow_text_link" onclick="isAddTag(2)"><span class="bpf_workflow_blue">添加加签人</span></p>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>韩朝 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>范冰 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>罗秀 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>石玉 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>梁德斌 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>王宁 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>仝倩倩 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>郭福 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>杨青 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>王强 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>于福 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>祖弘 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>刘增 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>郑桂 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <span class="bpf_workflow_add_tag" onclick="isAddTag(2)"></span>
                                            </div>
                                        </div>
                                        <div class="bpf_workflow_modal_form">
                                            <label for="currentAddNodeInfo.cloneNode" class="bpf_workflow_modal_label">节点</label>
                                            <div class="bpf_workflow_received_list">
                                                <select class="bpf_workflow_modal_select">
                                                    <option value="[object Object]">审批【范冰 】</option>
                                                </select>
                                                <input type="radio" id="bpf_workflow_after" name="radio_input" value="1" checked="checked">
                                                <label for="bpf_workflow_after" class="bpf_workflow_radio">之后                                </label>
                                                <input type="radio" id="bpf_workflow_before" name="radio_input" value="0">
                                                <label for="bpf_workflow_before" class="bpf_workflow_radio">之前                                </label>
                                            </div>
                                        </div>
                                        <div class="bpf_workflow_modal_form">
                                            <label for="" class="bpf_workflow_modal_label">审批类型</label>
                                            <div class="bpf_workflow_received_list">
                                                <input type="radio" id="bpf_workflow_order" name="radio_type" value="1" checked="checked">
                                                <label for="bpf_workflow_order" class="bpf_workflow_radio">顺序审批                                </label>
                                                <input type="radio" id="bpf_workflow_both" name="radio_type" value="2">
                                                <label for="bpf_workflow_both" class="bpf_workflow_radio">同时审批                                </label>
                                                <input type="radio" id="bpf_workflow_tz" name="radio_type" value="3" style="display: none;">
                                                <label for="bpf_workflow_tz" class="bpf_workflow_radio">通知                                </label>
                                            </div>
                                        </div>
                                        <div class="bpf_workflow_modal_form">
                                            <label for="" class="bpf_workflow_modal_label">加签节点名称</label>
                                            <div class="bpf_workflow_received_list">
                                                <p class="bpf_workflow_modal_nodename">
                                                    <input type="text" placeholder="" name="tagName" value="范冰 加签"></p>
                                            </div>
                                        </div>
                                        <div class="bpf_workflow_modal_form idea" style="display: none;">
                                            <label for="" class="bpf_workflow_modal_label"><span class="bpf_workflow_red">*</span>加签意见</label>
                                            <div class="bpf_workflow_received_list">
                                                <textarea name="" id="" cols="30" rows="10" class="bpf_workflow_modal_textarea bpf_workflow_add_tags"></textarea></div>
                                        </div>
                                        <div class="bpf_workflow_modal_btns"><span class="bpf_workflow_btn bpf_workflow_modal_sure add_tag_sure">确定</span> <span class="bpf_workflow_btn bpf_workflow_modal_cancel">取消</span></div>
                                    </div>
                                </div>
                            </div>
                            <div class="bpf_workflow_modal" style="display: none;">
                                <div class="bpf_workflow_modal_bg">
                                    <h2>转发</h2>
                                    <span class="bpf_workflow_modal_close"></span>
                                    <div class="bpf_workflow_modal_body">
                                        <div class="bpf_workflow_modal_form">
                                            <div>
                                                <label for="" class="bpf_workflow_modal_label"><span class="bpf_workflow_red">*</span>接收人</label></div>
                                            <div class="bpf_workflow_received_list divert_tag">
                                                <p class="bpf_workflow_text_link" onclick="isAddTag(3)"><span class="bpf_workflow_blue">添加接收人</span></p>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>韩朝 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>范冰 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>罗秀 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>石玉 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>梁德斌 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>王宁 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>仝倩倩 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>郭福 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>杨青 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>王强 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>于福 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>祖弘 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>刘增 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <div class="bpf_workflow_tag bpf_workflow_tag_bg" datatype="1"><span>郑桂 </span><span class="bpf_workflow_tag_icon"></span></div>
                                                <span class="bpf_workflow_add_tag" style="display: none;" onclick="isAddTag(3)"></span>
                                            </div>
                                        </div>
                                        <div class="bpf_workflow_modal_form">
                                            <label for="" class="bpf_workflow_modal_label"><span class="bpf_workflow_red">*</span>转发意见</label>
                                            <div class="bpf_workflow_received_list">
                                                <textarea name="" id="" cols="30" rows="10" class="bpf_workflow_modal_textarea divert_textarea"></textarea></div>
                                        </div>
                                        <div class="bpf_workflow_modal_btns"><span class="bpf_workflow_btn bpf_workflow_modal_sure divert">确定</span> <span class="bpf_workflow_btn bpf_workflow_modal_cancel">取消</span></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>--%>

                </div>
            </div>
    </section>

</asp:Content>
