<%--
=========================================================================================================
  Module      : SP用注文完了画面用レコメンド表示出力コントローラ(BodyRecommendAtOrderComplete.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/SmartPhone/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyRecommendAtOrderComplete.ascx.cs" Inherits="Form_Common_BodyRecommendAtOrderComplete" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<style  type="text/css">
	div.fixedPurchaseOrderPrice div.subCartList div.message{
		padding:10px;
	}
	div.fixedPurchaseOrderPrice div.subCartList div.message p{
		font-size:9px;
		align-content:center;
		text-align:left;
	}
	div.fixedPurchaseOrderPrice div.subCartList div.bottom{
		width:340px;
		background:url(../../Contents/ImagesPkg/common/btm_blockA.gif) no-repeat left bottom;
	}
	div.fixedPurchaseOrderPrice div.subCartList h3{
		background:none;
		
	}
	div.fixedPurchaseOrderPrice div.subCartList div.block{
		display:block;
		padding:5px 21px 9px 21px;
	}
	div.fixedPurchaseOrderPrice div.subCartList{
		margin-top:10px;
		background:url(../../Contents/ImagesPkg/common/bg_blockA.gif) repeat-y left top;
	}
</style>
<%-- レコメンドボタン押下した際の処理 --%>
<script type="text/javascript">
	<!--
	<% if (Constants.ENABLES_ORDERCONFIRM_MODAL_WINDOW_ON_RECOMMENDATION_AT_ORDERCOMPLETE == false) { %>
	$(function () {
		$("div#recommendLink a").attr("OnClick", "return exec_submit()");
	});
	<% } else { %>
	$(document).ready(function () {
		$('#recommendLink a').click(function () {
			$('#modalRecommend').css("display", "block");
			return false;
		});
	});
	<% } %>
	//-->
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- レコメンド表示 --%>
<div id="recommendLink">
<%= this.RecommendDisplay %>
</div>
<%-- △編集可能領域△ --%>
<%-- 注文実行リンク--%>
<asp:LinkButton ID="lbOrder" runat="server" OnClick="lbOrder_Click" />
<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
	<asp:Repeater ID="rSubscriptionBoxCourseList" runat="server">
		<ItemTemplate>
			<asp:HiddenField ID="hfVariationId" runat="server" />
			<asp:DropDownList id="ddlSubscriptionCourseId" DataTextField="Text" DataValueField="Value" Visible="False" runat="server" AutoPostBack="True" />
		</ItemTemplate>
	</asp:Repeater>
<% } %>

<%-- ▽定期価格表記▽ --%>
<div class="fixedPurchaseOrderPrice">
<asp:Repeater ID="rFixedPurchaseOrderPrice" ItemType="CartObject" runat="server">
	<HeaderTemplate>
	</HeaderTemplate>
	<ItemTemplate>
		<uc:BodyFixedPurchaseOrderPrice runat="server" Cart="<%# Item %>" />
	</ItemTemplate>
	<FooterTemplate>
	</FooterTemplate>
</asp:Repeater>
<p class="clr"></p>
</div>
