<%--
=========================================================================================================
  Module      : クリックポイント付与画面(ClickPoint.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/ClickPoint.aspx.cs" Inherits="Form_ClickPoint" Title="クリックポイント付与ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
<h2>クリックポイント付与</h2>
<center>
	<%-- 既にポイント付与している時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.SpentPoint) {%>
		<div class="order-unit">
		既にご利用済みです<br />
		<div class="button-next">
		<a href="<%: Constants.PATH_ROOT %>" class="btn"">TOPへ戻る</a>
		</div>
		</div>
	<%}%>

	<%-- URLにルールIDがない時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.UrlError) {%>
		<div class="order-unit">
		このURLは正しくありません<br />
		<a href="<%: Constants.PATH_ROOT %>" class="btn">TOPへ戻る</a>
		</div>
	<%}%>

	<%-- ポイント付与する時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.ProvidingPoint) {%>
		<div class="order-unit">
		ありがとうございます<br />
		<%: this.IncNum %>ポイント付与しました<br />
		<a href="<%: Constants.PATH_ROOT %>" class="btn">TOPへ戻る</a>
		</div>
	<%}%>

	<%-- ポイント付与期間が終了した時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.Expired) {%>
		<div class="order-unit">
		このキャンペーンは終了しました<br />
		<a href="<%: Constants.PATH_ROOT %>" class="btn">TOPへ戻る</a>
		</div>
	<%}%>

	<%-- URLに間違いがあった時 --%>
	<% if (this.ClickPointSelect == ClickPointStatus.Error) {%>
		<div class="order-unit">
		ポイント付与できませんでした<br />
		<a href="<%: Constants.PATH_ROOT %>" class="btn">TOPへ戻る</a>
		</div>
	<%}%>
</center>
</div>
</asp:Content>