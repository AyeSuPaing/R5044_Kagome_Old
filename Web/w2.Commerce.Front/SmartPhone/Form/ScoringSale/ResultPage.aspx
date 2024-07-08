<%--
=========================================================================================================
  Module      : Result Page (ResultPage.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/ScoringSale/ResultPage.aspx.cs" Inherits="Form_ScoringSale_ResultPage" Title="Result Page" %>
<%@ Register TagPrefix="uc" TagName="AccessLogTrackerScript" Src="~/Form/Common/AccessLogTrackerScript.ascx" %>
<!DOCTYPE html>
<html lang="jp">
<head>
	<meta charset="UTF-8">
	<meta name="viewport" content="width=device-width, initial-scale=1.0">
	<title></title>
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Css/ScoringSale/base.css" />
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Css/ScoringSale/reset.css" />
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Css/ScoringSale/user.css" />
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Css/ScoringSale/scoringsale.css" />
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Js/slick/slick-theme.css" />
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Js/slick/slick.css" />
	<link rel ="preconnect" href="https://fonts.googleapis.com" />
	<link rel ="preconnect" href="https://fonts.gstatic.com" crossorigin />
	<script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.11.1.min.js"></script>
	<script type="text/javascript" src="<%= Constants.PATH_ROOT + "SmartPhone/Js/scoringsale_js.js" %>"></script>
	<script type="text/javascript" src="<%= Constants.PATH_ROOT + "SmartPhone/Js/slick/slick.min.js" %>"></script>
	<link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700&family=Noto+Sans+JP:wght@100;400;500;700&display=swap" rel="stylesheet">
	<style>
		.scoringsale_product_detail{
			padding-top: 20px;
			padding-bottom: 20px;
		}
	</style>
	<script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.js"></script>
</head>
<body>
	<form onsubmit="return (document.getElementById('__EVENTVALIDATION') != null);" runat="server">
		<div id="Wrap">
		<div class="wrapBottom">
			<div class="wrapTop">
				<div id="Contents">
					<div id="scoringsale" class="scoringsale colorPattern-1">
						<div class="scoringsale_inner">
							<div class="scoringsale_result">
								<h2 class="scoringsale_ttl"><%#: this.ScoringSale.ResultPageTitle %></h2>
								<% if (this.RadarChartUseFlag == Constants.FLG_SCORINGSALE_RADAR_CHART_USE_FLG_ON) { %>
								<canvas id="Chart"></canvas>
								<% } %>
								<div class="scoringsale_htmlcontent">
									<%# this.HtmlContentUp %>
								</div>
								<div class="scoringsale_product">
									<div class="scoringsale_product_flex">
										<div class="scoringsale_product_img">
											<ul class="scoringsale_product_img_main">
												<li>
													<w2c:ProductImage ID="ProductImage1" data-zoom-image="" ImageSize="LL" IsVariation="false" ProductMaster="<%# this.ProductMaster %>" runat="server" />
												</li>
												<asp:Repeater ID="Repeater1" DataSource="<%# this.ProductSubImageList %>" runat="server">
													<ItemTemplate>
														<li>
															<img
																class="zoomTarget"
																src="<%#: CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>"
																data-image="<%#: CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>"
																data-zoom-image="<%#: CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>" />
														</li>
													</ItemTemplate>
												</asp:Repeater>
											</ul>
											<ul class="scoringsale_product_img_thumb">
												<li>
													<w2c:ProductImage ID="ProductImage2" data-zoom-image="" ImageSize="LL" IsVariation="false" ProductMaster="<%# this.ProductMaster %>" runat="server" />
												</li>
												<asp:Repeater ID="Repeater2" DataSource="<%# this.ProductSubImageList %>" runat="server">
													<ItemTemplate>
														<li>
															<img
																class="zoomTarget"
																src="<%#: CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>"
																data-image="<%#: CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>"
																data-zoom-image="<%#: CreateProductSubImageUrl(this.ProductMaster, Constants.PRODUCTIMAGE_FOOTER_LL, (int)Eval(Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO)) %>" />
														</li>
													</ItemTemplate>
												</asp:Repeater>
											</ul>
										</div>
										<div class="scoringsale_product_info">
											<!-- 商品アイコン -->
											<ul class="scoringsale_product_info_icons">
											</ul>
											<!-- 商品名 -->
											<h2 class="scoringsale_product_info_name"><%# StringUtility.ToEmpty(this.ProductMaster[Constants.FIELD_PRODUCT_NAME]) %></h2>
											<div class="clearFix" style="margin-bottom: 10px">
												<p class="productDetailId">[<span class="productId"><%# StringUtility.ToEmpty(this.ProductMaster[Constants.FIELD_PRODUCT_PRODUCT_ID]) %>]</span></p>
											</div>
											<!-- 商品価格・税区分・加算ポイント -->
											<div class="scoringsale_product_info_price">
												<div id="Div1" class="display-price" visible='<%# GetProductMemberRankPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
													<p class="price_nomal">
														<%#: ProductPage.GetProductMemberRankPrice(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)).ToPriceString(true) %><span class="tax">(<%#: GetTaxIncludeString(this.ProductMaster) %>)</span>
													</p>
													<strike>
														<p class="tax display-price-discount">
															<%#: ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)).ToPriceString(true) %>
															(<%#: GetTaxIncludeString(this.ProductMaster) %>)
														</p>
													</strike>
												</div>
												<div id="Div2" class="display-price" visible='<%# GetProductTimeSalesValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
													<p class="price_nomal">
														<%#: ProductPage.GetProductTimeSalePriceNumeric(this.ProductMaster).ToPriceString(true) %><span class="tax">(<%#: GetTaxIncludeString(this.ProductMaster) %>)</span>
													</p>
													<strike>
														<p class="tax display-price-discount">
															<%#: ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)).ToPriceString(true) %>
															(<%#: GetTaxIncludeString(this.ProductMaster) %>)
														</p>
													</strike>
												</div>
												<div id="Div3" class="display-price" visible='<%# GetProductSpecialPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
													<p class="price_nomal">
														<%#: ProductPage.GetProductSpecialPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)).ToPriceString(true) %><span class="tax">(<%#: GetTaxIncludeString(this.ProductMaster) %>)</span>
													</p>
													<strike>
														<p class="tax display-price-discount">
															<%#: ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)).ToPriceString(true) %>
															(<%#: GetTaxIncludeString(this.ProductMaster) %>)
														</p>
													</strike>
												</div>
												<div id="Div4" class="display-price" visible='<%# GetProductNormalPriceValid(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)) %>' runat="server">
													<p>
														<%#: ProductPage.GetProductPriceNumeric(this.ProductMaster, (this.HasVariation == false) || (this.VariationSelected)).ToPriceString(true) %><span class="tax">(<%#: GetTaxIncludeString(this.ProductMaster) %>)</span>
													</p>
												</div>
											</div>
											<div class="scoringsale_product_info_select scoringsale_product_info_flex">
												<!-- 商品詳細1 -->
												<div class="scoringsale_product_detail">
													<p class="scoringsale_product_detail_tex">
														<%# GetProductDescription() %>
													</p>
												</div>
											</div>
											<div class="scoringsale_product_info_cartBtn">
												<asp:LinkButton ID="lbGoProductDetail" class="btn_cv" runat="server" Text="<%#: this.ScoringSale.ResultPageBtnCaption %>" Onclick="lbGoProductDetail_Click" />
											</div>
										</div>
									</div>
								</div>
								<div class="scoringsale_htmlcontent">
									<%# this.HtmlContentDown %>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
			<!--wrapTop-->
		</div>
		<!--wrapBottom-->
	</div>
	<script type="text/javascript">
		$(function () {
			// Set property of css
			var color = '<%= this.ScoringSale.ThemeColor %>';
			document.body.style.setProperty('--color-common', color);

			switch (color) {
				case '#DB6F39':
					document.body.style.setProperty('--color-cv', '#338F55');
					break;

				case '#000':
					document.body.style.setProperty('--color-cv', '#1073AA');
					break;

				case '#84AC27':
					document.body.style.setProperty('--color-cv', '#177138');
					break;

				case '#CE7272':
					document.body.style.setProperty('--color-cv', '#B41616');
					break;

				case '#0A56A3':
					document.body.style.setProperty('--color-cv', '#D3A200');
					break;
			}
		});

		function hexToRgb(hex) {
			return ['0x' + hex[1] + hex[2] | 0, '0x' + hex[3] + hex[4] | 0, '0x' + hex[5] + hex[6] | 0];
		}

		var color = '<%= this.ScoringSale.ThemeColor %>';
		var colorCode = hexToRgb(color);
		var color = 'RGBA(' + colorCode + ', 1)';
		var colorAlpha = 'RGBA(' + colorCode + ', 0.5';
		var chartElement = document.getElementById("Chart");
		if(chartElement)
		{
			var chart = new Chart(chartElement, {
				type: 'radar',
				data: {
					labels: <%=  this.ScoreAxisNames %>,
					datasets: [{
						label: '',
						data: <%= this.ScoreAxisDatas %>,
						backgroundColor: colorAlpha,
						borderColor: color,
						borderWidth: 1,
						pointBackgroundColor: color
					}]
				},
				options: {
					title: {
						display: true,
						text: '<%= this.RadarChartTitle %>'
					},
					scale: {
						ticks: {
							suggestedMin: 0,
							suggestedMax: 10,
							callback: function (value, index, values) {
								return value
							}
						}
					},
					legend: {
						display: false
					}
				}
			});
		}
	</script>
	</form>
	<%-- w2 access log tracker output --%>
	<uc:AccessLogTrackerScript runat="server" />
</body>
</html>