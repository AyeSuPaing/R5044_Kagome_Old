<%--
=========================================================================================================
  Module      : スマートフォン用定期購入情報解約完了画面(FixedPurchaseCancelComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseCancelComplete.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseCancelComplete" Title="定期購入情報変更完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="user-unit">
	<div class="content">
		<h2>変更完了</h2>
		<p class="msg">
			定期購入情報の<% if (this.IsCancel) { %>解約<% } %><% if (this.IsSuspend) { %>休止<% } %>を受け付けました。<br />
			今後ともどうぞよろしくお願い申し上げます。
		</p>
		<div class="button">
			<p>
				<asp:LinkButton ID="lbFixedPurchaseDetail" runat="server" OnClick="lbFixedPurchaseDetail_Click" class="btn">詳細へ戻る</asp:LinkButton>
				<br />
				<a href="<%: Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST %>" class="btn">一覧へ戻る</a>
			</p>
		</div>
	</div>
</div>
</asp:Content>