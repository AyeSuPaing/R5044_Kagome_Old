<%--
=========================================================================================================
  Module      : 楽天カード入力モーダルウィンドウ(RakutenCreditCardModal.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
﻿﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RakutenCreditCardModal.ascx.cs" Inherits="Form.Common.Form_Common_RakutenCreditCardModal" %>
<asp:HiddenField id="hfMyCardMount" Value="<%# this.CreditCardNo4 %>" runat="server" />
<asp:HiddenField id="hfMyCvvMount" Value="<%# this.SecurityCode %>" runat="server" />
<asp:HiddenField id="hfMyExpirationYearMount" Value="<%# this.SelectedExpireYear %>" runat="server" />
<asp:HiddenField id="hfMyExpirationMonthMount" Value="<%# this.SelectedExpireMonth %>" runat="server" />
<asp:HiddenField id="hfAuthorNameCard" Value="<%# this.AuthorName %>" runat="server" />
<asp:HiddenField id="hfCreditCardCompany" Value="<%# this.CreditCompany %>" runat="server" />
<asp:HiddenField id="hfTokenIsExpected" runat="server" />
<button id="edit-button-<%#: this.CartIndex %>" class="btn btn-mini" onclick="RakutenPaymentPayvault.initialize(<%#: this.CartIndex %>); return false;">入力</button>
<div id="modal-rakuten-payment-<%#: this.CartIndex %>" style="z-index: 101; top: 0; left: 0; width: 100%; height: 120%; position: fixed; background-color: rgba(128, 128, 128, 0.75); display:none;">
	<div id="dvUserBox">
	<div id="dvUserFltContents">
	<div id="dvUserCreditCardInput">
	<div id="rakuten-payment-<%#: this.CartIndex %>" data-cart-index="<%#: this.CartIndex %>" class="rakuten-payment-form">
		<h1>クレジットカード情報</h1>
		<div id="rakuten-payvault-form-<%#: this.CartIndex %>">
			<%-- If createToken is successful, data of each iframe field from Payvault would be rendered here --%>
		</div>
		<div class="rakuten-payment-modal" cellspacing="0">
		<table cellspacing="0">
			<tr>
				<th>
					<strong>カード番号</strong>&nbsp;<span class="fred">※</span>
				</th>
				<td>
					<div id="rakuten-card-mount-<%#: this.CartIndex %>"><%-- Payvault iframe card number is rendered here --%></div>
					<small class="fred">
					<span id="cvCreditCardNo1Rakuten-<%#: this.CartIndex %>" class="error_inline" style="color:Red;visibility:hidden;"></span>
					</small>
					<small class="fgray">
						カードの表記のとおりご入力ください。<br />
						例：<br />
						1234567890123456（ハイフンなし）
					</small>
				</td>
			</tr>
			<tr>
				<th>
					<strong>有効期限</strong>&nbsp;<span class="fred">※</span>
				</th>
				<td>
					<div style="display: flex; align-items: center;">
						<div id="rakuten-expiration-month-mount-<%#: this.CartIndex %>" style="width: 50px;"><%-- Payvault iframe expiration month is rendered here --%></div>&nbsp;/&nbsp;
						<div id="rakuten-expiration-year-mount-<%#: this.CartIndex %>"><%-- Payvault iframe expiration year is rendered here --%></div>
					</div>
					<span id="cvCreditExpire-<%#: this.CartIndex %>" class="error_inline" style="color:Red;visibility:hidden;"></span>
				</td>
			</tr>
			<tr>
				<th>
					<strong>カード名義人</strong>&nbsp;<span class="fred">※</span>
				</th>
				<td>
					<p>
						<asp:TextBox id="tbCreditAuthorNameRakuten" name="tbCreditAuthorNameRakuten" runat="server" MaxLength="50" Text="" class="input_widthB input_border" autocomplete="off" Type="text" title=""></asp:TextBox>
						<small class="fred">
							<asp:CustomValidator ID="cvCreditAuthorNameRakuten" runat="Server"
								ControlToValidate="tbCreditAuthorNameRakuten"
								ValidationGroup="OrderPayment"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								CssClass="error_inline" />
						</small>
					</p>
					<p>例：「TAROU YAMADA」</p>
				</td>
			</tr>
			<tr>
				<th>
					<strong>セキュリティコード</strong>&nbsp;<span class="fred">※</span>
				</th>
				<td>
					<div id="rakuten-cvv-mount-<%#: this.CartIndex %>"><%-- Payvault iframe security code (cvv) is rendered here --%></div>
					<span id="cvCreditSecurityCode-<%#: this.CartIndex %>" class="error_inline" style="color:Red;visibility:hidden;"></span>
					<p>セキュリティコードは、クレジットカードの背面または前面に記載されている３～４桁の番号です。</p>
				</td>
			</tr>
			<tr runat="server" visible="<%# this.IsOrder && OrderCommon.CreditInstallmentsSelectable %>">
				<th>
					<strong>支払回数</strong>&nbsp;<span class="fred">※</span>
				</th>
				<td>
					<p><asp:DropDownList id="dllCreditInstallmentsRakuten" runat="server" DataSource="<%# this.InstallmentCodeList %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# this.SelectedInstallmentCode %>" CssClass="input_border"></asp:DropDownList>
				<br/><span class="fgray">※AMEX/DINERSは一括のみとなります。</span></p>
				</td>
			</tr>
		</table>
			</div>
		<div style="padding-top: 10px;  margin-left: 714px;" >
			<button id="btnClose" class="btn btn-success" onclick="CloseCreditCardInputModal(<%#: this.CartIndex %>); return false;">閉じる</button>
		</div>
	</div>
	</div>
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