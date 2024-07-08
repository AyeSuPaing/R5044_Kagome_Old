<%--
=========================================================================================================
  Module      : 登録解除画面(Unsubscribe.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Mail/Unsubscribe.aspx.cs" Inherits="Form_Mail_Unsubscribe" Title="メール登録解約ページ" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
	
	<div id="dvUnsubscribeInfoBox">
		<h2 id="hUnsubscribeTitle">お知らせメール配信解除</h2>
		<div id="dvUnsubscribeInfo">
			<h6 id="hUnsubscribeContent">お知らせメールの配信を解除しました</h6>
		</div>
	</div>
	<div class="dvUnsubscribeBtnBox">
		<p>
			<span>
				<a href="<%: Constants.PATH_ROOT %>" class="btn btn-large btn-inverse">トップページへ</a>
			</span>
		</p>
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
</asp:Content>
