<%--
=========================================================================================================
  Module      : スマートフォン用コーディネート詳細画面(CoordinateDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="~/Form/Coordinate/CoordinateDetail.aspx.cs" Inherits="Form_Coordinate_CoordinateDetail" Title="コーディネート詳細ページ" %>
<%@ Import Namespace="w2.Domain.Coordinate" %>

<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/product.css") %>" rel="stylesheet" type="text/css" media="all" />
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>SmartPhone/Js/imagesloaded.pkgd.min.js"></script>
<script type="text/javascript" charset="UTF-8" src="<%= Constants.PATH_ROOT %>SmartPhone/Js/flipsnap.js"></script>

<script type="text/javascript" language="javascript">
	// 商品詳細：画像スライド
	//$(window).load(function(){
	$(function () {
		detailImageFlick();
	});

	var clickName;

	function detailImageFlick() {
		var len = $('.flipsnap .item').length,

			$pointer = $('.product-image-sub a'),
			$variation = $('.drop-down'),
			$targetDropDown,
			$targetPoint,
			$next = $('.product-image-detail .next').click(function () { flipsnap.toNext(); }),
			$prev = $('.product-image-detail .prev').click(function () { flipsnap.toPrev(); }),
			lenGap = len - $pointer.length;

		$('.flick-wrap').removeClass('loading');
		$('.flipsnap').css('visibility', 'visible');

		$('.flipsnap').css({ width: len * 304 });
		var flipsnap = Flipsnap('.flipsnap', {
			distance: 304
		});

		// ページング
		$pointer.each(function (i) {
			if ((i === 0) && (lenGap === 0)) { $(this).attr('class', 'current'); }
			$(this).attr('name', i);

		});

		flipsnap.element.addEventListener('fspointmove', function () {
			$pointer.filter('.current').removeClass('current');
			$pointer.eq(flipsnap.currentPoint - lenGap).addClass('current');
			var variationId = $pointer.eq(flipsnap.currentPoint - lenGap).data('variationid');
			$variation.find('option[value=' + variationId + ']').prop("selected", true);
			// メイン画像にスライドした時、サブ画像リストが選択された表記をしないようにする
			if ((lenGap !== 0) && (flipsnap.currentPoint === 0)) {
				$pointer.filter('.current').removeClass('current');
			}
		}, false);
		$pointer.click(
			function () {
				clickName = Number(this.name);
				flipsnap.moveToPoint(clickName + lenGap);
				$pointer.filter('.current').removeClass('current');
				$('.product-image-sub a[name=' + (clickName) + ']').addClass('current');
			}
		);
		// 前へ 次へ
		$('.product-image-detail .prev').attr('disabled', 'disabled');
		flipsnap.element.addEventListener('fspointmove', function () {
			$next.attr('disabled', !flipsnap.hasNext());
			$prev.attr('disabled', !flipsnap.hasPrev());
		}, false);
	}
</script>
<%-- △編集可能領域△ --%>

</asp:Content>

<%-- ▽編集可能領域：プロパティ設定▽ --%>
<script runat="server">
	public new void Page_Init(Object sender, EventArgs e)
	{
		base.Page_Init(sender, e);
		// 同じスタッフのコーディネートリスト設定
		this.MaxDispCountOfSameStaff = 2;
		this.AddDispCountOfSameStaff = 2;

		// 同じ店舗のコーディネートリスト設定
		this.MaxDispCountOfSameRealShop = 2;
		this.AddDispCountOfSameRealShop = 2;

		// 同じ商品のコーディネートリスト設定
		this.MaxDispCountOfSameProduct = 2;
		this.AddDispCountOfSameProduct = 2;
	}
</script>
<%-- △編集可能領域△ --%>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>

<%-- 表示更新の為のpageLoad --%>
<script type="text/javascript" language="javascript">
	function bodyPageLoad() {
		if (Sys.WebForms == null) return;
		var isAsyncPostback = Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();
		if (isAsyncPostback) {
			detailImageFlick(); //画像スライド
		}
	}
</script>
	
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<%-- タイトル --%>
<div class="breadcrumbs">
	<ul>
		<li>
			<a href="<%= Constants.PATH_ROOT %>Form/Coordinate/CoordinateTop.aspx">TOP</a><span>&ensp;＞&ensp;</span><%#: this.Coordinate.CoordinateTitle %>
		</li>
	</ul>
</div>

<div class="wrap-product-image">
<div class="accesstop_info"  style="position: relative;">
	<div runat="server" Visible="<%# ShouldShowStaff(this.Coordinate) %>">
		<h5 class="accesstop_brandlogo">
			<img align="left"  id="picture" src="<%#: this.StaffImagePath %>" alt="スタッフ画像" height="40px" border="0" style="padding-right: 5px"/>
		</h5>
		<h4 class="accesstop_brand"><%#: this.Coordinate.StaffName %></h4>
		<div class="accesstop_shop"><%#: this.Coordinate.RealShopName %> / <%#: this.Coordinate.StaffHeight %>cm</div>
	</div>

	<% if(this.Coordinate.DisplayDate != null){ %>
	<span class="accesstop_date"><%=DisplayDate(this.Coordinate.DisplayDate) %></span>
	<% } %>
	<asp:LinkButton style="z-index:11;position:absolute; top:65px; left:85px; width: 70px;" ID="lbLike" OnClick="lbLikeBtn_Click" runat="server">
		<img src="<%# CreateLikeImage() %>" width="20px;" height="20px;"/>&ensp;<%#: this.LikeCount %>
	</asp:LinkButton>
</div>
<%-- ▽メイン画像▽ --%>
<div class="product-image-detail">
<div class="flick-wrap loading">
	<div class="flipsnap">
		<%-- ▽画像一覧▽ --%>
		<asp:Repeater ID="Repeater2" DataSource="<%# this.CoordinateImages %>" runat="server">
		<ItemTemplate>
			<div id="Div2" class="item" runat="server">
				<img src="<%#: ((string)Container.DataItem) %>" alt="" />
			</div>
		</ItemTemplate>
		</asp:Repeater>
		<%-- △画像一覧△ --%>
	</div>
</div>
<div class="product-image-sub">
	<%--画像一覧--%>
	<asp:Repeater ID="Repeater3" DataSource="<%# this.CoordinateImages %>"  runat="server">
		<ItemTemplate>
			<a id="A1" href="javascript:void(0);"  runat="server">
			<img src="<%#: ((string)Container.DataItem) %>" alt="" /></a>
		</ItemTemplate>
	</asp:Repeater>
</div>
<div id="Div3" class="arrow" runat="server">
	<p class="prev">前へ</p>
	<p class="next">次へ</p>
</div>
</div>
</div>
	
<div id="dvProductDetailArea" class="ChangesByVariation" runat="server">
<div id="detailOne">
<div style="text-align: center; padding-top: 10px;padding-bottom: 10px;">
	<% if(this.IsFollowing){%>
	<div style="text-align: center; padding-top: 10px;padding-bottom: 10px;">
		<asp:LinkButton CssClass="afterfollow" onclick="lbFollowBtn_Click" Visible="<%# this.IsStaffValid %>" runat="server">フォロー中</asp:LinkButton>
	</div>
	<% }else{%>
	<div style="text-align: center; padding-top: 10px;padding-bottom: 10px;">
		<asp:LinkButton CssClass="beforefollow" onclick="lbFollowBtn_Click" Visible="<%# this.IsStaffValid %>" runat="server">フォローする</asp:LinkButton>
	</div>
	<% }%>
</div>

<%-- SNSボタン ※mixiチェックはクライアント毎にデベロッパ登録したキーを設定する必要あり --%>
<div class="sns">
	<%-- twitter --%>
	<a href="https://twitter.com/share" class="twitter-share-button" data-count="none" data-lang="ja">ツイート</a><script type="text/javascript" src="https://platform.twitter.com/widgets.js"></script>
	<%-- mixi --%>
	<a href="javascript:void(0);" onclick='<%# WebSanitizer.HtmlEncode("window.open('http://mixi.jp/share.pl?u=" + HttpUtility.UrlEncode(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_COORDINATE_DETAIL + "?" + Constants.REQUEST_KEY_COORDINATE_ID + "=" + this.CoordinateId + "&k=01ac61d95d41a50ea61d0c5ab84adf0cfbf62f7d") + "','share',['width=632','height=456','location=yes','resizable=yes','toolbar=no','menubar=no','scrollbars=no','status=no'].join(','));") %>'><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/mixi_bt_check_1.png" alt="mixiチェック" border="0" /></a>
	<%-- facebook --%>
	<iframe src="//www.facebook.com/plugins/like.php?href=<%# HttpUtility.UrlEncode(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_COORDINATE_DETAIL + "?" + Constants.REQUEST_KEY_COORDINATE_ID + "=" + this.CoordinateId) %>&amp;send=false&amp;layout=button_count&amp;width=450&amp;show_faces=false&amp;action=like&amp;colorscheme=light&amp;font=tahoma&amp;height=21" scrolling="no" frameborder="0" style="border:none; overflow:hidden; width:100px; height:21px;" allowTransparency="true"></iframe>
	<% if(string.IsNullOrEmpty(this.Coordinate.StaffInstagramId) == false){ %>
		<div style="padding: 10px 0px 5px 0px;"><p style="font-size: 1em; font-weight: bold;">Instagram :@<a target="_blank" href="https://www.instagram.com/<%#: this.Coordinate.StaffInstagramId %>"><%#: this.Coordinate.StaffInstagramId %></a></p></div>
	<% } %>
</div>

<div class="coodinate_itemdetail" style="background-color: #eee; padding-top: 12px;">
	<%# WebSanitizer.HtmlEncodeChangeToBr(this.Coordinate.CoordinateSummary) %>
</div>

<% if (this.SelectedProductList.Any()) { %>
<div class="coodinate_itemphoto" style="padding-bottom: 10px;">
<h5 class="coodinate_subtitle">このコーディネートに使っているアイテム</h5>
<ul id="itemContainer">
<asp:Repeater OnItemCreated="RepeaterItemCreated" DataSource='<%# this.CoordinateProductList %>' runat="server" >
<ItemTemplate>
<li class="clearfix" >
	<p class="item_photo">
	<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CreateProductDetailUrl(Container.DataItem, HasVariation(Container.DataItem))) %>' target="_blank">
		<w2c:ProductImage ID="ProductImage2" ImageTagId="picture" ImageSize="L" IsVariation="<%# HasVariation(Container.DataItem) %>" ProductMaster='<%# Container.DataItem %>' runat="server" />
	</a>
	<h5 class="coodinate_item"><%#:  DisplayLimit( (string)Eval("name"), 8) %></h5>
	<p class="coodinate_price">
	<span class="item_price">
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
	</span>
	</p>
	<p style="padding-top: 3px; padding-bottom: 3px;"><asp:DropDownList ID="ddlVariationSelect1" CssClass="ddl" SelectedIndex="<%# GetSelectedIndex(Container.DataItem, 1) %>" DataSource='<%# GetDropDownList(Container.DataItem, 1) %>' DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server" /></p>
	<p style="padding-top: 3px; padding-bottom: 3px;"><asp:DropDownList ID="ddlVariationSelect2" CssClass="ddl" SelectedIndex="<%# GetSelectedIndex(Container.DataItem, 2) %>" DataSource='<%# GetDropDownList(Container.DataItem, 2) %>' DataTextField="Text" DataValueField="Value" OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" AutoPostBack="True" runat="server" Visible='<%# HasTwoAxes((string)GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID))%>'/></p>
	</p>
</li>
	</ItemTemplate>
</asp:Repeater>
</ul>
</div>
<% } %>
	
<div id="Div7" class="wrap-buyable" style="padding-bottom: 20px;" runat="server">
<div class="button add-to-cart">
<div style="text-align: right;clear: left;">
	<p class="error" style="padding-bottom: 10px; color: red;">
		<%# WebSanitizer.HtmlEncodeChangeToBr(this.AlertMessage) %><br/>
		<%# WebSanitizer.HtmlEncodeChangeToBr(this.ErrorMessage) %>
	</p>
</div>
<% if(IsOneSelected()) { %>
	<div style="text-align: right;clear: left;"><asp:LinkButton ID="lbCartAdd" OnClick="lbCartAdd_Click" CssClass="btn" runat="server">まとめてカートに入れる</asp:LinkButton></div>
<% }else{ %>
	<div style="text-align: right;clear: left;"><asp:LinkButton disabled CssClass="btn" runat="server">商品を選択してください</asp:LinkButton></div>
<% }%>
</div>
</div>
</div>
</div>

</ContentTemplate>
</asp:UpdatePanel>

<asp:UpdatePanel ID="UpdatePanel2" runat="server">
<ContentTemplate>
	<div id="StaffList" class="StaffList" runat="server" Visible="<%# ShouldShowStaff(this.Coordinate) %>">
	<section class="search unit">
		<h3><%#: this.Coordinate.StaffName %>のコーディネート</h3>
			<div style="padding-top: 10px">
				<asp:Repeater ID="Repeater1" DataSource='<%# this.CoordinateListOfSameStaff %>' runat="server">
					<HeaderTemplate>
						<ul class="product-list-2 clearfix">
					</HeaderTemplate>
					<ItemTemplate>
						<li>
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CoordinatePage.CreateCoordinateDetailUrl(Container.DataItem)) %>'>
								<div class="product-image">
								<img src="<%#: CoordinatePage.CreateCoordinateImageUrl((((CoordinateModel)Container.DataItem).CoordinateId), 1) %>"/></a>
						</div>
						</a>
						<div class="product-name">
							<img style="padding-right: 5px;" align="left"  id="picture" src="<%#: CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="60px" border="0"/>
							<p><%#: ((CoordinateModel)Container.DataItem).StaffName %></p>
							<p><%#: ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
							<p><%#: ((CoordinateModel)Container.DataItem).RealShopName %></p>
						</div>
						</li>
					</ItemTemplate>
					<FooterTemplate>
					</ul>
					</FooterTemplate>
				</asp:Repeater>
				<div class="view-more">
					<% if(this.MaxDispCountOfSameStaff <= this.CoordinateListOfSameStaff.Count()){ %>
					<asp:LinkButton class="btn" ID="AddSameStaff" OnClick="AddMaxDispCountOfSameStaff_Click" runat="server">もっと見る</asp:LinkButton>
					<% } %>
				</div>
			</div>
	</section>
	</div>
	
	<div id="RealShopList" class="RealShopList" runat="server" Visible="<%# ShouldShowRealShop(this.Coordinate) %>">
	<section class="search unit">
		<h3>同じ店舗のコーディネート</h3>
			<div style="padding-top: 10px">
				<asp:Repeater ID="Repeater4" DataSource='<%# this.CoordinateListOfSameRealShop %>' runat="server">
					<HeaderTemplate>
						<ul class="product-list-2 clearfix">
					</HeaderTemplate>
					<ItemTemplate>
						<li>
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CoordinatePage.CreateCoordinateDetailUrl(Container.DataItem)) %>'>
								<div class="product-image">
								<img src="<%#: CoordinatePage.CreateCoordinateImageUrl((((CoordinateModel)Container.DataItem).CoordinateId), 1) %>"/></a>
						</div>
						</a>
						<div class="product-name">
							<div runat="server" Visible="<%# ShouldShowStaff((CoordinateModel)Container.DataItem) %>">
								<img style="padding-right: 5px;" align="left"  id="picture" src="<%#: CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="60px" border="0"/>
								<p><%#: ((CoordinateModel)Container.DataItem).StaffName %></p>
								<p><%#: ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
							</div>
							<p><%#: ((CoordinateModel)Container.DataItem).RealShopName %></p>
						</div>
						</li>
					</ItemTemplate>
					<FooterTemplate>
					</ul>
					</FooterTemplate>
				</asp:Repeater>
				<div class="view-more">
					<% if(this.MaxDispCountOfSameRealShop <= this.CoordinateListOfSameRealShop.Count()){ %>
					<asp:LinkButton class="btn" ID="LinkButton1" OnClick="AddMaxDispCountOfSameRealShop_Click" runat="server">もっと見る</asp:LinkButton>
					<% } %>
				</div>
			</div>
	</section>
	</div>
	
	<div id="ProductList" class="ProductList" runat="server">
	<section class="search unit">
		<h3>同じ商品のコーディネート</h3>
			<div style="padding-top: 10px">
				<asp:Repeater ID="Repeater5" DataSource='<%# this.CoordinateListOfSameProduct %>' runat="server">
					<HeaderTemplate>
						<ul class="product-list-2 clearfix">
					</HeaderTemplate>
					<ItemTemplate>
						<li>
							<a href='<%# WebSanitizer.UrlAttrHtmlEncode(CoordinatePage.CreateCoordinateDetailUrl(Container.DataItem)) %>'>
								<div class="product-image">
								<img src="<%#: CoordinatePage.CreateCoordinateImageUrl((((CoordinateModel)Container.DataItem).CoordinateId), 1) %>"/></a>
						</div>
						</a>
						<div class="product-name">
							<div runat="server" Visible="<%# ShouldShowStaff((CoordinateModel)Container.DataItem) %>">
								<img style="padding-right: 5px;" align="left"  id="picture" src="<%#: CoordinatePage.GetStaffImagePath(((CoordinateModel)Container.DataItem).StaffId) %>" alt="スタッフ画像" height="60px" border="0"/>
								<p><%#: ((CoordinateModel)Container.DataItem).StaffName %></p>
								<p><%#: ((CoordinateModel)Container.DataItem).StaffHeight %>cm</p>
							</div>
							<p><%#: ((CoordinateModel)Container.DataItem).RealShopName %></p>
						</div>
						</li>
					</ItemTemplate>
					<FooterTemplate>
					</ul>
					</FooterTemplate>
				</asp:Repeater>
				<div class="view-more">
					<% if(this.MaxDispCountOfSameProduct <= this.CoordinateListOfSameProduct.Count()){ %>
					<asp:LinkButton class="btn" ID="LinkButton2" OnClick="AddMaxDispCountOfSameProduct_Click" runat="server">もっと見る</asp:LinkButton>
					<% } %>
				</div>
			</div>
	</section>
	</div>

	</ContentTemplate>
</asp:UpdatePanel>
<%-- △編集可能領域△ --%>
<%-- UPDATE PANELここまで --%>
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</asp:Content>