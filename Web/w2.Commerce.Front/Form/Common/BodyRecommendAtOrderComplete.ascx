<%--
=========================================================================================================
  Module      : 注文完了画面用レコメンド表示出力コントローラ(BodyRecommendAtOrderComplete.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyRecommendAtOrderComplete.ascx.cs" Inherits="Form_Common_BodyRecommendAtOrderComplete" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>
	
--%>
<%if (Constants.RECOMMEND_OPTION_ENABLED) { %>
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
		background-color:#ed16ed;
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

		<div class="orderComplete">
			<div class="background">
			<div class="bottom">
			<div>
			<h3>おすすめ商品</h3>
			</div>
			<div>
			<div class="productList">
			<div class="product" style="border-top:none;border-bottom:none;">
			<div id ="recommendLink">
				<%--頒布会ドロップダウンリスト --%>
				<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
					<asp:DropDownList id="ddlSubscriptionCourseId" Visible="False" runat="server" />
				<% } %>
			<%-- ▽編集可能領域：コンテンツ▽ --%>
				<%-- レコメンド表示 --%>
				<%= this.RecommendDisplay %>
				<%-- △編集可能領域△ --%>
				<p class="clr"></p>
			</div>
			</div>
			<div style="width:400px;margin-left:auto;margin-top:5px;" align="right">
				<%-- 注文実行リンク--%>
				<asp:LinkButton ID="lbOrder" runat="server" OnClick="lbOrder_Click" />
			</div>
		</div>
		<% if(Constants.ENABLES_ORDERCONFIRM_MODAL_WINDOW_ON_RECOMMENDATION_AT_ORDERCOMPLETE == false){ %>
		<%-- ▽定期価格表記▽ --%>
		<div class="fixedPurchaseOrderPrice">
		<asp:Repeater ID="rFixedPurchaseOrderPrice" ItemType="w2.App.Common.Order.CartObject" runat="server">
			<HeaderTemplate>
				<div style="width:340px;float:left;margin:5px 21px 9px 21px;">
			</HeaderTemplate>
			<ItemTemplate>
				<uc:BodyFixedPurchaseOrderPrice runat="server" Cart="<%# Item %>" />
			</ItemTemplate>
			<FooterTemplate>
				</div>
			</FooterTemplate>
		</asp:Repeater>
		<p class="clr"></p>
		</div>
		<% } %>
	</div>
</div>
</div>
</div>
<% } %>
