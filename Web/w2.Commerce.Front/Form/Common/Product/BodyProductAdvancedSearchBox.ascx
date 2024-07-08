<%--
=========================================================================================================
  Module      : 商品詳細検索ボックス出力コントローラ(BodyProductAdvancedSearchBox.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductAdvancedSearchBox.ascx.cs" Inherits="Form_Common_Product_BodyProductAdvancedSearchBox" %>
<%@ Import Namespace="ProductListDispSetting" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
<%
	// 検索テキストボックスEnterで検索させる（UpdatePanelで括っておかないと非同期処理時に検索が効かなくなる）
	this.SetAttributes();
%>
</ContentTemplate>
</asp:UpdatePanel>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<div id="dvProductAdvancedSearch" runat="server" class="dvProductAdvancedSearch">
<div class="title"><h3><img src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/icn_advanceSearch.png">条件を指定して検索</h3></div>
<div class="toogle">
<table>
<tbody>
<tr>
	<th>フリーワード</th>
	<td class="sort-word">
		<w2c:ExtendedTextBox ID="tbSearchWord" type="search" runat="server" MaxLength="250" placeholder="検索キーワード"></w2c:ExtendedTextBox>
	</td>
	<th>カテゴリー</th>
	<td class="sort-category">
		<asp:DropDownList ID="ddlCategories" Runat="server"></asp:DropDownList>
	</td>
</tr>
<tr>
	<th>カラー</th>
	<td>
		<asp:DropDownList ID="ddlColors" runat="server" Style="display:none;" DataTextField="text" DataValueField="value"></asp:DropDownList>
		<asp:Repeater runat="server" ID="rColors" DataSource="<%# ProductColorUtility.GetProductColorList() %>" ItemType="w2.App.Common.Product.ProductColor">
			<HeaderTemplate><div class="categoryList" style="display: inline-block;"></HeaderTemplate>
			<ItemTemplate>
				<img ID="<%# "iColor" + Item.Id %>" name="iColor" data-color='<%#: Item.Id %>' src='<%#: Item.Url %>' width="35" Height="35" Style="padding: 4px 4px 4px 4px;" />
			</ItemTemplate>
			<FooterTemplate></div></FooterTemplate>
		</asp:Repeater>
	</td>
</tr>
<tr>
	<th>価格帯</th>
	<td colspan="3">
		<ul class="sort-price">
		<% if (this.CurrentCurrencyCode == "JPY") { %>
		<li><input id="discount1" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true) %> /><label for="discount1">指定なし</label></li>
		<li><input id="discount2" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 0m, 3000m) %> /><label for="discount2">～ 3,000円</label></li>
		<li><input id="discount3" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 3000m, 5000m) %> /><label for="discount3">3,000円 ～ 5,000円</label></li>
		<li><input id="discount4" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 5000m, 8000m) %> /><label for="discount4">5,000円 ～ 8,000円</label></li>
		<li><input id="discount5" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 8000m, 10000m) %> /><label for="discount5">8,000円 ～ 10,000円</label></li>
		<li><input id="discount6" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 10000m, 15000m) %> /><label for="discount6">10,000円 ～ 15,000円</label></li>
		<li><input id="discount7" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 15000m, 20000m) %> /><label for="discount7">15,000円 ～ 20,000円</label></li>
		<li><input id="discount8" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 20000m, 30000m) %> /><label for="discount8">20,000円 ～ 30,000円</label></li>
		<li><input id="discount9" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, minPrice: 30000m) %> /><label for="discount9">30,000円 ～</label></li>
		<%} else if (this.CurrentCurrencyCode == "USD") { %>
		<li><input id="discount1" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true) %> /><label for="discount1">指定なし</label></li>
		<li><input id="discount2" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 0m, 30m) %> /><label for="discount2">～ $30.00</label></li>
		<li><input id="discount3" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 30m, 50m) %> /><label for="discount3">$30.00 ～ $50.00</label></li>
		<li><input id="discount4" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 50m, 80m) %> /><label for="discount4">$50.00 ～ $80.00</label></li>
		<li><input id="discount5" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 80m, 100m) %> /><label for="discount5">$80.00 ～ $100.00</label></li>
		<li><input id="discount6" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 100m, 150m) %> /><label for="discount6">$100.00 ～ $150.00</label></li>
		<li><input id="discount7" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 150m, 200m) %> /><label for="discount7">$150.00 ～ $200.00</label></li>
		<li><input id="discount8" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, 200m, 300m) %> /><label for="discount8">$200.00 ～ $300.00</label></li>
		<li><input id="discount9" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(true, minPrice: 300m) %> /><label for="discount9">$300.00 ～</label></li>
		<%} else { %>
		<li><input id="discount1" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(false) %> /><label for="discount1">指定なし</label></li>
		<li><input id="discount2" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(false, 0m, 3000m) %> /><label for="discount2">～ <%: CurrencyManager.ToPrice(3000m) %></label></li>
		<li><input id="discount3" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(false, 3000m, 5000m) %> /><label for="discount3"><%: CurrencyManager.ToPrice(3000m) %> ～ <%: CurrencyManager.ToPrice(5000m) %></label></li>
		<li><input id="discount4" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(false, 5000m, 8000m) %> /><label for="discount4"><%: CurrencyManager.ToPrice(5000m) %> ～ <%: CurrencyManager.ToPrice(8000m) %></label></li>
		<li><input id="discount5" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(false, 8000m, 10000m) %> /><label for="discount5"><%: CurrencyManager.ToPrice(8000m) %> ～ <%: CurrencyManager.ToPrice(10000m) %></label></li>
		<li><input id="discount6" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(false, 10000m, 15000m) %> /><label for="discount6"><%: CurrencyManager.ToPrice(10000m) %> ～ <%: CurrencyManager.ToPrice(15000m) %></label></li>
		<li><input id="discount7" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(false, 15000m, 20000m) %> /><label for="discount7"><%: CurrencyManager.ToPrice(15000m) %> ～ <%: CurrencyManager.ToPrice(20000m) %></label></li>
		<li><input id="discount8" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(false, 20000m, 30000m) %> /><label for="discount8"><%: CurrencyManager.ToPrice(20000m) %> ～ <%: CurrencyManager.ToPrice(30000m) %></label></li>
		<li><input id="discount9" type="radio" name="price" <%: CreateAttributesForPriceSearchControl(false, minPrice: 30000m) %> /><label for="discount9"><%: CurrencyManager.ToPrice(30000m) %> ～</label></li>
		<%} %>
		</ul>
	</td>
</tr>
<% if (this.DisplayStockFilter) { %>
<asp:Repeater ID="rStockList" runat="server">
	<HeaderTemplate>
		<tr>
		<th>在庫</th>
		<td colspan="3">
		<%-- 在庫有無 --%>
		<ul class="horizon">
	</HeaderTemplate>
	<ItemTemplate>
		<li>
			<input id="udns<%# ((ProductListDispSettingModel)Container.DataItem).SettingId %>" name="udns" type="radio" value="<%# ((ProductListDispSettingModel)Container.DataItem).SettingId %>" <%#: (((StringUtility.ToEmpty(Request["udns"]) == "") && (((ProductListDispSettingModel)Container.DataItem).SettingId == "0")) || StringUtility.ToEmpty(Request["udns"]) == ((ProductListDispSettingModel)Container.DataItem).SettingId) ? "checked" : "" %> />
			<label for="udns<%# ((ProductListDispSettingModel)Container.DataItem).SettingId %>"><%#: ((ProductListDispSettingModel)Container.DataItem).SettingName %></label>
		</li>
	</ItemTemplate>
	<FooterTemplate>
		</ul>
		</td>
		</tr>
	</FooterTemplate>
</asp:Repeater>
<% } %>
<% if (this.DisplayFixedPurchaseFilter) {%>
<tr>
	<th>通常・定期</th>
	<td colspan="3">
	<%-- 通常・定期 --%>
	<ul class="horizon">
	<li>
		<input id="fpfl0" name=<%: Constants.FORM_NAME_FIXED_PURCHASE_RADIO_BUTTON %> type="radio" value="0" <%= ((StringUtility.ToEmpty(Request["fpfl"]) == "0") || (StringUtility.ToEmpty(Request["fpfl"]) == "")) ? "checked" : "" %> />
		<label for="fpfl0">すべて表示</label>
	</li>
	<li>
		<input id="fpfl1" name=<%: Constants.FORM_NAME_FIXED_PURCHASE_RADIO_BUTTON %> type="radio" value="1" <%= (StringUtility.ToEmpty(Request["fpfl"]) == "1") ? "checked" : "" %> />
		<label for="fpfl1">通常購入可能</label>
	</li>
	<li>
		<input id="fpfl2" name=<%: Constants.FORM_NAME_FIXED_PURCHASE_RADIO_BUTTON %> type="radio" value="2" <%= (StringUtility.ToEmpty(Request["fpfl"]) == "2") ? "checked" : "" %> />
		<label for="fpfl2">定期購入可能</label>
	</li>
	</ul>
	</td>
</tr>
<% } %>
<tr>
	<th>セール</th>
	<td>
		<ul class="horizon">
			<li>
				<input id="sfl" name=<%: Constants.FORM_NAME_SALEFILTER_CHECKBOX %> type="checkbox" value="1" <%= (Request["sfl"] == "1") ? "checked" : "" %> />
				<label for="sfl">セールのみ対象</label>
			</li>
		</ul>
	</td>
</tr>

<% if (this.DisplaySubscriptionBoxFilter) {%>
	<tr>
		<th>頒布会検索</th>
		<td class="sort-word">
			<w2c:ExtendedTextBox ID="tbSubscriptionBoxSearchWord" type="search" runat="server" MaxLength="250" placeholder="キーワード"></w2c:ExtendedTextBox>
		</td>
	</tr>
<% } %>

<%-- For option brand enabled --%>
<% if (this.DisplayBrandFilter) { %>
<tr>
	<th>ブランド</th>
	<td colspan="3">
		<ul class="sort-price">
			<asp:HiddenField ID="hfBrandId" Value="" runat="server" />
			<asp:Repeater ID="rBrandList" runat="server">
				<ItemTemplate>
				<li>
					<input id="<%#: ((DataRowView)Container.DataItem)[Constants.FIELD_PRODUCTBRAND_BRAND_ID] %>" name="iBrand" type="radio"
						value="<%#: ((DataRowView)Container.DataItem)[Constants.FIELD_PRODUCTBRAND_BRAND_ID] %>" />
					<label for="<%#: ((DataRowView)Container.DataItem)[Constants.FIELD_PRODUCTBRAND_BRAND_ID] %>">
						<%#: ((DataRowView)Container.DataItem)[Constants.FIELD_PRODUCTBRAND_BRAND_NAME] %></label>
				</li>
				</ItemTemplate>
			</asp:Repeater>
		</ul>
	</td>
</tr>
<% } %>
</tbody>
</table>
<div class="button">
	<asp:LinkButton ID="lbSearch" runat="server" OnClick="lbSearch_Click" CssClass="btn-sort-search btn btn-mid btn-inverse">絞り込む</asp:LinkButton>
</div>
</div>
</div>

<script>
	$(".dvProductAdvancedSearch h3").on("click", function () {
		$(".toogle").slideToggle('slow');
	});

	var requestColor = "<%# this.ProductColorId %>";
	$("[name=iColor]").each(function (i) {
		e = $("#" + $("[name=iColor]")[i].id);
		if ($("[name=iColor]")[i].getAttribute("data-color") == requestColor) {
			e.css('outline', 'thin solid #000000');
		}
		e.on('click', function (elem) {
			var data = elem.target.getAttribute("data-color");
			var ddl = $("#<%# ddlColors.ClientID %>");
			if (data != ddl.val()) {
				ddl.val(data);
				$(this).css('outline', 'thin solid #000000');
			} else {
				ddl.val("");
				$(this).css('outline', '');
			}
			$("[name=iColor]").each(function (i) {
				var e2 = $("#" + $("[name=iColor]")[i].id);
				if ($("[name=iColor]")[i].getAttribute("data-color") != data) {
					e2.css('outline', '');
				}
			});
		});
	});

	// For option search brand enabled
	<% if (this.HasControlAndOptionBrandEnabled) { %>
	$("[name=iBrand]").each(function (index, element) {
		// Event click for each input radio buttons and set value for hidden field brand id
		$("#" + element.id).click(function () {
			var valueChoose = $("#" + element.id).val();
			$("#" + "<%= this.WhfBandId.ClientID %>").val(valueChoose);
		});

		// Set attribute checked for input type radio buttons
		if ($("#" + element.id).val() == $("#" + "<%= this.WhfBandId.ClientID %>").val()) {
			$("#" + element.id).prop("checked", true);
		} else {
			$("#" + element.id).removeAttr("checked", false);
		}
	});
	<% } %>
</script>
<%-- △編集可能領域△ --%>
