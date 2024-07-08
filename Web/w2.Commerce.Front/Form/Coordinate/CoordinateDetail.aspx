<%--
=========================================================================================================
  Module      : コーディネート詳細画面(CoordinateDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Register TagPrefix="uc" TagName="BodyCoordinateCategoryTree" Src="~/Form/Common/Coordinate/BodyCoordinateCategoryTree.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Coordinate/CoordinateDetail.aspx.cs" Inherits="Form_Coordinate_CoordinateDetail" Title="コーディネート詳細ページ" %>
<%@ Import Namespace="w2.Domain.ContentsTag" %>
<%@ Import Namespace="w2.Domain.CoordinateCategory" %>
<%@ Import Namespace="w2.Domain.Coordinate" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<script type="text/javascript" src="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Js/jquery.elevateZoom-3.0.8.min.js") %>"></script>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<script type="text/javascript">
	//<![CDATA[
	$(function () {
		$('#zoomCoordinate').elevateZoom({
			zoomWindowWidth: 393,
			zoomWindowHeight: 393,
			responsive: true,
			zoomWindowOffetx: 15,
			borderSize: 1,
			cursor: "pointer"
		});

		$('.zoomTarget').click(function (e) {
			var image = $(this).data('image');
			var zoom_image = $(this).data('zoom-image');
			var ez = $('#zoomCoordinate').data('elevateZoom');
			ez.swaptheimage(image, zoom_image);
		});
	});
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			$(function () {
				$('#zoomCoordinate').elevateZoom({
					zoomWindowWidth: 393,
					zoomWindowHeight: 393,
					responsive: true,
					zoomWindowOffetx: 15,
					borderSize: 1,
					cursor: "pointer"
				});

				$('.zoomTarget').click(function (e) {
					$('.zoomTarget').removeClass('selected');
					$(this).addClass('selected');
					var image = $(this).data('image');
					var zoom_image = $(this).data('zoom-image');
					var ez = $('#zoomCoordinate').data('elevateZoom');
					ez.swaptheimage(image, zoom_image);
				});
			});
			//twttr.widgets.load(); //Reload twitter button
		}
	}
	//]]>
</script>
<%-- △編集可能領域△ --%>
</asp:Content>

<%-- ▽編集可能領域：プロパティ設定▽ --%>
<script runat="server">
	public new void Page_Init(Object sender, EventArgs e)
	{
		base.Page_Init(sender, e);

		// 同じスタッフのコーディネートリスト設定
		this.MaxDispCountOfSameStaff = 4;
		this.AddDispCountOfSameStaff = 4;

		// 同じ店舗のコーディネートリスト設定
		this.MaxDispCountOfSameRealShop = 4;
		this.AddDispCountOfSameRealShop = 4;

		// 同じ商品のコーディネートリスト設定
		this.MaxDispCountOfSameProduct = 4;
		this.AddDispCountOfSameProduct = 4;
	}
</script>
<%-- △編集可能領域△ --%>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table id="tblLayout" class="tblLayout_ProductDetail">
<tr>

<td>
<div id="secondary">
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<uc:BodyCoordinateCategoryTree runat="server"/>
<%-- △レイアウト領域△ --%>
</div>
</td>

<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="primary">

<!--▽ 上部カテゴリバー ▽-->
<div id="breadcrumb">
	<ul>
	<li><a href="<%= Constants.PATH_ROOT %>Form/Coordinate/CoordinateTop.aspx">TOP</a></li>
	<li>
		<span>&raquo;&ensp;&ensp;<%#:this.Coordinate.CoordinateTitle %></span>
	</li>
	</ul>
</div>
<!--△ 上部カテゴリバー △-->

<div id="dvProductDetailArea">
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>
<div id="detailImage">
	<!-- コーディネート画像 -->
	
	<div class="mainImage" style="position: relative;">
		<img src="<%#: (string)(this.CoordinateImages[0]) %>" id="zoomCoordinate" style="width: 370px; height: 494px;"/>
		<asp:LinkButton style="z-index: 1000;position:absolute; top:20px; left:20px; width: 70px;" ID="lbLike" OnClick="lbLikeBtn_Click" runat="server"><img src="<%# CreateLikeImage() %>" width="30px;" height="30px;"/>&ensp;<%#: this.LikeCount %></asp:LinkButton>
	</div>
	<!-- サブ画像一覧 -->
	<%-- ▽サブ画像一覧▽ --%>
	<div class="coordinateSubImage">
	<ul>
	<asp:Repeater ID="Repeater3" DataSource="<%# this.CoordinateImages %>" Visible="<%# (this.CoordinateImages.Count != 0) %>" runat="server">
		<ItemTemplate>
			<li id="Li1" runat="server">
				<a href="javascript:void(0);" title="test">
					<!--
					-->
					<img width="87.5px" height="116px" class="zoomTarget" src="<%#: ((string)Container.DataItem) +"?"+DateTime.Now  %>" data-image="<%#: ((string)Container.DataItem) %>" data-zoom-image="<%#: (string)Container.DataItem %>" />
				</a>
			</li>
		</ItemTemplate>
	</asp:Repeater>
	</ul>
	</div>
	<%-- △サブ画像一覧△ --%>
	<%-- SNSボタン ※mixiチェックはクライアント毎にデベロッパ登録したキーを設定する必要あり --%>
	<ul class="snsList clearFix">
		<li><iframe src="//www.facebook.com/plugins/like.php?href=<%# HttpUtility.UrlEncode(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_COORDINATE_DETAIL + "?" + Constants.REQUEST_KEY_COORDINATE_ID + "=" + this.CoordinateId) %>&amp;send=false&amp;layout=button_count&amp;width=450&amp;show_faces=false&amp;action=like&amp;colorscheme=light&amp;font=tahoma&amp;height=21" scrolling="no" frameborder="0" style="border:none; overflow:hidden; width:100px; height:21px;" allowTransparency="true"></iframe></li>
		<li><a href="javascript:void(0);" onclick='<%# WebSanitizer.HtmlEncode("window.open('http://mixi.jp/share.pl?u=" + HttpUtility.UrlEncode(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_COORDINATE_DETAIL + "?" + Constants.REQUEST_KEY_COORDINATE_ID + "=" + this.CoordinateId + "&k=01ac61d95d41a50ea61d0c5ab84adf0cfbf62f7d") + "','share',['width=632','height=456','location=yes','resizable=yes','toolbar=no','menubar=no','scrollbars=no','status=no'].join(','));") %>'><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/mixi_bt_check_1.png" alt="mixiチェック" border="0" /></a></li>
		<li><a href="https://twitter.com/share" class="twitter-share-button" data-count="none" data-lang="ja">ツイート</a><script type="text/javascript" src="https://platform.twitter.com/widgets.js"></script></li>
	</ul>

</div>

<div id="detailOne"  style="padding: 0px 0 0 10px;">
<div class="clearFix box2" runat="server" Visible="<%# ShouldShowStaff(this.Coordinate) %>">
	<img align="left"  id="picture" src="<%#: this.StaffImagePath %>" alt="スタッフ画像" height="80px" border="0" style="padding: 5px"/>
	
	<% if(this.IsFollowing){%>
	<asp:LinkButton ID="LinkButton1" class="btn afterfollow" runat="server" Visible="<%# this.IsStaffValid %>" onclick="lbFollowBtn_Click">フォロー中</asp:LinkButton>
	<% }else{%>
		<asp:LinkButton ID="LinkButton2" class="btn beforefollow" runat="server" Visible="<%# this.IsStaffValid %>" onclick="lbFollowBtn_Click">フォローする</asp:LinkButton>
	<% }%>

	<p style="padding: 5px 5px"><%#: this.Coordinate.StaffName %></p>
	<p style="padding: 5px 5px"><%#: this.Coordinate.RealShopName %></p>
	<p style="padding: 5px 5px">身長:<%#: this.Coordinate.StaffHeight %>cm</p>
	<p style="padding: 5px 5px"><%#: this.Coordinate.StaffProfile %></p>
</div>

<% if(this.Coordinate.DisplayDate != null){ %>
<div style="padding: 10px 0px 0px 0px;"><p style="font-size: 1em; font-weight: bold;">公開日:<%=DisplayDate(this.Coordinate.DisplayDate) %></p></div>
<% } %>
<% if(string.IsNullOrEmpty(this.Coordinate.StaffInstagramId) == false){ %>
<div style="padding: 10px 0px 5px 0px;"><p style="font-size: 1em; font-weight: bold;">Instagram :@<a target="_blank" href="https://www.instagram.com/<%#: this.Coordinate.StaffInstagramId %>"><%#: this.Coordinate.StaffInstagramId %></a></p></div>
<% } %>
<div style="padding: 10px 0px 20px 0px;"><p><%# WebSanitizer.HtmlEncodeChangeToBr(this.Coordinate.CoordinateSummary) %></p></div>


<% if(this.SelectedProductList.Any()){ %>
<div class="coordinateImage">
	<p style="font-size: 1em; font-weight: bold;">このコーディネートに使っている商品</p>
<ul>
<div id="Div1" class="ChangesByVariation" runat="server">
<asp:Repeater ID="Repeater1" OnItemCreated="RepeaterItemCreated" DataSource='<%# this.CoordinateProductList %>' runat="server" >
<ItemTemplate>
<li>
	<a title="<%#: (string)Eval("name") %>" href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, HasVariation((DataRowView)Container.DataItem))) %>' target="_blank">
	<w2c:ProductImage ID="ProductImage2" ImageTagId="picture" ImageSize="LL" IsVariation="<%# HasVariation((DataRowView)Container.DataItem) %>" ProductMaster='<%# Container.DataItem %>' runat="server" />
	</a>
	
	<p style="margin-bottom: 5px;">
			<!-- 商品名 -->
				<p style="font-size: 0.8em"><%#:  DisplayLimit( (string)Eval("name"), 12) %></p>
			<div id="dvProductSubInfo" class="clearFix">
	</p>
	<p style="margin-bottom: 5px;">
	<div class="wrapProductPrice">
	<%-- ▽商品会員ランク価格有効▽ --%>
	<div id="Div2" visible='<%# ProductPage.GetProductMemberRankPriceValid(Container.DataItem, HasVariation((DataRowView)Container.DataItem)) %>' runat="server">
		<p class="productPrice"><span><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, HasVariation((DataRowView)Container.DataItem))) %></strike></span>(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>)</p>
		<p class="productPrice"><span><%#: CurrencyManager.ToPrice(ProductPage.GetProductMemberRankPrice(Container.DataItem, HasVariation((DataRowView)Container.DataItem))) %></span>(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>)</p>
	</div>
	<%-- △商品会員ランク価格有効△ --%>
	<%-- ▽商品セール価格有効▽ --%>
	<div id="Div3" visible='<%# ProductPage.GetProductTimeSalesValid(Container.DataItem, HasVariation((DataRowView)Container.DataItem)) %>' runat="server">
		<p class="productPrice"><span><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, HasVariation((DataRowView)Container.DataItem))) %></strike></span>(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>)</p>
		<p class="productPrice"><span><%#: CurrencyManager.ToPrice(ProductPage.GetProductTimeSalePriceNumeric(Container.DataItem)) %></span>(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>)</p>
	</div>
	<%-- △商品セール価格有効△ --%>
	<%-- ▽商品特別価格有効▽ --%>
	<div id="Div4" visible='<%# ProductPage.GetProductSpecialPriceValid(Container.DataItem, HasVariation((DataRowView)Container.DataItem)) %>' runat="server">
		<p class="productPrice"><span><strike><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, HasVariation((DataRowView)Container.DataItem))) %></strike></span>(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>)</p>
		<p class="productPrice"><span><%#: CurrencyManager.ToPrice(ProductPage.GetProductSpecialPriceNumeric(Container.DataItem, HasVariation((DataRowView)Container.DataItem))) %></span>(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>)</p>
	</div>
	<%-- △商品特別価格有効△ --%>
	<%-- ▽商品通常価格有効▽ --%>
	<div id="Div5" visible='<%# ProductPage.GetProductNormalPriceValid(Container.DataItem, HasVariation((DataRowView)Container.DataItem)) %>' runat="server">
		<p class="productPrice"><span><%#: CurrencyManager.ToPrice(ProductPage.GetProductPriceNumeric(Container.DataItem, HasVariation((DataRowView)Container.DataItem))) %></span>(<%# WebSanitizer.HtmlEncode(ProductPage.GetTaxIncludeString(Container.DataItem)) %>)</p>
	</div>
	<%-- △商品通常価格有効△ --%>
	</div>
	</p>
<p><asp:DropDownList ID="ddlVariationSelect1" CssClass="ddl" SelectedIndex="<%# GetSelectedIndex(Container.DataItem, 1) %>" DataSource='<%# GetDropDownList(Container.DataItem, 1) %>' DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server" /></p>
<p><asp:DropDownList ID="ddlVariationSelect2" CssClass="ddl" SelectedIndex="<%# GetSelectedIndex(Container.DataItem, 2) %>" DataSource='<%# GetDropDownList(Container.DataItem, 2) %>' DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server" Visible='<%# HasTwoAxes((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID))%>'/></p>
</li>
</ItemTemplate>
</asp:Repeater>
</div>
</ul>
</div>

<div id="Div6" class="ChangesByVariation" runat="server" style="padding-bottom: 10px;">
<div style="text-align: right;clear: left;padding-bottom: 10px;">
	<p class="error">
		<%# WebSanitizer.HtmlEncodeChangeToBr(this.AlertMessage) %><br/>
		<%# WebSanitizer.HtmlEncodeChangeToBr(this.ErrorMessage) %>
	</p>
</div>
	<% if((IsOneSelected()) && (string.IsNullOrEmpty(this.ErrorMessage))) { %>
	<div style="text-align: right;clear: left;"><asp:LinkButton ID="lbCartAdd" OnClick="lbCartAdd_Click" class="btn btn-mid btn-inverse" runat="server">まとめてカートに入れる</asp:LinkButton></div>
	<% }else{ %>
	<div style="text-align: right;clear: left;"><asp:LinkButton disabled class="btn btn-mid btn-inverse" runat="server">商品を選択してください</asp:LinkButton></div>
	<% }%>
</div>
<% } %>

<% if(this.Coordinate.TagList.Any()){ %>
<div style="padding-bottom: 20px;">
	<p style="padding-bottom: 10px;font-size: 1em; font-weight: bold;">タグ</p>
	<asp:Repeater ID="Repeater2" DataSource='<%# this.Coordinate.TagList %>' runat="server" >
		<ItemTemplate>
		<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_KEYWORD, ((ContentsTagModel)Container.DataItem).ContentsTagName))%>" style="text-decoration: underline;"><%#: ((ContentsTagModel)Container.DataItem).ContentsTagName %>&nbsp;</a>
		</ItemTemplate>
	</asp:Repeater>
</div>
<% } %>

<% if(this.Coordinate.CategoryList.Any()){ %>
<div style="padding-bottom: 20px;">
	<p style="padding-bottom: 10px;font-size: 1em; font-weight: bold;">カテゴリ</p>
	<asp:Repeater ID="Repeater4"  DataSource='<%# this.Coordinate.CategoryList %>' runat="server" >
		<ItemTemplate>
		<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateListUrl(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, ((CoordinateCategoryModel)Container.DataItem).CoordinateCategoryId))%>" style="text-decoration: underline;"><%#: ((CoordinateCategoryModel)Container.DataItem).CoordinateCategoryName %>&nbsp;</a>
		</ItemTemplate>
	</asp:Repeater>
</div>
<% } %>

</ContentTemplate>
</asp:UpdatePanel>
</div>
</div>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
<div id="divBottomArea" style="padding-bottom: 10px;">

<div id="StaffList" class="StaffList" runat="server">
<div class="MoreList" runat="server" Visible="<%# ShouldShowStaff(this.Coordinate) %>">
	<p class="title"><%#: this.Coordinate.StaffName %>のコーディネート</p>
	<asp:Repeater ID="Repeater5" DataSource='<%# this.CoordinateListOfSameStaff %>' runat="server" >
		<ItemTemplate>
			<div class="glbPlist column4">
				<ul>
					<li class="coordinatethumb">
						<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateDetailUrl(Container.DataItem))%>">
							<img src="<%#: CoordinatePage.CreateCoordinateImageUrl(((CoordinateModel)Container.DataItem).CoordinateId , 1) %>" />
						</a>
					</li>
					<li class="name">
						<a href="#" class="pid" >
							<img align="left" style="padding-right: 5px;" id="picture" src="<%#: CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="40px" border="0"/>
							<p style="padding-bottom: 2px;"><%#: ((CoordinateModel)Container.DataItem).StaffName %></p>
							<p style="padding-bottom: 2px;"><%#: ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
							<p style="padding-bottom: 2px;"><%#: ((CoordinateModel)Container.DataItem).RealShopName %></p>
						</a>
					</li>
				</ul>
			</div>
		</ItemTemplate>
	</asp:Repeater>
	<div style="padding-top: 10px;text-align: center; clear: left;">
		<% if(this.MaxDispCountOfSameStaff <= this.CoordinateListOfSameStaff.Count()){ %>
		<asp:LinkButton class="btn btn-mid btn-inverse" ID="AddSameStaff" OnClick="AddMaxDispCountOfSameStaff_Click" runat="server">もっと見る</asp:LinkButton>
		<% } %>
	</div>
</div>
</div>

<div id="RealShopList" class="RealShopList" runat="server">
<div class="MoreList" runat="server" Visible="<%# ShouldShowRealShop(this.Coordinate) %>">
	<p class="title">同じ店舗のコーディネート</p>
	<asp:Repeater ID="Repeater6" DataSource='<%# this.CoordinateListOfSameRealShop %>' runat="server" >
		<ItemTemplate>
			<div class="glbPlist column4">
				<ul>
					<li class="coordinatethumb">
						<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateDetailUrl(Container.DataItem))%>">
							<img src="<%#: CoordinatePage.CreateCoordinateImageUrl(((CoordinateModel)Container.DataItem).CoordinateId , 1) %>"/></a>
						</a>
					</li>
					<li class="name">
						<a href="#" class="pid" >
							<div runat="server" Visible="<%# ShouldShowStaff((CoordinateModel)Container.DataItem) %>">
								<img align="left" style="padding-right: 5px;" id="picture" src="<%#: CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="40px" border="0"/>
								<p style="padding-bottom: 2px;"><%#: ((CoordinateModel)Container.DataItem).StaffName %></p>
								<p style="padding-bottom: 2px;"><%#: ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
							</div>
							<p style="padding-bottom: 2px;"><%#: ((CoordinateModel)Container.DataItem).RealShopName %></p>
						</a>
					</li>
				</ul>
			</div>
		</ItemTemplate>
	</asp:Repeater>
	<div style="padding-top: 10px;text-align: center; clear: left;">
		<% if(this.MaxDispCountOfSameRealShop <= this.CoordinateListOfSameRealShop.Count()){ %>
		<asp:LinkButton class="btn btn-mid btn-inverse" ID="AddSameRealShop" OnClick="AddMaxDispCountOfSameRealShop_Click" runat="server">もっと見る</asp:LinkButton>
		<% } %>
	</div>
</div>
</div>

<div id="ProductList" class="ProductList" runat="server">
<div id="Div7" class="MoreList" runat="server">
<p class="title">同じ商品のコーディネート</p>
<asp:Repeater ID="SameProduct" DataSource='<%# this.CoordinateListOfSameProduct %>' runat="server" >
	<ItemTemplate>
		<div class="glbPlist column4">
			<ul>
				<li class="coordinatethumb">
					<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateCoordinateDetailUrl(Container.DataItem))%>">
						<img src="<%#: CoordinatePage.CreateCoordinateImageUrl(((CoordinateModel)Container.DataItem).CoordinateId , 1) %>" />
					</a>
					<li class="name">
						<a href="#" class="pid" >
							<div runat="server" Visible="<%# ShouldShowStaff((CoordinateModel)Container.DataItem) %>">
								<img align="left" style="padding-right: 5px;" id="picture" src="<%#: CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="40px" border="0"/>
								<p style="padding-bottom: 2px;"><%#: ((CoordinateModel)Container.DataItem).StaffName %></p>
								<p style="padding-bottom: 2px;"><%#: ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
							</div>
							<p style="padding-bottom: 2px;"><%#: ((CoordinateModel)Container.DataItem).RealShopName %></p>
						</a>
					</li>
				</li>
			</ul>
		</div>
	</ItemTemplate>
</asp:Repeater>
<div style="padding-top: 10px; text-align: center; clear: left;">
	<% if(this.MaxDispCountOfSameProduct <= this.CoordinateListOfSameProduct.Count()){ %>
	<asp:LinkButton class="btn btn-mid btn-inverse" ID="AddSameProduct" OnClick="AddMaxDispCountOfSameProduct_Click" runat="server">もっと見る</asp:LinkButton>
	<% } %>
</div>
</div>
</div>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- △編集可能領域△ --%>

<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>

<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
</asp:Content>