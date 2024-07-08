<%--
=========================================================================================================
  Module      : 定期台帳編集入力ページ(FixedPurchaseModifyInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="FixedPurchaseModifyInput.aspx.cs" Inherits="Form_FixedPurchase_FixedPurchaseModifyInput" %>
<%-- ▼削除禁止：クレジットカードTokenコントロール▼ --%>
<%@ Register TagPrefix="uc" TagName="CreditToken" Src="~/Form/Common/CreditToken.ascx" %>
<%-- ▲削除禁止：クレジットカードTokenコントロール▲ --%>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.Domain.Payment" %>
<%@ Import Namespace="w2.Common.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
	$(function () {
		var $setScheduleFlg = $("#<%= cbScheduleAutoComputeFlg.ClientID %>");
		var $schedule = $("#scheduleSetting");

		// スケジュール指定フラグチェックボックス クリックイベント設定
		$setScheduleFlg.click(function () {
			turnVisible($setScheduleFlg.prop("checked"), $schedule);
		});

		// 自動計算フラグチェックボックス クリックイベント設定
		var $autoCalcFlg = $("#<%= cbNextNextShippingDateAutoComputeFlg.ClientID %>");
		var $nextNextShippingYear = $('#<%= ucNextNextShippingDate.DdlYear.ClientID %>');
		var $nextNextShippingMonth = $('#<%= ucNextNextShippingDate.DdlMonth.ClientID %>');
		var $nextNextShippingDay = $('#<%= ucNextNextShippingDate.DdlDay.ClientID %>');
		$autoCalcFlg.click(function () {
			var flg = $autoCalcFlg.prop("checked");
			turnValid(flg, $nextNextShippingYear);
			turnValid(flg, $nextNextShippingMonth);
			turnValid(flg, $nextNextShippingDay);
		});

		//---------------------------------------
		// 表示非表示切り替え処理
		//---------------------------------------
		var turnVisible = function (flg, $control) {
			$control.css({ "display": (flg ? "none" : "block") });
		}

		//---------------------------------------
		// 有効無効切り替え処理
		//---------------------------------------
		var turnValid = function (flg, $control) {
			$control.prop("disabled", flg);
		}

		// ページ読み込み時に一回実行
		turnVisible($setScheduleFlg.prop("checked"), $schedule);
		turnValid($autoCalcFlg.prop("checked"), $nextNextShippingYear);
		turnValid($autoCalcFlg.prop("checked"), $nextNextShippingMonth);
		turnValid($autoCalcFlg.prop("checked"), $nextNextShippingDay);
	});
</script>
<script type="text/javascript">
	<!--
	// 選択商品
	var selected_product_index = 0;

// 商品一覧画面表示
	function open_product_list(link_file, window_name, window_type, index) {
		if(link_file.includes("<%= Constants.KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX %>")) {
			// 選択商品を格納
			selected_product_index = index;
			// ウィンドウ表示
			open_window(link_file, window_name, window_type);
		} else {
		var shipping_type_product_ids = '';
		<% foreach(RepeaterItem ri in rItemList.Items){ %>
		<%-- 配送料種別取得用商品ID連結 --%>
		var product_id = $('#<%= ((TextBox)ri.FindControl("tbProductId")).ClientID %>').val();
		if (product_id != '') {
			if (shipping_type_product_ids != '') shipping_type_product_ids += ','
			shipping_type_product_ids += product_id;
		}
		<% } %>
		link_file += '&<%= Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS %>=' + encodeURIComponent(shipping_type_product_ids);

		// 選択商品を格納
		selected_product_index = index;
		// ウィンドウ表示
		open_window(link_file, window_name, window_type);
	}
	}

// 商品一覧で選択された商品情報を設定
	function set_productinfo(product_id, supplier_id, variation_id, product_name, product_display_price, product_special_price, product_price, sale_id, fixed_purchase_id) {
		<%
		// 注文商品数分ループ
		int loop = 0;
		foreach (RepeaterItem ri in rItemList.Items)
		{
		%>
		if (selected_product_index == <%= loop %>)
		{
			document.getElementById('<%= ((TextBox)ri.FindControl("tbProductId")).ClientID %>').value = product_id;
			document.getElementById('<%= ((TextBox)ri.FindControl("tbVariationId")).ClientID %>').value = variation_id;
			document.getElementById('<%= ((TextBox)ri.FindControl("tbItemQuantity")).ClientID %>').value = 1;
			if (product_id != '')
			{
				document.getElementById('<%= btnReCalculate.ClientID %>').click();
			}
		}
		<%
			loop++;
		}
		%>
	}

	
	// PayTg：PayTg端末状態確認
	function execGetPayTgDeviceStatus(apiUrl) {
		<% if(Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED) { %>
		mockWindow = window.open(apiUrl, 'CheckDeviceStatusPayTgMock', 'width=500,height=300,top=120,left=420,status=NO,scrollbars=no');
		<% } else { %>
		var requestCheckDeviceStatus = $.ajax({
			url: apiUrl,
			type: "GET",
			dataType: "json",
			cache: false
		});

		requestCheckDeviceStatus.done(function (data) {
			document.getElementById('<%= hfCanUseDevice.ClientID%>').value = data["canUseDevice"];
			document.getElementById('<%= hfStateMessage.ClientID%>').value = data["stateMessage"];
		})
		<% } %>
	}

	// PayTg：端末状態確認モックのレスポンス取得
	function getResponseFromCheckDeviceStatusMock(result) {
		// モック画面閉じる
		mockWindow.close();
		setTimeout(function () {
			var jsonRes = JSON.parse(result);
			document.getElementById('<%= hfCanUseDevice.ClientID%>').value = jsonRes["canUseDevice"];
			document.getElementById('<%= hfStateMessage.ClientID%>').value = jsonRes["stateMessage"];
			if (jsonRes["canUseDevice"] === "false" || jsonRes["stateMessage"] === "未接続") {
				alert('決済端末に接続できません。再度お試しください。');
			}
		}, 100);
	}

	// PayTg：カード登録実行
	function execCardRegistration(url) {
		lockScreen();
		hideConfirmButtonArea();
		<% if (Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED) { %>
		mockWindow = window.open(url, 'RegisterCardMock', 'width=750,height=550,top=120,left=420');
		mockWindow.onbeforeunload = function () {
		};
		<% } else { %>
			// PayTG専用端末の状態チェック
			var requestCheckDevice = $.ajax({
				url: "<%= Constants.PAYMENT_SETTING_PAYTG_DEVICE_STATUS_CHECK_URL %>",
				type: "GET",
				dataType: "json",
				cache: false
			});

		requestCheckDevice.done(function(data) {
			if (data["canUseDevice"] === true) {
				registerCreditCard(url);
			} else {
				unlockScreen(false, false);
				showConfirmButtonArea();
			}
		});

		requestCheckDevice.fail(function(error) {
			unlockScreen(false, false);
			console.log(error);
			showConfirmButtonArea();
		});
		<% } %>
		return false;
	}

	// PayTg：クレジットカード登録
	function registerCreditCard(url) {
		var postData = JSON.parse($('#<%= hfPayTgPostData.ClientID %>').val());
		// null値を含まないようにデータを整形する
		var cleanedData = {};
		for (var key in postData) {
			if (postData[key] !== null) {
				cleanedData[key] = postData[key];
			}
		}
		var requestRegisterCard = $.ajax({
			url: url,
			type: "POST",
			contentType: 'application/json',
			data: JSON.stringify(cleanedData),
			cache: false
		});
		requestRegisterCard.done(function (result) {
			// PayTG連携のレスポンスはHiddenFieldに保持する
			$('#<%= hfPayTgResponse.ClientID %>').val(JSON.stringify(result));
			// サーバー側でPayTG連携のレスポンス処理を行う
			$('#<%= btnProcessPayTgResponse.ClientID %>').click();
			// ロック画面を解除
			unlockScreen((result["mstatus"] === "success"), true);
		});
		requestRegisterCard.fail(function (error) {
			console.log(error);
			unlockScreen(false, false);
		});

		return false;
	}

	// PayTg：カード登録モックのレスポンス取得
	function getResponseFromMock(result) {
		// モック画面閉じる
		mockWindow.close();
		setTimeout(function () {
			// ロック画面を解除
			var jsonRes = JSON.parse(result);
			unlockScreen((jsonRes["mstatus"] === "success"), true);
			showConfirmButtonArea();

			// レスポンスはHiddenFieldに保持する
			$('#<%= hfPayTgResponse.ClientID %>').val(result);
			// サーバー側でPayTG連携のレスポンス処理を行う
			$('#<%= btnProcessPayTgResponse.ClientID %>').click();
		}, 100);
	}

	// Show confirm button area
	function showConfirmButtonArea() {
		$('.action_part_bottom').show();
	}

	// Hide confirm button area
	function hideConfirmButtonArea() {
		$('.action_part_bottom').hide();
	}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr id="trTitleFixedpurchaseTop" runat="server">
	</tr>
	<tr id="trTitleFixedpurchaseMiddle" runat="server">
		<td><h1 class="page-title">定期台帳</h1></td>
	</tr>
	<tr id="trTitleFixedpurchaseBottom" runat="server">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr>
		<td><h1 class="cmn-hed-h2">定期台帳編集</h1></td>
	</tr>
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<div class="action_part_top">
													<span id="spanUpdateHistoryConfirmTop" runat="server" visible =" false">( <a href="javascript:open_window('<%= UpdateHistoryPage.CreateUpdateHistoryConfirmUrl(Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_FIXEDPURCHASE, this.FixedPurchaseContainer.UserId, this.FixedPurchaseContainer.FixedPurchaseId) %>','updatehistory','width=980,height=850,top=5,left=600,status=NO,scrollbars=yes,resizable=yes')">履歴参照</a> )</span>
													<asp:Button id="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnConfirmTop" runat="server" OnClientClick="if(checkClickConfirm() == false) {return;} doPostbackEvenIfCardAuthFailed=false;" UseSubmitBehavior="False" Text="  確認する  " OnClick="btnConfirm_Click" />
												</div>
												<%--▽ 決済情報 ▽--%>
												<div id="dvFixedPurchasePayment" runat="server">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ 支払方法エラー表示 ▽--%>
													<tr id="trFixedPurchasePaymentErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="2">エラーメッセージ</td>
													</tr>
													<tr id="trFixedPurchasePaymentErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="2">
															<asp:Label ID="lbFixedPurchasePaymentErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ 支払方法エラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">決済情報</td>
													</tr>
													<tr>
														<td class="edit_title_bg" width="120">支払方法 <span class="notice">*</span></td>
														<td class="edit_item_bg" width="630">
															<asp:DropDownList ID="ddlOrderPaymentKbn" runat="server" OnSelectedIndexChanged="ddlOrderPaymentKbn_SelectedIndexChanged" AutoPostBack="true" Width="180" /><br/>
															<asp:Literal ID="lbOrderPaymentInfo" runat="server" />
															<p><asp:Label ID="lbPaymentUserManagementLevelMessage" runat="server" ForeColor="red" /></p>
															<p><asp:Label ID="lbPaymentOrderOwnerKbnMessage" runat="server" ForeColor="red" /></p>
														</td>
													</tr>
													<%-- ▽クレジット決済場合は表示▽ --%>
													<tr id="trPaymentKbnCredit" runat="server">
														<td class="edit_title_bg">クレジットカード選択 <span class="notice">*</span></td>
														<td class="edit_item_bg">
															<div style="margin-bottom:10px"></div>
															<%-- ▽現在利用しているクレジットカードを利用する▽ --%>
															<div id="divUseNowCreditCard" runat="server">
															<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="625">
																<tr class="edit_title_bg">
																	<td align="left">
																		<asp:RadioButton ID="rbUseNowCreditCard" GroupName="SelectCreditCard" runat="server" OnCheckedChanged="rbCreditCard_OnCheckedChanged" AutoPostBack="true" Text=" 現在利用しているクレジットカードを利用する "  /></td>
																</tr>
																<tr id="trUseNowCreditCard" runat="server" class="edit_item_bg">
																	<td align="left">
																		<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="600">
																			<%if (OrderCommon.CreditCompanySelectable) {%>
																			<tr>
																				<td class="edit_title_bg" width="120">カード会社</td>
																				<td class="edit_item_bg"><asp:Literal id="lUseNowCreditCardCopmanyName" runat="server" /></td>
																			</tr>
																			<%} %>
																			<tr>
																				<td class="edit_title_bg" width="120">カード番号</td>
																				<td class="edit_item_bg">************<asp:Literal id="lUseNowCreditCardLastFourDigit" runat="server" /></td>
																			</tr>
																			<tr>
																				<td class="edit_title_bg">有効期限</td>
																				<td class="edit_item_bg"><asp:Literal id="lUseNowCreditCardExpirationMonth" runat="server" />/<asp:Literal id="lUseNowCreditCardExpirationYear" runat="server" /> (月/年)</td>
																			</tr>
																			<tr>
																				<td class="edit_title_bg">カード名義人</td>
																				<td class="edit_item_bg"><asp:Literal id="lUseNowCreditCardAuthorName" runat="server" /></td>
																			</tr>
																			<tr id="trUseNowCreditCardSecurityCode" runat="server">
																				<td class="edit_title_bg">セキュリティコード</td>
																				<td class="edit_item_bg">****</td>
																			</tr>
																			<%-- ▽分割支払い有効の場合は表示▽ --%>
																			<tr id="trUseNowCreditCardInstallments" runat="server">
																				<td class="edit_title_bg">支払回数 <span class="notice">*</span></td>
																				<td class="edit_item_bg"><asp:DropDownList id="ddlUseNowCreditCardInstallments" runat="server"></asp:DropDownList>&nbsp;&nbsp;※AMEX/DINERSは一括のみとなります。</td>
																			</tr>
																			<%-- △分割支払い有効の場合は表示△ --%>
																		</table>
																	</td>
																</tr>
															</table>
															<div style="margin-bottom:10px"></div>
															</div>
															<%-- △現在利用しているクレジットカードを利用する△ --%>
															<%-- ▽新しいクレジットカードを利用する▽ --%>
															<asp:PlaceHolder ID="phNewCreditCard" Visible="<%# (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) %>" runat="server">
															<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="625">
																<tr class="edit_title_bg">
																	<td align="left">
																		<asp:RadioButton ID="rbNewCreditCard" GroupName="SelectCreditCard" runat="server" OnCheckedChanged="rbCreditCard_OnCheckedChanged" AutoPostBack="true" Text=" 新しいクレジットカードを利用する "  /></td>
																</tr>
																<tbody id="tbodyNewCreditCard" runat="server">
																	<tr class="edit_item_bg">
																	<td align="left">
																		<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="600">
																			<%--▼▼▼ カード情報入力フォーム表示 ▼▼▼--%>
																			<%if (this.CanUseCreditCardNoForm) {%>
																			<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
																				<tbody id="divCreditCardNoToken" runat="server">
																					<%if (OrderCommon.CreditCompanySelectable) { %>
																						<tr>
																							<td class="edit_title_bg">カード会社<span class="notice">*</span></td>
																							<td class="edit_item_bg"><asp:DropDownList id="ddlCreditCardCompany" runat="server"></asp:DropDownList></td>
																						</tr>
																					<%} %>
																					<tr>
																						<td class="edit_title_bg" width="150">
																							<%if (this.CreditTokenizedPanUse) {%>永久トークン<%}else{%>カード番号<%} %><span class="notice">*</span>
																						</td>
																						<td id="tdCreditNumber" class="edit_item_bg" runat="server"><asp:TextBox id="tbCreditCardNo1" pattern="[0-9]*" Width="160" MaxLength="16" autocomplete="off" runat="server"></asp:TextBox></td>
																						<td id="tdGetCardInfo" class="edit_item_bg" runat="server"><asp:Button id="btnGetCreditCardInfo" Text="  決済端末と接続  " onClick="btnGetCardInfo_Click" runat="server"/>※決済端末と接続ボタンを押下したあと、決済端末でカード番号を入力してください。</td>
																						<div id="payTgModal" class="payTgModal">
																							<div class="payTgModalOuter">
																								<div class="payTgModalMargin">
																								</div>
																								<div class="payTgModalContents">
																									<h1 style="font-size: 16px;">PayTG決済結果待機中・・・</h1><br/>
																									<h1 style="font-size: 16px;">テンキー端末の操作を完了してください。</h1>
																								</div>
																							</div>
																						</div>
																					</tr>
																					<tr id="trCreditExpire" runat="server">
																						<td class="edit_title_bg">有効期限<span class="notice">*</span></td>
																						<td class="edit_item_bg"><asp:DropDownList id="ddlCreditExpireMonth" runat="server"></asp:DropDownList>/<asp:DropDownList id="ddlCreditExpireYear" runat="server"></asp:DropDownList> (月/年)</td>
																					</tr>
																					<tr>
																						<td class="edit_title_bg">カード名義人<span class="notice">*</span></td>
																						<td class="edit_item_bg"><asp:TextBox id="tbCreditAuthorName" runat="server" Width="180" autocomplete="off"></asp:TextBox></td>
																					</tr>
																					<tr id="trSecurityCode" runat="server">
																						<td class="edit_title_bg">セキュリティコード</td>
																						<td class="edit_item_bg"><asp:TextBox id="tbCreditSecurityCode" runat="server" Width="60" MaxLength="4" autocomplete="off"></asp:TextBox></td>
																					</tr>
																				</tbody>
																				<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>
																				<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
																				<tbody id="divCreditCardForTokenAcquired" runat="server">
																					<%if (OrderCommon.CreditCompanySelectable) {%>
																						<tr>
																							<td class="edit_title_bg">カード会社</td>
																							<td class="edit_item_bg"><asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server"></asp:Literal></td>
																						</tr>
																					<%} %>
																					<tr>
																						<td class="edit_title_bg">カード番号</td>
																						<td class="edit_item_bg">
																							************<asp:Literal ID="lLastFourDigitForTokenAcquired" runat="server"></asp:Literal>
																							<asp:LinkButton id="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server">再入力</asp:LinkButton>
																						</td>
																					</tr>
																					<tr>
																						<td class="edit_title_bg">有効期限</td>
																						<td class="edit_item_bg">
																							<asp:Literal ID="lExpirationMonthForTokenAcquired" runat="server"></asp:Literal>
																							/
																							<asp:Literal ID="lExpirationYearForTokenAcquired" runat="server"></asp:Literal>
																							(月/年)
																						</td>
																					</tr>
																					<tr>
																						<td class="edit_title_bg">カード名義人</td>
																						<td class="edit_item_bg">
																							<asp:Literal ID="lCreditAuthorNameForTokenAcquired" runat="server"></asp:Literal>
																						</td>
																					</tr>
																				</tbody>
																			<%--▲▲▲ カード情報入力フォーム表示 ▲▲▲--%>
																			<%} else {%>
																			<%--▼▼▼ カード情報入力フォーム非表示 ▼▼▼--%>
																				<td class="edit_item_bg" align="left" colspan="2">
																					<span class="notice">
																						クレジットカード番号は入力できません。<br />
																						<%if (this.NeedsRegisterProvisionalCreditCardCardKbnZeus) {%>クレジットカード番号の入力は決済用タブレットにて行ってください。<%} %>
																						<%if (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus) {%>
																							登録すると「<%: new PaymentService().Get(this.LoginOperatorShopId, Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID).PaymentName %>」として登録されます。
																						<%} %>
																					</span>
																				</td>
																			<%--▲▲▲ カード情報入力フォーム非表示 ▲▲▲--%>
																			<%} %>
																			<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>
																			<%-- ※定期は仮クレカの場合はここで支払回数選択とカード登録を行う --%>
																			<%-- ▽分割支払い有効の場合は表示▽ --%>
																			<tr id="trInstallments" runat="server">
																				<td class="edit_title_bg" style="width: 140px;">支払回数 <span class="notice">*</span></td>
																				<td class="edit_item_bg"><asp:DropDownList id="dllCreditInstallments" runat="server"></asp:DropDownList>&nbsp;&nbsp;※AMEX/DINERSは一括のみとなります。</td>
																			</tr>
																			<%-- △分割支払い有効の場合は表示△ --%>
																			<tr id="trRegistCreditCard" runat="server">
																				<td class="edit_title_bg">登録する</td>
																				<td class="edit_item_bg"><asp:CheckBox ID="cbRegistCreditCard" runat="server" Text="  登録する" AutoPostBack="true" OnCheckedChanged="cbRegistCreditCard_OnCheckedChanged" /></td>
																			</tr>
																			<tr id="trUserCreditCardName" runat="server">
																				<td class="edit_title_bg">クレジットカード登録名<span class="notice">*</span></td>
																				<td class="edit_item_bg">
																					<asp:TextBox ID="tbUserCreditCardName" runat="server" MaxLength="30"></asp:TextBox><br/>
																					※クレジットカードを保存する場合はご入力ください。
																				</td>
																			</tr>
																		</table>
																	</td>
																	</tr>
																</tbody>
															</table>
															</asp:PlaceHolder>
															<div style="margin-bottom:10px"></div>
															<%-- △新しいクレジットカードを利用する△ --%>
															<%-- ▽登録済みのクレジットカードを利用する▽ --%>
															<div id="divRegisteredCreditCard" runat="server">
															<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="625">
																<tr class="edit_title_bg">
																	<td align="left">
																		<asp:RadioButton ID="rbRegisteredCreditCard" GroupName="SelectCreditCard" runat="server" OnCheckedChanged="rbCreditCard_OnCheckedChanged" AutoPostBack="true" Text=" 登録済みのクレジットカードを利用する "  /></td>
																</tr>
																<tr id="trRegisteredCreditCard" runat="server" class="edit_item_bg">
																	<td align="left">
																		<table class="edit_table" cellspacing="1" cellpadding="0" border="0" width="550">
																			<tr>
																				<td class="edit_title_bg" width="140">登録済みクレジットカード</td>
																				<td class="edit_item_bg"><asp:DropDownList ID="ddlRegisteredCreditCards" DataTextField="CardDispName" DataValueField="BranchNo" runat="server" OnSelectedIndexChanged="ddlRegisteredCreditCards_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
																			</tr>
																			<tr>
																				<td class="edit_title_bg" width="140">カード番号</td>
																				<td class="edit_item_bg">************<asp:Literal id="lRegisteredCreditCardLastFourDigit" runat="server" /></td>
																			</tr>
																			<tr>
																				<td class="edit_title_bg">有効期限</td>
																				<td class="edit_item_bg"><asp:Literal id="lRegisteredCreditCardExpirationMonth" runat="server" />/<asp:Literal id="lRegisteredCreditCardExpirationYear" runat="server" />(月/年)</td>
																			</tr>
																			<tr>
																				<td class="edit_title_bg">カード名義人</td>
																				<td class="edit_item_bg"><asp:Literal id="lRegisteredCreditCardAuthorName" runat="server" /></td>
																			</tr>
																			<tr id="trRegisteredCreditCardSecurityCode" runat="server">
																				<td class="edit_title_bg">セキュリティコード</td>
																				<td class="edit_item_bg">****</td>
																			</tr>
																			<%-- ▽分割支払い有効の場合は表示▽ --%>
																			<tr id="trRegisteredCreditCardInstallments" runat="server">
																				<td class="edit_title_bg">支払回数 <span class="notice">*</span></td>
																				<td class="edit_item_bg"><asp:DropDownList id="dllCreditInstallments2" runat="server"></asp:DropDownList>&nbsp;&nbsp;※AMEX/DINERSは一括のみとなります。</td>
																			</tr>
																			<%-- △分割支払い有効の場合は表示△ --%>
																		</table>
																	</td>
																</tr>
															</table>
															<div style="margin-bottom:10px"></div>
															</div>
															<%-- △登録済みのクレジットカードを利用する△ --%>
														</td>
													</tr>
													<%-- △クレジット決済場合は表示△ --%>
												</table>
												</div>
												<%--△ 決済情報 △--%>
												<%--▽ 配送先情報 ▽--%>
												<div id="dvFixedPurchaseShipping" runat="server">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ 配送先情報エラー表示 ▽--%>
													<tr id="trFixedPurchaseShippingErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
													</tr>
													<tr id="trFixedPurchaseShippingErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="4">
															<asp:Label ID="lbFixedPurchaseShippingErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<tr id="dvErrorShippingConvenience" style="display:none;">
														<td class="edit_item_bg" style="color:red;" align="left" colspan="4">
															<%= WebMessages.GetMessages(WebMessages.ERRMSG_CONVENIENCE_STORE_NOT_VALID) %>
														</td>
													</tr>
													<%--<asp:HiddenField ID="hfUserShipping" runat="server" />--%>
													<asp:HiddenField ID="hfCvsShopId" runat="server" Value="<%# this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreId %>" />
													<asp:HiddenField ID="hfCvsShopName" runat="server" Value="<%# this.FixedPurchaseContainer.Shippings[0].ShippingName %>" />
													<asp:HiddenField ID="hfCvsShopAddress" runat="server" Value="<%# this.FixedPurchaseContainer.Shippings[0].ShippingAddr4 %>" />
													<asp:HiddenField ID="hfCvsShopTel" runat="server" Value="<%# this.FixedPurchaseContainer.Shippings[0].ShippingTel1 %>" />
													<asp:HiddenField ID="hfCvsShopFlg" runat="server" Value="<%# this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg %>" />
													<%--△ 配送先情報エラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center" colspan="4">配送先情報</td>
													</tr>
													<tr>
														<td class="edit_item_bg" colspan="4">
															<asp:DropDownList ID="ddlUserShipping" OnSelectedIndexChanged="ddlUserShippingKbn_SelectedIndexChanged" AutoPostBack="true" CssClass="UserShipping" DataTextField="Text" DataValueField="Value" runat="server"></asp:DropDownList>
															<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
															<asp:DropDownList
																ID="ddlShippingReceivingStoreType"
																runat="server"
																CssClass="UserShipping"
																DataTextField="Text"
																DataValueField="Value"
																AutoPostBack="true"
																OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged" />
															<asp:Button CssClass="CvsSearch" ID="btnOpenConvenienceStoreMapEcPay" runat="server" OnClick="btnOpenConvenienceStoreMapEcPay_Click" Text="  検索  "/>
															<% } else { %>
															<span id="buttonConvenienceStoreMapPopup" runat="server">
																<input class="CvsSearch" onclick="javascript:openConvenienceStoreMapPopup();" type="button" value="検索"/>
															</span>
															<% } %>
														</td>
													</tr>
													<tbody id="tbSeletedAddress" runat="server">
														<tr>
															<td class="detail_title_bg" align="left" width="160"><%: ReplaceTag("@@User.name.name@@") %></td>
															<td class="detail_item_bg" align="left" colspan="3">
																<asp:Label ID="lbSeletedAddressName" runat="server" Text="<%# this.User.Name %>"></asp:Label>
																<% if (this.IsShippingAddrJp) { %>
																	（<asp:Label ID="lbSeletedAddressNameKana" Text="<%# this.User.NameKana %>" runat="server" />）
																<% } %>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left">住所</td>
															<td class="detail_item_bg" align="left" colspan="3">
																<asp:Label ID="lbSeletedAddressZip" Text="<%# this.User.Zip %>" runat="server" />
																<asp:Label ID="lbSeletedAddressAddr1" Text="<%# this.User.Addr1 %>" runat="server" />&nbsp;
																	<asp:Label ID="lbSeletedAddressAddr2" Text="<%# this.User.Addr2 %>" runat="server" />&nbsp;
																	<asp:Label ID="lbSeletedAddressAddr3" Text="<%# this.User.Addr3 %>" runat="server" />&nbsp;
																	<asp:Label ID="lbSeletedAddressAddr4" Text="<%# this.User.Addr4 %>" runat="server" />&nbsp;
																	<asp:Label ID="lbSeletedAddressAddr5" Text="<%# this.User.Addr5 %>" runat="server" />&nbsp;
																<asp:Label ID="lbSeletedAddressAddrCountryName" Text="<%# this.User.AddrCountryName %>" runat="server" />
															</td>
														</tr>
														<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
														<tr>
															<td class="detail_title_bg" align="left">企業名・部署名</td>
															<td class="detail_item_bg" align="left" colspan="3">
																<asp:Label ID="lbSeletedAddressCompanyName" Text="<%# this.User.CompanyName %>" runat="server" />&nbsp
																	<asp:Label ID="lbSeletedAddressCompanyPostName" Text="<%# this.User.CompanyPostName %>" runat="server" /></td>
														</tr>
														<%} %>
														<tr>
															<td class="detail_title_bg" align="left">電話番号</td>
															<td class="detail_item_bg" align="left" colspan="3">
																<asp:Label ID="lbSeletedAddressTel1" Text="<%# this.User.Tel1 %>" runat="server" /></td>
														</tr>
													</tbody>
													<tbody id="tbodyConvenienceStore" class="edit_title_bg CONVENIENCE_STORE" runat="server">
														<tr>
															<td class="detail_title_bg" align="left" width="160">店舗ID</td>
															<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopId">
																<span>
																	<asp:Literal ID="lCvsShopId" runat="server" />
																</span>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="160">店舗名称</td>
															<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopName">
																<span>
																	<asp:Literal ID="lCvsShopName" runat="server" />
																</span>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="160">店舗住所</td>
															<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopAddress">
																<span>
																	<asp:Literal ID="lCvsShopAddress" runat="server" />
																</span>
															</td>
														</tr>
														<tr>
															<td class="detail_title_bg" align="left" width="160">店舗電話番号</td>
															<td class="edit_item_bg" align="left" colspan="3" id="tdCvsShopTel">
																<span>
																	<asp:Literal ID="lCvsShopTel" runat="server" />
																</span>
															</td>
														</tr>
													</tbody>
													<tbody id="tbodyUserShipping" class="UserShippingNormal" runat="server">
														<tr>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.name.name@@") %> <span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																姓：<asp:TextBox ID="tbShippingName1" runat="server" Width="90" MaxLength="10"></asp:TextBox> 名：<asp:TextBox ID="tbShippingName2" runat="server" Width="90" MaxLength="10"></asp:TextBox></td>
															<% if (this.IsShippingAddrJp) { %>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.name_kana.name@@") %> <span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																姓：<asp:TextBox ID="tbShippingNameKana1" runat="server" Width="90" MaxLength="20"></asp:TextBox> 名：<asp:TextBox ID="tbShippingNameKana2" runat="server" Width="90" MaxLength="20"></asp:TextBox></td>
															<% } %>
														</tr>
														<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%: ReplaceTag("@@User.country.name@@", this.ShippingAddrCountryIsoCode) %>
																<span class="notice">*</span>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:DropDownList ID="ddlShippingCountry" runat="server" AutoPostBack="true"></asp:DropDownList></td>
														</tr>
														<% } %>
														<% if (this.IsShippingAddrJp) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.zip.name@@") %> <span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbShippingZip1" runat="server" Width="60" MaxLength="3"></asp:TextBox>－
																<asp:TextBox ID="tbShippingZip2" runat="server" Width="60" MaxLength="4"></asp:TextBox>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.addr1.name@@") %> <span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:DropDownList ID="ddlShippingAddr1" runat="server"></asp:DropDownList></td>
														</tr>
														<% } %>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%: ReplaceTag("@@User.addr2.name@@", this.ShippingAddrCountryIsoCode) %>
																<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbShippingAddr2" runat="server" Width="300" MaxLength="40"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%: ReplaceTag("@@User.addr3.name@@", this.ShippingAddrCountryIsoCode) %>
																<% if (this.IsShippingAddrJp) { %><span class="notice">*</span><% } %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbShippingAddr3" runat="server" Width="300" MaxLength="50"></asp:TextBox>
															</td>
														</tr>
														<tr <%: (Constants.DISPLAY_ADDR4_ENABLED || (this.IsShippingAddrJp == false)) ? "" : "style=\"display:none;\""  %>>
															<td class="edit_title_bg" align="left" width="120">
																<%: ReplaceTag("@@User.addr4.name@@", this.ShippingAddrCountryIsoCode) %>
																<% if (this.IsShippingAddrJp == false) { %><span class="notice">*</span><% } %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbShippingAddr4" runat="server" Width="300" MaxLength="50"></asp:TextBox></td>
														</tr>
														<% if (this.IsShippingAddrJp == false) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%: ReplaceTag("@@User.addr5.name@@", this.ShippingAddrCountryIsoCode) %>
																<% if (this.IsShippingAddrUs) { %><span class="notice">*</span><% } %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<% if (this.IsShippingAddrUs) { %>
																<asp:DropDownList ID="ddlShippingAddr5" runat="server"></asp:DropDownList>
																<% } else { %>
																<asp:TextBox ID="tbShippingAddr5" runat="server" Width="300" MaxLength="40"></asp:TextBox>
																<% } %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="120">
																<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %>
																<% if (this.IsUserAddrZipNecessary) { %><span class="notice">*</span><% } %>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbShippingZipGlobal" runat="server" Width="150" MaxLength="20" />
																<asp:LinkButton
																	ID="lbSearchAddrFromShippingZipGlobal"
																	OnClick="lbSearchAddrFromShippingZipGlobal_Click"
																	Style="display:none;"
																	runat="server" />
															</td>
														</tr>
														<% } %>
														<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
														<tr>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.company_name.name@@")%> <span class="notice"></span></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbShippingCompanyName" runat="server" Width="300" MaxLength="50"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.company_post_name.name@@")%> <span class="notice"></span></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbShippingCompanyPostName" runat="server" Width="300" MaxLength="50"></asp:TextBox></td>
														</tr>
														<%} %>
														<tr>
															<% if (this.IsShippingAddrJp) { %>
															<td class="edit_title_bg" align="left" width="120"><%: ReplaceTag("@@User.tel1.name@@") %> <span class="notice">*</span></td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbShippingTel1" runat="server" Width="60" MaxLength="6"></asp:TextBox>－
																<asp:TextBox ID="tbShippingTel2" runat="server" Width="60" MaxLength="4"></asp:TextBox>－
																<asp:TextBox ID="tbShippingTel3" runat="server" Width="60" MaxLength="4"></asp:TextBox></td>
															<% } else { %>
															<td class="edit_title_bg" align="left" width="120">
																<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %>
																<span class="notice">*</span>
															</td>
															<td class="edit_item_bg" align="left" colspan="3">
																<asp:TextBox ID="tbShippingTel1Global" runat="server" Width="200" MaxLength="30"></asp:TextBox></td>
															<% } %>
														</tr>
													</tbody>
													<tr>
														<td class="edit_title_bg" align="left" width="120">配送情報反映対象</td>
														<td class="edit_item_bg" align="left" colspan="3">
															<asp:CheckBoxList ID="cblUpdatePattern" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" />
															<br/>※変更前の情報が同一の定期台帳及びユーザー情報へ変更内容を反映します。
														</td>
													</tr>
												</table>
												<br/>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="4">配送方法関連情報</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="120">配送方法</td>
														<td class="edit_item_bg" align="left" colspan="3">
															<asp:DropDownList ID="ddlShippingMethod" runat="server" OnSelectedIndexChanged="ddlShippingMethod_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
															<asp:Button ID="btnSetShippingMethod" runat="server" OnClick="btnSetShippingMethod_Click" Text="  配送方法自動判定  " />
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="120">配送サービス</td>
														<td class="edit_item_bg" align="left" colspan="3">
															<asp:DropDownList ID="ddlDeliveryCompany" runat="server" OnSelectedIndexChanged="ddlDeliveryCompany_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
													</tr>
													<% if (this.IsValidShippingTimeSetFlg) { %>
													<tr id="trShippingTime1" class="ShippingTime1" runat="server">
														<td class="edit_title_bg" align="left" width="120">配送希望時間帯</td>
														<td class="edit_item_bg" align="left" colspan="3">
															<asp:DropDownList ID="ddlShippingTime" runat="server"></asp:DropDownList></td>
													</tr>
													<%} %>
												</table>
												</div>
												<%--△ 配送先情報 △--%>
												<%--▽ 配送パターン ▽--%>
												<div id="dvFixedPurchasePattern" runat="server">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ 配送パターンエラー表示 ▽--%>
													<tr id="trFixedPurchasePatternErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
													</tr>
													<tr id="trFixedPurchasePatternErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="4">
															<asp:Label ID="lbFixedPurchasePatternErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ 配送パターンエラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">配送パターン</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="160">配送パターン</td>
														<td class="edit_item_bg" align="left" >
															<asp:UpdatePanel UpdateMode="Conditional" runat="server">
															<ContentTemplate>
															<div id="divAlertMessage" runat="server" style="margin-bottom:10px;" visible="false">
																<asp:Label ID="lbFixedPurchasePatternAlertMessages" runat="server" ForeColor="red" />
															</div>
															<dl>
																<dt id="dtMonthlyDate" runat="server">
																	<asp:RadioButton ID="rbFixedPurchaseDays" Text="月間隔日付指定" GroupName="FixedPurchaseShippingPattern" runat="server" OnCheckedChanged="rbFixedPurchaseDays_CheckedChanged" AutoPostBack="true" />
																</dt>
																<dd id="ddMonthlyDate" style="padding-left:25px" runat="server">
																	<asp:DropDownList ID="ddlMonth" runat="server" OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
																	ヶ月ごと
																	<asp:DropDownList ID="ddlMonthlyDate" runat="server"></asp:DropDownList>
																	日に届ける
																	<br />
																</dd>
																<dt id="dtWeekAndDay" runat="server">
																	<asp:RadioButton ID="rbFixedPurchaseWeekAndDay" Text="月間隔・週・曜日指定" GroupName="FixedPurchaseShippingPattern" runat="server" OnCheckedChanged="rbFixedPurchaseWeekAndDay_CheckedChanged" AutoPostBack="true" />
																</dt>
																<dd id="ddWeekAndDay" style="padding-left:25px" runat="server">
																	<asp:DropDownList ID="ddlIntervalMonths" runat="server" OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
																	ヶ月ごと
																	<asp:DropDownList ID="ddlWeekOfMonth" runat="server"></asp:DropDownList>
																	<asp:DropDownList ID="ddlDayOfWeek" runat="server"></asp:DropDownList>
																	に届ける
																	<br />
																</dd>
																<dt id="dtIntervalDays" runat="server">
																	<asp:RadioButton ID="rbFixedPurchaseIntervalDays" Text="配送日間隔指定" GroupName="FixedPurchaseShippingPattern" runat="server" OnCheckedChanged="rbFixedPurchaseIntervalDays_CheckedChanged" AutoPostBack="true" />
																</dt>
																<dd id="ddIntervalDays" style="padding-left:25px" runat="server">
																	<asp:DropDownList ID="ddlIntervalDays" runat="server" OnSelectedIndexChanged="ddlIntervalDays_OnSelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
																	日ごとに届ける
																	<br />
																</dd>
																<dt id="dtEveryNWeek" runat="server">
																	<asp:RadioButton ID="rbFixedPurchaseWeekAndDayOfWeek" Text="週間隔・曜日指定" GroupName="FixedPurchaseShippingPattern" runat="server" OnCheckedChanged="rbFixedPurchaseEveryNWeek_CheckedChanged" AutoPostBack="true" />
																</dt>
																<dd id="ddEveryNWeek" style="padding-left:25px" runat="server">
																	<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week" runat="server" OnSelectedIndexChanged="ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
																		週間ごとの
																	<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek" runat="server" OnSelectedIndexChanged="ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
																	に届ける
																	<br />
																</dd>
															</dl>
															</ContentTemplate>
															</asp:UpdatePanel>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="160"></td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox ID="cbScheduleAutoComputeFlg" runat="server" Text=" 最後の定期購入日から配送日を自動計算する" />
														</td>
													</tr>
												</table>
												<div id="scheduleSetting">
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">配送スケジュール</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="160">次回配送日</td>
														<td class="edit_item_bg" align="left" >
															<uc:DateTimeInput ID="ucNextShippingDate" runat="server" HasTime="False" HasBlankSign="False" HasBlankValue="False" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="160">次々回配送日</td>
														<td class="edit_item_bg" align="left">
															<uc:DateTimeInput ID="ucNextNextShippingDate" runat="server" HasTime="False" HasBlankSign="False" HasBlankValue="False" />
															　<asp:CheckBox runat="server" ID="cbNextNextShippingDateAutoComputeFlg" Text=" 次回配送日から自動計算する" />
														</td>
													</tr>
												</table>
												</div>
												</div>
												<%--△ 配送パターン △--%>
												<%if (OrderCommon.DisplayTwInvoiceInfo(this.FixedPurchaseContainer.Shippings[0].ShippingCountryIsoCode)) { %>
												<%--▽ Invoice ▽--%>
												<div id="dvInvoice" runat="server">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ Invoice Error Display ▽--%>
													<tr id="trInvoiceErrorMessageTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="4">エラーメッセージ</td>
													</tr>
													<tr id="trInvoiceErrorMessage" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="4">
															<asp:Label ID="lbInvoiceError" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ Invoice Error Display △--%>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">電子発票</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">発票種類</td>
														<td class="edit_item_bg" align="left" width="75%" colspan="3">
															<asp:DropDownList ID="ddlUniformInvoice" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUniformInvoice_SelectedIndexChanged"></asp:DropDownList>
															<asp:DropDownList ID="ddlUniformInvoiceOrCarryTypeOption" DataTextField="Text" DataValueField="Value" runat="server" AutoPostBack ="true" Visible="false" OnSelectedIndexChanged="ddlUniformInvoiceOrCarryTypeOption_SelectedIndexChanged"></asp:DropDownList>
														</td>
													</tr>
													<% if (this.IsPersonal) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">共通性載具</td>
														<td class="edit_item_bg" align="left" width="75%" colspan="3">
															<asp:DropDownList ID="ddlCarryType" OnSelectedIndexChanged="ddlCarryType_SelectedIndexChanged" AutoPostBack="True" runat="server"></asp:DropDownList>
															<asp:TextBox ID="tbCarryTypeOption1" runat="server" Visible="false" placeholder="例:/AB201+9(限8個字)" MaxLength="8"></asp:TextBox>
															<asp:TextBox ID="tbCarryTypeOption2" runat="server" Visible="false" placeholder="例:TP03000001234567(限16個字)" MaxLength="16"></asp:TextBox>
														</td>
													</tr>
													<% } %>
													<% if (this.IsCompany) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">統一編号</td>
														<td class="edit_item_bg" align="left" width="75%" colspan="3">
															<asp:TextBox ID="tbCompanyOption1" runat="server" placeholder="例:12345678" MaxLength="8"></asp:TextBox>
															<asp:Label ID="lbCompanyOption1" Visible="false" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">会社名</td>
														<td class="edit_item_bg" align="left" width="75%" colspan="3">
															<asp:TextBox ID="tbCompanyOption2" runat="server" placeholder="例:○○有限股份公司" MaxLength="20"></asp:TextBox>
															<asp:Label ID="lbCompanyOption2" Visible="false" runat="server"></asp:Label>
														</td>
													</tr>
													<% } %>
													<% if (this.IsDonate) { %>
													<tr>
														<td class="edit_title_bg" align="left" width="25%">寄付先コード</td>
														<td class="edit_item_bg" align="left" width="75%" colspan="3">
															<asp:TextBox ID="tbDonateOption1" runat="server" MaxLength="7"></asp:TextBox>
															<asp:Label ID="lbDonateOption1" Visible="false" runat="server"></asp:Label>
														</td>
													</tr>
													<% } %>
												</table>
												</div>
												<%--△ Invoice △--%>
												<% } %>
												<%--▽ 商品変更 ▽--%>
												<div id="dvFixedPurchaseItemList" runat="server">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<%--▽ 商品変更エラー表示 ▽--%>
													<tr id="trFixedPurchaseItemErrorMessagesTitle" runat="server" visible="false">
														<td class="edit_title_bg" align="center" colspan="8">エラーメッセージ</td>
													</tr>
													<tr id="trFixedPurchaseItemErrorMessages" runat="server" visible="false">
														<td class="edit_item_bg" align="left" colspan="8">
															<asp:Label ID="lbFixedPurchaseItemErrorMessages" runat="server" ForeColor="red" />
														</td>
													</tr>
													<%--△ 商品変更エラー表示 △--%>
													<tr>
														<td class="edit_title_bg" align="center" colspan="<%: this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount ? 5 : 7 %>">商品情報</td>
														<td class="edit_title_bg" align="center">
															<asp:Button ID="btnAddProduct" runat="server" Text="  追加  " Onclick="btnAddProduct_Click" /><br />
															<asp:Button ID="btnReCalculate" runat="server" Text="  再計算  " Onclick="btnReCalculate_Click" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="center" width="10%" rowspan="2" style="height: 31px">
															検索/削除
														</td>
														<td class="edit_title_bg" align="center" width="20%">
															商品ID
														</td>
														<td class="edit_title_bg" align="center" width="35%" rowspan="2">
															商品名
														</td>
															<% if (this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false) { %>
														<td class="edit_title_bg" align="center" width="6%" rowspan="2">
															単価（<%: this.ProductPriceTextPrefix %>）
														</td>
															<%} %>
														<td class="edit_title_bg" align="center" width="6%" rowspan="2">
															数量
														</td>
														<td class="edit_title_bg" align="center" width="15%" colspan="2">
															購入回数
														</td>
															<% if (this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false) { %>
														<td class="edit_title_bg" align="center" width="6%" rowspan="2">
															小計（<%: this.ProductPriceTextPrefix %>）
														</td>
															<%} %>
													</tr>
													<tr>
														<td class="edit_title_bg" align="center" style="height: 15px">
															(商品ID+) バリエーションID
														</td>
														<td class="edit_title_bg" align="center" width="6%">注文基準</td>
														<td class="edit_title_bg" align="center" width="6%">出荷基準</td>
													</tr>
													<asp:Repeater id="rItemList" runat="server" OnItemDataBound="rItemList_OnDataBound" OnItemCommand="rItemList_ItemCommand">
														<ItemTemplate>
															<tr class="edit_item_bg">
																<td align="center" rowspan="2">
																		<input id="inputSearchProduct" type="button" value="検索" onclick="javascript:open_product_list('<%= SarchUrl() %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%# Container.ItemIndex %>	');" />
																	<span visible="<%# (this.CanDelete) %>" runat="server">
																		<asp:Button ID="btnDeleteProduct" CommandName="delete" CommandArgument="<%# Container.ItemIndex %>" Text="削除" runat="server" />
																	</span>
																</td>
																<td align="left">
																	<asp:TextBox ID="tbProductId" runat="server" Text="<%# ((FixedPurchaseItemInput)Container.DataItem).ProductId %>" Width="130" MaxLength="30"></asp:TextBox>
																	<asp:HiddenField ID="hfProductIdForOptionSetting" Value="<%# ((FixedPurchaseItemInput)Container.DataItem).ProductId %>" runat="server" />
																</td>
																<td align="left" rowspan="2">
																	<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).CreateProductJointName())%>
																</td>
																	<td align="right" rowspan="2" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																	<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).GetValidPrice().ToPriceString(true))%>
																</td>
																<td align="center" rowspan="2">
																	<asp:TextBox id="tbItemQuantity" runat="server" Text="<%# ((FixedPurchaseItemInput)Container.DataItem).ItemQuantity %>" Width="30" MaxLength="3"></asp:TextBox>
																</td>
																<td align="center" rowspan="2">
																	<asp:TextBox id="tbItemOrderCount" runat="server" Text="<%# ((FixedPurchaseItemInput)Container.DataItem).ItemOrderCount %>" Width="33" MaxLength="3"></asp:TextBox>回
																</td>
																<td align="center" rowspan="2">
																	<asp:TextBox id="tbItemShippedCount" runat="server" Text="<%# ((FixedPurchaseItemInput)Container.DataItem).ItemShippedCount %>" Width="33" MaxLength="3"></asp:TextBox>回
																</td>
																	<td align="right" rowspan="2" Visible="<%# this.FixedPurchaseContainer.IsSubscriptionBoxFixedAmount == false %>" runat="server">
																	<%# WebSanitizer.HtmlEncode(((FixedPurchaseItemInput)Container.DataItem).GetItemPrice().ToPriceString(true))%>
																</td>
															</tr>
															<tr class="edit_item_bg">
																<td align="left">
																	<asp:TextBox ID="tbVariationId" runat="server" Text="<%# ((FixedPurchaseItemInput)Container.DataItem).VId %>" Width="90" MaxLength="60"></asp:TextBox>
																	<asp:Button ID="btnGetProductData" CommandArgument="<%# Container.ItemIndex %>" CommandName="get" Text="取得" runat="server" />
																</td>
															</tr>
															<tr>
																<td align="center" class="edit_title_bg" rowspan="2">付帯情報</td>
																<td colspan="7" class="edit_title_bg" align="center" runat="server" Visible="<%# (GetProductOptionValueSettings((FixedPurchaseItemInput)Container.DataItem).Count > 0) %>">
																	<asp:RadioButton ID="rbProductOptionValueInputFormText" GroupName="ProductOptionValueInput" Text="&nbsp;テキスト入力" OnCheckedChanged="rbProductOptionValueInputForm_OnCheckedChanged" Checked="True" AutoPostBack="true" runat="server" />
																	<asp:RadioButton ID="rbProductOptionValueInputForm" GroupName="ProductOptionValueInput" Text="&nbsp;簡単入力" OnCheckedChanged="rbProductOptionValueInputForm_OnCheckedChanged" AutoPostBack="true" runat="server" />
																</td>
															</tr>
															<tr>
																<td align="left" colspan="7" class="edit_item_bg">
																	<div id="dvProductOptionValueInputFormText" runat="server">
																		<asp:TextBox ID="tbProductOptionTexts" Text='<%# ((FixedPurchaseItemInput)Container.DataItem).GetDisplayProductOptionTexts() %>' Width="99.5%" runat="server"></asp:TextBox>
																	</div>
																	<div id="dvProductOptionValueInputForm" Visible="False" runat="server">
																	<asp:Repeater ID="rProductOptionValueSettings" ItemType="w2.App.Common.Product.ProductOptionSetting" runat="server">
																		<ItemTemplate>
																			<div>
																				<span class="accessory-info-label w10" title="<%#: Item.ValueName %>" style="display: inline-block"><%#: Item.ValueName %><span class="notice" runat="server" visible="<%# Item.IsNecessary %>">*</span></span>
																				<span class="ta-center" style="display: inline-block">
																					<asp:DropDownList runat="server" ID="ddlProductOptionValueSetting" DataSource='<%# Item.SettingValuesListItemCollection %>' Visible='<%# Item.IsSelectMenu || Item.IsDropDownPrice%>' SelectedValue='<%# Item.GetDisplayProductOptionSettingSelectedValue() %>' />
																					<asp:TextBox ID="tbProductOptionValueSetting" Text='<%# Item.SelectedSettingValueForTextBox %>' Visible='<%# Item.IsTextBox %>' runat="server" />
																					<asp:Repeater ID="rCblProductOptionValueSetting" DataSource='<%# Item.SettingValuesListItemCollection %>' ItemType="System.Web.UI.WebControls.ListItem" Visible='<%# Item.IsCheckBox || Item.IsCheckBoxPrice%>' runat="server">
																						<ItemTemplate>
																							<span title="<%# Eval("Text") %>">
																								<asp:CheckBox ID="cbProductOptionValueSetting" Text='<%# Item.Text %>' Checked='<%# Item.Selected %>' runat="server" />
																							</span>
																						</ItemTemplate>
																					</asp:Repeater>
																				</span>
																			</div>
																		</ItemTemplate>
																	</asp:Repeater>
																	</div>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												</div>
												<%--△ 商品変更 △--%>
												<div class="action_part_bottom">
													<%--▼▼ クレジット Token保持用 ▼▼--%>
													<asp:HiddenField ID="hfCreditToken" Value="" runat="server" />
													<%--▲▲ クレジット Token保持用 ▲▲--%>
													<%--▼▼ PayTg 端末状態保持用 ▼▼--%>
													<asp:HiddenField ID="hfCanUseDevice" runat="server" />
													<asp:HiddenField ID="hfStateMessage" runat="server" />
													<%--▲▲ PayTg 端末状態保持用 ▲▲--%>
													<%--▼▼ カード情報取得用 ▼▼--%>
													<asp:HiddenField ID="hfPayTgSendId" runat="server" />
													<asp:HiddenField ID="hfPayTgPostData" runat="server" />
													<asp:HiddenField ID="hfPayTgResponse" runat="server" />
													<asp:Button ID="btnProcessPayTgResponse" runat="server" style="display: none;" OnClick="btnProcessPayTgResponse_Click" />
													<input type="hidden" id="hidCinfo" name="hidCinfo" value="<%= CreateGetCardInfoJsScriptForCreditToken() %>" />
													<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
													<%--▲▲ カード情報取得用 ▲▲--%>

													<asp:Button id="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnConfirmBottom" runat="server" OnClientClick="if(checkClickConfirm() == false) {return;}  doPostbackEvenIfCardAuthFailed=false;" UseSubmitBehavior="False" Text="  確認する  " OnClick="btnConfirm_Click" />
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
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";
	var isCheckStore = '<%= this.UserShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON ? true : false %>';

	function checkClickConfirm() {
		var requestactionKbn = '<%=this.RequestActionKbn %>';
		switch (requestactionKbn) {

			// Check Store Exist
			case '<%= ACTION_KBN_SHIPPING %>':

				if(isCheckStore == 'False') return true;
				var shopId = $("#<%=hfCvsShopId.ClientID %>").val();
				var shopValid = true;
				$.ajax({
					type: "POST",
					url: "<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXEDPURCHASE_MODIFY_INPUT %>/CheckStoreIdValid",
					data: JSON.stringify({ storeId: shopId }),
					contentType: "application/json; charset=utf-8",
					dataType: "json",
					cache: false,
					async: false,
					success: function (data) {
						if (data.d) {
							$('#dvErrorShippingConvenience').css("display", "none");
						}
						else {
							shopValid = false;
							$('#dvErrorShippingConvenience').css("display", "");
						}
					}
				});

				return shopValid;

			// 次回配送日過去日付チェック
		case '<%= ACTION_KBN_PATTERN %>':

			// 「最後の定期購入日から配送日を自動計算する」チェックボックスが未チェックの場合のみ過去日付チェック
			if ($("#<%= cbScheduleAutoComputeFlg.ClientID %>").prop('checked') == false) {
				var year = $("#<%= ucNextShippingDate.DdlDatePart1ClientId %>").val();
				var month = $("#<%= ucNextShippingDate.DdlDatePart2ClientId %>").val();
				var day = $("#<%= ucNextShippingDate.DdlDatePart3ClientId %>").val();
				var nextShippingDate = new Date(year, (month - 1), day, 0, 0, 0);
				if (checkPastDay(nextShippingDate)) {
					return confirm('次回配送日に過去の日付が指定されてます。\nこのまま更新してもよろしいですか？');
				}
				return true;
			}
		}

		return true;
	}
	
	function checkPastDay(targetDate) {
		var targetYear = targetDate.getFullYear();
		var targetMonth = targetDate.getMonth() + 1;
		var targetDay = targetDate.getDate();
 
		var today = new Date();
		var currentYear = today.getFullYear();
		var currentMonth = today.getMonth() + 1;
		var currentDay = today.getDate();

		if (targetYear == currentYear) {
			if (targetMonth == currentMonth) {
				return targetDay < currentDay;
			} else {
				return targetMonth < currentMonth;
			}
		} else {
			return targetYear < currentYear;
		}
	}

	<% if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup() {
		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl() %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel, shopNum) {
		var elements = document.getElementsByClassName('CONVENIENCE_STORE')[0];
		// For display
		elements.querySelector('[id$="tdCvsShopId"] > span').innerHTML = cvsspot;
		elements.querySelector('[id$="tdCvsShopName"] > span').innerHTML = name;
		elements.querySelector('[id$="tdCvsShopAddress"] > span').innerHTML = addr;
		elements.querySelector('[id$="tdCvsShopTel"] > span').innerHTML = tel;

		// For display
		$("#<%=hfCvsShopId.ClientID %>").val(cvsspot);
		$("#<%=hfCvsShopName.ClientID %>").val(name);
		$("#<%=hfCvsShopAddress.ClientID %>").val(addr);
		$("#<%=hfCvsShopTel.ClientID %>").val(tel);
	}
	<% } %>

	<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
	<%-- Set convenience store Ec pay data --%>
	function setConvenienceStoreEcPayData(cvsspot, name, addr, tel) {
		if(cvsspot != "")
		{
			var elements = document.getElementsByClassName('CONVENIENCE_STORE')[0];
			// For display
			elements.querySelector('[id$="tdCvsShopId"] > span').innerHTML = cvsspot;
			elements.querySelector('[id$="tdCvsShopName"] > span').innerHTML = name;
			elements.querySelector('[id$="tdCvsShopAddress"] > span').innerHTML = addr;
			elements.querySelector('[id$="tdCvsShopTel"] > span').innerHTML = tel;

			// For get value
			$("#<%=hfCvsShopId.ClientID %>").val(cvsspot);
			$("#<%=hfCvsShopName.ClientID %>").val(name);
			$("#<%=hfCvsShopAddress.ClientID %>").val(addr);
			$("#<%=hfCvsShopTel.ClientID %>").val(tel);
		}
	}
	<% } %>

	execAutoKanaWithKanaType(
		$("#<%= tbShippingName1.ClientID %>"),
		$("#<%= tbShippingNameKana1.ClientID %>"),
		$("#<%= tbShippingName2.ClientID %>"),
		$("#<%= tbShippingNameKana2.ClientID %>"));

	<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
	// Textbox change search zip global
	textboxChangeSearchGlobalZip(
		'<%= tbShippingZipGlobal.ClientID %>',
		'<%= lbSearchAddrFromShippingZipGlobal.UniqueID %>');
	<% } %>
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>
</asp:Content>
