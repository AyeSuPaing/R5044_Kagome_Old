<%--
=========================================================================================================
  Module      : 注文方法設定一覧画面(UserDefaultOrderSettingList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ page language="C#" masterpagefile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserDefaultOrderSettingList.aspx.cs" Inherits="Form_User_UserDefaultOrderSettingList" title="注文方法設定一覧ページ"  MaintainScrollPositionOnPostBack="true" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="user-unit">
	<h2>注文方法の保存</h2>
	<div class="msg">
		<p>既定の注文方法をご変更する場合は、「編集する」ボタンをクリックして下さい。</p>
	</div>

	<% if (this.IsDispCompleteMessageForUserDefaultOrderSetting) { %>
		<span style="padding:0 18px;font-weight:bold;">注文方法の保存が完了しました。</span>
		<br />
		<br />
	<%} %>

	<div style="padding:0 18px">
		<% if (Constants.GIFTORDER_OPTION_ENABLED == false || Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED) {%>
		<div class="dvDefaultOrderSetting">
			<h3 style="color:#fff;background-color:#000">既定の配送先住所</h3>
			<dl class="user-form">
				<div id="trDefaultShippingName" runat="server">
					<dt>配送先名</dt>
					<dd>
						<asp:Literal ID="lShippingName" runat="server"></asp:Literal>
					</dd>
				</div>
				<div id="trDefaultShippingInfo" runat="server">
					<dt>お届け先</dt>
					<dd>
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
						<% } %><br />
						<asp:Literal ID="lShippingTel1" runat="server"></asp:Literal>
					</dd>
				</div>
			</dl>
		</div>
		<%} %>

		<div class="dvDefaultOrderSetting">
			<h3 style="color:#fff;background-color:#000">既定のお支払方法</h3>
			<dl class="user-form">
				<dt>
					お支払方法
				</dt>
				<dd>
					<asp:Literal ID="lDefaultPayment" runat="server" />
					<asp:HiddenField ID="hfPaymentId" runat="server" />
				</dd>
				<% if (((hfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
						&& (Constants.CONST_PAYMENT_CVS_TYPE == Constants.PaymentCvs.Rakuten.ToString()))
					|| (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Zeus)
					|| (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Paygent)) { %>
				<dt id="trCvsType" runat="server">
					支払いコンビニ
				</dt>
				<dd>
					<asp:Literal ID="lCvsTypeName" runat="server" />
				</dd>
				<% } %>
				<div id="dvUserCreditCardInfo" runat="server" visible="false" class="dvUserCreditCardInfo">
					<dt>
						クレジットカード登録名
					</dt>
					<dd>
						<asp:Literal ID="lCardDispName" runat="server"></asp:Literal></td>
					</dd>
					<dt>
						登録カード詳細
					</dt>
					<dd>
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
					</dd>
				</div>
			</dl>
		</div>

	<%if (this.WrappedUserDefaultOrderInput.WlHfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF){%>
		<div style="padding:0 18px">
			<uc:PaymentDescriptionCvsDef runat="server" ID="ucPaymentDescriptionCvsDef" />
			<br/>
		</div>
	<% } %>
	<div id="dvCreditCardNoErrorMessageLink" runat="server" visible="false" style="padding:0 18px">
		<span style="color:red;"><asp:Literal ID="lCreditCardNoErrorMessage" runat="server"></asp:Literal></span>
		<br />
		<br />
	</div>
	<span style="color:red"><asp:Literal ID="lTryLinkAfterPayErrorMessage" runat="server"></asp:Literal></span>
	<% if (OrderCommon.DisplayTwInvoiceInfo()) { %>
	<div class="dvDefaultOrderSetting">
		<h3 style="color:#fff;background-color:#000">既定の電子発票</h3>
		<dl class="user-form">
			<span id="trDefaultInvoiceName" runat="server">
			<dt>電子発票情報名</dt>
			<dd>
				<asp:Literal ID="lInvoiceName" runat="server"></asp:Literal>
				<asp:HiddenField ID="hfInvoiceNo" runat="server" />
			</dd>
			</span>
			<span id="trDefaultInvoiceInfo" runat="server">
			<dt>電子発票</dt>
			<dd>
				<p><asp:Literal ID="lUniformInvoiceInformation" runat="server"></asp:Literal></p>
				<p><asp:Literal ID="lCarryTypeInformation" runat="server"></asp:Literal></p>
				<p><asp:Literal ID="lUniformInvoiceTypeOption1" runat="server"></asp:Literal></p>
				<p><asp:Literal ID="lUniformInvoiceTypeOption2" runat="server"></asp:Literal></p>
			</dd>
			</span>
		</dl>
	</div>
	<% } %>
	</div>
	<div class="user-footer">
		<div class="button-next">
			<span><asp:LinkButton ID="lbEdit"  OnClientClick="return exec_submit();" runat="server" class="btn btn-large btn-inverse" AutoPostBack="true" OnClick="lbEdit_Click">
			編集する</asp:LinkButton></span>
		</div>
		<div class="button-prev">
			<span><a href="<%: (Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>" class="btn btn-large">戻る</a></span>
		</div>
	</div>
</div>

</asp:Content>
