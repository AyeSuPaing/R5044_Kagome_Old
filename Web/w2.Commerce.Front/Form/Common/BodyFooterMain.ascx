<%--
=========================================================================================================
  Module      : 共通フッタ出力コントローラ(BodyFooterMain.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="AccessLogTrackerScript" Src="~/Form/Common/AccessLogTrackerScript.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyFooterMain.ascx.cs" Inherits="Form_Common_BodyFooterMain" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<%-- ▽編集可能領域：フッタ領域▽ --%>
<div class="inner">
<ul id="FootNav">
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/first.aspx") %>">はじめての方へ</a></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_INQUIRY_INPUT) %>">お問い合わせ</a></li>
	<li><a href="#">よくある質問</a></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/privacy.aspx") %>">プライバシーポリシー</a></li>
	<li><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Page/termofuse.aspx") %>">特定商取引法に基づく表記</a></li>
	<li><a href="#">会社概要</a></li>
</ul>

<div id="coryRight"><address>Copyright © W2 Co.,Ltd. Since 2005 All Rights Reserved.</address></div>
<div id="ToTop"><a href="#Header">ページTOPへ</a></div>
<%if (this.IsSmartPhone 
		&& SmartPhoneUtility.GetSmartPhoneUrl(
								Request.AppRelativeCurrentExecutionFilePath,
								Request.UserAgent,
								HttpContext.Current) != null)
{%>
<div style="text-align:right;padding-right:20px;"><a class="button" href="<%= WebSanitizer.UrlAttrHtmlEncode(this.ChangeToSmartPhoneSiteUrl) %>">スマートフォンサイトへ</a> &gt;</div>
<%} %>
</div>
<%-- △編集可能領域△ --%>

<%-- w2アクセスログトラッカー出力 --%>
<uc:AccessLogTrackerScript id="AccessLogTrackerScript1" runat="server" />
