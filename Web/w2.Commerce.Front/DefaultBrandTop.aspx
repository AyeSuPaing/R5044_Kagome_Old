<%--
=========================================================================================================
  Module      : ブランドトップ画面(DefaultBrandTop.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%@ Register TagPrefix="uc" TagName="BodyProductCategoryTree" Src="~/Form/Common/Product/BodyProductCategoryTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRanking" Src="~/Form/Common/Product/BodyProductRanking.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendAdvanced" Src="~/Form/Common/Product/BodyProductRecommendAdvanced.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductHistory" Src="~/Form/Common/Product/BodyProductHistory.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyNews" Src="~/Form/Common/BodyNews.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFeaturePageList" Src="~/Form/Common/FeaturePage/BodyFeaturePageList.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyProductRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyProductRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyCategoryRecommendByRecommendEngine" Src="~/Form/Common/Product/BodyCategoryRecommendByRecommendEngine.ascx" %>
<%@ Register TagPrefix="uc" TagName="Parts010RCMD_001" Src="~/Page/Parts/Parts010RCMD_001.ascx" %>
<%@ Register TagPrefix="uc" TagName="Parts010RCMD_002" Src="~/Page/Parts/Parts010RCMD_002.ascx" %>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Default.aspx.cs" Inherits="Default" Title="ｗ２ショッピングデモサイト トップページ" %>
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
	<link rel="Alternate" media="handheld" href="<%= GetMobileUrl() %>" />
<% } %>
<%= this.BrandAdditionalDsignTag %>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>Js/mainvisual.js"></script>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<span style="color: #fff"><%# this.BrandId %></span>
<div id="maincontents">
    <div id="mainvisual">

        <div id="mainvisual-image">
            <div id="photoset-main">
            <% if (this.BrandId == "brand1"){ %>
                <p><a href=""><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/slide01B1.jpg" alt=""/></a></p>
                <p><a href=""><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/slide01B1.jpg" alt=""/></a></p>
            <% } %>
            <% if (this.BrandId == "brand2"){ %>
                <p><a href=""><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/slide01B2.jpg" alt=""/></a></p>
                <p><a href=""><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/slide01B2.jpg" alt=""/></a></p>
            <% } %>
            </div>
        </div>

        <div id="mainvisual-thumb">
            <ul>
            <% if (this.BrandId == "brand1"){ %>
                <li><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/slide01B1_thumb.jpg" alt=""/></li>
                <li><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/slide01B1_thumb.jpg" alt=""/></li>
            <% } %>
            <% if (this.BrandId == "brand2"){ %>
                <li><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/slide01B2_thumb.jpg" alt=""/></li>
                <li><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/slide01B2_thumb.jpg" alt=""/></li>
            <% } %>
            </ul>
        </div>

        <div id="mainvisual-txt">
            <p class="line01">テキスト01テキスト01テキスト01テキスト01</p>
            <p class="line01">テキスト02テキスト02テキスト02テキスト02</p>
        </div>
    </div>
</div>

<div class="mb10">
    <a href=""><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/bnr_top980a.jpg" alt=""/></a>
</div>

<div>
    <a href=""><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/bnr_top980b.jpg" alt=""/></a>
</div>

<table id="tblLayout">
<tr>
<td>
<div id="secondary">
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<div class="bnrBtn newMember">
    <a href="<%= Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION %>">新規会員登録</a>
</div>
<div class="unit">
    <a href="">
        <img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/bnr_side.jpg" alt="" />
    </a>
</div>
<div class="unit">
    <uc:BodyNews runat="server"/>
</div>
<uc:BodyProductCategoryTree runat="server" />
<%-- △レイアウト領域△ --%>
</div>
</td>
<td>
<div id="primary" class="top">
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- △編集可能領域△ --%>
<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<uc:BodyProductRanking runat="server" />
<uc:BodyProductRecommendAdvanced runat="server" />
<uc:BodyFeaturePageList runat="server"/>
<%--
<uc:BodyProductRecommendByRecommendEngine runat="server" />
<uc:BodyProductRecommendByRecommendEngine runat="server" />
<uc:BodyCategoryRecommendByRecommendEngine runat="server" />
--%>
<%-- △レイアウト領域△ --%>
</div>
</div>
</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>

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
	lProductRecommendByRecommendEngineUserControls[0].RecommendCode = "pc111";
	// レコメンドタイトルを設定します
	lProductRecommendByRecommendEngineUserControls[0].RecommendTitle = "おすすめ商品一覧";
	// 商品最大表示件数を設定します
	lProductRecommendByRecommendEngineUserControls[0].MaxDispCount = 5;
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
	lProductRecommendByRecommendEngineUserControls[1].RecommendCode = "pc112";
	// レコメンドタイトルを設定します
	lProductRecommendByRecommendEngineUserControls[1].RecommendTitle = "おすすめ商品一覧";
	// 商品最大表示件数を設定します
	lProductRecommendByRecommendEngineUserControls[1].MaxDispCount = 5;
	// レコメンド対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[1].DispCategoryId = "";
	// レコメンド非対象にするカテゴリIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[1].NotDispCategoryId = "";
	// レコメンド非対象にするアイテムIDを設定します（複数選択時はカンマ区切りで指定）
	lProductRecommendByRecommendEngineUserControls[1].NotDispRecommendProductId = "";
}

// 1つ目のカテゴリレコメンド

<%-- △編集可能領域△ --%>
}

</script>
</asp:Content>