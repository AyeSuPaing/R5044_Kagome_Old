<%--
=========================================================================================================
  Module      : スマートフォン用商品検索ボックス出力コントローラ(BodyProductSearchBox.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductSearchBox.ascx.cs" Inherits="Form_Common_Product_BodyProductSearchBox" %>
<%--
下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="最終更新者" %>
--%>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
	<ContentTemplate>
<% 
	// 検索テキストボックスEnterで検索させる
	this.WtbSearchWord.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSearch.UniqueID + "',''); return false;}";
%>
	</ContentTemplate>
</asp:UpdatePanel>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<!-- 商品検索フォーム -->
<div class="search">
  <div class="search-item">
  <ul>
    <li><img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/common/icon-search-box.png" alt="検索" width="30" /></li>
    <li><w2c:ExtendedTextBox ID="tbSearchWord" type="search" runat="server" MaxLength="250" placeholder="キーワードから商品を探す">
    </w2c:ExtendedTextBox></li>
    <li><asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_Click" CssClass="btn">検索</asp:LinkButton></li>
  </ul>
  </div>
  <!--
  <div class="dropdown">
    <asp:DropDownList ID="ddlCategories" Runat="server"></asp:DropDownList>
  </div>
  -->
</div>
<%-- △編集可能領域△ --%>
