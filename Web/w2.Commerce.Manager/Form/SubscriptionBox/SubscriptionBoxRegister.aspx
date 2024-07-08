<%--
=========================================================================================================
  Module      : 頒布会コース登録画面(SubscriptionBoxRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="SubscriptionBoxRegister.aspx.cs" Inherits="Form_SubscriptionBox_SubscriptionBoxRegister" maintainScrollPositionOnPostBack="true" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<%@ Import Namespace="Input.SubscriptionBox" %>
<asp:Content ID="header" ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr><td><h1 class="page-title">頒布会コース管理</h1></td></tr>
	<!--▽ 登録編集 ▽-->
	<tr id="trEdit" runat="server" Visible="False"><td><h1 class="cmn-hed-h2">頒布会コース編集</h1></td></tr>
	<tr id="trRegister" runat="server" Visible="False"><td><h1 class="cmn-hed-h2">頒布会コース登録</h1></td></tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td>
						<table class="info_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td>
												<div class="action_part_top" Visible="<%# (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE) && !this.IsPostBack %>" runat="server" >
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">頒布会コース設定を登録/更新しました。</td>
														</tr>
													</table>
												</div>
												<br />
												<div class="action_part_top">
													<asp:Button ID="btnBackListTop" runat="server" Text="  一覧へ戻る  " onclick="btnBackList_Click" CausesValidation="false"/>
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('頒布会コース設定を削除します。よろしいですか？');" />
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" OnClientClick="return confirm('表示内容で更新します。よろしいですか？');" />
												</div>
												<table class="edit_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tbody id="tbdyErrorMessages" visible="<%# string.IsNullOrEmpty(this.ErrorMessage) == false %>" runat="server">
														<tr>
															<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
														</tr>
														<tr>
															<td class="edit_item_bg" style="color: red;" colspan="4">
																<%# this.ErrorMessage %>
															</td>
														</tr>
													</tbody>
													<tr>
														<td align="center" class="edit_title_bg" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg" width="30%">頒布会コースID<span class="notice"><%: this.CanEditCourseId ? "*" : "" %></span></td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbCourseId" Visible="<%# this.CanEditCourseId %>" Text="<%# this.Input.CourseId %>" MaxLength="30" runat="server"></asp:TextBox>
															<asp:Literal Visible="<%# this.CanEditCourseId == false %>" Text="<%#: this.Input.CourseId %>" runat="server" />
														</td>
													</tr>
													<tr visible="<%# this.DisplayFirstSelectionPageUrl %>" runat="server">
														<td align="left" class="detail_title_bg" width="250">頒布会初回選択画面URL</td>
														<td align="left" class="detail_item_bg">
															<div class="add_cart_url">
																<a class="btn-clipboard" href="#" data-clipboard-text="<%# GetFirstSelectionPageUrl() %>">【頒布会初回選択画面】URLをコピー</a>
															</div>
														</td>
													</tr>
													<tr runat="server" visible="<%# this.DisplayAddCartUrl %>">
														<td align="left" class="detail_title_bg" width="250">カート投入URL</td>
														<td align="left" class="detail_item_bg">
															<div class="add_cart_url">
																<a class="btn-clipboard" href="#" data-clipboard-text="<%# GetSubscriptionBoxAddCartUrl() %>">【頒布会購入用】URLをコピー</a>
															</div>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg">頒布会コース管理名 <span class="notice">*</span></td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbManagementName" Text="<%# this.Input.ManagementName %>" MaxLength="50" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg">頒布会コース表示名 <span class="notice">*</span></td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbDisplayName" Text="<%# this.Input.DisplayName %>" MaxLength="50" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr id="trSelectProductSaleKbn" runat="server">
														<td class="edit_title_bg" align="left">商品決定方法 <span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:RadioButtonList
																ID="rblDeterminationMethod"
																SelectedValue="<%# this.Input.OrderItemDeterminationType %>"
																RepeatDirection="Horizontal"
																RepeatLayout="Flow"
																runat="server"
																AutoPostBack="true"
																OnSelectedIndexChanged="rblDeterminationMethod_SelectedIndexChanged" /></td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg">最低購入数量</td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbMinimumPurchaseQuantity" Text="<%# this.Input.MinimumPurchaseQuantity %>" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg">最大購入数量</td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbMaximumPurchaseQuantity" Text="<%# this.Input.MaximumPurchaseQuantity %>" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg">最低購入種類数</td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbMinimumNumberOfProducts" Text="<%# this.Input.MinimumNumberOfProducts %>" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg">最大購入種類数</td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbMaximumNumberOfProducts" Text="<%# this.Input.MaximumNumberOfProducts %>" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg">商品合計金額下限（税込）</td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbMinimumAmount" Text="<%# this.Input.MinimumAmount.ToPriceString() %>" runat="server"/>
														</td>
													</tr>
													<tr>
														<td align="left" class="edit_title_bg">商品合計金額上限（税込）</td>
														<td align="left" class="edit_item_bg">
															<asp:TextBox ID="tbMaximumAmount" Text="<%# this.Input.MaximumAmount.ToPriceString() %>" runat="server"/>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">定額設定</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbFixedAmountFlg" AutoPostBack="True" Checked="<%# this.Input.IsFixedAmount %>" Text="有効" runat="server" />
														</td>
													</tr>
													<% if (cbFixedAmountFlg.Checked) { %>
														<tr>
															<td class="edit_title_bg" align="left">定額価格</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbFixedAmount" runat="server" MaxLength="7" Text="<%# this.Input.FixedAmount.ToPriceString() %>" Width="100" />
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">税率カテゴリ</td>
															<td class="edit_item_bg" align="left">
																<asp:DropDownList id="ddlTaxCategory" runat="server" />
															</td>
														</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">マイページでの商品変更可否</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbAllowChangeProductOnFront" Checked="<%# this.Input.AreItemsChangeableByUser %>" Text="有効" runat="server" /></td>
													</tr>
													<tr Visible="<%# this.Input.IsOrderDeterminationTypeNumberTime %>" runat="server">
														<td class="edit_title_bg" align="left">繰り返し設定</td>
														<td class="edit_item_bg" align="left">
															<asp:RadioButtonList ID="rbRenewalType" RepeatDirection="Vertical" RepeatLayout="Flow" runat="server" AutoPostBack="True" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">表示優先順<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
														<asp:TextBox ID="tbSubScriptionBoxPriority" Text="<%# this.Input.DisplayPriority %>" runat="server" Width="50" MaxLength="7" />
													</tr>
													<tr Visible="<%# this.Input.IsOrderDeterminationTypeNumberTime %>" runat="server">
														<td class="edit_title_bg" align="left">頒布会初回選択画面</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbFirstSelectableFlg"  Checked="<%# this.Input.IsUsingFirstSelectablePage %>" Text="有効" AutoPostBack="True" runat="server" /></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">有効フラグ</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbValidFlg" Checked="<%# this.Input.IsValid %>" Text="有効" runat="server" /></td>
													</tr>
												</table>
												<div style="width: 100%">
													<img src="../../Images/Common/sp.gif" height="12px" alt="" />
												</div>
												<div class="action_part_top">
													<input type="button" value="  商品追加  " onclick="javascript:open_window('<%= WebSanitizer.UrlAttrHtmlEncode(GetProductSearchPopupUrl()) %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes');"/>
													<asp:HiddenField ID="hfShopId" runat="server" />
													<asp:HiddenField ID="hfProductId" runat="server" />
													<asp:HiddenField ID="hfVariationId" runat="server" />
													<asp:HiddenField ID="hfProductName" runat="server" />
													<asp:HiddenField ID="hfShippingType" runat="server" />
												</div>
												<table class="list_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<thead>
														<tr>
															<td align="center" class="edit_title_bg" colspan="7">選択可能商品</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="center" width="22%">選択可能期間</td>
															<td class="edit_title_bg" align="center" width="12%">商品ID</td>
															<td class="edit_title_bg" align="center" width="12%">商品バリエーションID</td>
															<td class="edit_title_bg" align="center" width="12%">商品名</td>
															<td class="edit_title_bg" align="center" width="22%">キャンペーン期間</td>
															<td class="edit_title_bg" align="center" width="10%">キャンペーン価格</td>
															<td class="edit_title_bg" align="center" width="10%">削除</td>
														</tr>
													</thead>
													<tbody>
														<tr class="list_alert" Visible="<%# this.Input.Items.Any() == false  %>" runat="server">
															<td colspan="7">
																選択可能商品を一つ以上追加してください。
															</td>
														</tr>
														<asp:Repeater
															ID="rItems"
															ItemType="Input.SubscriptionBox.SubscriptionBoxItemInput"
															DataSource="<%# this.Input.Items %>"
															OnItemDataBound="rItems_OnItemDataBound"
															OnItemCommand="rItems_ItemCommand"
															runat="server">
															<ItemTemplate>
																<asp:HiddenField ID="hfShopId" Value="<%# Item.ShopId %>" runat="server" />
																<asp:HiddenField ID="hfProductId" Value="<%# Item.ProductId %>"  runat="server" />
																<asp:HiddenField ID="hfVariationId" Value="<%# Item.VariationId %>"  runat="server" />
																<asp:HiddenField ID="hfProductName" Value='<%# Item.ProductName %>' runat="server" />
																<asp:HiddenField ID="hfShippingType" Value="<%# Item.ShippingType %>" runat="server" />

																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td class="edit_item_bg ta-center">
																		<uc:DateTimePickerPeriodInput
																			id="ucSelectableDateRange"
																			IsHideTime="True"
																			IsNullStartDateTime="True"
																			IsNullEndDateTime="True"
																			StartDate="<%# Item.SelectableSinceDate %>"
																			EndDate="<%# Item.SelectableUntilDate %>"
																			runat="server" />
																	</td>
																	<td class="edit_item_bg ta-center"><%#: Item.ProductId %></td>
																	<td class="edit_item_bg ta-center"><%#: Item.VariationId %></td>
																	<td class="edit_item_bg ta-center"><%#: (string.IsNullOrEmpty(Item.ProductName)) ? "削除済み商品" : Item.ProductName %></td>
																	<td class="edit_item_bg ta-center"><uc:DateTimePickerPeriodInput id="ucOrderScheduledCampaignDatePeriod" runat="server" IsNullStartDateTime="true" IsNullEndDateTime="True" /> </td>
																	<td class="edit_item_bg ta-center"><asp:TextBox ID="tbCampaignPrice" runat="server" Text="<%# Item.CampaignPrice.ToPriceString()  %>" Width="100%" style="margin: 0"/></td>
																	<td class="edit_item_bg ta-center">
																		<asp:Button ID="btnDeleteProduct" runat="server" Text="削除" CommandName="delete" CommandArgument='<%# Container.ItemIndex %>' />
																	</td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>
												<span style="display:none"><asp:LinkButton ID="lbSetSelectableProduct" runat="server" OnClick="lbSetSelectableProduct_OnClick" /></span>
												<div style="width: 100%">
													<img src="../../Images/Common/sp.gif" height="12px" alt=""/>
												</div>
												<div class="action_part_top">
													<asp:Button ID="btnAddDefault" Enabled="<%# this.Input.Items.Any() %>" Text="期間/回数追加" OnClick="btnAddDefault_Click" runat="server" />
												</div>
												<table class="list_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<thead>
														<tr>
															<td align="center" class="edit_title_bg" colspan="5"><%: this.Input.IsOrderDeterminationTypePeriod ? "期間" : "回数" %>別 デフォルト注文商品</td>
														</tr>
														<tr>
															<td class="edit_title_bg ta-center" width="25%">
																<%: this.Input.IsOrderDeterminationTypePeriod ? "期間" : "回数" %>
																<% if (this.Input.IsOrderDeterminationTypePeriod) { %>
																	<span class="notice">*</span>
																<% } %>
															</td>
															<td class="edit_title_bg ta-center" width="10%">必須商品設定</td>
															<td class="edit_title_bg ta-center" width="50%">商品</td>
															<td class="edit_title_bg ta-center" width="5%">数量</td>
															<td class="edit_title_bg ta-center" width="10%">削除</td>
														</tr>
													</thead>
													<tbody>
														<tr class="list_alert" Visible="<%# this.Input.DefaultItems.Any() == false  %>" runat="server">
															<td colspan="6">
																デフォルト注文商品が設定されていません。<br />
																選択可能商品を追加した後、デフォルト注文商品を設定してください。
															</td>
														</tr>
														<asp:Repeater
															ID="rDefaultItems"
															DataSource="<%# this.Input.DefaultItems %>"
															ItemType="Input.SubscriptionBox.SubscriptionBoxDefaultItemInput"
															OnItemCommand="rDefaultItems_OnItemCommand"
															OnItemDataBound="rDefaultItems_OnItemDataBound"
															runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td class="edit_item_bg" align="center" rowspan="<%#: Item.Items.Count + 1 %>">
																		<div style="padding: 10px;">
																			<div Visible="<%# this.Input.IsOrderDeterminationTypeNumberTime %>" runat="server">
																				<%# Item.Count %>回目
																			</div>
																			<div Visible="<%# this.Input.IsOrderDeterminationTypePeriod %>" runat="server">
																				<uc:DateTimePickerPeriodInput
																					id="ucTermDateRange"
																					IsHideTime="True"
																					IsSetDefaultToday="True"
																					StartDate="<%# Item.TermSinceDate %>"
																					EndDate="<%# Item.TermUntilDate %>"
																					runat="server" />
																			</div>
																			<asp:Button ID="addDefaultProduct" runat="server" Text="商品追加" CommandName="add_subscription_box_default" CommandArgument="<%# Container.ItemIndex %>" Enabled='<%# CheckActivateAddProduct(string.Format("{0}/{1}/{2}", Item.Items[0].ShopId, Item.Items[0].ProductId, Item.Items[0].VariationId)) %>'/>
																		</div>
																	</td>
																</tr>
																<asp:Repeater
																	DataSource="<%# Item.Items %>"
																	ID="rDefaultSubItems"
																	ItemType="Input.SubscriptionBox.SubscriptionBoxDefaultSubItemInput"
																	OnItemCreated="rDefaultSubItems_OnItemCreated"
																	OnItemCommand="rDefaultSubItems_OnItemCommand"
																	runat="server">
																	<ItemTemplate>
																		<tr>
																			<td class="edit_item_bg" style="text-align: center">
																				<asp:CheckBox ID="cbNecessaryProductFlg" Checked='<%# Item.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID %>' Enabled='<%# CheckActivateAddProduct(string.Format("{0}/{1}/{2}", Item.ShopId, Item.ProductId, Item.VariationId)) %>' runat="server" />
																			</td>
																			<td class="edit_item_bg">
																				<asp:DropDownList ID="ddlDefaultItem" Items="<%# CreateDefaultItemDropDownDataSource(((IDataItemContainer)((RepeaterItem)Container.Parent.Parent)).DisplayIndex) %>" SelectedValue='<%# string.Format("{0}/{1}/{2}", Item.ShopId, Item.ProductId, Item.VariationId) %>' runat="server" AutoPostBack="True" Width="95%"/>
																			</td>
																			<td class="edit_item_bg" style="text-align: center">
																				<asp:TextBox ID="tbItemQuantity" runat="server" MaxLength="2" Text="<%# Item.ItemQuantity %>" Visible='<%# CheckActivateAddProduct(string.Format("{0}/{1}/{2}", Item.ShopId, Item.ProductId, Item.VariationId)) && CanDisplayItemQuantityWithFirstSelectableFlgValid(((RepeaterItem)Container.Parent.Parent).ItemIndex) %>' Width="50" /><br/>
																			</td>
																			<td class="edit_item_bg ta-center">
																				<asp:Button ID="btnDeleteProduct" runat="server" Text="削除" CommandName="delete" CommandArgument='<%# Item.BranchNo %>'/>
																			</td>
																			<asp:HiddenField ID="hfShopId" Value="<%# Item.ShopId %>" runat="server"/>
																			<asp:HiddenField ID="hfProductId" Value="<%# Item.ProductId %>" runat="server"/>
																			<asp:HiddenField ID="hfVariationId" Value="<%# Item.VariationId %>" runat="server"/>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
																<asp:HiddenField ID="hfCount" Value="<%# Item.Count %>" runat="server"/>
															</ItemTemplate>
														</asp:Repeater>
													</tbody>
												</table>
												<%--▽ 「作成日」「更新日」「更新者」 ▽--%>
												<% if (this.ShouldShowLastChanged) { %>
													<div runat="server">
														<br />
														<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tbody runat="server">
															<tr>
																<td class="edit_title_bg" align="left" width="15%">作成日</td>
																<td class="edit_item_bg">
																	<%#: DateTimeUtility.ToStringForManager(this.Input.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
																</td>
																<td class="edit_title_bg" align="left" width="15%">更新日</td>
																<td class="edit_item_bg">
																	<%#: DateTimeUtility.ToStringForManager(this.Input.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
																</td>
																<td class="edit_title_bg" align="left" width="15%">更新者</td>
																<td class="edit_item_bg">
																	<%#: this.Input.LastChanged %>
																</td>
															</tr>
															</tbody>
														</table>
													</div>
												<% } %>
												<%--△ 「作成日」「更新日」「更新者」 △--%>
												<div class="action_part_bottom">
													<asp:Button ID="btnBackListBottom" runat="server" Text="  一覧へ戻る  " onclick="btnBackList_Click" />
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " OnClick="btnCopyInsert_Click" />
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " OnClick="btnDelete_Click" OnClientClick="return confirm('頒布会コース設定を削除します。よろしいですか？');" />
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " OnClick="btnInsert_Click" />
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " OnClick="btnUpdate_Click" OnClientClick="return confirm('表示内容で更新します。よろしいですか？');" />
												</div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 登録編集 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>

<script type="text/javascript">
	function set_productinfo(product_id, supplier_id, v_id, product_name, limited_fixed_purchase_kbn1_setting, limited_fixed_purchase_kbn3_setting, taxRate, shipping_type) {
		$('#<%: hfVariationId.ClientID %>').val(v_id);
		$('#<%: hfProductId.ClientID %>').val(product_id);
		$('#<%: hfProductName.ClientID %>').val(product_name);
		$('#<%: hfShippingType.ClientID %>').val(shipping_type);
		__doPostBack('<%= lbSetSelectableProduct.UniqueID %>','');
	}

	// ページロード処理
	function bodyPageLoad(sender, args) {
		BindCartAddUrlCopyEvent(".btn-clipboard");
	}

	// カート投入URLコピーイベントのバインド処理
	function BindCartAddUrlCopyEvent(cssClass) {
		var cp = new ClipboardJS(cssClass);
		cp.on("success", function (e) {
			alert("コピーに成功しました。");
		});
		cp.on("error", function (e) {
			alert("コピーに失敗しました。");
		});
	}
</script>

</asp:Content>
