<%--
=========================================================================================================
  Module      : リアル店舗商品在庫一覧出力コントローラ(BodyRealShopProductStockList.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyRealShopProductStockList.ascx.cs" Inherits="Form_Common_Product_BodyRealShopProductStockList" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="ｗ２ユーザー" %>

--%>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
base.Page_Init(sender, e);

<%-- ▽編集可能領域：プロパティ設定▽ --%>
//リアル店舗IDを設定します。（カンマ区切りで複数指定可）
//＜例＞0001,0002
//this.RealShopId = "";

//郵便番号を設定します。（カンマ区切りで複数指定可）
//＜例＞111-1111,222-2222
//this.Zip = "";

//都道府県を設定します。（カンマ区切りで複数指定可）
//＜例＞茨城県,栃木県,群馬県,埼玉県,千葉県,東京都,神奈川県
//※"関東"などのエリア絞込みで利用を想定。
//this.Addr1 = "";

//在庫有の店舗のみかどうかを設定します。（true:在庫有の店舗のみ,false:全ての店舗）
//this.InStock = false;
<%-- △編集可能領域△ --%>
}
</script>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<% if (this.RealShop.Count != 0){ %>
<p class="attention">※表示されている情報は、<%= DateTimeUtility.ToStringFromRegion(this.OldestDateChanged, DateTimeUtility.FormatType.ShortDate2Letter) %>閉店時点のものになります。</p>
<% }else{ %>
<p class="attention">※取扱いの店舗はありません。</p>
<% } %>

<%-- ▽リアル店舗商品在庫一覧ループ▽ --%>
<asp:Repeater ID="rRealShopList" runat="server">
	<HeaderTemplate>
		<table class="shopList">
			<thead>
				<tr>
					<th>ショップ</th>
					<td>住所 / 電話番号 / 営業時間</td>
					<td>在庫状況</td>
				</tr>
				<tbody>
			</thead>
	</HeaderTemplate>
	<ItemTemplate>
				<tr>
					<th width="200">
						<div visible='<%# (string)GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_URL) != "" %>' runat="server"><a href="<%# WebSanitizer.UrlAttrHtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_URL)) %>" target="_shopurl"><%# WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_NAME)) %></a></div>
						<div visible='<%# (string)GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_URL) == "" %>' runat="server"><%#: GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_NAME) %></div>
					</th>
					<td width="400">
						<%# IsCountryJp((string)GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_COUNTRY_ISO_CODE))
								? "〒" + WebSanitizer.HtmlEncode(GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_ZIP)) + "<br/>"
								: "" %>
						<%#: GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_ADDR1) %><%#: GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_ADDR2) %><%#: GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_ADDR3) %><%#: GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_ADDR4) %><%#: GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_ADDR5) %>
						<%#: (IsCountryJp((string)GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_COUNTRY_ISO_CODE)) == false)
								? GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_ZIP)
								: "" %>
						<%#: GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_COUNTRY_NAME) %>
						<%#: GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_TEL) %><br/>
						<%#: GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOP_OPENING_HOURS) %>
					</td>
					<td class="stock"><%# (int)GetKeyValue(Container.DataItem, Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK) > 0 ? "在庫有り" : "在庫なし" %></td>
				</tr>
	</ItemTemplate>
	<FooterTemplate>
				</tbody>
			</talbe>
	</FooterTemplate>
</asp:Repeater>
<%-- △リアル店舗商品在庫一覧ループ△ --%>
<%-- △編集可能領域△ --%>