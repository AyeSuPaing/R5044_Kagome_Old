<%--
=========================================================================================================
  Module      : 定期購入情報解約完了画面(FixedPurchaseCancelComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseCancelComplete.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseCancelComplete" Title="定期購入情報変更完了ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">
	<h2>定期購入情報詳細</h2>
	<div id="dvFixedPurchaseDetail" class="unit">
		<p class="completeInfo">
			定期購入情報の<% if (this.IsCancel) { %>解約<% } %><% if (this.IsSuspend) { %>休止<% } %>を受け付けました。<br />
			今後ともどうぞよろしくお願い申し上げます。
		</p>
		<div class="dvUserBtnBox">
			<p>
				<asp:LinkButton Text="詳細へ戻る" runat="server" OnClick="lbFixedPurchaseDetail_Click" class="btn btn-large btn-inverse" />
				<a href="<%: Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST %>" class="btn btn-large btn-inverse">一覧へ戻る</a>
			</p>
		</div>
	</div>
</div>
</asp:Content>