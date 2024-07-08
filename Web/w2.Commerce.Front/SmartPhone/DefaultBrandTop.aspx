<%--
=========================================================================================================
  Module      : ブランドトップ画面(DefaultBrandTop.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRanking" Src="~/SmartPhone/Form/Common/Product/BodyProductRanking.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendAdvanced" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendAdvanced.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCategoryRecommendByRecommendEngine" Src="~/SmartPhone/Form/Common/Product/BodyCategoryRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductHistory" Src="~/SmartPhone/Form/Common/Product/BodyProductHistory.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyNews" Src="~/SmartPhone/Form/Common/BodyNews.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductSearchBox" Src="~/SmartPhone/Form/Common/Product/BodyProductSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFeaturePageList" Src="~/SmartPhone/Form/Common/FeaturePage/BodyFeaturePageList.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Default.aspx.cs" Inherits="Default" Title="ｗ２ショッピングデモサイト トップページ" %>
<%--

下記は保持用のダミー情報です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<% if (Constants.PRODUCT_BRAND_ENABLED == false) { %>
		<meta name="robots" content="noindex">
	<%} %>
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<% if (Constants.MOBILEOPTION_ENABLED){%>
	<link rel="Alternate" media="handheld" href="<%= WebSanitizer.HtmlEncode(GetMobileUrl()) %>" />
<% } %>
<%= this.BrandAdditionalDsignTag %>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<span style="color: #fff"><%# this.BrandId %></span>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- <uc:BodyProductSearchBox runat="server" /> --%>
<section class="hero unit">
	<% if (this.BrandId == "brand1"){ %>
		<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/brand01/hero01.jpg" alt="" width="320" />
	<% } else if (this.BrandId == "brand2"){ %>
		<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/brand02/hero01.jpg" alt="" width="320" />
	<% } else { %>
		<img src="<%= Constants.PATH_ROOT %>SmartPhone/Contents/ImagePkg/hero01.jpg" alt="" width="320" />
	<% } %>
</section>
<uc:BodyProductRecommendAdvanced runat="server" />
<uc:BodyProductRecommendByRecommendEngine runat="server" />
<uc:BodyCategoryRecommendByRecommendEngine runat="server" Visible="false" />
<uc:BodyProductRanking runat="server" />
<uc:BodyNews runat="server"/>
<uc:BodyProductHistory runat="server"/>
<section class="search unit">
	<h3>キーワードから探す</h3>
	<uc:BodyProductSearchBox runat="server" />
</section>
	
<uc:BodyFeaturePageList runat="server"/>
<%-- △編集可能領域△ --%>
<script runat="server">
public new void Page_Load(Object sender, EventArgs e)
{
base.Page_Load(sender, e);

var recommendEngineUserControls = WebControlUtility.GetRecommendEngineUserControls(this.Form.FindControl("ContentPlaceHolder1"));
var lProductRecommendByRecommendEngineUserControls = recommendEngineUserControls.Item1;
var lCategoryRecommendByRecommendEngineUserControls = recommendEngineUserControls.Item2;

<%-- ▽編集可能領域：プロパティ設定▽ --%>
// 外部レコメンド連携パーツ設定
// 1つ目の商品レコメンド
if (lProductRecommendByRecommendEngineUserControls.Count > 0)
{
	// レコメンドコードを設定します
	lProductRecommendByRecommendEngineUserControls[0].RecommendCode = "sp111";
	// レコメンドタイトルを設定します
	lProductRecommendByRecommendEngineUserControls[0].RecommendTitle = "おすすめ商品一覧";
	// 商品最大表示件数を設定します
	lProductRecommendByRecommendEngineUserControls[0].MaxDispCount = 6;
	// レコメンド対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[0].DispCategoryId = "";
	// レコメンド非対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[0].NotDispCategoryId = "";
	// レコメンド非対象にするアイテムIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[0].NotDispRecommendProductId = "";
}

// 2つ目の商品レコメンド
if (lProductRecommendByRecommendEngineUserControls.Count > 1)
{
	//レコメンドコードを設定します
	lProductRecommendByRecommendEngineUserControls[1].RecommendCode = "p002";
	//アイテムコードを設定します
	//特定の商品に対する行動履歴から、おすすめ商品を取得したい場合に設定します。
	//「"P"+商品ID」の形式で記述してください。【記述例】P001
	lProductRecommendByRecommendEngineUserControls[1].ItemCode = "";
	//商品最大表示件数を設定します
	lProductRecommendByRecommendEngineUserControls[1].MaxDispCount = 5;
	//商品画像サイズを設定します (S/M/L/LL)
	lProductRecommendByRecommendEngineUserControls[1].ImageSize = "M";
}
	
// 1つ目のカテゴリレコメンド
if (lCategoryRecommendByRecommendEngineUserControls.Count > 0)
{
	//レコメンドコードを設定します
	//lCategoryRecommendByRecommendEngineUserControls[0].RecommendCode = "p005";
	//アイテムコードを設定します
	//特定のカテゴリに対する行動履歴から、おすすめカテゴリを取得したい場合に設定します。
	//「"C"+カテゴリID」の形式で記述してください。【記述例】C001
	lCategoryRecommendByRecommendEngineUserControls[0].ItemCode = "";
	//表示区分を設定します (0:該当カテゴリのみ表示/1:パンくずリスト表示)
	lCategoryRecommendByRecommendEngineUserControls[0].DispKbn = "0";
	//商品最大表示件数を設定します
	lCategoryRecommendByRecommendEngineUserControls[0].MaxDispCount = 5;
}
<%-- △編集可能領域△ --%>
}

</script>
</asp:Content>