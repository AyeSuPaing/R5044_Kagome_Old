<%--
=========================================================================================================
  Module      : 商品同梱設定・編集ページ(ProductBundleRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register Src="~/Form/Common/KeepFormData.ascx" TagName="KeepFormData" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductBundleRegister.aspx.cs" Inherits="Form_ProductBundle_ProductBundleRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<script>
	// The selected product index
	var selected_product_index = 0;

	var product_search_kbn = "";

	// 商品一覧画面表示
	function open_product_list(link_file, window_name, window_type, index, popupProductSearchKbn) {
		var shipping_type_product_ids = '';

		product_search_kbn = popupProductSearchKbn;
		
		if (product_search_kbn == '<%= POPUP_PRODUCT_SEARCH_KBN_GRANT_PRODUCT %>') {
			// 定期商品のみ検索
			link_file += '&<%= Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT %>=<%= Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID %>';
		}
		else if ((product_search_kbn == '<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_PRODUCT %>')
			&& (product_search_kbn == '<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_VARIATION %>')
			&& (product_search_kbn == '<%= POPUP_PRODUCT_SEARCH_KBN_EXCEPT_PRODUCT %>')
			&& (product_search_kbn == '<%= POPUP_PRODUCT_SEARCH_KBN_EXCEPT_VARIATION %>'))
		{
			shipping_type_product_ids = GetShippingTypeProductIds(product_search_kbn);
			if (shipping_type_product_ids != ''){
				link_file += '&<%= Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS %>=' + encodeURIComponent(shipping_type_product_ids);
			}
		}

		// 選択商品を格納
		selected_product_index = index;

		open_window(link_file, window_name, window_type);
	}

	// 配送種別取得用の商品ID取得
	function GetShippingTypeProductIds(productSearchKbn) {
		var inputClientId = '<%= tbProductId.ClientID %>';
		
		var ids = $('#' + inputClientId).val();
		return (ids != '') ? '' : ids.replace(/\r?\n/, ',');
	}

	// 商品一覧で選択された商品情報を設定
	function set_productinfo(product_id, supplier_id, variation_id, product_name, product_display_price, product_special_price, product_price, sale_id, fixed_purchase_id, fixedpurchasekbn3setting)
	{
		switch (product_search_kbn){
			case '<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_PRODUCT %>':
				if (CheckProductExist('<%= tbProductId.ClientID %>', product_id)) break;
				AddProductItem('<%= tbProductId.ClientID %>', product_id);
				break;

			case '<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_VARIATION %>':
				var addValue = (variation_id.trim().length == 0) ? product_id : product_id + ',' + product_id + variation_id;
				if (CheckProductExist('<%= tbProductVariationId.ClientID %>', addValue)) break;
				AddProductItem('<%= tbProductVariationId.ClientID %>', addValue);
				break;

			case '<%= POPUP_PRODUCT_SEARCH_KBN_EXCEPT_PRODUCT %>':
				if (CheckProductExist('<%= tbExceptProductId.ClientID %>', product_id)) break;
				AddProductItem('<%= tbExceptProductId.ClientID %>', product_id);
				break;

			case '<%= POPUP_PRODUCT_SEARCH_KBN_EXCEPT_VARIATION %>':
				var addExcludedVariationValue = (variation_id.trim().length == 0) ? product_id : product_id + ',' + product_id + variation_id;
				if (CheckProductExist('<%= tbExceptProductVariationId.ClientID %>', addExcludedVariationValue)) break;
				AddProductItem('<%= tbExceptProductVariationId.ClientID %>', addExcludedVariationValue);
				break;

			case '<%= POPUP_PRODUCT_SEARCH_KBN_GRANT_PRODUCT %>':
				if (CheckGrantProductExist(product_id, variation_id)) return;
				<%
		var iLoop = 0;
		foreach (RepeaterItem item in rGrantProductIdList.Items)
		{
				%>
					if (selected_product_index == <%= iLoop %>) {
						document.getElementById('<%= ((TextBox)item.FindControl("tbGrantProductId")).ClientID %>').value = product_id;
						document.getElementById('<%= ((TextBox)item.FindControl("tbGrantProductVariationId")).ClientID %>').value = variation_id;
					}
				<%
			iLoop++;
		}
				%>
				break;
		}
	}

	// カテゴリ一覧で選択されたカテゴリIDをセット
	function set_categoryinfo(categoryId) {
		switch (product_search_kbn){
		case '<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_CATEGORY %>':
			if (CheckProductExist('<%= tbProductCategoryId.ClientID %>', categoryId)) break;
			AddProductItem('<%= tbProductCategoryId.ClientID %>', categoryId);
			break;

		case '<%= POPUP_PRODUCT_SEARCH_KBN_EXCEPT_PRODUCT_CATEGORY %>':
			if (CheckProductExist('<%= tbExceptProductCategoryId.ClientID %>', categoryId)) break;
			AddProductItem('<%= tbExceptProductCategoryId.ClientID %>', categoryId);
			break;
		}
	}

	// 広告コード一覧で選択された広告IDをセット
	function SelectAdvCode(advCode) {
		switch (product_search_kbn) {
		case '<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_ADVCODES_FIRST %>':
			if (CheckProductExist('<%= tbTargetAdvCodesFirst.ClientID %>', advCode)) break;
			AddProductItem('<%= tbTargetAdvCodesFirst.ClientID %>', advCode);
			break;

		case '<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_ADVCODES_NEW %>':
			if (CheckProductExist('<%= tbTargetAdvCodesNew.ClientID %>', advCode)) break;
			AddProductItem('<%= tbTargetAdvCodesNew.ClientID %>', advCode);
			break;
		}
	}

	// クーポンコードをセット
	function set_coupon_code(couponCode) {
		if (CheckProductExist('<%= tbTargetCouponCodes.ClientID %>', couponCode)) return;
		AddProductItem('<%= tbTargetCouponCodes.ClientID %>', couponCode);
	}

	// ターゲットリスト一覧で選択されたターゲットリストIDをセット
	function SetTargetList(text, data_count, value) {
		$('#<%= tbTargetList.ClientID %>').val(value);
	}

	// 商品ID・バリエーションIDの重複チェック
	function CheckProductExist(targetElement, addValue){
		var currentItems = $('#' + targetElement).val().replace("\r\n", "\n").split("\n");
		
		for (i = 0; i < currentItems.length; i++) {
			if (currentItems[i].trim() == addValue) {
				return true;
			}
		}
		return false;
	}

	// 商品ID・バリエーションIDを転記
	function AddProductItem(targetElement, addValue)
	{
		var currentItems = $('#' + targetElement).val();
		$('#' + targetElement).val(currentItems + ((currentItems.trim().length == 0) ? "" : "\r\n") + addValue);
	}

	// 同梱商品の重複チェック
	function CheckGrantProductExist(productId, variationId)
	{
		var result = false;

		$('#grant_item_table tr.tr_grant_items').each(function () {
			var grantProductId = $(this).find('input.grant_product_id').val();
			var grantProductVariationId = $(this).find('input.grant_product_variation_id').val();
			
			if ((productId == grantProductId) && (variationId == grantProductVariationId)) result = true;
		});

		return result;
	}
	</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<!--▽ Title ▽-->
	<tr><td><h1 class="page-title">商品同梱設定</h1></td></tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
	<td>
		<h1 class="cmn-hed-h2">商品同梱設定編集</h1>
	</td>
	</tr>
	<tr id="trEdit" runat="server" Visible="False">
	<td>
		<h1 class="cmn-hed-h2">編集ページ</h1>
	</td>
	</tr>
	<!--△ End title △-->

	<!--▽ Registration ▽-->
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
				<div id="divComp" runat="server" class="action_part_top" Visible="False">
				<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
					<tr class="info_item_bg">
						<td align="left"><asp:Label ID="lMessage" runat="server"></asp:Label>
						</td>
					</tr>
				</table>
				</div>
				<div class="action_part_top">
					<asp:Button id="btnBack" runat="server" Text="  一覧へ戻る  " OnClick="btnBack_Click" />
					<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
					<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
					<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
					<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" />
				</div>
				<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
				<tbody>
					<tr>
						<td class="edit_title_bg" align="left" width="35%" colspan="2">商品同梱ID<span class="notice">*</span></td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbBundleId" runat="server" Width="100" MaxLength="30"></asp:TextBox>
							<asp:Literal ID="lBundleId" runat="server"></asp:Literal>
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">商品同梱名<span class="notice">*</span></td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbBundleName" runat="server" Width="300" MaxLength="100"></asp:TextBox>
						</td>
					</tr>
					<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) {%>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">対象注文種別<span class="notice">*</span></td>
						<td class="edit_item_bg" align="left">
							<asp:RadioButtonList id="rblTargetOrderType" class="radioButtonList" Runat="server" RepeatDirection="Horizontal"
								AutoPostBack="true" CellPadding="50" CellSpacing="50" RepeatLayout="Flow"></asp:RadioButtonList>
						</td>
					</tr>
					<% if (rblTargetOrderType.SelectedValue != Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL) { %>
					<tr>
						<td class="auto-style1" align="left" colspan="2">対象定期注文回数（台帳出荷基準単位）
						</td>
						<td class="auto-style2" align="left">
							<asp:TextBox id="tbTargetOrderFixedPurchaseCountFrom" runat="server" Width="70" MaxLength="7"></asp:TextBox> 回目から
							<asp:TextBox id="tbTargetOrderFixedPurchaseCountTo" runat="server" Width="70" MaxLength="7"></asp:TextBox> 回目まで
							<br/>未入力の場合は「無制限」として設定されます。
						</td>
					</tr>
					<%} %>
					<%} %>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">重複適用フラグ</td>
						<td class="edit_item_bg" align="left">
							<asp:CheckBox ID="cbMultipleApplyFlg" runat="server" Text="重複適用"></asp:CheckBox>
							<br />
							重複適用フラグが有効な商品同梱設定は、1注文に対して他の商品同梱設定と重ねて適用します。
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">適用優先順<span class="notice">*</span></td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbApplyOrder" runat="server" Width="50" MaxLength="7"></asp:TextBox>
							<br />
							1つの注文に複数の商品同梱設定が該当する場合、適用優先順の昇順で設定を適用します。<br />
							※適用優先順が同じ場合は商品同梱IDの昇順に適用されます。
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">説明文</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbDescription" runat="server" Width="480" TextMode="MultiLine" Columns="80" Rows="3"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">開始日時<span class="notice">*</span> - 終了日時</td>
						<td class="edit_item_bg" align="left">
							<div id="endDate">
								<uc:DateTimePickerPeriodInput id="ucDisplayPeriod" runat="server" IsNullEndDateTime="true"/>
							</div>
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">商品同梱設定適用種別<span class="notice">*</span></td>
						<td class="edit_item_bg" align="left">
							<asp:RadioButtonList ID="rblApplyType" class="radioButtonList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">ユーザ利用可能回数<span class="notice">*</span></td>
						<td class="edit_item_bg" align="left">
							<asp:RadioButtonList id="rblUsableTimesKbn" class="radioButtonList" Runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" OnSelectedIndexChanged="rblUsableTimesKbn_OnSelectedIndexChanged" AutoPostBack="true"></asp:RadioButtonList>
							<span id="spanUsableTimes" runat="server" Visible="False">
								&nbsp;&nbsp;<asp:TextBox id="tbUsableTimes" runat="server" Width="100" MaxLength="100"></asp:TextBox>回
							</span>
						</td>
					</tr>
					<%if (Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE){%>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">ターゲットリストID<span class="notice"></span></td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbTargetList" runat="server" Width="200" MaxLength="100"></asp:TextBox>
							<input id="inputSearchTargetList" type="button" value="  検索  " onclick="javascript:open_product_list(
									'<%= WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(
										MenuAuthorityHelper.ManagerSiteType.Mp,
										(Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_TARGETLIST_LIST_POPUP))) %>',
								'SetTargetList',
								'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
								'0',
								'<%= POPUP_PRODUCT_SEARCH_KBN_TARGETLIST %>');" />
							<asp:CheckBox ID="cbTargetIdExceptFlg" runat="server" Text="除外する"></asp:CheckBox>
						</td>
					</tr>
					<%} %>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">対象注文の商品合計</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbTargetOrderPriceSubtotalMin" runat="server" Width="100" MaxLength="100"></asp:TextBox><%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : CurrencyManager.KeyCurrencyUnit %>以上
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">対象商品個数</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbTargetProductCountMin" runat="server" Width="100" MaxLength="100"></asp:TextBox>個以上
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">決済種別</td>
						<td class="edit_item_bg" align="left">
							<asp:CheckBoxList ID="cblTargetPaymentIds" runat="server" DataTextField="Key" DataValueField="Value" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxList>
						</td>
					</tr>
					<% if (Constants.W2MP_AFFILIATE_OPTION_ENABLED) { %>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">広告コード（初回分）<br />（<strong>改行区切り</strong>で入力してください）</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbTargetAdvCodesFirst" runat="server" Width="98%" TextMode="MultiLine" Columns="80" Rows="3"></asp:TextBox>
							<div style="float:left; margin:2px 11px 0px 10px">
								<span id="spSearchAdv" runat="server">
									<input type="button" value=" 検索 " onclick="javascript:open_product_list(
										'<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_ADVPOPUP +"?window_kbn=1") %>',
										'SelectAdvCode',
										'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
										'0',
										'<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_ADVCODES_FIRST %>');" />
								</span>
							</div>
							<div style="float:right; margin:5px 4px 0px 0px">
								<asp:LinkButton ID="lbTargetAdvCodesFirstResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbTargetAdvCodesFirstResize" Text="拡大する" CommandArgument="" />
								<asp:LinkButton ID="lbTargetAdvCodesFirstResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbTargetAdvCodesFirstResize" Text="縮小する" CommandArgument="" />
							</div>
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">広告コード（最新分）<br />（<strong>改行区切り</strong>で入力してください）</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbTargetAdvCodesNew" runat="server" Width="98%" TextMode="MultiLine" Columns="80" Rows="3"></asp:TextBox>
							<div style="float:left; margin:2px 11px 0px 10px">
								<span id="spSearchAdvNew" runat="server">
									<input type="button" value=" 検索 " onclick="javascript:open_product_list(
										'<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_ADVPOPUP +"?window_kbn=1") %>',
										'SelectAdvCode',
										'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
										'0',
										'<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_ADVCODES_NEW %>');" />
								</span>
							</div>
							<div style="float:right; margin:5px 4px 0px 0px">
								<asp:LinkButton ID="lbTargetAdvCodesNewResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbTargetAdvCodesNewResize" Text="拡大する" CommandArgument="" />
								<asp:LinkButton ID="lbTargetAdvCodesNewResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbTargetAdvCodesNewResize" Text="縮小する" CommandArgument="" />
							</div>
						</td>
					</tr>
					<% } %>
					<% if (this.UsableCouponSearch) { %>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">クーポンコード<br />（<strong>改行区切り</strong>で入力してください）</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox id="tbTargetCouponCodes" runat="server" Width="98%" TextMode="MultiLine" Columns="80" Rows="3" />
							<div style="float:left; margin:2px 11px 0px 10px">
								<span>
									<input type="button" value=" 検索 " onclick="javascript:window.open(
										'<%: GetCouponListPopupPage() %>',
										'クーポンリスト',
										'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes');" />
								</span>
							</div>
							<div style="float:right; margin:5px 4px 0px 0px">
								<asp:LinkButton ID="lbTargetCouponCodesResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbTargetCouponCodesResize" Text="拡大する" CommandArgument="" />
								<asp:LinkButton ID="lbTargetCouponCodesResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbTargetCouponCodesResize" Text="縮小する" CommandArgument="" />
							</div>
						</td>
					</tr>
					<% } %>
					<tr>
						<td class="edit_title_bg" align="left" colspan="2">有効フラグ</td>
						<td class="edit_item_bg" align="left">
							<asp:CheckBox ID="cbValidFlg" runat="server" Text="有効"></asp:CheckBox>
						</td>
					</tr>
				</tbody>
				</table>
				<br />
				<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr>
						<td class="edit_title_bg" align="left" width="20%"></td>
						<td class="edit_title_bg" align="center" width="30%">対象商品<br />（<strong>改行区切り</strong>で入力してください）</td>
						<td class="edit_title_bg" align="center" width="30%">対象外商品<br />（<strong>改行区切り</strong>で入力してください）</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left" width="20%" >指定方法<span class="notice">*</span></td>
						<td class="edit_item_bg" align="left">
							<asp:RadioButtonList ID="rblTargetProductKbn" class="radioButtonList" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server">
								<asp:ListItem Text="条件指定" Value="SELECT" />
								<asp:ListItem Text="全商品" Value="ALL" />
							</asp:RadioButtonList>
						</td>
						<td class="edit_item_bg" align="left"></td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left">商品ID</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox ID="tbProductId" runat="server" Width="98%" TextMode="MultiLine" Text="" />
							<div style="float:left; margin:2px 11px 0px 10px">
								<span id="spSearchProduct" runat="server">
									<input type="button" value=" 検索 " onclick="javascript:open_product_list(
										'<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT
											+ Constants.PAGE_MANAGER_PRODUCT_SEARCH
											+ "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN
											+ "=" + Constants.KBN_PRODUCT_SEARCH_PRODUCT
											+ "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG
											+ "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>',
										'product_search',
										'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
										'0',
										'<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_PRODUCT %>');" />
								</span>
							</div>
							<div style="float:right; margin:5px 4px 0px 0px">
								<asp:LinkButton ID="lbProductResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbProductResize" Text="拡大する" CommandArgument="" />
								<asp:LinkButton ID="lbProductResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbProductResize" Text="縮小する" CommandArgument="" />
							</div>
						</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox ID="tbExceptProductId" runat="server" Width="98%" TextMode="MultiLine" Text="" />
							<div style="float:left; margin:2px 11px 0px 10px">
								<span id="spSearchExcludedProduct" runat="server">
									<input type="button" value=" 検索 " onclick="javascript:open_product_list(
										'<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT
											+ Constants.PAGE_MANAGER_PRODUCT_SEARCH
											+ "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN
											+ "=" + Constants.KBN_PRODUCT_SEARCH_PRODUCT
											+ "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG
											+ "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>',
										'product_search',
										'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
										'0',
										'<%= POPUP_PRODUCT_SEARCH_KBN_EXCEPT_PRODUCT %>');" />
								</span>
							</div>
							<div style="float:right; margin:5px 4px 0px 0px">
								<asp:LinkButton ID="lbExceptProductResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbExceptProductResize" Text="拡大する" CommandArgument="" />
								<asp:LinkButton ID="lbExceptProductResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbExceptProductResize" Text="縮小する" CommandArgument="" />
							</div>
						</td>
					</tr>
					<tr>
						<td class="edit_title_bg" align="left">
							バリエーションID<br />
							(<b>「商品ID,バリエーションID」</b>の形式で<br/>入力してください)
						</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox ID="tbProductVariationId" runat="server" Width="98%" TextMode="MultiLine" Text="" />
							<div style="float:left; margin:2px 11px 0px 10px">
								<span id="spSearchProductVariation" runat="server">
									<input type="button" value=" 検索 " onclick="javascript:open_product_list(
										'<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT
											+ Constants.PAGE_MANAGER_PRODUCT_SEARCH
											+ "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN
											+ "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION
											+ "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG
											+ "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>',
										'product_search',
										'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
										'0',
										'<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_VARIATION %>');" />
								</span>
							</div>
							<div style="float:right; margin:5px 4px 0px 0px">
								<asp:LinkButton ID="lbProductVariationResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbProductVariationResize" Text="拡大する" CommandArgument="" />
								<asp:LinkButton ID="lbProductVariationResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbProductVariationResize" Text="縮小する" CommandArgument="" />
							</div>
						</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox ID="tbExceptProductVariationId" runat="server" Width="98%" TextMode="MultiLine" Text="" />
							<div style="float:left; margin:2px 11px 0px 10px">
								<span id="spSearchExcludedProductVariation" runat="server">
									<input type="button" value=" 検索 " onclick="javascript:open_product_list(
										'<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH
											+ "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN
											+ "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION
											+ "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG
											+ "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>',
										'product_search',
										'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
										'0',
										'<%= POPUP_PRODUCT_SEARCH_KBN_EXCEPT_VARIATION %>');" />
								</span>
							</div>
							<div style="float:right; margin:5px 4px 0px 0px">
								<asp:LinkButton ID="lbExceptProductVariationResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbExceptProductVariationResize" Text="拡大する" CommandArgument="" />
								<asp:LinkButton ID="lbExceptProductVariationResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbExceptProductVariationResize" Text="縮小する" CommandArgument="" />
							</div>
						</td>
					</tr>
					<tr id="trProductCategoryID" visible="false" runat="server">
						<td class="edit_title_bg" align="left">商品カテゴリID</td>
						<td class="edit_item_bg" align="left">
							<asp:TextBox ID="tbProductCategoryId" runat="server" Width="98%" TextMode="MultiLine" Text="" />
							<div style="float:left; margin:2px 11px 0px 10px">
								<span id="spSearchProductCategory" runat="server">
									<input type="button" value=" 検索 " onclick="javascript:open_product_list(
										'<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_CATEGORY_SEARCH) %>',
										'set_categoryinfo',
										'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
										'0',
										'<%= POPUP_PRODUCT_SEARCH_KBN_TARGET_CATEGORY %>');" />
								</span>
							</div>
							<div style="float:right; margin:5px 4px 0px 0px">
								<asp:LinkButton ID="lbProductCategoryResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbProductCategoryResize" Text="拡大する" CommandArgument="" />
								<asp:LinkButton ID="lbProductCategoryResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbProductCategoryResize" Text="縮小する" CommandArgument="" />
							</div>
						</td>
							<td class="edit_item_bg" align="left">
							<asp:TextBox ID="tbExceptProductCategoryId" runat="server" Width="98%" TextMode="MultiLine" Text="" />
							<div style="float:left; margin:2px 11px 0px 10px">
								<span id="spSearchExcludedProductCategory" runat="server">
									<input type="button" value=" 検索 " onclick="javascript:open_product_list(
										'<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_CATEGORY_SEARCH) %>',
										'product_search',
										'width=850,height=700,top=120,left=420,status=NO,scrollbars=yes',
										'0',
										'<%= POPUP_PRODUCT_SEARCH_KBN_EXCEPT_PRODUCT_CATEGORY %>');" />
								</span>
							</div>
							<div style="float:right; margin:5px 4px 0px 0px">
								<asp:LinkButton ID="lbExceptProductCategoryResizeLarge" runat="server" OnClick="lbResize_Click" CommandName="lbExceptProductCategoryResize" Text="拡大する" CommandArgument="" />
								<asp:LinkButton ID="lbExceptProductCategoryResizeNormal" runat="server" OnClick="lbResize_Click" CommandName="lbExceptProductCategoryResize" Text="縮小する" CommandArgument="" />
							</div>
						</td>
					</tr>
				</table>
				<br />
				<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
					<tr>
						<td class="edit_title_bg" align="left" width="35%">同梱商品<span class="notice">*</span></td>
						<td class="edit_title_bg" style="padding: 0px 0px 2px 0px" align="left" >
							<table id="grant_item_table" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tbody>
								<tr>
									<td class="edit_title_bg" align="center" style="border-bottom: 1px solid #c0c0c0; border-right: 1px solid #c0c0c0;">商品ID</td>
									<td class="edit_title_bg" align="center" style="border-bottom: 1px solid #c0c0c0; border-right: 1px solid #c0c0c0;">バリエーションID</td>
									<td class="edit_title_bg" align="center" style="border-bottom: 1px solid #c0c0c0; border-right: 1px solid #c0c0c0;">数量</td>
									<td class="edit_title_bg" align="center" style="border-bottom: 1px solid #c0c0c0; border-right: 1px solid #c0c0c0;">初回のみ同梱</td>
									<td class="edit_title_bg" align="center" style="border-bottom: 1px solid #c0c0c0;">
										<asp:Button runat="server" ID="btnTargetProductIdAdd" Text="  追加  " OnClick="btnTargetProductIdAdd_Click" />
									</td>
								</tr>
									<asp:Repeater ID="rGrantProductIdList" OnItemCommand="rGrantProductIdList_ItemCommand" ItemType="ProductBundleItemInput" runat="server">
										<ItemTemplate>
											<tr class="tr_grant_items">
												<td id="tdGrantProductId" class="edit_item_bg" align="left" width="25%" style="border-right: 1px solid #c0c0c0;">
													<asp:TextBox id="tbGrantProductId" CssClass="grant_product_id" runat="server" style="margin-top: 3px; width: 92%;" Text="<%# Item.GrantProductId %>"></asp:TextBox>
												</td>
												<td id="tdGrantProductVariationId" class="edit_item_bg" align="left" width="25%" style="border-right: 1px solid #c0c0c0;">
													<asp:TextBox ID="tbGrantProductVariationId" CssClass="grant_product_variation_id" runat="server" style="margin-top: 3px; width: 92%;" Text="<%# Item.GrantProductVariationId %>"></asp:TextBox>
												</td>
												<td id="tdGrantProductCount" class="edit_item_bg" align="left" width="15%" style="border-right: 1px solid #c0c0c0;">
													x <asp:TextBox id="tbGrantProductCount" CssClass="grant_product_count" runat="server" style="margin-top: 3px; width: 75%" Text="<%# Item.GrantProductCount %>"></asp:TextBox>
												</td>
												<td id="tdOrderedProductExceptFlg" class="edit_item_bg" align="center" width="10%" style="border-right: 1px solid #c0c0c0;">
													<asp:CheckBox ID="cbOrderedProductExceptFlg" runat="server" Checked="<%# Item.OrderedProductExceptFlg %>"></asp:CheckBox>
												</td>
												<td id="tdButtons" class="edit_item_bg" width="18%">
													<input id="inputSearchProduct" type="button" value="  検索  "
														onclick="javascript:open_product_list('<%= WebSanitizer.UrlAttrHtmlEncode(Constants.PATH_ROOT
																									+ Constants.PAGE_MANAGER_PRODUCT_SEARCH
																									+ "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN
																									+ "=" + Constants.KBN_PRODUCT_SEARCH_VARIATION
																									+ "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG
																									+ "=" + Constants.FLG_PRODUCT_VALID_FLG_VALID) %>'
																	,'product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes', '<%# Container.ItemIndex %>	', '<%# POPUP_PRODUCT_SEARCH_KBN_GRANT_PRODUCT %>');" />
													<asp:Button runat="server" ID="btnTargetProductIdDelete" Text="  削除  " CommandName="Delete" CommandArgument="<%# Container.ItemIndex %>" />
												</td>
											</tr>
										</ItemTemplate>
									</asp:Repeater>
								</tbody>
							</table>
						</td>
					</tr>
				</table>
				<div class="action_part_bottom">
					<asp:Button id="btnBackBottom" runat="server" Text="  一覧へ戻る  " OnClick="btnBack_Click" />
					<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" OnClick="btnCopyInsert_Click" />
					<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" OnClick="btnDelete_Click" OnClientClick="return confirm('情報を削除してもよろしいですか？');" />
					<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" OnClick="btnInsert_Click" />
					<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" OnClick="btnUpdate_Click" />
				</div>
				<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
					<tr class="info_item_bg">
						<td align="left">
							備考<br />
							【商品同梱処理について】<br />
							 ・商品同梱処理概要<br />
							  対象商品のいずれかが購入された場合に実行されます。<br />
							  同梱商品は、0円かつ送料無料で同梱されます。<br />
							<br />
							 ・商品同梱数量<br />
							  商品同梱設定適用種別により同梱数量の計算方法が変化します。<br />
							  ただし、商品区分が「チラシ」の商品を同梱商品に設定した場合、「数量設定に関わらず必ず数量1」として同梱されます。<br />
							  〇注文単位<br />
							   対象商品を1つ以上含む注文である場合に同梱商品を数量設定に応じて同梱します。<br />
							   例）対象商品を「商品1」、「商品2」の2種類、同梱商品を「同梱商品1」を数量3とした設定が適用される場合<br />
								「商品1」「商品2」のいずれかを2つ購入 > 「同梱商品1」を3つ同梱<br />
								「商品1」「商品2」を両方2つ購入 > 「同梱商品1」を3つ同梱<br />
								「商品1」「商品2」以外の商品を購入 > 同梱なし<br />
							  〇購入商品単位<br />
							   対象商品の購入1つにつき、同梱商品を数量設定に応じて同梱します。<br />
							   例）対象商品を「商品1」、「商品2」の2種類、同梱商品を「同梱商品1」を数量2、「チラシ1」を数量2とした設定が適用される場合<br />
								「商品1」を2つ購入 > 「同梱商品1」を4つ、「チラシ1」を1つ同梱<br />
								「商品1」と「商品2」を1つずつ購入 > 「同梱商品1」を4つ、「チラシ1」を1つ同梱<br />
								「商品2」を1つ購入 > 「同梱商品1」を2つ、「チラシ1」を1つ同梱<br />
							<br />
							【対象注文種別について】<br />
							 ・対象注文種別は、どういった注文内容の際に商品同梱処理が実行されるかを設定します。<br />
							  「通常注文」 > 定期商品を「含まない」注文を行った際に商品同梱処理が実行されます。<br />
							  「定期注文」 > 定期商品を「含む」注文を行った際に商品同梱処理が実行されます。<br />
							  「すべての注文」 > 定期商品の購入有無を問わず商品同梱処理が実行されます。<br />
							<br />
							【ユーザ利用可能回数について】<br />
							・「対象注文種別」の条件に関わらず、この商品同梱設定を同一のユーザーに対して適用した回数を制限します。<br />
							<br />
							【初回のみ同梱について】<br />
							 ・「対象注文種別」の条件に関わらず、過去同梱された商品を再度付与するかを制限します。<br />
							<br />
							【対象注文の商品合計について】<br />
							 ・同梱対象となる注文の商品合計を適用条件として利用しています。<br />
							 ・割引、手数料などを考慮しません。<br />
							<br />
							【対象商品個数について】<br />
							 ・同梱対象となる注文のうち、対象商品に設定されている商品の個数を適用条件として利用しています。<br />
							<% if (Constants.PRODUCT_SET_OPTION_ENABLED) { %>
							 ・商品セット数を考慮します。<br />
							<% } %>
							<br />
							【広告コードについて】<br />
							・広告コード（初回分）は、購入したユーザーに設定されている広告コードを適用対象にします。<br />
							・広告コード（最新分）は、注文時設定されている広告コードを利用している場合に適用対象となります。<br />
							<br />
							<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
							【クーポンコードについて】<br />
							・設定したクーポンが購入時に使用された場合に、商品を同梱します。<br />
							・商品同梱が適用されない場合でも、クーポンは消費されます。<br />
							<br />
							<% } %>
							【商品同梱設定の注意点】<br />
							 ・対象商品と同梱商品の配送種別が異なる場合、商品同梱設定登録時にエラーとなります。<br />
							 ・商品情報変更により対象商品と同梱商品の配送種別が一致しない場合、商品同梱処理は実行されなくなります。<br />
							 ・対象商品と対象外商品をまとめて購入する時、対象外商品の設定が優先されます。<br />
							 ・「除外する」チェックボックスにチェックをONにする場合、指定したターゲットリスト以外のユーザー情報を対象にします。<br />
						</td>
					</tr>
				</table>
				<br />
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
	<!--△ End registration △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<asp:HiddenField ID="hfCurrentPopup" runat="server" />
<asp:HiddenField ID="hfCurrentTargetItem" runat="server" />
<!--▽ Keep form data. It use when come back from Error Page ▽-->
<uc:KeepFormData ID="KeepFormData" runat="server" />
<!--△ Keep form data. It use when come back from Error Page △-->
</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="ContentPlaceHolderHead">
	<style type="text/css">
		.auto-style1
		{
			background-color: #e7e7e7;
			height: 27px;
		}
		.auto-style2
		{
			background-color: #f8f8f8;
			height: 27px;
		}
		.radioButtonList label {
			margin-right: 10px;
			vertical-align: middle;
		}
	</style>
</asp:Content>
