<%--
=========================================================================================================
  Module      : クリックポイント付与画面(ClickPoint.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/ClickPoint.aspx.cs" Inherits="Form_ClickPoint" Title="クリックポイント付与ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
<h2>クリックポイント付与</h2>
<center>
	<%-- 既にポイント付与している時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.SpentPoint) {%>
		<div id="dvLogin" class="unit clearFix">
		既にご利用済みです<br />
		<div class="button-next">
		<a href="<%: Constants.PATH_ROOT %>" class="btn-org btn-large btn-org-blk">TOPへ戻る</a>
		</div>
		</div>
	<%}%>

	<%-- URLにルールIDがない時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.UrlError) {%>
		<div id="dvLogin" class="unit clearFix">
		このURLは正しくありません<br />
		<a href="<%: Constants.PATH_ROOT %>" class="btn-org btn-large btn-org-blk">TOPへ戻る</a>
		</div>
	<%}%>

	<%-- ポイント付与する時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.ProvidingPoint) {%>
		<div id="dvLogin" class="unit clearFix">
		ありがとうございます<br />
		<%: this.IncNum %>ポイント付与しました<br />
		<a href="<%: Constants.PATH_ROOT %>" class="btn-org btn-large btn-org-blk">TOPへ戻る</a>
		</div>
	<%}%>

	<%-- ポイント付与期間が終了した時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.Expired) {%>
		<div id="dvLogin" class="unit clearFix">
		このキャンペーンは終了しました<br />
		<a href="<%: Constants.PATH_ROOT %>" class="btn-org btn-large btn-org-blk">TOPへ戻る</a>
		</div>
	<%}%>

	<%-- URLに間違いがあった時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.Error) {%>
		<div id="dvLogin" class="unit clearFix">
		ポイント付与できませんでした<br />
		<a href="<%: Constants.PATH_ROOT %>" class="btn-org btn-large btn-org-blk">TOPへ戻る</a>
		</div>
	<%}%>
</center>
</div>
</asp:Content>