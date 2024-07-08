<%--
=========================================================================================================
  Module      : 特集エリアパーツテンプレート画面(PartsBannerTemplate.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="FeatureAreaUserControl" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ Page Title="" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
base.Page_Init(sender, e);

<%-- ▽編集可能領域：プロパティ設定▽ --%>
// 特集エリア
this.FeatureAreaId = "id";

<%-- △編集可能領域△ --%>
}
</script>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<asp:Repeater id="rFeatureArea" ItemType="FeatureAreaUserControl.FeatureAreaBannerInput" Runat="server">
<HeaderTemplate>
	<%-- ▽開始タグ▽ --%>
	<%-- △開始タグ△ --%>
</HeaderTemplate>
<ItemTemplate>
	<%-- ▽繰り返しタグ▽ --%>
	<%-- △繰り返しタグ△ --%>
</ItemTemplate>
<FooterTemplate>
	<%-- ▽終了タグ▽ --%>
	<%-- △終了タグ△ --%>
</FooterTemplate>
</asp:Repeater>
<script>
	<!--
	<%-- ▽スクリプトタグ▽ --%>
	<%-- △スクリプトタグ△ --%>
	// -->
</script>
<%-- △編集可能領域△ --%>