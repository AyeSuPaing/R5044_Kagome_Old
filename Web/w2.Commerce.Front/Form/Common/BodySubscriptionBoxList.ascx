<%--
=========================================================================================================
Module      : 頒布会コース一覧出力コントローラ(BodySubscriptionBoxList.ascx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodySubscriptionBoxList.ascx.cs" Inherits="Form_Common_BodySubscriptionBoxList" %>
<%@ Import Namespace="w2.Domain.SubscriptionBox" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:Repeater ItemType="w2.Domain.SubscriptionBox.SubscriptionBoxModel" ID="rSubscriptionBoxCourseList" Visible="<%# this.CourseListCount > 0 %>" runat="server">
	<HeaderTemplate>
		<div id="dvSubscriprionBoxList" class="unit">
			<h3>頒布会コース一覧</h3>
			<div class="listProduct clearFix">
	</HeaderTemplate>
	<ItemTemplate>
		<div class="glbPlist column4">
			<ul>
				<li class="name">
					<%#: Item.DisplayName %>
				</li>
				<li class="id">
					<%#: Item.CourseId %>
				</li>
				<li>
					<a href="<%#: CreateDetailUrl(Item.CourseId) %>" class="btn-sort-search btn btn-mid btn-inverse">詳細ページへ</a>
				</li>
			</ul>
		</div>
	</ItemTemplate>
</asp:Repeater>

	<%-- △編集可能領域△ --%>