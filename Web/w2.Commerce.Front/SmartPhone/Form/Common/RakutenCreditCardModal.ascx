<%--
=========================================================================================================
  Module      : 楽天カード入力フォームコントロール(RakutenCreditCardModal.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
﻿﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/RakutenCreditCardModal.ascx.cs" Inherits="Form.Common.Form_Common_RakutenCreditCardModal" %>
<asp:HiddenField id="hfMyCardMount" Value="<%# this.CreditCardNo4 %>" runat="server" />
<asp:HiddenField id="hfMyCvvMount" Value="<%# this.SecurityCode %>" runat="server" />
<asp:HiddenField id="hfMyExpirationYearMount" Value="<%# this.SelectedExpireYear %>" runat="server" />
<asp:HiddenField id="hfMyExpirationMonthMount" Value="<%# this.SelectedExpireMonth %>" runat="server" />
<asp:HiddenField id="hfAuthorNameCard" Value="<%# this.AuthorName %>" runat="server" />
<asp:HiddenField id="hfCreditCardCompany" Value="<%# this.CreditCompany %>" runat="server" />

<button id="edit-button-<%#: this.CartIndex %>" onclick="RakutenPaymentPayvault.initialize(<%#: this.CartIndex %>); return false;">入力</button>
<div id="modal-rakuten-payment-<%#: this.CartIndex %>" style="z-index: 20000; top: 0; left: 0; width: 100%; height: 120%; position: fixed; background-color: rgba(128, 128, 128, 0.75); display:none;" class="rakuten-credit-form">
	<div id="rakuten-payment-<%#: this.CartIndex %>" OnClientClick="<%#: this.CartIndex %>" data-cart-index="<%#: this.CartIndex %>" class="rakuten-payment-form">
		<h3>クレジットカード情報</h3>
		<ins><span class="necessary">*</span>は必須入力となります。</ins>
		<div id="rakuten-payvault-form-<%#: this.CartIndex %>">
			<%-- If createToken is successful, data of each iframe field from Payvault would be rendered here --%>
		</div>
		<ul>
			<li class="card-nums">
				<h4>カード番号<span class="require">※</span></h4>
				<div id="rakuten-card-mount-<%#: this.CartIndex %>"><%-- Payvault iframe card number is rendered here --%></div>
				<p class="attention">
				<span id="cvCreditCardNo1Rakuten-<%#: this.CartIndex %>" class="error_inline" style="color:Red;visibility:hidden;"></span>
				</p>
				<p style="text-align:left;">
					カードの表記のとおりご入力ください。<br />
					例：<br />
					1234567890123456（ハイフンなし）
				</p>
			</li>
			<li class="card-exp">
				<h4>有効期限<span class="require">※</span></h4>
				<div style="display: flex; align-items: center;">
					<div id="rakuten-expiration-month-mount-<%#: this.CartIndex %>" style="width: 50px;"><%-- Payvault iframe expiration month is rendered here --%></div>&nbsp;/&nbsp;
					<div id="rakuten-expiration-year-mount-<%#: this.CartIndex %>"><%-- Payvault iframe expiration year is rendered here --%></div>
				</div>
				<p class="attention">
					<span id="cvCreditExpire-<%#: this.CartIndex %>" class="error_inline" style="color:Red;visibility:hidden;"></span>
				</p>
			</li>
			<li class="card-name">
				<h4>カード名義人<span class="require">※</span></h4>&nbsp;
				<asp:TextBox id="tbCreditAuthorNameRakuten" name="tbCreditAuthorNameRakuten" runat="server" MaxLength="50" Text="" class="input_widthB input_border" autocomplete="off" Type="text" title=""></asp:TextBox>
				<p class="attention">
					<asp:CustomValidator ID="cvCreditAuthorNameRakuten" runat="Server"
						ControlToValidate="tbCreditAuthorNameRakuten"
						ValidationGroup="OrderPayment"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ClientValidationFunction="ClientValidate"
						CssClass="error_inline" />
				</p>
				<p style="text-align:left;">例：「TAROU YAMADA」</p>
			</li>
			<li class="card-sequrity">
				<h4>セキュリティコード<span class="require">※</span></h4>
				<div id="rakuten-cvv-mount-<%#: this.CartIndex %>"><%-- Payvault iframe security code (cvv) is rendered here --%></div>
				<span id="cvCreditSecurityCode-<%#: this.CartIndex %>" class="error_inline" style="color:Red;visibility:hidden;"></span>
				<p>セキュリティコードは、クレジットカードの背面または前面に記載されている３～４桁の番号です。</p>
			</li>
			<li class="card-sequrity" runat="server" visible="<%# this.IsOrder && OrderCommon.CreditInstallmentsSelectable %>">
				<h4>支払回数<span class="require">※</span></h4>
				<p style="text-align:left;"><asp:DropDownList id="dllCreditInstallmentsRakuten" runat="server" DataSource="<%# this.InstallmentCodeList %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# this.SelectedInstallmentCode %>" CssClass="input_border"></asp:DropDownList>
				<br/><span class="fgray">※AMEX/DINERSは一括のみとなります。</span></p>
			</li>
		</ul>
		<div>
			<button id="btnClose" class="btn" onclick="CloseCreditCardInputModal(<%#: this.CartIndex %>); return false;">閉じる</button>
		</div>
	</div>
</div>
<div id="temp-card-info-<%#: this.CartIndex %>">
	<strong>カード番号</strong>
	<p>
		XXXXXXXXXXXX<asp:Label ID="lLastFourDigitForTokenAcquiredRakuten" Text="<%# this.CreditCardNo4 %>" runat="server"></asp:Label><br />
	</p>
	<strong>有効期限</strong>
	<p>
		<asp:Label ID="lExpirationMonthForTokenAcquiredRakuten" Text="<%# this.SelectedExpireMonth %>" runat="server"></asp:Label>
		&nbsp;/&nbsp;
		<asp:Label ID="lExpirationYearForTokenAcquiredRakuten" Text="<%# this.SelectedExpireYear %>" runat="server"></asp:Label> (月/年)
	</p>
	<strong>カード名義人</strong>
	<p>
		<asp:Label ID="lCreditAuthorNameForTokenAcquiredRakuten" Text="<%# this.AuthorName %>" runat="server"></asp:Label><br />
	</p>
</div>