<%--
=========================================================================================================
  Module      : スマートフォン用商品カテゴリリスト出力コントローラ(BodyProductCategoryLinks.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="c#" Inherits="Form_Common_Product_BodyProductCategoryLinks" CodeFile="~/Form/Common/Product/BodyProductCategoryLinks.ascx.cs" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:Repeater id="rCategoriesLink" runat="server">
<HeaderTemplate><div class="breadcrumbs"><a href="<%# WebSanitizer.HtmlEncode(Constants.PATH_ROOT) %>">トップ</a></HeaderTemplate>
<ItemTemplate>
	<span>＞</span>
	<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateCategoryLinkUrl(Eval(Constants.FIELD_PRODUCTCATEGORY_SHOP_ID), Eval(Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID), Eval(Constants.FIELD_PRODUCTCATEGORY_URL), (string)Eval(Constants.FIELD_PRODUCTCATEGORY_NAME))) %>'>
	<%# WebSanitizer.HtmlEncode( Eval(Constants.FIELD_PRODUCTCATEGORY_NAME)) %></a>
</ItemTemplate>
<FooterTemplate></div></FooterTemplate>
</asp:Repeater>
<%-- △編集可能領域△ --%>