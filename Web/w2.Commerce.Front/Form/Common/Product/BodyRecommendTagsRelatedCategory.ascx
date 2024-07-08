<%--
=========================================================================================================
  Module      : おすすめタグ出力コントローラ(BodyRecommendTagsRelatedCategory.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyRecommendTagsRelatedCategory.ascx.cs" Inherits="Form_Common_Product_BodyRecommendTagsRelatedCategory" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>
--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="dvTagList" visible="<%# (this.Tags != null && this.Tags.Count != 0) %>" runat="server">
	<div id ="dvRecommend">
	<h3>関連タグ</h3>
	<asp:Repeater DataSource="<%# this.Tags %>" ItemType="w2.App.Common.Awoo.ClassifyProductType.Tags" runat="server" >
		<HeaderTemplate>
		</HeaderTemplate>
		<ItemTemplate>
			<a class="recommendTag" href="<%# CreateRecommendProductsUrl(Item) %>">
				<span><%# Item.Text %></span>
			</a>
		</ItemTemplate>
	</asp:Repeater>
		</div>
</div>
<%-- △編集可能領域△ --%>
