<%--
=========================================================================================================
  Module      : 注文配送先商品選択画面(OrderShippingSelectProduct.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%-- ▽ユーザーコントロール宣言領域▽ --%>
<%-- △ユーザーコントロール宣言領域△ --%>
<%@ Page Title="複数配送先情報入力ページ" Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderShippingSelectProduct.aspx.cs" Inherits="Form_Order_OrderShippingSelectProduct" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
<%@ FileInfo LayoutName="Default" %><%@ FileInfo LastChanged="最終更新者" %>

--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%-- ▽編集可能領域：HEAD追加部分▽ --%>
<%-- △編集可能領域△ --%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table id="tblLayout">
<tr>
<td>
<%-- ▽レイアウト領域：レフトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
<td>
<div id="divTopArea">
<%-- ▽レイアウト領域：トップエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

<%-- ▽編集可能領域：コンテンツ▽ --%>
<p id="CartFlow"></p>
<h2 class="ttlA">
<span class="btn_nxt_item"><a href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>"><small>次へ進む</small></a></span>
<span class="btn_back btn_back_item"><a onclick="<%= this.BackOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.BackEvent) %>"><small>戻る</small></a></span>
<em><img src="../../Contents/ImagesPkg/order/ttl_user_esd03.gif" alt="配送先情報選択" width="164" height="18" /></em>
</h2>

<p>配送先に関する情報を入力して下さい。<br /><span class="fred">※</span>&nbsp;は必須入力です。</p>
<br class="clr" />

<%-- 次へイベント用リンクボタン --%>
<asp:LinkButton ID="lbNext" OnClick="lbNext_Click" ValidationGroup="OrderShipping" runat="server"></asp:LinkButton>
<%-- 戻るイベント用リンクボタン --%>
<asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server"></asp:LinkButton>

<%-- UPDATE PANEL開始 --%>
<asp:UpdatePanel ID="upUpdatePanel" runat="server">
<ContentTemplate>

<small id="hcErrorMessage" enableviewstate="false" class="fred" runat="server"></small>

<asp:Repeater id="rCartList" Runat="server">
<ItemTemplate>
<%-- ▼配送先情報▼ --%>
<div class="orderBoxLarge">
<h3>
	<div class="cartNo">カート番号<%# Container.ItemIndex + 1 %><%# WebSanitizer.HtmlEncode(DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）"))%></div>
	<div class="cartLink"><a href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST) %>">カートへ戻る</a></div>
</h3>

<div class="bottom">
<asp:Repeater id="rCartShippings" DataSource="<%# ((CartObject)Container.DataItem).Shippings %>" runat="server">
<ItemTemplate>
<h4 visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" runat="server">
<div class="cartNo">
配送情報<%# Container.ItemIndex + 1 %>
</div>
<div class="cartLink">
<asp:LinkButton ID="lbShippingModify" OnClick="lbShippingModify_Click" OnClientClick="return confirm('前の画面に戻って変更しますか？')" runat="server">配送情報変更</asp:LinkButton>
</div>
</h4>

<%-- ▽送り主▽ --%>
<div id="divShippingInputForm" class="userListFloat orderBoxLarge list" visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" runat="server">
<h5>
送り主
（<%# WebSanitizer.HtmlEncode(GetOrderSenderAddrKbnString((CartShipping)Container.DataItem, "新規", "注文者"))%>）
</h5>

<div id="divSenderDisp" runat="server">
<dl>
<%-- 氏名 --%>
<dt><%: ReplaceTag("@@User.name.name@@") %></dt>

<dd><%# WebSanitizer.HtmlEncode(Eval("SenderName1")) %><%# WebSanitizer.HtmlEncode(Eval("SenderName2")) %>&nbsp;様
	<%#: (bool)(Eval("IsSenderAddrJp")) ? "（" + Eval("SenderNameKana1") + Eval("SenderNameKana2") + " さま）" : "" %><br />
</dd>
<dt>
	<%: ReplaceTag("@@User.addr.name@@") %>
</dt>
<dd>
<%# (bool)(Eval("IsSenderAddrJp")) ? WebSanitizer.HtmlEncode(Eval("SenderZip")) + "<br />" : "" %>
<%#: Eval("SenderAddr1") %> <%#: Eval("SenderAddr2") %><br />
<%#: Eval("SenderAddr3") %> <%#: Eval("SenderAddr4") %> <%#: Eval("SenderAddr5") %><br />
<%# ((bool)(Eval("IsSenderAddrJp")) == false) ? WebSanitizer.HtmlEncode(Eval("SenderZip")) + "<br />" : "" %>
<%#: Eval("SenderCountryName") %><br />
</dd>
<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
<%-- 企業名・部署名 --%>
<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
	<%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
<dd>
<%# WebSanitizer.HtmlEncode(Eval("SenderCompanyName"))%>&nbsp<%# WebSanitizer.HtmlEncode(Eval("SenderCompanyPostName"))%>
</dd>
<%} %>
<%-- 電話番号 --%>
<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
<dd>
<%#: Eval("SenderTel1") %>
</dd>
</dl>
</div>
</div><!--userListFloat-->
<%-- △送り主△ --%>

<%-- ▽配送先▽ --%>
<div id="divShippingInputForm2" class="orderBoxLarge userListFloat list" visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" runat="server">
<h5>
配送先（<%# WebSanitizer.HtmlEncode(GetOrderShippingAddrKbnString((CartShipping)Container.DataItem, "新規", "注文者")) %>）
</h5>

<%-- ▽配送先表示▽ --%>
<div>
<dl>
<%-- 氏名 --%>
<dt><%: ReplaceTag("@@User.name.name@@") %></dt>
<dd>
<%# WebSanitizer.HtmlEncode(Eval("Name1")) %> <%# WebSanitizer.HtmlEncode(Eval("Name2")) %>&nbsp;様
<%#: (bool)(Eval("IsShippingAddrJp")) ? "（" + Eval("NameKana1") + Eval("NameKana2") + " さま）" : ""%><br />
</dd>
<dt>
	<%: ReplaceTag("@@User.addr.name@@") %>
</dt>
<dd>
<%# (bool)(Eval("IsShippingAddrJp")) ? WebSanitizer.HtmlEncode(Eval("Zip")) + "<br />" : "" %>
<%#: Eval("Addr1") %> <%#: Eval("Addr2") %><br />
<%#: Eval("Addr3") %> <%#: Eval("Addr4") %> <%#: Eval("Addr5") %><br />
<%# ((bool)(Eval("IsShippingAddrJp")) == false) ? WebSanitizer.HtmlEncode(Eval("Zip")) + "<br />" : "" %>
<%#: Eval("ShippingCountryName") %>
</dd>
<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
<%-- 企業名・部署名 --%>
<dt><%: ReplaceTag("@@User.company_name.name@@")%>・
	<%: ReplaceTag("@@User.company_post_name.name@@")%></dt>
<dd>
<%# WebSanitizer.HtmlEncode(Eval("CompanyName"))%>&nbsp<%# WebSanitizer.HtmlEncode(Eval("CompanyPostName"))%>
</dd>
<%} %>
<%-- 電話番号 --%>
<dt><%: ReplaceTag("@@User.tel1.name@@") %></dt>
<dd>
<%#: Eval("Tel1") %>
</dd>
</dl>
</div>
<%-- △配送先表示△ --%>

</div><!--userListFloat-->
<%-- △配送先△ --%>

<br class="clr" />

<%-- ▽商品▽ --%>
<div class="orderBoxLarge userProductBox">
<h5>商品</h5>
<div id="hcAllocateProductToShipping" class="userProductTitle" visible="false" runat="server">
<%--asp:DropDownList ID="ddlProducts" runat="server"></asp:DropDownList>&nbsp;--%>
下記より配送する商品を選択してください。
<span id="hcNoAlloacatedProductMessage" style="color:Red" visible="false" runat="server">&nbsp;[配送する商品が割り当てられていません]</span><br />
<asp:HiddenField ID="hfProductListFormat" Value='<%# "{0}[{2}] {1}" %>' runat="server" />
<asp:ListBox ID="lbProducts" Width="580" Rows="3"   runat="server"></asp:ListBox>
<asp:LinkButton ID="lbAllocateProductToShipping" OnClick="lbAllocateProductToShipping_Click" runat="server">配送先に商品を追加</asp:LinkButton>
</div>
<%-- ▽該当商品一覧▽ ※商品一覧はポストバックで更新されるためDataSourceは裏で指定 --%>
<div class="userProduct">
<asp:Repeater id="rAllocatedProducts" Runat="server">
<ItemTemplate>
<div class="<%# (((IList)((Repeater)Container.Parent).DataSource).Count == Container.ItemIndex + 1) ? "last" : "" %>">
<dl>
<dt><w2c:ProductImage ID="ProductImage1" ProductMaster="<%# ((CartShipping.ProductCount)Container.DataItem).Product %>" ImageSize="S" runat="server" /></dt>
<dd><strong><%# WebSanitizer.HtmlEncode(((CartShipping.ProductCount)Container.DataItem).Product.ProductJointName)%></strong>
<%# (((CartShipping.ProductCount)Container.DataItem).Product.GetProductTag("tag_cart_product_message").Length != 0) ? "<small>" + WebSanitizer.HtmlEncode(((CartShipping.ProductCount)Container.DataItem).Product.GetProductTag("tag_cart_product_message")) + "</small>" : ""%>
<p id="P1" visible='<%#((CartShipping.ProductCount)Container.DataItem).Product.ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server"><b>
<asp:Repeater ID="rProductOptionSettings" DataSource='<%# ((CartShipping.ProductCount)Container.DataItem).Product.ProductOptionSettingList %>' runat="server">
	<ItemTemplate>
		<%# WebSanitizer.HtmlEncode(((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue()) %>
		<%# (((ProductOptionSetting)Container.DataItem).GetDisplayProductOptionSettingSelectValue() != "") ? "<br />" : "" %>
	</ItemTemplate>
</asp:Repeater>
</b></p>

<span id="Span1" visible="<%# FindCart(Container.DataItem).IsGift %>" runat="server">
<p class="quantity">数量 <asp:TextBox ID="tbProductCount" Runat="server" Text="<%# ((CartShipping.ProductCount)Container.DataItem).Count %>" MaxLength="3"></asp:TextBox>
	 x <%#: CurrencyManager.ToPrice(StringUtility.ToNumeric(((CartShipping.ProductCount)Container.DataItem).Product.Price)) %>
	<% if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED) { %>
	<span runat="server" Visible="<%# ((CartShipping.ProductCount)Container.DataItem).Product.ProductOptionSettingList.HasOptionPrice %>"><%#: " + " + CurrencyManager.ToPrice(((CartShipping.ProductCount)Container.DataItem).Product.TotalOptionPrice) %></span>
	<% } %>
	 = <%#: CurrencyManager.ToPrice(StringUtility.ToNumeric((((CartShipping.ProductCount)Container.DataItem).Product.Price + ((CartShipping.ProductCount)Container.DataItem).Product.TotalOptionPrice) * ((CartShipping.ProductCount)Container.DataItem).Count)) %>
	<asp:LinkButton ID="lbRecalculateCart" OnClick="lbRecalculateCart_Click" Runat="server"></asp:LinkButton></p>
<p id="P2" class="delete" runat="server"><asp:LinkButton ID="lReleaseProductAllocation" OnClick="lReleaseProductAllocation_Click" Runat="server">商品割り当て解除</asp:LinkButton></p>
</span>
<span id="Span2" visible="<%# FindCart(Container.DataItem).IsGift == false %>" runat="server">
<p class="quantity">数量 <%# ((CartShipping.ProductCount)Container.DataItem).Count %></p>
</span>

</dl>
<p class="clr"><img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1"  /></p>
</div>
</ItemTemplate>
</asp:Repeater>
<small id="hcErrorMessage" enableviewstate="false" class="fred" runat="server"></small>
</div><!--userProduct-->

<%-- △該当商品一覧△ --%>
</div>
<%-- △商品△ --%>

<%-- ▽配送指定▽ --%>
<div id="Div2" runat="server" visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" class="userListFloat orderBoxLarge list">
<h5 id="H1" visible="<%# CanInputShippingTo(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>" runat="server">配送指定</h5>
<div>
	配送方法を選択して下さい。
	<asp:DropDownList ID="ddlShippingMethod" DataSource="<%# this.ShippingMethodList[((RepeaterItem)Container.Parent.Parent).ItemIndex] %>" OnSelectedIndexChanged="ddlShippingMethodList_OnSelectedIndexChanged" DataTextField="text" DataValueField="value" AutoPostBack="true" runat="server"></asp:DropDownList>
</div>
<div id="dvDeliveryCompany" visible="<%# CanDisplayDeliveryCompany(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
<br />
	配送サービスを選択して下さい。
	<asp:DropDownList ID="ddlDeliveryCompany" DataSource="<%# GetDeliveryCompanyListItem(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" OnSelectedIndexChanged="ddlDeliveryCompanyList_OnSelectedIndexChanged" DataTextField="Value" DataValueField="Key" AutoPostBack="true" runat="server"/>
</div>
<br />
<div id="dvShipppingDateTime" visible="<%# CanInputDateOrTimeSet(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server" style='<%# HasFixedPurchase(Container) && (DisplayFixedPurchaseShipping(Container) == false) ? "padding-bottom: 0px" : "" %>'>
配送希望日時を選択して下さい。
<dl id="dlShipppingDateTime" runat="server">
<dt id="dtShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">配送希望日</dt>
<dd id="ddShippingDate" visible="<%# CanInputDateSet(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">
	<asp:DropDownList id="ddlShippingDate" CssClass="input_border" runat="server" DataSource="<%# GetShippingDateList(((CartShipping)Container.DataItem), this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex]) %>" DataTextField="text" DataValueField="value" SelectedValue="<%# GetShippingDate((CartShipping)Container.DataItem, this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex]) %>"
		OnSelectedIndexChanged="ddlFixedPurchaseShippingDate_OnCheckedChanged" AutoPostBack="true"></asp:DropDownList>
	<br />
	<asp:Label ID="lShippingDateErrorMessage" CssClass="fred" runat="server"></asp:Label>
</dd>
<dt id="dtShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server">配送希望時間帯</dt>
<dd id="ddShippingTime" visible="<%# CanInputTimeSet(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" runat="server" class="last">
	<asp:DropDownList id="ddlShippingTime" runat="server" DataSource="<%# GetShippingTimeList(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>" DataTextField="Text" DataValueField="Value" SelectedValue="<%# GetShippingTime(((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>"></asp:DropDownList>
</dd>
</dl>
</div>
</div><!--userList2-->
<%-- △配送指定△ --%>

<%-- ▽Invoice▽ --%>
<div style="min-height:134px" id="sInvoices" class="userListFloat orderBoxLarge list" runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo(((CartShipping)Container.DataItem).ShippingCountryIsoCode) %>">
	<h5>電子発票</h5>
		<dl id="divUniformInvoiceType" runat="server">
			<dd>発票種類</dd>
				<dl>
					<dd>
						<asp:DropDownList ID="ddlUniformInvoiceType" runat="server"
							CssClass="input_border"
							DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE) %>"
							DataTextField="text"
							DataValueField="value"
							OnSelectedIndexChanged="ddlUniformInvoiceType_SelectedIndexChanged"
							AutoPostBack="true">
						</asp:DropDownList>
						<asp:DropDownList ID="ddlUniformInvoiceTypeOption" runat="server"
							CssClass="input_border"
							DataTextField="text"
							DataValueField="value"
							OnSelectedIndexChanged="ddlUniformInvoiceTypeOption_SelectedIndexChanged"
							AutoPostBack="true"
							Visible="false">
						</asp:DropDownList>
					</dd>
				</dl>
				<dl id="dlUniformInvoiceOption1_8" runat="server" visible="false">
					<br />
					<dd>統一編号</dd>
					<dd>
						<asp:TextBox ID="tbUniformInvoiceOption1_8" placeholder="例:12345678" Text="<%# ((CartShipping)Container.DataItem).UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="8"/>
						<asp:CustomValidator
							ID="cvUniformInvoiceOption1_8" runat="server"
							ControlToValidate="tbUniformInvoiceOption1_8"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							CssClass="error_inline" />
						<asp:Label ID="lbUniformInvoiceOption1_8" runat="server" Text="<%# ((CartShipping)Container.DataItem).UniformInvoiceOption1 %>" Visible="false"></asp:Label>
					</dd>
					<br />
					<dd>会社名</dd>
					<dd>
						<asp:TextBox ID="tbUniformInvoiceOption2" placeholder="例:○○有限股份公司" Text="<%# ((CartShipping)Container.DataItem).UniformInvoiceOption2 %>" Width="220" runat="server" MaxLength="20"/>
						<asp:CustomValidator
							ID="cvUniformInvoiceOption2" runat="server"
							ControlToValidate="tbUniformInvoiceOption2"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							CssClass="error_inline" />
						<asp:Label ID="lbtbUniformInvoiceOption2" runat="server" Text="<%# ((CartShipping)Container.DataItem).UniformInvoiceOption2 %>" Visible="false"></asp:Label>
					</dd>
				</dl>

				<dl id="dlUniformInvoiceOption1_3" runat="server" visible="false">
					<br />
					<dd>寄付先コード</dd>
					<dd>
						<asp:TextBox ID="tbUniformInvoiceOption1_3" Text="<%# ((CartShipping)Container.DataItem).UniformInvoiceOption1 %>" Width="220" runat="server" MaxLength="7" />
						<asp:CustomValidator
							ID="cvUniformInvoiceOption1_3" runat="server"
							ControlToValidate="tbUniformInvoiceOption1_3"
							ValidationGroup="OrderShippingGlobal"
							ValidateEmptyText="true"
							ClientValidationFunction="ClientValidate"
							SetFocusOnError="true"
							CssClass="error_inline" />
						<asp:Label ID="lbUniformInvoiceOption1_3" Text="<%# ((CartShipping)Container.DataItem).UniformInvoiceOption1 %>" runat="server" Visible="false"></asp:Label>
					</dd>
				</dl>
				<dl id="dlUniformInvoiceTypeRegist" runat="server" visible="false">
					<dd>
						<asp:CheckBox ID="cbSaveToUserInvoice" Checked="<%# ((CartShipping)Container.DataItem).UserInvoiceRegistFlg %>" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
					</dd>
					<dl id="dlUniformInvoiceTypeRegistInput" runat="server" visible="false">
						<dd>
							電子発票情報名<span class="fred">※</span>
							<asp:TextBox ID="tbUniformInvoiceTypeName" Text="<%# ((CartShipping)Container.DataItem).InvoiceName %>" MaxLength="30" runat="server"></asp:TextBox>
							<asp:CustomValidator
								ID="cvUniformInvoiceTypeName" runat="server"
								ControlToValidate="tbUniformInvoiceTypeName"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								CssClass="error_inline" />
						</dd>
					</dl>
				</dl>
		</dl>
		<dl id="divInvoiceCarryType" runat="server">
			<dd>共通性載具</dd>
				<dl>
					<dd>
						<asp:DropDownList ID="ddlInvoiceCarryType" runat="server"
							CssClass="input_border"
							DataSource="<%# ValueText.GetValueItemList(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE) %>"
							DataTextField="text"
							DataValueField="value"
							OnSelectedIndexChanged="ddlInvoiceCarryType_SelectedIndexChanged"
							AutoPostBack="true"
							Width="250"></asp:DropDownList>
					</dd>
					<dd>
					</dd>
					<dd>
						<asp:DropDownList ID="ddlInvoiceCarryTypeOption" runat="server"
							CssClass="input_border"
							DataTextField="text"
							DataValueField="value"
							OnSelectedIndexChanged="ddlInvoiceCarryTypeOption_SelectedIndexChanged"
							AutoPostBack="true"
							Visible="false">
						</asp:DropDownList>
					</dd>
					<dd>
					</dd>
					<dd>
					</dd>
					<dd id="divCarryTypeOption" runat ="server">
						<div id="divCarryTypeOption_8" runat="server" visible="false">
							<asp:TextBox ID="tbCarryTypeOption_8" Width="220" runat="server" Text="<%# ((CartShipping)Container.DataItem).CarryTypeOptionValue %>" placeholder="例:/AB201+9(限8個字)" MaxLength="8" />
							<asp:CustomValidator
								ID="cvCarryTypeOption_8"
								runat="server"
								ControlToValidate="tbCarryTypeOption_8"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								CssClass="error_inline" />
						</div>
						<div id="divCarryTypeOption_16" runat="server" visible="false">
							<asp:TextBox ID="tbCarryTypeOption_16" Width="220" Text="<%# ((CartShipping)Container.DataItem).CarryTypeOptionValue %>" runat="server" placeholder="例:TP03000001234567(限16個字)" MaxLength="16" />
							<asp:CustomValidator
								ID="cvCarryTypeOption_16"
								runat="server"
								ControlToValidate="tbCarryTypeOption_16"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								CssClass="error_inline" />
						</div>
					</dd>
					<dd>
						<asp:Label runat="server" ID="lbCarryTypeOptionText" Visible="false" Text="載具コード"></asp:Label>
					</dd>
					<dd>
						<asp:Label runat="server" ID="lbCarryTypeOption" Visible="false"></asp:Label>
					</dd>
					<dl id="dlCarryTypeOptionRegist" runat="server" visible="false">
						<dd>
							<asp:CheckBox ID="cbCarryTypeOptionRegist" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true" OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
						</dd>
						<dd id="divCarryTypeOptionName" runat="server" visible="false">
							電子発票情報名<span class="fred">※</span>
							<asp:TextBox ID="tbCarryTypeOptionName" Text="<%# ((CartShipping)Container.DataItem).InvoiceName %>" runat="server" MaxLength="30"></asp:TextBox>
							<asp:CustomValidator
								ID="cvCarryTypeOptionName" runat="server"
								ControlToValidate="tbCarryTypeOptionName"
								ValidationGroup="OrderShippingGlobal"
								ValidateEmptyText="true"
								ClientValidationFunction="ClientValidate"
								SetFocusOnError="true"
								CssClass="error_inline" />
						</dd>
					</dl>
				</dl>
		</dl>
	</div>
<%-- △Invoice△ --%>

<%-- ▽定期購入配送パターン▽--%>
<div id="Div3" visible="<%# (FindCart(Container.DataItem)).HasFixedPurchase %>" runat="server" class="userListFloat orderBoxLarge list">
<%-- ▽デフォルトチェックの設定▽--%>
<%-- ラジオボタンのデータバインド <%#.. より前で呼び出してください。 --%>
<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG ? SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container.Parent.Parent).ItemIndex, 3, 2, 1, 4) : SetFixedPurchaseDefaultCheckPriority(((RepeaterItem)Container.Parent.Parent).ItemIndex, 2, 3, 1, 4) %>
<%-- △ - - - - - - - - - - - △--%>
<h5 id="H2" style="margin-top:15px" visible="<%# (FindCart(Container.DataItem)).HasFixedPurchase && DisplayFixedPurchaseShipping(Container) %>" runat="server">定期購入 配送パターンの指定</h5>
	<span visible="<%# DisplayFixedPurchaseShipping(Container) %>" runat="server">「定期購入」はご希望の配送パターン・配送時間を指定して定期的に商品をお届けするサービスです。下記の配送パターンからお選び下さい。</span>

	<dl style="margin-top: 10px;" visible="<%# DisplayFixedPurchaseShipping(Container) %>" runat="server">
		<dt id="Dt1" style="width: 130px" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
			<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_Date" 
				Text="月間隔日付指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 1) %>" 
				GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /></dt>
		<dd id="ddFixedPurchaseMonthlyPurchase_Date" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">　
			<asp:DropDownList ID="ddlFixedPurchaseMonth"
				DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
				runat="server">
			</asp:DropDownList>
			ヶ月ごと
			<asp:DropDownList ID="ddlFixedPurchaseMonthlyDate"
				DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true, false, true) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, "MonthlyDate") %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
				runat="server">
			</asp:DropDownList>
			日に届ける
		<small><asp:CustomValidator runat="Server" 
			ControlToValidate="ddlFixedPurchaseMonth" 
			ValidationGroup="OrderShipping" 
			ValidateEmptyText="true" 
			SetFocusOnError="true" 
			CssClass="error_inline" />
		</small>
		<small><asp:CustomValidator ID="CustomValidator15" runat="Server" 
			ControlToValidate="ddlFixedPurchaseMonthlyDate" 
			ValidationGroup="OrderShipping" 
			ValidateEmptyText="true" 
			SetFocusOnError="true" 
			CssClass="error_inline" />
		</small>
		</dd>
		<dt id="Dt2" style="width: 130px" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">
			<asp:RadioButton ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay" 
				Text="月間隔・週・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 2) %>" 
				GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /></dt>
		<dd id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">　
			<asp:DropDownList ID="ddlFixedPurchaseIntervalMonths"
				DataSource="<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true, true) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" runat="server" />
			ヶ月ごと
			<asp:DropDownList ID="ddlFixedPurchaseWeekOfMonth"
				DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, "WeekOfMonth") %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
				runat="server">
			</asp:DropDownList>
			<asp:DropDownList ID="ddlFixedPurchaseDayOfWeek"
				DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, "DayOfWeek") %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
				runat="server">
			</asp:DropDownList>
			に届ける
		<small><asp:CustomValidator runat="Server"
			ControlToValidate="ddlFixedPurchaseIntervalMonths"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			CssClass="error_inline" />
		</small>
		<small><asp:CustomValidator ID="CustomValidator16" runat="Server" 
			ControlToValidate="ddlFixedPurchaseWeekOfMonth" 
			ValidationGroup="OrderShipping" 
			ValidateEmptyText="true" 
			SetFocusOnError="true" 
			CssClass="error_inline" />
		</small>
		<small><asp:CustomValidator ID="CustomValidator17" runat="Server" 
			ControlToValidate="ddlFixedPurchaseDayOfWeek" 
			ValidationGroup="OrderShipping" 
			ValidateEmptyText="true" 
			SetFocusOnError="true" 
			CssClass="error_inline" />
		</small>
		</dd>
		<dt id="Dt3" style="width: 130px" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
			<asp:RadioButton ID="rbFixedPurchaseRegularPurchase_IntervalDays" 
				Text="配送日間隔指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 3) %>" 
				GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /></dt>
		<dd id="ddFixedPurchaseRegularPurchase_IntervalDays" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">　
			<asp:DropDownList ID="ddlFixedPurchaseIntervalDays"
				DataSource='<%# GetFixedPurchaseIntervalDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false) %>'
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, "IntervalDays") %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true" 
				runat="server">
			</asp:DropDownList>
			日ごとに届ける
		<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%# this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
		<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%# this.ShopShippingList[((RepeaterItem)Container.Parent.Parent).ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
		<small><asp:CustomValidator ID="CustomValidator18" runat="Server" 
			ControlToValidate="ddlFixedPurchaseIntervalDays" 
			ValidationGroup="OrderShipping" 
			ValidateEmptyText="true" 
			SetFocusOnError="true" 
			CssClass="error_inline" />
		</small>
		</dd>
		<dt id="Dt4" style="width: 130px" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
			<asp:RadioButton ID="rbFixedPurchaseEveryNWeek"
				Text="週間隔・曜日指定" Checked="<%# GetFixedPurchaseKbnInputChecked(((RepeaterItem)Container.Parent.Parent).ItemIndex, 4) %>"
				GroupName="FixedPurchaseShippingPattern" OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged" AutoPostBack="true" runat="server" /></dt>
		<dd id="ddFixedPurchaseEveryNWeek" visible="<%# GetFixedPurchaseShippingPaternEnabled(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" runat="server">
			<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week"
				DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, true) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
				runat="server">
			</asp:DropDownList>
			週間ごと
			<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
				DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(((RepeaterItem)Container.Parent.Parent).ItemIndex, false) %>"
				DataTextField="Text" DataValueField="Value" SelectedValue='<%# GetFixedPurchaseSelectedValue(((RepeaterItem)Container.Parent.Parent).ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
				OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged" AutoPostBack="true"
				runat="server">
			</asp:DropDownList>
			に届ける
		</dd>
		<small><asp:CustomValidator
			ID="cvFixedPurchaseEveryNWeek"
			runat="Server"
			ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			CssClass="error_inline"/>
		</small>
		<small><asp:CustomValidator
			ID="cvFixedPurchaseEveryNWeekDayOfWeek"
			runat="Server"
			ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
			ValidationGroup="OrderShipping"
			ValidateEmptyText="true"
			SetFocusOnError="true"
			CssClass="error_inline"/>
		</small>
	</dl>
	<small ID="sErrorMessage" class="fred" runat="server"></small>
	<br />
	<dl>
		<dt id="dtFirstShippingDate" style="width: 130px" visible="true" runat="server">初回配送予定日</dt>
		<dd visible="true" runat="server" style="padding-left: 20px;">
			<asp:Label ID="lblFirstShippingDate" runat="server"></asp:Label>
			<asp:Label ID="lblFirstShippingDateNoteMessage" visible="false" runat="server">
				<br>配送予定日は変更となる可能性がありますことをご了承ください。
			</asp:Label>
			<asp:Literal ID="lFirstShippingDateDayOfWeekNoteMessage" visible="false" runat="server">
				<br>曜日指定は次回配送日より適用されます。
			</asp:Literal>
		</dd>
		<dt id="dtNextShippingDate" style="width: 130px" visible="true" runat="server">2回目の配送日を選択</dt>
		<dd visible="true" runat="server" style="padding-left: 20px;">
			<asp:Label ID="lblNextShippingDate" visible="false" runat="server"></asp:Label>
			<asp:DropDownList ID="ddlNextShippingDate" visible="false" OnDataBound="ddlNextShippingDate_OnDataBound" runat="server"></asp:DropDownList>
		</dd>
	</dl>
</div>
<%-- △定期購入配送パターン△ --%>

<%-- ▽のし情報▽--%>
<div visible="<%# (FindCart(Container.DataItem)).IsGift && (GetWrappingPaperFlgValid(((RepeaterItem)Container.Parent.Parent).ItemIndex) || GetWrappingBagFlgValid(((RepeaterItem)Container.Parent.Parent).ItemIndex)) %>" runat="server" class="userListFloat orderBoxLarge list">
<h5>
のし・包装情報
&nbsp;&nbsp;<asp:LinkButton ID="lbCopyWrappingInfoToOtherShippings" OnClick="lbCopyWrappingInfoToOtherShippings_Click" runat="server">他の配送先にコピー</asp:LinkButton>
</h5>
<dl visible='<%# GetWrappingPaperFlgValid(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' runat="server">
<dt>のし</dt>
<dd>
種類 <asp:DropDownList ID="ddlWrappingPaperType" DataSource='<%# GetWrappingPaperTypes(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' SelectedValue='<%# ((CartShipping)Container.DataItem).WrappingPaperType %>' DataTextField="text" DataValueField="value" runat="server"></asp:DropDownList>
</dd>
<dd>
差出人<asp:TextBox ID="tbWrappingPaperName" Text="<%# ((CartShipping)Container.DataItem).WrappingPaperName %>" MaxLength="200" Width="200" runat="server"></asp:TextBox>
</dd>
</dl>
<dl visible='<%# GetWrappingBagFlgValid(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' runat="server">
<dt>包装</dt>
<dd>
種類 <asp:DropDownList ID="ddlWrappingBagType" DataSource='<%# GetWrappingBagTypes(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' SelectedValue='<%# ((CartShipping)Container.DataItem).WrappingBagType %>' DataTextField="text" DataValueField="value" runat="server"></asp:DropDownList>
</dd>
</dl>
</div>
<%-- △のし情報△ --%>

<br class="clr" />

</ItemTemplate>
</asp:Repeater>

<%-- ▽注文メモ▽--%>
<div id="Div4" visible="<%# ((CartObject)Container.DataItem).OrderMemos.Count != 0 %>" runat="server" class="userListWide orderBoxLarge list">
<asp:Repeater ID="rMemos" runat="server" DataSource="<%# ((CartObject)Container.DataItem).OrderMemos %>">
<HeaderTemplate>
	<h5>注文メモ</h5>
	<div class="inner">
</HeaderTemplate>
<ItemTemplate>
	<strong><%# WebSanitizer.HtmlEncode(Eval(CartOrderMemo.FIELD_ORDER_MEMO_NAME)) %></strong><br />
	<asp:TextBox ID="tbMemo"  runat="server" Text="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_TEXT) %>" CssClass="<%# Eval(CartOrderMemo.FIELD_ORDER_MEMO_CSS) %>" TextMode="MultiLine"></asp:TextBox><br /><br />
	<small id="sErrorMessageMemo" runat="server" class="fred" ></small>
</ItemTemplate>
<FooterTemplate>
	</div>
</FooterTemplate>
</asp:Repeater>
</div>
<br class="clr" />
<%-- △注文メモ△ --%>

</div><!--bottom-->
</div><!--orderBoxLarge-->
<%-- ▲配送先情報▲ --%>

</ItemTemplate>
</asp:Repeater>

<%-- UpdatePanel外のイベントを実行したいためこのような呼び出し方となっている --%>
<p class="btmbtn btn_nxt_item right"><a href="<%= WebSanitizer.HtmlEncode(this.NextEvent) %>"><small>次へ進む</small></a></p>
<p class="btmbtn btn_back_item right"><a onclick="<%= this.BackOnClick %>" href="<%= WebSanitizer.HtmlEncode(this.BackEvent) %>"><small>戻る</small></a></p>

<br class="clr" />


</ContentTemplate>
</asp:UpdatePanel>
<%-- UPDATE PANELここまで --%>

<%-- △編集可能領域△ --%>

<div id="divBottomArea">
<%-- ▽レイアウト領域：ボトムエリア▽ --%>
<%-- △レイアウト領域△ --%>
</div>

</td>
<td>
<%-- ▽レイアウト領域：ライトエリア▽ --%>
<%-- △レイアウト領域△ --%>
</td>
</tr>
</table>
</asp:Content>

