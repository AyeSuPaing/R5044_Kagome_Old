<%--
=========================================================================================================
  Module      : 共通ヘッダ出力コントローラ(BodyHeaderMain.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyMiniCart" Src="~/Form/Common/BodyMiniCart.ascx" %>
<%@ Register TagPrefix="uc" TagName="GlobalChangeMenu" Src="~/Form/Common/Global/GlobalChangeMenu.ascx" %>
<%@ Control Language="c#" AutoEventWireup="true" Inherits="Form_Common_BodyHeaderMain" CodeFile="~/Form/Common/BodyHeaderMain.ascx.cs" %>
<%--

下記は保持用のダミー情報です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
<%
	// 検索テキストボックスEnterで検索させる（UpdatePanelで括っておかないと非同期処理時に検索が効かなくなる）
	this.WtbSearchWord.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + WlbSearch.UniqueID + "',''); return false;}";
%>
</ContentTemplate>
</asp:UpdatePanel>

<%-- ▽編集可能領域：共通ヘッダ領域▽ --%>
<asp:UpdatePanel ID="upUpdatePanel2" runat="server">
<ContentTemplate>
<% this.Reload(); %>
<div id="Head">
	<div class="inner clearFix">
		<%--▼ グローバル:切り替えタグ ▼--%>
		<uc:GlobalChangeMenu runat="server"/>
		<%--▲ グローバル:切り替えタグ ▲--%>

		<div id="LogoMain">
			<h1><a href="<%= WebSanitizer.HtmlEncode(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT) %>">w2Commerce <span>DEMO SITE</span></a></h1>
		</div>

		<div id="HeadRight" class="clearfix">
		<div id="HeadSearch" class="clearfix">
			<div class="wrapSearch">
				<div class="textBox">
					<%
						tbSearchWord.Attributes["placeholder"] = "何をお探しですか？";
					 %>
					<asp:TextBox ID="tbSearchWord" runat="server" MaxLength="250"></asp:TextBox>
				</div>
				<div class="btnSearch">
					<asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_Click">
					<img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/icn_search.gif" />
					</asp:LinkButton>
				</div>
			</div>
			<p class="advanceSearch"><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/AdvancedSearch.aspx") %>">詳しく検索</a></p>
		</div>


		<div id="HeadMembers" class="hoverMenu">
			<%if (this.IsLoggedIn) { %>
				<a href="<%: this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE %>" class="line2">ようこそ <%= WebSanitizer.HtmlEncode(this.LoginUserName) %> 様
					<% if (Constants.W2MP_POINT_OPTION_ENABLED) { %>
						<br />
						ポイント <%= WebSanitizer.HtmlEncode(GetNumeric(this.LoginUserPointUsable)) %>pt
					<% } %>
				</a>
			<% }else{ %>
				<a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN + "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + HttpUtility.UrlEncode(this.NextUrl)) %>" class="line1">ログイン</a>
			<% } %>
			<div class="menu clearFix">
				<%if (this.IsLoggedIn) { %>
				<%-- ▽ポイントオプション利用時▽ --%>
				<%if (Constants.W2MP_POINT_OPTION_ENABLED) {%>
				<ul>
					<li>通常ポイント&nbsp;<%: GetNumeric(this.LoginUserBasePoint) %>pt</li>
					<% if (this.LoginUserPointExpiry.HasValue) {%>
						<li>&nbsp;&nbsp;&nbsp;有効期限:<%: DateTimeUtility.ToStringFromRegion(this.LoginUserPointExpiry, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime)%></li>
					<% } %>
					<% if (this.IsLimitedTermPointUsable){ %>
					<li>
						期間限定ポイント&nbsp;<%: GetNumeric(this.LoginUserLimitedTermPointTotal) %>pt
					</li>
					<li>
						&nbsp;&nbsp;&nbsp;有効期間中&nbsp;<%: GetNumeric(this.LoginUserLimitedTermPointUsableTotal) %>pt
					</li>
					<% } %>
					<li>（仮ポイント&nbsp;<%: GetNumeric(this.LoginUserPointTemp) %>pt）</li>
					<li><span style="font-size: 0.8em;"><a href="<%: this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE %>" style="cursor: pointer; display: <%: this.HasLimitedTermPoint ? "inline" : "none" %>;">内訳を表示</a></span></li>
				</ul>
				<%} %>
				<%-- △ポイントオプション利用時△ --%>
				<%} %>
				<%if (this.IsLoggedIn == false) { %>
				<ul>
					<li>
						<asp:LinkButton ID="lbUserRegist" runat="server" OnClick="lbUserRegist_Click" Text ="会員登録はこちら"/>
					</li>
					<%if (Constants.USEREAZYREGISTERSETTING_OPTION_ENABLED) {%>
						<li>
							<asp:LinkButton ID="lbUserEasyRegist" runat="server" OnClick="lbUserEasyRegist_Click" Text ="かんたん会員登録はこちら"/>
						</li>
					<%} %>
				</ul>
				<%} %>
				<ul>
					<li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MYPAGE) %>">マイページ</a></li>
					<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_LIST) %>">注文履歴・発送状況</a>
					<li>
					<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST) %>">お気に入り</a></li>
					<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_MAILMAGAZINE_REGIST_INPUT) %>">メールマガジン</a></li>
					<%if (this.IsLoggedIn) { %>
					<li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGOFF) %>" onclick="return confirm('ログアウトします。\nよろしいですか？');return false;">ログアウト</a></li>
					<% } %>
				</ul>
			</div>
		</div>

		<div id="HeadCartView" class="hoverMenu">
			<a href="<%: this.CartListPageUrl %>">
				ショッピングカート<br />
			<%= WebSanitizer.HtmlEncode(GetNumeric(this.ProductCount))%>点 / <%: CurrencyManager.ToPrice(this.ProductPriceSubtotal) %>
			</a>
			<div class="menu clearFix">
				<uc:BodyMiniCart id="BodyMiniCart" runat="server" />
			</div>
		</div>

		<div id="HeadUserNav" class="hoverMenu">
			<a href="javascript:void(0);"><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/icn_menubar.gif" /></a>
			<div class="menu clearFix">
				<h3>ヘルプ</h3>
				<ul>
				<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/first.aspx") %>">はじめての方へ</a></li>
				<li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_INQUIRY_INPUT) %>">お問い合わせ</a></li>
				<li><a href="#">よくある質問</a></li>
				<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/privacy.aspx") %>">プライバシーポリシー</a></li>
				<% if (Constants.REALSHOP_OPTION_ENABLED) { %>
				<li><a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_SHOP_LIST %>">店舗一覧</a></li>
				<% } %>
				</ul>
			</div>
		</div>

		</div>
	</div>
</div>

<div id="HeadGlobalNavi">
	<ul>
	<li class="onMenu"><a href="javascript:void(0);">Items</a>
		<div class="HeadGNaviList clearfix">
			<p class="title">Item Category</p>
			<div class="unitMenu">
			<ul>
				<li>・<a href="#">Item01</a></li>
				<li>・<a href="#">Item02</a></li>
				<li>・<a href="#">Item03</a></li>
				<li>・<a href="#">Item04</a></li>
				<li>・<a href="#">Item05</a></li>
			</ul>
			</div>
		</div>
	</li><!--
	--><li class="onMenu"><a href="javascript:void(0);">Brand</a>
		<div class="HeadGNaviList clearfix">
			<p class="title">Brand List</p>
			<div class="unitMenu">
			<ul>
				<li>・<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "DefaultBrandTop.aspx?bid=brand1") %>">Brand01</a></li>
				<li>・<a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "DefaultBrandTop.aspx?bid=brand2") %>">Brand02</a></li>
			</ul>
			</div>
		</div>
	</li><!--
	--><li><a href="<%= Constants.PATH_ROOT %>Form/Coordinate/CoordinateTop.aspx">Coordinate</a></li><!--
	--><li><a href="javascript:void(0);">News</a></li><!--
	--><li><a href="javascript:void(0);">Ranking</a></li><!--
	--><li><a href="javascript:void(0);">Campaign</a></li>
	</ul>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- △編集可能領域△ --%>

<%--
下記はファイル情報保持用のダミーです。削除しないでください。
<%@ FileInfo LastChanged="ｗ２ユーザー" %>
--%>