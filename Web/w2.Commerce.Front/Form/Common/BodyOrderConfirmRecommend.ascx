<%--
=========================================================================================================
  Module      : Body Order Confirm Recommend (BodyOrderConfirmRecommend.ascx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/BodyOrderConfirmRecommend.ascx.cs" Inherits="Form_Common_BodyOrderConfirmRecommend" %>
<%@ Register TagPrefix="uc" TagName="BodyRecommend" Src="~/Form/Common/BodyRecommend.ascx" %>
<%@ Register TagPrefix="uc" TagName="BodyFixedPurchaseOrderPrice" Src="~/Form/Common/BodyFixedPurchaseOrderPrice.ascx" %>
<%@ Register TagPrefix="uc" TagName="PaymentDescriptionCvsDef" Src="~/Form/Common/Order/PaymentDescriptionCvsDef.ascx" %>

<%
	this.WlbComplete.OnClientClick = (this.HideOrderButtonWithClick)
		? "return exec_submit(true)"
		: "return exec_submit(false)";
%>
<script type="text/javascript">
	var submitted = false;
	var isMyPage = null;
	var completeButton = null;

	function exec_submit(clearSubmitButton) {
		completeButton = document.getElementById('<%= lbCompleteAfterComfirmPayment.ClientID %>');

		if (submitted === false) {
			<% if (Constants.PRODUCT_ORDER_LIMIT_ENABLED) { %>
				var confirmMessage = '<%= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT) %>' + "\nよろしいですか？";
				<% if (this.HasOrderHistorySimilarShipping) { %>
					if (confirm(confirmMessage) === false) return false;
				<% } %>
			<% } %>
		}

		if (submitted) return false;

		submitted = true;

		<% if (this.WrCartList.Items.Count >= 1) { %>
			if (clearSubmitButton) {
				if (document.getElementById('<%= this.WlbComplete.ClientID %>') != null) {
					document.getElementById('<%= this.WlbComplete.ClientID %>').style.display = "none";
				}

				if (document.getElementById('processing2') != null) {
					document.getElementById('processing2').style.display = "inline";
				}
			}
		<% } %>

		return true;
	}
</script>
<div id="modalRecommend" style="display: none;">
	<div class="modal-content">
		<div class="btmbtn above cartstep">
			<h2 class="ttlA">&nbsp;&nbsp;ご注文内容確認</h2>
			<ul>
				<li>
					<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal == false) { %>
					<asp:LinkButton
						class="btn-recommend-close"
						style="font-weight: 600;"
						runat="server"
						OnClientClick="return closeModalRecommend()"
						Text="X" />
					<span id="processing1" style="display: none">
						<center>
							<strong>
								ただいま決済処理中です。<br />画面が切り替わるまでそのままお待ちください。
							</strong>
						</center>
					</span>
					<% } %>
				</li>
			</ul>
		</div>
		<div id="CartList">
			<asp:Repeater ID="rCartList" runat="server" OnItemCommand="rCartList_ItemCommand">
				<ItemTemplate>
					<div class="main">
						<div class="submain">
							<%-- ▼注文内容▼ --%>
							<div class="column">
								<div>
									<h2>
										<img src="../../Contents/ImagesPkg/order/sttl_cash_confirm.gif" alt="注文情報" width="64" height="16" />
									</h2>
								</div>
								<div class="orderBox">
									<h3>カート番号<%#: (Container.ItemIndex + 1) %>
										<%#: DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）") %>
									</h3>
									<div class="bottom">
										<div class="box">
											<em>本人情報確認</em>
											<div>
												<dl>
													<%-- 氏名 --%>
													<dt>
														<%: ReplaceTag("@@User.name.name@@") %>：
													</dt>
													<dd><%#: GetCartOwner(Container.DataItem).Name1 %><%#: GetCartOwner(Container.DataItem).Name2 %>&nbsp;様</dd>
													<% if ((this.IsAmazonCv2Guest == false)
														|| (Constants.AMAZON_PAYMENT_CV2_ENABLED && Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED)) { %>
													<%-- 氏名（かな） --%>
													<div visible="<%# ((string.IsNullOrEmpty(GetCartOwner(Container.DataItem).NameKana) == false)) %>" runat="server">
														<dt visible="<%# GetCartOwner(Container.DataItem).IsAddrJp %>" runat="server">
															<%: ReplaceTag("@@User.name_kana.name@@") %>：
														</dt>
														<dd visible="<%# GetCartOwner(Container.DataItem).IsAddrJp %>" runat="server">
															<%#: GetCartOwner(Container.DataItem).NameKana1 %><%#: GetCartOwner(Container.DataItem).NameKana2 %>&nbsp;さま
														</dd>
													</div>
													<dt>
														<%: ReplaceTag("@@User.mail_addr.name@@") %>：
													</dt>
													<% } %>
													<dd>
														<%#: GetTextDisplay(GetCartOwner(Container.DataItem).MailAddr, true) %>
													</dd>
													<% if ((this.IsAmazonCv2Guest == false)
														|| (Constants.AMAZON_PAYMENT_CV2_ENABLED && Constants.AMAZON_PAYMENT_CV2_USE_SHIPPING_AS_OWNER_ENABLED)) { %>
													<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
													<dt>
														<%: ReplaceTag("@@User.mail_addr2.name@@") %>：
													</dt>
													<dd>
														<%#: GetTextDisplay(GetCartOwner(Container.DataItem).MailAddr2, true) %>
													</dd>
													<br />
													<% } %>
													<dt>
														<%: ReplaceTag("@@User.addr.name@@") %>：
													</dt>
													<dd>
														<p>
															<%# WebSanitizer.HtmlEncodeChangeToBr(GetAddressDisplayText(GetCartOwner(Container.DataItem))) %>
														</p>
													</dd>
													<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
													<dt>
														<%: ReplaceTag("@@User.company_name.name@@") %>・
														<%: ReplaceTag("@@User.company_post_name.name@@") %>：
													</dt>
													<dd>
														<%#: GetCartOwner(Container.DataItem).CompanyName %><br />
														<%#: GetCartOwner(Container.DataItem).CompanyPostName %>
													</dd>
													<% } %>
													<%-- 電話番号 --%>
													<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
													<dd><%#: GetCartOwner(Container.DataItem).Tel1 %></dd>
													<dt><%: ReplaceTag("@@User.tel2.name@@") %>：</dt>
													<dd><%#: GetCartOwner(Container.DataItem).Tel2 %>&nbsp;</dd>
													<% } %>
													<dt><%: ReplaceTag("@@User.mail_flg.name@@") %></dt>
													<dd><%#: ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, GetCartOwner(Container.DataItem).StatusRequestDeliveryOfNotificationMail) %>
														<br />
														&nbsp;
													</dd>
												</dl>
												<p class="clr">
													<img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" />
												</p>
											</div>
										</div>
										<!--box-->
										<asp:Repeater ID="rCartShippings" DataSource='<%# Eval("Shippings") %>' runat="server">
											<ItemTemplate>
												<div class="box">
													<em>
														配送情報
														<span visible="<%# FindCart(Container.DataItem).IsGift %>" runat="server"><%#: (Container.ItemIndex + 1) %></span>
													</em>
													<div>
														<dl>
															<div runat="server" visible="<%# GetCartShipping(Container.DataItem).IsConvenienceStoreFlagOn %>">
																<span style="color: red; display: block;">
																	<asp:Literal ID="lShippingCountryErrorMessage" runat="server" />
																</span>
																</br>
																<dt>店舗ID：</dt>
																<dd><%#: GetCartShipping(Container.DataItem).ConvenienceStoreId %>&nbsp;</dd>
																<dt>店舗名称：</dt>
																<dd><%#: GetCartShipping(Container.DataItem).Name1 %>&nbsp;</dd>
																<dt>店舗住所：</dt>
																<dd><%#: GetCartShipping(Container.DataItem).Addr4 %>&nbsp;</dd>
																<dt>店舗電話番号：</dt>
																<dd><%#: GetCartShipping(Container.DataItem).Tel1 %>&nbsp;</dd>
															</div>
															<span visible="<%# IsStorePickupDisplayed(Container.DataItem) %>" runat="server">
																<dt>
																	<%: ReplaceTag("@@User.addr.name@@") %>：
																</dt>
																<dd>
																	<p>
																		<%# WebSanitizer.HtmlEncodeChangeToBr(GetAddressDisplayText(GetCartShipping(Container.DataItem))) %>
																	</p>
																</dd>
																<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
																<dt>
																	<%: ReplaceTag("@@User.company_name.name@@") %>・
																	<%: ReplaceTag("@@User.company_post_name.name@@") %>：
																</dt>
																<dd>
																	<%#: Eval("CompanyName") %>&nbsp<%#: Eval("CompanyPostName") %>
																</dd>
																<% } %>
																<%-- 氏名 --%>
																<dt><%: ReplaceTag("@@User.name.name@@") %>：</dt>
																<dd><%#: Eval("Name1") %><%#: Eval("Name2") %>&nbsp;様</dd>
																<%-- 氏名（かな） --%>
																<div visible="<%# ((string.IsNullOrEmpty(GetCartShipping(Container.DataItem).NameKana) == false) && GetCartShipping(Container.DataItem).IsShippingAddrJp) %>" runat="server">
																	<dt>
																		<%: ReplaceTag("@@User.name_kana.name@@") %>：
																	</dt>
																	<dd>
																		<%#: GetCartShipping(Container.DataItem).NameKana %>&nbsp;さま
																	</dd>
																</div>
																<%-- 電話番号 --%>
																<dt><%: ReplaceTag("@@User.tel1.name@@") %>：</dt>
																<dd><%#: Eval("Tel1") %></dd>
																<span visible="<%# FindCart(Container.DataItem).IsGift %>" class="sender" runat="server">
																	<dt>送り主：</dt>
																	<dd>
																		<p>
																			<%# WebSanitizer.HtmlEncodeChangeToBr(GetAddressDisplayText(GetCartShipping(Container.DataItem))) %>
																			<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
																			<br />
																			<%#: Eval("SenderCompanyName") %>&nbsp<%#: Eval("SenderCompanyPostName") %>
																			<% } %>
																		</p>
																	</dd>
																	<dd><%#: Eval("SenderName1") %><%#: Eval("SenderName2") %>&nbsp;様</dd>
																	<dd visible="<%# GetCartShipping(Container.DataItem).IsSenderAddrJp %>" runat="server">
																		<%#: Eval("SenderNameKana1") %><%#: Eval("SenderNameKana2") %>&nbsp;さま
																	</dd>
																	<dd visible="<%# GetCartShipping(Container.DataItem).IsSenderAddrJp %>" runat="server">
																		<%#: Eval("SenderTel1") %>
																	</dd>
																</span>
															</span>
															<span visible="<%# IsStorePickupDisplayed(Container.DataItem, true) %>" runat="server">
																<dt>
																	受取店舗 :
																</dt>
																<dd>
																	<p><%#: GetRealShopName(Container.ItemIndex) %></p>
																</dd>
																<dt>
																	受取店舗住所 :
																</dt>
																<dd>
																	<p>
																		<%#: "〒" + Eval("Zip") %>
																		<br />
																		<%#: Eval("Addr1") %> <%#: Eval("Addr2") %>
																		<br />
																		<%#: Eval("Addr3") %>
																		<br />
																		<%#: Eval("Addr4") %>
																		<br />
																		<%#: Eval("Addr5") %>
																	</p>
																</dd>
																<dt>
																	営業時間 :
																</dt>
																<dd>
																	<p><%#: GetRealShopOpeningHours(Container.ItemIndex) %></p>
																</dd>
																<dt>
																	店舗電話番号 :
																</dt>
																<dd>
																	<p><%#: Eval("Tel1")%></p>
																</dd>
															</span>
															<span id="hcProducts" visible="<%# FindCart(Container.DataItem).IsGift %>" runat="server">
																<dt>商品：</dt>
																<dd>
																	<asp:Repeater ID="rProductCount" DataSource="<%# GetCartShipping(Container.DataItem).ProductCounts %>" runat="server">
																		<ItemTemplate>
																			<dd>
																				<strong>
																					<%#: GetCartProduct(Container.DataItem).ProductJointName %>
																				</strong>
																				<small runat="server" visible='<%# GetCartProduct(Container.DataItem).IsDisplayProductTagCartProductMessage %>'>
																					<%#: GetCartProduct(Container.DataItem).ProductTagCartProductMessage %>
																				</small>
																				<p visible='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
																					<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList %>' runat="server">
																						<ItemTemplate>
																							<strong runat="server" visible="<%# (string.IsNullOrEmpty(GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false) %>">
																								<%#: GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
																							</strong>
																						</ItemTemplate>
																					</asp:Repeater>
																				</p>
																				<p>&nbsp;&nbsp;&nbsp;&nbsp; <%#: CurrencyManager.ToPrice(GetCartProduct(Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)&nbsp;&nbsp;x&nbsp;<%#: Eval("Count") %></p>
																			</dd>
																		</ItemTemplate>
																	</asp:Repeater>
																</dd>
															</span>
															<span visible="<%# IsStorePickupDisplayed(Container.DataItem) %>" runat="server">
																<dt>配送方法：</dt>
																<dd>
																	<%#: ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, GetCartShipping(Container.DataItem).ShippingMethod) %>
																</dd>
																<dt visible="<%# CanDisplayDeliveryCompany(GetCartIndexFromControl(Container), Container.ItemIndex) %>" runat="server">配送サービス：</dt>
																<dd visible="<%# CanDisplayDeliveryCompany(GetCartIndexFromControl(Container), Container.ItemIndex) %>" runat="server">
																	<%#: GetDeliveryCompanyName(GetCartShipping(Container.DataItem).DeliveryCompanyId) %>
																</dd>
																<dt visible='<%# Eval("SpecifyShippingDateFlg") %>' runat="server">配送希望日：</dt>
																<dd visible='<%# Eval("SpecifyShippingDateFlg") %>' runat="server"><%#: GetShippingDate(GetCartShipping(Container.DataItem)) %></dd>
																<dt visible='<%# Eval("SpecifyShippingTimeFlg") %>' runat="server">配送希望時間帯：</dt>
																<dd visible='<%# Eval("SpecifyShippingTimeFlg") %>' runat="server"><%#: GetShippingTime(GetCartShipping(Container.DataItem)) %></dd>
															</span>
															<span>
																<dt visible='<%# (string.IsNullOrEmpty(GetCartShipping(Container.DataItem).CartObject.GetOrderMemosForOrderConfirm().Trim()) == false) %>' runat="server">注文メモ：</dt>
																<dd visible='<%# (string.IsNullOrEmpty(GetCartShipping(Container.DataItem).CartObject.GetOrderMemosForOrderConfirm().Trim()) == false) %>' runat="server">
																	<%# WebSanitizer.HtmlEncodeChangeToBr(GetCartShipping(Container.DataItem).CartObject.GetOrderMemosForOrderConfirm()) %>
																</dd>
																<span runat="server" visible="<%# OrderCommon.DisplayTwInvoiceInfo(GetCartShipping(Container.DataItem).ShippingCountryIsoCode) %>">
																	<span>
																		<dt>発票種類：
																			<div runat="server" visible="<%# GetCartShipping(Container.DataItem).IsUniformInvoiceCompany %>">
																				<%#: ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@") %>：<br />
																				<%#: ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@") %>：
																			</div>
																			<div runat="server" visible="<%# GetCartShipping(Container.DataItem).IsUniformInvoiceDonate %>">
																				<%#: ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@") %>：
																			</div>
																		</dt>
																		<dd>
																			<%#: GetInformationInvoice(GetCartShipping(Container.DataItem)) %>
																		</dd>
																		<span runat="server" visible="<%# GetCartShipping(Container.DataItem).IsUniformInvoicePersonal %>">
																			<dt>共通性載具：
																				<div visible="<%# (string.IsNullOrEmpty(GetCartShipping(Container.DataItem).CarryType) == false) %>" runat="server">
																					載具コード：
																				</div>
																			</dt>
																			<dd><%#: GetInformationInvoice(GetCartShipping(Container.DataItem), true) %></dd>
																		</span>
																	</span>
																	<asp:CheckBox
																		ID="cbDefaultInvoice"
																		GroupName='<%#: "DefaultInvoiceSetting_" + Container.ItemIndex %>'
																		class="radioBtn DefaultInvoice"
																		Text="通常の電子発票情報に設定する"
																		OnCheckedChanged="cbDefaultInvoice_CheckedChanged"
																		runat="server"
																		AutoPostBack="true"
																		Visible="<%# this.IsLoggedIn %>" />
																	<br />
																</span>
																<br />
																<p><%#: GetReflectMemoToFixedPurchase(GetCartShipping(Container.DataItem).CartObject.ReflectMemoToFixedPurchase) %></p>
																<span visible='<%# ((bool)Eval("WrappingPaperValidFlg") && GetCartObject(Eval("CartObject")).IsGift) %>' runat="server">
																	<dt>のし種類：</dt>
																	<dd>
																		<%#: GetTextDisplay(GetCartShipping(Container.DataItem).WrappingPaperType, true, "なし") %>
																	</dd>
																	<dt>のし差出人：</dt>
																	<dd>
																		<%#: GetTextDisplay(GetCartShipping(Container.DataItem).WrappingPaperName, true, "なし") %>
																	</dd>
																</span>
																<span visible='<%# ((bool)Eval("WrappingBagValidFlg") && GetCartObject(Eval("CartObject")).IsGift) %>' runat="server">
																	<dt>包装種類：</dt>
																	<dd>
																		<%#: GetTextDisplay(GetCartShipping(Container.DataItem).WrappingBagType, true, "なし") %>
																	</dd>
																</span>
																<asp:Repeater
																	ID="rOrderExtendDisplay"
																	ItemType="OrderExtendItemInput"
																	DataSource="<%# GetDisplayOrderExtendItemInputs(GetCartIndexFromControl(Container)) %>"
																	runat="server"
																	Visible="<%# IsDisplayOrderExtend() %>">
																	<HeaderTemplate>
																		<br />
																		<span>
																			<em>注文確認事項</em>
																	</HeaderTemplate>
																	<ItemTemplate>
																		<div style="display: inline-block;">
																			<dt><%#: Item.SettingModel.SettingName %>：</dt>
																			<dd>
																				<%#: GetInputTextDisplay(Item.InputValue, Item.InputText) %>
																			</dd>
																		</div>
																	</ItemTemplate>
																	<FooterTemplate>
																		</span>
																	</FooterTemplate>
																</asp:Repeater>
															</span>
														</dl>
													</div>
													<div visible="<%# (((FindCart(Container.DataItem).HasFixedPurchase) || (FindCart(Container.DataItem).IsBeforeCombineCartHasFixedPurchase)) && (this.IsShowDeliveryPatternInputArea(FindCart(Container.DataItem)) == false)) %>" runat="server">
														<em visible="<%# FindCart(Container.DataItem).HasFixedPurchase %>" runat="server">定期配送情報</em>
														<div visible="<%# FindCart(Container.DataItem).HasFixedPurchase %>" runat="server">
															<dl>
																<dt>配送パターン：</dt>
																<dd><%#: GetCartShipping(Container.DataItem).GetFixedPurchaseShippingPatternString() %></dd>
																<dt>初回配送予定：</dt>
																<dd><%#: DateTimeUtility.ToStringFromRegion(GetCartShipping(Container.DataItem).GetFirstShippingDate(), DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %></dd>
																<dt>今後の配送予定：</dt>
																<dd><%#: DateTimeUtility.ToStringFromRegion(GetCartShipping(Container.DataItem).NextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %></dd>
																<dt></dt>
																<dd><%#: DateTimeUtility.ToStringFromRegion(GetCartShipping(Container.DataItem).NextNextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter) %></dd>
																<p>※定期解約されるまで継続して指定した配送パターンでお届けします。</p>
																<br />
																<dt visible='<%# GetCartShipping(Container.DataItem).SpecifyShippingTimeFlg %>' runat="server">配送希望時間帯：</dt>
																<dd visible='<%# GetCartShipping(Container.DataItem).SpecifyShippingTimeFlg %>' runat="server"><%#: GetShippingTime(GetCartShipping(Container.DataItem)) %></dd>
															</dl>
															<p class="clr">
																<img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" />
															</p>
														</div>
													</div>
												</div>
												<!--box-->
											</ItemTemplate>
										</asp:Repeater>
										<div class="box" visible="<%# this.IsShowDeliveryPatternInputArea(GetCartObject(Container.DataItem)) %>" runat="server">
											<div class="fixed">
												<%-- 定期購入 + 通常注文の注文同梱向け、定期購入配送パターン入力欄 --%>
												<em>定期購入 配送パターンの指定</em>
												<div>
													<div visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, false) %>" runat="server">
														<asp:RadioButton
															ID="rbFixedPurchaseMonthlyPurchase_Date"
															Text="月間隔日付指定"
															Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 1) %>"
															GroupName="FixedPurchaseShippingPattern"
															OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
															AutoPostBack="true"
															runat="server" />
														<div id="ddFixedPurchaseMonthlyPurchase_Date" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, true) %>" runat="server">
															<asp:DropDownList
																ID="ddlFixedPurchaseMonth"
																DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true) %>"
																DataTextField="Text"
																DataValueField="Value"
																SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTH) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
																AutoPostBack="true"
																runat="server" />
															ヶ月ごと
															<asp:DropDownList
																ID="ddlFixedPurchaseMonthlyDate"
																DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST) %>"
																DataTextField="Text"
																DataValueField="Value"
																SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
																AutoPostBack="true"
																runat="server" />
															日に届ける
															<small>
																<asp:CustomValidator runat="Server"
																	ControlToValidate="ddlFixedPurchaseMonth"
																	ValidationGroup="OrderShipping"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	CssClass="error_inline" />
															</small>
															<small>
																<asp:CustomValidator runat="Server"
																	ControlToValidate="ddlFixedPurchaseMonthlyDate"
																	ValidationGroup="OrderShipping"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	CssClass="error_inline" />
															</small>
														</div>
													</div>
													<div visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, true) %>" runat="server">
														<asp:RadioButton
															ID="rbFixedPurchaseMonthlyPurchase_WeekAndDay"
															Text="月間隔・週・曜日指定"
															Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 2) %>"
															GroupName="FixedPurchaseShippingPattern"
															OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
															AutoPostBack="true"
															runat="server" />
														<div id="ddFixedPurchaseMonthlyPurchase_WeekAndDay" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY, false) %>" runat="server">
															<asp:DropDownList
																ID="ddlFixedPurchaseIntervalMonths"
																DataSource="<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, true, true) %>"
																DataTextField="Text"
																DataValueField="Value"
																SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
																AutoPostBack="true"
																runat="server" />
															ヶ月ごと
															<asp:DropDownList
																ID="ddlFixedPurchaseWeekOfMonth"
																DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST) %>"
																DataTextField="Text"
																DataValueField="Value"
																SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
																AutoPostBack="true"
																runat="server" />
															<asp:DropDownList
																ID="ddlFixedPurchaseDayOfWeek"
																DataSource="<%# ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST) %>"
																DataTextField="Text"
																DataValueField="Value"
																SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
																AutoPostBack="true"
																runat="server" />
															に届ける
															<small>
																<asp:CustomValidator runat="Server"
																	ControlToValidate="ddlFixedPurchaseIntervalMonths"
																	ValidationGroup="OrderShipping"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	CssClass="error_inline" />
															</small>
															<small>
																<asp:CustomValidator runat="Server"
																	ControlToValidate="ddlFixedPurchaseWeekOfMonth"
																	ValidationGroup="OrderShipping"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	CssClass="error_inline" />
															</small>
															<small>
																<asp:CustomValidator runat="Server"
																	ControlToValidate="ddlFixedPurchaseDayOfWeek"
																	ValidationGroup="OrderShipping"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	CssClass="error_inline" />
															</small>
														</div>
													</div>
													<div visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, false) %>" runat="server">
														<asp:RadioButton
															ID="rbFixedPurchaseRegularPurchase_IntervalDays"
															Text="配送日間隔指定"
															Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 3) %>"
															GroupName="FixedPurchaseShippingPattern"
															OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
															AutoPostBack="true"
															runat="server" />
														<div id="ddFixedPurchaseRegularPurchase_IntervalDays" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, true) %>" runat="server">
															<asp:DropDownList
																ID="ddlFixedPurchaseIntervalDays"
																DataSource='<%# GetFixedPurchaseIntervalDropdown(Container.ItemIndex, false) %>'
																DataTextField="Text"
																DataValueField="Value"
																SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
																AutoPostBack="true"
																runat="server" />
															日ごとに届ける
															<small>
																<asp:CustomValidator runat="Server"
																	ControlToValidate="ddlFixedPurchaseIntervalDays"
																	ValidationGroup="OrderShipping"
																	ValidateEmptyText="true"
																	SetFocusOnError="true"
																	CssClass="error_inline" />
															</small>
														</div>
													</div>
													<div visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, false) %>" runat="server">
														<asp:RadioButton
															ID="rbFixedPurchaseEveryNWeek"
															Text="週間隔・曜日指定"
															Checked="<%# GetFixedPurchaseKbnInputChecked(Container.ItemIndex, 4) %>"
															GroupName="FixedPurchaseShippingPattern"
															OnCheckedChanged="rbFixedPurchaseShippingPattern_OnCheckedChanged"
															AutoPostBack="true"
															runat="server" />
														<div id="ddFixedPurchaseEveryNWeek" visible="<%# GetFixedPurchaseShippingPaternEnabled(Container.ItemIndex, Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, true) %>" runat="server">
															<asp:DropDownList
																ID="ddlFixedPurchaseEveryNWeek_Week"
																DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(Container.ItemIndex, true) %>"
																DataTextField="Text"
																DataValueField="Value"
																SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
																AutoPostBack="true"
																runat="server" />
															週間ごと
															<asp:DropDownList
																ID="ddlFixedPurchaseEveryNWeek_DayOfWeek"
																DataSource="<%# GetFixedPurchaseEveryNWeekDropdown(Container.ItemIndex, false) %>"
																DataTextField="Text"
																DataValueField="Value"
																SelectedValue='<%#: GetFixedPurchaseSelectedValue(Container.ItemIndex, Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK) %>'
																OnSelectedIndexChanged="ddlFixedPurchaseShippingPatternItem_OnCheckedChanged"
																AutoPostBack="true"
																runat="server" />
															に届ける
														</div>
														<small>
															<asp:CustomValidator
																ID="cvFixedPurchaseEveryNWeek"
																runat="Server"
																ControlToValidate="ddlFixedPurchaseEveryNWeek_Week"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true"
																CssClass="error_inline" />
															<asp:CustomValidator
																ID="cvFixedPurchaseEveryNWeekDayOfWeek"
																runat="Server"
																ControlToValidate="ddlFixedPurchaseEveryNWeek_DayOfWeek"
																ValidationGroup="OrderShipping"
																ValidateEmptyText="true"
																SetFocusOnError="true"
																CssClass="error_inline" />
														</small>
													</div>
													<asp:HiddenField ID="hfFixedPurchaseDaysRequired" Value="<%#: this.ShopShippingList[Container.ItemIndex].FixedPurchaseShippingDaysRequired %>" runat="server" />
													<asp:HiddenField ID="hfFixedPurchaseMinSpan" Value="<%#: this.ShopShippingList[Container.ItemIndex].FixedPurchaseMinimumShippingSpan %>" runat="server" />
												</div>
											</div>
										</div>

										<div class="last">
											<div class="box">
												<em>決済情報</em>
												<div>
													<dl>
														<dt>お支払い：</dt>
														<dd><%#: GetCartPayment(Container.DataItem).PaymentName %></dd>
														<dt visible='<%# GetCartPayment(Container.DataItem).IsPaymentEcPay %>' runat="server">支払い方法：</dt>
														<dd visible='<%# GetCartPayment(Container.DataItem).IsPaymentEcPay %>' runat="server">
															<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE, GetCartPayment(Container.DataItem).ExternalPaymentType) %>
														</dd>
														<dt visible='<%# GetCartPayment(Container.DataItem).IsPaymentNewebPay %>' runat="server">支払い方法：</dt>
														<dd visible='<%# GetCartPayment(Container.DataItem).IsPaymentNewebPay %>' runat="server">
															<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE + "_neweb", GetCartPayment(Container.DataItem).ExternalPaymentType) %>
														</dd>
														<dt visible='<%# GetCartPayment(Container.DataItem).IsPaymentCvsPre %>' runat="server">支払先コンビニ名</dt>
														<dd visible='<%# GetCartPayment(Container.DataItem).IsPaymentCvsPre %>' runat="server">
															<%#: GetCartObject(Container.DataItem).GetPaymentCvsName() %>
														</dd>
														<dt id="dtCvsDef" visible="<%# GetCartPayment(Container.DataItem).IsPaymentCvsDef %>" runat="server">
															<uc:PaymentDescriptionCvsDef runat="server" ID="ucPaymentDescriptionCvsDef" />
														</dt>
														<dt visible='<%# GetCartPayment(Container.DataItem).HasCreditCardCompany %>' runat="server">カード会社：</dt>
														<dd visible='<%# GetCartPayment(Container.DataItem).HasCreditCardCompany %>' runat="server">
															<%#: GetCartPayment(Container.DataItem).CreditCardCompanyName %>
														</dd>
														<dt visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">カード番号：</dt>
														<dd visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">
															XXXXXXXXXXXX<%#: GetCreditCardDispString(GetCartPayment(Container.DataItem)) %>
														</dd>
														<dt visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">有効期限：</dt>
														<dd visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">
															<%#: GetCartPayment(Container.DataItem).CreditExpireMonth %>/<%#: GetCartPayment(Container.DataItem).CreditExpireYear %> (月/年)
														</dd>
														<dt visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">支払い回数：</dt>
														<dd visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">
															<%#: ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, GetCartPayment(Container.DataItem).CreditInstallmentsCode) %>
														</dd>
														<dt visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">カード名義：</dt>
														<dd visible='<%# GetCartPayment(Container.DataItem).HasCreditCardNo %>' runat="server">
															<%#: GetCartPayment(Container.DataItem).CreditAuthorName %>
														</dd>
														<dt visible='<%# GetCartPayment(Container.DataItem).IsCredit %>' runat="server">登録：</dt>
														<dd visible='<%# GetCartPayment(Container.DataItem).IsCredit %>' runat="server">
															<%#: GetTextDisplayUserCreditCardRegistable(
																GetCartPayment(Container.DataItem).UserCreditCardRegistFlg) %>
														</dd>
													</dl>
													<p class="clr">
														<img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" />
													</p>
												</div>
												<%-- ▼領収書情報▼ --%>
												<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
												<p class="clr">
													<img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" />
												</p>
												<div>
													<em>領収書情報</em>
													<div>
														<dl>
															<dt>領収書希望：</dt>
															<dd><%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG, GetCartObject(Container.DataItem).ReceiptFlg) %></dd>
															<div runat="server" visible="<%# GetCartObject(Container.DataItem).IsReceiptFlagOn %>">
																<dt>宛名：</dt>
																<dd><%#: GetCartObject(Container.DataItem).ReceiptAddress %></dd>
																<dt>但し書き：</dt>
																<dd><%#: GetCartObject(Container.DataItem).ReceiptProviso %></dd>
															</div>
														</dl>
													</div>
												</div>
												<% } %>
												<%-- ▲領収書情報▲ --%>
											</div>
											<!--box-->
										</div>
										<!--last-->
									</div>
									<!--bottom-->
								</div>
								<!--orderBox-->
							</div>
							<!--column-->
							<%-- ▲注文内容▲ --%>
							<%-- ▼カート情報▼ --%>
							<div class="shoppingCart">
								<div>
									<h2>
										<img src="../../Contents/ImagesPkg/common/ttl_shopping_cart.gif" alt="ショッピングカート" width="141" height="16" />
									</h2>
									<div class="sumBox mrg_topA">
										<div class="subSumBoxB">
											<p>
												<img src="../../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" />
												<strong><%#: CurrencyManager.ToPrice(this.CartListRecommend.PriceCartListTotal) %></strong>
											</p>
										</div>
									</div>
									<!--sum-->
								</div>
								<div class="subCartList">
									<div class="bottom">
										<h3>カート番号<%#: (Container.ItemIndex + 1) %>
											<%#: DispCartDecolationString(Container.DataItem, "（ギフト）", "（デジタルコンテンツ）") %>
										</h3>
										<div class="block">
											<asp:Repeater ID="rCart" DataSource="<%# GetCartObject(Container.DataItem).Items %>" runat="server" OnItemDataBound="rCart_OnItemDataBound">
												<ItemTemplate>
													<%-- 通常商品 --%>
													<div class="singleProduct" visible="<%# ((GetCartProduct(Container.DataItem).IsSetItem == false) && (GetCartProduct(Container.DataItem).QuantitiyUnallocatedToSet != 0)) %>" runat="server">
														<div>
															<dl>
																<dt>
																	<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
																		<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
																	</a>
																	<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# (GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false) %>" />
																</dt>
																<dd>
																	<strong>
																		<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
																			<%#: GetCartProduct(Container.DataItem).ProductJointName %>
																		</a>
																		<%#: GetCartProduct(Container.DataItem).ProductJoinNameIfProductDetailLinkIsInvalid %>
																	</strong>
																	<small visible='<%# GetCartProduct(Container.DataItem).IsDisplayProductTagCartProductMessage %>' runat="server">
																		<%#: GetCartProduct(Container.DataItem).ProductTagCartProductMessage %>
																	</small>
																	<p visible='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
																		<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList %>' runat="server">
																			<ItemTemplate>
																				<strong runat="server" visible="<%# (string.IsNullOrEmpty(GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false) %>">
																					<%#: GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
																				</strong>
																			</ItemTemplate>
																		</asp:Repeater>
																	</p>
																	<p>数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%#: GetCartProduct(Container.DataItem).QuantitiyUnallocatedToSet %></p>
																	<p runat="server" ID="pPrice" style="padding-top: 2px; text-decoration: line-through" Visible="False" ><span runat="server" ID="sPrice"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
																	<p runat="server" ID="pSubscriptionBoxCampaignPrice" style="padding-top: 2px"><span runat="server" ID="sSubscriptionBoxCampaignPrice"><%#: CurrencyManager.ToPrice(((CartProduct)Container.DataItem).Price) %></span> (<%#: this.ProductPriceTextPrefix %>)</p>
																	<div Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" class="specifiedcommercialtransactionsOrderConfirmSellTimeName" runat="server">販売期間：<br/></div>
																	<div Visible="<%# ((CartProduct)Container.DataItem).IsDisplaySell && ((CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) == false) %>" class="specifiedcommercialtransactionsOrderConfirmSellTime" runat="server">
																		<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/>
																		<%#: DateTimeUtility.ToStringFromRegion(((CartProduct)Container.DataItem).SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>
																	</div>
																	<%--頒布会の選択可能期間--%>
																	<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="specifiedcommercialtransactionsOrderConfirmSellTimeName" runat="server" >販売期間</div>
																	<div Visible="<%# (CartProduct.IsDisplaySubscriptionBoxSelectTime(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)) %>" class="specifiedcommercialtransactionsOrderConfirmSellTime" runat="server" >
																		<%# WebSanitizer.HtmlEncodeChangeToBr((CartProduct.GetSubscriptionBoxSelectTermBr(this.ShopId, ((CartProduct)Container.DataItem).SubscriptionBoxCourseId,((CartProduct)Container.DataItem).ProductId,((CartProduct)Container.DataItem).VariationId)))%>
																	</div>
																	<p runat="server" ID="pSubscriptionBoxCampaignPeriod" Visible="False" style="color: red; padding-top: 2px">キャンペーン期間：<br>
																		<span ID="sSubscriptionBoxCampaignPeriodSince" class="specifiedcommercialtransactionsCampaignTime" runat="server"></span>～<br />
																		<span ID="sSubscriptionBoxCampaignPeriodUntil" class="specifiedcommercialtransactionsCampaignTime" runat="server"></span>
																	</p>
																	<div visible="<%# GetCartProduct(Container.DataItem).IsDispSaleTerm %>" class="specifiedcommercialtransactionsOrderConfirmSellTimeName" runat="server">タイムセール期間:<br/></div>
																	<div visible="<%# GetCartProduct(Container.DataItem).IsDispSaleTerm %>" class="specifiedcommercialtransactionsOrderConfirmSellTime" runat="server">
																		<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetProductSaleTermBr(this.ShopId, ((CartProduct)Container.DataItem).ProductSaleId)) %>
																	</div>
																	<p><%# WebSanitizer.HtmlEncodeChangeToBr(GetCartProduct(Container.DataItem).ReturnExchangeMessage) %></p>
																	<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
																		※配送料無料適用外商品です
																	</p>
																</dd>
															</dl>
														</div>
													</div>
													<!--singleProduct-->
													<%-- セット商品 --%>
													<div class="multiProduct" visible="<%# (GetCartProduct(Container.DataItem).IsSetItem) && (GetCartProduct(Container.DataItem).ProductSetItemNo == 1) %>" runat="server">
														<asp:Repeater ID="rProductSet" DataSource="<%# GetProductSetDisplay(GetCartProduct(Container.DataItem).ProductSet) %>" runat="server">
															<ItemTemplate>
																<div>
																	<dl>
																		<dt>
																			<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
																				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
																			</a>
																			<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# (GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false) %>" />
																		</dt>
																		<dd>
																			<strong>
																				<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
																					<%#: GetCartProduct(Container.DataItem).ProductJointName %>
																				</a>
																				<%#: GetCartProduct(Container.DataItem).ProductJoinNameIfProductDetailLinkIsInvalid %>
																			</strong>
																			<small visible='<%# GetCartProduct(Container.DataItem).IsDisplayProductTagCartProductMessage %>' runat="server">
																				<%#: GetCartProduct(Container.DataItem).ProductTagCartProductMessage %>
																			</small>
																			<p><%#: CurrencyManager.ToPrice(GetCartProduct(Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)&nbsp;&nbsp;x&nbsp;&nbsp;<%#: GetCartProduct(Container.DataItem).CountSingle %></p>
																			<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
																				※配送料無料適用外商品です
																			</p>
																		</dd>
																	</dl>
																</div>
																<table visible="<%# (GetCartProduct(Container.DataItem).ProductSetItemNo == GetCartProduct(Container.DataItem).ProductSet.Items.Count) %>" width="297" cellpadding="0" cellspacing="0" class="clr" runat="server">
																	<tr>
																		<th width="38">セット：</th>
																		<th width="50"><%#: GetProductSetCount(GetCartProduct(Container.DataItem)) %></th>
																		<th width="146"><%#: CurrencyManager.ToPrice(GetProductSetPriceSubtotal(GetCartProduct(Container.DataItem))) %> (<%#: this.ProductPriceTextPrefix %>)</th>
																		<td width="61"></td>
																	</tr>
																</table>
															</ItemTemplate>
														</asp:Repeater>
													</div>
													<!--multiProduct-->
												</ItemTemplate>
											</asp:Repeater>
											<%-- セットプロモーション商品 --%>
											<asp:Repeater ID="rCartSetPromotion" DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" runat="server">
												<ItemTemplate>
													<div class="multiProduct">
														<asp:Repeater ID="rCartSetPromotionItem" DataSource="<%# GetCartSetPromotion(Container.DataItem).Items %>" runat="server">
															<ItemTemplate>
																<div>
																	<dl>
																		<dt>
																			<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
																				<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" />
																			</a>
																			<w2c:ProductImage ProductMaster="<%# Container.DataItem %>" ImageSize="M" runat="server" Visible="<%# (GetCartProduct(Container.DataItem).IsProductDetailLinkValid() == false) %>" />
																		</dt>
																		<dd>
																			<strong>
																				<a target="_blank" href='<%# WebSanitizer.UrlAttrHtmlEncode(GetCartProduct(Container.DataItem).CreateProductDetailUrl()) %>' runat="server" visible="<%# GetCartProduct(Container.DataItem).IsProductDetailLinkValid() %>">
																					<%#: GetCartProduct(Container.DataItem).ProductJointName %></a>
																				<%#: GetCartProduct(Container.DataItem).ProductJoinNameIfProductDetailLinkIsInvalid %>
																			</strong>
																			<p visible='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList.IsSelectedProductOptionValueAll %>' runat="server">
																				<asp:Repeater ID="rProductOptionSettings" DataSource='<%# GetCartProduct(Container.DataItem).ProductOptionSettingList %>' runat="server">
																					<ItemTemplate>
																						<strong runat="server" visible="<%# (string.IsNullOrEmpty(GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue()) == false) %>">
																							<%#: GetProductOptionSetting(Container.DataItem).GetDisplayProductOptionSettingSelectValue() %>
																						</strong>
																					</ItemTemplate>
																				</asp:Repeater>
																			</p>
																			<p>数量：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%#: GetCartProduct(Container.DataItem).QuantityAllocatedToSet[GetCartSetPromotion(GetRepeaterItem(Container.Parent.Parent).DataItem).CartSetPromotionNo] %></p>
																			<p><%#: CurrencyManager.ToPrice(GetCartProduct(Container.DataItem).Price) %> (<%#: this.ProductPriceTextPrefix %>)</p>
																			<p><%# WebSanitizer.HtmlEncodeChangeToBr(GetCartProduct(Container.DataItem).ReturnExchangeMessage) %></p>
																			<p style="color:red;" visible="<%# ((CartProduct)Container.DataItem).IsDisplayExcludeFreeShippingText %>" runat="server">
																				※配送料無料適用外商品です
																			</p>
																		</dd>
																	</dl>
																</div>
															</ItemTemplate>
														</asp:Repeater>
														<dl class="setpromotion">
															<dt><%#: GetCartSetPromotion(Container.DataItem).SetpromotionDispName %></dt>
															<dd>
																<span visible="<%# GetCartSetPromotion(Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
																	<strike><%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).UndiscountedProductSubtotal) %></strike><br />
																</span>
																<span class="specifiedcommercialtransactionsCampaignTimeSpan">
																	<%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).UndiscountedProductSubtotal - GetCartSetPromotion(Container.DataItem).ProductDiscountAmount) %> (税込)
																</span>
															</dd>
															<br />
															<div class="SpecifiedcommercialtransactionsCampaignTimePadding100" Visible="<%# (((CartSetPromotion)Container.DataItem).Cart.Items[0].IsDisplaySell) %>" runat="server">販売期間：</div>
															<div class="SpecifiedcommercialtransactionsCampaignTimePadding110" Visible="<%# (((CartSetPromotion)Container.DataItem).Cart.Items[0].IsDisplaySell) %>"  runat="server">
																<%#: DateTimeUtility.ToStringFromRegion(((CartSetPromotion)Container.DataItem).Cart.Items[0].SellFrom, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>～<br/>
																<%#: DateTimeUtility.ToStringFromRegion(((CartSetPromotion)Container.DataItem).Cart.Items[0].SellTo, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime) %>
															</div>
															<div class="SpecifiedcommercialtransactionsCampaignTimePadding100" Visible="<%# ((CartSetPromotion)Container.DataItem).Cart.Items[0].IsDispSaleTerm %>" runat="server">タイムセール期間:</div>
															<div class="SpecifiedcommercialtransactionsCampaignTimePadding110" Visible="<%# ((CartSetPromotion)Container.DataItem).Cart.Items[0].IsDispSaleTerm %>" runat="server">
																<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetProductSaleTermBr(this.ShopId, ((CartSetPromotion)Container.DataItem).Cart.Items[0].ProductSaleId)) %>
															</div>
															<div class="SpecifiedcommercialtransactionsCampaignTimePadding100" Visible="<%# ((CartSetPromotion)Container.DataItem).IsDispSetPromotionTerm %>" runat="server">セットプロモーション期間:</div>
															<div class="SpecifiedcommercialtransactionsCampaignTimePadding110" Visible="<%# ((CartSetPromotion)Container.DataItem).IsDispSetPromotionTerm %>" runat="server">
																<%# WebSanitizer.HtmlEncodeChangeToBr(ProductCommon.GetSetPromotionTermBr(((CartSetPromotion)Container.DataItem).SetPromotionId)) %>
															</div>
														</dl>
													</div>
												</ItemTemplate>
											</asp:Repeater>
											<div class="priceList">
												<div>
													<dl class="bgc">
														<dt>小計(<%#: this.ProductPriceTextPrefix %>)</dt>
														<dd><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceSubtotal) %></dd>
													</dl>
													<% if (this.ProductIncludedTaxFlg == false) { %>
													<dl class='<%#: GetClassNameForDisplayRow() %>'>
														<dt>消費税</dt>
														<dd><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceSubtotalTax) %></dd>
													</dl>
													<% } %>
													<%-- セットプロモーション割引額(商品割引) --%>
													<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" runat="server">
														<ItemTemplate>
															<span visible="<%# GetCartSetPromotion(Container.DataItem).IsDiscountTypeProductDiscount %>" runat="server">
																<dl class='<%#: GetClassNameForDisplayRow() %>'>
																	<dt>
																		<%#: GetCartSetPromotion(Container.DataItem).SetpromotionDispName %>
																	</dt>
																	<dd class='<%#: GetClassNameForDisplayPrice(GetCartSetPromotion(Container.DataItem).ProductDiscountAmount) %>'>
																		<%#: GetPrefixForDisplayPrice(GetCartSetPromotion(Container.DataItem).ProductDiscountAmount) %>
																		<%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).ProductDiscountAmount) %>
																	</dd>
																</dl>
															</span>
														</ItemTemplate>
													</asp:Repeater>
													<% if (Constants.MEMBER_RANK_OPTION_ENABLED && this.IsLoggedIn) { %>
													<dl class='<%#: GetClassNameForDisplayRow() %>'>
														<dt>会員ランク割引額</dt>
														<dd class='<%#: GetClassNameForDisplayPrice(GetCartObject(Container.DataItem).MemberRankDiscount) %>'>
															<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).MemberRankDiscount) %>
															<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).MemberRankDiscount) %>
														</dd>
													</dl>
													<% } %>
													<% if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED && this.IsLoggedIn) { %>
													<dl class='<%#: GetClassNameForDisplayRow() %>'>
														<dt>定期会員割引額</dt>
														<dd class='<%#: GetClassNameForDisplayPrice(GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount) %>'>
															<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount) %>
															<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).FixedPurchaseMemberDiscountAmount) %>
														</dd>
													</dl>
													<% } %>
													<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
													<div runat="server" visible="<%# (GetCartObject(Container.DataItem).HasFixedPurchase) %>">
														<dl class='<%#: GetClassNameForDisplayRow() %>'>
															<dt>定期購入割引額</dt>
															<dd class='<%#: GetClassNameForDisplayPrice(GetCartObject(Container.DataItem).FixedPurchaseDiscount) %>'>
																<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).FixedPurchaseDiscount) %>
																<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).FixedPurchaseDiscount) %>
															</dd>
														</dl>
													</div>
													<% } %>
													<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
													<dl class='<%#: GetClassNameForDisplayRow() %>'>
														<dt>クーポン割引額</dt>
														<dd class='<%#: GetClassNameForDisplayPrice(GetCartObject(Container.DataItem).UseCouponPrice) %>'>
															<%#: GetCouponName(GetCartObject(Container.DataItem)) %>
															<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).UseCouponPrice) %>
															<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).UseCouponPrice) %>
														</dd>
													</dl>
													<% } %>
													<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
													<dl class='<%#: GetClassNameForDisplayRow() %>'>
														<dt>ポイント利用額</dt>
														<dd class='<%#: GetClassNameForDisplayPrice(GetCartObject(Container.DataItem).UsePointPrice) %>'>
															<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).UsePointPrice) %>
															<%#: GetDiscountPriceCalculate(GetCartObject(Container.DataItem).UsePointPrice) %>
														</dd>
													</dl>
													<% } %>
													<dl class='<%#: GetClassNameForDisplayRow() %>'>
														<dt>配送料金<span visible="<%# GetCartObject(Container.DataItem).IsGift %>" runat="server">合計</span></dt>
														<dd runat="server" visible="<%# ((GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateFlg) == false) %>">
															<%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceShipping) %></dd>
														<dd runat="server" visible="<%# GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateFlg %>">
															<%#: GetCartObject(Container.DataItem).ShippingPriceSeparateEstimateMessage %>
														</dd>
														<small style="color:red;" visible="<%# ((CartObject)Container.DataItem).IsDisplayFreeShiipingFeeText %>" runat="server">
															※配送料無料適用外の商品が含まれるため、<%#: CurrencyManager.ToPrice(((CartObject)Container.DataItem).PriceShipping) %>の配送料が請求されます
														</small>
													</dl>
													<%-- セットプロモーション割引額(配送料割引) --%>
													<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" runat="server">
														<ItemTemplate>
															<span visible="<%# GetCartSetPromotion(Container.DataItem).IsDiscountTypeShippingChargeFree %>" runat="server">
																<dl class='<%#: GetClassNameForDisplayRow() %>'>
																	<dt><%#: GetCartSetPromotion(Container.DataItem).SetpromotionDispName %>(送料割引)</dt>
																	<dd class='<%#: GetClassNameForDisplayPrice(GetCartSetPromotion(Container.DataItem).ShippingChargeDiscountAmount) %>'>
																		<%#: GetPrefixForDisplayPrice(GetCartSetPromotion(Container.DataItem).ShippingChargeDiscountAmount) %>
																		<%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).ShippingChargeDiscountAmount) %>
																	</dd>
																</dl>
															</span>
														</ItemTemplate>
													</asp:Repeater>
													<dl class='<%#: GetClassNameForDisplayRow() %>'>
														<dt>決済手数料</dt>
														<dd><%#: CurrencyManager.ToPrice(GetCartPayment(Container.DataItem).PriceExchange) %></dd>
													</dl>
													<%-- セットプロモーション割引額(決済手数料料割引) --%>
													<asp:Repeater DataSource="<%# GetCartObject(Container.DataItem).SetPromotions %>" runat="server">
														<ItemTemplate>
															<span visible="<%# GetCartSetPromotion(Container.DataItem).IsDiscountTypePaymentChargeFree %>" runat="server">
																<dl class='<%#: GetClassNameForDisplayRow() %>'>
																	<dt><%#: GetCartSetPromotion(Container.DataItem).SetpromotionDispName %>(決済手数料割引)</dt>
																	<dd class='<%#: GetClassNameForDisplayPrice(GetCartSetPromotion(Container.DataItem).PaymentChargeDiscountAmount) %>'>
																		<%#: GetPrefixForDisplayPrice(GetCartSetPromotion(Container.DataItem).PaymentChargeDiscountAmount) %>
																		<%#: CurrencyManager.ToPrice(GetCartSetPromotion(Container.DataItem).PaymentChargeDiscountAmount) %>
																	</dd>
																</dl>
															</span>
														</ItemTemplate>
													</asp:Repeater>
													<dl class='<%#: GetClassNameForDisplayRow() %>' visible="<%# (GetCartObject(Container.DataItem).PriceRegulation != 0) %>" runat="server">
														<dt>調整金額</dt>
														<dd class='<%#: GetClassNameForDisplayPrice(GetCartObject(Container.DataItem).PriceRegulation, true) %>'>
															<%#: GetPrefixForDisplayPrice(GetCartObject(Container.DataItem).PriceRegulation, true) %>
															<%#: CurrencyManager.ToPrice(Math.Abs(GetCartObject(Container.DataItem).PriceRegulation)) %></dd>
													</dl>
												</div>
												<p class="clr">
													<img src="../../Contents/ImagesPkg/common/clear.gif" alt="" width="1" height="1" />
												</p>
												<div>
													<dl class="result">
														<dt>合計(税込)</dt>
														<dd><%#: CurrencyManager.ToPrice(GetCartObject(Container.DataItem).PriceTotal) %></dd>
														<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
														<dt>決済金額(税込)</dt>
														<dd><%#: GetSettlementAmount(GetCartObject(Container.DataItem)) %></dd>
														<small style="color: red">
															<%#: string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMOUNT_VARIES_WITH_RATE), GetCartObject(Container.DataItem).SettlementCurrency) %>
														</small>
														<% } %>
													</dl>
													<small class="InternationalShippingAttention" runat="server" visible="<%# IsDisplayProductTaxExcludedMessage(GetCartObject(Container.DataItem)) %>">※国外配送をご希望の場合関税・商品消費税は料金に含まれず、商品到着後、現地にて税をお支払いいただくこととなりますのでご注意ください。</small>
												</div>
											</div>
											<!--priceList-->
										</div>
										<!--block-->
									</div>
									<!--bottom-->
								</div>
								<!--subCartList-->
								<div visible="<%# (GetCartObjectListByRepeater(((Repeater)Container.Parent).DataSource).Items.Count == (Container.ItemIndex + 1)) %>" runat="server">
									<div class="sumBox">
										<div class="subSumBox">
											<p>
												<img src="../../Contents/ImagesPkg/common/ttl_sum.gif" alt="総合計" width="52" height="16" />
												<strong><%#: CurrencyManager.ToPrice(this.CartListRecommend.PriceCartListTotal) %></strong>
											</p>
										</div>
										<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsLoggedIn) { %>
										<dl>
											<dt visible="<%# GetCartObject(Container.DataItem).IsHasFirstBuyPoint %>" runat="server">初回購入獲得ポイント</dt>
											<dd visible="<%# GetCartObject(Container.DataItem).IsHasFirstBuyPoint %>" runat="server">
												<%#: GetNumeric(GetCartObjectListByRepeater(((Repeater)Container.Parent).DataSource).TotalFirstBuyPoint) %>pt
											</dd>
											<dt>購入後獲得ポイント</dt>
											<dd><%#: GetNumeric(GetCartObjectListByRepeater(((Repeater)Container.Parent).DataSource).TotalBuyPoint) %>pt</dd>
										</dl>
										<small>※ 1pt = <%: CurrencyManager.ToPrice(1m) %></small>
										<% } %>
									</div>
									<!--sumBox-->
								</div>
								<!-- レコメンド設定 -->
								<uc:BodyRecommend
									runat="server"
									Cart="<%# GetCartObjectOrderComplete(Container.DataItem) %>"
									Visible="<%# (this.IsOrderCombined == false) %>" />
								<!-- 定期注文購入金額 -->
								<uc:BodyFixedPurchaseOrderPrice runat="server" Cart="<%# GetCartObject(Container.DataItem) %>" Visible="<%# GetCartObject(Container.DataItem).HasFixedPurchase %>" />
							</div>
							<!--shoppingCart-->
							<%-- ▲カート情報▲ --%>
							<br class="clr" />
						</div>
						<!--submain-->
					</div>
					<!--main-->
				</ItemTemplate>
			</asp:Repeater>
			<div style="text-align: left; padding: 10px 0; width: 780px; margin: 0 auto;" runat="server">
				<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal == false) { %>
				以下の内容をご確認のうえ、「注文を確定する」ボタンをクリックしてください。
				<% } %>
				<% if (this.IsDispCorrespondenceSpecifiedCommericalTransactions) { %>
				<br />
				<%= ShopMessage.GetMessageByPaymentId((this.BindingCartList == null) ? string.Empty : this.BindingCartList.Payment.PaymentId) %>
				<% } %>
			</div>
			<div class="btmbtn below">
				<ul>
					<li>
						<% if (SessionManager.IsChangedAmazonPayForFixedOrNormal) { %>
						<div style="text-align: left; display: inline-block; color: red;">
							カート内の商品が変更されました。<br />
							お手数ですが再度Amazon Payでの購入手続きに進んでください。<br />
							<br />
						</div>
						<div style="width: 200px">
							<div id="AmazonPayCv2Button"></div>
						</div>
						<% } else { %>
						<asp:LinkButton
							ID="lbComplete"
							runat="server"
							OnClick="lbComplete_Click"
							class="btn btn-large btn-success"
							Text="注文を確定する" />
						<% } %>
						<span id="processing2" style="display:none">
							<center>
								<strong>ただいま決済処理中です。<br />画面が切り替わるまでそのままお待ちください。</strong>
							</center>
						</span>
					</li>
					<li style="display: none;">
						<asp:LinkButton ID="lbCompleteAfterComfirmPayment" runat="server" OnClick="lbComplete_Click" />
					</li>
				</ul>
			</div>
		</div>
	</div>
</div>
<script>
	function closeModalRecommend() {
		$('#modalRecommend').css("display", "none");
		return false;
	}
</script>
