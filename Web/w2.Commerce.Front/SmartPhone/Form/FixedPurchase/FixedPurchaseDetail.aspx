<%--
=========================================================================================================
  Module      : スマートフォン用定期購入情報詳細画面(FixedPurchaseDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FixedPurchase/FixedPurchaseDetail.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseDetail" Title="定期購入情報詳細ページ" MaintainScrollPositionOnPostback="true" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%@ Register TagPrefix="uc" TagName="Layer" Src="~/SmartPhone/Form/Common/Layer/SearchResultLayer.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPal" Src="~/Form/Common/Order/PaymentDescriptionPayPal.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionTriLinkAfterPay" Src="~/Form/Common/Order/PaymentDescriptionTriLinkAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaypalScriptsForm" Src="~/Form/Common/PayPalScriptsForm.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutScript" Src="~/Form/Common/Order/PaidyCheckoutScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaidyCheckoutControl" Src="~/Form/Common/Order/PaidyCheckoutControl.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionAtone" Src="~/Form/Common/Order/PaymentDescriptionAtone.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionNPAfterPay" Src="~/Form/Common/Order/PaymentDescriptionNPAfterPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionLinePay" Src="~/Form/Common/Order/PaymentDescriptionLinePay.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionPayPay" Src="~/Form/Common/Order/PaymentDescriptionPayPay.ascx" %>
<%@ Register TagPrefix="uc" TagName="EcPayScript" Src="~/Form/Common/ECPay/EcPayScript.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenCreditCard" Src="~/Form/Common/RakutenCreditCardModal.ascx" %>
<%@ Register TagPrefix="uc" TagName="RakutenPaymentScript" Src="~/Form/Common/RakutenPaymentScript.ascx" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="w2.App.Common.Order.Payment.Paidy" %>
<%@ Import Namespace="w2.Domain.UserShipping" %>
<%@ Import Namespace="w2.App.Common.Affiliate" %>
<%@ Import Namespace="w2.App.Common.SubscriptionBox" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
	<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/order.css?20180131") %>" rel="stylesheet" type="text/css" media="all" />
	<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/user.css") %>" rel="stylesheet" type="text/css" media="all" />
	<style type="text/css">
		.modal-content {
			width: 90vw;
			height: 90vh; /* for all but iOS Safari */
			height: 90svh; /* for iOS Safari */
			margin: 0;
			padding: 0;
			background-color: #ffffff;
			position: fixed;
			z-index: 1;
			left: calc(100vw - 95vw);
			top: calc(100vh - 95vh);
			box-shadow: 15px #000;
		}
		.modal-content iframe {
			width: 100%;
			height: 100%;
			border: none;
		}
		.modal-bg {
			width: 100%;
			height: 100%;
			background-color: rgba(0, 0, 0, 0.7);
			position: fixed;
			top: 0;
			left: 0;
			z-index: 0;
		}
	</style>
	<script type="text/javascript">
		window.app = {
			closeModal: () => {
				document.getElementById('<%= lCloseSubscriptionBoxProductChangeModal.ClientID %>').click();
			}
		}
	</script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
<uc:EcPayScript runat="server" ID="ucECPayScript" />
<% } %>

<asp:LinkButton style="display: none;" OnClick="lCloseSubscriptionBoxProductChangeModal_OnClick" ID="lCloseSubscriptionBoxProductChangeModal" runat="server" />

<section class="wrap-user fixed-purchase-detail order-payment">
<div class="user-unit">
	<h2>定期購入情報詳細</h2>

	<div class="content">

	<%-- 解約理由 --%>
	<% if (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus) { %>
	<h3>購入解約</h3>
	<dl class="user-form">
		<% if (this.FixedPurchaseContainer.CancelReasonId != "") { %>
		<dt>解約理由</dt>
		<dd>
			<%#: this.CancelReason.CancelReasonName %>
		</dd>
		<% } %>
		<dt>解約メモ</dt>
		<dd>
			<%# WebSanitizer.HtmlEncodeChangeToBr(this.FixedPurchaseContainer.CancelMemo) %>
		</dd>
	</dl>
	<% } %>

	<%-- 休止理由 --%>
	<% if (this.FixedPurchaseContainer.IsSuspendFixedPurchaseStatus) { %>
		<h3>購入一時休止</h3>
		<dl class="user-form">
			<dt>再開予定日</dt>
			<dd>
				<%: (this.FixedPurchaseContainer.ResumeDate != null)
					? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.ResumeDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter)
					: "指定なし" %>
			</dd>
			<dt>休止理由</dt>
			<dd>
				<%# WebSanitizer.HtmlEncodeChangeToBr(this.FixedPurchaseContainer.SuspendReason) %>
			</dd>
		</dl>
	<% } %>

	<%-- 定期購入情報 --%>
	<div runat="server" visible="<%# ((string.IsNullOrEmpty(this.OrderNowMessagesHtmlEncoded) == false) || (this.RegisteredOrderIds.Count > 0)) %>" style="color: red; margin:5px 0px 5px 5px" >
		<asp:Label ID="lbOrderNowMessages" runat="server" />
		<%if ((this.RegisteredOrderIds.Count > 0) && (string.IsNullOrEmpty(this.OrderNowMessagesHtmlEncoded) == false)) { %> <br /> <%} %>
		<asp:Repeater ID="rOrderSuccess" runat="server" Visible="<%#  (this.RegisteredOrderIds.Count > 0) %>" ItemType="System.string">
			<HeaderTemplate>定期注文(</HeaderTemplate>
			<ItemTemplate>
				<span runat="server" visible ="<%# (Container.ItemIndex > 0) %>">&nbsp;</span>
				<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateOrderHistoryDetailUrl(Item)) %>"> <%#: Item %> </a>
			</ItemTemplate>
			<FooterTemplate>)が登録されました。</FooterTemplate>
		</asp:Repeater>
	</div>
	<div runat="server" ID="divSkipedMessage" visible="False" style="color: red; margin:5px 0px 5px 5px" >
		審査内容の有効性を判定するためのSMSを送信いたしました。<br/>
		審査が完了後、今すぐ注文で注文された内容が反映されます。
	</div>
	<strong>
		<p><asp:Literal runat="server" ID="lResumeFixedPurchaseMessage" Visible="<%# string.IsNullOrEmpty(this.ResumeFixedPurchaseMessageHtmlEncoded) == false %>" /></p>
	</strong>
		<div class="dvPointResetMessages" runat="server" Visible="<%# string.IsNullOrEmpty(this.PointResetMessages) == false %>">
			<asp:Literal runat="server" ID="lPointResetMessages" />
		</div>
	<br />
	<h3>定期購入情報</h3>
	<dl class="user-form">
		<dt>定期購入ID</dt>
		<dd>
			<%: this.FixedPurchaseContainer.FixedPurchaseId%>
			<span style="color:red" runat="server" Visible="<%# ((this.IsCancelable == false) && (this.FixedPurchaseContainer.IsCompleteStatus == false)) %>">※出荷回数が<%#: this.FixedPurchaseCancelableCount %>回以上から定期購入キャンセル、一時休止が可能になります。</span>
			<div class="button">
				<asp:LinkButton ID="btnCancelFixedPurchase" Text="定期購入解約" CssClass="btn" runat="server" OnClick="btnCancelFixedPurchase_Click" />
			</div>
			<div class="button">
				<asp:LinkButton ID="btnCancelFixedPurchaseReason" Text="定期購入解約（解約理由）" CssClass="btn" runat="server" OnClick="btnCancelFixedPurchaseReason_Click" />
			</div>
			<div class="button">
				<asp:LinkButton ID="btnSuspendFixedPurchase" Text="定期購入一時休止" CssClass="btn" runat="server" OnClick="btnSuspendFixedPurchase_Click" />
			</div>
		</dd>
		<dt>定期購入設定</dt>
		<dd>
			<%: OrderCommon.CreateFixedPurchaseSettingMessage(this.FixedPurchaseContainer)%>
			<div class="button" Visible="<%# this.DisplayFixedPurchaseShipping %>" runat="server">
				<asp:LinkButton ID="lbDisplayShippingPatternInfoForm" Visible="<%# ((this.DisplayFixedPurchaseShipping) && (this.FixedPurchaseContainer.IsCompleteStatus == false)) %>" Text="パターン変更" CssClass="btn" runat="server" OnClick="lbDisplayShippingPatternInfoForm_Click" />
			</div>

			<%-- 配送パターン --%>
			<div id="dvFixedPurchaseDetailShippingPattern" visible="false" runat="server">
				<dl class="order-form">
				<dd id="dtMonthlyDate" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseDays" Text="月間隔日付指定" GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseDays_OnCheckedChanged" AutoPostBack="true" runat="server" />
				</dd>
				<dd id="ddMonthlyDate" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseMonth" style="width:50px" runat="server"></asp:DropDownList>
					ヶ月ごと
					<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate" style="width:75px" runat="server"></asp:DropDownList>
					に届ける
					<p class="attention">
						<asp:CustomValidator ID="CustomValidator5" runat="server"
											ControlToValidate="ddlFixedPurchaseMonth"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false" />
						<asp:CustomValidator ID="CustomValidator1" runat="server"
											ControlToValidate="ddlFixedPurchaseMonthlyDate"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false" />
					</p>
				</dd>
				<dd id="dtWeekAndDay" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseWeekAndDay" Text="月間隔・週・曜日指定" GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseWeekAndDay_OnCheckedChanged" AutoPostBack="true" runat="server" />
				</dd>
				<dd id="ddWeekAndDay" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths" runat="server" />
					ヶ月ごと
					<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth" style="width:70px" runat="server"></asp:DropDownList>
					<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek" style="width:90px" runat="server"></asp:DropDownList>
					に届ける
					<p class="attention">
						<asp:CustomValidator runat="server"
											ControlToValidate="ddlFixedPurchaseIntervalMonths"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false" />
						<asp:CustomValidator ID="CustomValidator2" runat="server"
											ControlToValidate="ddlFixedPurchaseWeekOfMonth"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false" />
						<asp:CustomValidator ID="CustomValidator3" runat="server"
											ControlToValidate="ddlFixedPurchaseDayOfWeek"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false" />
					</p>
				</dd>
				<dd id="dtIntervalDays" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseIntervalDays" Text="配送間隔指定" GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseIntervalDays_OnCheckedChanged" AutoPostBack="true" runat="server" />
				</dd>
				<dd id="ddIntervalDays" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseIntervalDays" style="width:80px" runat="server"></asp:DropDownList>
					ごとに届ける
					<p class="attention">
						<asp:CustomValidator ID="CustomValidator4" runat="server"
											ControlToValidate="ddlFixedPurchaseIntervalDays"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false" />
					</p>
				</dd>
				<dd id="dtFixedPurchaseEveryNWeek" runat="server">
					<asp:RadioButton ID="rbFixedPurchaseEveryNWeek" Text="週間隔・曜日指定" GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseFixedPurchaseEveryNWeek_OnCheckedChanged" AutoPostBack="true" runat="server" />
				</dd>
				<dd id="ddFixedPurchaseEveryNWeek" style="padding-left:25px" runat="server">
					<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week" style="width:50px" runat="server"></asp:DropDownList>
					週間ごと
					<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek" style="width:75px" runat="server"></asp:DropDownList>
					に届ける
					<p class="attention">
						<asp:CustomValidator runat="Server"
											ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
											ValidationGroup="OrderShipping" ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false" />
						<asp:CustomValidator runat="Server"
											ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
											ValidationGroup="OrderShipping"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false" />
					</p>
				</dd>
				<div class="user-footer">
					<div class="button-next">
						<asp:LinkButton ID="btnUpdateShippingPatternInfo" Text="配送パターン更新" runat="server" CssClass="btn" OnClientClick="return confirm('配送パターンを変更します。\n本当によろしいですか？')" OnClick="btnUpdateShippingPatternInfo_Click" />
					</div>
					<div class="button-prev">
						<asp:LinkButton ID="btnCloseShippingPatternInfo" Text="キャンセル" runat="server" CssClass="btn" OnClick="btnCloseShippingPatternInfo_Click" />
					</div>
				</div>
				</dl>
			</div>
		</dd>
		<dt>最終購入日</dt>
		<dd>
			<%: DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.LastOrderDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) %>
		</dd>
		<dt>購入回数</dt>
		<dd>
			<%: this.FixedPurchaseContainer.OrderCount%>
		</dd>
		<% if (CanUsePointForPurchase()) { %>
		<dt>次回購入の利用ポイント</dt>
		<dd>
			<% if (this.CanUseAllPointFlg && (this.FixedPurchaseContainer.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON)) { %>
				<span>全ポイント継続利用</span>
			<% } else { %>
				<%: GetNumeric(this.FixedPurchaseContainer.NextShippingUsePoint) %><%: Constants.CONST_UNIT_POINT_PT %>&nbsp;
			<% } %>
			（利用可能ポイント：<%: GetNumeric(this.LoginUserPointUsableForFixedPurchase) %><%: Constants.CONST_UNIT_POINT_PT %>）<br />
			<asp:LinkButton ID="lbDisplayUpdateNextShippingUsePointForm" Visible="<%# ((this.CanCancelFixedPurchase) && (this.BeforeCancelDeadline) && (this.FixedPurchaseContainer.IsCompleteStatus == false) && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false)) %>" Text="  利用ポイント変更  " runat="server" OnClick="lbDisplayUpdateNextShippingUsePointForm_Click" class="btn" />&nbsp;
			<div id="dvNextShippingUsePoint" visible="false" runat="server">
				<asp:TextBox ID="tbNextShippingUsePoint" runat="server"></asp:TextBox><br />
				<% if (this.CanUseAllPointFlg) { %>
				<asp:CheckBox ID="cbUseAllPointFlg" Text="全ポイント継続利用 ※以降の注文にも適用されます"
					OnCheckedChanged="cbUseAllPointFlg_Changed" OnDataBinding="cbUseAllPointFlg_DataBinding"
					CssClass="checkBox" style="font-size: 11px;" AutoPostBack="True" runat="server" />
				<% } %>
				<asp:CustomValidator runat="server"
									ControlToValidate="tbNextShippingUsePoint"
									ValidationGroup="FixedPurchaseModifyInput"
									ValidateEmptyText="true"
									SetFocusOnError="true"
									ClientValidationFunction="ClientValidate"
									EnableClientScript="false"
									CssClass="error_inline" /><br />
				<span style="color: red" ><%= this.NextShippingUsePointErrorMessage %></span>
				<asp:LinkButton ID="lbUpdateNextShippingUsePoint" Text="  利用ポイント更新  " OnClick="lbUpdateNextShippingUsePoint_Click" OnClientClick="return confirm('ご利用ポイントを変更します。よろしいですか？')" runat="server" class="btn" />&nbsp;
				<asp:LinkButton ID="lbCloseUpdateNextShippingUsePointForm" Text="  キャンセル  " OnClientClick="return exec_submit();" OnClick="lbCloseUpdateNextShippingUsePointForm_Click" runat="server" class="btn" />&nbsp;
			</div>
			<small>
<pre style="display: inline">
※入力した利用ポイントは次回購入時に適用されます。
　次回配送日の <%# this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。
　（一度入力済みの利用ポイントを減らす際、
　入力済みポイントが有効期限切れの場合には、
　ポイントが消滅します。）
※次回注文生成時、ポイント数が
　注文時の利用可能ポイント数より大きい場合、
　適用されなかった分のポイントはお客様のポイントに戻されます。
　（ポイント有効期限切れの場合は、ポイントは戻らず消滅します。）
※定期購入に期間限定ポイントを利用することはできません。
</pre>
			</small>
		</dd>
		<% } %>
		<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
		<dt>次回購入の利用クーポン</dt>
		<dd>
			<%: (this.FixedPurchaseContainer.NextShippingUseCouponDetail == null) ? "利用なし" : this.FixedPurchaseContainer.NextShippingUseCouponDetail.DisplayName %>
			<asp:LinkButton ID="lbDisplayUpdateNextShippingUseCouponForm" Visible="<%# ((this.CanAddNextShippingUseCoupon) && (this.FixedPurchaseContainer.IsCompleteStatus == false) && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false)) %>" Enabled="<%# this.BeforeCancelDeadline %>" Text="  利用クーポン変更  " runat="server" OnClick="lbDisplayUpdateNextShippingUseCouponForm_Click" class="btn" />&nbsp;
			<asp:LinkButton ID="lbCancelNextShippingUseCoupon" Visible="<%# this.CanCancelNextShippingUseCoupon %>" Enabled="<%# this.BeforeCancelDeadline %>" Text="  キャンセル  " runat="server" OnClick="lbCancelNextShippingUseCoupon_Click" class="btn" />&nbsp;
			<div id="dvNextShippingUseCoupon" visible="false" runat="server">
				<div id="divCouponInputMethod" runat="server" Visible="<%# this.UsableCoupons.Length > 0 %>">
				<asp:RadioButtonList ID="rblCouponInputMethod" runat="server" AutoPostBack="true"
					OnSelectedIndexChanged="rblCouponInputMethod_SelectedIndexChanged" OnDataBinding="rblCouponInputMethod_DataBinding"
					DataSource="<%# GetCouponInputMethod() %>" DataTextField="Text" DataValueField="Value" RepeatColumns="2" RepeatDirection="Horizontal" RepeatLayout="Flow" />&nbsp;
				</div>
				<span id="dvCouponSelectArea" runat="server" Visible="<%# this.UsableCoupons.Length > 0 %>">
					<asp:DropDownList CssClass="input_border" style="width: 240px" ID="ddlNextShippingUseCouponList" runat="server" DataSource="<%# GetUsableCouponListItems() %>" DataTextField="Text" DataValueField="Value" AutoPostBack="true"></asp:DropDownList>
				</span>
				<span id="dvCouponCodeInputArea" runat="server">
					<span>クーポンコード</span>
					<asp:TextBox ID="tbNextShippingUseCouponCode" runat="server" MaxLength="30" autocomplete="off" />
				</span>
				<span><asp:LinkButton runat="server" ID="lbShowCouponBox" Text="クーポンBOX" OnClick="lbShowCouponBox_Click"  Visible="<%# this.UsableCoupons.Length > 0 %>"
					style="color: #ffffff !important; background-color: #000 !important;
					border-color: rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.1) rgba(0, 0, 0, 0.25); text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.25); display: inline-block;
					padding: 4px 10px 4px; margin-bottom: 0; font-size: 13px; line-height: 18px; text-align: center; vertical-align: middle; cursor: pointer;
					border: 1px solid #cccccc; border-radius: 4px; box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05); white-space: nowrap;" />&nbsp;
				</span>
				<div style='color: red; margin-top:5px; margin-bottom:5px;'><%: this.NextShippingUseCouponErrorMessage %></div>
				<asp:LinkButton ID="lbUseCoupon" Text="クーポンを適用" OnClick="btnUpdateUseCoupon_Click"  OnClientClick="return confirm('ご利用クーポンを変更します。よろしいですか？');" CssClass="btn" runat="server"/>&nbsp;
				<asp:LinkButton ID="lbCancel" Text="キャンセル" OnClientClick="return exec_submit();" OnClick="lbCloseUpdateNextShippingUseCouponForm_Click" CssClass="btn" runat="server"/>&nbsp;
				<!--▽クーポンBOX▽-->
				<div runat="server" id="dvCouponBox" style="z-index: 1; top: 0; left: 0; width: 100%; height: 120%; position: fixed; background-color: rgba(128, 128, 128, 0.75);" visible="false">
				<div id="dvCouponList" style="width: 100%; height: 320px; top: 50%; left: 0; text-align: center; background: #fff; position: fixed; z-index: 200001; margin:-180px 0 0 0;">
				<h2 style="height: 20px; color: #fff; background-color: #000; margin-bottom: 0; margin-top: 0px; z-index: 20003">クーポンBOX</h2>
				<div style="height: 260px; overflow: auto; -webkit-overflow-scrolling: touch; z-index: 20003">
				<asp:Repeater ID="rCouponList" ItemType="w2.Domain.Coupon.Helper.UserCouponDetailInfo" runat="server" DataSource="<%# this.UsableCoupons %>">
					<HeaderTemplate></HeaderTemplate>
					<ItemTemplate>
						<ol>
							<h3 style="margin: 0 auto; border: 1px #888888;  background-color: #CCC; color:black; font-weight: bold;">
								<%# (StringUtility.ToEmpty(Item.CouponDispName) != string.Empty)
									? StringUtility.ToEmpty(Item.CouponDispName)
									: StringUtility.ToEmpty(Item.CouponCode) %>
							</h3>
							<dl style="text-align: left;">
							<dd style="padding: 2px; text-align: left; margin-left: 0px;">
								クーポンコード：<%#: StringUtility.ToEmpty(Item.CouponCode) %>
								<asp:HiddenField runat="server" ID="hfCouponBoxCouponCode" Value="<%# Item.CouponCode %>" />
							</dd>
							<dd style="padding: 2px; text-align: left; margin-left: 0px;">
								有効期限：<%#:DateTimeUtility.ToStringFromRegion(Item.ExpireEnd, DateTimeUtility.FormatType.LongDateHourMinute1Letter) %>
							</dd>
							<dd style="padding: 2px; text-align: left; margin-left: 0px;">
								割引金額/割引率：
								<%#: (StringUtility.ToEmpty(Item.DiscountPrice) != "")
										? CurrencyManager.ToPrice(Item.DiscountPrice)
										: (StringUtility.ToEmpty(Item.DiscountRate) != "")
											? StringUtility.ToEmpty(Item.DiscountRate) + "%"
											: "-" %>
							</dd>
							<dd style="padding: 2px; text-align: left; margin-left: 0px;">
								利用可能回数：<%#: GetCouponCount(Item) %>
							</dd>
							<dd style="padding: 2px; text-align: left; margin-left: 0px;"><%#: StringUtility.ToEmpty(Item.CouponDispDiscription) %></dd>
							</dl>
							<div style="margin: 0 auto; width: 150px; padding: 10px; height: 60px; background-color: white;">
								<asp:LinkButton runat="server" id="lbCouponSelect" OnClick="lbCouponSelect_Click"
									style="padding: .3em 0; background-color: #333; color: #fff; margin-top: 1em; text-align: center; display: block; text-decoration: none !important; line-height: 1.5;" >
									このクーポンを使う
								</asp:LinkButton>
							</div>
						</ol>
					</ItemTemplate>
					<FooterTemplate></FooterTemplate>
				</asp:Repeater>
				</div>
				<div style="width: 100%; height: 40px; left: 0; text-align: center; border: 0.5px solid #efefef; background: #fff; position: fixed; z-index: 200002;">
					<asp:LinkButton ID="lbCouponBoxClose" OnClick="lbCouponBoxClose_Click" runat="server"
						style="width: 150px; align-content:center; padding: .3em 0; background-color: #ddd; color: #333; margin-top: 1em; text-align: center; display: block; text-decoration: none !important; line-height: 1.5; margin: auto; margin-top: 7px;">
						クーポンを利用しない
					</asp:LinkButton>
				</div>
				</div>
				</div>
				<!--△クーポンBOX△-->
			</div>
			<small>
				<br />
				※入力した利用クーポンは次回購入時に適用されます。<br />
				　次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
				※次回注文生成時、クーポンが本注文に適用不可となる場合、クーポンが戻されます。
			</small>
		</dd>
		<% } %>

		<dt>定期購入ステータス</dt>
		<dd>
			<span id="spFixedPurchaseStatus" runat="server">
			<%: this.FixedPurchaseContainer.FixedPurchaseStatusText%>
			</span>
			<asp:CheckBox runat="server" ID ="cbResumeFixedPurchase" Visible="<%# this.CanResumeFixedPurchase %>" autoPostBack="true" Text="定期注文を再開" />
			<asp:CheckBox runat="server" ID ="cbResumeSubscriptionBox" Visible="<%# CanResumeCourse(this.FixedPurchaseContainer) %>" autoPostBack="true" Text="頒布会コースを再開" />
			<% lbResumeFixedPurchase.OnClientClick = "return confirm('"
				+ "定期購入を再開します。よろしいですか？\\n\\n"
				+ "■注意点\\n"
				+ "' + getResumeShippingMesasge() + '"
				+ "');"; %>
			<% lbResumeSubscriptionBox.OnClientClick = "return confirm('"
				+ "頒布会コースを再開します。よろしいですか？\\n\\n"
				+ "■注意点\\n"
				+ "' + getResumeSubscriptionBoxShippingMesasge() + '"
				+ "');"; %>
			<span runat="server" Visible='<%# this.IsInvalidResumePaypay %>'>
				<br />
				<asp:Literal ID="lCancelPaypayNotification" runat="server"/>
			</span>
		</dd>
	</dl>
	<dl class="user-footer user-form">
		<% if (cbResumeFixedPurchase.Checked == true){%>
		<dd>
			次回配送日：<asp:DropDownList ID="ddlResumeFixedPurchaseDate" runat="server" ></asp:DropDownList>
		</dd>
		<div id="dvResumeFixedPurchaseErr" class="attention" visible = "false" runat="server"></div>
		<dd class="button-next">
			<asp:LinkButton runat="server" ID="lbResumeFixedPurchase" OnClick="lbResumeFixedPurchase_Click" CssClass="btn" Text="  定期購入を再開する  "></asp:LinkButton>
		</dd>
		<dd class="button-prev">
			<asp:LinkButton runat="server" ID="lbResumeFixedPurchaseCancel" OnClick="lbResumeFixedPurchaseCancel_Click" CssClass="btn" Text="  キャンセル  "></asp:LinkButton>
		</dd>
		<% } %>
		<% if (cbResumeSubscriptionBox.Checked == true){%>
		<dd>
			次回配送日：<asp:DropDownList ID="ddlResumeSubscriptionBoxDate" runat="server" ></asp:DropDownList>
		</dd>
		<div id="dvResumeSubscriptionBoxErr" class="attention" visible = "false" runat="server"></div>
		<dd class="button-next">
			<asp:LinkButton runat="server" ID="lbResumeSubscriptionBox" OnClick="lbResumeSubscriptionBox_Click" CssClass="btn" Text="  頒布会コースを再開する  "></asp:LinkButton>
		</dd>
		<dd class="button-prev">
			<asp:LinkButton runat="server" ID="lbResumeSubscriptionBoxCancel" OnClick="lbResumeFixedPurchaseCancel_Click" CssClass="btn" Text="  キャンセル  "></asp:LinkButton> <%--@todo--%>
		</dd>
		<%} %>
	</dl>
	<dl class="user-form">
		<dt>決済ステータス</dt>
		<dd>
			<span id="spPaymentStatus" runat="server"><%: this.FixedPurchaseContainer.PaymentStatusText%></span>
		</dd>
		<dt>お支払い方法</dt>
		<dd><%: this.Payment.PaymentName %>
				<%if (string.IsNullOrEmpty(this.FixedPurchaseContainer.CardInstallmentsCode) == false) { %>
						（<%: ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, this.FixedPurchaseContainer.CardInstallmentsCode)%>払い）
				<%} %>
				<% if (this.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) { %>
					<div style="margin: 10px 0;">
						<small>※現在のAmazon Payでの配送先情報、お支払い方法を表示しています。</small>
					</div>
					<iframe id="AmazonDetailWidget" src="<%: PageUrlCreatorUtility.CreateAmazonPayWidgetUrl(true, fixedPurchaseId: this.FixedPurchaseContainer.FixedPurchaseId) %>" style="width:100%;border:none;"></iframe>
				<% } %>
				<div id="Div2" class="button" runat="server" visible="<%# ((this.CanCancelFixedPurchase) && (this.IsDisplayInputOrderPaymentKbn) && (this.FixedPurchaseContainer.IsCompleteStatus == false) && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false)) %>">
					<asp:LinkButton
						ID="lbDisplayInputOrderPaymentKbn"
						Text="お支払い方法変更"
						runat="server"
						OnClick="lbDisplayInputOrderPaymentKbn_Click" class="btn" />
				</div>
				<%-- ▼PayPalログインここから▼ --%>
				<div style="display: <%= dvOrderPaymentPattern.Visible ? "block" : "none"%>">
					<%if (Constants.PAYPAL_LOGINPAYMENT_ENABLED) {%>
						<%
							ucPaypalScriptsForm.LogoDesign = "Payment";
							ucPaypalScriptsForm.AuthCompleteActionControl = lbPayPalAuthComplete;
						%>
						<uc:PaypalScriptsForm ID="ucPaypalScriptsForm" runat="server" />
						<div id="paypal-button"></div>
						<%if (SessionManager.PayPalCooperationInfo != null) {%>
							<%: (SessionManager.PayPalCooperationInfo != null) ? SessionManager.PayPalCooperationInfo.AccountEMail : "" %> 連携済<br/>
						<%} %>
						<br /><asp:LinkButton ID="lbPayPalAuthComplete" runat="server" OnClick="lbPayPalAuthComplete_Click"></asp:LinkButton>
					<% } %>
				</div>
				<%-- ▲PayPalログインここまで▲ --%>
				<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
				<asp:HiddenField ID="hfPaidyPaySelected" runat="server" />
				<div id="dvOrderPaymentPattern" visible="false" runat="server">
					<tr><!--Input payment update form-->
						<th>お支払い情報</th>
						<td id="CartList">
							<div class="orderBox">
								<div class="list">
								<span id="Span1" style="color:red" runat="server" visible="<%# (string.IsNullOrEmpty(StringUtility.ToEmpty(this.DispLimitedPaymentMessages[0])) == false) %>">
									<%# StringUtility.ToEmpty(this.DispLimitedPaymentMessages[0]) %>
									<br/>
								</span>
								<dl class="list">
									<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL) { %>
										<dl class="order-form payment-list">
											<dt><asp:DropDownList ID="ddlPayment" runat="server" DataSource="<%# this.ValidPayments[0] %>" ItemType="w2.Domain.Payment.PaymentModel" OnSelectedIndexChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" DataTextField="PaymentName" DataValueField="PaymentId" /></dt>
										</dl>
									<% } %>
									<asp:Repeater ID="rPayment" runat="server" DataSource="<%# this.ValidPayments[0] %>" ItemType="w2.Domain.Payment.PaymentModel" >
									<HeaderTemplate>
										<dl class="order-form payment-list">
									</HeaderTemplate>
										<ItemTemplate>
											<asp:HiddenField ID="hfPaymentId" Value='<%# Item.PaymentId %>' runat="server" />
											<asp:HiddenField ID="hfPaymentName" Value='<%# Item.PaymentName %>' runat="server" />
											<% if(Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB) { %>
											<dt class="title">
												<w2c:RadioButtonGroup ID="rbgPayment" GroupName='Payment' Checked="<%# this.FixedPurchaseContainer.OrderPaymentKbn == Item.PaymentId %>" Text="<%#: Item.PaymentName %>" OnCheckedChanged="rbgPayment_OnCheckedChanged" AutoPostBack="true" runat="server" />
											</dt>
											<% } %>
											<dd id="ddCredit" class="credit-card" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) %>" runat="server">
												<div class="inner">
												<%-- クレジット --%>
												<div id="Div3" class="box-center" runat="server">
													<asp:DropDownList ID="ddlUserCreditCard" visible="<%# this.IsCreditListDropDownDisplay %>" runat="server" OnSelectedIndexChanged="ddlUserCreditCard_OnSelectedIndexChanged" AutoPostBack="true" DataTextField="text" DataValueField="value" ></asp:DropDownList>
												</div>

												<%-- ▽新規カード▽ --%>
												<% if (IsNewCreditCard()){ %>
													<div id="divCreditCardInputForm" class="new" runat="server" >
													
														<% if (this.IsCreditCardLinkPayment() == false) { %>
														<%--▼▼ クレジット Token保持用 ▼▼--%>
														<asp:HiddenField ID="hfCreditToken" Value="" runat="server" />
														<%--▲▲ クレジット Token保持用 ▲▲--%>

														<%--▼▼ カード情報取得用 ▼▼--%>
														<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%# CreateGetCardInfoJsScriptForCreditToken(Container) %>" />
														<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
														<%--▲▲ カード情報取得用 ▲▲--%>
														<div id="divRakutenCredit" runat="server">
															<asp:LinkButton id="lbEditCreditCardNoForRakutenToken" CssClass="lbEditCreditCardNoForRakutenToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
															<uc:RakutenCreditCard
																ID="ucRakutenCreditCard"
																IsOrder="true"
																CartIndex="1"
																InstallmentCodeList="<%# this.CreditInstallmentsList %>"
																runat="server"/>
														</div>
														<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
														<div id="divCreditCardNoToken" runat="server">
														<%if (OrderCommon.CreditCompanySelectable) {%>
														<li class="card-company">
															<h4>カード会社</h4>
															<div><asp:DropDownList id="ddlCreditCardCompany" runat="server" DataTextField="Text" DataValueField="Value" CssClass="input_widthG input_border"></asp:DropDownList></div>
														</li>
														<% } %>
														<li class="card-nums">
															<p class="attention">
																<asp:CustomValidator ID="cvCreditCardNo1" runat="Server"
																	ControlToValidate="tbCreditCardNo1"
																	ValidationGroup="OrderPayment"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	CssClass="error_inline" />
																<span id="sErrorMessage" runat="server" />
															</p>
															<h4>カード番号<span class="require">※</span></h4>
															<div>
																<w2c:ExtendedTextBox id="tbCreditCardNo1" Type="tel" runat="server" CssClass="tel" MaxLength="16" autocomplete="off"></w2c:ExtendedTextBox>
															</div>
															<p>
																カードの表記のとおりご入力ください。<br />
																例：<br />
																1234567890123456（ハイフンなし）
															</p>
														</li>
															
														<li class="card-exp">
															<h4>有効期限</h4>
															<div>
																<asp:DropDownList id="ddlCreditExpireMonth" runat="server" DataSource="<%# this.CreditExpireMonth %>" ></asp:DropDownList>
																	/
																<asp:DropDownList id="ddlCreditExpireYear" runat="server" DataSource="<%# this.CreditExpireYear %>" ></asp:DropDownList>
																	 (月/年)
															</div>
														</li>
														<li class="card-name">
															<h4>カード名義人<span class="require">※</span></h4>
															<div>
																<p class="attention">
																	<asp:CustomValidator ID="cvCreditAuthorName" runat="Server"
																		ControlToValidate="tbCreditAuthorName"
																		ValidationGroup="OrderPayment"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		CssClass="error_inline" />
																</p>
																<w2c:ExtendedTextBox id="tbCreditAuthorName" Type="text" runat="server" MaxLength="50" autocomplete="off"></w2c:ExtendedTextBox>
															<p>例：「TAROU YAMADA」</p>
															</div>
														</li>

														<li id="Li1" class="card-sequrity" visible="<%# OrderCommon.CreditSecurityCodeEnable %>" runat="server">
															<h4>セキュリティコード<span class="require">※</span></h4>
															<div>
																<p class="attention">
																	<asp:CustomValidator ID="cvCreditSecurityCode" runat="Server"
																		ControlToValidate="tbCreditSecurityCode"
																		ValidationGroup="OrderPayment"
																		ValidateEmptyText="true"
																		SetFocusOnError="true"
																		CssClass="error_inline" />
																</p>
																<w2c:ExtendedTextBox id="tbCreditSecurityCode" Type="tel" runat="server" MaxLength="4" autocomplete="off"></w2c:ExtendedTextBox>
																<p>
																	<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/card-sequrity-code.gif" alt="セキュリティコードとは" width="280" />
																</p>
															</div>
														</li>
														</div>
														<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

														<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
														<div id="divCreditCardForTokenAcquired" runat="server">
															<%if (OrderCommon.CreditCompanySelectable) {%>
															<li>
																<h4>カード会社</h4>
																<div><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server"></asp:Literal><br /></div>
															</li>
															<%} %>
															<li>
															<h4>カード番号 <asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton></h4>
															<div>
																<p>XXXXXXXXXXXX<asp:Literal ID="lLastFourDigitForTokenAcquired" runat="server"></asp:Literal><br /></p>
															</div>
															</li>
															<li>
																<h4>有効期限</h4>
																<div><asp:Literal ID="lExpirationMonthForTokenAcquired" runat="server"></asp:Literal>
																	&nbsp;/&nbsp;
																	<asp:Literal ID="lExpirationYearForTokenAcquired" runat="server"></asp:Literal> (月/年)</div>
															</li>
															<li>
																<h4>カード名義人</h4>
																<div><asp:Literal ID="lCreditAuthorNameForTokenAcquired" runat="server"></asp:Literal><br /></div>
															</li>
														</div>
														<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>

														<li class="card-time" id="Li2" visible="<%# OrderCommon.CreditInstallmentsSelectable && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) %>" runat="server">
															<h4>支払い回数</h4>
															<div>
																<asp:DropDownList id="dllCreditInstallments" runat="server" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
																<p>AMEX/DINERSは一括のみとなります。</p>
															</div>

														</li>
														
														</ul>
														<% } else { %>
															<div>遷移する外部サイトでカード番号を入力してください。</div>
														<% } %>
														</div>

														<div class="box-center">
														<asp:CheckBox ID="cbRegistCreditCard" visible="<%# this.IsCreditRegistCheckBoxDisplay %>" runat="server" Text="このカードを登録する" OnCheckedChanged="cbRegistCreditCard_OnCheckedChanged" AutoPostBack="true" />
														</div>

														<div id="divUserCreditCardName" class="card-save" Visible="false" runat="server">
															<p class="msg">クレジットカードを保存する場合は、以下をご入力ください。</p>
															<h4>登録名<span class="require">※</span></h4>
															<asp:TextBox ID="tbUserCreditCardName" MaxLength="100" CssClass="input_widthD input_border" runat="server"></asp:TextBox>
															<div>
																<p class="attention">
																<asp:CustomValidator ID="cvUserCreditCardName" runat="Server"
																	ControlToValidate="tbUserCreditCardName"
																	ValidationGroup="OrderPayment"
																	ValidateEmptyText="true"
																	SetFocusOnError="true" />
																</p>
															</div>
														</div>

													</div>
													<%-- △新規カード△ --%>

													<%-- ▽登録済みカード▽ --%>
													<% }else{ %>
													<div id="divCreditCardDisp" runat="server">
														<ul>
															<%if (OrderCommon.CreditCompanySelectable) {%>
															<li>
																<h4>カード会社</h4>
																<div>
																	<%: this.CreditCardCompanyName %>
																</div>
															</li>
															<%} %>
															<li>
																<h4>カード番号</h4>
																<div>
																	XXXXXXXXXXXX<%: this.LastFourDigit %>
																</div>
															</li>
															<li>
																<h4>有効期限</h4>
																<div>
																	<%: this.ExpirationMonth %>/<%: this.ExpirationYear %> (月/年)
																</div>
															</li>
															<li>
																<h4>カード名義人</h4>
																<div>
																	<%: this.CreditAuthorName %>
																</div>
															</li>
															<li id="Li3" visible="<%# OrderCommon.CreditInstallmentsSelectable %>" runat="server">
																<h4>支払い回数</h4>
																<div><asp:DropDownList id="dllCreditInstallments2" runat="server" DataSource="<%# this.CreditInstallmentsList %>" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
																<p class="attention">※AMEX/DINERSは一括のみとなります。</p></div>
															</li>
														</ul>
													</div>
													<% } %>
													<%-- △登録済みカード△ --%>
												</div>
											</dd>

											<%-- コンビニ(後払い) --%>
										<dd id="ddCvsDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF) %>" runat="server">
											<uc:PaymentDescriptionCvsDef runat="server" id="ucPaymentDescriptionCvsDef" />
										</dd>

											<%-- 後付款(TriLink後払い) --%>
											<dd id="ddTriLinkAfterPayPayment" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY) %>" runat="server">
												<uc:PaymentDescriptionTriLinkAfterPay runat="server" id="ucPaymentDescriptionTryLinkAfterPay" />
											</dd>

											<%-- Amazon Pay --%>
											<dd id="ddAmazonPay" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) %>" runat="server">
												<div style="margin: 10px 0;">
													<small>※配送先情報、または、お支払い方法の変更を希望される方は「アドレス帳」→「お支払い方法」の順で選択してください。</small>
												</div>
												<iframe id="AmazonInputWidget" src="<%: PageUrlCreatorUtility.CreateAmazonPayWidgetUrl(false, fixedPurchaseId: this.FixedPurchaseContainer.FixedPurchaseId) %>" style="width:100%;height:650px;border:none;"></iframe>
												<div id="constraintErrorMessage" style="color:red;padding:5px" ClientIDMode="Static" runat="server"></div>
												<asp:HiddenField ID="hfAmazonBillingAgreementId" ClientIDMode="Static" runat="server" />
												<h4>配送方法</h4>
												<div>
													<asp:DropDownList ID="ddlShippingMethodForAmazonPay" OnSelectedIndexChanged="ddlShippingMethod_OnSelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
												</div>
												
												<h4>配送サービス</h4>
												<div>
													<asp:DropDownList ID="ddlDeliveryCompanyForAmazonPay" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" AutoPostBack="true" runat="server"/>
												</div>
												
												<% if (this.DeliveryCompany.IsValidShippingTimeSetFlg && (this.WddlShippingMethodForAmazonPay.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)) { %>
												<h4>配送希望時間帯</h4>
												<div>
													<asp:DropDownList ID="ddlShippingTimeForAmazonPay" runat="server"></asp:DropDownList>
												</div>
												<% } %>
											</dd>

											<%-- Amazon Pay(CV2) --%>
											<dd id="ddAmazonPayCv2" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) %>" runat="server">
												<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>
												<div id="AmazonPayCv2Button2" style="display: inline-block"></div>
												<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>
											</dd>


											<%-- 代金引換 --%>
											<dd id="ddCollect" class="inner" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT) %>" runat="server">
											</dd>

											<%-- PayPal --%>
											<dd id="ddPayPal" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL) %>" runat="server">

												<%if (SessionManager.PayPalCooperationInfo != null) {%>
													ご利用のPayPal アカウント：<br/>
													<b><%: SessionManager.PayPalCooperationInfo.AccountEMail %></b>
												<%} else {%>
													ご利用にはPayPalログインが必要です。
												<%} %>
												<uc:PaymentDescriptionPayPal runat="server" id="PaymentDescriptionPayPal" />
											</dd>

											<%-- Paidy --%>
											<dd id="ddPaidy" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY) %>" runat="server">
												<uc:PaidyCheckoutControl ID="ucPaidyCheckoutControl" runat="server" />
											</dd>

											<%-- atone翌月払い --%>
											<dd id="ddPaymentAtone" class="Atone_0" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE) %>" runat="server">
												<span id="spanErrorMessageForAtone" class="spanErrorMessageForAtone" style="color: red; display: none" runat="server"></span>
												<uc:PaymentDescriptionAtone runat="server" id="PaymentDescriptionAtone" />
											</dd>

											<%-- aftee決済設定 --%>
											<dd id="ddPaymentAftee" class="Aftee_0" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE) %>" runat="server">
												<span id="spanErrorMessageForAftee" class="spanErrorMessageForAftee" style="color: red; display: none" runat="server"></span>
											</dd>

											<!-- NP後払い -->
											<dd id="ddNpAfterPay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY) %>" runat="server">
												<uc:PaymentDescriptionNPAfterPay runat="server" id="PaymentDescriptionNPAfterPay" />
											</dd>

											<%-- LinePay --%>
											<dd id="ddLinePay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY) %>" runat="server">
												<uc:PaymentDescriptionLinePay runat="server" id="PaymentDescriptionLinePay" />
											</dd>

											<!-- PayPay -->
											<dd id="ddPayPay" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY) %>" runat="server">
												<uc:PaymentDescriptionPayPay runat="server" id="PaymentDescriptionPayPay" />
											</dd>

											<%-- （DSK）後払い --%>
											<dd id="ddDskDef" visible="<%# (Item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF) %>" runat="server">
												コンビニ後払い（DSK）
											</dd>
										</ItemTemplate>
										<FooterTemplate>
										</dl>
										</FooterTemplate>
									</asp:Repeater>
								</dl>
							</div><!--list-->
							</div>
						</td>
					</tr><!--End input payment update form-->

					<tr><!--Action button payment update form-->
						<th></th>
						<td>
							<div id="divOrderPaymentUpdateButtons" style="display: block"> 
								<asp:HiddenField ID="hfPaymentNameSelected" runat="server" />
								<asp:HiddenField ID="hfPaymentIdSelected" runat="server" />
								<asp:HiddenField ID="hfPaymentPriceSelected" runat="server" />
								<div class="user-footer">
									<div class="button-next">
										<asp:LinkButton ID="lbUpdatePayment" Text="情報更新" runat="server" ValidationGroup="OrderPayment"
											OnClientClick="doPostbackEvenIfCardAuthFailed=false;return AlertPaymentChange();"
											OnClick="btnUpdatePaymentPatternInfo_Click" class="btn" ></asp:LinkButton>
									</div>
									<div class="button-prev">
										<asp:LinkButton id="btnClosePaymentPatternInfo" Text="キャンセル" runat="server" OnClick="btnClosePaymentPatternInfo_Click" class="btn"></asp:LinkButton>
									</div>
								</div>
							</div>
							<div id="divOrderPaymentUpdateExecFroms" style="display: none"> 
								更新中です...
							</div>
							<small id="sErrorMessagePayment" class="notProductError" runat="server"></small>
						</td>
					</tr>
				</div>

				<% if ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.UserCreditCardInfo != null)){ %>
				<tr id="dvFixedPurchaseCurrentCard" runat="server">
					<th>利用クレジットカード情報</th>
					<td>
						<% if (this.UserCreditCardInfo.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_ON) { %>
						カード登録名: <%:this.UserCreditCardInfo.CardDispName %><%: this.UserCreditCardInfo.DispFlag ? "" : " (削除済)" %><br />
						<% } %>
						<%if (OrderCommon.CreditCompanySelectable && (this.UserCreditCardInfo.CompanyName != string.Empty)) {%>
						カード会社: <%: this.UserCreditCardInfo.CompanyName %><br />
						<%} %>
						カード番号: XXXXXXXXXXXX<%: this.UserCreditCardInfo.LastFourDigit %><br />
						有効期限: <%: this.UserCreditCardInfo.ExpirationMonth + "/" + this.UserCreditCardInfo.ExpirationYear + " (月/年)" %><br />
						カード名義人: <%: this.UserCreditCardInfo.AuthorName %>
						<span style="font-weight: bold; display: block; margin-top: 2px;"><%= this.CompleteMessage %></span>
					</td>
				</tr>
				<%} %>
		</dd>
		<dt>注文メモ</dt>
		<dd>
			<%#: (string.IsNullOrEmpty(this.FixedPurchaseContainer.Memo) ? "指定なし" : this.FixedPurchaseContainer.Memo) %>
		</dd>
		<%if (IsDisplayOrderExtend()) {%>
		<dt>注文確認事項</dt>
		<dd>
			<% if (this.IsOrderExtendModify) { %>
			<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server">
				<HeaderTemplate>
					<dl>
				</HeaderTemplate>
				<ItemTemplate>
					<%-- 項目名 --%>
					<dt><%#: Item.SettingModel.SettingName %><span class="require" runat="server" visible="<%# Item.SettingModel.IsNeecessary%>">※</span></dt>
					<%-- 概要 --%>
					<%# Item.SettingModel.OutlineHtmlEncode %>
					<dd>
						<%-- TEXT --%>
						<div runat="server" visible="<%# Item.SettingModel.CanUseModify && Item.SettingModel.IsInputTypeText%>">
							<asp:TextBox runat="server" ID="tbSelect" Width="250px"  MaxLength="100"></asp:TextBox>
						</div>
						<%-- DDL --%>
						<div runat="server" visible="<%# Item.SettingModel.CanUseModify && Item.SettingModel.IsInputTypeDropDown %>">
							<asp:DropDownList runat="server" ID="ddlSelect"></asp:DropDownList>
						</div>
						<%-- RADIO --%>
						<div runat="server" visible="<%# Item.SettingModel.CanUseModify && Item.SettingModel.IsInputTypeRadio %>">
							<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="radioBtn"></asp:RadioButtonList>
						</div>
						<%-- CHECK --%>
						<div runat="server" visible="<%# Item.SettingModel.CanUseModify && Item.SettingModel.IsInputTypeCheckBox %>">
							<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" CssClass="checkBox"></asp:CheckBoxList>
						</div>
						<div runat="server" visible="<%# (Item.SettingModel.CanUseModify == false) %>">
							<%#: Item.InputText %>
						</div>
						<%-- 検証文言 --%>
						<small><asp:Label runat="server" ID="lbErrMessage" CssClass="attention"></asp:Label></small>
						<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
						<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
					</dd>
				</ItemTemplate>
				<FooterTemplate>
					</dl>
				</FooterTemplate>
			</asp:Repeater>
			<% } else { %>
			<asp:Repeater ID="rOrderExtendDisplay" ItemType="OrderExtendItemInput" runat="server">
				<HeaderTemplate>
					<dl>
				</HeaderTemplate>
				<ItemTemplate>
						<dt><%#: Item.SettingModel.SettingName %>:</dt>
						<dd><%#: Item.InputText %></dd>
				</ItemTemplate>
				<FooterTemplate>
					</dl>
				</FooterTemplate>
			</asp:Repeater>
			<% } %>
			<div class="button">
				<asp:LinkButton ID="lbOrderExtend" Text="注文確認事項の変更" Visible="<%# ((this.CanCancelFixedPurchase) && (this.BeforeCancelDeadline) && (IsDisplayOrderExtendModifyButton()) && (this.FixedPurchaseContainer.IsCompleteStatus == false) && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false)) %>" OnClick="lbOrderExtend_OnClick"  runat="server" class="btn" />
			</div>
		</dd>
		<% if (this.IsOrderExtendModify) { %>
		<dd>
			<div id="divOrderExtendUpdateButtons" class="button" style="display: block"> 
				<asp:LinkButton ID="lbUpdateOrderExtend" Text="情報更新" runat="server" OnClientClick="return AlertDataChange('OrderPointUse', null);" OnClick="lbUpdateOrderExtend_OnClick" class="btn" /><br />
				<asp:LinkButton ID="lbHideOrderExtend" Text="キャンセル" runat="server" OnClientClick="return exec_submit();" OnClick="lbHideOrderExtend_OnClick" class="btn" />
			</div>
			<div id="divOrderExtendUpdateExecFroms" style="display: none"> 更新中です...</div>
		</dd>
		<% } %>
		<% } %>

		<% if ((this.FixedPurchaseShippingContainer.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
			|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) { %>
		<dt>次回配送日</dt>
		<dd>
			<span id="spNextShippingDate" runat="server">
			<%#: (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false) ? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.NextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-" %>
			</span>
			<br />
			<% if (this.DisplaySkipNextShipping) { %>
			<% if (this.HasClickSkipNextShipping) { %>
			<span runat="server" Visible="<%# this.CanCancelFixedPurchase && this.IsCancelable %>">次回配送日の <%# this.ShopShipping.FixedPurchaseCancelDeadline %>日前 までスキップまたは変更可能です。</span>
			<span runat="server" Visible="<%# this.CanCancelFixedPurchase && this.IsCancelable == false %>">出荷回数が<%# this.FixedPurchaseCancelableCount %>回以上からスキップ可能です。</span>
			<% } else { %>
			<span>定期購入スキップ制限回数を超えました。</span>
			<% } %>
			<% } %>
			<div class="button" Visible="<%# this.CanCancelFixedPurchase && this.BeforeCancelDeadline && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false) %>" runat="server">
				<% if (this.DisplaySkipNextShipping) { %>
				<asp:LinkButton ID="btnCancelNextShipping" Visible="<%# this.CanCancelFixedPurchase && this.BeforeCancelDeadline && this.IsCancelable && this.HasClickSkipNextShipping %>" Text="次回配送スキップ" CssClass="btn"
					runat="server" OnClientClick="return confirm('次回配送をスキップします。よろしいですか？')" OnClick="btnSkipNextShipping_Click" />
				<% } %>
				<asp:LinkButton ID="btnChangeNextShippingDate" style="margin-top:5px" runat="server" OnClick="btnChangeNextShippingDate_Click" Text="次回配送日変更" CssClass="btn" />
			</div>
			<div id="dvChangeNextShippingDate" visible="false" runat="server">
				<div>
					<asp:DropDownList ID="ddlNextShippingDate" runat="server" DataSource="<%# GetChangeNextShippingDateList() %>"
						DataTextField="text" DataValueField="value" SelectedValue ='<%# this.FixedPurchaseContainer.NextShippingDate.Value.ToString("yyyy/MM/dd") %>'
						OnSelectedIndexChanged="ddlNextShippingDate_SelectedIndexChanged" AutoPostBack="true" />
					<br />
					<asp:CheckBox id="cbUpdateNextNextShippingDate" runat="server" Checked="<%# Constants.FIXEDPURCHASEORDERNOW_NEXT_NEXT_SHIPPING_DATE_UPDATE_DEFAULT %>" Text="次々回配送日を自動計算する"></asp:CheckBox><br />
					<asp:Label runat="server" ID="lNextShippingDateErrorMessage" class="attention" />
				</div>
				<br />
				<div class="user-footer">
				<div  class="button-next">
					<asp:LinkButton ID="btnUpdateNextShippingDate" Text="次回配送日更新" runat="server"
						OnClientClick="return confirmUpdateNextNextShippingDate();" OnClick="btnUpdateNextShippingDate_Click" CssClass="btn" />
				</div>
				<div class="button-prev">
					<asp:LinkButton ID="btnCancelNextShippingDate" Text="キャンセル" runat="server" OnClick="btnCancelNextShippingDate_Click" CssClass="btn" />
				</div>
				</div>
			</div>
		</dd>
		<% } %>
		<dt>次々回配送日</dt>
		<dd>
			<span id="spNextNextShippingDate" runat="server">
			<%#: (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false) ? DateTimeUtility.ToStringFromRegion(this.FixedPurchaseContainer.NextNextShippingDate, DateTimeUtility.FormatType.ShortDateWeekOfDay1Letter) : "-" %>
			</span>
			<span runat="server" Visible="<%# (this.WcbUpdateNextNextShippingDate.Checked) && (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false) %>">※次回配送日の変更により左記のように変更されます。</span>
		</dd>
		<dt runat="server" Visible="<%# (this.FixedPurchaseContainer.IsOrderRegister) && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false)%>">今すぐ注文</dt>
		<dd runat="server" Visible="<%# (this.FixedPurchaseContainer.IsOrderRegister) && (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false)%>">
			<div>
				<asp:LinkButton ID="btnFixedPurchaseOrder" Text="  今すぐ注文  " runat="server" OnClientClick = "return confirm('今すぐ注文を登録します。よろしいですか？')" OnClick="btnFixedPurchaseOrder_Click" CssClass="btn" />
				<asp:CheckBox ID="cbUpdateNextShippingDate" Text=" 次回配送日を更新する " runat="server" Checked="<%# Constants.FIXEDPURCHASEORDERNOW_NEXT_SHIPPING_DATE_UPDATE_DEFAULT %>" />
			</div>
			<small>
				※設定された次回購入の利用ポイント数が本注文に適用されます。<br />
				　適用後、次回購入の利用ポイント数が「0」にリセットされます。<br />
				<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
				※設定された次回購入の利用クーポンが本注文に適用されます。<br />
				　適用後、次回購入の利用クーポンが「指定なし」にリセットされます。<br />
				<% } %>
				※今すぐ注文の配送日は「<%# ReplaceTag("@@DispText.shipping_date_list.none@@") %>」で登録されます。 <br />
				※「次回配送日を更新する」にチェックを入れた場合には、 <br />
				　定期購入情報の次回・次々回配送日が変更されます。 <br />
				※「次回配送日を更新する」にチェックをしない場合には、 <br />
				　定期購入情報の次回・次々回配送日は変更されません。 <br />
			</small>
		</dd>
	</dl>

	<%--▽領収書情報▽--%>
	<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
		<h3>領収書情報</h3>
		<% if (this.IsReceiptInfoModify == false) { %>
		<dl class="user-form">
			<dt>領収書希望</dt>
			<dd><%: ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG, this.FixedPurchaseContainer.ReceiptFlg) %></dd>
			<div runat="server" visible="<%# this.FixedPurchaseContainer.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
				<dt>宛名</dt>
				<dd><%: this.FixedPurchaseContainer.ReceiptAddress %></dd>
			</div>
			<div runat="server" visible="<%# this.FixedPurchaseContainer.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON %>">
				<dt>但し書き</dt>
				<dd><%: this.FixedPurchaseContainer.ReceiptProviso %></dd>
			</div>
		</dl>
		<% if (lbDisplayReceiptInfoForm.Visible) { %>
		<div class="button">
			<asp:LinkButton ID="lbDisplayReceiptInfoForm" Text="領収書情報変更" runat="server" Visible="<%# ((this.CanModifyReceiptInfo) && (this.FixedPurchaseContainer.IsCompleteStatus == false)) %>" OnClick="lbDisplayReceiptInfoForm_Click" CssClass="btn" />
		</div>
		<% } %>
		<% } else { %>
		<dl class="user-form">
			<dt>領収書希望</dt>
			<dd>
				<asp:DropDownList ID="ddlReceiptFlg" runat="server" DataTextField="Text" DataValueField="Value"
					OnSelectedIndexChanged="ddlReceiptFlg_OnSelectedIndexChanged" AutoPostBack="true" DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG) %>" />
			</dd>
			<div id="dvReceiptAddressProvisoInput" runat="server">
			<dt>宛名<span class="require">※</span></dt>
			<dd>
				<asp:TextBox ID="tbReceiptAddress" runat="server" Width="300" MaxLength="100" />
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbReceiptAddress"
					ValidationGroup="ReceiptRegisterModify"
					ClientValidationFunction="ClientValidate"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false"
					CssClass="error_inline" />
			</dd>
			<dt>但し書き<span class="require">※</span></dt>
			<dd>
				<asp:TextBox ID="tbReceiptProviso" runat="server" Width="300" MaxLength="100" />
				<asp:CustomValidator runat="Server"
					ControlToValidate="tbReceiptProviso"
					ValidationGroup="ReceiptRegisterModify"
					ClientValidationFunction="ClientValidate"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					EnableClientScript="false"
					CssClass="error_inline" />
			</dd>
			</div>
		</dl>
		<div class="user-footer">
			<div class="button-next">
				<asp:LinkButton Text="領収書情報更新" runat="server" OnClientClick="return confirm('領収書情報を変更してもよろしいですか？')" OnClick="btnUpdateReceiptInfo_Click" CssClass="btn" />
			</div>
			<div class="button-prev">
				<asp:LinkButton Text="キャンセル" runat="server" OnClick="lbDisplayReceiptInfoForm_Click" CssClass="btn" />
			</div>
			<p class="msg">※注文済みの領収書情報は変更されませんのでご注意ください。</p>
			<p class="msg" style="color: red;"><%: this.ReceiptInfoModifyErrorMessage %></p>
		</div>
		<% } %>
	<% } %>
	<%--△領収書情報△--%>

		<h3>お届け先情報</h3>
		<% if (this.IsUserShippingModify == false) { %>
		<% if (this.UseShippingAddress) { %>
		<dl class="user-form">
			<dt>
				<%: ReplaceTag("@@User.addr.name@@") %>
			</dt>
			<dd>
				<%= IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode)
					? "〒" + WebSanitizer.HtmlEncode(this.FixedPurchaseShippingContainer.ShippingZip) + "<br />"
					: "" %>
				<%: this.FixedPurchaseShippingContainer.ShippingAddr1%>
				<%: this.FixedPurchaseShippingContainer.ShippingAddr2%><br />
				<%: this.FixedPurchaseShippingContainer.ShippingAddr3%>
				<%: this.FixedPurchaseShippingContainer.ShippingAddr4%>
				<%: this.FixedPurchaseShippingContainer.ShippingAddr5%><br />
				<%= (IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode) == false)
					? WebSanitizer.HtmlEncode(this.FixedPurchaseShippingContainer.ShippingZip) + "<br />"
					: "" %>
				<%: this.FixedPurchaseShippingContainer.ShippingCountryName%>
			</dd>
			<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
			<dt><%: ReplaceTag("@@User.company_name.name@@") %>/<%: ReplaceTag("@@User.company_post_name.name@@") %></dt>
			<dd>
				<%: this.FixedPurchaseShippingContainer.ShippingCompanyName%>/<%: this.FixedPurchaseShippingContainer.ShippingCompanyPostName%>
			</dd>
			<%} %>
			<dt><%: ReplaceTag("@@User.name.name@@") %></dt>
			<dd>
				<%: this.FixedPurchaseShippingContainer.ShippingName1%><%: this.FixedPurchaseShippingContainer.ShippingName2%>&nbsp;様<br />
				<% if (IsCountryJp(this.FixedPurchaseShippingContainer.ShippingCountryIsoCode)) { %>
				（<%: this.FixedPurchaseShippingContainer.ShippingNameKana1%><%: this.FixedPurchaseShippingContainer.ShippingNameKana2%>&nbsp;さま）
				<% } %>
			</dd>
			<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
			<dd>
				<%: this.FixedPurchaseShippingContainer.ShippingTel1%>
			</dd>
			</dl>
			<% } %>
			<% if (this.UseShippingReceivingStore) { %>
				<dl class="user-form">
					<dt><%: ReplaceTag("@@DispText.shipping_convenience_store.shopId@@") %></dt>
					<dd><%: this.FixedPurchaseShippingContainer.ShippingReceivingStoreId %></dd>
					<dt><%: ReplaceTag("@@DispText.shipping_convenience_store.shopName@@") %></dt>
					<dd><%: this.FixedPurchaseShippingContainer.ShippingName %></dd>
					<dt><%: ReplaceTag("@@DispText.shipping_convenience_store.shopAddress@@") %></dt>
					<dd><%: this.FixedPurchaseShippingContainer.ShippingAddr4 %></dd>
					<dt><%: ReplaceTag("@@DispText.shipping_convenience_store.shopTel@@") %></dt>
					<dd><%: this.FixedPurchaseShippingContainer.ShippingTel1 %></dd>
				</dl>
			<% } %>
			<dl class="user-form">
			<dt>配送方法</dt>
			<dd>
				<%: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, this.FixedPurchaseShippingContainer.ShippingMethod) %>
			</dd>
			<dt>配送サービス</dt>
			<dd>
				<%: this.DeliveryCompany.DeliveryCompanyName %>
			</dd>
			<% if (this.DeliveryCompany.IsValidShippingTimeSetFlg && (this.FixedPurchaseShippingContainer.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS) && this.UseShippingAddress)
			{ %>
					<dt>配送希望時間帯</dt>
				<% if (this.IsShippingTimeModifyAmazonPayCv2 == false) { %>
					<dd>
						<%#: this.HasShippingTime %>
						<% if (this.Payment.IsPaymentIdAmazonPayCv2) { %>
							<div class="button">
								<asp:LinkButton Text="配送希望時間帯変更" runat="server" OnClientClick="return exec_submit();" OnClick="lbModifyShippingTimeAmazonPayCv2_Click" class="btn" />
							</div>
						<% } %>
					</dd>
				<% } else { %>
					<dd>
						<asp:DropDownList ID="ddlShippingTimeAmazonPayCv2" runat="server"></asp:DropDownList>
					</dd>
					<dd>
						<div class="button">
							<asp:LinkButton Text="配送希望時間帯更新" runat="server" OnClientClick="return confirm('配送希望時間帯を変更してもよろしいですか？')" OnClick="lbUpdateShippingTimeAmazonPayCv2_Click" class="btn" />
						</div>
						<div class="button">
							<asp:LinkButton Text="キャンセル" runat="server" OnClick="lbModifyShippingTimeAmazonPayCv2_Click" class="btn" />
						</div>
					</dd>
				<% } %>
			<% } %>
		</dl>
		<% if (lbDisplayUserShippingInfoForm.Visible) { %>
			<div class="button">
				<% if (this.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) { %>
				<asp:LinkButton ID="lbDisplayUserShippingInfoForm" Visible="<%# ((this.CanCancelFixedPurchase) && (this.BeforeCancelDeadline) && (this.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) && (this.FixedPurchaseContainer.IsCompleteStatus == false)) %>" Text="お届け先情報変更" CssClass="btn" runat="server" OnClick="lbDisplayUserShippingInfoForm_Click" />
				<% } else if(this.CanCancelFixedPurchase && this.BeforeCancelDeadline) { %>
				<%--▼▼ Amazon Pay(CV2)ボタン ▼▼--%>
				<div id="AmazonPayCv2Button" style="display: inline-block"></div>
				<%--▲▲ Amazon Pay(CV2)ボタン ▲▲--%>
				<% } %>
			</div>
		<% } %>
		<% } else { %>
		<asp:HiddenField ID="hfCvsShopFlg" runat="server" Value='<%# this.FixedPurchaseShippingContainer.ShippingReceivingStoreFlg %>' />
		<asp:HiddenField ID="hfSelectedShopId" runat="server" Value='<%# this.FixedPurchaseShippingContainer.ShippingReceivingStoreId %>' />
		<div class="user-footer">
			<br />
			<div>
				<asp:DropDownList
					DataTextField="Text"
					DataValueField="Value"
					SelectedValue='<%# IsDisplayButtonConvenienceStore(this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg)
						? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE
						: null %>'
					ID="ddlShippingType"
					AutoPostBack="true"
					OnSelectedIndexChanged="ddlShippingType_SelectedIndexChanged"
					DataSource='<%# GetPossibleShippingType(this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg) %>'
					runat="server"
					Width="100%"
					CssClass="UserShippingAddress">
				</asp:DropDownList>
				<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
				<asp:DropDownList
					ID="ddlShippingReceivingStoreType"
					DataTextField="Text"
					DataValueField="Value"
					Visible='<%# IsDisplayButtonConvenienceStore(this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg) %>'
					AutoPostBack="true"
					OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged"
					DataSource='<%# ShippingReceivingStoreType() %>'
					runat="server"
					CssClass="UserShippingAddress">
				</asp:DropDownList>
				<% } %>
			</div>
			<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
			<div id="spConvenienceStoreSelect" visible="<%# (this.UseShippingReceivingStore && (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) %>" runat="server">
				<a href="javascript:openConvenienceStoreMapPopup();" class="btn btn-success convenience-store-button">Family/OK/Hi-Life</a>
			</div>
			<div id="spConvenienceStoreEcPaySelect" runat="server" visible='<%# ((this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) && Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) %>'>
				<asp:LinkButton
					ID="lbOpenEcPay"
					runat="server"
					class="btn btn-success convenience-store-button"
					OnClick="lbOpenEcPay_Click"
					Text="  電子マップ  " />
			</div>
			<div id="dvErrorShippingConvenience" style="display:none;" runat="server">
				<span class="require"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_GROCERY_STORE) %></span>
			</div>
			<div id="dvErrorPaymentAndShippingConvenience" runat="server" visible="false">
				<span class="error_inline" style="color:red;"><%#: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_METHOD_CHANGED_TO_CONVENIENCE_STORE) %></span>
			</div>
			<% } %>
		</div>
		<dl visible="<%# this.UseShippingAddress %>" id="divShippingInputFormInner" runat="server" class="user-form">
			<dt>
				<%-- 氏名 --%>
				<%: ReplaceTag("@@User.name.name@@") %><span class="require">※</span>
			</dt>
			<dd class="name">
				<p class="attention">
					<asp:CustomValidator
						ID="cvShippingName1"
						runat="Server"
						ControlToValidate="tbShippingName1"
						ValidationGroup="FixedPurchaseModifyInput"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
					<asp:CustomValidator
						ID="cvShippingName2"
						runat="Server"
						ControlToValidate="tbShippingName2"
						ValidationGroup="FixedPurchaseModifyInput"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
				</p>
				<% tbShippingName1.Attributes["placeholder"] = ReplaceTag("@@User.name1.name@@"); %>
				<% tbShippingName2.Attributes["placeholder"] = ReplaceTag("@@User.name2.name@@"); %>
				<w2c:ExtendedTextBox ID="tbShippingName1" MaxLength='<%# GetMaxLength("@@User.name1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox ID="tbShippingName2" MaxLength='<%# GetMaxLength("@@User.name2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			</dd>
			<% if (this.IsShippingAddrJp) { %>
			<dt>
				<%-- 氏名（かな） --%>
				<%: ReplaceTag("@@User.name_kana.name@@") %><span class="require">※</span>
			</dt>
			<dd class="name-kana">
				<p class="attention">
					<asp:CustomValidator
						ID="cvShippingNameKana1"
						runat="Server"
						ControlToValidate="tbShippingNameKana1"
						ValidateEmptyText="true"
						SetFocusOnError="true"
						ValidationGroup="FixedPurchaseModifyInput" />
					<asp:CustomValidator
						ID="cvShippingNameKana2"
						runat="Server"
						ControlToValidate="tbShippingNameKana2"
						ValidationGroup="FixedPurchaseModifyInput"
						ValidateEmptyText="true"
						SetFocusOnError="true" />
				</p>
				<% tbShippingNameKana1.Attributes["placeholder"] = ReplaceTag("@@User.name1.name@@"); %>
				<% tbShippingNameKana2.Attributes["placeholder"] = ReplaceTag("@@User.name2.name@@"); %>
				<w2c:ExtendedTextBox ID="tbShippingNameKana1" MaxLength='<%# GetMaxLength("@@User.name_kana1.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
				<w2c:ExtendedTextBox ID="tbShippingNameKana2" MaxLength='<%# GetMaxLength("@@User.name_kana2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			</dd>
			<% } %>
			<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
			<dt>
				<%: ReplaceTag("@@User.country.name@@", this.ShippingAddrCountryIsoCode) %>
				<span class="require">※</span>
			</dt>
			<dd>
				<asp:DropDownList runat="server" id="ddlShippingCountry" AutoPostBack="true" OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged"></asp:DropDownList>
				<asp:CustomValidator
					ID="cvShippingCountry"
					runat="Server"
					ControlToValidate="ddlShippingCountry"
					ValidationGroup="FixedPurchaseModifyInput"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					ClientValidationFunction="ClientValidate"
					EnableClientScript="false"
					CssClass="error_inline" />
			</dd>
			<% } %>
			<% if (this.IsShippingAddrJp) { %>
			<dt>
				<%-- 郵便番号 --%>
				<%: ReplaceTag("@@User.zip.name@@") %>
				<span class="require">※</span>
			</dt>
			<dd class="zip">
				<p class="attention" id="addrSearchErrorMessage">
				<asp:CustomValidator
					ID="cvShippingZip1"
					runat="Server"
					ControlToValidate="tbShippingZip"
					ValidationGroup="FixedPurchaseModifyInput"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
					<%: this.ZipInputErrorMessage %>
				</p>
				<w2c:ExtendedTextBox ID="tbShippingZip" OnTextChanged="lbSearchShippingAddr_Click" Type="tel" MaxLength="8" runat="server" />
				<asp:LinkButton ID="lbSearchShippingAddr" runat="server" OnClick="lbSearchShippingAddr_Click" CssClass="btn-add-search" OnClientClick="return false;">住所検索</asp:LinkButton>
				<%--検索結果レイヤー--%>
				<uc:Layer ID="ucLayer" runat="server" />
			</dd>
			<% } %>
			<dt>
				<%: ReplaceTag("@@User.addr.name@@", this.ShippingAddrCountryIsoCode) %>
				<span class="require">※</span>
			</dt>
			<dd class="address">
				<p class="attention">
				<asp:CustomValidator
					ID="cvShippingAddr1"
					runat="Server"
					ControlToValidate="ddlShippingAddr1"
					ValidationGroup="FixedPurchaseModifyInput"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% if (this.IsShippingAddrTw == false) { %>
				<asp:CustomValidator
					ID="cvShippingAddr2"
					runat="Server"
					ControlToValidate="tbShippingAddr2"
					ValidationGroup="FixedPurchaseModifyInput"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<asp:CustomValidator
					ID="cvShippingAddr3"
					runat="Server"
					ControlToValidate="tbShippingAddr3"
					ValidationGroup="FixedPurchaseModifyInput"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				<% } %>
				<asp:CustomValidator
					ID="cvShippingAddr4"
					runat="Server"
					ControlToValidate="tbShippingAddr4"
					ValidationGroup="FixedPurchaseModifyInput"
					ValidateEmptyText="true"
					SetFocusOnError="true"
					CssClass="error_inline" />
				</p>
				<ul>
				<% if (this.IsShippingAddrJp) { %>
				<li><asp:DropDownList ID="ddlShippingAddr1" runat="server"></asp:DropDownList></li>
				<% } %>
				<li>
					<%-- 市区町村 --%>
					<% if (this.IsShippingAddrTw) { %>
						<asp:DropDownList runat="server" ID="ddlShippingAddr2" DataSource="<%# this.UserTwCityList %>" AutoPostBack="true" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlShippingAddr2_SelectedIndexChanged"></asp:DropDownList>
					<% } else { %>
						<w2c:ExtendedTextBox ID="tbShippingAddr2" placeholder='<%# ReplaceTag("@@User.addr2.name@@", this.ShippingAddrCountryIsoCode) %>' MaxLength='<%# GetMaxLength("@@User.addr2.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					<% } %>
				</li>
				<li>
					<%-- 番地 --%>
					<% if (this.IsShippingAddrTw) { %>
						<asp:DropDownList runat="server" ID="ddlShippingAddr3" DataTextField="Key" DataValueField="Value" Width="95" ></asp:DropDownList>
					<% } else { %>
						<w2c:ExtendedTextBox ID="tbShippingAddr3" placeholder='<%# ReplaceTag("@@User.addr3.name@@", this.ShippingAddrCountryIsoCode) %>' MaxLength='<%# GetMaxLength("@@User.addr3.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					<% } %>
				</li>
				<li <%= (Constants.DISPLAY_ADDR4_ENABLED || (this.IsShippingAddrJp == false)) ? "" : "style=\"display:none;\""  %>>
					<%-- ビル・マンション名 --%>
					<w2c:ExtendedTextBox ID="tbShippingAddr4" placeholder='<%# ReplaceTag("@@User.addr4.name@@", this.ShippingAddrCountryIsoCode) %>' MaxLength='<%# GetMaxLength("@@User.addr4.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
				</li>
				<% if (this.IsShippingAddrJp == false) { %>
				<li>
					<%-- 州 --%>
					<% if (this.IsShippingAddrUs) { %>
					<asp:DropDownList runat="server" id="ddlShippingAddr5"></asp:DropDownList>
						<asp:CustomValidator
							ID="cvShippingAddr5Ddl"
							runat="Server"
							ControlToValidate="ddlShippingAddr5"
							ValidationGroup="FixedPurchaseModifyInputGlobal"
							ValidateEmptyText="true"
							SetFocusOnError="true"
							ClientValidationFunction="ClientValidate"
							EnableClientScript="false"
							CssClass="error_inline" />
					<% } else { %>
					<w2c:ExtendedTextBox ID="tbShippingAddr5" placeholder='<%# ReplaceTag("@@User.addr5.name@@", this.ShippingAddrCountryIsoCode) %>' MaxLength='<%# GetMaxLength("@@User.addr5.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
					<% } %>
				</li>
				<% } %>
				</ul>
			</dd>
			<%-- 郵便番号（海外向け） --%>
			<% if (this.IsShippingAddrJp == false) { %>
			<dt>
				<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %>
				<% if (this.IsShippingAddrZipNecessary) { %><span class="require">※</span><% } %>
			</dt>
			<dd class="zip">
				<p class="attention">
				<asp:CustomValidator
					ID="cvShippingZipGlobal"
					runat="Server"
					ControlToValidate="tbShippingZipGlobal"
					ValidationGroup="FixedPurchaseModifyInputGlobal"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbShippingZipGlobal" Type="tel" MaxLength="20" runat="server"></w2c:ExtendedTextBox>
				<asp:LinkButton
					ID="lbSearchAddrFromShippingZipGlobal"
					OnClick="lbSearchAddrFromShippingZipGlobal_Click"
					Style="display:none;"
					runat="server" />
			</dd>
			<% } %>
			<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
			<dt>
				<%-- 企業名 --%>
				<%: ReplaceTag("@@User.company_name.name@@")%><span class="require"></span>
			</dt>
			<dd class="company-name">
				<p class="attention">
				<asp:CustomValidator
					ID="cvShippingCompanyName"
					runat="Server"
					ControlToValidate="tbShippingCompanyName"
					ValidationGroup="FixedPurchaseModifyInput"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbShippingCompanyName" placeholder='<%# ReplaceTag("@@User.company_name.name@@") %>' MaxLength='<%# GetMaxLength("@@User.company_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			</dd>
			<dt>
				<%-- 部署名 --%>
				<%: ReplaceTag("@@User.company_post_name.name@@")%><span class="require"></span>
			</dt>
			<dd class="company-post">
				<p class="attention">
				<asp:CustomValidator
					ID="cvShippingCompanyPostName"
					runat="Server"
					ControlToValidate="tbShippingCompanyPostName"
					ValidationGroup="FixedPurchaseModifyInput"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbShippingCompanyPostName" placeholder='<%# ReplaceTag("@@User.company_post_name.name@@") %>' MaxLength='<%# GetMaxLength("@@User.company_post_name.length_max@@") %>' runat="server"></w2c:ExtendedTextBox>
			</dd>
			<%} %>
			<% if (this.IsShippingAddrJp) { %>
			<dt>
				<%-- 電話番号 --%>
				<%: ReplaceTag("@@User.tel1.name@@") %><span class="require">※</span>
			</dt>
			<dd class="tel">
				<p class="attention">
				<asp:CustomValidator
					ID="cvShippingTel1"
					runat="Server"
					ControlToValidate="tbShippingTel"
					ValidationGroup="FixedPurchaseModifyInput"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbShippingTel" Type="tel" CssClass="tel shortTel" style="width:100%;" MaxLength="13" runat="server" />
			</dd>
			<% } else { %>
			<dt>
				<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
				<span class="require">※</span>
			</dt>
			<dd class="tel">
				<p class="attention">
				<asp:CustomValidator
					ID="cvShippingTel1Global"
					runat="Server"
					ControlToValidate="tbShippingTel1Global"
					ValidationGroup="FixedPurchaseModifyInputGlobal"
					ValidateEmptyText="true"
					SetFocusOnError="true" />
				</p>
				<w2c:ExtendedTextBox ID="tbShippingTel1Global" Type="tel" CssClass="tel" MaxLength="30" runat="server"></w2c:ExtendedTextBox>
			</dd>
			<% } %>
			</dl>
			<dl runat="server" id="divShippingInputFormConvenience" visible="<%# this.UseShippingReceivingStore %>" class="user-form shipping_convenience">
			<dt><%: ReplaceTag("@@DispText.shipping_convenience_store.shopId@@") %></dt>
			<dd id="ddCvsShopId">
				<span style="font-weight:normal;">
					<asp:Literal ID="lCvsShopId" runat="server" Text="<%# this.FixedPurchaseShippingContainer.ShippingReceivingStoreId %>"></asp:Literal>
				</span>
				<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# this.FixedPurchaseShippingContainer.ShippingReceivingStoreId %>" />
			</dd>
			<dt><%: ReplaceTag("@@DispText.shipping_convenience_store.shopName@@") %></dt>
			<dd id="ddCvsShopName">
				<span style="font-weight:normal;">
					<asp:Literal ID="lCvsShopName" runat="server" Text="<%# this.FixedPurchaseShippingContainer.ShippingName %>"></asp:Literal>
				</span>
				<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# this.FixedPurchaseShippingContainer.ShippingName %>" />
			</dd>
			<dt><%: ReplaceTag("@@DispText.shipping_convenience_store.shopAddress@@") %></dt>
			<dd id="ddCvsShopAddress">
				<span style="font-weight:normal;">
					<asp:Literal ID="lCvsShopAddress" runat="server" Text="<%# this.FixedPurchaseShippingContainer.ShippingAddr4 %>"></asp:Literal>
				</span>
				<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# this.FixedPurchaseShippingContainer.ShippingAddr4 %>" />
			</dd>
			<dt><%: ReplaceTag("@@DispText.shipping_convenience_store.shopTel@@") %></dt>
			<dd id="ddCvsShopTel">
				<span style="font-weight:normal;">
					<asp:Literal ID="lCvsShopTel" runat="server" Text="<%# this.FixedPurchaseShippingContainer.ShippingTel1 %>"></asp:Literal>
				</span>
				<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# this.FixedPurchaseShippingContainer.ShippingTel1 %>" />
			</dd>
			
		</dl>
		<dl class="user-form" runat="server" visible="false" id="tbOwnerAddress">
			<dt>住所</dt>
			<dd>
				<% if (IsCountryJp(this.LoginUser.AddrCountryIsoCode)) { %>
					〒<%: this.LoginUser.Zip %><br />
				<% } %>
				<%: this.LoginUser.Addr1 %>
				<%: this.LoginUser.Addr2 %>
				<%: this.LoginUser.Addr3 %>
				<%: this.LoginUser.Addr4 %><br />
				<%: this.LoginUser.Addr5 %>
				<% if (IsCountryJp(this.LoginUser.AddrCountryIsoCode) == false) { %>
					<%: this.LoginUser.Zip %><br />
				<% } %>
			</dd>
			<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
				<%-- 企業名・部署名 --%>
				<dt><%: ReplaceTag("@@User.company_name.name@@") %>・
					<%: ReplaceTag("@@User.company_post_name.name@@") %></dt>
				<dd>
					<%: this.LoginUser.CompanyName %><br />
					<%: this.LoginUser.CompanyPostName %>
				</dd>
			<% } %>
			<%-- 氏名 --%>
			<dt><%: ReplaceTag("@@User.name.name@@", this.LoginUser.AddrCountryIsoCode) %></dt>
			<dd>
				<%: this.LoginUser.Name1%><%: this.LoginUser.Name2%>&nbsp;様
				<% if (IsCountryJp(this.LoginUser.AddrCountryIsoCode)) { %>
					（<%: this.LoginUser.NameKana1%><%: this.LoginUser.NameKana2%>&nbsp;さま）
				<% } %>
			</dd>
			<%-- 電話番号 --%>
			<dt><%: ReplaceTag("@@User.tel1.name@@", this.LoginUser.AddrCountryIsoCode) %></dt>
			<dd>
				<%: this.LoginUser.Tel1 %>
			</dd>
		</dl>
		<asp:Repeater Visible="false" runat="server" ItemType="UserShippingModel" DataSource="<%# this.UserShippingAddr %>" ID="rOrderShippingList">
			<ItemTemplate>
				<dl class='<%# string.Format("user-form user_addres{0}", Item.ShippingNo) %>' runat="server" visible="<%# (Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
					<asp:HiddenField runat="server" ID="hfShippingNo" Value="<%# Item.ShippingNo %>" />
					<dt>店舗ID</dt>
					<dd id="ddCvsShopId">
						<span style="font-weight:normal;">
							<asp:Literal ID="lCvsShopId" runat="server" Text="<%# Item.ShippingReceivingStoreId %>"></asp:Literal>
						</span>
						<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# Item.ShippingReceivingStoreId %>" />
					</dd>
					<dt>店舗名</dt>
					<dd id="ddCvsShopName">
						<span style="font-weight:normal;">
							<asp:Literal ID="lCvsShopName" runat="server" Text="<%# Item.ShippingName %>"></asp:Literal>
						</span>
						<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# Item.ShippingName %>" />
					</dd>
					<dt>店舗住所</dt>
					<dd id="ddCvsShopAddress">
						<span style="font-weight:normal;">
							<asp:Literal ID="lCvsShopAddress" runat="server" Text="<%# Item.ShippingAddr4 %>"></asp:Literal>
						</span>
						<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# Item.ShippingAddr4 %>" />
					</dd>
					<dt>店舗電話番号</dt>
					<dd id="ddCvsShopTel">
						<span style="font-weight:normal;">
							<asp:Literal ID="lCvsShopTel" runat="server" Text="<%# Item.ShippingTel1 %>"></asp:Literal>
						</span>
						<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# Item.ShippingTel1 %>" />
					</dd>
				</dl>
				<dl class="user-form" runat="server" visible="<%# (Item.ShippingReceivingStoreFlg != Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) %>">
					<dt><%: ReplaceTag("@@User.addr.name@@") %></dt>
					<dd>
						<%# IsCountryJp(Item.ShippingCountryIsoCode)
							? "〒" + WebSanitizer.HtmlEncode(Item.ShippingZip) + "<br />"
							: "" %>
						<%#: Item.ShippingAddr1 %>
						<%#: Item.ShippingAddr2 %>
						<%#: Item.ShippingAddr3 %>
						<%#: Item.ShippingAddr4 %>
						<%#: Item.ShippingAddr5 %>
						<%# (IsCountryJp(Item.ShippingCountryIsoCode) == false)
								? WebSanitizer.HtmlEncode(Item.ShippingZip) + "<br />"
								: "" %>
						<%#: Item.ShippingCountryName %>
					</dd>
					<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
						<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
							<%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
						<dd><%#: Item.ShippingCompanyName %><br />
							<%#: Item.ShippingCompanyPostName %></dd>
					<%} %>
					<%-- 氏名 --%>
					<dt><%: ReplaceTag("@@User.name.name@@") %></dt>
					<dd>
						<%#: Item.ShippingName %>&nbsp;様<br />
						<%#: IsCountryJp(Item.ShippingCountryIsoCode)
							? "(" + Item.ShippingNameKana + " さま)" 
							: "" %>
					</dd>
					<%-- 電話番号 --%>
					<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
					<dd>
						<%#: Item.ShippingTel1 %>
					</dd>
				</dl>
			</ItemTemplate>
		</asp:Repeater>
		<dl class="user-form">
			<dt>配送方法</dt>
				<dd>
					<asp:DropDownList ID="ddlShippingMethod" OnSelectedIndexChanged="ddlShippingMethod_OnSelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
				</dd>
			
			<dt>配送サービス</dt>
			<dd>
				<asp:DropDownList ID="ddlDeliveryCompany" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" AutoPostBack="true" runat="server"/>
			</dd>
			<% if (this.DeliveryCompany.IsValidShippingTimeSetFlg
					&& (this.WddlShippingMethod.SelectedValue
						== Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					&& this.UseShippingAddress)
				{ %>
					<div id="dvShippingTime" runat="server">
						<dt>配送希望時間帯</dt>
						<dd>
							<asp:DropDownList ID="ddlShippingTime" runat="server"></asp:DropDownList>
						</dd>
					</div>
				<% } %>
		</dl>

		<div class="user-footer">
			<div class="button-next">
				<asp:LinkButton ID="btnUpdateUserShippingInfo" Text="お届け先情報更新" runat="server" CssClass="btn" OnClientClick="return CheckStoreAvailable();" OnClick="btnUpdateUserShippingInfo_Click" />
			</div>
			<div class="button-prev">
				<asp:LinkButton ID="btnCloseUserShippingInfo" Text="キャンセル" runat="server" CssClass="btn" OnClientClick="return exec_submit();" OnClick="lbDisplayUserShippingInfoForm_Click" />
			</div>
		</div>
		<p class="msg">※注文済みのお届け先情報は、変更されませんのでご注意ください。</p>
		<small id="sErrorMessageShipping" class="notProductError" runat="server"></small>
		<br />
		<br />
		<%} %>

		<%-- 利用発票情報 --%>
		<% if (this.IsShowTwFixedPurchaseInvoiceInfo) { %>
			<h3>利用発票情報</h3>
			<% if (this.IsTwFixedPurchaseInvoiceModify == false) { %>
				<dl class="user-form">
					<dt>発票種類</dt>
					<dd>
						<%: ValueText.GetValueText(Constants.TABLE_TWFIXEDPURCHASEINVOICE, Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE, this.TwFixedPurchaseInvoiceModel.TwUniformInvoice) %>
						<% if (this.TwFixedPurchaseInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY) { %>
							<p style="padding-bottom: 5px; padding-top: 15px;">統一編号</p>
							<%: this.TwFixedPurchaseInvoiceModel.TwUniformInvoiceOption1 %>
							<p style="padding-bottom: 5px; padding-top: 5px;">会社名</p>
							<%: this.TwFixedPurchaseInvoiceModel.TwUniformInvoiceOption2 %>
						<% } %>
						<% if (this.TwFixedPurchaseInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE) { %>
							<p style="padding-bottom: 5px; padding-top: 15px;">寄付先コード</p>
							<%: this.TwFixedPurchaseInvoiceModel.TwUniformInvoiceOption1 %>
						<% } %>
					</dd>
					<% if (this.TwFixedPurchaseInvoiceModel.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL) { %>
					<dt>共通性載具</dt>
					<dd>
						<%: ValueText.GetValueText(Constants.TABLE_TWFIXEDPURCHASEINVOICE, Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE, this.TwFixedPurchaseInvoiceModel.TwCarryType) %>
						<br />
						<%: this.TwFixedPurchaseInvoiceModel.TwCarryTypeOption %>
					</dd>
					<% } %>
				</dl>
				<div class="button">
					<asp:LinkButton Visible="<%# this.CanCancelFixedPurchase && this.BeforeCancelDeadline %>" Text="  発票情報変更  " CssClass="btn" runat="server" OnClick="lbDisplayTwInvoiceInfoForm_Click" />
				</div>
			<% } else { %>
				<dl class="user-form">
					<dt>発票種類</dt>
					<dd>
						<asp:DropDownList ID="ddlTaiwanUniformInvoice" DataTextField="Text" DataValueField="Value" runat="server"
							OnSelectedIndexChanged="ddlTaiwanUniformInvoice_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
						&nbsp;
						<span id="spTaiwanUniformInvoiceOptionKbnList" runat="server">
							<asp:DropDownList ID="ddlTaiwanUniformInvoiceOptionKbnList" DataTextField="Text" DataValueField="Value" runat="server"
								OnSelectedIndexChanged="ddlTaiwanUniformInvoiceOptionKbnList_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
						</span>
						<div id="dvInvoiceForm" runat="server" style="padding-top:5px;" visible="false">
							<div id="dvInvoiceDisp" runat="server" visible="false">
								<br />
								<div id="dvInvoiceDispForCompanyType" runat="server">
									<p style="padding-bottom: 5px; padding-top: 5px;">統一編号</p>
									<asp:Literal ID="lCompanyCode" runat="server"></asp:Literal>
									<p style="padding-bottom: 5px; padding-top: 5px;">会社名</p>
									<asp:Literal ID="lCompanyName" runat="server"></asp:Literal>
								</div>
								<div id="dvInvoiceDispForDonateType" runat="server">
									<p style="padding-bottom: 5px; padding-top: 5px;">寄付先コード</p>
									<asp:Literal ID="lDonationCode" runat="server"></asp:Literal>
								</div>
								<br />
							</div>
							<div id="dvInvoiceInput" runat="server" visible="false">
							<br />
							<div id="dvInvoiceInputForCompanyType" runat="server" visible="false">
								<p style="padding-bottom: 5px; padding-top: 5px;">統一編号</p>
								<w2c:ExtendedTextBox runat="server" ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" MaxLength="8"></w2c:ExtendedTextBox>
								<p class="attention">
									<asp:CustomValidator
										ID="cvUniformInvoiceOption1_8"
										runat="Server"
										ControlToValidate="tbUniformInvoiceOption1_8"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										EnableClientScript="false"
										CssClass="error_inline" />
								</p>
								<p style="padding-bottom: 5px; padding-top: 5px;">会社名</p>
								<w2c:ExtendedTextBox runat="server" ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" MaxLength="20"></w2c:ExtendedTextBox>
								<p class="attention">
									<asp:CustomValidator
										ID="cvUniformInvoiceOption2"
										runat="Server"
										ControlToValidate="tbUniformInvoiceOption2"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										EnableClientScript="false"
										CssClass="error_inline" />
								</p>
							</div>
							<div id="dvInvoiceInputForDonateType" runat="server" visible="false">
								<p style="padding-bottom: 5px; padding-top: 5px;">寄付先コード</p>
								<w2c:ExtendedTextBox runat="server" ID="tbUniformInvoiceOption1_3" MaxLength="7"></w2c:ExtendedTextBox>
								<p class="attention">
									<asp:CustomValidator
										ID="cvUniformInvoiceOption1_3"
										runat="Server"
										ControlToValidate="tbUniformInvoiceOption1_3"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										SetFocusOnError="true"
										ClientValidationFunction="ClientValidate"
										EnableClientScript="false"
										CssClass="error_inline" />
								</p>
							</div>
							<asp:CheckBox ID="cbSaveToUserInvoice" Visible="false" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
							<div id="dvUniformInvoiceTypeRegistInput" runat="server" visible="false">
								電子発票情報名<span class="fred">※</span>
								<w2c:ExtendedTextBox ID="tbUniformInvoiceTypeName" MaxLength="30" runat="server"></w2c:ExtendedTextBox>
								<p class="attention">
								<asp:CustomValidator
									ID="cvUniformInvoiceTypeName" runat="server"
									ControlToValidate="tbUniformInvoiceTypeName"
									ValidationGroup="OrderShippingGlobal"
									ValidateEmptyText="true"
									ClientValidationFunction="ClientValidate"
									SetFocusOnError="true"
									EnableClientScript="false"
									CssClass="error_inline" />
								</p>
							</div>
							<br />
							</div>
						</div>
					</dd>
					<div id="trInvoiceDispForPersonalType" runat="server" visible="false">
						<dt>共通性載具</dt>
						<dd>
							<asp:DropDownList ID="ddlTaiwanCarryType" DataTextField="Text" DataValueField="Value" runat="server"
								OnSelectedIndexChanged="ddlTaiwanCarryType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
							&nbsp;
							<span id="spTaiwanCarryKbnList" runat="server" visible="false">
							<asp:DropDownList ID="ddlTaiwanCarryKbnList" DataTextField="Text" DataValueField="Value" runat="server"
								OnSelectedIndexChanged="ddlTaiwanCarryKbnList_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
							</span>
							<div id="dvInvoiceNoEquipmentMessage" runat="server" style="color:red; padding-top:15px;" visible="false">
								<asp:Literal ID="lInvoiceNoEquipmentMessage" runat="server" Text="※電子発票は商品と一緒に送ります。"></asp:Literal>
							</div>
							<div id="dvInvoiceDispForPersonalType" runat="server" visible="false">
								<br />
								<asp:Literal ID="lInvoiceCode" runat="server"></asp:Literal>
								<br />
							</div>
							<div id="dvInvoiceInputForPersonalType" runat="server" visible="false">
								<br />
								<div id="dvCarryTypeOption_8" runat="server" visible="false">
									<w2c:ExtendedTextBox runat="server" ID="tbCarryTypeOption_8" placeholder="例:/AB201+9(限8個字)" MaxLength="8"></w2c:ExtendedTextBox>
									<p class="attention">
										<asp:CustomValidator
											ID="cvCarryTypeOption_8"
											runat="Server"
											ControlToValidate="tbCarryTypeOption_8"
											ValidationGroup="OrderShippingGlobal"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false"
											CssClass="error_inline" />
									</p>
								</div>
								<div id="dvCarryTypeOption_16" runat="server" visible="false">
									<w2c:ExtendedTextBox runat="server" ID="tbCarryTypeOption_16" placeholder="例:TP03000001234567(限16個字)" MaxLength="16"></w2c:ExtendedTextBox>
									<p class="attention">
										<asp:CustomValidator
											ID="cvCarryTypeOption_16"
											runat="Server"
											ControlToValidate="tbCarryTypeOption_16"
											ValidationGroup="OrderShippingGlobal"
											ValidateEmptyText="true"
											SetFocusOnError="true"
											ClientValidationFunction="ClientValidate"
											EnableClientScript="false"
											CssClass="error_inline" />
									</p>
								</div>
								<asp:CheckBox ID="cbCarryTypeOptionRegist" runat="server" Visible="false" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
								<div id="dvCarryTypeOptionName" runat="server" visible="false">
									電子発票情報名<span class="fred">※</span>
									<w2c:ExtendedTextBox ID="tbCarryTypeOptionName" runat="server" MaxLength="30"></w2c:ExtendedTextBox>
									<p class="attention">
									<asp:CustomValidator
										ID="cvCarryTypeOptionName" runat="server"
										ControlToValidate="tbCarryTypeOptionName"
										ValidationGroup="OrderShippingGlobal"
										ValidateEmptyText="true"
										ClientValidationFunction="ClientValidate"
										SetFocusOnError="true"
										EnableClientScript="false"
										CssClass="error_inline" />
									</p>
								</div>
								<br />
							</div>
						</dd>
					</div>
				</dl>
				<div class="user-footer">
					<div class="button-next">
						<asp:LinkButton ID="btnUpdateInvoiceInfo" Text="  情報更新  " runat="server" CssClass="btn" OnClientClick="return confirm('発票情報を変更してもよろしいですか？')" OnClick="btnUpdateInvoiceInfo_Click" />
					</div>
					<div class="button-prev">
						<asp:LinkButton Text="  キャンセル  " runat="server" CssClass="btn" OnClientClick="return exec_submit();" OnClick="lbDisplayTwInvoiceInfoForm_Click" />
					</div>
				</div>
			<% } %>
		<% } %>
		<div Visible="<%# this.FixedPurchaseContainer.IsSubsctriptionBox %>" runat="server">
			<h3>頒布会情報</h3>
			<div cellspacing="0">
				<dl class="user-form">
					<dt>頒布会コース名</dt>
					<dd>
						<%#: GetSubscriptionBoxDisplayName() %>
					</dd>
				</dl>
				<dl class="user-form" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount %>" runat="server">
					<dt>定額価格</dt>
					<dd>
						<%: CurrencyManager.ToPrice(this.FixedPurchaseContainer.SubscriptionBoxFixedAmount.ToPriceDecimal()) %>
					</dd>
				</dl>
			</div>
		</div>
		<br />
		<br />
		<%-- 購入商品一覧 --%>
		<h3>お届け商品</h3>
		<div id="dvListProduct" runat="server" visible="true">
		<asp:Repeater ID="rItem" ItemType="FixedPurchaseItemInput" runat="server" OnItemDataBound="rItem_OnItemDataBound">
		<HeaderTemplate>
			<dl class="user-form">
		</HeaderTemplate>
		<ItemTemplate>
			<dd>
				<ul>
					<li>
						<w2c:ProductImage ImageSize="S" IsVariation='<%# Item.HasVariation %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="True" />
					</li>
					<li>
						<%#: Item.CreateProductJointName() %>
						[<span class="productId"><%#: (Item.ProductId == Item.VariationId) ? Item.ProductId : Item.VariationId %></span>]
						<span visible='<%# Item.ProductOptionTexts != "" %>' runat="server">
						<br />
							<%# WebSanitizer.HtmlEncode(Item.GetDisplayProductOptionTexts()).Replace("　", "<br />")%>
						</span>
					</li>
					<li>
						<% if (this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false) { %>
							<span ID="sProductPrice" runat="server">
								<%#: CurrencyManager.ToPrice(Item.ProductPriceIncludedOptionPrice) %>
							</span>
							（<%#: this.ProductPriceTextPrefix %>）&nbsp;x&nbsp;
						<% } %>
						<span ID="sItemQuantity" runat="server">
							<%#: StringUtility.ToNumeric(Item.ItemQuantity)%>
						</span>
						<%: (this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount ? "個" : "") %>
						<% if (this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false) { %>
							&nbsp;=&nbsp;
							<span ID="sOrderSubtotal" runat="server">
								<%#: CurrencyManager.ToPrice(Item.ItemPriceIncludedOptionPrice) %>
							</span>
						<% } %>
					</li>
						<% if(this.FixedPurchaseContainer.IsSubsctriptionBox == false){ %>
					<li>
						<div visible='<%# Item.HasVariation %>' style="margin-top: 5px" runat="server">
							<asp:LinkButton Text="バリエーション変更" runat="server" OnClick="lbChangeProductVariation_Click"
								CommandArgument='<%# string.Format("{0},{1},{2}",Item.ProductId,Item.VariationId,Container.ItemIndex) %>' Enabled="<%# this.BeforeCancelDeadline %>" class="btn" ID="lbChangeVariation" />
							<% this.DisplayVariationArea = true; %>
						</div>
						<div id="dvUpdateProductVariation" visible='<%# (this.WhfOldVariationId.Value == Item.VariationId) %>' style="margin-top: 5px" runat="server">
							<asp:DropDownList ID="ddlProductVariationList" runat="server" AutoPostBack="true"
								DataSource="<%# GetVariationListItems(Item.ProductId) %>" DataTextField="text"
								DataValueField="value" OnSelectedIndexChanged="ddlProductVariationList_OnSelectedIndexChanged">
							</asp:DropDownList>
							<p>
								<w2c:ProductImage ID="ProductImage" ImageSize="S"
									ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, this.WhfSelectedVariationId.Value) %>"
									IsVariation="True" runat="server" Visible="True" />
							</p>
							<p>単価（<%#: this.ProductPriceTextPrefix %>）<%# CurrencyManager.ToPrice(this.NewPrice) %></p>
							<div style="margin-top: 5px" runat="server">
								<asp:LinkButton ID="lbProductUpdate" Text="商品更新" OnClick="lbUpdateProductVariation_Click" OnClientClick="return AlertProductChange(this);" CssClass="btn" runat="server" />
								<asp:LinkButton ID="lbUpdateCancel" Text="キャンセル" OnClick="lbCloseUpdateProductVariationForm_Click" CssClass="btn" runat="server" />
							</div>
						</div>
						<div visible='<%# IsDisplayButtonChangeProductOptionValue(Container) %>' style="margin-top: 5px" runat="server">
							<asp:LinkButton Text="選択項目変更" OnClick="lbChangeProductOptionValue_Click" CommandArgument='<%# string.Format("{0},{1}", Container.ItemIndex, Item.ProductOptionTexts) %>' Enabled="<%# this.BeforeCancelDeadline %>" class="btn" ID="lbChangeProductOptionValue" runat="server" />
						</div>
						<div id="dvProductOptionValue" visible="<%# IsDisplayProductOptionValueArea(Container) %>" style="margin: 10px 0" runat="server">
							<!-- ▽ 付帯情報編集欄 ▽ -->
							<asp:Repeater ID="rProductOptionValueSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" DataSource="<%# GetProductOptionSettingList(Item) %>" runat="server">
								<HeaderTemplate>
									<div class="wrap-product-incident">
								</HeaderTemplate>
								<ItemTemplate>
									<dl>
										<dt>
											<%# (Item.IsTextBox == false) ? WebSanitizer.HtmlEncode(Item.ValueName) : ""%>
											<asp:Label ID="lblProductOptionValueSetting" Text='<%# Item.ValueName%>' Visible='<%# Item.IsTextBox %>' runat="server" /><span class="require" runat="server" visible="<%# Item.IsNecessary%>">※</span>
										</dt>
										<dd>
											<asp:Repeater ID="rCblProductOptionValueSetting" DataSource='<%# Item.SettingValuesListItemCollection %>' ItemType="System.Web.UI.WebControls.ListItem" Visible='<%# Item.IsCheckBox || Item.IsCheckBoxPrice %>' runat="server">
												<ItemTemplate>
													<asp:CheckBox ID="cbProductOptionValueSetting" Text='<%# GetAbbreviatedProductOptionTextForDisplay(Item.Text) %>' Checked='<%# Item.Selected %>' runat="server" />
													<asp:HiddenField ID="hfCbOriginalValue" runat="server" Value='<%# Item.Text %>' />
												</ItemTemplate>
											</asp:Repeater>
											<p class="attention"><asp:Label ID="lblProductOptionCheckboxErrorMessage" runat="server" /></p>
											<asp:DropDownList ID="ddlProductOptionValueSetting" DataSource='<%# Item.SettingValuesListItemCollection %>' Visible='<%# Item.IsSelectMenu || Item.IsDropDownPrice %>' SelectedValue='<%# Item.GetDisplayProductOptionSettingSelectedValue() %>' runat="server" />
											<p class="attention"><asp:Label ID="lblProductOptionDropdownErrorMessage" runat="server" /></p>
											<asp:TextBox ID="tbProductOptionValueSetting" placeholder='<%# Item.DefaultValue%>' Visible='<%# Item.IsTextBox %>' runat="server" />
											<p class="attention"><asp:Label ID = "lblProductOptionErrorMessage" runat="server" /></p>
										</dd>
									</dl>
								</ItemTemplate>
								<FooterTemplate>
									</div>
								</FooterTemplate>
							</asp:Repeater>
							<!-- △ 付帯情報編集欄 △ -->
							<div style="margin-top: 5px" runat="server">
								<asp:LinkButton ID="lbProductOptionValueUpdate" Text="更新" OnClick="lbUpdateProductOptionValue_Click" OnClientClick="return AlertProductOptionValueChange();" CssClass="btn" CommandArgument='<%# Item.ProductId %>' runat="server" />
							</div>
							<div style="margin-top: 5px" runat="server">
								<asp:LinkButton ID="lbProductOptionValueCancel" Text="キャンセル" OnClick="lbCloseUpdateProductOptionValueForm_Click" CssClass="btn" runat="server" />
							</div>
						</div>
					</li>
						<% } %>
				</ul>
			</dd>
			<asp:HiddenField ID="hfProductPrice" Value="<%#: Item.GetValidPrice().ToPriceString() %>" runat="server"/>
			<asp:HiddenField ID="hfOrderSubtotal" Value="<%#: Item.ItemPriceIncludedOptionPrice.ToPriceString() %>" runat="server"/>
			<asp:HiddenField ID="hfFixedPurchaseItemNo" Value="<%#: Item.FixedPurchaseItemNo %>" runat="server"/>
			<asp:HiddenField ID="hfProductId" Value="<%#: Item.ProductId %>" runat="server"/>
			<asp:HiddenField ID="hfVariationId" Value="<%#: Item.VariationId %>" runat="server"/>
		</ItemTemplate>
		<FooterTemplate>
			</dl>
		</FooterTemplate>
		</asp:Repeater>
		<% if (this.FixedPurchaseContainer.IsSubsctriptionBox == false) { %>
		<% if (this.DisplayVariationArea && this.IsDisplayMessageAboutChangeVariation) { %>
		<small>
			※バリエーションは次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
		</small>
		<% } %>
		<% if (this.IsDisplayMessageAboutChangeProductOptionValue) { %>
			<small>
				※選択項目は次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
			</small>
		<% } %>
		<div id="dvVariationErrorMessage" class="notProductError" visible="false" runat="server"></div>
		<div id="dvProductOptionValueErrorMessage" class="error" visible="false" runat="server"></div>
		<asp:HiddenField runat="server" ID="hfProductId" />
		<asp:HiddenField runat="server" ID="hfOldVariationId" />
		<asp:HiddenField runat="server" ID="hfSelectedVariationId" />
		<asp:HiddenField runat="server" ID="hfOldSubtotal" />
		<asp:HiddenField runat="server" ID="hfNewSubtotal" />
		<asp:HiddenField runat="server" ID="hfVariationName" />
		<asp:HiddenField runat="server" ID="hfProductOptionTexts" />
		<% }else {%>
			<% if(this.IsAbleToChangeSubscriptionBoxItemOnFront) { %>
			<div class="button">
				<asp:LinkButton ID="btnChangeProduct" Text="  お届け商品変更  " class="btn" Enabled="<%# this.BeforeCancelDeadline %>" runat="server" OnClick="btnChangeProduct_Click" />
				<% if (Constants.SUBSCRIPTION_BOX_PRODUCT_CHANGE_METHOD.IsModal()) { %>
					<asp:LinkButton ID="btnChangeProductWithClearingSelection" Text="  すべて入れ替える  " style="margin-top: 5px;" class="btn" Enabled="<%# this.BeforeCancelDeadline %>" runat="server" OnClick="btnChangeProduct_Click" />
				<% } %>
			</div>
			<small id="sErrorMessageSubscriptionBox" runat="server"></small>
			<% } %>
		<% } %>
		</div>
	

		<% if (this.FixedPurchaseContainer.IsSubsctriptionBox) { %>
		<div id="dvModifySubscription" runat="server" visible="false">
			<div>
				<small ID="sErrorQuantity" style="color:red" runat="server"></small>
			</div>
			<asp:Repeater ID="rItemModify" ItemType="FixedPurchaseItemInput" runat="server" OnItemDataBound="rItemModify_OnItemDataBound" OnItemCommand="rProductChange_ItemCommand">
			<HeaderTemplate>
				<dl class="user-form">
			</HeaderTemplate>
			<ItemTemplate>
				<dd>
					<ul>
						<li class="productImage">
							<w2c:ProductImage  ImageSize="S" IsVariation='<%# Item.HasVariation %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="True" />
						</li>
						<li class="productDelete">
							<asp:Button ID="lnkUpdate" Text="  削除  " Visible="<%# CanModify(Item.VariationId) %>" runat="server" CommandName="DeleteRow" CommandArgument='<%# Container.ItemIndex %>' class="btn"/>
						</li>
						<li class="productName">
							<asp:DropDownList ID="ddlProductName" Enabled="<%# CanModify(Item.VariationId) %>" DataSource="<%# GetSubscriptionBoxProductList(Item.ProductId,Item.VariationId) %>" DataTextField="Text" DataValueField="Value" runat="server" 
								OnSelectedIndexChanged="ReCalculation" AutoPostBack="true" style="width:240px"></asp:DropDownList>
						</li>
						<li class="productPrice" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
							<span ID="sProductPrice" runat="server">
								<%#: CurrencyManager.ToPrice(Item.GetValidPrice()) %>
							</span>
							（<%#: this.ProductPriceTextPrefix %>)<br />
							<span runat="server" Visible="<%# CanModify(Item.VariationId) == false %>"><%: GetSubscriptionBoxNecessaryMessage() %></span>
						</li>
						<li class="orderCount">
							&nbsp;x&nbsp; <asp:DropDownList ID="ddlQuantityUpdate" runat="server" DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ReCalculation" AutoPostBack="true" />
						</li>
						<li class="orderSubtotal" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
							&nbsp;=&nbsp;
							<span ID="sOrderSubtotal" runat="server">
								<%#: CurrencyManager.ToPrice(Item.ItemPriceIncludedOptionPrice) %>
							</span>
						</li>
						<asp:HiddenField ID="hfProductPrice" Value="<%#: Item.GetValidPrice().ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfOrderSubtotal" Value="<%#: Item.ItemPriceIncludedOptionPrice.ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfProductId" Value="<%#: Item.ProductId %>" runat="server"/>
						<asp:HiddenField ID="hfVariationId" Value="<%#: Item.VariationId %>" runat="server"/>
						<%-- 商品変更数選択肢（1から設定分表示） --%>
						<asp:HiddenField ID="hfMaxItemQuantity" runat="server" Value="10" />
						<asp:HiddenField ID="hfItemQuantity" runat="server" Value="<%#: Item.ItemQuantity %>" />
					</ul>
				</dd>
			</ItemTemplate>
			<FooterTemplate>
				</dl>
			</FooterTemplate>
			</asp:Repeater>
			<div class="user-footer">
				<div class="button-next" >
					<asp:LinkButton ID="btnAddProduct" Text="  商品追加  " class="btn" runat="server" OnClick="btnAddProduct_Click" Visible="<%# CanAddProduct() %>" />
				</div>
				<div class="button-next">
					<asp:LinkButton ID="btnUpdateProduct" Text="  更新  " class="btn" runat="server" OnClick="btnUpdateProduct_Click" />
				</div>
				<div class="button-prev">
					<asp:LinkButton ID="btnCloseProduct" Text="  キャンセル  " runat="server" class="btn" OnClick="btnCloseProduct_Click" />
				</div>
			</div>
		</div>
		<% } %>
	
	<div class="dvFixedPurchaseItem" id="dvModifyFixedPurchase" runat="server" visible="false">
			<div>
				<small ID="sProductModifyErrorMessage" class="notProductError" runat="server"></small>
			</div>
		<table cellspacing="0">
				<asp:Repeater ID="rFixedPurchaseModifyProducts" ItemType="FixedPurchaseItemInput" OnItemDataBound="rFixedPurchaseModifyProducts_OnItemDataBound" runat="server">
					<HeaderTemplate>
						<tr>
							<th class="subscriptionBoxProductName" colspan="2">
								商品名
							</th>
							<th class="subscriptionBoxProductPrice" runat="server">
								単価（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<% if (IsDisplayOptionPrice()){ %>
							<th class="productOptionPrice">
								オプション価格（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<% } %>
							<th class="subscriptionBoxOrderCount">
								注文数
							</th>
							<th class="subscriptionBoxOrderSubtotal" runat="server">
								小計（<%#: this.ProductPriceTextPrefix %>）
							</th>
							<th class="subscriptionBoxProductDelete">
								<%# Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED == false ? "削除" : string.Empty %>
							</th>
						</tr>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td class="productImage">
								<w2c:ProductImage ImageSize="S" IsVariation='<%# Item.HasVariation %>' ProductMaster="<%# GetProduct(Item.ShopId, Item.ProductId, Item.VariationId) %>" runat="server" Visible="True" />
							</td>
							<td class="productName">
								<%#: Item.CreateProductJointName() %>
								<br />
								[<span class="productId"><%#: (Item.ProductId == Item.VariationId) ? Item.ProductId : Item.VariationId %></span>]
								<span visible='<%# Item.ProductOptionTexts != "" %>' runat="server">
									<br />
									<%# WebSanitizer.HtmlEncode(Item.GetDisplayProductOptionTexts()).Replace("　", "<br />")%>
								</span>
							</td>
							<td class="productPrice" runat="server">
								<span ID="sProductPrice" runat="server"><%#: CurrencyManager.ToPrice(Item.GetValidPrice()) %></span>
							</td>
							<% if (IsDisplayOptionPrice()){ %>
							<td class="productPrice" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
								<span ID="oProductPrice" runat="server">
									<%#: Item.ProductOptionSettingsWithSelectedValues.HasOptionPrice == false ? "―" : CurrencyManager.ToPrice(Item.OptionPrice) %>
								</span>
							</td>
							<% } %>
							<td class="orderCount">
								<asp:DropDownList ID="ddlQuantityUpdate" runat="server" DataTextField="Text" DataValueField="Value" OnTextChanged="ProductModifyReCalculation" AutoPostBack="true" />
							</td>
							<td class="orderSubtotal" runat="server">
								<span ID="sOrderSubtotal" runat="server">
									<%#: CurrencyManager.ToPrice(Item.ItemPriceIncludedOptionPrice) %>
								</span>
							</td>
							<td class="delete" visible="<%# (Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED == false) || (Item.HasProductChangeSetting == false) %>" runat="server">
								<span visible="<%# Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED %>" runat="server">削除</span>
							</td>
							<td class="delete">
								<asp:CheckBox ID="cbDeleteProduct" AutoPostBack="true" OnCheckedChanged="cbDeleteProduct_CheckedChanged" Checked="<%# Item.ModifyDeleteTarget %>" runat="server" />
							</td>
							<td Visible="<%# Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED && Item.HasProductChangeSetting %>" runat="server">
								<asp:Button ID="btnFixedPurchaseProductChange" Text="変更" OnClick="btnFixedPurchaseProductChange_OnClick" CommandArgument="<%# Item.ProductId %>" style="border: 1px solid #cccccc; border-radius: 4px;" runat="server" />
							</td>
						</tr>
						<tr>
							<td>
								<div visible='<%# IsDisplayButtonChangeProductOptionValue(Container) %>' style="margin-top: 5px" runat="server">
									<asp:LinkButton Text="選択項目変更" OnClick="lbChangeProductOptionValue_Click" CommandArgument='<%# string.Format("{0},{1}", Container.ItemIndex, Item.ProductOptionTexts) %>' Enabled="<%# this.BeforeCancelDeadline %>" class="btn" ID="lbChangeProductOptionValue" runat="server" />
								</div>
								<div id="dvProductOptionValue" visible="<%# IsDisplayProductOptionValueArea(Container) %>" style="margin: 10px 0" runat="server">
									<!-- ▽ 付帯情報編集欄 ▽ -->
									<asp:Repeater ID="rProductOptionValueSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" DataSource="<%# GetProductOptionSettingList(Item) %>" runat="server">
										<HeaderTemplate>
											<div class="wrap-product-incident">
											</HeaderTemplate>
										<ItemTemplate>
											<dl>
												<dt>
													<%# (Item.IsTextBox == false) ? WebSanitizer.HtmlEncode(Item.ValueName) : ""%>
													<asp:Label ID = "lblProductOptionValueSetting" Text = '<%# Item.ValueName%>' visible='<%# Item.IsTextBox %>' runat="server" /><span class="require" runat="server" visible="<%# Item.IsNecessary%>">※</span>
												</dt>
												<dd>
													<asp:Repeater ID="rCblProductOptionValueSetting" ItemType="System.Web.UI.WebControls.ListItem" DataSource='<%# Item.SettingValuesListItemCollection %>' Visible='<%# Item.IsCheckBox || Item.IsCheckBoxPrice %>' runat="server" >
														<ItemTemplate>
															<asp:CheckBox ID="cbProductOptionValueSetting" Text='<%# GetAbbreviatedProductOptionTextForDisplay(Item.Text) %>' Checked='<%# Item.Selected %>' runat="server" />
															<asp:HiddenField ID="hfCbOriginalValue" runat="server" Value='<%# Item.Text %>' />
														</ItemTemplate>
													</asp:Repeater>
													<p class="attention"><asp:Label ID="lblProductOptionCheckboxErrorMessage" runat="server" /></p>
													<asp:DropDownList ID="ddlProductOptionValueSetting" DataSource='<%# Item.SettingValuesListItemCollection %>' visible='<%# Item.IsSelectMenu || Item.IsDropDownPrice  %>' SelectedValue='<%# Item.GetDisplayProductOptionSettingSelectedValue() %>' runat="server" />
													<p class="attention"><asp:Label ID="lblProductOptionDropdownErrorMessage" runat="server" /></p>
													<asp:TextBox ID ="tbProductOptionValueSetting" placeholder = '<%# Item.DefaultValue%>' visible='<%# Item.IsTextBox %>' runat="server" />
													<p class="attention"><asp:Label ID = "lblProductOptionErrorMessage" runat="server" /></p>
												</dd>
											</dl>
										</ItemTemplate>
										<FooterTemplate>
											</div>
										</FooterTemplate>
									</asp:Repeater>
									<!-- △ 付帯情報編集欄 △ -->
									<div style="margin-top: 5px" runat="server">
										<asp:LinkButton ID="lbInputProductOptionValueUpdate" Text="更新" OnClick="lbInputProductOptionValueUpdate_Click" CssClass="btn" CommandArgument='<%# Item.ProductId %>' runat="server" />
										<asp:LinkButton ID="lbInputProductOptionValueCancel" Text="キャンセル" OnClick="lbInputProductOptionValueCancel_Click" CssClass="btn" runat="server" />
									</div>
								</div>
							</td>
						</tr>
						<asp:HiddenField ID="hfProductPrice" Value="<%# Item.GetValidPrice().ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfOrderSubtotal" Value="<%# Item.ItemPriceIncludedOptionPrice.ToPriceString() %>" runat="server"/>
						<asp:HiddenField ID="hfFixedPurchaseItemNo" Value="<%# Item.FixedPurchaseItemNo %>" runat="server"/>
						<asp:HiddenField ID="hfProductId" Value="<%# Item.ProductId %>" runat="server"/>
						<asp:HiddenField ID="hfVariationId" Value="<%# Item.VariationId %>" runat="server"/>
						<asp:HiddenField ID="hfProductOptionTexts" Value="<%# Item.ProductOptionTexts %>" runat="server"/>
						<%-- 商品変更数選択肢（1から設定分表示） --%>
						<asp:HiddenField ID="hfMaxItemQuantity" runat="server" Value="10" />
						<asp:HiddenField ID="hfItemQuantity" runat="server" Value="<%#: Item.ItemQuantity %>" />
					</ItemTemplate>
				</asp:Repeater>
			</table>
			<% if (this.DisplayVariationArea && this.IsDisplayMessageAboutChangeVariation) { %>
			<small>
				※バリエーションは次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
			</small>
			<% } %>
			<% if (this.IsDisplayMessageAboutChangeProductOptionValue) { %>
			<small>
				※選択項目は次回お届け予定日の <%#: this.ShopShipping.FixedPurchaseCancelDeadline %>日前 まで変更可能です。<br />
			</small>
			<% } %>
			<div id="Div1" class="error" visible="false" runat="server"></div>
			<div id="Div4" class="error" visible="false" runat="server"></div>
			<asp:HiddenField runat="server" ID="HiddenField1" />
			<asp:HiddenField runat="server" ID="HiddenField2" />
			<asp:HiddenField runat="server" ID="HiddenField3" />
			<asp:HiddenField runat="server" ID="HiddenField4" />
			<asp:HiddenField runat="server" ID="HiddenField5" />
			<asp:HiddenField runat="server" ID="HiddenField6" />
			<asp:HiddenField runat="server" ID="HiddenField7" />
		<div class="right" style="margin-bottom: 20px;">
			<asp:LinkButton ID="btnModifyAddProduct" Text="  商品追加  "  CssClass="btn" OnClick="btnModifyAddProduct_OnClick" runat="server"/>
			<asp:LinkButton ID="btnModifyConfirm" OnClientClick="return AlertProductModify()" Text="  更新  " CssClass="btn" OnClick="btnModifyConfirm_OnClick" runat="server" style="margin-top:5px"/>
			<asp:LinkButton ID="btnModifyCancel" Text="  キャンセル  " CssClass="btn" OnClick="btnModifyCancel_OnClick" runat="server" style="margin-top:5px"/>
		</div>
	</div>
	<div class="right" style="width: 100%; margin-bottom: 20px;" Visible="<%# this.IsOrderModifyBtnDisplay %>" runat="server">
		<asp:LinkButton id ="btnModifyProducts" CssClass="btn" OnClick="btnModifyProducts_OnClick" runat="server">
			<%# WebSanitizer.HtmlEncode(Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED ? "お届け商品変更" : "お届け商品数変更") %>
		</asp:LinkButton>
	</div>

		<div class="dvPlannedTotalAmountForTheNextOrder">
			<dl class="user-form">
				<dt>次回注文時のお支払い予定金額</dt>
			</dl>
			<div class="cart-unit">
				<div class="cart-unit-footer">
					<dl>
						<dt>商品合計</dt>
						<dd>
							<asp:Literal runat="server" ID="lNextProductSubTotal" Text="<%#: CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.PriceSubtotal) %>"></asp:Literal>
						</dd>
					</dl>
					<%-- クーポン情報リスト(有効な場合) --%>
					<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
					<dl>
						<dt>クーポン割引額</dt>
						<dd class="discount">
							<span>
								<asp:Literal runat="server" ID="lNextCouponName" Text="<%#: GetCouponName(this.NextShippingFixedPurchaseCart) %>"/>
							</span>
							<span>
								<asp:Literal runat="server" ID="lNextOrderCouponUse" Text='<%#: ((this.NextShippingFixedPurchaseCart.UseCouponPriceForProduct > 0) ? "-" : "") + CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.UseCouponPriceForProduct) %>'/>
							</span>
						</dd>
					</dl>
					<% } %>
					<%-- ポイント情報リスト(有効な場合) --%>
					<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
					<dl>
						<dt>ポイント利用額</dt>
						<dd class="discount">
							<% if (this.CanUseAllPointFlg && this.FixedPurchaseContainer.IsUseAllPointFlg) { %>
							全ポイント継続利用
							<% } else { %>
								<asp:Literal runat="server" ID="lNextUsePointPrice" Text='<%#: ((this.NextShippingFixedPurchaseCart.UsePointPrice > 0) ? "-" : "") + CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.UsePointPrice) %>'/>
							<% } %>
						</dd>
					</dl>
					<% } %>
					<%-- 定期購入割引 --%>
					<dl runat="server">
						<dt>定期購入割引額</dt>
						<dd class="discount">
							<asp:Literal runat="server" ID="lNextFixedPurchaseDiscountPrice" Text='<%#: ((this.NextShippingFixedPurchaseCart.FixedPurchaseDiscount > 0) ? "-" : "") + CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.FixedPurchaseDiscount) %>'/>
						</dd>
					</dl>
					<%-- 配送料金 --%>
					<% if (Constants.W2MP_COUPON_OPTION_ENABLED && this.IsFreeShippingByCoupon) { %>
					<dl>
						<dt>配送料金</dt>
						<dd class="discount">クーポン利用のため無料</dd>
					</dl>
					<% } %>
					<dl>
						<dt>総合計(税込)</dt>
						<dd>
							<asp:Literal runat="server" ID="lNextOrderTotal" Text="<%#: CurrencyManager.ToPrice(this.PlannedTotalAmountForNextOrder) %>"/>
						</dd>
					</dl>
					<p>※配送料金と決済手数料は次回注文確定時に確定します。</p>
					<p>※表示価格はあくまでも目安であり、実際の価格は次回購入時に各種キャンペーン状況やポイント・クーポンの適用可否によって異なる場合がございます。予めご了承ください。</p>
				</div>
			</div>
		</div>

		<div class="order-history-list order-unit dvOrderHistoryList" ID="dvOrderHistoryList" runat="server">
			<h2>定期購入履歴</h2>
			<asp:HiddenField runat="server" ID="hfPagerMaxDispCount" Value="10"/>
			<asp:Repeater ID="rOrderList" Runat="server">
				<HeaderTemplate>
					<p class="msg">※履歴には、定期購入した商品だけではなく一緒に注文した商品も表示されます。</p>
					<p class="msg">※ご注文内容の詳細をご覧になるには【ご注文番号】のリンクを押してください。</p>
					<%-- ページャ --%>
					<div class="pager-wrap above"><%= this.PagerHtml %></div>
					<div class="content">
					<ul>
				</HeaderTemplate>
				<ItemTemplate>
					<li>
						<h3>ご注文番号</h3>
						<h4 class="order-id"><a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>'>
								<%#: Eval(Constants.FIELD_ORDER_ORDER_ID) %></a></h4>
						<dl class="order-form">
							<dt>ご購入日</dt>
							<dd><%#: DateTimeUtility.ToStringFromRegion(Eval(Constants.FIELD_ORDER_ORDER_DATE), DateTimeUtility.FormatType.ShortDate2Letter) %></dd>
							<dt>総合計</dt>
							<dd><%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL)) %></dd>
							<dt>ご注文状況</dt>
							<dd><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, Eval(Constants.FIELD_ORDER_ORDER_STATUS)) %><%#: (string)Eval(Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN) == Constants.FLG_ORDER_SHIPPED_CHANGED_KBN_CHANAGED ? "（変更有り）" : "" %></dd>
							<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
								<!--show credit status if using GMO-->
								<dt runat="server" visible="<%# ((Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) %>">
									与信状況
								</dt>
								<dd visible="<%# ((Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (Eval(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).ToString() == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)) %>" runat="server">
									<%# ValueText.GetValueText(Constants.TABLE_ORDER,Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, Eval(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS))%>
								</dd>
							<% } %>
							<dt>配送希望日</dt>
							<dd><%# WebSanitizer.HtmlEncodeChangeToBr(GetShippingDate(Eval(Constants.FIELD_ORDER_ORDER_ID).ToString())) %></dd>
							<% if(this.DisplayScheduledShippingDate) { %>
							<dt>出荷予定日</dt>
							<dd><%#: GetScheduledShippingDate(Eval(Constants.FIELD_ORDER_ORDER_ID).ToString()) %></dd>
							<% } %>
						</dl>
						<div class="button order-histori-button">
							<a href='<%#: Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) %>' class="btn">
							詳細はこちら</a>
						</div>
					</li>
				</ItemTemplate>
				<FooterTemplate>
					</ul>
					</div>
					<%-- ページャ --%>
					<div class="pager-wrap above"><%= this.PagerHtml %></div>
				</FooterTemplate>
			</asp:Repeater>
		</div>
	</div>
</div>

<div class="user-footer">
	<div class="button-next">
		<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Form/FixedPurchase/FixedPurchaseList.aspx") %>" class="btn">戻る</a>
	</div>
</div>

<%-- 頒布会：商品変更（モーダル方式） --%>
<div id="dvModifySubscriptionBoxModalBg" class="modal-bg" Visible="False" runat="server">
	<div id="dvModifySubscriptionBoxModalContent" class="modal-content">
		<iframe src="<%: this.SubscriptionBoxProductChangeModalUrl %>" scrolling="no">
			お使いのブラウザで商品変更画面を表示することができません。
			最新のブラウザをご利用ください。
		</iframe>
	</div>
</div>

</section>
<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
<script src="https://static-fe.payments-amazon.com/checkout.js"></script>
<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>
<script type="text/javascript">
	bindEvent();
	scrollToFixedPurchaseItems();

	<%-- UpdataPanelの更新時のみ処理を行う --%>
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			bindEvent();
			window.exec_submit_flg = 0;
		}
	}

	<%-- イベントをバインドする --%>
	function bindEvent() {
		bindExecAutoKana();
		bindExecAutoChangeKana();
		bindZipCodeSearch();
		<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
		showAmazonButton('#AmazonPayCv2Button');
		showAmazonButton('#AmazonPayCv2Button2');
		<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>
		<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
		bindTwAddressSearch();
		<% } %>
	}

	<%-- 氏名（姓・名）の自動振り仮名変換のイベントをバインドする --%>
	function bindExecAutoKana() {
		execAutoKanaWithKanaType(
			$("#<%= tbShippingName1.ClientID %>"),
			$("#<%= tbShippingNameKana1.ClientID %>"),
			$("#<%= tbShippingName2.ClientID %>"),
			$("#<%= tbShippingNameKana2.ClientID %>"));
	}

	<%-- ふりがな（姓・名）のかな←→カナ自動変換イベントをバインドする --%>
	function bindExecAutoChangeKana() {
		execAutoChangeKanaWithKanaType(
			$("#<%= tbShippingNameKana1.ClientID %>"),
			$("#<%= tbShippingNameKana2.ClientID %>"));
	}

	<%-- 郵便番号検索のイベントをバインドする --%>
	function bindZipCodeSearch() {
		// Check zip code input on click
		clickSearchZipCodeForSp(
			'<%= this.WtbShippingZip.ClientID %>',
			'<%= this.WtbShippingZip1.ClientID %>',
			'<%= this.WtbShippingZip2.ClientID %>',
			'<%= this.WlbSearchShippingAddr.ClientID %>',
			'<%= this.WlbSearchShippingAddr.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage',
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_NECESSARY", "郵便番号") %>',
			'<%: w2.Common.Util.Validator.GetErrorMessage("CHECK_LENGTH", "郵便番号", "7") %>');

		// Check zip code input on text box change
		textboxChangeSearchZipCodeForSp(
			'<%= this.WtbShippingZip.ClientID %>',
			'<%= this.WtbShippingZip1.ClientID %>',
			'<%= this.WtbShippingZip2.ClientID %>',
			'<%= this.WtbShippingZip1.UniqueID %>',
			'<%= this.WtbShippingZip2.UniqueID %>',
			'<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_ZIPCODE_SEARCHER_GET_ADDR_JSON %>',
			'#addrSearchErrorMessage');

		<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
		// Textbox change search zip global
		textboxChangeSearchGlobalZip(
			'<%= this.WtbShippingZipGlobal.ClientID %>',
			'<%= this.WlbSearchAddrFromShippingZipGlobal.UniqueID %>');
		<% } %>
	}

	<%--▼▼ Amazon Pay(CV2)スクリプト ▼▼--%>
	function showAmazonButton(divId) {
		showAmazonPayCv2Button(
			divId,
			'<%= Constants.PAYMENT_AMAZON_SELLERID %>',
			<%= Constants.PAYMENT_AMAZON_ISSANDBOX.ToString().ToLower() %>,
			'<%= this.AmazonRequest.Payload %>',
			'<%= this.AmazonRequest.Signature %>',
			'<%= Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID %>');
	}
	<%--▲▲ Amazon Pay(CV2)スクリプト ▲▲--%>

	$(document).on('click', '.search-result-layer-close', function () {
		closePopupAndLayer();
	});

	$(document).on('click', '.search-result-layer-addr', function () {
		bindSelectedAddr($('li.search-result-layer-addr').index(this));
	});

	<%-- 複数住所検索結果からの選択値を入力フォームにバインドする --%>
	function bindSelectedAddr(selectedIndex) {
		var selectedAddr = $('.search-result-layer-addrs li').eq(selectedIndex);
		$("#<%= ddlShippingAddr1.ClientID %>").val(selectedAddr.find('.addr').text());
		$("#<%= tbShippingAddr2.ClientID %>").val(selectedAddr.find('.city').text() + selectedAddr.find('.town').text());
		$("#<%= tbShippingAddr3.ClientID %>").focus();

		closePopupAndLayer();
	}

	<%-- 定期購入再開の確認メッセージを生成 --%>
	function getResumeShippingMesasge() {
		var selectedValue = $("#<%= ddlResumeFixedPurchaseDate.ClientID %>").val();
		if (selectedValue == "<%= ReplaceTag("@@DispText.shipping_date_list.none@@") %>") {
			return "次回配送日・次々回配送日は自動で計算します。"
		} else {
			return "次々回配送日は自動で計算します。"
		}
	}

	<%-- 頒布会コース再開の確認メッセージを生成 --%>
	function getResumeSubscriptionBoxShippingMesasge() {
		var selectedValue =  $("#<%= ddlResumeSubscriptionBoxDate.ClientID %>").val();
		if (selectedValue == "<%= ReplaceTag("@@DispText.shipping_date_list.none@@") %>") {
			return "次回配送日・次々回配送日は自動で計算します。"
		} else {
			return "次々回配送日は自動で計算します。"
		}
	}
	<% if(Constants.GLOBAL_OPTION_ENABLE) { %>
	<%-- 台湾郵便番号取得関数 --%>
	function bindTwAddressSearch() {
		$('#<%= this.WddlShippingAddr3.ClientID %>').change(function (e) {
			$('#<%= this.WtbShippingZipGlobal.ClientID %>').val(
				$('#<%= this.WddlShippingAddr3.ClientID %>').val());
		});
	}
	<% } %>

	<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup() {
		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl() %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel) {
		var elements = document.getElementsByClassName("shipping_convenience")[0];
		if (elements == undefined || elements == null) {
			var seletedShippingNo = $('.UserShippingAddress').val();
			var className = '.user_addres' + seletedShippingNo;
			elements = $(className)[0];
			var hfSelectedShopId = $("#<%= WhfSelectedShopId.ClientID %>");
			hfSelectedShopId.val(cvsspot);
		}

		// For display
		elements.querySelector('[id$="ddCvsShopId"] > span').innerHTML = cvsspot;
		elements.querySelector('[id$="ddCvsShopName"] > span').innerHTML = name;
		elements.querySelector('[id$="ddCvsShopAddress"] > span').innerHTML = addr;
		elements.querySelector('[id$="ddCvsShopTel"] > span').innerHTML = tel;

		// For get value
		elements.querySelector('[id$="hfCvsShopId"]').value = cvsspot;
		elements.querySelector('[id$="hfCvsShopName"]').value = name;
		elements.querySelector('[id$="hfCvsShopAddress"]').value = addr;
		elements.querySelector('[id$="hfCvsShopTel"]').value = tel;
	}

	<%-- Check Store Available --%>
	function CheckStoreAvailable() {
		<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
		var isShopValid = false;
		var isShippingConvenience = ($("#<%= WhfCvsShopFlg.ClientID %>").val() == '<%= Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON %>');
		if (isShippingConvenience) {
			var hfCvsShopId = $("#<%= WhfShippingReceivingStoreId.ClientID %>");
			var hfSelectedShopId = $("#<%= WhfSelectedShopId.ClientID %>");
			var shopId = hfCvsShopId.val();
			if (shopId == undefined || shopId == null || shopId == '') {
				shopId = hfSelectedShopId.val();
			}
			var dvErrorShippingConvenience = $("#<%= WdvErrorShippingConvenience.ClientID %>");
			$.ajax({
				type: "POST",
				url: "<%= Constants.PATH_ROOT + "SmartPhone/" + Constants.PAGE_FRONT_FIXED_PURCHASE_DETAIL %>/CheckStoreIdValid",
				data: JSON.stringify({ storeId: shopId }),
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				cache: false,
				async: false,
				success: function (data) {
					if (data.d) {
						isShopValid = true;
						dvErrorShippingConvenience.css("display", "none");
					}
					else {
						dvErrorShippingConvenience.css("display", "");
					}
				}
			});

			if (isShopValid == false) return isShopValid;
		}
		<% } %>

		return confirm('お届け先情報を変更してもよろしいですか？');
	}
	<% } %>

	<%--領収書情報が削除されるの確認メッセージ作成 --%>
	function getDeleteReceiptInfoMessage() {
		var message = "";

		// 領収書情報が削除される場合、アラート追加
		<% if (Constants.RECEIPT_OPTION_ENABLED && (this.FixedPurchaseContainer.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON)) { %>
		if ("<%= string.Join(",", Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN) %>".indexOf(document.getElementById("<%= hfPaymentIdSelected.ClientID %>").value) !== -1)
		{
			message = '\n指定したお支払い方法は、領収書の発行ができません。\n'
				+ '保存されている「領収書情報」が削除されます。\n';
		}
		<% } %>
		return message;
	}

	<% if (string.IsNullOrEmpty(this.ScrollPositionIdAfterProductOptionUpdate) == false){ %>
	$(window).on('load', function () {
		setTimeout(function () {
			scrollToElementCenter("<%= this.ScrollPositionIdAfterProductOptionUpdate %>");
		}, 0);
	});
	<% } %>

	// Alert Payment Change
	function AlertPaymentChange() {
		var result = false;
		var spanErrorMessageForAtone = document.getElementsByClassName('spanErrorMessageForAtone');
		if ((spanErrorMessageForAtone.length > 0)
			&& (spanErrorMessageForAtone[0].style.display == "block")) {
			return result;
		}

		var spanErrorMessageForAftee = document.getElementsByClassName('spanErrorMessageForAftee');
		if ((spanErrorMessageForAftee.length > 0)
			&& (spanErrorMessageForAftee[0].style.display == "block")) {
			return result;
		}
		if (confirm('お支払い方法を変更します。' + getDeleteReceiptInfoMessage() + '\nよろしいですか？')) {
			doPostbackEvenIfCardAuthFailed = false;
			<% if (Constants.PAYMENT_PAIDY_OPTION_ENABLED) { %>
			result = false;

			PaidyPayProcess();
			<% } else { %>
			result = true;
			<% } %>
		} else {
			doPostbackEvenIfCardAuthFailed = true;
		}

		return result;
	}

	function confirmUpdateNextNextShippingDate() {
		var isChecked = $('[id$="cbUpdateNextNextShippingDate"]').prop('checked');
		var hasCheckBox = JSON.parse('<%= this.WcbUpdateNextNextShippingDate.HasInnerControl %>'.toLowerCase());
		var isSetting = JSON.parse('<%= Constants.FIXEDPURCHASEORDERNOW_NEXT_NEXT_SHIPPING_DATE_UPDATE_DEFAULT %>'.toLowerCase());
		if (hasCheckBox ? isChecked : isSetting){ 
			return confirm('次回配送予定日と次々回配送予定日を変更します。\n本当によろしいでしょうか？');
		}else{ 
			return confirm('次回配送予定日を変更します。\n本当によろしいですか？');
		} 
	}

	//商品変更時の確認フォーム
	function AlertProductChange() {
		var messagePayment;
		var priceTotalOld = document.getElementById("<%= hfOldSubtotal.ClientID %>").value;
		var priceTotalNew = document.getElementById("<%= hfNewSubtotal.ClientID %>").value;
		messagePayment = '商品を「' + document.getElementById("<%= hfVariationName.ClientID %>").value + '」に変更します。';
		if (priceTotalOld != priceTotalNew) {
			messagePayment += '\n変更に伴い、価格が変更されます。\n\n'
				+ '    変更前の価格：' + priceTotalOld + '\n'
				+ '    変更後の価格：' + priceTotalNew + '\n\n';
		}
		messagePayment += 'よろしいですか？\n\n';
		return confirm(messagePayment);
	}
	
	//商品付帯情報変更時の確認フォーム
	function AlertProductOptionValueChange() {
		const regex = /(\d+)/; // 正規表現パターンを定義する
		var limitCouponUseMinPrice = <%= this.FixedPurchaseContainer.NextShippingUseCouponDetail == null
			? 0
			: this.FixedPurchaseContainer.NextShippingUseCouponDetail.UsablePrice == null
				? 0
				: this.FixedPurchaseContainer.NextShippingUseCouponDetail.UsablePrice %>;
		var limitPointUseMinPrice = <%= this.FixedPurchaseContainer.NextShippingUsePoint > 0 ? Constants.POINT_MINIMUM_PURCHASEPRICE :0 %>;
		var limitPointUseMinPrice = <%= this.FixedPurchaseContainer.NextShippingUsePoint > 0 ? Constants.POINT_MINIMUM_PURCHASEPRICE :0 %>;
		var productOptionPrice = 0;
		var productPrice = 0;

		// チェックボックス型の商品付帯価格の合算
		$('[id$=cbProductOptionValueSetting]:checked').each(function () {
			var id = $(this).attr('id');
			var label = $('label[for="' + id + '"]').text();
			var match = label.replace(",", "").match(regex);
			if (match) productOptionPrice = productOptionPrice + Number(match[0]);
		});

		// ドロップダウンリスト型の商品付帯価格の合算
		$('[id$=ddlProductOptionValueSetting] option:selected').each(function () {
			var match = $(this).text().replace(",", "").match(regex);
			if (match) productOptionPrice = productOptionPrice + Number(match[0]);
		});

		var match = $('[id$=hfOrderSubtotal]').val().replace(",", "").match(regex);
		if (match) productPrice = Number(match[0]);

		console.log(productPrice + "付帯:" + productOptionPrice);
		console.log(limitCouponUseMinPrice + "point:" + limitPointUseMinPrice);
		var disableUseCoupon = "<%= Constants.W2MP_COUPON_OPTION_ENABLED %>" && ((productPrice + productOptionPrice) < limitCouponUseMinPrice);
		var disableUsePoints = "<%= Constants.W2MP_POINT_OPTION_ENABLED %>" && ((productPrice + productOptionPrice) < limitPointUseMinPrice);

		var messageConfirmModify = "";
		if (disableUseCoupon && disableUsePoints) {
			messageConfirmModify = "現在の価格はクーポンの適用外です。\n現在の価格はポイント適用外です。\nOKを押すことで価格が更新され、クーポンとポイントが\nリセットされます";
		}
		else if (disableUseCoupon) {
			messageConfirmModify = "現在の価格はクーポンの適用外です。\nOKを押すことで価格が更新され、クーポンがリ\nセットされます";
		}
		else if (disableUsePoints) {
			messageConfirmModify = "現在の価格はポイントの適用外です。\nOKを押すことで価格が更新され、ポイントがリ\nセットされます";
		} else {
			messageConfirmModify = '選択項目を変更します。\n\nよろしいですか？\n\n';
		}

		return confirm(messageConfirmModify);
	}

	// 頒布会商品変更時の確認フォーム
	function AlertSubscriptionBoxProductChange() {
		var message = '次回配送商品を変更します。 \nよろしいですか？';
		return confirm(message);
	}

	//商品変更時の確認フォーム
	function AlertProductModify() {
		return confirm("次回お届け商品を変更します。\n\nよろしいですか？");
	}

	// 商品追加画面からの遷移時、定期アイテム要素までスクロールする
	function scrollToFixedPurchaseItems() {
		if (document.referrer.indexOf("<%= Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD %>") !== -1) {
			var top = $("#<%= dvModifyFixedPurchase.ClientID %>").offset().top;
			$(window).scrollTop(top);
		}
	}
</script>

<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>

<%--▼▼ Paidy用スクリプト ▼▼--%>
<script type="text/javascript">
	var buyer = <%= PaidyUtility.CreatedBuyerDataObjectForPaidyPayment(this.LoginUserId) %>;
	var hfPaidyTokenIdControlId = "<%= this.WhfPaidyTokenId.ClientID %>";
	var hfPaidyPaySelectedControlId = "<%= this.WhfPaidyPaySelected.ClientID %>";
	var updatePaymentUniqueID = "<%= this.WlbUpdatePayment.UniqueID %>";
	var isHistoryPage = true;
</script>
<uc:PaidyCheckoutScript ID="ucPaidyCheckoutScript" runat="server" />
<%--▲▲ Paidy用スクリプト ▲▲--%>
<input type="hidden" id="fraudbuster" name="fraudbuster" />
<script type="text/javascript" src="//cdn.credit.gmo-ab.com/psdatacollector.js"></script>
<uc:RakutenPaymentScript ID="ucRakutenPaymentScript" runat="server" />
</asp:Content>
