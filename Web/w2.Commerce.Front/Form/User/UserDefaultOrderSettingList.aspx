<%--
=========================================================================================================
  Module      : 注文方法設定一覧画面(UserDefaultOrderSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserDefaultOrderSettingList.aspx.cs" Inherits="Form_User_UserDefaultOrderSettingList" Title="注文方法設定一覧ページ" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserFltContents">

	<h2>注文方法の保存</h2>

	<div id="dvUserModifyInput" class="unit">

		<%-- メッセージ --%>
		<div class="dvContentsInfo">
			<p>既定の注文方法をご変更する場合は、「編集する」ボタンをクリックして下さい。</p>
		</div>
		
		<%-- 注文完了メッセージ --%>
		<% if (this.IsDispCompleteMessageForUserDefaultOrderSetting) { %>
			<span style="font-weight:bold;font-size:14px;">注文方法の保存が完了しました。</span>
			<br />
			<br />
			<br />
		<%} %>


		<% if (Constants.GIFTORDER_OPTION_ENABLED == false || Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED) {%>
		<div class="dvDefaultOrderSetting">
			<h3>既定の配送先住所</h3>
				<table cellspacing="0">
					<tr id="trDefaultShippingName" runat="server">
						<th>配送先名</th>
						<td>
							<asp:Literal ID="lShippingName" runat="server"></asp:Literal>
							<asp:HiddenField ID="hfShippingNo" runat="server" />
						</td>
					</tr>
					<tr id="trDefaultShippingInfo" runat="server">
						<th>お届け先</th>
						<td>
							<% if (this.IsShippingAddrJp) { %>
							〒<asp:Literal ID="lShippingZip" runat="server"></asp:Literal><br />
							<% } %>
							<asp:Literal ID="lShippingAddr1" runat="server"></asp:Literal>
							<asp:Literal ID="lShippingAddr2" runat="server"></asp:Literal><br />
							<asp:Literal ID="lShippingAddr3" runat="server"></asp:Literal>
							<asp:Literal ID="lShippingAddr4" runat="server"></asp:Literal><br />
							<asp:Literal ID="lShippingAddr5" runat="server"></asp:Literal>
							<% if (this.IsShippingAddrJp == false) { %>
							<asp:Literal ID="lShippingZipGlobal" runat="server"></asp:Literal><br />
							<% } %>
							<asp:Literal ID="lShippingCountryName" runat="server"></asp:Literal>
							<asp:Literal ID="lShippingName1" runat="server"></asp:Literal><asp:Literal ID="lShippingName2" runat="server"></asp:Literal>&nbsp;様
							<% if (this.IsShippingAddrJp) { %>
							（<asp:Literal ID="lShippingNameKana1" runat="server"></asp:Literal><asp:Literal ID="lShippingNameKana2" runat="server"></asp:Literal>&nbsp;さま）
							<% } %>
							<br />
							<asp:Literal ID="lShippingTel1" runat="server"></asp:Literal>
						</td>
					</tr>
				</table>
		</div>
		<%} %>
		<br />
		<br />
		<div class="dvDefaultOrderSetting">
			<h3>既定のお支払方法</h3>
			<table cellspacing="0">
				<tr>
					<th>お支払方法</th>
					<td>
						<asp:Literal ID="lDefaultPayment" runat="server" />
						<asp:HiddenField ID="hfPaymentId" runat="server" />
					</td>
				</tr>
				<% if (((hfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
						&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten))
					|| OrderCommon.IsPaymentCvsTypeZeus
					|| OrderCommon.IsPaymentCvsTypePaygent)
				{ %>
				<tr id="trCvsType" runat="server">
					<th>支払いコンビニ</th>
					<td>
						<asp:Literal ID="lCvsTypeName" runat="server" />
						<asp:HiddenField ID="lCvsType" runat="server" />
						<asp:HiddenField ID="hfCvsType" runat="server" />
					</td>
				</tr>
				<% } %>
				<div id="dvUserCreditCardInfo" runat="server" visible="false" class="dvUserCreditCardInfo">
					<tr id="trDefaultCardDispName" runat="server">
						<th>クレジットカード登録名</th>
						<td>
							<asp:Literal ID="lCardDispName" runat="server"></asp:Literal>
							<asp:HiddenField ID="hfUserCreditCardBranchNo" runat="server" />
						</td>
					</tr>
					<tr>
						<th>登録カード詳細</th>
						<td>
							<% if (OrderCommon.CreditCompanySelectable) { %>
							<ul>
								<li class="itemname">カード会社&nbsp;：<asp:Literal ID="lCardCompanyName" runat="server"></asp:Literal></li>
							</ul>
							<% } %>
							<ul>
								<li class="itemname">カード番号&nbsp;：XXXXXXXXXXXX<asp:Literal ID="lLastFourDigit" runat="server"></asp:Literal></li>
							</ul>
							<ul>
								<li class="itemname">有効期限&nbsp;&nbsp;&nbsp;&nbsp;：<asp:Literal ID="lExpirationMonth" runat="server"></asp:Literal>/<asp:Literal ID="lExpirationYear" runat="server"></asp:Literal> (月/年)</li>
							</ul>
							<ul>
								<li class="itemname">カード名義&nbsp;：<asp:Literal ID="lAuthorName" runat="server"></asp:Literal></li>
							</ul>
						</td>
					</tr>
				</div>
			</table>
		</div>
		<%if (this.WrappedUserDefaultOrderInput.WlHfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF){%>
		<br/>
		<uc:PaymentDescriptionCvsDef runat="server" ID="ucPaymentDescriptionCvsDef" />
		<% } %>
		<div id="dvCreditCardNoErrorMessageLink" runat="server" visible="false">
			<br />
			<span style="color:red;"><asp:Literal ID="lCreditCardNoErrorMessage" runat="server"></asp:Literal></span>
		</div>
		<span style="color:red;"><asp:Literal ID="lTryLinkAfterPayErrorMessage" runat="server"></asp:Literal></span>
		<% if (OrderCommon.DisplayTwInvoiceInfo()) { %>
		<br />
		<br />
		<div class="dvDefaultOrderSetting">
			<h3>既定の電子発票</h3>
				<table cellspacing="0">
					<tr id="trDefaultInvoiceName" runat="server">
						<th>電子発票情報名</th>
						<td>
							<asp:Literal ID="lInvoiceName" runat="server"></asp:Literal>
							<asp:HiddenField ID="hfInvoiceNo" runat="server" />
						</td>
					</tr>
					<tr id="trDefaultInvoiceInfo" runat="server">
						<th>電子発票</th>
						<td>
							<p><asp:Literal ID="lUniformInvoiceInformation" runat="server"></asp:Literal></p>
							<p><asp:Literal ID="lCarryTypeInformation" runat="server"></asp:Literal></p>
							<p><asp:Literal ID="lUniformInvoiceTypeOption1" runat="server"></asp:Literal></p>
							<p><asp:Literal ID="lUniformInvoiceTypeOption2" runat="server"></asp:Literal></p>
						</td>
					</tr>
				</table>
		</div>
		<% } %>
		<div class="dvUserBtnBox">
			<p>
				<span><a href="<%: (Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn btn-large">
					戻る</a></span>
				<span><asp:LinkButton ID="lbEdit"  OnClientClick="return exec_submit();" runat="server" class="btn btn-large btn-inverse" AutoPostBack="true" OnClick="lbEdit_Click">
					編集する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>

</asp:Content>
