<%--
=========================================================================================================
  Module      : MYページメニュー出力コントローラ(BodyMyPageMenu.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyProductSearchBox" Src="~/SmartPhone/Form/Common/Product/BodyProductSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyMiniCart" Src="~/SmartPhone/Form/Common/BodyMiniCart.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryTree" Src="~/SmartPhone/Form/Common/Product/BodyProductCategoryTree.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyMyPageMenu.ascx.cs" Inherits="Form_Common_BodyMyPageMenu" %>
<%--

下記は保持用のダミー情報です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>

--%>

<%-- ▽編集可能領域：共通ヘッダ領域▽ --%>
<nav>
<h2>マイページメニュー</h2>
<ul class="user-nav">
	<li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_MODIFY_INPUT) %>">会員登録の確認・変更</a></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST) %>">お気に入りリスト</a></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FOLLOW_LIST) %>">フォローリスト</a></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_LIKE_LIST) %>">いいねリスト</a></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_LIST) %>">ご注文情報一覧</a></li>
	<%if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
		<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_LIST) %>">定期購入一覧</a></li>
	<%} %>
	<% if (Constants.CROSS_POINT_OPTION_ENABLED) { %>
	<li>
		<a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_STORE_ORDER_HISTORY_LIST %>">
			店舗購入履歴一覧
		</a>
	</li>
	<% } %>
	<%if (this.DisplayMailSendLogs) { %>
		<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_RECIEVE_MAIL_LIST) %>">受信メール履歴</a></li>
	<%} %>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_LIST) %>">入荷お知らせメール</a></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_WITHDRAWAL_INPUT) %>">退会手続き</a></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_LIST) %>">アドレス帳</a></li>
	<%if (Constants.MAX_NUM_REGIST_CREDITCARD > 0) { %>
		<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST) %>">クレジットカード情報</a></li>
	<% } %>
	<%if (Constants.W2MP_POINT_OPTION_ENABLED) { %>   
		<li><a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_USERPOINTHISTORY_LIST %>">ポイント履歴一覧</a></li>
	<% } %>
	<%if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
		<li><a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_COUPON_BOX %>">
			クーポンBOX</a></li>
	<%} %>
	<%if (Constants.COMMON_SOCIAL_LOGIN_ENABLED) { %>
		<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION) %>">ソーシャルログイン連携</a></li>
	<% } %>
	<% if ((this.IsEasyUser == false) && Constants.TWOCLICK_OPTION_ENABLE) { %>
	<li><a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_DEFAULT_ORDER_SETTING_LIST %>">
		注文方法の保存</a></li>
	<% } %>
	<% if (OrderCommon.DisplayTwInvoiceInfo()) { %>
		<li><a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_INVOICE_LIST %>">電子発票管理</a></li>
	<% } %>
	<% if (Constants.INTRODUCTION_COUPON_OPTION_ENABLED) { %>
	<li>
		<a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_FRIEND_REFERRAL %>">
			お友達紹介コード
		</a>
	</li>
	<% } %>
	<% if (Constants.PAYMENT_GMO_POST_ENABLED) { %>
	<li>
		<a href="<%: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_FRAME_GUARANTEE %>">
			枠保証登録情報
		</a>
	</li>
	<% } %>
</ul>
</nav>
<%-- △編集可能領域△ --%>