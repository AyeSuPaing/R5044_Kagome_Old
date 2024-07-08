<%--
=========================================================================================================
  Module      : 注文完了画面(OrderComplete.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyRecommend" Src="~/Form/Common/BodyRecommendAtOrderComplete.ascx" %>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register TagPrefix="uc" TagName="AffiliateTag" Src="~/Form/Common/AffiliateTag.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderComplete.aspx.cs" Inherits="Form_Order_OrderComplete" Title="注文完了ページ" %>
<%@ Import Namespace="w2.Common.Util" %>
<%@ Register TagPrefix="uc" TagName="BodyOrderConfirmRecommend" Src="~/Form/Common/BodyOrderConfirmRecommend.ascx" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ContentPlaceHolderID="AffiliateTagHead" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagHead"
					Location="head"
					Datas="<%# this.OrderList %>"
					IsAlreadyDisplayed="<%# this.IsAleadyDisplayed %>"
					runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyTop" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyTop"
					Location="body_top"
					Datas="<%# this.OrderList %>"
					IsAlreadyDisplayed="<%# this.IsAleadyDisplayed %>"
					runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyBottom" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyBottom"
					Location="body_bottom"
					Datas="<%# this.OrderList %>"
					IsAlreadyDisplayed="<%# this.IsAleadyDisplayed %>"
					runat="server"/>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table id="tblLayout">
<tr>
<td>
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="CartList">
	
<p id="CartFlow">
	<% if (Constants.CART_LIST_LP_OPTION == false) { %>
	<img src="../../Contents/ImagesPkg/order/cart_step04.gif" alt="注文完了" width="781" height="58" />
	<% } else { %>
	<img src="../../Contents/ImagesPkg/order/cart_lp_step02.gif" alt="注文完了" width="781" height="58" />
	<% } %>
</p>
<div class="cartstep">
	<h2 class="ttlC">注文完了</h2>
</div>

<div class="orderComplete">
注文内容を記載したEメールをお送りしました。届かない場合など御座いましたらご連絡ください。<br />
今後とも当店のご利用を心よりお待ち申し上げております。<br />
<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
	<div id="hGmoInreviewContent" runat="server"></div>
<% } %>
<%if (this.IsLoggedIn == false) { %>
	<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
	<br />
	今ご登録頂けると、<%= WebSanitizer.HtmlEncode(GetNumeric(PointOptionUtility.GetAddPoint((List<CartObject>)Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_CART_LIST])))%>ポイントが加算されます。<br />
	<%} %>
	<br />
	<div align="center">
		<a href="<%= WebSanitizer.UrlAttrHtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION) %>" class="btn btn-large btn-inverse">
			会員登録を行う</a>
	</div>
<%} %>

<%-- 外部決済キャンセルは注文確認画面へ戻る --%>
<% if ((this.CartList.Items.Count != 0) && (rErrorMesseges.Visible == false)) { %>
	<br />
	<div class="error_inline" align="center">
	※注文が完了していないカートがあります。続けて注文を行う場合はこちら。<br />
	<% if (this.CartList.Items.Count(item => item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY) != 0) { %>
		Paidyのご利用は承認されませんでした。恐れ入りますが他のお支払方法をご選択ください。<br />
	<%} %>
	<asp:LinkButton runat="server" OnClick="lbRetryOrder_Click">続けて注文を行う</asp:LinkButton><br />
	</div>
<%} %>

<%-- ▼エラー情報▼ --%>
<asp:Repeater ID="rErrorMesseges" runat="server">
<HeaderTemplate>
<span class="error_inline">
<br />
※一部の注文にエラーが発生致しました。<br />
</HeaderTemplate>
<ItemTemplate>
	<%# WebSanitizer.HtmlEncode(Container.DataItem) %><br />
</ItemTemplate>
<FooterTemplate>
</span>
</FooterTemplate>
</asp:Repeater>
<asp:LinkButton ID="lbRetryOrder" runat="server" OnClick="lbRetryOrder_Click">失敗した注文をやり直す<br /></asp:LinkButton>
<%-- ▲エラー情報▲ --%>
</div>

<%-- ▼ドコモケータイ払い用決済情報▼ --%>
<asp:PlaceHolder ID="pfDocomoPayment" runat="server">
<div class="orderComplete">
<div class="background">
<div class="bottom">
<h3></h3>
<div class="orderDetail2">
<div class="suborderDetail">
<img src="../../Contents/ImagesPkg/order/h2_docomo_payment_mail.gif" alt="ドコモケータイ払い" /><br /><br />
<%-- ドコモケータイ払い注文（目立たせたいため完了情報よりも上に配置） --%>
<p>
	ドコモケータイ払いの決済は、携帯電話で行っていただく必要があります。<br />
	決済を行う携帯電話のメールアドレスを入力し、送信ボタンを押してください。<br />
	決済処理は、メールに記載されている内容にしたがって進めてください。<br />
	ドメイン指定受信を設定されている方は、必ず「<%= WebSanitizer.HtmlEncode(ShopMessage.GetMessage("ShopMailDomain")) %>」を受信できるように設定してください。<br />
	<asp:TextBox ID="tbMobileMailAddr" MaxLength="240" Width="150" runat="server"></asp:TextBox>@docomo.ne.jp
	&nbsp;&nbsp;<asp:Button ID="bSendDocomoPaymentMail" runat="server" OnClick="bSendDocomoPaymentMail_Click" Text="  送信  " /><br />
	<strong><%= this.DocomoPaymentErrorMessage %></strong>
</p>
</div>
</div>
</div>
</div>
</div>
</asp:PlaceHolder>
<%-- ▲ドコモケータイ払い用決済情報▲ --%>

	<%-- DSK後払いで与信がHOLDの場合 --%>
	<div runat="server" Visible="<%# this.IsDskDeferredAuthResultHold %>" class="orderComplete">
		<p>
			DSK後払いをご利用いただきありがとうございます。現在は与信審査中ですのでしばらくお待ちください。<br>
			審査結果によってはご利用いただけない場合がございます。<br>
			その場合には別の決済方法に変更させていただく場合がございます。
		</p>
	</div>
	<%-- スコア後払いで与信がHOLDの場合 --%>
	<div runat="server" Visible="<%# this.IsScoreDeferredAuthResultHold %>" class="orderComplete">
		<p>
			<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SCORE_CVSDEFAUTH_ERROR) %>
		</p>
	</div>
	<%-- ベリトランス後払いで与信がHOLDの場合 --%>
	<div runat="server" Visible="<%# this.IsVeritransDeferredAuthResultHold %>" class="orderComplete">
		<p>
			<%: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_VERITRANS_CVSDEFAUTH_ERROR) %>
		</p>
	</div>
	<%-- GMOアトカラの与信結果が審査中の場合 --%>
	<div runat="server" Visible="<%# this.IsGmoAtokaraAuthResultHold %>" class="orderComplete">
		<p>
			アトカラをご利用いただきありがとうございます。現在は与信審査中ですのでしばらくお待ちください。
		</p>
	</div>
<% if(Constants.RECOMMEND_OPTION_ENABLED){ %>
<uc:BodyOrderConfirmRecommend runat="server" OrderList="<%# this.OrderList %>" />
<% } %>
<uc:BodyRecommend runat="server" OrderList="<%# this.OrderList %>" />
	<asp:Repeater ID="rOrderHistory" runat="server">
<ItemTemplate>
	<div class="orderComplete">
	<div class="background">
	<div class="bottom">
	<h3>ご注文明細 <%# WebSanitizer.HtmlEncode(((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_GIFT_FLG) == Constants.FLG_ORDER_GIFT_FLG_ON) ? "(ギフト)" : "") %>
		<%# WebSanitizer.HtmlEncode(((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG) == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON) ? "(デジタルコンテンツ)" : "") %>
	</h3>
	
	<%-- ▼注文情報▼ --%>
	<div class="orderDetail">
	<div class="suborderDetail">
	<%-- ▽基本情報１▽ --%>
	<div class="left">
	<em>注文番号：&nbsp;<%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_ID)) %></em>
	<div>
	<dl>
	<dt>注文日時：</dt>
	<dd><%#: DateTimeUtility.ToStringFromRegion(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_DATE), DateTimeUtility.FormatType.LongDateHourMinute1Letter) %></dd>
	<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
		<dt>オプション価格：</dt>
		<dd><%#: CurrencyManager.ToPrice(GetTotalOptionPrice((DataView)Container.DataItem)) %></dd>
	<% } %>
	<dt>合計金額：</dt>
	<dd><%#: CurrencyManager.ToPrice(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_TOTAL)) %></dd>
	<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
	<dt>決済金額：</dt>
	<dd><%#: GetSettlementAmount(((IList)Container.DataItem)[0]) %></dd>
	<div runat="server" visible="<%# GetOrderShippingVisible(((DataView)Container.DataItem)) %>">
	<dt>決済手数料：</dt>
	<dd><%#: CurrencyManager.ToPrice((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE)) %></dd>
	</div>
	<% } %>
	<dt>注文者：</dt>
	<dd><%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDEROWNER_OWNER_NAME1)) %><%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDEROWNER_OWNER_NAME2)) %>&nbsp;様<br />
		<%#: (IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE))) &&
			(string.IsNullOrEmpty((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1) + DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2)) == false)
			? "(" + DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1) + DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2) + " さま)"
			: "" %>
	</dd>
	<dt>お支払方法：</dt>
	<dd><%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], "order_payment_kbn_name")) %><%# (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE) != "" ? "(" + WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, (DataBinder.Eval(((IList)Container.DataItem)[0], "order_payment_kbn") == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY) ? OrderCommon.CreditInstallmentsValueTextFieldName : Constants.FIELD_CREDIT_INSTALLMENTS_NEWEBPAY, DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE))) + ")" : "" %></dd>
	<span runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo(StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE))) %>">
		<span runat="server" visible="<%# GetOrderInvoiceVisible(StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_ORDER_ID))) %>">
			<dt>発票番号：</dt>
			<dd><%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO) %><br /></dd>
		</span>
		<dt>
			発票種類：
			<div runat="server" visible="<%# StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE)) == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY %>">
				<%# ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@") %>：<br />
				<%# ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@") %>：
			</div>
			<div runat="server" visible="<%# StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE)) == Constants.FLG_TW_UNIFORM_INVOICE_DONATE %>">
				<%# ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@") %>：
			</div>
		</dt>
		<dd>
			<%# GetInformationUniformInvoiceInvoice(
				StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE)),
				StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1)),
				StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2))) %>
		</dd>
		<span runat="server" visible="<%# StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE)) == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL%>">
			<dt>
				共通性載具：
				<div visible="<%# string.IsNullOrEmpty(StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE))) == false %>" runat="server">
					載具コード：
				</div>
			</dt>
			<dd><%# GetInformationCarryTypeInvoice(StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE)), StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION))) %></dd>
		</span>
	</span>

	<dd class="paymentinfo">
	<%-- コンビニ決済用 --%>
	<span visible='<%# NeedToShowPaymentMessageHtml((string)GetKeyValue(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PAYMENT_KBN)) %>' runat="server">
		<%# GetKeyValue(this.OrderInfos[Container.ItemIndex], Constants.PAYMENT_MESSAGE_HTML) %>
	</span>
	</dd>
	</dl>
	<div visible='<%# ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_GIFT_FLG) == Constants.FLG_ORDER_GIFT_FLG_ON) == false %>' class="right" runat="server">
	<div visible='<%# (DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_MEMO).ToString().Trim() != "")  %>' class="right" runat="server">
	<em>注文メモ</em>
	<p><%# WebSanitizer.HtmlEncodeChangeToBr(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_MEMO)) %></p>
	</div>
	</div>
		<asp:Repeater ID="rOrderExtendDisplay"
					ItemType="OrderExtendItemInput"
					DataSource="<%# GetOrderExtendItemInput((DataView)Container.DataItem) %>"
					runat="server"
					Visible="<%# IsDisplayOrderExtend() %>" >
			<HeaderTemplate>
				<div class="left">
				<em>注文確認事項</em>
			</HeaderTemplate>
			<ItemTemplate>
				<div style="display: inline-block">
					<dt><%#: Item.SettingModel.SettingName %>：</dt>
					<dd><%#: (string.IsNullOrEmpty(Item.InputValue)) ? "指定なし" : Item.InputText %></dd>
				</div>
			</ItemTemplate>
			<FooterTemplate>
				</div>
			</FooterTemplate>
		</asp:Repeater>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	</div><!--left-->
	<%-- △基本情報１△ --%>
	<%-- ▽基本情報２（ギフトオフ かつ デジタルコンテンツでない）▽ --%>
	<div visible='<%# (((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_GIFT_FLG) == Constants.FLG_ORDER_GIFT_FLG_ON) == false) && ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG) == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_OFF) %>' class="right" runat="server">
	<em>配送先情報</em>
	<div>
	<div runat="server" visible="<%# (GetOrderShippingVisible(((DataView)Container.DataItem)) == false)
		&& string.IsNullOrEmpty(WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID))) %>">
	<dl>
	<dt><%: ReplaceTag("@@User.addr.name@@") %>：</dt>
	<dd>
		<p>
			<%# IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE))
				? "〒" + WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)) + "<br />"
				: "" %>
			<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1) %>
			<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2) %><br />
			<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3) %>
			<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>
			<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %><br />
			<%# (IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE)) == false)
				? WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)) + "<br />"
				: "" %>
			<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME) %>
		</p>
	</dd>
	<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
	<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
		<%: ReplaceTag("@@User.company_post_name.name@@")%>：</dt>
	<dd><%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME)) %>&nbsp
		<%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME)) %></dd>
	<%} %>
	<%-- 氏名 --%>
	<dt><%: ReplaceTag("@@User.name.name@@") %>：</dt>
	<dd><%# WebSanitizer.HtmlEncodeChangeToBr(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1)) %><%# WebSanitizer.HtmlEncodeChangeToBr(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2)) %>&nbsp;様<br />
		<%#: (IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE))) &&
				(string.IsNullOrEmpty((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) + DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2)) == false)
			? "(" + DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) + DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2) + " さま)"
			: "" %>
	</dd>
	<%-- 電話番号 --%>
	<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
	<dd><%# WebSanitizer.HtmlEncodeChangeToBr(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1)) %></dd>
	</div>
	<div runat="server" visible="<%# (string.IsNullOrEmpty(WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID))) == false) %>">
		<dt>
			受取店舗：
		</dt>
		<dd>
			<p><%#: GetRealShopName(WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID))) %></p>
		</dd>
		<dt>
			受取店舗住所：
		</dt>
		<dd>
			<p>
				<%#: "〒" + DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP) %>
				<br />
				<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1) %>
				<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2) %>
				<br />
				<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3) %>
				<br />
				<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %>
				<br />
				<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5) %>
			</p>
		</dd>
		<dt>
			営業時間：
		</dt>
		<dd>
			<p><%#: GetRealShopOpeningHours(WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID))) %></p>
		</dd>
		<dt>
			店舗電話番号：
		</dt>
		<dd>
			<p><%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1) %></p>
		</dd>
	</div>
	<div runat="server" visible="<%# (GetOrderShippingVisible(((DataView)Container.DataItem)) == false)
		&& string.IsNullOrEmpty(WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID))) %>">
	<dt visible='<%# (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG) == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID %>' runat="server">
		配送希望時間帯：</dt>
	<dd visible='<%# (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG) == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID %>' runat="server">
		<%# WebSanitizer.HtmlEncode((w2.Common.Util.Validator.IsNullEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], "shipping_time_message")) == false)
			? DataBinder.Eval(((IList)Container.DataItem)[0], "shipping_time_message")
			: ReplaceTag("@@DispText.shipping_time_list.none@@")) %></dd>
	</dl>
	</div>
	<div runat="server" visible="<%# GetOrderShippingVisible(((DataView)Container.DataItem)) %>">
		<dt>店舗ID：</dt>
		<dd><%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID) %></dd>
		<dt>店舗名称：</dt>
		<dd><%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %></dd>
		<dt>店舗住所：</dt>
		<dd><%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4) %></dd>
		<dt>店舗電話番号：</dt>
		<dd>
			<%#: ((w2.Common.Util.Validator.IsNullEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1)) == false)
				? DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1)
				: ReplaceTag("@@DispText.shipping_convenience_store.none@@")) %>
		</dd>
	</div>
	<div runat="server" visible="<%# string.IsNullOrEmpty(WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID))) %>">
	<dt>配送方法：</dt>
	<dd>
		<%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD))) %>
	</dd>
	<dt visible='<%# (((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG) == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID)
		&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)
		&& (StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)) %>' runat="server">
		配送希望日：</dt>
	<dd visible='<%# (((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG) == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID)
		&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)
		&& (StringUtility.ToEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)) == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)) %>' runat="server">
		<%#: (w2.Common.Util.Validator.IsNullEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE)) == false)
				? DateTimeUtility.ToStringFromRegion(((DateTime)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE)), DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)
				: ReplaceTag("@@DispText.shipping_date_list.none@@") %></dd>
	</div>
	<em visible="<%# m_lFixedPurchaseFlgs[Container.ItemIndex] %>" runat="server">定期配送情報</em>
	<dl visible="<%# m_lFixedPurchaseFlgs[Container.ItemIndex] %>" runat="server">
        <dt visible='<%# IsShippingPatternChanged(Container.ItemIndex) %>' runat="server">初回配送パターン：</dt>
        <dd visible='<%# IsShippingPatternChanged(Container.ItemIndex) %>' runat="server"><%#: Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED ? WebSanitizer.HtmlEncode(m_lFixedPurchaseFirstPatternStrings[Container.ItemIndex]) : string.Empty%></dd>
        <dt visible='<%# IsShippingPatternChanged(Container.ItemIndex) %>' runat="server">2回目以降の配送パターン：</dt>
        <dt visible='<%# IsShippingPatternChanged(Container.ItemIndex) == false %>' runat="server">配送パターン：</dt>
		<dd><%# WebSanitizer.HtmlEncode(m_lFixedPurchasePatternStrings[Container.ItemIndex])%></dd>
        <dt>初回配送予定：</dt>
		<dd><%#: DateTimeUtility.ToStringFromRegion(m_lFixedPurchaseFirstShippingDates[Container.ItemIndex], DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)%></dd>
		<dt>今後の配送予定：</dt>
		<dd><%#: DateTimeUtility.ToStringFromRegion(m_lFixedPurchaseNextShippingDates[Container.ItemIndex], DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)%></dd>
		<dt></dt>
		<dd><%# DateTimeUtility.ToStringFromRegion(m_lFixedPurchaseNextNextShippingDates[Container.ItemIndex], DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)%></dd>
		<div runat="server" visible="<%# GetOrderShippingVisible(((DataView)Container.DataItem)) == false %>">
			<dt visible='<%# m_lShippingTimeSetFlgs[Container.ItemIndex] %>' runat="server">配送希望時間帯：</dt>
			<dd visible='<%# m_lShippingTimeSetFlgs[Container.ItemIndex] %>' runat="server"><%# WebSanitizer.HtmlEncode(m_lShippingTimeMessages[Container.ItemIndex]) %></dd>
		</div>
	</dl>
	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	</div>
	</div><!--right-->
	<%-- △基本情報２（ギフトオフ）△ --%>
	<%-- ▽基本情報２（ギフトオン）▽ --%>
	<div visible='<%# ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_GIFT_FLG) == Constants.FLG_ORDER_GIFT_FLG_ON) %>' class="right" runat="server">
	<div visible='<%# (DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_MEMO).ToString().Trim() != "")  %>' class="right" runat="server">
	<em>注文メモ</em>
	<p><%# WebSanitizer.HtmlEncodeChangeToBr(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_MEMO)) %></p>
	</div>
	</div>
	<br class="clr" />
	</div><!--suborderDetail-->
	</div><!--orderDetail-->
	<%-- ▲注文情報▲ --%>

	<%-- ▼注文内容▼ --%>
	<%-- ▽商品一覧（ギフトオフ）▽ --%>
	<div visible='<%# (((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_GIFT_FLG) == Constants.FLG_ORDER_GIFT_FLG_ON) == false) %>' runat="server">
	<h4>注文内容</h4>
	<div class="productList">
	<asp:Repeater DataSource="<%# Container.DataItem %>" runat="server">
	<ItemTemplate>
		<%-- 通常商品 --%>
		<div class="product" visible='<%# ((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_SET_ID) == "") && (StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO)) == "") %>' runat="server">
		<div>
		<dl class="name">
		<dt>
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), "", "", "", (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME), "")) %>' runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
				<w2c:ProductImage ProductMaster="<%# GetProduct((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID)) %>" ImageSize="M" IsVariation="true" runat="server" />
			</a>
			<w2c:ProductImage ProductMaster="<%# GetProduct((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID)) %>" ImageSize="M" IsVariation="true" runat="server"  Visible="<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) %>" />
		</dt>
		<dd>
			<span>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), "", "", "", (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME), "")) %>' runat="server"  Visible='<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>'>
				<%#: GetOrderItemProductTranslationName(Container.DataItem) %>
				</a>
				<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) ? WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) : ""%>
			</span>
		</dd>
		<dd visible='<%# (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS) != ""%>'>
			<%# WebSanitizer.HtmlEncode(ProductOptionSettingHelper.GetDisplayProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))).Replace("　", "<br />")%>
		</dd>
		<dd >
			<small>
				<asp:Repeater DataSource='<%# this.OrderItemSerialKeys[((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) + (Eval(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO).ToString())] %>' runat="server">
				<ItemTemplate>
					<br />
					&nbsp;シリアルキー:&nbsp;<%# Eval(Constants.FIELD_SERIALKEY_SERIAL_KEY)%>
				</ItemTemplate>
				</asp:Repeater>
			</small>
		</dd>
		</dl>
		<p class="subscriptionbox" Visible="<%# string.IsNullOrEmpty(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT))) == false %>" runat="server"><%#: GetSubscriptionBoxDisplayName(Eval(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID).ToString()) %></p>
		<p class="quantity" Visible="<%# string.IsNullOrEmpty(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT))) == false %>" runat="server"><%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY))) %>個</p>
		<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
			<p class="quantityOne" Visible="<%# string.IsNullOrEmpty(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT))) %>" runat="server">
				<%#: (string.IsNullOrEmpty((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))
						|| (ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)) == 0))
						? CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE))
						: ProductOptionSettingHelper.ToDisplayProductOptionPrice(ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)), (decimal)Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) %>
				x <%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY))) %>
			</p>
		<% } else { %>
			<p class="quantityOne" Visible="<%# string.IsNullOrEmpty(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT))) %>" runat="server"><%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) %> x <%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY))) %></p>
		<% } %>
			<p class="subtotal" Visible="<%# string.IsNullOrEmpty(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT))) %>" runat="server">&nbsp;<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_ITEM_PRICE)) %>(<%#: this.ProductPriceTextPrefix %>)</p>
		</div>
		<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div>
		<%-- セットプロモーション商品 --%>
		<div class="product" visible='<%# (StringUtility.ToEmpty(Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO)) != "") && ((int)Eval(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO) == 1) %>' runat="server">
		<div>
		<asp:Repeater DataSource="<%# GetOrderSetPromotionItemList((DataRowView)Container.DataItem) %>" runat="server">
		<HeaderTemplate>
			<table cellpadding="0" cellspacing="0" summary="ショッピングカート" width="100%">
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="name">
					<dl>
						<dt>
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), "", "", "", (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME), "")) %>' runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
								<w2c:ProductImage ProductMaster="<%# GetProduct((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID)) %>" ImageSize="M" IsVariation="true" runat="server" />
							</a>
							<w2c:ProductImage ProductMaster="<%# GetProduct((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID)) %>" ImageSize="M" IsVariation="true" runat="server" Visible="<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) %>" />
						</dt>
						<dd>
							<span>
								<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), "", "", "", (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME), "")) %>' runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
									<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) %>
								</a>
								<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) ? WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) : ""%>
							</span>
						</dd>
						<dd visible='<%# (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS) != ""%>'>
							<%# WebSanitizer.HtmlEncode(ProductOptionSettingHelper.GetDisplayProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))).Replace("　", "<br />")%>
						</dd>
						<dd >
							<small>
								<asp:Repeater DataSource='<%# this.OrderItemSerialKeys[((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) + (Eval(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO).ToString())] %>' runat="server">
								<ItemTemplate>
									<br />
									&nbsp;シリアルキー:&nbsp;<%# Eval(Constants.FIELD_SERIALKEY_SERIAL_KEY)%>
								</ItemTemplate>
								</asp:Repeater>
							</small>
						</dd>
					</dl>
				</td>
				<td width="200" Visible="<%# string.IsNullOrEmpty(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT))) == false %>" runat="server">
					<%#: StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY)) %>個
				</td>
				<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED){ %>
						<td width="200" class="quantityOne">
							<%#: (string.IsNullOrEmpty((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))
								|| (ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)) == 0))
								     ? CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE))
								     : ProductOptionSettingHelper.ToDisplayProductOptionPrice(ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)), (decimal)Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) %>
							x <%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY))) %>
						</td>
				<% } else { %>
					<td width="200" Visible="<%# string.IsNullOrEmpty(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT))) %>" runat="server">
						<%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) %> x <%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY))) %>
					</td>
				<% } %>
				<td class="subtotal" width="299" rowspan="<%# ((IList)((Repeater)Container.Parent).DataSource).Count %>" Visible="<%# (Container.ItemIndex == 0) && (string.IsNullOrEmpty(StringUtility.ToEmpty(Eval(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT)))) %>" runat="server">
					<%#: GetOrderSetPromotionDispNameTranslationName(Container.DataItem) %><br />
					<span visible="<%# (decimal)Eval(Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT) != 0 %>" runat="server">
					<strike><%#: CurrencyManager.ToPrice((decimal)Eval(Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL)) %>(税込)</strike><br />
					</span>
					<%#: CurrencyManager.ToPrice((decimal)Eval(Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL) - (decimal)Eval(Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT)) %>(税込)
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
		</asp:Repeater>
		</div>
		</div>
		<%-- セット商品 --%>
		<div class="product" visible='<%# CheckTopSetItem(Container.DataItem) %>' runat="server">
		<div>
		<asp:Repeater DataSource='<%# GetSetItemList(Container.DataItem) %>' runat="server">
		<HeaderTemplate>
			<table cellpadding="0" cellspacing="0" summary="ショッピングカート" width="100%">
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
			<td width="286" class="name">
			<dl>
			<dt>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), "", "", "", (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME), "")) %>' runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
					<w2c:ProductImage ProductMaster="<%# GetProduct((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID)) %>" ImageSize="M" IsVariation="true" runat="server" />
				</a>
				<w2c:ProductImage ProductMaster="<%# GetProduct((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID)) %>" ImageSize="M" IsVariation="true" runat="server" Visible="<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) %>" />
			</dt>
			<dd>
				<span>
					<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), "", "", "", (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME), "")) %>' runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
						<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) %>
					</a>
					<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) ? WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) : "" %>
				</span>
			</dd>
			</dl></td>
			<td width="200"><%#: CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)) %> x <%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE))) %></td>
			<td width="120" rowspan="<%# ((IList)((Repeater)Container.Parent).DataSource).Count %>" visible="<%# CheckTopSetItem(Container.DataItem) %>" class="quantity" runat="server">数量&nbsp;&nbsp;&nbsp;<%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT))) %></td>
			<td width="179" rowspan="<%# ((IList)((Repeater)Container.Parent).DataSource).Count %>" visible="<%# CheckTopSetItem(Container.DataItem) %>" class="subtotal" runat="server"><%#: CurrencyManager.ToPrice(StringUtility.ToNumeric(GetProductSetPrice(Container.DataItem))) %> (<%#: this.ProductPriceTextPrefix %>)</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
		</asp:Repeater>
		</div>
		<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div>
	</ItemTemplate>
	</asp:Repeater>
	</div>
	</div>
	<%-- △商品一覧（ギフトオフ）△ --%>

	<%-- ▽配送先・商品一覧（ギフトオン）▽  --%>
	<div visible='<%# ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_GIFT_FLG) == Constants.FLG_ORDER_GIFT_FLG_ON) %>' runat="server">
	<asp:Repeater ID="rCartShippings" DataSource="<%# CreteShippingsAndItems(Container.DataItem)%>" runat="server">
	<ItemTemplate>	
		<h4>配送情報 <%# IsGift(((RepeaterItem)GetOuterControl(Container, typeof(RepeaterItem))).DataItem) ? (Container.ItemIndex + 1).ToString() : ""%></h4>
		<div class="productList">
		<%-- ▽商品情報▽ --%>
		<asp:Repeater ID="rCartProducts" DataSource="<%# Container.DataItem %>" runat="server">
		<ItemTemplate>
		<div class="product2" visible='<%# (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_SET_ID) == "" %>' runat="server">
		<div>
		<dl class="name">
		<dt>
			<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), "", "", "", (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME), "")) %>' runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
				<w2c:ProductImage ID="ProductImage1" ProductMaster="<%# GetProduct((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID)) %>" ImageSize="S" IsVariation="true" runat="server" />
			</a>
			<w2c:ProductImage ProductMaster="<%# GetProduct((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID)) %>" ImageSize="M" IsVariation="true" runat="server" Visible="<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) %>" />
		</dt>
		<dd>
			<span>
				<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl((string)Eval(Constants.FIELD_ORDERITEM_SHOP_ID), "", "", "", (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID), (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID), (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME), "")) %>' runat="server" Visible="<%# IsProductDetailLinkValid((DataRowView)Container.DataItem) %>">
					<%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) %>
				</a>
				<%# (IsProductDetailLinkValid((DataRowView)Container.DataItem) == false) ? WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDERITEM_PRODUCT_NAME)) : ""%>
			</span>
		</dd>
		<dd visible='<%# (string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS) != ""%>'>
			<%# WebSanitizer.HtmlEncode(ProductOptionSettingHelper.GetDisplayProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))).Replace("　", "<br />")%>
		</dd>
		<dd >
			<small>
				<asp:Repeater DataSource='<%# this.OrderItemSerialKeys[((string)Eval(Constants.FIELD_ORDER_ORDER_ID)) + (Eval(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO).ToString())] %>' runat="server">
				<ItemTemplate>
					<br />
					&nbsp;シリアルキー:&nbsp;<%# Eval(Constants.FIELD_SERIALKEY_SERIAL_KEY)%>
				</ItemTemplate>
				</asp:Repeater>
			</small>
		</dd>
		</dl>
		<p class="quantity">
			<%#: (string.IsNullOrEmpty((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))
				|| (ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)) == 0))
					? CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE))
					: string.Format("({0}+{1})", CurrencyManager.ToPrice(Eval(Constants.FIELD_ORDERITEM_PRODUCT_PRICE)), ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS)))%>
			x <%# WebSanitizer.HtmlEncode(StringUtility.ToNumeric(Eval(Constants.FIELD_ORDERITEM_ITEM_QUANTITY))) %>
		</p>
		</div>
		<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
		</div>
		</ItemTemplate>
		</asp:Repeater>
		<%-- △商品情報△ --%>
		</div>

		<%-- ▽（デジタルコンテンツでない）▽  --%>
		<div class="orderDetail" visible='<%# ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG) == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_OFF) %>' runat="server">
		<%-- ▽送り主▽ --%>
		<div class="box">
		<em>
		送り主
		</em>
		<dl>
		<%-- 氏名 --%>
		<dt><%: ReplaceTag("@@User.name.name@@") %>：</dt>

		<dd><%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_NAME1))%><%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_NAME2))%>&nbsp;様<br />
		<%#: IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE))
			? "(" + DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1) + DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2) + " さま)"
			: "" %>
			<br />
		</dd>
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>：
		</dt>
		<dd>
		<%#: IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE))
			? DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_ZIP)
			: ""%>
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1)%> <%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2)%><br />
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3)%> <%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4)%><br />
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5)%>
		<%#: (IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE)) == false)
			? DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_ZIP)
			: "" %><br />
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME)%>
		</dd>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<dt><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%>：</dt>
		<dd>
		<%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME)) %><br />
		<%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME)) %>
		</dd>
		<%} %>
		<%-- 電話番号 --%>
		<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
		<dd>
		<%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SENDER_TEL1))%>
		</dd>
		</dl>
		</div>
		<%-- △送り主△ --%>

		<%-- ▽配送先▽ --%>
		<div class="box">
		<em>配送先</em>
		<dl>
		<%-- 氏名 --%>
		<dt><%: ReplaceTag("@@User.name.name@@") %>：</dt>
		<dd>
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1) %> <%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2) %>&nbsp;様
		<%#: IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE)) ? (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1) + (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2) + " さま" : "" %>
		<br />
		</dd>
		<dt>
			<%: ReplaceTag("@@User.addr.name@@") %>：
		</dt>
		<dd>
		<%#: IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE))
			? DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)
			: ""%>
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)%><br />
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1)%> <%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2)%><br />
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3)%> <%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4)%><br />
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5)%>
		<%#: (IsCountryJp((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE)) == false)
			? DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP)
			: ""%><br />
		<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME)%><br />
		</dd>
		<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
		<dt><%: ReplaceTag("@@User.company_name.name@@")%>・<%: ReplaceTag("@@User.company_post_name.name@@")%>：</dt>
		<dd>
		<%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME))%><br />
		<%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME))%>
		</dd>
		<%} %>
		<%-- 電話番号 --%>
		<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
		<dd>
		<%# WebSanitizer.HtmlEncode(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1))%>
		</dd>
		</div>
		<%-- △配送先△ --%>

		<%-- ▽配送希望▽ --%>
		<div class="box" visible="<%# ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG) == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID) || ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG) == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID) %>" runat="server">
		<em>配送希望</em>
		<dl>
		<dt visible='<%# (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG) == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID %>' runat="server">
			配送希望日：</dt>
		<dd id="Dd1" visible='<%# (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG) == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID %>' runat="server">
			<%#: (w2.Common.Util.Validator.IsNullEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE)) == false)
					? DateTimeUtility.ToStringFromRegion(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE), DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)
					: ReplaceTag("@@DispText.shipping_date_list.none@@") %></dd>
		<dt visible='<%# (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG) == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID %>' runat="server">
			配送希望時間帯：</dt>
		<dd visible='<%# (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG) == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID %>' runat="server">
			<%# WebSanitizer.HtmlEncode(
				(w2.Common.Util.Validator.IsNullEmpty(DataBinder.Eval(((IList)Container.DataItem)[0], "shipping_time_message")) == false)
					? DataBinder.Eval(((IList)Container.DataItem)[0], "shipping_time_message") : ReplaceTag("@@DispText.shipping_time_list.none@@")) %></dd>
		</dl>
		</div>
		<%-- △配送希望△ --%>

		<%-- ▽定期購入情報▽ --%>
		<div class="box" visible="<%# m_lFixedPurchaseFlgs[((RepeaterItem)Container.Parent.Parent.Parent).ItemIndex] %>" runat="server">
		<em>定期配送情報</em>
		<dl>
			<dt>配送パターン：</dt>
			<dd><%# WebSanitizer.HtmlEncode(m_lFixedPurchasePatternStrings[((RepeaterItem)Container.Parent.Parent.Parent).ItemIndex])%></dd>
			<dt>初回配送予定：</dt>
			<dd><%#: DateTimeUtility.ToStringFromRegion(m_lFixedPurchaseFirstShippingDates[((RepeaterItem)Container.Parent.Parent.Parent).ItemIndex], DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)%></dd>
			<dt>今後の配送予定：</dt>
			<dd><%#: DateTimeUtility.ToStringFromRegion(m_lFixedPurchaseNextShippingDates[((RepeaterItem)Container.Parent.Parent.Parent).ItemIndex], DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)%></dd>
			<dt></dt>
			<dd><%#: DateTimeUtility.ToStringFromRegion(m_lFixedPurchaseNextNextShippingDates[((RepeaterItem)Container.Parent.Parent.Parent).ItemIndex], DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)%></dd>
			<dt visible='<%# m_lShippingTimeSetFlgs[((RepeaterItem)Container.Parent.Parent.Parent).ItemIndex] %>' runat="server">配送希望時間帯：</dt>
			<dd visible='<%# m_lShippingTimeSetFlgs[((RepeaterItem)Container.Parent.Parent.Parent).ItemIndex] %>' runat="server"><%# WebSanitizer.HtmlEncode(m_lShippingTimeMessages[((RepeaterItem)Container.Parent.Parent.Parent).ItemIndex])%></dd>
		</dl>
		</div>
		<%-- △定期購入情報△ --%>

		<%-- ▽のし・包装情報▽ --%>
		<div class="box" visible='<%# ((string)this.OrderList[((RepeaterItem)GetOuterControl(Container, typeof(RepeaterItem))).ItemIndex][Container.ItemIndex][Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID) || ((string)this.OrderList[((RepeaterItem)GetOuterControl(Container, typeof(RepeaterItem))).ItemIndex][Container.ItemIndex][Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID) %>' runat="server">
		<em>のし・包装情報</em>
		<span visible='<%# (string)this.OrderList[((RepeaterItem)GetOuterControl(Container, typeof(RepeaterItem))).ItemIndex][Container.ItemIndex][Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID %>' runat="server">
		<dt>のし種類：</dt>
		<dd>
			<%# WebSanitizer.HtmlEncode(((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE) == "") ? "なし" : (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE))%>
		</dd>
		<dt>のし差出人：</dt>
		<dd>
			<%# WebSanitizer.HtmlEncode(((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME) == "") ? "なし" : (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME))%>
		</dd>
		</span>
		<span visible='<%# (string)this.OrderList[((RepeaterItem)GetOuterControl(Container, typeof(RepeaterItem))).ItemIndex][Container.ItemIndex][Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID %>' runat="server">
		<dt>包装種類：</dt>
		<dd>
			<%# WebSanitizer.HtmlEncode(((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE) == "") ? "なし" : (string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE))%>
		</dd>
		</span>
		</div>
		<%-- △のし・包装情報△ --%>

		</div>
		<%-- △（デジタルコンテンツでない）△  --%>

	</ItemTemplate>
	</asp:Repeater>
	</div>
	<%-- △配送先・商品一覧（ギフトオン）△  --%>

	<div class="productList">
	<div class="cartOrder">
	<div class="subcartOrder">
	<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
	<div class="sumBox">
	<dl>
	<dt>獲得ポイント</dt>
	<dd><%# WebSanitizer.HtmlEncode(GetNumeric(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_POINT_ADD))) %>pt</dd>
	</dl>
	<small>※ 1pt = <%: CurrencyManager.ToPrice(1m) %></small>
	</div><!--sum-->
	<%} %>
	<div class="priceList">
	<div>
	<dl class="bgc">
	<dt>小計(<%#: this.ProductPriceTextPrefix %>)</dt>
	<dd><%#: CurrencyManager.ToPrice(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL)) %></dd>
	</dl>
	<%if (this.ProductIncludedTaxFlg == false) { %>
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt>消費税</dt>
			<dd><%#: CurrencyManager.ToPrice((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX)) %></dd>
		</dl>
	<%} %>
	<asp:Repeater DataSource='<%# GetOrderSetPromotions((DataView)Container.DataItem) %>' Visible="<%# IsOrderItemsAllSubscriptionBoxFixedAmount(Container.ItemIndex) == false %>" runat="server">
	<ItemTemplate>
		<span visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG] == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON %>" runat="server">
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt><%#: GetOrderSetPromotionDispNameTranslationName(Container.DataItem) %></dt>
			<dd class='<%# ((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT] > 0) ? "minus" : "" %>'><%# ((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT] > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT])) %></dd>
		</dl>
		</span>
	</ItemTemplate>
	</asp:Repeater>
	<%if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn) { %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>会員ランク割引額</dt>
	<dd class='<%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE) > 0) ? "minus" : "" %>'><%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE) > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE)) %></dd>
	</dl>
	<%} %>
	<%if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn) { %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>定期会員割引額</dt>
	<dd class='<%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT) > 0) ? "minus" : "" %>'><%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT) > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT)) %></dd>
	</dl>
	<%} %>
	<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>クーポン割引額</dt>
	<dd class='<%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_COUPON_USE) > 0) ? "minus" : "" %>'>
		<%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_COUPON_USE) > 0)
			? string.Format("({0})", GetCouponName((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_ID)))
			: string.Empty %>
		<%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_COUPON_USE) > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_COUPON_USE)) %>
	</dd>
	</dl>
	<%} %>
	<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>ポイント利用額</dt>
	<dd class='<%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_POINT_USE_YEN) > 0) ? "minus" : "" %>'><%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_POINT_USE_YEN) > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_POINT_USE_YEN)) %></dd>
	</dl>
	<%} %>
	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
	<span runat="server" visible='<%# ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG) == "1") %>'>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>定期購入割引額</dt>
	<dd class='<%# ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE) > 0) ? "minus" : "" %>'><%#: ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE) > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE)) %></dd>
	</dl>
	</span>
	<%} %>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>配送料金</dt>
	<dd runat="server" style='<%# ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG) == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID) ? "display:none;" : ""%>'>
		<%#: CurrencyManager.ToPrice((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING)) %></dd>
	<dd runat="server" style='<%# ((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG) == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID) ? "display:none;" : ""%>'>
		<%# WebSanitizer.HtmlEncode((string)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE)) %></dd>
	</dl>
	<asp:Repeater DataSource='<%# GetOrderSetPromotions((DataView)Container.DataItem) %>' runat="server">
	<ItemTemplate>
		<span visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON %>" runat="server">
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt><%#: GetOrderSetPromotionDispNameTranslationName(Container.DataItem) %>(送料割引)</dt>
			<dd class='<%# ((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT] > 0) ? "minus" : "" %>'><%# ((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT] > 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT])) %></dd>
		</dl>
		</span>
	</ItemTemplate>
	</asp:Repeater>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
	<dt>決済手数料</dt>
	<dd><%#: CurrencyManager.ToPrice((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE)) %></dd>
	</dl>
	<asp:Repeater DataSource='<%# GetOrderSetPromotions((DataView)Container.DataItem) %>' runat="server">
	<ItemTemplate>
		<span visible="<%# (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON %>" runat="server">
		<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>'>
			<dt><%#: GetOrderSetPromotionDispNameTranslationName(Container.DataItem) %>(決済手数料割引)</dt>
			<dd class='<%# ((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT] > 0) ? "minus" : "" %>'><%# ((decimal)((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT] > 0) ? "-" : ""%><%#: CurrencyManager.ToPrice(StringUtility.ToNumeric(((Hashtable)Container.DataItem)[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT])) %></dd>
		</dl>
		</span>
	</ItemTemplate>
	</asp:Repeater>
	<dl class='<%= (this.DispNum++ % 2 == 0) ? "" : "bgc" %>' visible="<%# (((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_REGULATION)) != 0) %>" runat="server">
	<dt>調整金額</dt>
		<dd class='<%#: (((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_REGULATION)) < 0) ? "minus" : "" %>'>
			<%#: ((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_REGULATION) < 0) ? "-" : "" %><%#: CurrencyManager.ToPrice(Math.Abs((decimal)DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_REGULATION))) %>
		</dd>
	</dl>
	</div>

	<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
	<div>
	<dl class="result">
	<dt>合計(税込)</dt>
	<dd><%#: CurrencyManager.ToPrice(DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_PRICE_TOTAL)) %></dd>
	<%if (Constants.GLOBAL_OPTION_ENABLE) { %>
	<dt>決済金額：</dt>
	<dd><%#: GetSettlementAmount(((IList)Container.DataItem)[0]) %></dd>
	<% } %>
	</dl>
	</div>
	</div><!--priceList-->
	<br class="clr" />
	</div><!--subcartOrder-->
	</div><!--cartOrder-->
	</div><!--productList-->
	</div><!--bottom-->
	</div><!--background-->
	</div><!--orderComplete-->
	<%-- ▲注文内容▲ --%>

	<%-- ▼獲得ポイント表示▼ --%>
	<div class="orderCompleteSum" visible="<%# (((IList)((Repeater)Container.Parent).DataSource).Count == (Container.ItemIndex + 1)) %>" runat="server">
	<div class="suborderCompleteSum">
	<%if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
	<div class="left">
	<div class="sumBox">
	<dl>
	<dt>総獲得ポイント</dt>
	<dd><%# GetNumeric(GetOrderPointAddTotal((IList)((Repeater)Container.Parent).DataSource)) %> pt</dd>
	</dl>
	<small>※ 1pt = <%: CurrencyManager.ToPrice(1m) %></small>
	</div><!--sum-->
	</div><!--left-->
	<%} %>
	<div class="right">
	<div class="sumBox">
	<div class="subSumBoxB">
	<p><img src="../../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" /><strong><%#: CurrencyManager.ToPrice(GetOrderPriceTotalSummary((IList)((Repeater)Container.Parent).DataSource)) %></strong></p>
	<small class="InternationalShippingAttention" runat="server" visible="<%# IsDisplayProductTaxExcludedMessage(Container.DataItem) %>">※国外配送をご希望の場合関税・商品消費税は料金に含まれず、商品到着後、現地にて税をお支払いいただくこととなりますのでご注意ください。</small>
	</div>
	</div><!--sum-->
	<span class="btn_continue_shopping"><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>" class="btn btn-large btn-inverse"><small>買い物を続ける</small></a></span>
	</div><!--right-->
	<br class="clr" />
	</div><!--suborderCompleteSum-->
	</div><!--orderCompleteSum-->
	<%-- ▲獲得ポイント表示▲ --%>
	<w2c:FacebookConversionAPI
		EventName="Purchase"
		EventId="<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_ID) %>"
		CustomDataOrderId="<%#: DataBinder.Eval(((IList)Container.DataItem)[0], Constants.FIELD_ORDER_ORDER_ID) %>"
		UserId="<%#: this.LoginUserId %>"
		runat="server" />
</ItemTemplate>
</asp:Repeater>
<!-- シルバーエッグ連携時使用 -->
<%--<uc:BodyProductRecommendByRecommendEngine runat="server" RecommendCode="pc514" RecommendTitle="おすすめ商品一覧" MaxDispCount="6" RecommendProductId="<%# GetOrderProductsForSilveregg() %>" RecommendOrderList="<%# this.OrderList %>" DispCategoryId="" NotDispCategoryId="" NotDispRecommendProductId="" />--%>

<%-- ▼GoogleAnalyticsタグ出力▼ --%>
<% if ((Constants.GOOGLEANALYTICS_ENABLED) && (Constants.SETTING_PRODUCTION_ENVIRONMENT)) { %>
<script type="text/javascript" charset="Shift_JIS" src="<%= Constants.PATH_ROOT %>Js/cookie-utils.js"></script>
<asp:Repeater ID="rGoogleAnalytics" runat="server">
<HeaderTemplate>
	<%-- GA4用 --%>
	<script async src="https://www.googletagmanager.com/gtag/js?id=<%= (string.IsNullOrEmpty(Constants.GOOGLEANALYTICS_PROFILE_ID) ? Constants.GOOGLEANALYTICS_MEASUREMENT_ID : Constants.GOOGLEANALYTICS_PROFILE_ID) %>"></script>
	<script type="text/javascript">
		window.dataLayer = window.dataLayer || [];
		function gtag() { dataLayer.push(arguments); }
		gtag('js', new Date());
		<% if (string.IsNullOrEmpty(Constants.GOOGLEANALYTICS_PROFILE_ID) == false) { %>
			gtag('config', '<%= Constants.GOOGLEANALYTICS_PROFILE_ID %>');
		<% } %>
		gtag('config', '<%= Constants.GOOGLEANALYTICS_MEASUREMENT_ID %>');
	</script>
</HeaderTemplate>
<ItemTemplate>
	<script type='text/javascript'>
		<%-- GoogleAnalyticsタグ制御用注文IDが存在する？--%>
		var cookieKey = '<%# Constants.COOKIE_KEY_GOOGLEANALYTICS_ORDER_ID + (string)((Hashtable)Container.DataItem)[Constants.FIELD_ORDER_ORDER_ID] %>';
		if (!getCookie(cookieKey)) {
			<%-- GA4トランザクション計測タグ --%>
			gtag('event', 'purchase', {
				"transaction_id": "<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDER_ORDER_ID] %>",
				"value": '<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL].ToPriceString() %>',
				"currency": "<%#: Constants.CONST_KEY_CURRENCY_CODE %>",
				"shipping": "<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING].ToPriceString() %>",
				"items": [
					<asp:Repeater DataSource='<%# ((Hashtable)Container.DataItem)["order_items"] %>' runat="server">
						<ItemTemplate>
							{
								"item_id": "<%#:((Hashtable)Container.DataItem)[Constants.FIELD_ORDERITEM_VARIATION_ID] %>",
								"item_name": "<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERITEM_PRODUCT_NAME] %>",
								"category": "<%#: GetProductBrandName((Hashtable)Container.DataItem) + GetProductCategoryName((Hashtable)Container.DataItem) %>",
								"quantity": <%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] %>,
								"price": "<%#: ((Hashtable)Container.DataItem)[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToPriceString() %>",
							},
						</ItemTemplate>
					</asp:Repeater>
				]
			});

			setCookie(cookieKey, null);
		}
	
	</script>
</ItemTemplate>
</asp:Repeater>
<%} %>
<%-- ▲GoogleAnalyticsログ出力（UniversalAnalytics版）▲ --%>

<%-- CRITEOタグ（引数：注文情報） --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# this.OrderList %>" />

</div>
<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
<input type="hidden" id="fraudbuster" name="fraudbuster" />
<script type="text/javascript" src="//cdn.credit.gmo-ab.com/psdatacollector.js"></script>
</asp:Content>
