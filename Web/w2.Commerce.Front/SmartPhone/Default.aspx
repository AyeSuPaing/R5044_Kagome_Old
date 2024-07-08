<%--
=========================================================================================================
  Module      : トップ画面(Default.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="Parts000TMPL_999" Src="~/SmartPhone/Page/Parts//Parts000TMPL_999.ascx" %>
<%@ Register TagPrefix="uc" TagName="Parts900FAT_999" Src="~/SmartPhone/Page/Parts//Parts900FAT_999.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendAdvanced" Src="~/SmartPhone/Form/Common/Product/BodyProductRecommendAdvanced.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductSearchBox" Src="~/SmartPhone/Form/Common/Product/BodyProductSearchBox.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRanking" Src="~/SmartPhone/Form/Common/Product/BodyProductRanking.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductHistory" Src="~/SmartPhone/Form/Common/Product/BodyProductHistory.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyNews" Src="~/SmartPhone/Form/Common/BodyNews.ascx" %>
<%@ Register TagPrefix="uc" TagName="Criteo" Src="~/Form/Common/Criteo.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateListForTop" Src="~/SmartPhone/Form/Common/Coordinate/BodyCoordinateListForTop.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFeaturePageList" Src="~/SmartPhone/Form/Common/FeaturePage/BodyFeaturePageList.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Default.aspx.cs" Inherits="Default" Title="ｗ２ショッピングデモサイト トップページ" %>

<%--

下記は保持用のダミー情報です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<% if (Constants.MOBILEOPTION_ENABLED){%>
	<link rel="Alternate" media="handheld" href="<%= WebSanitizer.HtmlEncode(GetMobileUrl()) %>" />
<% } %>
<%= this.BrandAdditionalDsignTag %>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<span style="color: #fff"><%# this.BrandId %></span>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<uc:Parts900FAT_999 runat="server" />
<uc:Parts000TMPL_999 runat="server" />
<%-- △レイアウト領域△ --%>
</div>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<uc:BodyNews runat="server"/>
<uc:BodyProductRanking runat="server" />
<uc:BodyProductRecommendAdvanced runat="server" />
<uc:BodyProductHistory runat="server" />
<section class="search unit">
	<h3>キーワードから探す</h3>
	<uc:BodyProductSearchBox runat="server" />
</section>

<%-- ▽コーディネート▽ --%>
<uc:BodyCoordinateListForTop ID="BodyCoordinateList" runat="server" />
<%-- △コーディネート△ --%>

<uc:BodyFeaturePageList runat="server"/>

<%-- △編集可能領域△ --%>
<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
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
	// レコメンドコードを設定します
	lProductRecommendByRecommendEngineUserControls[1].RecommendCode = "sp112";
	// レコメンドタイトルを設定します
	lProductRecommendByRecommendEngineUserControls[1].RecommendTitle = "おすすめ商品一覧";
	// 商品最大表示件数を設定します
	lProductRecommendByRecommendEngineUserControls[1].MaxDispCount = 6;
	// レコメンド対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[1].DispCategoryId = "";
	// レコメンド非対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[1].NotDispCategoryId = "";
	// レコメンド非対象にするアイテムIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[1].NotDispRecommendProductId = "";
}

// 1つ目のカテゴリレコメンド
if (lCategoryRecommendByRecommendEngineUserControls.Count > 0)
{
    //レコメンドコードを設定します
    lCategoryRecommendByRecommendEngineUserControls[0].RecommendCode = "p005";
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

<%-- CRITEOタグ --%>
<uc:Criteo ID="criteo" runat="server" Datas="<%# null %>" />
</asp:Content>