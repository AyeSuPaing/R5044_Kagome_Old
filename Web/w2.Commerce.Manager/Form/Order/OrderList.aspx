<%--
=========================================================================================================
  Module      : 注文情報一覧ページ(OrderList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.User" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Reference Page="~/Form/PdfOutput/PdfOutput.aspx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderList.aspx.cs" Inherits="Form_Order_OrderList" %>
<%-- マスタ出力コントロール --%>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<%@ Register TagPrefix="uc" TagName="LabelOrderIdAndIconHelp" Src="~/Form/Common/LabelOrderIdAndIconHelp.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<link rel="stylesheet" type="text/css" href="../../Css/hide-field-button-style.css">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">受注情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
	<tr>
		<td>
			<table class="box_border cmn-section" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="search_box_bg">
						<table class="wide-content" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table class="wide-content" cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_table">
												<table class="cmn-form" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" style="width: 165px"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" /><uc:LabelOrderIdAndIconHelp ID="LabelOrderIdAndIconHelp1" runat="server" /></td>
														<td class="search_item_bg" width="130"><asp:TextBox id="tbOrderId" runat="server" Width="125"></asp:TextBox></td>

														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文ステータス</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlOrderStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>

														<td class="search_title_bg" width="122"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />並び順</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlSortKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="83" rowspan="<%= CalculateSearchRowSpan() %>">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_LIST %>">クリア</a>&nbsp;
																<a href="javascript:Reset();">リセット</a>
															</div>
															<div class="search_btn_sub">
																<%-- 下記は件数によって表示するリンクを切り替えています。 --%>
																<asp:LinkButton id="lbPdfOutput" Runat="server" OnClick="lbPdfOutput_Click">納品書出力</asp:LinkButton>
																<asp:LinkButton id="lbPdfOutputUnsync" Runat="server" Visible="false" OnClick="lbPdfOutputUnsync_Click">納品書出力</asp:LinkButton>
															</div>
															<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
															<div class="search_btn_sub">
																<%-- 下記は件数によって表示するリンクを切り替えています。 --%>
																<asp:LinkButton id="lbPdfOutputReceipt" Runat="server" OnClick="lbPdfOutputReceipt_Click">領収書出力</asp:LinkButton>
																<asp:LinkButton id="lbPdfOutputReceiptUnsync" Runat="server" Visible="false" OnClick="lbPdfOutputReceiptUnsync_Click">領収書出力</asp:LinkButton>
															</div>
															<% } %>
															<div class="search_btn_sub">
																<asp:LinkButton id="lbTotalPickingListOutputUnsync" Runat="server" Visible="false" OnClick="lbTotalPickingListOutputUnsync_Click">ﾋﾟｯｷﾝｸﾞﾘｽﾄ</asp:LinkButton>
															</div>
															<% if (Constants.INVOICECSV_ENABLED) { %>
															<asp:Repeater ID="rShippingLabelExport" runat="server">
																<ItemTemplate>
																<div class="search_btn_sub">
																	<asp:LinkButton id="lbShippingLabelExport" Runat="server" CommandArgument="<%# Container.ItemIndex %>" OnClick="lbShippingLabelExport_Click">
																		<%# ((OrderFileExportShippingLabel.ShippingLabelExportSetting)Container.DataItem).DisplayName %>
																	</asp:LinkButton>
																</div>
																</ItemTemplate>
															</asp:Repeater>
															<% } %>
															<asp:Repeater ID="rDownloadAnchorTextList" runat="server">
																<ItemTemplate>
																<div class="search_btn_sub">
																	<asp:LinkButton id="lbInteractionDataExport" Runat="server" CommandArgument="<%# ((KeyValuePair<int, string>)Container.DataItem).Key %>" OnClick="lbInteractionDataExport_Click">
																		<%# ((KeyValuePair<int, string>)Container.DataItem).Value %>
																	</asp:LinkButton>
																</div>
																</ItemTemplate>
															</asp:Repeater>
															<%if (Constants.PDF_OUTPUT_ORDERSTATEMENT_ENABLED) {%>
															<div class="search_btn_sub">
																<%-- 下記は件数によって表示するリンクを切り替えています。 --%>
																<asp:LinkButton id="lbPdfOutputOrderStatement" Runat="server" OnClick="lbPdfOutputOrderStatement_Click">受注明細書出力</asp:LinkButton>
																<asp:LinkButton id="lbPdfOutputOrderStatementUnsync" Runat="server" Visible="false" OnClick="lbPdfOutputOrderStatementUnsync_Click">受注明細書出力</asp:LinkButton>
																<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
																<asp:LinkButton id="lbPrintInvoiceOrderForTwECPay" Runat="server" OnClick="lbPrintInvoiceOrderForTwECPay_Click">荷物送り状印刷</asp:LinkButton>
																<% } %>
															</div>
															<%} %>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ユーザーID</td>
														<td class="search_item_bg"><asp:TextBox id="tbUserId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文者名</td>
														<td class="search_item_bg"><asp:TextBox id="tbOwnerName" runat="server" Width="125"></asp:TextBox></td>
														<% if (this.IsShippingCountryAvailableJp) { %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" /><%: ReplaceTag("@@User.name_kana.name@@") %></td>
														<td class="search_item_bg"><asp:TextBox id="tbOwnerNameKana" runat="server" Width="125"></asp:TextBox></td>
														<% } %>
													</tr>
													<tr>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />メールアドレス</td>
														<td class="search_item_bg"><asp:TextBox id="tbOwnerMailAddr" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品ID</td>
														<td class="search_item_bg"><asp:TextBox id="tbProductId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品名</td>
														<td class="search_item_bg"><asp:TextBox id="tbProductName" runat="server" Width="125"></asp:TextBox></td>
													</tr>
													<tr>
													<td class="search_title_bg" width="130"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ステータス更新日</td>
													<td class="search_item_bg" colspan="5">
														<asp:DropDownList ID="ddlOrderUpdateDateStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>が
														<div id="orderUpdate" style="display: inline-block">
															<uc:DateTimePickerPeriodInput id="ucOrderUpdateDatePeriod" runat="server" IsNullStartDateTime="true" IsHideTime="true" /> の間
															<span class="search_btn_sub">(<a href="Javascript:SetYesterday('order');">昨日</a>｜<a href="Javascript:SetToday('order');">今日</a>｜<a href="Javascript:SetThisMonth('order');">今月</a>)</span>
														</div>
													</td>
													</tr>
													<tr id="hide-field_OwnerTelAddrZip" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文者郵便番号</td>
														<td class="search_item_bg"><asp:TextBox id="tbOwnerZip" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文者住所</td>
														<td class="search_item_bg"><asp:TextBox id="tbOwnerAddr" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文者電話番号</td>
														<td class="search_item_bg"><asp:TextBox id="tbOwnerTel" runat="server" Width="125"></asp:TextBox></td>
													</tr>
													<tr id="hide-field_PaymentStatusOwnerKbn" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />入金ステータス</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlOrderPaymentStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文者区分</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList id="ddlOwnerKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>
													<tr id="trOrderExtendStatus" class="hide-field_OrderExtendStatus" runat="server" style="display: none">
														<td class="search_title_bg" width="130"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />拡張ステータス</td>
														<td class="search_item_bg" >
															<asp:DropDownList id="ddlOrderExtendStatusName" runat="server" CssClass="search_item_width"></asp:DropDownList>が
															<asp:DropDownList id="ddlOrderExtendStatus" runat="server"></asp:DropDownList>のステータス	
														</td>
														<td id ="tdDemandStatus" class="search_title_bg" runat="server"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />督促ステータス</td>
														<td id ="tdDemandStatusList" class="search_item_bg" runat="server" colspan="3">
 															<asp:DropDownList id="ddlDemandStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>
													<tr id="hide-field_OrderExtendUpdate" style="display: none">
														<td class="search_title_bg" width="130">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />拡張ステータス更新日</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlOrderUpdateDateExtendStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>が
															<div id="extendStatusUpdateDate" style="display:inline-block;">
																<uc:DateTimePickerPeriodInput id="ucExtendStatusUpdateDatePeriod" runat="server" IsNullStartDateTime="true" /> の間
																<span class="search_btn_sub">(<a href="Javascript:SetYesterday('extendStatusUpdateDate');">昨日</a>｜<a href="Javascript:SetToday('extendStatusUpdateDate');">今日</a>｜<a href="Javascript:SetThisMonth('extendStatusUpdateDate');">今月</a>)</span>
															</div>
														</td>
													</tr>
													<tr id="hide-field_OrderExternalPayment" style="display: none">
														<td class="search_title_bg" width="130"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />決済種別</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlOrderPaymentKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />決済ステータス</td>
														<td class="search_item_bg" width="115">
															<asp:DropDownList id="ddlExternalPaymentStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送種別</td>
														<td class="search_item_bg" width="115">
															<asp:DropDownList id="ddlOrderShippingKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>
													<tr id="hide-field_PaymentOrderPurchaseInfo" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />決済注文ID</td>
														<td class="search_item_bg"><asp:TextBox id="tbPaymentOrderId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />決済取引ID</td>
														<td class="search_item_bg"><asp:TextBox id="tbCardTranId" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg" style="width: 150px"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文区分</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlOrderKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>
													<tr id="hide-field_PaymentAuthDate" style="display: none">
														<td class="search_title_bg" width="130"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />最終与信日時</td>
														<td class="search_item_bg" colspan="5">
															<div id="externalPaymentAuthDate">
															<uc:DateTimePickerPeriodInput id="ucExternalPaymentAuthDatePeriod" runat="server" IsNullStartDateTime="true"/> の間
																<span class="search_btn_sub">(<a href="Javascript:SetYesterday('externalPaymentAuth');">昨日</a>｜<a href="Javascript:SetToday('externalPaymentAuth');">今日</a>｜<a href="Javascript:SetThisMonth('externalPaymentAuth');">今月</a>)</span>
															&nbsp;
															<asp:CheckBox ID="cbExternalPaymentAuthDateNone" runat="server" Text=" 値なしを含む" Checked="false"></asp:CheckBox>
															</div>
														</td>
													</tr>
													<tr id="hide-field_OrderShippingReservedStatus" style="display: none">
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送伝票番号</td>
														<td class="search_item_bg" width="115">
															<asp:TextBox id="tbShippingCheckNo" runat="server" Width="125"></asp:TextBox></td>
														<%--▽ 実在庫利用が有効な場合は表示 ▽--%>
														<% if (Constants.REALSTOCK_OPTION_ENABLED){ %>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />引当状況</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList id="ddlOrderStockReservedStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="130"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />出荷状況</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlOrderShippedStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<%--△ 実在庫利用が有効な場合は表示 △--%>
														<% } else { %>
														<td class="search_item_bg" colspan="4"></td>
														<% } %>
													</tr>
													<%--▽ 返品交換情報 ▽--%>
													<tr id="hide-field_ReturnsExchangeInfo" style="display: none">
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />出荷後変更区分</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList id="ddlShippedChangedKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<%-- 配送料別途見積もり利用する場合 --%>
														<% if (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED){ %>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />送料の別途見積</td>
														<td class="search_item_bg" width="115">
															<asp:DropDownList id="ddlSeparateEstimatesFlg" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<% } else { %>
														<td class="search_item_bg" colspan="2"></td>
														<% } %>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />返品交換注文</td>
														<td class="search_item_bg" width="115">
															<asp:CheckBox ID="cbReturnExchange" Text="返品交換を含む" runat="server" AutoPostBack="True"></asp:CheckBox>
														</td>
													</tr>
													<% if (cbReturnExchange.Checked){ %>
													<tr id="hide-field_ReturnExchangeChecked" style="display: none">
														<td class="search_title_bg" width="130"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />返品交換区分</td>
														<td class="search_item_bg" width="130">
															<asp:DropDownList id="ddlReturnExchangeKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />返品交換都合区分</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList id="ddlReturnExchangeReasonKbn" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>
													<tr id="hide-field_ReturnExchangeStatus" style="display: none">
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />返品交換ステータス</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList id="ddlOrderReturnExchangeStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />返金ステータス</td>
														<td class="search_item_bg" colspan="3">
															<asp:DropDownList id="ddlOrderRepaymentStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>
													<tr id="hide-field_ReturnExchangeDate" style="display: none">
														<td class="search_title_bg" width="130"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />返品交換返金更新日</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList id="ddlReturnExchangeRepaymentUpdateDateStatus" runat="server" Width="130">
																<asp:ListItem Value=""></asp:ListItem>
																<asp:ListItem Value="order_return_exchange_receipt_date">返品交換受付日</asp:ListItem>
																<asp:ListItem Value="order_return_exchange_arrival_date">返品交換商品到着日</asp:ListItem>
																<asp:ListItem Value="order_return_exchange_complete_date">返品交換完了日</asp:ListItem>
																<asp:ListItem Value="order_repayment_date">返金日</asp:ListItem>
															</asp:DropDownList>
															が
															<div id="returnExchangeRepaymentUpdateDateStatus" style="display: inline-block;">
															<uc:DateTimePickerPeriodInput ID="ucReturnExchangeRepaymentUpdateDateStatus" runat="server" IsNullStartDateTime="true"/>
															の間
																<span class="search_btn_sub">(<a href="Javascript:SetYesterday('return_status_repayment');">昨日</a>｜<a href="Javascript:SetToday('return_status_repayment');">今日</a>｜<a href="Javascript:SetThisMonth('return_status_repayment');">今月</a>)</span>
															</div>
														</td>
													</tr>
													<% } %>
													<%--△ 返品交換情報 △--%>
													<%--▽ モール連携オプションまたは外部連携注文取込が有効の場合 ▽--%>
													<% if (Constants.MALLCOOPERATION_OPTION_ENABLED || Constants.URERU_AD_IMPORT_ENABLED) { %>
													<tr id="hide-field_MallEnabled" style="display: none">
														<td class="search_title_bg" width="130">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />サイト</td>
														<td class="search_item_bg" colspan="5">
															<asp:DropDownList id="ddlSiteName" runat="server" CssClass="search_item_width"></asp:DropDownList></td>
													</tr>
													<% } %>
													<%--△ モール連携オプションまたは外部連携注文取込が有効の場合 △--%>
													<% if (Constants.URERU_AD_IMPORT_ENABLED) { %>
													<tr id="hide-field_MallOptionEnabled" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />外部連携受注ID</td>
														<td class="search_item_bg"><asp:TextBox ID="tbExternalOrderId" Width="125" MaxLength="50" runat="server"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />外部連携取込<br />&nbsp;&nbsp;&nbsp;ステータス</td>
														<td class="search_item_bg"><asp:DropDownList ID="ddlExternalImportStatus" Width="125" runat="server"></asp:DropDownList></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />モール連携ステータス</td>
														<td class="search_item_bg"><asp:DropDownList ID="ddlMallLinkStatus" Width="125" runat="server"></asp:DropDownList></td>
													</tr>
													<%} %>
													<tr id="hide-field_MemberRankEnabled" style="display: none">
														<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
															<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />会員ランク</td>
															<td class="search_item_bg" colspan="<%= Constants.SETPROMOTION_OPTION_ENABLED ? 0 : 5 %>">
																<asp:DropDownList ID="ddlMemberRankId" runat="server" CssClass="search_item_width"></asp:DropDownList>
															</td>
														<% } %>

														<% if (Constants.SETPROMOTION_OPTION_ENABLED) { %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />セット<br />&nbsp;&nbsp;プロモーションID</td>
														<td class="search_item_bg" colspan="<%= Constants.MEMBER_RANK_OPTION_ENABLED ? 3 : 5 %>">
															<asp:TextBox id="tbSetPromotionId" runat="server" Width="125"></asp:TextBox>
														</td>
														<% } %>

													</tr>
													<% if (Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED || Constants.SUBSCRIPTION_BOX_OPTION_ENABLED){ %>
													<tr id="hide-field_NoveltyEnabled" style="display: none">
														<%--▽ ノベルティOPが有効の場合 ▽--%>
														<% if (Constants.NOVELTY_OPTION_ENABLED){ %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ノベルティID</td>
														<td class="search_item_bg" colspan="<%= Constants.RECOMMEND_OPTION_ENABLED ? 1 : 5 %>"><asp:TextBox id="tbNoveltyId" runat="server" Width="125" MaxLength="30" /></td>
														<% } %>
														<%--△ ノベルティOPが有効の場合 △--%>
														<%--▽ レコメンド設定OPが有効の場合 ▽--%>
														<% if (Constants.RECOMMEND_OPTION_ENABLED){ %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />レコメンドID</td>
														<td class="search_item_bg" colspan="<%= Constants.NOVELTY_OPTION_ENABLED ? 3 : 5 %>"><asp:TextBox id="tbRecommendId" runat="server" Width="125" MaxLength="30" /></td>
														<% } %>
														<%--△ レコメンド設定OPが有効の場合 △--%>
													</tr>
													<% } %>
													<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED || Constants.GIFTORDER_OPTION_ENABLED) { %>
													<tr id="hide-field_ContentsOptionEnabled" style="display: none">
														<%-- デジタルコンテンツオプションが有効の場合 --%>
														<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED) { %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />デジタルコンテンツ</td>
														<td class="search_item_bg">
														<asp:DropDownList id="ddlDigitalContentsFlg" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<% } %>
														<%-- ギフトオプションが有効の場合 --%>
														<%if (Constants.GIFTORDER_OPTION_ENABLED){ %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ギフト購入</td>
														<td class="search_item_bg">
															<asp:DropDownList id="ddlGiftFlg" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<% } %>
														<% if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED  && Constants.GIFTORDER_OPTION_ENABLED) { %>
														<td class="search_item_bg" colspan="2"></td>
														<% } else if ((Constants.DIGITAL_CONTENTS_OPTION_ENABLED  ^ Constants.GIFTORDER_OPTION_ENABLED) == true) { %><%-- true: 1箇所 --%>
														<td class="search_item_bg" colspan="4"></td>
														<% }%>
													</tr>
													<% }%>
													<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
													<tr id="hide-field_FixedPurchaseOptionEnabled" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />定期購入商品</td>
														<td class="search_item_bg">
														<asp:DropDownList id="ddlFixedPurchase" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />定期購入回数<br/>　(注文時点)</td>
														<td class="search_item_bg">
														<asp:TextBox id="tbOrderCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbOrderCountTo" runat="server" Width="45" />
														</td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />定期購入回数<br/>　(出荷時点)</td>
														<td class="search_item_bg">
														<asp:TextBox id="tbShippedCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbShippedCountTo" runat="server" Width="45" />
														</td>
													<% } %>
													</tr>
													<tr id="hide-field_OrderCountSubscribeCount" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />購入回数<br/>　(注文時点)</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbOrderTotalCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbOrderTotalCountTo" runat="server" Width="45" />
														</td>
														<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />頒布会コースID</td>
														<td class="search_item_bg"><asp:TextBox id="tbSubscriptionBoxCourseId" runat="server" Width="125" MaxLength="30" /></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />頒布会購入回数</td>
														<td class="search_item_bg">
															<asp:TextBox id="tbSubscriptionBoxOrderCountFrom" runat="server" Width="45" />&nbsp;～&nbsp;<asp:TextBox id="tbSubscriptionBoxOrderCountTo" runat="server" Width="45" />
														</td>
														<% } %>
													</tr>
													<% if (Constants.ORDER_EXTEND_OPTION_ENABLED) { %>
													<tr id="hide-field_OrderExtendOptionEnabled" style="display: none">
														<td class="search_title_bg" width="106px"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文拡張項目</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList id="ddlOrderExtendName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlOrderExtend_SelectedIndexChanged"></asp:DropDownList>
															<asp:DropDownList id="ddlOrderExtendFlg" Visible="false" runat="server"></asp:DropDownList>
															<asp:TextBox ID="tbOrderExtendText" Visible="false" runat="server" Width="425px"></asp:TextBox>
															<asp:DropDownList id="ddlOrderExtendText" Visible="false" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<% } %>
													<tr id="hide-field_OrderMemo" style="display: none">
														<td class="search_title_bg" width="106px"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文メモ</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlOrderMemoFlg" runat="server"></asp:DropDownList>
															<asp:TextBox ID="tbMemo" runat="server" width="387"></asp:TextBox>
														</td>
													</tr>
													<tr id="hide-field_ManagementMemo" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />管理メモ</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlOrderManagementMemoFlg" runat="server"></asp:DropDownList>
															<asp:TextBox ID="tbManagementMemo" runat="server" Width="387"></asp:TextBox>
														</td>
													</tr>
													<tr id="hide-field_ShippingMemo" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送メモ</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlShippingMemoFlg" runat="server" />
															<asp:TextBox ID="tbShippingMemo" runat="server" Width="387" />
														</td>
													</tr>
													<tr id="hide-field_PaymentMemo" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />決済連携メモ</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlOrderPaymentMemoFlg" runat="server"></asp:DropDownList>
															<asp:TextBox ID="tbPaymentMemo" runat="server" width="387"></asp:TextBox>
														</td>
													</tr>
													<tr id="hide-field_RelationMemo" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />外部連携メモ</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlOrderRelationMemoFlg" runat="server"></asp:DropDownList>
															<asp:TextBox ID="tbRelationMemo" runat="server" Width="387"></asp:TextBox>
														</td>
													</tr>
													<tr id="hide-field_UserMemo" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ユーザー特記欄</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlUserMemoFlg" runat="server"></asp:DropDownList>
															<asp:TextBox ID="tbUserMemo" runat="server" Width="387"></asp:TextBox>
														</td>
													</tr>
													<tr id="hide-field_ProductOption" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品付帯情報</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlProductOptionFlg" runat="server"></asp:DropDownList>
															<asp:TextBox ID="tbProductOption" runat="server" Width="387"></asp:TextBox>
														</td>
													</tr>
													<%if (Constants.W2MP_AFFILIATE_OPTION_ENABLED){ %>
													<tr id="hide-field_AdsCode" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />広告コード</td>
														<td align="left" class="search_item_bg" colspan="5">
															<asp:DropDownList ID="ddlAdvCode" runat="server" Width="130"></asp:DropDownList>
															<asp:TextBox ID="tbAdvCode" runat="server" Width="200"></asp:TextBox>
														</td>
													</tr>
													<%} %>
													<%--▽ 法人項目表示フラグが有効の場合 ▽--%>
													<%if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
													<tr id="hide-field_CompanyName" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />注文者の<%: ReplaceTag("@@User.company_name.name@@")%></td>
														<td class="search_item_bg" colspan="5">
															<asp:TextBox id="tbCompanyName" runat="server" Width="125"></asp:TextBox>
														</td>
													</tr>
													<%} %>
													<tr id="hide-field_UserManagementLevel" style="display: none">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />ユーザー<br />&nbsp;&nbsp;&nbsp;管理レベル</td>
														<td class="search_item_bg" width="510" colspan="5">
															<asp:DropDownList id="ddlUserManagementLevelId" runat="server" CssClass="search_item_width"></asp:DropDownList>
															<asp:CheckBox ID="cbUserManagementLevelExclude" runat="server" Text="除外する" Checked="false"></asp:CheckBox>
														</td>
													</tr>
													<tr id="hide-field_ShippingSection" style="display: none">
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送先</td>
														<td class="search_item_bg" width="510">
															<asp:DropDownList id="ddlAnotherShippingFlag" runat="server" CssClass="search_item_width"></asp:DropDownList>
															<%if (Constants.GIFTORDER_OPTION_ENABLED) { %>
															※複数配送先注文は、何れの選択肢でも検索対象になる場合があります。
															<%} %>
														</td>
														<td class="search_title_bg" width="95">
															<img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送状態
														</td>
														<td class="search_item_bg" width="510">
															<asp:DropDownList id="ddlShippingStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送方法</td>
														<td class="search_item_bg"><asp:DropDownList id="ddlShippingMethod" runat="server" CssClass="search_item_width" /></td>
													</tr>
													<% if (Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED) { %>
													<tr id="hide-field_ShippingStatus" style="display: none">
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />完了状態コード</td>
														<td class="search_item_bg" width="115">
															<asp:DropDownList id="ddlShippingStatusCode" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
														<td class="search_title_bg" width="95"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />現在の状態</td>
														<td class="search_item_bg" width="115">
															<asp:DropDownList id="ddlShippingCurrentStatus" runat="server" CssClass="search_item_width"></asp:DropDownList>
														</td>
													</tr>
													<% } %>
													<% if (this.IsSearchShippingAddr1) { %>
													<tr id="hide-field_SearchShippingAddr" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送先：都道府県</td>
														<td class="search_item_bg" colspan="5">
															 <span ID="LocalArea1"><asp:CheckBox ID="cbLocalArea1" runat="server" style="font-weight:bold" Text="北海道"></asp:CheckBox></span>
															<span ID="LocalArea2"><asp:CheckBox ID="cbLocalArea2" runat="server" style="font-weight:bold" Text="東北"></asp:CheckBox></span>
															<span ID="LocalArea3"><asp:CheckBox ID="cbLocalArea3" runat="server" style="font-weight:bold" Text="関東"></asp:CheckBox></span>
															<span ID="LocalArea4"><asp:CheckBox ID="cbLocalArea4" runat="server" style="font-weight:bold" Text="中部"></asp:CheckBox></span>
															<span ID="LocalArea5"><asp:CheckBox ID="cbLocalArea5" runat="server" style="font-weight:bold" Text="近畿"></asp:CheckBox></span>
															<span ID="LocalArea6"><asp:CheckBox ID="cbLocalArea6" runat="server" style="font-weight:bold" Text="中国"></asp:CheckBox></span>
															<span ID="LocalArea7"><asp:CheckBox ID="cbLocalArea7" runat="server" style="font-weight:bold" Text="四国"></asp:CheckBox></span>
															<span ID="LocalArea8"><asp:CheckBox ID="cbLocalArea8" runat="server" style="font-weight:bold" Text="九州/沖縄"></asp:CheckBox></span>
															<asp:CheckBoxlist id="cblShippingPrefectures" runat="server" DataTextField="Key" DataValueField="Value"
																			RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="50"></asp:CheckBoxlist>
														</td>
													</tr>
													<% } %>
													<% if (OrderCommon.CanDisplayInvoiceBundle()) { %>
													<tr id="hide-field_DisplayInvoice" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />請求書同梱フラグ</td>
														<td class="search_item_bg"><asp:DropDownList id="ddlInvoiceBundleFlg" runat="server" CssClass="search_item_width"></asp:DropDownList></td>
														<td class="search_item_bg" colspan="4"></td>
													</tr>
													<% } %>
													<%--▽ 出荷予定日オプションが有効の場合 ▽--%>
													<% if (this.UseLeadTime) { %>
													<tr id="hide-field_UseLeadTime" style="display: none">
														<td class="search_title_bg" width="130">
															<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
															出荷予定日
														</td>
														<td class="search_item_bg" colspan="5">
															<div id="scheduledShippingDate">
															<uc:DateTimePickerPeriodInput id="ucOrderScheduledShippingDatePeriod" runat="server" IsNullStartDateTime="true" IsHideTime="True" /> の間
																<span class="search_btn_sub">(<a href="Javascript:SetYesterday('scheduled_shipping_date');">昨日</a>｜<a href="Javascript:SetToday('scheduled_shipping_date');">今日</a>｜<a href="Javascript:SetThisMonth('scheduled_shipping_date');">今月</a>)</span>
															&nbsp;
															<asp:CheckBox ID="cbOrderScheduledShippingNone" runat="server" Checked="false" Text="を含む"></asp:CheckBox>
															</div>
														</td>
													</tr>
													<% } %>
													<tr id="hide-field_ShippingDate" style="display: none">
														<td class="search_title_bg" width="130"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />配送希望日</td>
														<td class="search_item_bg" colspan="5">
															<div id="orderShippingDate">
															<uc:DateTimePickerPeriodInput id="ucOrderShippingDatePeriod" runat="server" IsNullStartDateTime="true" IsHideTime="True" /> の間
																<span class="search_btn_sub">(<a href="Javascript:SetYesterday('shippingdate');">昨日</a>｜<a href="Javascript:SetToday('shippingdate');">今日</a>｜<a href="Javascript:SetThisMonth('shippingdate');">今月</a>)</span>
															&nbsp;
															<asp:CheckBox ID="cbOrderShippingNone" runat="server" Checked="false" Text="を含む"></asp:CheckBox>
															</div>
														</td>
													</tr>
													<tr id="hide-field_ShippingCountry" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送先名</td>
														<td class="search_item_bg"><asp:TextBox id="tbShippingName" runat="server" Width="125"></asp:TextBox></td>
														<% if (this.IsShippingCountryAvailableJp) { %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送先名（かな）</td>
														<td class="search_item_bg"><asp:TextBox id="tbShippingNameKana" runat="server" Width="125"></asp:TextBox></td>
														<% } %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送先郵便番号</td>
														<td class="search_item_bg"><asp:TextBox id="tbShippingZip" runat="server" Width="125"></asp:TextBox></td>
														<% if (this.IsShippingCountryAvailableJp == false) { %>
															<td class="search_item_bg" colspan="2"></td>
														<% } %>
													</tr>
													<tr id="hide-field_ShippingInfo" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送先住所</td>
														<td class="search_item_bg"><asp:TextBox id="tbShippingAddr" runat="server" width="125"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />配送先電話番号</td>
														<td class="search_item_bg" colspan="3"><asp:TextBox id="tbShippingTel1" runat="server" Width="125"></asp:TextBox></td>
													</tr>
													<tr id="hide-field_CouponInfo" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />クーポンコード</td>
														<td class="search_item_bg"><asp:TextBox id="tbCouponCode" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />クーポン名（管理用）</td>
														<td class="search_item_bg"><asp:TextBox id="tbCouponName" runat="server" Width="125"></asp:TextBox></td>
														<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品セールID</td>
														<td class="search_item_bg"><asp:TextBox id="tbProductSaleId" runat="server" Width="125"></asp:TextBox></td>
														<% } else { %>
														<td class="search_item_bg" colspan="2"></td>
														<% } %>
													</tr>
													<% if (Constants.PRODUCTBUNDLE_OPTION_ENABLED) { %>
													<tr id="hide-field_ProductBundleOption" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />商品同梱ID</td>
														<td class="search_item_bg"  colspan="5"><asp:TextBox ID="tbProductBundleId" runat="server" Width="125"></asp:TextBox></td>
													</tr>
													<%} %>
													<%--▼領収書希望有無▼--%>
													<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
													<tr id="hide-field_ReceiptOption" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />領収書希望</td>
														<td class="search_item_bg" colspan="5"><asp:DropDownList id="ddlReceiptFlg" runat="server" /></td>
													</tr>
													<% } %>
													<% if (Constants.STORE_PICKUP_OPTION_ENABLED) { %>
													<tr id="hide-field_StorePickupStatusOption" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />店舗受取ステータス</td>
														<td class="search_item_bg" colspan="5"><asp:DropDownList id="ddlStorePickupStatus" runat="server" /></td>
													</tr>
													<% } %>
													<%--▲領収書希望有無▲--%>
													<%--▼Taiwan Invoice▼--%>
													<% if (OrderCommon.DisplayTwInvoiceInfo()) { %>
													<tr id="hide-field_TwInvoiceOption" style="display: none">
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />發票番号</td>
														<td class="search_item_bg"><asp:TextBox id="tbInvoiceNo" runat="server" Width="125"></asp:TextBox></td>
														<td class="search_title_bg"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />發票ステータス</td>
														<td class="search_item_bg"><asp:DropDownList id="ddlInvoiceStatus" runat="server" Width="125"></asp:DropDownList></td>
														<td class="search_item_bg" colspan="2"></td>
													</tr>
													<% } %>
													<%--Taiwan Invoice--%>
												</table>
											<%-- 検索項目を表示・非表示ボタン --%>

												<div id="order-hide-search-field-slide-toggle" style="text-align: center;">
													<span id="check-toggle-text-order">全ての検索項目を表示</span>
													<span id="check-toggle-open">
														<span class="toggle-state-icon icon-arrow-down"/>
													</span>
												</div>

											<a name="<%: Constants.ANCHOR_NAME_FOR_DISP_PROVISIONAL_CREDITCARD %>"></a>
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
										<%-- マスタ出力 --%>
										<tr>
											<td class="search_table">
												<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="Order" TableWidth="758" />
											</td>
										</tr>
										<tr><td><img height="5" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 検索 △-->
	<tr>
		<td>
			<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
		</td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr id="orderListSearchResult">
		<td>
			<h2 class="cmn-hed-h2">受注情報一覧</h2>
		</td>
	</tr>
	<tr>
		<td>
			<table class="box_border cmn-section" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td><img height="12" alt="" src="../../Images/Common/sp.gif" border="0" class="hide" /></td>
								<td align="center">
									<div id="divOrderList" runat="server">
										<table cellspacing="0" cellpadding="0" border="0" class="wide-content">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td align="left">
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" border="0" style="width: 750px">
														<tr>
															<td style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
															<td class="action_list_sp" style="height: 22px;">
																<%if (Constants.MARKETINGPLANNER_TARGETLIST_OPTION_ENABLE){%>
																	<% btnImportTargetList.OnClientClick = "javascript:open_window('" + ImportTargetListUrlCreator.Create(Constants.KBN_WINDOW_POPUP) + "','Import','width=850,height=370,top=120,left=420,status=no,scrollbars=yes');return false;"; %>
																	<asp:Button ID="btnImportTargetList" runat="server" Text="  ターゲットリスト作成  " Enabled="false" EnableViewState="False" UseSubmitBehavior="False" CssClass="cmn-btn-sub-action" />
																<%} %>
															</td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<div id="divDisplayList" runat="server">
											<tr>
												<td>
													<div>
														<% if (rList.Items.Count > 0) { %>
														<%-- ▽ テーブルヘッダ ▽ --%>
														<div>
															<table border="0" cellpadding="0" cellspacing="0">
																<tr>
																	<td>
																		<!-- ▽ 固定ヘッダ ▽ -->	
																		<div class="div_header_left">
																			<table class="list_table tableHeaderFixed" cellspacing="1" cellpadding="3" border="0" style="height:100%">
																				<tr class="list_title_bg">
																					<td width="110" align="center">注文ID</td>
																				</tr>
																			</table>
																		</div>
																		<!-- △ 固定ヘッダ △ -->
																	</td>
																	<td>
																		<!-- ▽ ヘッダ ▽ -->	
																		<div class="div_header_right">
																			<table class="list_table tableHeader" cellspacing="1" cellpadding="3" width="<%: this.ColumnDisplaySettings.Where(x => x.CanDisplay).Sum(x => x.ColmunWidth) %>" border="0" style="height:103px">
																				<!-- 水平ヘッダ -->
																				<tr class="list_title_bg">
																					<asp:Repeater ID="rManagerListDispSetting" ItemType="w2.Domain.ManagerListDispSetting.ManagerListDispSettingModel" runat="server" >
																					<ItemTemplate>
																							<td id="Td1" align="center" width="<%#: Item.ColmunWidth %>" visible="<%# (((Item == null) == false) && Item.CanDisplay && Item.IsNotFixedColmun && IsOptionCooperation(Item.DispColmunName)) %>" runat="server">
																								<%# HtmlSanitizer.HtmlEncodeChangeToBr(ConvertDispColumnNameFormat(Item.DispColmunName)) %>
																							</td>
																					</ItemTemplate>
																					</asp:Repeater>
																				</tr>
																			</table>
																		</div>
																		<!-- △ ヘッダ △ -->	
																	</td>
																</tr>
															</table>
														</div>
														<%-- △ テーブルヘッダ △ --%>

														<%-- ▽ テーブルデータ ▽ --%>
														<div class="div_data" style=" max-height: 420px; height:<%= rList.Items.Count * 50 + 20 %>px;">
															<%-- ▽ 固定データ ▽ --%>
															<div class="div_data_left">
																	<table class="list_table tableDataFix" cellspacing="1" cellpadding="3" border="0" style=" max-height: 420px; height:<%= rList.Items.Count * 50 + 20 %>px;">
																		 <!-- 垂直ヘッダ -->
																		<asp:Repeater id="rTableFixColumn" Runat="server">
																			<HeaderTemplate>
																			</HeaderTemplate>
																			<ItemTemplate>
																				<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateOrderDetailUrl((String)Eval(Constants.FIELD_ORDER_ORDER_ID))) %>')" style="height:<%#: Eval(Constants.FIELD_ORDER_ORDER_ID).ToString().Length >= 20 ? "47px" : "33px" %>">
																					<td align="left" width="110"><%# WebSanitizer.HtmlEncode(Eval(Constants.FIELD_ORDER_ORDER_ID)) %></td>
																				</tr>
																			</ItemTemplate>
																		</asp:Repeater>
																	</table>
															</div>
															<%-- △ 固定データ △ --%>

															<%-- ▽ データ ▽ --%>
															<div class="div_data_right">
																<table class="list_table tableData" cellspacing="1" cellpadding="3" width="<%= this.ColumnDisplaySettings.Where(x => x.CanDisplay).Sum(x => x.ColmunWidth) %>" border="0" style="max-height: 420px; height:<%= rList.Items.Count * 50 +20 %>px;">
																<asp:Repeater id="rList" Runat="server">
																	<HeaderTemplate>
																	</HeaderTemplate>
																	<ItemTemplate>
																		<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%# WebSanitizer.UrlAttrHtmlEncode(CreateOrderDetailUrl((String)Eval(Constants.FIELD_ORDER_ORDER_ID))) %>')" style="height:<%#: Eval(Constants.FIELD_ORDER_ORDER_ID).ToString().Length >= 20 ? "49px" : "33px" %>">
																			<%--表示設定データの取得--%>
																			<asp:Repeater ID="Repeater1" DataSource="<%# this.ColumnDisplaySettings %>" ItemType="w2.Domain.ManagerListDispSetting.ManagerListDispSettingModel" runat="server">
																				<ItemTemplate>
																					<%--表示データ--%>
																					<td id="Td2" align="<%#: Item.ColmunAlign %>" width="<%#: Item.ColmunWidth %>" runat="server" visible="<%# (((Item == null) == false) && Item.CanDisplay && Item.IsNotFixedColmun && IsOptionCooperation(Item.DispColmunName)) %>">
																						<%#: ConvertItemFormatForDisplayList(Item.DispColmunName, ((DataView)rList.DataSource)[((RepeaterItem)Container.Parent.Parent).ItemIndex][Item.DispColmunName], ((DataView)rList.DataSource)[((RepeaterItem)Container.Parent.Parent).ItemIndex][Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID].ToString()) %>
																					</td>
																				</ItemTemplate>
																			</asp:Repeater>
																		</tr>
																	</ItemTemplate>
																</asp:Repeater>
															</table>
															</div>
															<%-- △ データ △ --%>
														</div>
														<%-- △ テーブルデータ △ --%>
														<% } else { %>
														<%-- ▽ エラーの場合 ▽ --%>
															<table class="list_table" cellspacing="1" cellpadding="3" width="734" border="0" style="height:100%"> <!-- 水平ヘッダ -->
																<tr id="trListError" class="list_alert" runat="server" Visible="False">
																	<td id="tdErrorMessage" colspan="31" runat="server"></td>
																</tr>
															</table>
														<%-- △ エラーの場合 △ --%>
														<% } %>
													</div>
												</td>
											</tr>
											</div>
											<div id="divDisplayMessage" runat="server">
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_alert">
															<td colspan="9"><%= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NOT_SEARCH_DEFAULT) %></td>
														</tr>
													</table>
												</td>
											</tr>
											</div>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="info_item_bg">
															<td align="left">備考<br />
																・氏名の後ろに「<%= Constants.USERSYMBOL_REPEATER %>」付のユーザーは注文を２回以上しているリピーターユーザー、「<%= Constants.USERSYMBOL_HAS_NOTE %>」付のユーザーは特記事項のある特記ユーザーを表します。<br />
																<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
																・定期購入商品を『含む』とした場合、「定期購入商品のみ」または「通常・定期商品が混在」注文を対象に絞り込みます。<br />
																<%} %>
																・ユーザー管理レベルを検索する際に、「除外する」にチェックを入れて検索すると、選択した管理レベル以外のユーザーを一覧に表示できます。
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// 昨日設定
	function SetYesterday(set_date) {
		// ステータス更新日
		if (set_date == 'order') {
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucOrderUpdateDatePeriod.ClientID %>');
		}
		// 拡張ステータス更新日
		else if (set_date == 'extendStatusUpdateDate') {
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucExtendStatusUpdateDatePeriod.ClientID %>');
		}
		// 返品交換返金更新日
		else if (set_date == 'return_status_repayment') {
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucReturnExchangeRepaymentUpdateDateStatus.ClientID %>');
		}
		// 配送希望日
		else if (set_date == 'shippingdate') {
			document.getElementById('<%= ucOrderShippingDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucOrderShippingDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucOrderShippingDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucOrderShippingDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucOrderShippingDatePeriod.ClientID %>');
		}
		// 最終与信日
		else if (set_date == 'externalPaymentAuth') {
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucExternalPaymentAuthDatePeriod.ClientID %>');
		}
		// 出荷予定日
		else if (set_date == 'scheduled_shipping_date') {
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now.Date.AddDays(-1)) %>';
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucOrderScheduledShippingDatePeriod.ClientID %>');
		}
	}

	// 今日設定
	function SetToday(set_date) {
		// ステータス更新日
		if (set_date == 'order') {
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucOrderUpdateDatePeriod.ClientID %>');
		}
		// 拡張ステータス更新日
		else if (set_date == 'extendStatusUpdateDate') {
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucExtendStatusUpdateDatePeriod.ClientID %>');
		}
		// 返品交換返金更新日
		else if (set_date == 'return_status_repayment') {
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucReturnExchangeRepaymentUpdateDateStatus.ClientID %>');
		}
		// 配送希望日
		else if (set_date == 'shippingdate') {
			document.getElementById('<%= ucOrderShippingDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucOrderShippingDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucOrderShippingDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucOrderShippingDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucOrderShippingDatePeriod.ClientID %>');
		}
		// 最終与信日
		else if (set_date == 'externalPaymentAuth') {
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucExternalPaymentAuthDatePeriod.ClientID %>');
		}
		// 出荷予定日
		else if (set_date == 'scheduled_shipping_date') {
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetDisplayDateString(DateTime.Now) %>';
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucOrderScheduledShippingDatePeriod.ClientID %>');
		}
	}

	// 今月設定
	function SetThisMonth(set_date) {
		// ステータス更新日
		if (set_date == 'order') {
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucOrderUpdateDatePeriod.ClientID %>');
		}
		// 拡張ステータス更新日
		else if (set_date == 'extendStatusUpdateDate') {
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucExtendStatusUpdateDatePeriod.ClientID %>');
		}
		// 返品交換返金更新日
		else if (set_date == 'return_status_repayment') {
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucReturnExchangeRepaymentUpdateDateStatus.ClientID %>');
		}
		// 配送希望日
		else if (set_date == 'shippingdate') {
			document.getElementById('<%= ucOrderShippingDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucOrderShippingDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucOrderShippingDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucOrderShippingDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucOrderShippingDatePeriod.ClientID %>');
		}
		// 最終与信日
		else if (set_date == 'externalPaymentAuth') {
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucExternalPaymentAuthDatePeriod.ClientID %>');
		}
		// 出荷予定日
		else if (set_date = 'scheduled_shipping_date') {
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfStartDate.ClientID %>').value = '<%= DateTimeUtility.GetStartDateThisMonthString() %>';
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfStartTime.ClientID %>').value = '00:00:00';
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfEndDate.ClientID %>').value = '<%= DateTimeUtility.GetEndDateThisMonthString() %>';
			document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfEndTime.ClientID %>').value = '23:59:59';
			reloadDisplayDateTimePeriod('<%= ucOrderScheduledShippingDatePeriod.ClientID %>');
		}
	}

	// テキストボックスの有効無効切り替えイベントの設定
	$(function () {
		// 注文メモ検索のテキストボックス
		SetDisplayMemoTextBox($("#<%=ddlOrderMemoFlg.ClientID %>"), $("#<%=tbMemo.ClientID %>"));
		$("#<%=ddlOrderMemoFlg.ClientID %>").change(function () { SetDisplayMemoTextBox($("#<%=ddlOrderMemoFlg.ClientID %>"), $("#<%=tbMemo.ClientID %>")); });

		// 管理メモのテキストボックス
		SetDisplayMemoTextBox($("#<%=ddlOrderManagementMemoFlg.ClientID %>"), $("#<%=tbManagementMemo.ClientID %>"));
		$("#<%=ddlOrderManagementMemoFlg.ClientID %>").change(function () { SetDisplayMemoTextBox($("#<%=ddlOrderManagementMemoFlg.ClientID %>"), $("#<%=tbManagementMemo.ClientID %>")); });

		// 配送メモのテキストボックス
		var shippingMemoFlg = $("#<%=ddlShippingMemoFlg.ClientID %>");
		var shippingMemo = $("#<%=tbShippingMemo.ClientID %>");
		SetDisplayMemoTextBox(shippingMemoFlg, shippingMemo);
		shippingMemoFlg.change(function () { SetDisplayMemoTextBox(shippingMemoFlg, shippingMemo); });

		// 決済連携メモのテキストボックス
		SetDisplayMemoTextBox($("#<%=ddlOrderPaymentMemoFlg.ClientID %>"), $("#<%=tbPaymentMemo.ClientID %>"));
		$("#<%=ddlOrderPaymentMemoFlg.ClientID %>").change(function () { SetDisplayMemoTextBox($("#<%=ddlOrderPaymentMemoFlg.ClientID %>"), $("#<%=tbPaymentMemo.ClientID %>")); });

		// 外部連携メモのテキストボックス
		SetDisplayMemoTextBox($("#<%=ddlOrderRelationMemoFlg.ClientID %>"), $("#<%=tbRelationMemo.ClientID %>"));
		$("#<%=ddlOrderRelationMemoFlg.ClientID %>").change(function () { SetDisplayMemoTextBox($("#<%=ddlOrderRelationMemoFlg.ClientID %>"), $("#<%=tbRelationMemo.ClientID %>")); });

		// User Memo
		SetDisplayMemoTextBox($("#<%=ddlUserMemoFlg.ClientID %>"), $("#<%=tbUserMemo.ClientID %>"));
		$("#<%=ddlUserMemoFlg.ClientID %>").change(function () { SetDisplayMemoTextBox($("#<%=ddlUserMemoFlg.ClientID %>"), $("#<%=tbUserMemo.ClientID %>")); });

		// 商品付帯情報のテキストボックス
		SetDisplayMemoTextBox($("#<%=ddlProductOptionFlg.ClientID %>"), $("#<%=tbProductOption.ClientID %>"));
		$("#<%=ddlProductOptionFlg.ClientID %>").change(function () { SetDisplayMemoTextBox($("#<%=ddlProductOptionFlg.ClientID %>"), $("#<%=tbProductOption.ClientID %>")); });

		// AdvCode
		SetDisplayValueTextBox($("#<%=ddlAdvCode.ClientID %>"), $("#<%=tbAdvCode.ClientID %>"));
		$("#<%=ddlAdvCode.ClientID %>").change(function () { SetDisplayValueTextBox($("#<%=ddlAdvCode.ClientID %>"), $("#<%=tbAdvCode.ClientID %>")); });
	});

	// Form Reset
	function Reset() {
		document.getElementById('<%= this.Form.ClientID %>').reset();
		SetDisplayMemoTextBox($("#<%=ddlUserMemoFlg.ClientID %>"), $("#<%=tbUserMemo.ClientID %>"));
		SetDisplayMemoTextBox($("#<%=ddlOrderMemoFlg.ClientID %>"), $("#<%=tbMemo.ClientID %>"));
		SetDisplayMemoTextBox($("#<%=ddlOrderManagementMemoFlg.ClientID %>"), $("#<%=tbManagementMemo.ClientID %>"));
		SetDisplayMemoTextBox($("#<%=ddlShippingMemoFlg.ClientID %>"), $("#<%=tbShippingMemo.ClientID %>"));
		SetDisplayMemoTextBox($("#<%=ddlOrderPaymentMemoFlg.ClientID %>"), $("#<%=tbPaymentMemo.ClientID %>"));
		SetDisplayMemoTextBox($("#<%=ddlOrderRelationMemoFlg.ClientID %>"), $("#<%=tbRelationMemo.ClientID %>"));
		SetDisplayMemoTextBox($("#<%=ddlProductOptionFlg.ClientID %>"), $("#<%=tbProductOption.ClientID %>"));
		SetDisplayValueTextBox($("#<%=ddlAdvCode.ClientID %>"), $("#<%=tbAdvCode.ClientID %>"));

		document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucOrderUpdateDatePeriod.HfStartTime.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucOrderUpdateDatePeriod.HfEndTime.ClientID %>').value = '<%= string.Empty %>';
		reloadDisplayDateTimePeriod('<%= ucOrderUpdateDatePeriod.ClientID %>');

		document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfStartTime.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucExtendStatusUpdateDatePeriod.HfEndTime.ClientID %>').value = '<%= string.Empty %>';
		reloadDisplayDateTimePeriod('<%= ucExtendStatusUpdateDatePeriod.ClientID %>');

		document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfStartDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfStartTime.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfEndDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucExternalPaymentAuthDatePeriod.HfEndTime.ClientID %>').value = '<%= string.Empty %>';
		reloadDisplayDateTimePeriod('<%= ucExternalPaymentAuthDatePeriod.ClientID %>');

		var cbReturnExchangeChecked = document.getElementById('<%= cbReturnExchange.ClientID %>')
		var isReturnExchangeChecked = cbReturnExchangeChecked.checked
		if (isReturnExchangeChecked) {
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfStartDate.ClientID %>').value = '<%= string.Empty %>';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfStartTime.ClientID %>').value = '<%= string.Empty %>';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfEndDate.ClientID %>').value = '<%= string.Empty %>';
			document.getElementById('<%= ucReturnExchangeRepaymentUpdateDateStatus.HfEndTime.ClientID %>').value = '<%= string.Empty %>';
			reloadDisplayDateTimePeriod('<%= ucReturnExchangeRepaymentUpdateDateStatus.ClientID %>');
		}

		document.getElementById('<%= ucOrderShippingDatePeriod.HfStartDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucOrderShippingDatePeriod.HfStartTime.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucOrderShippingDatePeriod.HfEndDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucOrderShippingDatePeriod.HfEndTime.ClientID %>').value = '<%= string.Empty %>';
		reloadDisplayDateTimePeriod('<%= ucOrderShippingDatePeriod.ClientID %>');

		document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfStartDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfStartTime.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfEndDate.ClientID %>').value = '<%= string.Empty %>';
		document.getElementById('<%= ucOrderScheduledShippingDatePeriod.HfEndTime.ClientID %>').value = '<%= string.Empty %>';
		reloadDisplayDateTimePeriod('<%= ucOrderScheduledShippingDatePeriod.ClientID %>');
	}

	// ドロップダウンの値に応じて、テキストボックスの有効無効を切り替える。
	function SetDisplayMemoTextBox($ddlEle, $tbEle) {
		if ($ddlEle.val() != 1) {
			$tbEle.val("");
			$tbEle.attr("disabled", "disabled");
		}
		else {
			$tbEle.removeAttr("disabled");
		}
	}

	// Set Display Adv TextBox
	function SetDisplayValueTextBox($ddlEle, $tbEle) {
		if ($ddlEle.val() == "") {
			$tbEle.val("");
			$tbEle.attr("disabled", "disabled");
		}
		else {
			$tbEle.removeAttr("disabled");
		}
	}

</script>

<script type="text/javascript">
	$(function () {
		$(".tableHeader").css("table-layout", "fixed");
		$(".tableData").css("table-layout", "fixed");

		$(".tableData").find("td").each(function () {
			$(this).attr("title", $(this).text()).css("overflow", "hidden");
		});

		setWidthTwoTable("tableData", "tableHeader");
		setHeightTwoTable("tableHeaderFixed", "tableHeader");

		$(".div_data_left").height($(".div_data_right").outerHeight());
		setHeightTwoTable("tableDataFix", "tableData");
		setHeightTwoTable("tableData", "tableDataFix"); // ２つテーブルを同じ高さを設定するため
		setWidthTwoTable("tableData", "tableHeader");
		setWidthTwoTable("tableHeader", "tableData");
		$(".div_header_left").height($(".div_header_right").outerHeight());

		scrollLeftTwoTable("div_header_right", "div_data_right");
		scrollTopTwoTable("div_data_left", "div_data_right");

		hoverTwoTable("tableDataFix", "tableData");
		hoverTwoTable("tableData", "tableDataFix");

		var isMobile = getMobileOperatingSystem();
		if (isMobile) {
			$('.div_data_left').css('overflow-x', 'hidden');
			$('.div_header_right').css('overflow-y', 'hidden');
		}
		else {
			$('.div_data_left').css('overflow-x', 'scroll');
			$('.div_header_right').css('overflow-y', 'scroll');
		}

		$(window).bind('resize', function () {
			var isMobile = getMobileOperatingSystem();
			if (isMobile) {
				$('.div_data_left').css('overflow-x', 'hidden');
				$('.div_header_right').css('overflow-y', 'hidden');
			}
			else {
				$('.div_data_left').css('overflow-x', 'scroll');
				$('.div_header_right').css('overflow-y', 'scroll');
			}
		});
	});
</script>

<%--//// 検索時の非表示--%>
<script type="text/javascript" src="<%= ResolveUrl("~/Js/hide-show_search_field.js") %>"></script>

<% if (this.IsSearchShippingAddr1) { %>
	<script type="text/javascript" src="<%= ResolveUrl("~/Js/prefectures.js") %>"></script>
<% } %>

</asp:Content>
