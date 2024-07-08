<%--
=========================================================================================================
  Module      : 新着情報一覧画面(NewsList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace = "w2.Domain.News" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/NewsList.aspx.cs" Inherits="Form_NewsList" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
	<title>お知らせ一覧ページ</title>
	<link href="../Css/common.css" rel="stylesheet" type="text/css" media="all" />
</head>
<body id="InformationList">
<form id="form1" runat="server">
		<div id="dvInformationArea">
			<dl>
				<dt class="clearFix">
						お知らせ
				</dt>
				<%-- ▽新着情報ループ▽ --%>
				<asp:Repeater ID="rNewsList" runat="server" ItemType="NewsModel">
				<HeaderTemplate>
				<dd>
					<ul>
				</HeaderTemplate>
				<ItemTemplate>
						<li class="clearFix">
							<span class="infoDate">
								<%#: DateTimeUtility.ToStringFromRegion(Item.DisplayDateFrom, DateTimeUtility.FormatType.ShortDate2Letter) %>
							</span>
							<span class="infoContent">
								<%# Item.GetNewsTextHtml() %>
							</span>
						</li>
				</ItemTemplate>
				<FooterTemplate>
					</ul>
				</dd>
				</FooterTemplate>
				</asp:Repeater>
				<%-- △新着情報ループ△ --%>
			</dl>
			<p class="InformationFooter">
				<a href="javascript:window.close();" class="btn btn-mini">閉じる</a>
			</p>
		</div>
</form>
</body>
</html>