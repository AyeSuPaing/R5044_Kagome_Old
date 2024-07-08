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
		<section class="unit">
			<h3 class="title">頒布会コース一覧</h3>
			<ul class="product-list-4 clearfix">
	</HeaderTemplate>
	<ItemTemplate>
		<li>
			<ul>
				<li>
					<div>
						<%#: Item.DisplayName %>
					</div>
					<div>
						<%#: Item.CourseId %>
					</div>
					<div>
						<a href="<%#: CreateDetailUrl(Item.CourseId) %>" class="btn">詳細ページへ</a>
					</div>
				</li>
			</ul>
		</li>
	</ItemTemplate>
</asp:Repeater>


<%-- △編集可能領域△ --%>