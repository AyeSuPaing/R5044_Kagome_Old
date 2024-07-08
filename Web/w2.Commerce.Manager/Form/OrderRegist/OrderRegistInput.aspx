<%--
=========================================================================================================
  Module      : 新規注文登録ページ(OrderRegistInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="OrderRegistInput.aspx.cs" Inherits="Form_OrderRegist_OrderRegistInput" MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc" TagName="FieldMemoSetting" Src="~/Form/Common/FieldMemoSetting/BodyFieldMemoSetting.ascx" %>
<%@ Register Src="~/Form/Common/CreditToken.ascx" TagPrefix="uc" TagName="CreditToken" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<%@ Import Namespace="System.Web.UI.HtmlControls" %>
<%@ Import Namespace="w2.App.Common.Input.Order" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Product" %>
<%@ Import Namespace="w2.App.Common.User" %>
<%@ Import Namespace="w2.App.Common.Order.OrderCombine" %>
<%@ Import Namespace="w2.App.Common.Order.FixedPurchaseCombine" %>
<%@ Import Namespace="w2.Domain.Order" %>
<%@ Import Namespace="w2.Domain.FixedPurchase" %>
<%@ Import Namespace="w2.Domain.FixedPurchase.Helper" %>
<%@ Import Namespace="w2.Domain.UserShipping" %>
<%@ Import Namespace="w2.Domain.Payment" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Import Namespace="w2.App.Common.Option" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<style>
		.scroll-vertical
		{
			height: 400px;
			overflow-y: scroll;
			overflow-x: hidden;
	}

		.scroll-vertical-autocomplete
		{
			max-height: 400px;
			overflow-y: scroll;
			overflow-x: hidden;
		}

		.scroll-vertical::-webkit-scrollbar,
			.scroll-vertical-autocomplete::-webkit-scrollbar
			{
			width: 3px;
	}

		.scroll-vertical::-webkit-scrollbar-track,
			.scroll-vertical-autocomplete::-webkit-scrollbar-track
			{
			-webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3);
	}

		.scroll-vertical::-webkit-scrollbar-thumb,
			.scroll-vertical-autocomplete::-webkit-scrollbar-thumb
			{
			-webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.5);
}

		.product-option div
		{
			border-bottom: 1px solid #bababa;
			line-height: 1;
			width: 101%;
			word-break: break-all;
			white-space: normal;
			padding-top: 2px;
			padding-bottom: 2px;
}

			.product-option div span:first-child
			{
			width: 15%;
		}

			.product-option div:last-child
			{
			border-bottom: none !important;
		}

			.product-option div span label
			{
			padding-left: 2px;
			padding-right: 5px;
		}

		.messege-re-order
		{
			text-align: right;
			padding-top: 50px;
	}

		.btn_help
		{
			top: -4px;
	}

		.btn_pagetop
		{
			bottom: 90px;
}

		.no-discount
		{
			border: none;
			background-color: #fff;
}

		.block-order-regist-input-section-error
		{
			text-align: left;
}

		.popup_body
		{
			padding-top: initial;
}

		.block-order-regist-input-section-contents
		{
			position: relative;
			text-align: -webkit-left;
		}

		.autocomplete
		{
			position: relative;
			display: inline;
		}

		.autocomplete-header
		{
			position: absolute;
			z-index: 3;
			border: 1px solid #4b4b4b;
		}

			.autocomplete-header table th
			{
			padding-left: 5px;
			font-weight: normal;
			background: #e1e1e1;
			text-align: left;
			border-bottom: 1px solid #333333;
		}

		.autocomplete-items
		{
			position: absolute;
			z-index: 3;
			width: max-content;
			border: 1px solid #4b4b4b;
			word-break: break-all;
		}

			.autocomplete-items table
			{
			padding: 10px;
			cursor: pointer;
			background-color: #fff;
			border-bottom: 1px solid #d4d4d4;
		}

				.autocomplete-items table td
				{
			padding-left: 5px;
		}

				.autocomplete-items table:hover
				{
			background-color: #e9e9e9;
		}

		.autocomplete-active
		{
			background-color: DodgerBlue !important;
			color: #ffffff;
		}

		.main-contents-inner
		{
			padding-bottom: 100px;
		}

		.sidemenu_slim .main-contents
		{
			margin-left: 30px;
			-webkit-transition: 0.3s;
			-o-transition: 0.3s;
			transition: 0.3s;
		}

		.form-element-group-content select
		{
			max-width: 15em;
		}

		.block-order-regist-input-section .product-list .resize img
		{
			width: 35px;
		}

		.form-element-group-title,
		.form-element-group-content,
		.form-element-group-content label
		{
			word-break: break-all;
			white-space: normal;
			padding-right: 5px;
		}

		.block-order-regist-input-section.memo textarea,
		.form-element-group .form-element-group-content textarea
		{
			min-width: 100%;
			min-height: 50px;
		}

		.input-number
		{
			text-align: right;
		}

		.item-name
		{
			word-break: break-all;
			white-space: normal;
			max-width: 15em;
		}

		.form-element-group-list
		{
			border: none !important;
			border-radius: 0px !important;
			background: none !important;
		}

		table
		{
			table-layout: unset !important;
		}

		.notice
		{
			font-size: 14px;
		}
		div[id$=tbProductIdNoneAutocompleteHeader] th,
		div[id$=tbProductIdNoneAutocompleteItems] td,
		div[id$=tbProductIdAutocompleteHeader] th,
		div[id$=tbProductIdAutocompleteItems] td
		{
			border: none!important;
		}

		.suggestHeader {
			cursor:default;
		}
		<%--
		.copyright {
			margin-bottom: 70px;
		}
		--%>
	</style>
	<asp:Button ID="btnTooltipInfo" runat="server" Style="display: none;" />
	<asp:Button ID="btnProductOptionValueSetting" Style="display: none" OnClick="btnProductOptionValueSetting_OnClick" runat="server" />
	<asp:HiddenField ID="hfProductItemSelectIndex" runat="server" />
	<asp:HiddenField ID="hfPaidyTokenId" runat="server" />
	<div class="main-contents page-order-regist">
		<div class="messege-re-order" id="dvReOrderWarning" runat="server">
			<span>【再注文】をご使用の場合、注文者情報や商品情報等が元の注文情報から取り入れされます。<br />
				しかし、商品価格の変更等により、元の注文情報と差異が出る可能性がありますのでご注意ください。
			</span>
		</div>
		<h1 class="page-title" style="text-align: left; font-size: 20px;">新規注文登録</h1>
		<div class="block-order-regist-input">
			<div class="block-order-regist-input-block1">
				<!-- ▽注文区分▽ -->
				<div class="block-order-regist-input-section order-info">
					<div id="dvAdvCodeErrorMessage" class="block-order-regist-input-section-error" visible="false" runat="server">
						<asp:Literal ID="lAdvCodeErrorMessages" runat="server" />
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								注文区分
							</div>
							<div class="form-element-group-content">
								<asp:DropDownList ID="ddlOrderKbn" runat="server" />
							</div>
						</div>
						<!-- ▽広告コード情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								広告コード
							</div>
							<div class="form-element-group-content">
								<div class="keyword-search">
									<div class="autocomplete">
										<asp:TextBox class="keyword-search-input w100" ID="tbAdvCode" placeholder="広告コード/媒体名で検索" MaxLength="30" runat="server" />
									</div>
									<a class="keyword-search-submit inner-btn"
										href="javascript:open_advcode_list('set_advcode_from_list','width=850,height=600,top=110,left=380,status=NO,scrollbars=yes')">
										<span class="icon-search" />
									</a>
								</div>
								<div class="media-name">
									<div class="media-name-title">
										媒体名
									</div>
									<div class="media-name-contents">
										<span style="word-break: break-all; max-width: 70%;">
											<asp:Label ID="lbAdvName" runat="server" />
											<asp:HiddenField ID="hfAdvName" runat="server" />
										</span>
										<asp:LinkButton ID="lbClearAdvCode" class="btn-clear" runat="server" Text="クリア" OnClick="lbClearAdvCode_Click" Style="display: none;" />
									</div>
								</div>
							</div>
						</div>
						<!-- △広告コード情報△ -->
					</div>
				</div>
				<!-- △注文区分△ -->
				<!-- ▽注文者情報▽ -->
				<div class="block-order-regist-input-section user-info">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-user"></span>
							<span class="block-order-regist-input-section-title-label">注文者情報</span>
						</h2>
						<div class="block-order-regist-input-section-header-right" id="dvSearchUser" runat="server">
							<div class="keyword-search">
								<div class="autocomplete">
									<asp:TextBox ID="tbUserOrderSearch" autocomplete="new-password" runat="server" class="keyword-search-input" placeholder="ID、名前等で検索" />
								</div>
								<a href="javascript:open_user_list('<%: (Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_USER_LIST + "?") %>','user_search','width=850,height=610,top=120,left=320,status=NO,scrollbars=yes');" class="keyword-search-submit inner-btn">
									<span class="icon-search" />
								</a>
							</div>
						</div>
					</div>
					<div class="block-order-regist-input-section-error" runat="server" visible="false" id="dvOrderOwnerErrorMessages">
						<asp:Literal ID="lOrderOwnerErrorMessages" runat="server" />
					</div>
					<div class="tabs">
						<% if (string.IsNullOrEmpty(lUserId.Text) == false)
						   { %>
													<%-- ▽再注文情報設定用(非表示)▽ --%>
						<p style="display: none">
							<asp:Button ID="btnSetReOrderData" Text="  再注文情報設定  " runat="server" OnClick="btnSetReOrderData_Click" />
						</p>
													<asp:HiddenField ID="hfReOrderId" runat="server" />
						<% } %>
													<%-- △再注文情報設定用(非表示)△ --%>
						<div class="tabs-tab">
							<a href="javascript:void(0)" class="tab is-active" data-tab-content-selector="#tab-content-1">基本情報</a>
												</div>
						<% if (string.IsNullOrEmpty(lUserId.Text) == false)
						   { %>
						<div class="tabs-tab">
							<a href="javascript:void(0)" class="tab" data-tab-content-selector="#tab-content-2">購入履歴(<asp:Literal ID="lOrderAndFixPurchaseHistoryCount" Text="0" runat="server" />件)
								<% if (lCount.Text != "0")
								   { %>
								<span class="caution">
									<asp:Literal ID="lCount" Text="0" runat="server" />
								</span>
								<% } %>
							</a>
						</div>
						<div class="tabs-tab" runat="server">
							<a href="javascript:void(0)" class="tab" data-tab-content-selector="#tab-content-3">ユーザー属性</a>
						</div>
						<% } %>
					</div>
					<div class="tab-contents">
						<div class="tab-content" id="tab-content-1" style="display: block;">
							<div class="tab-content-contents">
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										ユーザーID
									</div>
									<div class="form-element-group-content" runat="server">
										<a href="javascript:open_window('<%: (Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CONFIRM_POPUP + "?user_id=" + hfUserId.Value +"&action_status=detail&reorder_flg=1&window_kbn=1") %>','usercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');">
											<asp:Literal ID="lUserId" runat="server" />
										</a>
										<asp:LinkButton ID="lbUserClear" runat="server" OnClick="lbUserClear_Click" class="btn btn-sub btn-size-s" Text="  クリア  " Visible="false" />
										<asp:Literal ID="lUserIdNonSet" runat="server" />
										<asp:HiddenField ID="hfUserId" runat="server" />
										<asp:HiddenField ID="hfNewUserId" runat="server" />
										<asp:HiddenField ID="hfFixedPurchaseMember" runat="server" />
															<% if ((string.IsNullOrEmpty(hfUserId.Value) == false) && this.IsAvailableRank) { %>
										<br />
										（会員ランク：<asp:Literal ID="lMemberRankName" runat="server" />）
															<%} %>
										<asp:HiddenField ID="hfMemberRankId" runat="server" />
															<%-- ▽ユーザー情報設定用(非表示)▽ --%>
										<p style="display: none">
											<asp:Button ID="btnGetUserData" Text="  ユーザー情報取得  " runat="server" OnClick="btnGetUserData_Click" />
										</p>
															<%-- △ユーザー情報設定用(非表示)△ --%>
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.name.name@@") %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerName1" class="w40 neccessary" runat="server" MaxLength="10" />
										<asp:TextBox ID="tbOwnerName2" class="w40 neccessary" runat="server" MaxLength="10" />
									</div>
								</div>
								<% if (this.IsOwnerAddrJp)
														   { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.name_kana.name@@") %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerNameKana1" runat="server" class="w40 neccessary" MaxLength="20" />
										<asp:TextBox ID="tbOwnerNameKana2" runat="server" class="w40 neccessary" MaxLength="20" />
									</div>
								</div>
														<% } %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										注文者区分<span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList ID="ddlOwnerKbn" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlOwnerKbn_SelectedIndexChanged" />
										<asp:Literal ID="lOwnerKbn" runat="server" />
										<asp:HiddenField ID="hfOwnerKbn" Value="OFF_USER" runat="server" />
										<p>
											<asp:Label ID="lbSelectPaymentOrderOwnerKbnMessage" runat="server" ForeColor="red" />
										</p>
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.tel1.name@@", this.OwnerAddrCountryIsoCode) %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<% if (this.IsOwnerAddrJp)
															   { %>
										<asp:TextBox ID="tbOwnerTel1_1" runat="server" MaxLength="6" class="w30 neccessary" />
										-
										<asp:TextBox ID="tbOwnerTel1_2" runat="server" MaxLength="4" class="w28 neccessary" />
										-
										<asp:TextBox ID="tbOwnerTel1_3" runat="server" MaxLength="4" class="w28 neccessary" />
										<% }
															   else
															   { %>
										<asp:TextBox ID="tbOwnerTel1Global" runat="server" MaxLength="30" class="w100 neccessary" />
															<% } %>
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
															<%: ReplaceTag("@@User.tel2.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<% if (string.IsNullOrEmpty(hfUserId.Value))
														   { %>
									<div class="form-element-group-content">
										<% if (this.IsOwnerAddrJp)
															   { %>
										<asp:TextBox ID="tbOwnerTel2_1" runat="server" MaxLength="6" class="w30" />
										-
										<asp:TextBox ID="tbOwnerTel2_2" runat="server" MaxLength="4" class="w28" />
										-
										<asp:TextBox ID="tbOwnerTel2_3" runat="server" MaxLength="4" class="w28" />
										<% }
															   else
															   { %>
										<asp:TextBox ID="tbOwnerTel2Global" runat="server" MaxLength="30" class="w100" />
															<% } %>
									</div>
									<% }
														   else
														   { %>
									<div class="form-element-group-content">
										<asp:Literal ID="lOwnerTel2" runat="server" />
									</div>
														<% } %>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										メールアドレス
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerMailAddr" runat="server" MaxLength="256" class="w100" />
									</div>
								</div>
								<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: StringUtility.ToHankaku("モバイルメールアドレス") %>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerMailAddr2" runat="server" MaxLength="256" class="w100" />
									</div>
								</div>
													<% } %>
								<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.country.name@@", this.OwnerAddrCountryIsoCode) %>
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList runat="server" ID="ddlOwnerCountry" AutoPostBack="true" Width="55%" />
									</div>
								</div>
													<% } %>
								<% if (this.IsOwnerAddrJp)
													   { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.zip.name@@") %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerZip1" runat="server" MaxLength="3" class="w20 neccessary" />
										-
										<asp:TextBox ID="tbOwnerZip2" runat="server" MaxLength="4" class="w30 neccessary" AutoPostBack="true" OnTextChanged="tbOwnerZip2_OnTextChanged" />
										<asp:Button ID="btnOwnerZipSearch" Text="  住所検索  " runat="server" OnClick="btnOwnerZipSearch_Click" class="btn btn-sub btn-size-s" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.addr1.name@@") %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList ID="ddlOwnerAddr1" runat="server" class="neccessary" AutoPostBack="true" OnSelectedIndexChanged="ddlOwnerAddr1_OnSelectedIndexChanged" />
									</div>
								</div>
													<% } %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.addr2.name@@", this.OwnerAddrCountryIsoCode) %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerAddr2" runat="server" MaxLength="40" class="w100 neccessary" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
															<%: ReplaceTag("@@User.addr3.name@@", this.OwnerAddrCountryIsoCode) %>
										<% if (this.IsOwnerAddrJp)
										   { %>
										<span class="notice">*</span>
										<% } %>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerAddr3" runat="server" MaxLength="50" class="w100 neccessary" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid" <%= (Constants.DISPLAY_ADDR4_ENABLED ? string.Empty : "style=\"display:none;\"") %>>
									<div class="form-element-group-title">
										<%: StringUtility.ToHankaku(ReplaceTag("@@User.addr4.name@@", this.OwnerAddrCountryIsoCode)) %>
										<% if (this.IsOwnerAddrJp == false)
													   { %>
										<span class="notice">*</span>
										<% } %>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerAddr4" runat="server" MaxLength="50" class="w100" />
									</div>
								</div>
								<% if (this.IsOwnerAddrJp == false)
								   { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
															<%: ReplaceTag("@@User.addr5.name@@", this.OwnerAddrCountryIsoCode) %>
										<% if (this.IsOwnerAddrUs)
															   { %>
										<span class="notice">*</span>
										<% } %>
									</div>
									<div class="form-element-group-content">
										<% if (this.IsOwnerAddrUs)
										   { %>
										<asp:DropDownList runat="server" ID="ddlOwnerAddr5" />
										<% }
										   else
										   { %>
										<asp:TextBox runat="server" ID="tbOwnerAddr5" class="w100" />
															<% } %>
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
															<%: ReplaceTag("@@User.zip.name@@", this.OwnerAddrCountryIsoCode) %>
										<% if (this.IsOwnerAddrZipNecessary)
															   { %>
															<span class="notice">*</span>
															<% } %>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerZipGlobal" runat="server" MaxLength="20" class="w100 neccessary" />
															<asp:LinkButton
																ID="lbSearchAddrFromOwnerZipGlobal"
																OnClick="lbSearchAddrFromOwnerZipGlobal_Click"
											Style="display: none;"
																runat="server" />
									</div>
								</div>
													<% } %>
								<% if (Constants.DISPLAY_CORPORATION_ENABLED)
								   { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.company_name.name@@") %>
															<span class="notice"></span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerCompanyName" runat="server" MaxLength="50" class="w100" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.company_post_name.name@@") %>
															<span class="notice"></span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbOwnerCompanyPostName" runat="server" MaxLength="50" class="w100" />
									</div>
								</div>
								<% } %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.sex.name@@") %>
									</div>
									<div class="form-element-group-content">
										<asp:RadioButtonList ID="rblOwnerSex" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.birth.name@@") %>
									</div>
									<div class="form-element-group-content">
										<uc:DateTimeInput ID="ucOwnerBirth" runat="server" YearList="<%# DateTimeUtility.GetBirthYearListItem() %>" HasTime="False" HasBlankSign="False" HasBlankValue="True" />
									</div>
								</div>
								<% if (Constants.GLOBAL_OPTION_ENABLE)
								   { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										アクセス国ISOコード
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList ID="ddlAccessCountryIsoCode" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										表示言語ロケールID
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList class="w100" ID="ddlDispLanguageLocaleId" runat="server"
											OnSelectedIndexChanged="ddlDispLanguageLocaleId_SelectedIndexChanged"
											AutoPostBack="true"
											Width="100%" />
										<br />
										言語コード(
										<asp:Literal ID="lDispLanguageCode" runat="server" />
										)
										<asp:HiddenField ID="hfDispLanguageCode" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										表示通貨ロケールID
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList ID="ddlDispCurrencyLocaleId" runat="server"
											OnSelectedIndexChanged="ddlDispCurrencyLocaleId_SelectedIndexChanged"
											AutoPostBack="true"
											Width="100%" />
										<br />
										通貨コード(
										<asp:Literal ID="lDispCurrencyCode" runat="server" />
										)
										<asp:HiddenField ID="hfDispCurrencyCode" runat="server" />
									</div>
								</div>
								<% } %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										ユーザー特記欄
									</div>
									<div class="form-element-group-content" style="max-width: 190px">
										<asp:TextBox ID="tbUserMemo" runat="server" TextMode="MultiLine" Rows="2" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: StringUtility.ToHankaku("ユーザー管理レベル") %>
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList ID="ddlUserManagementLevel" runat="server"
											OnSelectedIndexChanged="ddlUserManagementLevel_SelectedIndexChanged"
											AutoPostBack="true"
											Width="100%" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										メール配信希望
									</div>
									<div class="form-element-group-content">
										<asp:RadioButtonList ID="rblMailFlg" Style="display: inline" runat="server" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid" id="dvSelectPaymentUserManagementLevelMessage" runat="server" visible="false">
									<div class="form-element-group-title">
										<asp:Label ID="Label1" runat="server" Text="注意喚起" ForeColor="red" />
									</div>
									<div class="form-element-group-content">
										<asp:Label ID="lbSelectPaymentUserManagementLevelMessage" runat="server" ForeColor="red" />
									</div>
								</div>
								<div id="dvMemberRank" runat="server" visible="false">
									<div class="form-element-group form-element-group-horizontal-grid" runat="server">
										<div class="form-element-group-title">
											会員ランク
										</div>
										<div class="form-element-group-content">
											<asp:Literal ID="lUserMemberRankName" runat="server" />
										</div>
									</div>
									<div class="form-element-group form-element-group-horizontal-grid" runat="server">
										<div class="form-element-group-title">
											特典
										</div>
										<div class="form-element-group-content">
											<asp:Literal ID="lUserMemberRankBenefit" runat="server" />
										</div>
									</div>
									<div id="dvMemberRankMemo" visible="false" class="form-element-group form-element-group-horizontal-grid" runat="server">
										<div class="form-element-group-title">
											ランクメモ
										</div>
										<div class="form-element-group-content">
											<asp:Literal ID="lUserMemberRankMemo" runat="server" />
										</div>
									</div>
								</div>
								<% if (string.IsNullOrEmpty(hfUserId.Value) == false)
								   { %>
								<div class="form-element-group form-element-group-horizontal user-info-save-owner">
									<asp:CheckBox ID="cbAllowSaveOwnerIntoUser" Text="注文者情報の変更をユーザー情報に反映" runat="server" />
								</div>
								<% } %>
							</div>
						</div>
						<div class="tab-content" id="tab-content-2" style="display: none;">
							<!-- ▽通常注文▽ -->
							<div class="order-history-section">
								<h3 class="order-title">通常注文</h3>
								<div id="dvHideOrderHistory" runat="server">
									<p class="order-history-section-nodata note">購入履歴はありません。</p>
								</div>
								<div runat="server" id="dvShowOrderHistory" visible="false">
									<div id="dvOrderHistoryList" runat="server">
										<table class="user-info-table order">
											<tr>
												<th colspan="2">
													<div class="order-data">
														<span class="order-data-inner">
															<span class="order-data-row">
																<span class="order-data-date">注⽂⽇時</span>
															</span>
															<span class="order-data-row">
																<span class="order-data-id">注⽂ID</span>
															</span>
														</span>
														<span class="order-data-status">
															<span class="order-data-row">
																<span class="order-data-status-order-text">注⽂S</span>/<span class="order-data-status-payment-text">⼊⾦S</span>
															</span>
															<span class="order-data-row">
																<span class="order-data-status">注文区分</span>/<span class="order-data-status">決済種別</span>
															</span>
														</span>
													</div>
												</th>
													</tr>
													<tr>
												<th class="item-name">注⽂商品</th>
												<th class="total-price">合計⾦額</th>
													</tr>
													<tr>
												<td colspan="2" class="separate"></td>
											</tr>
											<asp:Repeater ID="rOrderHistoryList" ItemType="OrderModel" runat="server" OnItemCommand="rOrderHistoryList_ItemCommand">
												<ItemTemplate>
													<tr>
														<td colspan="2">
															<div class="order-data">
																<span class="order-data-inner">
																	<span class="order-data-row">
																		<span class="order-data-date"><%#: Item.OrderDate %></span>
																	</span>
																	<span class="order-data-row">
																		<a href="javascript:open_window('<%#: CreateOrderDetailUrl(Item.OrderId, true, false) %>','ordercontact','width=1100,height=800,top=110,left=380,status=NO,scrollbars=yes,resizable=YES');" class="order-data-id">
																			<%#: Item.OrderId %>
																		</a>
																	</span>
																</span>
																<span class="order-data-status">
																	<span class="order-data-row">
																		<span class="order-data-status-order-icon <%#: GetCssClassForStatus(Item.OrderStatus, Constants.FIELD_ORDER_ORDER_STATUS) %>">
																			<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, Item.OrderStatus) %>
																		</span>
																		<span class="order-data-status-payment-icon <%#: GetCssClassForStatus(Item.OrderPaymentStatus, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS) %>">
																			<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Item.OrderPaymentStatus) %>
																		</span>
																	</span>
																	<span class="order-data-row">
																		<span class="order-data-status-payment-icon">
																			<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, Item.OrderKbn) %>
																		</span>
																		<span class="order-data-status-payment-icon" title="<%#: StringUtility.ToEmpty(Item.PaymentName) %>">
																			<%#: StringUtility.AbbreviateString(Item.PaymentName, 10) %>
																		</span>
																	</span>
																</span>
															</div>
														</td>
													</tr>
													<tr>
														<td class="item-name">
															<ul>
																<%# GetOrderItemFieldHtml(Item) %>
															</ul>
														</td>
														<td class="total-price">
															<span class="price-value"><%#: Item.OrderPriceTotal.ToPriceString(true) %></span>
														</td>
													</tr>
													<tr>
														<td colspan="2" class="reorder ta-right">
															<asp:LinkButton class="btn btn-main btn-size-s" Text="  再注文  " runat="server"
																OnClientClick="return confirm_reorder()"
																CommandName="ReOrder"
																CommandArgument="<%# Item.OrderId %>"
																Visible="<%# CanReOrder(Item) %>" />
														</td>
													</tr>
													<tr>
														<td colspan="2" class="separate"></td>
													</tr>
												</ItemTemplate>
											</asp:Repeater>
												</table>
									</div>
								</div>
							</div>
							<!-- △通常注文△ -->
							<!-- ▽定期注文▽ -->
							<div class="order-history-section">
								<h3 class="order-title">定期注文</h3>
								<div id="dvHideFixedPurchaseHistory" runat="server">
									<p class="order-history-section-nodata note">購入履歴はありません。</p>
								</div>
								<div runat="server" id="dvShowFixedPurchaseHistory" visible="false">
									<div id="dvFixedPurchaseHistoryList" runat="server">
										<table class="user-info-table order">
														<tr>
												<th colspan="2">
													<div class="order-data">
														<span class="order-data-inner">
															<span class="order-data-row">
																<span class="order-data-date">初回注⽂⽇時</span>
															</span>
															<span class="order-data-row">
																<span class="order-data-id">定期購⼊ID</span>
															</span>
														</span>
														<span class="order-data-status">
															<span class="order-data-row">
																<span class="order-data-status-order-text">定期購⼊S</span>/<span class="order-data-status-payment-text">決済S</span>
															</span>
															<span class="order-data-row">
																<span class="order-data-status">注文区分</span>/<span class="order-data-status">決済種別</span>
															</span>
														</span>
													</div>
												</th>
														</tr>
														<tr>
												<th>注⽂商品</th>
												<th class="total-price">合計⾦額</th>
														</tr>
														<tr>
												<th colspan="2" class="teiki-data">
													<div class="teiki-data-detail">
														<div>定期購⼊区分</div>
														<div>注⽂回数 / 出荷回数</div>
														<div class="last">次回配送⽇</div>
													</div>
												</th>
														</tr>
														<tr>
												<td colspan="2" class="separate"></td>
														</tr>
											<asp:Repeater ID="rFixedPurchaseHistoryList" runat="server" ItemType="UserFixedPurchaseListSearchResult" OnItemCommand="rFixedPurchaseHistoryList_ItemCommand">
												<ItemTemplate>
														<tr>
														<td colspan="2">
															<div class="order-data">
																<span class="order-data-inner">
																	<span class="order-data-row">
																		<span class="order-data-date">
																			<%#: Item.FixedPurchaseDateBgn %>
																		</span>
																	</span>
																	<span class="order-data-row">
																		<a href="javascript:open_window('<%#: FixedPurchasePage.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId, true) %>','ordercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');" class="order-data-id">
																			<%#: Item.FixedPurchaseId %>
																		</a>
																	</span>
																</span>
																<span class="order-data-status">
																	<span class="order-data-row">
																		<span class="order-data-status-order-icon <%#: GetCssClassForStatus(Item.FixedPurchaseStatus, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS) %>">
																			<%#: ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, Item.FixedPurchaseStatus) %>
																		</span>
																		<span class="order-data-status-payment-icon <%#: GetCssClassForStatus(Item.PaymentStatus, Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS) %>">
																			<%#: ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS, Item.PaymentStatus) %>
																		</span>
																	</span>
																	<span class="order-data-row">
																		<span class="order-data-status-payment-icon ">
																			<%#: ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_ORDER_KBN, Item.OrderKbn) %>
																		</span>
																		<span class="order-data-status-payment-icon">
																			<%#: ValueText.GetValueText(Constants.TABLE_PAYMENT, CONST_KEY_PAYMENT_TYPE, Item.OrderPaymentKbn) %>
																		</span>
																	</span>
																</span>
															</div>
														</td>
														</tr>
														<tr>
														<td class="item-name">
															<ul>
																<%# GetOrderItemFieldHtml(Item) %>
															</ul>
															</td>
														<td class="total-price">
															<span class="price-value"><%#: GetItemPriceTotal(Item).ToPriceString(true) %></span>
														</td>
														</tr>
														<tr>
														<td colspan="2" class="teiki-data">
															<div class="teiki-data-detail">
																<div>
																	<%#: OrderCommon.CreateFixedPurchaseSettingMessage(Item) %>
																</div>
																<div>
																	<%#: Item.OrderCount %> / <%#: Item.ShippedCount %>
																</div>
																<div class="last">
																	<%#: Item.NextShippingDate.HasValue ? Item.NextShippingDate.Value.ToString(CONST_FORMAT_SHORT_DATE) : "-" %>
																</div>
																<div class="last order-btn">
																	<asp:LinkButton class="btn btn-main btn-size-s" Text="  再注文  " runat="server"
																		OnClientClick="return confirm_reorder()"
																		CommandName="ReOrderFixedPurchase"
																		CommandArgument="<%# Item.FixedPurchaseId %>"
																		Visible="<%# CanReOrder(Item) %>" />
																</div>
															</div>
														</td>
														</tr>
														<tr>
														<td colspan="2" class="separate"></td>
														</tr>
												</ItemTemplate>
											</asp:Repeater>
										</table>
									</div>
								</div>
							</div>
							<!-- △定期注文△ -->
						</div>
						<!-- ▽ユーザー属性▽ -->
						<div class="tab-content" id="tab-content-3" style="display: none;">
							<table class="user-info-table date">
														<tr>
									<th>最終購⼊⽇</th>
									<td class="count">
										<asp:Literal ID="lLastOrderDate" runat="server" Text="-" />
									</td>
														</tr>
															<tr>
									<th>2回⽬購⼊⽇</th>
									<td class="count">
										<asp:Literal ID="lSecondOrderDate" runat="server" Text="-" />
									</td>
															</tr>
								<tr>
									<th>初回購⼊⽇</th>
									<td class="count">
										<asp:Literal ID="lFirstOrderDate" runat="server" Text="-" />
									</td>
								</tr>
													</table>
							<table class="user-info-table period">
													<tr>
									<th>離脱期間<br />
										（最終購⼊から経った期間）</th>
									<td class="count">
										<asp:Literal ID="lAwayDays" runat="server" Text="-" />
										日
														</td>
													</tr>
													<tr>
									<th>在籍期間<br />
										（初回購⼊から最終購⼊までの期間）</th>
									<td class="count">
										<asp:Literal ID="lEnrollmentDays" runat="server" Text="-" />日
														</td>
													</tr>
							</table>
							<table class="user-info-table purchase">
													<tr>
									<th colspan="3">累計購⼊⾦額</th>
								</tr>
								<tr>
									<td class="title" rowspan="2">注⽂基準</td>
									<td>全体</td>
									<td class="count">
										<asp:Literal ID="lOrderAmountOrderAll" runat="server" Text="0" />円
														</td>
								</tr>
								<tr>
									<td>定期のみ</td>
									<td class="count">
										<asp:Literal ID="lOrderAmountOrderFp" runat="server" Text="0" />円
														</td>
													</tr>
													<tr>
									<td class="title" rowspan="2">出荷基準</td>
									<td>全体</td>
									<td class="count">
										<asp:Literal ID="lOrderAmountShipAll" runat="server" Text="0" />円
														</td>
								</tr>
								<tr>
									<td>定期のみ</td>
									<td class="count">
										<asp:Literal ID="lOrderAmountShipFp" runat="server" Text="0" />円
														</td>
													</tr>
												</table>
							<table class="user-info-table purchase">
														<tr>
									<th colspan="3">累計購⼊回数</th>
														</tr>
														<tr>
									<td class="title" rowspan="2">注⽂基準</td>
									<td>全体</td>
									<td class="count">
										<asp:Literal ID="lOrderCountOrderAll" runat="server" Text="0" />回
															</td>
														</tr>
													<tr>
									<td>定期のみ</td>
									<td class="count">
										<asp:Literal ID="lOrderCountOrderFp" runat="server" Text="0" />回
									</td>
													</tr>
													<tr>
									<td class="title last" rowspan="2">出荷基準</td>
									<td>全体</td>
									<td class="count">
										<asp:Literal ID="lOrderCountShipAll" runat="server" Text="0" />回
														</td>
													</tr>
								<tr>
									<td class="last">定期のみ</td>
									<td class="count last">
										<asp:Literal ID="lOrderCountShipFp" runat="server" Text="0" />回
									</td>
								</tr>
							</table>
						</div>
						<!-- △ユーザー属性△ -->
					</div>
				</div>
				<!-- △注文者情報△ -->
				<!-- ▽配送先情報▽ -->
				<div class="block-order-regist-input-section ShippingTo" id="dvShippingTo" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-shippingto"></span>
							<span class="block-order-regist-input-section-title-label">配送先情報</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-error" runat="server" id="dvOrderShippingErrorMessages" visible="false">
						<asp:Literal ID="lOrderShippingErrorMessages" runat="server" />
					</div>
					<div class="shipping-same-as-owner">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								配送先
							</div>
							<div class="form-element-group-content">
								<asp:DropDownList id="ddlShippingKbnList" runat="server" class="js-toggle-form" AutoPostBack="true" OnSelectedIndexChanged="ddlShippingKbnList_Changed" onChange="javascript:InitializeOrderShipping();" />
							</div>
						</div>
						<div class="form-element-group-content" id="dvRealStoreList" runat="server">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									受取店舗
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList id="ddlRealStore" Class="w100" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRealStore_SelectedIndexChanged" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									店舗住所
								</div>
								<div class="form-element-group-content">
									<asp:Label ID="lbStoreAddress" runat="server" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									営業時間
								</div>
								<div class="form-element-group-content">
									<asp:Label ID="lbStoreOpeningHours" runat="server" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									店舗電話番号
								</div>
								<div class="form-element-group-content">
									<asp:Label ID="lbStoreTel" runat="server" />
								</div>
							</div>
						</div>
						<span runat="server" id="spShippingSameNgMsg" class="notice" visible="False">
							<br />
							注文者が配送可能な国ではないため同じにはできません</span>
					</div>
					<div id="dvShippingList" runat="server" class="block-order-regist-input-section-contents shipping-same-as-owner-contents">
						<div class="form-element-group form-element-group-horizontal-grid address-info-selector" runat="server" id="dvUserShipping">
							<div class="form-element-group-title">
								アドレス帳
							</div>
							<div class="form-element-group-content" id="dvUserShippingList" runat="server">
																<asp:DropDownList ID="ddlUserShipping" CssClass="UserShipping" runat="server" OnSelectedIndexChanged="ddlUserShipping_SelectedIndexChanged">
									<asp:ListItem Value="NEW" Text="配送先入力" />
																</asp:DropDownList>
																<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
																<asp:DropDownList
																	ID="ddlShippingReceivingStoreType"
																	CssClass="UserShipping"
																	DataTextField="Text"
																	DataValueField="Value"
																	runat="server"
																	AutoPostBack="true"
																	OnSelectedIndexChanged="ddlShippingReceivingStoreType_SelectedIndexChanged" />
								<asp:Button CssClass="btnCvsSearch btn btn-size-s" ID="btnOpenConvenienceStoreMapEcPay" runat="server" OnClick="btnOpenConvenienceStoreMapEcPay_Click" Text="  検索  " />
																<% } else { %>
								<input class="btnCvsSearch btn btn-size-s" onclick="javascript:openConvenienceStoreMapPopup();" type="button" value="  検索  " />
																<% } %>
								<asp:Button ID="btnRefreshPayment" Style="display: none;" runat="server" OnClick="btnRefreshPayment_Click" />
							</div>
						</div>
						<div class="tbShippingItemNew shippingItem NEW">
							<div id="dvOrderShipping" runat="server">
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.name.name@@") %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingName1" runat="server" MaxLength="10" class="w40 neccessary" />
										<asp:TextBox ID="tbShippingName2" runat="server" MaxLength="10" class="w40 neccessary" />
									</div>
								</div>
															<% if (this.IsShippingAddrJp) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.name_kana.name@@") %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingNameKana1" runat="server" MaxLength="20" class="w40 neccessary" />
										<asp:TextBox ID="tbShippingNameKana2" runat="server" MaxLength="20" class="w40 neccessary" />
									</div>
								</div>
															<% } %>
														<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.country.name@@", this.ShippingAddrCountryIsoCode) %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList ID="ddlShippingCountry" runat="server" AutoPostBack="true" Width="55%" />
									</div>
								</div>
														<% } %>
														<% if (this.IsShippingAddrJp) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.zip.name@@") %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingZip1" runat="server" MaxLength="3" class="w20 neccessary" />
										-
										<asp:TextBox ID="tbShippingZip2" runat="server" MaxLength="4" class="w30 neccessary"  AutoPostBack="true" OnTextChanged="tbShippingZip2_OnTextChanged" />
										<asp:Button ID="btnShippingZipSearch" Text="  住所検索  " runat="server" OnClick="btnShippingZipSearch_Click" class="btn btn-sub btn-size-s" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.addr1.name@@") %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:DropDownList ID="ddlShippingAddr1" runat="server" class="neccessary"  AutoPostBack="true" OnSelectedIndexChanged="ddlShippingAddr1_OnSelectedIndexChanged" />
									</div>
								</div>
														<% } %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.addr2.name@@", this.ShippingAddrCountryIsoCode) %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingAddr2" runat="server" MaxLength="40" class="w100 neccessary" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.addr3.name@@", this.ShippingAddrCountryIsoCode) %>
														<% if (this.IsShippingAddrJp) { %>
										<span class="notice">*</span>
														<% } %>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingAddr3" runat="server" MaxLength="40" class="w100 neccessary"></asp:TextBox>
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title" <%= (Constants.DISPLAY_ADDR4_ENABLED ? string.Empty : "style=\"display:none;\"") %>>
										<%: StringUtility.ToHankaku(ReplaceTag("@@User.addr4.name@@", this.ShippingAddrCountryIsoCode)) %>
										<% if (this.IsShippingAddrJp == false) { %>
																<span class="notice">*</span>
										<% } %>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingAddr4" runat="server" MaxLength="40" class="w100" />
									</div>
								</div>
														<% if (this.IsShippingAddrJp == false) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
																<%: ReplaceTag("@@User.addr5.name@@", this.ShippingAddrCountryIsoCode) %>
																<% if (this.IsShippingAddrUs) { %><span class="notice">*</span><% } %>
									</div>
									<div class="form-element-group-content">
																<% if (this.IsShippingAddrUs) { %>
										<asp:DropDownList ID="ddlShippingAddr5" runat="server" />
																<% } else { %>
										<asp:TextBox ID="tbShippingAddr5" runat="server" MaxLength="40" class="w100"></asp:TextBox>
																<% } %>
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
																<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %>
										<% if (this.IsShippingAddrZipNecessary) { %>
										<span class="notice">*</span>
										<% } %>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingZipGlobal" runat="server" MaxLength="20" class="w100 neccessary" />
																<asp:LinkButton
																	ID="lbSearchAddrFromShippingZipGlobal"
																	OnClick="lbSearchAddrFromShippingZipGlobal_Click"
											Style="display: none;"
																	runat="server" />
									</div>
								</div>
														<% } %>
								<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
																<%: ReplaceTag("@@User.company_name.name@@") %>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingCompanyName" runat="server" MaxLength="40" class="w100" />
									</div>
								</div>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
																<%: ReplaceTag("@@User.company_post_name.name@@") %>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingCompanyPostName" runat="server" MaxLength="40" class="w100" />
									</div>
								</div>
														<%} %>
															<% if (this.IsShippingAddrJp) { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.tel1.name@@") %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingTel1_1" runat="server" MaxLength="6" class="w30 neccessary" />
										-
									<asp:TextBox ID="tbShippingTel1_2" runat="server" MaxLength="4" class="w28 neccessary" />
										-
									<asp:TextBox ID="tbShippingTel1_3" runat="server" MaxLength="4" class="w28 neccessary" />
									</div>
								</div>
															<% } else { %>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%: ReplaceTag("@@User.tel1.name@@", this.ShippingAddrCountryIsoCode) %><span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingTel1" runat="server" MaxLength="30" class="w100 neccessary" />
									</div>
								</div>
															<% } %>
								<div class="form-element-group form-element-group-horizontal-grid user-info-save-owner" id="dvCheckBoxSaveNewAddress" runat="server">
									<asp:CheckBox ID="cbAlowSaveNewAddress" Text=" 配送先情報を保存する" runat="server" />
								</div>
								<div class="form-element-group form-element-group-horizontal-grid" id="dvShippingName" style="display: none">
									<div class="form-element-group-title">
										配送先名<span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbShippingName" runat="server" class="w100 neccessary" />
									</div>
								</div>
							</div>
						</div>
						<div class="tbConvenienceStore CONVENIENCE_STORE" runat="server">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
															店舗ID<span class="notice">*</span><asp:HiddenField ID="hfDefineShippingConvenienceStore" runat="server" />
								</div>
								<div class="form-element-group-content" id="tdCvsShopNo">
															<span>
																<asp:Literal ID="lCvsShopNo" runat="server" />
															</span>
															<asp:HiddenField ID="hfCvsShopNo" runat="server" />
															<asp:HiddenField ID="hfUserShipping" runat="server" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
															店舗名<span class="notice">*</span>
								</div>
								<div class="form-element-group-content" id="tdCvsShopName">
															<span>
																<asp:Literal ID="lCvsShopName" runat="server" />
															</span>
															<asp:HiddenField ID="hfCvsShopName" runat="server" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
															店舗住所<span class="notice">*</span>
								</div>
								<div class="form-element-group-content" id="tdCvsShopAddress">
															<span>
																<asp:Literal ID="lCvsShopAddress" runat="server" />
															</span>
															<asp:HiddenField ID="hfCvsShopAddress" runat="server" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									店舗電話番号<span class="notice">*</span>
								</div>
								<div class="form-element-group-content" id="tdCvsShopTel">
															<span>
																<asp:Literal ID="lCvsShopTel" runat="server" />
															</span>
															<asp:HiddenField ID="hfCvsShopTel" runat="server" />
								</div>
							</div>
						</div>
						<asp:Repeater ID="rpAddressBook" runat="server" ItemType="UserShippingModel">
							<ItemTemplate>
								<div id="tbShippingItem_<%#: Item.ShippingNo %>" class="shippingItem <%#: Item.ShippingNo %>">
									<div <%# ((Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF) ? string.Empty : "style=\"display:none;\"") %>>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																		<%: ReplaceTag("@@User.name.name@@") %>
											</div>
											<div class="form-element-group-content" runat="server" visible="<%# IsCountryJp(Item.ShippingCountryIsoCode) %>">
												<%#: Item.ShippingName %> （<%#: Item.ShippingNameKana %>）
											</div>
											<div class="form-element-group-content" runat="server" visible="<%# (IsCountryJp(Item.ShippingCountryIsoCode) == false) %>">
																		<%#: Item.ShippingName %>
											</div>
										</div>
																<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																			<%#: ReplaceTag("@@User.country.name@@", Item.ShippingCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
																			<%#: Item.ShippingCountryName %>
											</div>
										</div>
																<% } %>
										<div visible="<%# IsCountryJp(Item.ShippingCountryIsoCode) %>" runat="server" class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																		<%: ReplaceTag("@@User.zip.name@@") %>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingZip %>
											</div>
										</div>
										<div visible="<%# IsCountryJp(Item.ShippingCountryIsoCode) %>" runat="server" class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																		<%: ReplaceTag("@@User.addr1.name@@") %>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingAddr1 %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																		<%#: ReplaceTag("@@User.addr2.name@@", Item.ShippingCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingAddr2 %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																		<%#: ReplaceTag("@@User.addr3.name@@", Item.ShippingCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingAddr3 %>
											</div>
										</div>
										<div <%= (Constants.DISPLAY_ADDR4_ENABLED ? string.Empty : "style=\"display:none;\"") %> class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%#: StringUtility.ToHankaku(ReplaceTag("@@User.addr4.name@@", Item.ShippingCountryIsoCode)) %>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingAddr4 %>
											</div>
										</div>
										<div visible="<%# (IsCountryJp(Item.ShippingCountryIsoCode) == false) %>" runat="server" class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<%#: ReplaceTag("@@User.addr5.name@@", this.ShippingAddrCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingAddr5 %>
											</div>
										</div>
										<div visible="<%# (IsCountryJp(Item.ShippingCountryIsoCode) == false) %>" runat="server" class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																		<%: ReplaceTag("@@User.zip.name@@", this.ShippingAddrCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingZip %>
											</div>
										</div>
										<% if (Constants.DISPLAY_CORPORATION_ENABLED) { %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																		<%: ReplaceTag("@@User.company_name.name@@")%>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingCompanyName %>
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																		<%: ReplaceTag("@@User.company_post_name.name@@")%>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingCompanyPostName %>
											</div>
										</div>
																<%} %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
																		<%#: ReplaceTag("@@User.tel1.name@@", Item.ShippingCountryIsoCode) %>
											</div>
											<div class="form-element-group-content">
																		<%#: Item.ShippingTel1 %>
											</div>
										</div>
									</div>
									<div <%# ((Item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON) ? string.Empty : "style=\"display:none;\"") %>>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												店舗ID
																</div>
											<div class="form-element-group-content">
																			<span>
													<asp:Literal ID="lCvsShopNo" Text="<%#: Item.ShippingReceivingStoreId %>" runat="server" />
																			</span>
																			<asp:HiddenField ID="hfCvsShopNo" Value="<%# Item.ShippingReceivingStoreId %>" runat="server" />
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												店舗名
											</div>
											<div class="form-element-group-content">
																			<span>
													<asp:Literal ID="lCvsShopName" Text="<%#: Item.ShippingName %>" runat="server" />
																			</span>
																			<asp:HiddenField ID="hfCvsShopName" Value="<%# Item.ShippingName %>" runat="server" />
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												店舗住所
											</div>
											<div class="form-element-group-content">
																			<span>
													<asp:Literal ID="lCvsShopAddress" Text="<%#: Item.ShippingAddr4 %>" runat="server" />
																			</span>
																			<asp:HiddenField ID="hfCvsShopAddress" Value="<%# Item.ShippingAddr4 %>" runat="server" />
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												店舗電話番号
											</div>
											<div class="form-element-group-content">
																			<span>
													<asp:Literal ID="lCvsShopTel" Text="<%#: Item.ShippingTel1 %>" runat="server" />
																			</span>
																			<asp:HiddenField ID="hfCvsShopTel" Value="<%# Item.ShippingTel1 %>" runat="server" />
											</div>
										</div>
									</div>
																</div>
													</ItemTemplate>
												</asp:Repeater>
												</div>
				</div>
												<!-- △配送先情報△ -->
			</div>
			<div class="block-order-regist-input-block2">
												<!-- ▽商品情報▽ -->
				<div class="block-order-regist-input-section product-data">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-goods"></span>
							<span class="block-order-regist-input-section-title-label">商品情報</span>
						</h2>
						<div class="block-order-regist-input-section-header-right">
							<span class="add-row">
								<asp:TextBox ID="tbAddRow" runat="server" class="add-row-input" MaxLength="2" />
								<asp:Button ID="btnAddProduct" class="add-row-btn btn btn-sub btn-size-s" runat="server" Text="  追加  " OnClick="btnAddProduct_Click" />
								<asp:Button ID="btnReCalculate" class="btn btn-main btn-size-s" runat="server" Text="  再計算  " OnClick="btnReCalculate_Click" Style="outline: none" />
							</span>
						</div>
					</div>
					<div id="dvOrderItemErrorMessage" class="block-order-regist-input-section-error" visible="false" runat="server">
						<asp:Literal ID="lOrderItemErrorMessages" runat="server" />
					</div>
					<div id="dvOrderItemProductOptionErrorMessage" class="block-order-regist-input-section-error" visible="false" runat="server">
						<asp:Literal ID="lProductOptionErrorMessage" runat="server" />
					</div>
					<div id="dvSubscriptionBoxOrderItemErrorMessage" class="block-order-regist-input-section-error" visible="false" runat="server">
						<asp:Literal ID="lSubscriptionBoxOrderItemErrorMessages" runat="server" />
					</div>
					<!-- ▽頒布会▽ -->
					<% if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED) { %>
					<table class="edit_table" cellspacing="1" cellpadding="4" width="758" border="0">
						<tbody>
							<tr>
								<td class="edit_title_bg" align="left" width="15%" style="border:0px; border-radius: 0px; background-color: #f8f8f8 !important;">頒布会コース選択</td>
								<td class="block-order-regist-input-section" align="left" width="75%" style="border:0px; border-radius: 0px; background-color:#f8f8f8;">
									<asp:DropDownList ID="ddlSubscriptionBox" OnSelectedIndexChanged="ddlSubscriptionBox_OnSelectedIndexChanged" AutoPostBack="True" Width="600px" runat="server" />
								</td>
							</tr>
						</tbody>
					</table>
					<% } %>
					<!-- △頒布会△ -->
					<% if (dvWarningMessage.Visible || dvOrderItemNoticeMessage.Visible)
					   { %>
					<div class="block-order-regist-input-section-error" runat="server">
						注意喚起<br />
						<div id="dvWarningMessage" visible="false" runat="server">
							<asp:Literal ID="lWarningMessage" runat="server" />
						</div>
						<%-- 定期購入オプションが有効の場合 --%>
						<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
						<div id="dvOrderItemNoticeMessage" visible="false" runat="server">
							<asp:Literal ID="lOrderItemNoticeMessage" runat="server" />
						</div>
						<% } %>
					</div>
					<% } %>
					<div class="product-list">
						<table class="product-list-table">
							<tr>
								<th class="thum" rowspan="3">商品<br />
									画像</th>
								<th class="product-id">商品ID</th>
								<th class="variation-id">バリエーションID</th>
								<%-- ▽ 定期OPが有効の場合 ▽--%>
								<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
								   { %>
								<th class="fixed-purchase" style="width: 10%">定期</th>
								<% } %>
								<%-- ▽ セールOPが有効の場合 ▽--%>
								<% if (Constants.PRODUCT_SALE_OPTION_ENABLED)
								   { %>
								<th class="product-sale-id">セールID</th>
								<% } %>
								<%-- △ セールOPが有効の場合 △--%>
								<% if (HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
								<th class="product-price" rowspan="3">単価</th>
								<% } %>
								<th class="item-quantity" rowspan="3">数量</th>
								<% if (HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
								<th class="tax" rowspan="3">税率</th>
								<th class="item-price" rowspan="3">小計</th>
								<% } %>
								<th class="delete" rowspan="3">削除</th>
							</tr>
							<tr>
								<th class="product-name" colspan="<%= this.ProductColSpanNumber %>">商品名</th>
							</tr>
							<tr>
								<th class="option" colspan="<%= this.ProductColSpanNumber %>">付帯情報</th>
							</tr>
							<tr>
								<td colspan="<%= HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 7 : 10 %>" class="separate"></td>
							</tr>
							<%-- ▽商品情報詳細▽ --%>
							<asp:Repeater ID="rItemList" runat="server" OnItemCommand="rItemList_ItemCommand" OnItemDataBound="rItemList_ItemDataBound">
								<ItemTemplate>
									<tbody id="default-product-input-<%#: Container.ItemIndex %>" <%# ((IsSelectedProduct(GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_PRODUCT_ID)) == false) ? string.Empty : "style='display: none'") %>>
													<tr>
											<td class="thum" rowspan="2"></td>
											<td class="edit">
												<div class="keyword-search">
													<asp:TextBox ID="tbProductIdNone" MaxLength="200" runat="server" Text="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_PRODUCT_ID) %>" class="keyword-search-input product-id" placeholder="商品を検索" />
													<asp:HiddenField ID="hfProductIdNoneSelectedIndex" Value="<%# Container.ItemIndex %>" runat="server" />
													<a href="javascript:open_product_list('<%#: CreateProductSearchUrl() %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%# Container.ItemIndex %>');" class="keyword-search-submit">
														<span class="icon-search" />
													</a>
												</div>
														</td>
											<td></td>
														<%--▽ 定期OPが有効の場合 ▽--%>
											<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
											<td></td>
											<% } %>
											<%-- △ 定期OPが有効の場合 △--%>
														<%--▽ セールOPが有効の場合 ▽--%>
											<% if (Constants.PRODUCT_SALE_OPTION_ENABLED){ %>
											<td></td>
											<% } %>
														<%--△ セールOPが有効の場合 △--%>
											<% if (HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
											<td rowspan="2"></td>
											<% } %>
											<td rowspan="2"></td>
											<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
											<% if (HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
											<td rowspan="2"></td>
											<td class="ta-right" rowspan="2"></td>
											<% } %>
											<td class="ta-center" rowspan="2"></td>
											<% } %>
										</tr>
										<tr>
											<td colspan="4"></td>
													</tr>
									</tbody>
									<tbody id="product-input-<%#: Container.ItemIndex %>" <%# (IsSelectedProduct(GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_PRODUCT_ID)) ? string.Empty : "style='display: none'") %>>
													<tr>
											<td class="thum" rowspan="3">
												<%# ProductImage.GetHtmlImageTag(Container.DataItem, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_S)%>
												<asp:HiddenField ID="hfProductImageHead" Value='<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_IMAGE_HEAD) %>' runat="server" />
											</td>
											<td class="edit">
												<div class="keyword-search">
													<asp:TextBox ID="tbProductId" MaxLength="30" runat="server" Text="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_PRODUCT_ID) %>" class="keyword-search-input product-id" placeholder="商品を検索" />
													<asp:HiddenField ID="hfProductIdForOptionSetting" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_PRODUCT_ID) %>" runat="server" />
													<asp:HiddenField ID="hfSupplierId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_SUPPLIER_ID) %>" runat="server" />
													<asp:HiddenField ID="hfNoveltyId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_NOVELTY_ID) %>" runat="server" />
													<asp:HiddenField ID="hfProductIdSelectedIndex" Value="<%# Container.ItemIndex %>" runat="server" />
													<a href="javascript:open_product_list('<%#: CreateProductSearchUrl() %>','product_search','width=850,height=700,top=120,left=420,status=NO,scrollbars=yes','<%# Container.ItemIndex %>');" class="keyword-search-submit">
														<span class="icon-search" />
													</a>
												</div>
												<asp:HiddenField ID="hfSubscriptionBoxCourseId" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID) %>" runat="server" />
												<asp:HiddenField ID="hfSubscriptionBoxFixedAmount" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT) %>" runat="server" />
												<asp:HiddenField ID="hfIsOrderCombined" Value="<%# GetKeyValue(Container.DataItem, CONST_HASHKEY_CART_PRODUCT_IS_ORDER_COMBINED) %>" runat="server" />
											</td>
											<td class="edit">
												<asp:DropDownList ID="ddlVariationIdList" runat="server" class="w100"
													AutoPostBack="True"
													DataValueField="Value"
													DataTextField="Text"
													OnSelectedIndexChanged="ddlVariationId_SelectedIndexChanged" />
												<asp:HiddenField ID="hfProductVariationId" runat="server" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_VARIATION_ID) %>" />
																</td>
																<%--▽ 定期OPが有効の場合 ▽--%>
											<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED){ %>
											<td class="ta-center">
												<asp:CheckBox ID="cbFixedPurchase" OnCheckedChanged="cbFixedPurchase_CheckedChanged" Checked='<%# (bool)(GetKeyValue(Container.DataItem, CONST_KEY_FIXED_PURCHASE) ?? false) %>' AutoPostBack="true" runat="server" />
												<asp:HiddenField ID="hfFixedPurchaseMemberDiscountAmount" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT) %>" runat="server" />
												<asp:HiddenField ID="hfLimitedFixedPurchaseKbn1Setting" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING) %>" runat="server" />
												<asp:HiddenField ID="hfLimitedFixedPurchaseKbn3Setting" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING) %>" runat="server" />
												<asp:HiddenField ID="hfLimitedFixedPurchaseKbn4Setting" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING) %>" runat="server" />
												<asp:HiddenField ID="hfFixedPurchaseDiscountSettingValue" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE) %>" runat="server" />
												<asp:HiddenField ID="hfFixedPurchaseDiscountSettingType" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE) %>" runat="server" />
																</td>
																<%} %>
											<%-- △ 定期OPが有効の場合 △--%>
																<%--▽ セールOPが有効の場合 ▽--%>
											<% if (Constants.PRODUCT_SALE_OPTION_ENABLED){ %>
											<td class="edit">
												<asp:TextBox ID="tbProductSaleId" MaxLength="8" runat="server" class="w100" Text="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_PRODUCTSALE_ID) %>" OnTextChanged="tbProductSaleId_OnTextChanged" AutoPostBack="True"/>
																</td>
																<%} %>
																<%--△ セールOPが有効の場合 △--%>
											<asp:HiddenField ID="hfOptionPrice" Value='<%# GetKeyValue(Container.DataItem, Constants.KEY_OPTION_INCLUDED_ITEM_PRICE).ToPriceString() %>' runat="server" />
											<td class="edit ta-right" rowspan="3" visible="<%# IsSubscriptionBoxFixedAmount() == false %>" runat="server">
												<asp:TextBox ID="tbProductPrice" MaxLength="7" runat="server" class="w100 input-number" Text="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_PRODUCT_PRICE).ToPriceString() %>" />
																</td>
											<td class="edit ta-center" rowspan="3">
												<asp:TextBox ID="tbItemQuantity" MaxLength="3" runat="server" class="w100 input-number" Text="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_ITEM_QUANTITY) %>" />
											</td>
											<td class="ta-center" rowspan="3" visible="<%# HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
												<%#: IsCartProductSubscriptionBoxFixedAmount(Container.DataItem) == false
													     ? string.Format("{0}%", TaxCalculationUtility.GetTaxRateForDIsplay(GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE)))
													     : "-" %>
												<asp:HiddenField ID="hfTaxRate" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE) %>" runat="server" />
											</td>
											<td class='<%# IsCartProductSubscriptionBoxFixedAmount(Container.DataItem) == false ? "ta-right" : "ta-center" %>' rowspan="3" visible="<%# HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false %>" runat="server">
												<%#: IsCartProductSubscriptionBoxFixedAmount(Container.DataItem) == false
													     ? GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_ITEM_PRICE).ToPriceString(true)
													     : string.Format("定額({0})", GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT).ToPriceString(true)) %>
											</td>
											<td class="ta-center" rowspan="3">
												<span visible="<%# CheckCanDelete(Container) %>" runat="server">
													<asp:LinkButton ID="lbDeleteProduct" class="btn-delete btn btn-main btn-size-s" CommandName="delete" CommandArgument="<%# Container.ItemIndex %>" runat="server"><span class="icon-close"></span></asp:LinkButton>
												</span>
																</td>
															</tr>
										<tr>
											<td class="edit" colspan="<%= this.ProductColSpanNumber %>">
												<asp:TextBox ID="tbProductName" MaxLength="200" class="w100" Text="<%#: CreateProductJoinName(Container.DataItem) %>" runat="server" />
												<asp:HiddenField ID="hfOrderSetPromotionNo" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO) %>" runat="server" />
												<asp:HiddenField ID="hfOrderSetPromotionName" Value="<%# GetKeyValue(Container.DataItem, Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME) %>" runat="server" />
																</td>
															</tr>
										<tr class="product-option">
											<td colspan="<%= this.ProductColSpanNumber %>">
												<span class="accessory-info" style="display: block">
													<asp:Repeater ID="rProductOptionValueSettings" ItemType="ProductOptionSetting" DataSource='<%# GetKeyValue(Container.DataItem, CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS) %>' runat="server">
														<ItemTemplate>
															<div>
																<span class="accessory-info-label" title="<%#: Item.ValueName %>" style="display: inline-block "><%#: Item.ValueName %></span>
																<span class="ta-center" style="display: inline-block">
																	<span class="notice" runat="server" visible="<%# Item.IsNecessary %>">*</span>
																	<asp:DropDownList runat="server" ID="ddlProductOptionValueSetting"
																		DataSource='<%# Item.SettingValuesListItemCollection %>'
																		Visible='<%# ((Item.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU) || (Item.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU)) %>'
																		SelectedValue='<%# Item.GetDisplayProductOptionSettingSelectedValue() %>'
																		Style="width: 90%" />
																	<asp:TextBox ID="txtProductOptionValueSetting" Text='<%# ((Item.SelectedSettingValue == null) ? Item.DefaultValue : Item.SelectedSettingValue) %>' Visible='<%# Item.IsTextBox %>' runat="server" />
																	<asp:Repeater ID="rCblProductOptionValueSetting" ItemType="System.Web.UI.WebControls.ListItem" DataSource='<%# Item.SettingValuesListItemCollection %>' Visible='<%# ((Item.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX) || (Item.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX)) %>' runat="server">
																	<ItemTemplate>
																			<span title="<%# Item.Text %>">
																				<asp:CheckBox ID="cbProductOptionValueSetting" Text='<%# Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED ? Item.Text : StringUtility.AbbreviateString(Item.Text, 10) %>' Checked='<%# Item.Selected %>' runat="server" />
																			</span>
																	</ItemTemplate>
																	</asp:Repeater>
																</span>
															</div>
																</ItemTemplate>
																</asp:Repeater>
												</span>
																</td>
															</tr>
										<tr>
											<td colspan="<%= HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() ? 8 : 11 %>" class="separate"></td>
										</tr>
									</tbody>
														</ItemTemplate>
													</asp:Repeater>
							<!-- △商品情報詳細△ -->
												</table>
					</div>
				</div>
												<!-- △商品情報△ -->
			<!-- ▽ノベルティ選択▽ -->
				<% if (Constants.NOVELTY_OPTION_ENABLED) { %>
				<div id="dvNovelty" class="block-order-regist-input-section product-data novelty" visible="false" runat="server">
					<div id="dvOrderNoveltyList" class="product-list novelty" runat="server">
						<table class="product-list-table">
													<tr>
								<th colspan="12" class="product-list-table-title">
									<span class="product-list-table-title-icon icon-present"></span>
									ノベルティ選択
								</th>
													</tr>
																			<tr>
								<th colspan="11">ノベルティ名(管理用)</th>
																			</tr>
																			<tr>
								<th class="thum" rowspan="2">商品<br />
									画像</th>
								<th colspan="2" class="w100">商品名</th>
								<th class="product-price" rowspan="2">単価</th>
								<th class="add ta-center ItemAdd" rowspan="2">追加</th>
																			</tr>
																			<tr>
								<th class="product-id">商品ID</th>
								<th class="variation-id">バリエーションID</th>
																			</tr>
							<!-- ▽ノベルティ商品情報▽ -->
							<asp:Repeater ID="rNoveltyList" runat="server" ItemType="CartNovelty">
								<ItemTemplate>
									<tr>
										<td colspan="11" class="separate"></td>
									</tr>
									<tr visible="<%# (Item.GrantItemList.Length > 0) %>" runat="server">
										<td colspan="11"><%#: Item.NoveltyName %></td>
									</tr>
									<asp:Repeater ID="rNoveltyItem" runat="server" ItemType="CartNoveltyGrantItem" OnItemCommand="rNoveltyItem_ItemCommand" DataSource="<%# Item.GrantItemList %>">
																			<ItemTemplate>
																				<tr>
												<td class="thum resize" rowspan="2">
													<%# ProductImage.GetHtmlImageTag(Item.ProductInfo, JugdementNoveltyVariation(Item) ? ProductType.Variation : ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_S)%>
																					</td>
												<td colspan="2" class="w100" style="padding-top: 5px; padding-bottom: 5px; word-break: break-all;"><%#: Item.JointName %></td>
												<td class="ta-right" rowspan="2"><%#: Item.Price.ToPriceString(true) %></td>
												<td class="ta-center" rowspan="2">
													<asp:LinkButton ID="lbAddNovelty" class="btn btn-main btn-size-s" runat="server" Text="  追加  " CommandName="add" CommandArgument='<%# string.Format("{0},{1}", ((RepeaterItem)Container.Parent.Parent).ItemIndex, Container.ItemIndex) %>' />
																					</td>
																				</tr>
																				<tr>
												<td class="product-id"><%#: Item.ProductId %>
													<asp:HiddenField ID="hfNoveltyId" runat="server" Value="<%# Item.NoveltyId %>" />
													<asp:HiddenField ID="hfProductId" runat="server" Value="<%# Item.ProductId %>" />
													<asp:HiddenField ID="hfVariationId" runat="server" Value="<%# Item.VariationId %>" />
													<asp:HiddenField ID="hfProductName" runat="server" Value="<%# Item.JointName %>" />
													<asp:HiddenField ID="hfProductPrice" runat="server" Value="<%# Item.Price %>" />
													<asp:HiddenField ID="hfTaxRate" runat="server" Value="<%# Item.ProductInfo[Constants.FIELD_PRODUCT_TAX_RATE] %>" />
													<asp:HiddenField ID="hfProductImageHead" runat="server" Value="<%# Item.ProductInfo[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] %>" />
													<asp:HiddenField ID="hfShopId" runat="server" Value="<%# Item.ProductInfo[Constants.FIELD_PRODUCT_SHOP_ID] %>" />

												</td>
												<td class="variation-id">
													<%#: JugdementNoveltyVariation(Item) ? Item.VariationId : "-" %>
																					</td>
																				</tr>
																			</ItemTemplate>
																		</asp:Repeater>
																	</ItemTemplate>
							</asp:Repeater>
							<!-- △ノベルティ商品情報△ -->
																		</table>
					</div>
				</div>
				<% } %>
				<!-- △ノベルティ選択△ -->
				<div class="block-order-regist-input-section-row">
					<div class="block-order-regist-input-section discount <%= ((Constants.W2MP_COUPON_OPTION_ENABLED || (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser)) ?  string.Empty : "no-discount") %>"
						style="<%= (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser) ? string.Empty: "padding-block-end:inherit" %>">
						<div class="discount-method-list">
							<!-- ▽クーポン情報▽ -->
							<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
							<div id="dvCouponErrorMessage" class="block-order-regist-input-section-error" visible="false" runat="server">
								<asp:Literal ID="lCouponErrorMessage" runat="server" />
							</div>
							<div class="discount-method-list-coupon">
								<div id="dvHideCoupon" runat="server" style="text-align: left">
									<h3>クーポン利用 <span class="discount-method-list-coupon-available"></span></h3>
									<p class="note">注⽂者/商品情報確定後に表⽰されます</p>
								</div>
								<div id="dvShowCoupon" runat="server" visible="false" style="text-align: left">
									<h3>クーポン利用
										<span class="discount-method-list-coupon-available">
											<a id="user_coupon_list" href="javascript:open_window('<%: CreateUserCouponListUrl("UserCoupons") %>','coupon_list','width=1000,height=610,top=120,left=320,status=NO,scrollbars=yes');">ユーザー所持：
												<asp:Literal ID="lUserCouponCount" runat="server" Text="0" />
												枚</a>&nbsp;&nbsp;&nbsp;&nbsp;
											<a id="usable_coupon_list" href="javascript:open_window('<%: CreateUserCouponListUrl("UsableCoupons") %>','coupon_list','width=1000,height=610,top=120,left=320,status=NO,scrollbars=yes');">利用可能：
												<asp:Literal ID="lUsableCoupon" runat="server" Text="0" />
												枚
											</a>
										</span>
									</h3>
									<div class="discount-method-list-coupon-use">
										<div class="autocomplete">
											<asp:TextBox ID="tbCouponUse" runat="server"></asp:TextBox>
										</div>
										<asp:Button ID="btnApplyCoupon" runat="server" class="btn btn-main btn-size-s" Text="  適用  " OnClick="btnApplyCoupon_Click" />
									</div>
									<div class="discount-method-list-coupon-detail" id="dvShowCouponExist" runat="server" visible="false">
										<p><span class="discount-method-list-coupon-detail-title">クーポン名(管理用)：</span><asp:Literal ID="lCouponName" runat="server" /></p>
										<p><span class="discount-method-list-coupon-detail-title">クーポン名(ユーザ表示用)：</span><asp:Literal ID="lCouponDispName" runat="server" /></p>
										<p><span class="discount-method-list-coupon-detail-title">クーポン表示額／率：</span><asp:Literal ID="lCouponDiscount" runat="server" /></p>
									</div>
								</div>
							</div>
							<% } %>
							<!-- △クーポン情報△ -->
							<!-- ▽ポイント情報▽ -->
							<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser) { %>
							<div id="dvPointErrorMessage" class="block-order-regist-input-section-error" visible="false" runat="server">
								<asp:Literal ID="lPointErrorMessage" runat="server" />
							</div>
							<div id="dvHidePoint" class="discount-method-list-point" runat="server" style="text-align: left">
								<h3>ポイント利用</h3>
								<p class="note">注⽂者/商品情報確定後に表⽰されます</p>
							</div>
							<div id="dvShowPoint" runat="server" visible="false" style="text-align: left">
								<div class="discount-method-list-point">
									<h3>ポイント利用
									<span class="discount-method-list-coupon-available">
										<asp:Literal ID="lUserPointUsable" runat="server" Text="0" />
										<%: Constants.CONST_UNIT_POINT_PT %>利用可能
										&nbsp;&nbsp;&nbsp;&nbsp;
										<asp:Button ID="btnApplyUsablePointAll" runat="server" class="btn btn-main btn-size-s" Text="  全適用  " OnClick="btnApplyUsablePointAll_Click" />
									</span>
									</h3>
									<div class="discount-method-list-point-use">
										<asp:TextBox ID="tbPointUse" runat="server" class="w30 input-number" Text="0" />&nbsp;&nbsp;
									<asp:HiddenField ID="hfUserPointUsable" Value="0" runat="server" />
										<asp:Button ID="btnApplyPoint" runat="server" class="btn btn-main btn-size-s" Text="  適用  " OnClick="btnApplyPoint_Click" />
										<% if (this.CanUseAllPointFlg) { %>
										<asp:CheckBox ID="cbUseAllPointFlg" Text="定期注文で利用可能なポイント<br/>すべてを継続使用する"
											OnCheckedChanged="btnApplyUseAllPointFlg_Click" Visible="false"
											Style="margin-left: 1.4em; text-indent: -1.4em;" AutoPostBack="True" runat="server" />
										<% } %>
									</div>
								</div>
								<div class="discount-method-list-point-give">
									付与ポイント<span class="point"><asp:Literal ID="lPointBuy" runat="server" Text="0" />
										<%: Constants.CONST_UNIT_POINT_PT %></span>
								</div>
							</div>
															<% } %>
							<!-- △ポイント情報△ -->
						</div>
					</div>
					<asp:HiddenField ID="hfShippingType" runat="server" />
					<asp:HiddenField ID="hfIsDigitalContents" runat="server" />
					<asp:HiddenField ID="hfShippingKbn" runat="server" />
															<!-- ▽合計情報▽ -->
					<div class="block-order-regist-input-section order-detail-info">
						<div class="color">
							<table class="order-detail-info-table">
																		<tr>
									<th>商品合計</th>
									<td><span class="price-value">
										<asp:Literal ID="lPriceSubTotal" runat="server" /></span></td>
																		</tr>
								<% if (this.ProductIncludedTaxFlg == false) { %>
																			<tr>
									<th>消費税額</th>
									<td class="price-value">
										<asp:Literal ID="lbOrderPriceTax" runat="server" />
																				</td>
																			</tr>
								<% } %>
								<asp:Repeater ID="rOrderSetPromotionProductDiscount" ItemType="CartSetPromotion" runat="server">
									<ItemTemplate>
										<tr visible="<%# Item.IsDiscountTypeProductDiscount %>" runat="server">
											<th>
												<%# Item.CartSetPromotionNo %>：<%#: Item.SetpromotionName %>割引額<br />
												(ID:<%# Item.SetpromotionId %>,商品割引分)
											</th>
											<td runat="server" class="price-value ta-right">
												<span class='<%# (Item.ProductDiscountAmount > 0 ? "discount" : string.Empty) %>' style="color: red">
													<%#: (Item.ProductDiscountAmount * -1).ToPriceString(true) %>
												</span>
											</td>
										</tr>
									</ItemTemplate>
								</asp:Repeater>
								<%--▽ 会員ランクOPが有効の場合 ▽--%>
								<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
								<tr id="trMemberRankDiscount" runat="server">
									<th>会員ランク割引</th>
									<td class="price-value">
										<asp:Literal ID="lMemberRankDiscount" runat="server" />
																			</td>
																		</tr>
								<% } %>
																		<%--▽ 会員ランクOPと定期OPが有効の場合 ▽--%>
								<% if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
								<tr id="trFixedPurchaseMemberDiscountAmount" runat="server">
									<th>定期会員割引</th>
									<td class="price-value">
										<asp:Literal ID="lFixedPurchaseMemberDiscountAmount" runat="server" /></td>
																		</tr>
								<% } %>
								<%--△ 会員ランクOPが有効の場合 △ --%>
																		<%--▽ クーポンOPが有効の場合 ▽--%>
								<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
								<tr id="trCouponDiscount" runat="server">
									<th>クーポン割引額</th>
									<td><span class="price-value">
										<asp:Literal ID="lCouponUsePrice" runat="server" /></span></td>
																		</tr>
								<% } %>
																		<%--△ クーポンOPが有効の場合 △--%>
																		<%-- 定期購入オプションが有効の場合 --%>
																		<%--▽ ポイントOPが有効 AND 会員の場合 ▽--%>
								<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser) { %>
								<tr id="trPointDiscount" runat="server">
									<th>ポイント利用額</th>
									<td><span class="price-value">
										<asp:Literal ID="lPointUsePrice" runat="server" /></span></td>
																		</tr>
								<% } %>
																		<%--△ ポイントOPが有効 AND 会員の場合 △--%>
								<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
								<tr id="trFixedPurchaseDiscountPrice" runat="server">
									<th>定期購入割引額</th>
									<td class="price-value">
										<asp:Literal ID="lFixedPurchaseDiscountPrice" runat="server" /></td>
																		</tr>
																		<% } %>
																		<tr>
									<th>配送料</th>
									<td><span class="price-value">
										<asp:Literal ID="lOrderPriceShipping" runat="server" /></span></td>
																		</tr>
								<asp:Repeater ID="rOrderSetPromotionShippingDiscount" ItemType="CartSetPromotion" runat="server">
																		<ItemTemplate>
										<tr visible="<%# Item.IsDiscountTypeShippingChargeFree %>" runat="server">
											<th>
												<%# Item.CartSetPromotionNo %>：<%#: Item.SetpromotionName %>割引額<br />
												(ID:<%# Item.SetpromotionId %>,配送料割引分)
											</th>
											<td class="price-value ta-right">
												<span class='<%# Item.ShippingChargeDiscountAmount > 0 ? "discount" : string.Empty %>' style="color: red">
													<%#: (Item.ShippingChargeDiscountAmount * -1).ToPriceString(true) %>
																					</span>
																				</td>
																			</tr>
																		</ItemTemplate>
																		</asp:Repeater>
																		<tr>
									<th>決済手数料</th>
									<td><span class="price-value">
										<asp:Literal ID="lOrderPriceExchange" runat="server" /></span></td>
																		</tr>
								<asp:Repeater ID="rOrderSetPromotionSettlementDiscount" ItemType="CartSetPromotion" runat="server">
																		<ItemTemplate>
										<tr visible="<%# Item.IsDiscountTypePaymentChargeFree %>" runat="server">
											<th>
												<%# Item.CartSetPromotionNo %>：<%#: Item.SetpromotionName %>割引額<br />
												(ID:<%# Item.SetpromotionId %>,決済手数料料割引分)
											</th>
											<td class="price-value ta-right">
												<span class='<%# (Item.PaymentChargeDiscountAmount > 0 ? "discount" : string.Empty) %>' style="color: red">
													<%#: (Item.PaymentChargeDiscountAmount * -1).ToPriceString(true) %>
																					</span>
																				</td>
																			</tr>
																		</ItemTemplate>
																		</asp:Repeater>
								<tr class="adjust">
									<th>調整金額</th>
									<td>
										<span class="price-value" style="margin-right: 3px"><%= ((CurrencyManager.IsJapanKeyCurrencyCode) ? "&yen;" : string.Empty) %></span>
										<asp:TextBox ID="tbOrderPriceRegulation" runat="server" class="w50 input-number" />
																			</td>
																		</tr>
								<asp:Repeater ID="rTotalPriceByTaxRate" ItemType="CartPriceInfoByTaxRate" runat="server">
																			<ItemTemplate>
																				<tr runat="server">
											<th class="ta-right">合計金額内訳(税率<%#: TaxCalculationUtility.GetTaxRateForDIsplay(Item.TaxRate)%>%)
											</th>
											<td class="price-value ta-right">
																						<%# GetMinusNumberNoticeHtml(Item.PriceTotal, true) %>
																					</td>
																				</tr>
																			</ItemTemplate>
																		</asp:Repeater>
								<tr id="trOrderPriceTotal" class="total" runat="server">
									<th><strong>金額合計</strong></th>
									<td><strong><span class="price-value">
										<asp:Literal ID="lOrderPriceTotal" runat="server" /></span></strong></td>
																		</tr>
								<tr id="trSettlementAmount" class="total" runat="server" visible="false">
									<th>決済金額</th>
									<td><span class="price-value">
										<asp:Literal ID="lSettlementAmount" runat="server" /></span></td>
																		</tr>
																	</table>
						</div>
																<!-- △合計情報△ -->
					</div>
				</div>
				<!-- ▽注文同梱同梱後のカート内容▽ -->
				<% if (Constants.ORDER_COMBINE_OPTION_ENABLED) { %>
				<div id="dvCombinedCart" runat="server" visible="false">
					<div class="block-order-regist-input-section product-data">
						<div class="block-order-regist-input-section-header">
							<h2 class="block-order-regist-input-section-title">
								<span class="block-order-regist-input-section-title-icon icon-box"></span>
								<span class="block-order-regist-input-section-title-label">注文同梱情報</span>
							</h2>
							<div class="block-order-regist-input-section-header-right">
								<span class="add-row">
									<asp:Button ID="btnClearOrderCombine" class="btn btn-sub btn-size-s" runat="server" Text="  同梱解除  " OnClick="btnClearOrderCombine_Click" />
								</span>
							</div>
						</div>
						<div class="block-order-regist-input-section-error">
							注文同梱後に注文者情報および商品情報、調整金額を修正しても注文情報に反映されません。修正される際には一度同梱解除ください。
						</div>
						<div id="dvCombinedCartErrorMessage" class="block-order-regist-input-section-error" Visible="False" runat="server">
							<asp:Literal ID="lCombinedCartErrorMessage" runat="server" />
						</div>
						<div class="product-list">
							<table class="product-list-table">
								<tbody>
												<tr>
										<th class="thum" rowspan="1">商品<br />
											画像</th>
										<th class="product-name" rowspan="1">商品名</th>
										<th class="product-price" rowspan="1">単価</th>
										<th class="item-quantity" rowspan="1">数量</th>
										<th class="item-price" rowspan="1">小計</th>
												</tr>
													<tr>
										<td colspan="11" class="separate"></td>
												</tr>
									<asp:Repeater ID="rCombinedCart" runat="server" ItemType="CartObject">
										<ItemTemplate>
											<asp:Repeater ID="rCombinedCartItem" runat="server" ItemType="CartProduct" DataSource="<%# Item.Items %>">
												<ItemTemplate>
														<tr>
															<td class="thum" rowspan="1">
															<%# ProductImage.GetHtmlImageTag(Container.DataItem, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_S) %>
															<td rowspan="1" style="padding-top: 5px; padding-bottom: 5px; word-break: break-all"><%#: Item.ProductJointName %></td>
															<td class='<%# Item.IsSubscriptionBoxFixedAmount() == false ? "ta-right" : "ta-center" %>' rowspan="1">
																<%#: Item.IsSubscriptionBoxFixedAmount() == false ? Item.Price.ToPriceString(true) : "-" %>
															</td>
															<td rowspan="1"><%#: Item.Count %></td>
															<td class='<%# Item.IsSubscriptionBoxFixedAmount() == false ? "ta-right" : "ta-center" %>' rowspan="1">
																<%#: Item.IsSubscriptionBoxFixedAmount() == false
																	     ? Item.PriceSubtotal.ToPriceString(true)
																	     : string.Format("定額({0})", Item.SubscriptionBoxFixedAmount.ToPriceString(true)) %>
															</td>
														</tr>
														<tr>
															<td colspan="11" class="separate"></td>
														</tr>
													</ItemTemplate>
												</asp:Repeater>
										</ItemTemplate>
									</asp:Repeater>
								</tbody>
							</table>
						</div>
					</div>
					<asp:Repeater ID="rCombinedCartInfo" runat="server" ItemType="CartObject">
						<ItemTemplate>
							<div class="block-order-regist-input-section-row">
								<div class="block-order-regist-input-section discount">
									<div class="discount-method-list">
										<div class="discount-method-list-coupon">
											<h3>決済種別</h3>
											<div class="discount-method-list-point-use">
												<%#: ValueText.GetValueText(Constants.TABLE_PAYMENT, CONST_KEY_PAYMENT_TYPE, Item.Payment.PaymentId) %>
											</div>
										</div>
										<div class="discount-method-list-point">
											<h3>配送種別</h3>
											<div class="discount-method-list-point-use">
												<p><%#: GetShopShippingName(Item.ShippingType) %></p>
											</div>
										</div>
										<div class="discount-method-list-point">
											<h3>配送間隔</h3>
											<div class="discount-method-list-point-use">
												<p><%#: GetFixedPurchasePatternSettingMessage(Item.GetShipping().FixedPurchaseKbn, Item.GetShipping().FixedPurchaseSetting) %></p>
											</div>
										</div>
									</div>
								</div>
								<div class="block-order-regist-input-section order-detail-info">
									<table class="order-detail-info-table">
										<tbody>
												<tr>
												<th>商品合計</th>
												<td><span class="price-value"><%#: Item.PriceSubtotal.ToPriceString(true) %></span></td>
												</tr>
												<asp:Repeater DataSource="<%# (Item.SetPromotions != null)
												? Item.SetPromotions.Items.Where(setPromotion => (setPromotion.ProductDiscountFlg == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON))
												: null %>"
												ItemType="CartSetPromotion" runat="server">
													<ItemTemplate>
														<tr visible="<%# (Item.ProductDiscountFlg == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON) %>" runat="server">
														<th>
																<%#: Container.ItemIndex %>：<%#: Item.SetpromotionDispName %>割引額<br />
																(ID:<%#: Item.SetpromotionId %>,商品割引分)
														</th>
														<td>
															<span class="price-value" style='<%# ((Item.ProductDiscountAmount > 0) ? "color:red": string.Empty) %>'>
																<%#: (Item.ProductDiscountAmount * -1).ToPriceString(true) %>
																</span>
															</td>
														</tr>
													</ItemTemplate>
												</asp:Repeater>
												<%--▽ 会員ランクOPが有効の場合 ▽--%>
											<% if (Constants.MEMBER_RANK_OPTION_ENABLED) { %>
											<tr id="trMemberRankDiscount" runat="server">
												<th>会員ランク割引</th>
												<td>
													<span class="price-value" style='<%# ((Item.MemberRankDiscount > 0) ? "color:red": string.Empty) %>'>
														<%#: (Item.MemberRankDiscount * -1).ToPriceString(true) %>
														</span>
													</td>
												</tr>
											<% } %>
												<%--▽ 会員ランクOPと定期OPが有効の場合 ▽--%>
											<% if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
											<tr id="trFixedPurchaseMemberDiscount" runat="server">
												<th>定期会員割引</th>
												<td>
													<span class="price-value" style='<%# ((Item.FixedPurchaseMemberDiscountAmount > 0) ? "color:red": string.Empty) %>'>
														<%#: (Item.FixedPurchaseMemberDiscountAmount * -1).ToPriceString(true) %>
														</span>
													</td>
												</tr>
											<% } %>
											<%--▽ クーポンOPが有効の場合 ▽--%>
											<% if (Constants.W2MP_COUPON_OPTION_ENABLED) { %>
											<tr id="trCouponDiscount" runat="server">
												<th>クーポン割引額</th>
												<td>
													<span class="price-value" style='<%# ((Item.UseCouponPrice > 0) ? "color:red": string.Empty) %>'>
														<%#: (Item.UseCouponPrice * -1).ToPriceString(true) %>
														</span>
													</td>
												</tr>
											<% } %>
											<%--▽ ポイントOPが有効 AND 会員の場合 ▽--%>
											<% if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser) { %>
											<tr id="trPointDiscount" runat="server">
												<th>ポイント利用額</th>
												<td>
													<span class="price-value" style='<%# ((Item.UsePointPrice > 0) ? "color:red": string.Empty) %>'>
														<%#: (Item.UsePointPrice * -1).ToPriceString(true) %>
														</span>
													</td>
												</tr>
											<% } %>
												<%-- 定期購入オプションが有効の場合 --%>
											<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
											<tr id="trFixedPurchaseDiscountPrice" runat="server">
												<th>定期購入割引額</th>
												<td>
													<span class="price-value" style='<%# ((Item.FixedPurchaseDiscount > 0) ? "color:red": string.Empty) %>'>
														<%#: (Item.FixedPurchaseDiscount * -1).ToPriceString(true) %>
														</span>
													</td>
												</tr>
												<% } %>
												<tr>
												<th>配送料</th>
												<td><span class="price-value"><%#: Item.PriceShipping.ToPriceString(true) %></span></td>
												</tr>
											<asp:Repeater DataSource="<%# (Item.SetPromotions != null)
													? Item.SetPromotions.Items.Where(setPromotion => (setPromotion.ShippingChargeFreeFlg == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON))
													: null %>"
												ItemType="CartSetPromotion" runat="server">
												<ItemTemplate>
													<tr visible="<%# (Item.ShippingChargeFreeFlg == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON) %>" runat="server">
														<th>
															<%#: Container.ItemIndex %>：<%#: Item.SetpromotionDispName %>割引額<br />
															(ID:<%#: Item.SetpromotionId %>,配送料割引分)
														</th>
														<td>
															<span class="price-value" style='<%# ((Item.ShippingChargeDiscountAmount > 0) ? "color:red": string.Empty) %>'>
																<%#: (Item.ShippingChargeDiscountAmount * -1).ToPriceString(true) %>
															</span>
														</td>
													</tr>
												</ItemTemplate>
												</asp:Repeater>
												<tr>
												<th>決済手数料</th>
												<td><span class="price-value"><%#: Item.Payment.PriceExchange.ToPriceString(true) %></span></td>
												</tr>
											<asp:Repeater DataSource="<%# (Item.SetPromotions != null)
												? Item.SetPromotions.Items.Where(setPromotion => (setPromotion.PaymentChargeFreeFlg == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON))
												: null %>"
												ItemType="CartSetPromotion" runat="server">
												<ItemTemplate>
													<tr visible="<%# (Item.PaymentChargeFreeFlg == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON) %>" runat="server">
															<td>
															<%#: Container.ItemIndex %>：<%#: Item.SetpromotionDispName %>割引額<br />
															(ID:<%#: Item.SetpromotionId %>,決済手数料料割引分)
																			</td>
																				<td>
															<span class="price-value" style='<%# ((Item.PaymentChargeDiscountAmount > 0) ? "color:red": string.Empty) %>'>
																<%#: (Item.PaymentChargeDiscountAmount * -1).ToPriceString(true) %>
															</span>
																								</td>
																								</tr>
																											</ItemTemplate>
																										</asp:Repeater>
											<tr id="trPriceRegulation" class="adjust" runat="server">
												<th>調整金額</th>
												<td><%#: Item.PriceRegulation.ToPriceString(true) %></td>
																								</tr>
											<tr class="total">
												<th><strong>金額合計</strong></th>
												<td>
													<strong>
														<span class="price-value">
															<%#: Item.PriceTotal.ToPriceString(true) %>
														</span>
													</strong>
																									</td>
																								</tr>
										</tbody>
									</table>
								</div>
							</div>
																							</ItemTemplate>
																						</asp:Repeater>
																	</div>
				<% } %>
				<!-- △注文同梱同梱後のカート内容△ -->
				<!-- ▽メモ▽ -->
				<div class="block-order-regist-input-section memo">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-memo"></span>
							<span class="block-order-regist-input-section-title-label">メモ</span>
						</h2>
																</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">調整金額メモ
								<uc:FieldMemoSetting runat="server"
									Title="調整金額メモ"
									FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
									TableName="<%# Constants.TABLE_ORDER %>"
									FieldName="<%# Constants.FIELD_ORDER_REGULATION_MEMO %>" />
								</span>
													</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbRegulationMemo" runat="server" TextMode="MultiLine" Rows="2" />
																									</div>
						</div>
						<!-- ▽メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<div class="form-element-group-title">
									<span class="memo-title-text" style="width: 131px">注文メモ
									<% if (cbReflectMemoToFixedPurchase.Visible) { %>
																									<br />
										<asp:CheckBox ID="cbReflectMemoToFixedPurchase" Text="定期台帳へも反映" Visible="false" Checked="true" runat="server" />
										<% } %>
										<uc:FieldMemoSetting runat="server"
											Title="注文メモ"
											FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
											TableName="<%# Constants.TABLE_ORDER %>"
											FieldName="<%# Constants.FIELD_ORDER_MEMO %>" />
									</span>
								</div>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:Repeater ID="rOrderMemos" runat="server">
																											<ItemTemplate>
										<p class="memo-input-title">[<%# Eval(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_NAME) %>]</p>
										<asp:TextBox ID="tbMemo" runat="server" Text="<%# Eval(Constants.FIELD_ORDERMEMOSETTING_DEFAULT_TEXT) %>" TextMode="MultiLine" Rows="8" Style="height: 100px;" />
																											</ItemTemplate>
																										</asp:Repeater>
																	</div>
							<div id="dvOrderMemoForCombine" class="form-element-group-content" runat="server" visible="false">
								<asp:TextBox ID="tbMemoForCombine" runat="server" TextMode="MultiLine" Rows="8" Style="height: 100px;" />
																</div>
													</div>
						<!-- △メモ情報△ -->
						<!-- ▽決済連携メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">決済連携メモ
								<uc:FieldMemoSetting runat="server"
									Title="決済連携メモ"
									FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
									TableName="<%# Constants.TABLE_ORDER %>"
									FieldName="<%# Constants.FIELD_ORDER_PAYMENT_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbPaymentMemo" runat="server" TextMode="MultiLine" Rows="2" />
							</div>
						</div>
						<!-- △決済連携メモ情報△ -->
						<!-- ▽管理メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">管理メモ
								<uc:FieldMemoSetting runat="server"
									Title="管理メモ"
									FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
									TableName="<%# Constants.TABLE_ORDER %>"
									FieldName="<%# Constants.FIELD_ORDER_MANAGEMENT_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbManagerMemo" runat="server" TextMode="MultiLine" Rows="2" />
							</div>
						</div>
						<!-- △管理メモ情報△ -->
						<!-- ▽定期購入管理メモ情報▽ -->
						<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">定期購入管理メモ
									<uc:FieldMemoSetting runat="server"
										Title="定期購入管理メモ"
										FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
										TableName="<%# Constants.TABLE_ORDER %>"
										FieldName="<%# Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbFixedPurchaseManagementMemo" runat="server" TextMode="MultiLine" Rows="2" />
							</div>
						</div>
												<% } %>
						<!-- △定期購入管理メモ情報△ -->
						<!-- ▽外部連携メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">外部連携メモ
								<uc:FieldMemoSetting runat="server"
									Title="外部連携メモ"
									FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
									TableName="<%# Constants.TABLE_ORDER %>"
									FieldName="<%# Constants.FIELD_ORDER_RELATION_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbRelationMemo" runat="server" TextMode="MultiLine" Rows="2" />
							</div>
						</div>
						<!-- △外部連携メモ情報△ -->
						<!-- ▽配送メモ情報▽ -->
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<span class="memo-title-text">配送メモ
								<uc:FieldMemoSetting runat="server"
									Title="配送メモ"
									FieldMemoSettingList="<%# this.FieldMemoSettingList %>"
									TableName="<%# Constants.TABLE_ORDER %>"
									FieldName="<%# Constants.FIELD_ORDER_SHIPPING_MEMO %>" />
								</span>
							</div>
							<div class="form-element-group-content" style="width: 131px">
								<asp:TextBox ID="tbShippingMemo" runat="server" TextMode="MultiLine" Rows="2" />
							</div>
						</div>
						<!-- △配送メモ情報△ -->
					</div>
				</div>
				<!-- △メモ△ -->
			</div>
			<div class="block-order-regist-input-block3">
												<!-- ▽配送指定情報▽ -->
				<div class="block-order-regist-input-section ShippingInfo" id="dvShippingInfo" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-shipping"></span>
							<span class="block-order-regist-input-section-title-label">配送指定</span>
						</h2>
												</div>
					<div class="block-order-regist-input-section-contents" runat="server" id="dvHideDeliveryDesignation" style="text-align: left">
						<p class="note">注⽂者/商品情報確定後に表⽰されます</p>
					</div>
					<div runat="server" id="dvShowDeliveryDesignation" visible="false">
						<div class="block-order-regist-input-section-error" id="dvDeliveryErrorMessage" visible="false" runat="server">
							<asp:Literal ID="lShippingInfoErrorMessages" runat="server" />
						</div>
						<div class="block-order-regist-input-section-contents">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									配送方法
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList class="w100" ID="ddlShippingMethod" runat="server" OnSelectedIndexChanged="ddlShippingMethod_SelectedIndexChanged" AutoPostBack="true" />
									<p style="padding: 5px 0 5px 0">
										<asp:Button ID="btnSetShippingMethod" class="btn btn-main btn-size-s" runat="server" Text="  配送方法自動判定  " OnClick="btnSetShippingMethod_Click" />
									</p>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									配送サービス
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList class="w100" ID="ddlDeliveryCompany" runat="server" OnSelectedIndexChanged="ddlDeliveryCompany_SelectedIndexChanged" AutoPostBack="true" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid" id="dvShippingTime" runat="server">
								<div class="form-element-group-title">
									配送希望時間帯
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlShippingTime" runat="server" DataTextField="Text" DataValueField="Value" class="w100" />
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid" id="dvShippingDate" runat="server">
								<div class="form-element-group-title">
									配送希望日
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlShippingDate" runat="server" OnSelectedIndexChanged="ddlShippingDate_SelectedIndexChanged" AutoPostBack="true" class="w100" />
									<p>
										<span class="note" style="color: red">
											<asp:Literal ID="lShippingDateErrorMessages" runat="server" />
										</span>
									</p>
								</div>
							</div>
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									配送伝票番号
								</div>
								<div class="form-element-group-content">
									<asp:TextBox ID="tbShippingCheckNo" MaxLength="50" class="w100" runat="server" />
								</div>
							</div>
						</div>
					</div>
				</div>
												<!-- △配送指定情報△ -->
												<!-- ▽定期購入配送パターン情報▽ -->
												<asp:HiddenField ID="hfHasFixedPurchase" Value="false" runat="server" />
				<div id="dvShippingFixedPurchase" class="block-order-regist-input-section shipping-pattern" visible="false" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-calendar"></span>
							<span class="block-order-regist-input-section-title-label">定期配送パターン</span>
						</h2>
					</div>
					<div id="dvFixedPurchasePatternErrorMessages" class="block-order-regist-input-section-error" runat="server" visible="false">
						<asp:Literal ID="lFixedPurchasePatternErrorMessage" runat="server" />
					</div>
					<div class="block-order-regist-input-section-contents">
						<h3 class="form-element-group-list-title">配送パターン</h3>
						<dl class="form-element-group-list">
							<dt id="dtMonthlyPurchase_Date" runat="server">
								<span>
									<asp:RadioButton ID="rbMonthlyPurchase_Date" Text="月間隔日付指定" GroupName="FixedPurchaseShippingPattern" runat="server"
										OnCheckedChanged="rbMonthlyPurchase_Date_CheckedChanged" AutoPostBack="true" />
								</span>
							</dt>
							<dd id="ddMonthlyPurchase_Date" runat="server">
								<asp:DropDownList ID="ddlMonth" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
																	ヶ月ごと
							<asp:DropDownList ID="ddlMonthlyDate" DataTextField="Text" DataValueField="Value" runat="server"
								OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
																	日に届ける
																	<br />
							</dd>
							<dt id="dtMonthlyPurchase_WeekAndDay" runat="server">
								<span>
									<asp:RadioButton ID="rbMonthlyPurchase_WeekAndDay" Text="月間隔・週・曜日指定" GroupName="FixedPurchaseShippingPattern" runat="server"
										OnCheckedChanged="rbMonthlyPurchase_WeekAndDay_CheckedChanged" AutoPostBack="true" />
																</span>
							</dt>
							<dd id="ddMonthlyPurchase_WeekAndDay" runat="server">
								<asp:DropDownList ID="ddlIntervalMonths" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
								ヶ月ごと
							<asp:DropDownList ID="ddlWeekOfMonth" DataTextField="Text" DataValueField="Value" runat="server"
								OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
								<asp:DropDownList ID="ddlDayOfWeek" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged" AutoPostBack="true" />
																	に届ける
																	<br />
							</dd>
							<dt id="dtRegularPurchase_IntervalDays" runat="server">
								<span>
									<asp:RadioButton ID="rbRegularPurchase_IntervalDays" Text="配送日間隔指定" GroupName="FixedPurchaseShippingPattern" runat="server"
										OnCheckedChanged="rbRegularPurchase_IntervalDays_CheckedChanged" AutoPostBack="true" />
																</span>
							</dt>
							<dd id="ddRegularPurchase_IntervalDays" runat="server">
								<asp:DropDownList ID="ddlIntervalDays" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlIntervalDays_OnSelectedIndexChanged" AutoPostBack="true" />
																	日ごとに届ける
																	<br />
							</dd>
							<dt id="dtPurchase_EveryNWeek" runat="server">
								<span>
										<asp:RadioButton ID="rbPurchase_EveryNWeek" Text="週間隔・曜日指定" GroupName="FixedPurchaseShippingPattern" runat="server"
											OnCheckedChanged="rbPurchase_EveryNWeek_CheckedChanged" AutoPostBack="true" />
								</span>
							</dt>
							<dd id="ddPurchase_EveryNWeek" runat="server">
								<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_Week" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged" AutoPostBack="true" />
																週間ごとの
								<asp:DropDownList ID="ddlFixedPurchaseEveryNWeek_DayOfWeek" DataTextField="Text" DataValueField="Value" runat="server"
									OnSelectedIndexChanged="ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged" AutoPostBack="true" />
																に届ける
																<br />
							</dd>
						</dl>
													<asp:HiddenField ID="hfDaysRequired" runat="server" />
													<asp:HiddenField ID="hfMinSpan" runat="server" />
						<!-- ▽初回配送予定日▽ -->
						<h3 class="form-element-group-list-title">初回配送予定日</h3>
						<dl class="form-element-group-list">
							<dt>
								<asp:Literal ID="lFirstShippingDate" runat="server" />
							</dt>
							<dt>
								<asp:DropDownList
									ID="ddlFirstShippingDate"
									Visible="false"
									OnSelectedIndexChanged="ddlFirstShippingDate_SelectedIndexChanged"
									AutoPostBack="true"
									runat="server" />
							</dt>
						</dl>
						<!-- △初回配送予定日△ -->
						<!-- ▽次回配送日指定▽ -->
						<h3 class="form-element-group-list-title">次回配送日指定</h3>
						<dl class="form-element-group-list">
							<dt>
								<asp:Literal ID="lNextShippingDate" runat="server" />
													<br />
								<asp:DropDownList
									ID="ddlNextShippingDate"
									Visible="false"
									OnSelectedIndexChanged="ddlNextShippingDate_SelectedIndexChanged"
									AutoPostBack="true"
									runat="server" />
							</dt>
						</dl>
						<!-- △次回配送日指定△ -->
						<p class="note">※メール便の場合、配送予定日は数日ずれる可能性があります。</p>
												</div>
				</div>
												<!-- △定期購入配送パターン情報△ -->
				<!-- ▽領収書情報▽ -->
				<% if (Constants.RECEIPT_OPTION_ENABLED) { %>
				<div class="block-order-regist-input-section" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-label">領収書情報</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-error" id="dvReceiptErrorMessages" visible="false" runat="server">
						<asp:Literal ID="lReceiptErrorMessages" runat="server" />
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								領収書希望
							</div>
							<div class="form-element-group-content">
								<asp:RadioButtonList ID="rblReceiptFlg" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
									OnSelectedIndexChanged="rblReceiptFlg_SelectedIndexChanged" AutoPostBack="True" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								宛名
								<% if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON) { %>
								<span class="notice">*</span>
								<% } %>
							</div>
							<div class="form-element-group-content">
								<asp:TextBox ID="tbReceiptAddress" runat="server" MaxLength="100" class="w100" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								但し書き
								<% if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON) { %>
								<span class="notice">*</span>
								<% } %>
							</div>
							<div class="form-element-group-content">
								<asp:TextBox ID="tbReceiptProviso" runat="server" MaxLength="100" class="w100" />
							</div>
						</div>
					</div>
				</div>
				<% } %>
				<!-- △領収書情報△ -->
				<!-- ▽電子発票▽ -->
				<% if (OrderCommon.DisplayTwInvoiceInfo() && this.IsShippingAddrTw) { %>
				<div class="block-order-regist-input-section" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-label">電子発票</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-error" id="dvElectronicBillErrorMessages" visible="false" runat="server">
						<asp:Literal ID="lElectronicBillErrorMessages" runat="server" />
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								発票種類
							</div>
							<div class="form-element-group-content">
								<asp:DropDownList ID="ddlUniformInvoiceType" runat="server"
									DataTextField="text"
									DataValueField="value"
									OnSelectedIndexChanged="ddlUniformInvoiceType_SelectedIndexChanged"
									AutoPostBack="true">
								</asp:DropDownList>
								<asp:DropDownList ID="ddlUniformInvoiceOrCarryTypeOption" runat="server"
									DataTextField="text"
									DataValueField="value"
									OnSelectedIndexChanged="ddlUniformInvoiceOrCarryTypeOption_SelectedIndexChanged"
									AutoPostBack="true"
									Visible="false">
								</asp:DropDownList>
							</div>
						</div>
						<div id="dvUniformInvoiceOption1_1" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								統一編号<span id="spUniformInvoiceOption1_1" class="notice" runat="server">*</span>
							</div>
							<div class="form-element-group-content">
								<asp:TextBox ID="tbUniformInvoiceOption1_1" placeholder="例:12345678" runat="server" MaxLength="8" class="w100" />
								<asp:Literal ID="lUniformInvoiceOption1_1" Visible="false" runat="server" />
							</div>
						</div>
						<div id="dvUniformInvoiceOption1_2" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								会社名<span id="spUniformInvoiceOption1_2" class="notice" runat="server">*</span>
							</div>
							<div class="form-element-group-content">
								<asp:TextBox ID="tbUniformInvoiceOption1_2" placeholder="例:○○有限股份公司" runat="server" MaxLength="20" class="w100" />
								<asp:Literal ID="lUniformInvoiceOption1_2" Visible="false" runat="server" />
							</div>
						</div>
						<div id="dvUniformInvoiceOption2" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								寄付先コード<span id="spUniformInvoiceOption2" class="notice" runat="server">*</span>
							</div>
							<div class="form-element-group-content">
								<asp:TextBox ID="tbUniformInvoiceOption2" runat="server" MaxLength="7" class="w100" />
								<asp:Literal ID="lUniformInvoiceOption2" Visible="false" runat="server" />
							</div>
						</div>
						<div id="dvUniformInvoiceTypeRegist" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-content">
								<asp:CheckBox ID="cbSaveToUserInvoice" runat="server" Text="電子発票管理情報を保存する" AutoPostBack="true"
									OnCheckedChanged="cbSaveToUserInvoice_CheckedChanged" />
							</div>
						</div>
						<div id="dvUniformInvoiceTypeRegistInput" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								電子発票情報名<span class="notice">*</span>
							</div>
							<div class="form-element-group-content">
								<asp:TextBox ID="tbUniformInvoiceTypeName" MaxLength="30" runat="server" class="w100" />
							</div>
						</div>
						<div id="dvInvoiceCarryType" runat="server" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								共通性載具<span class="notice" id="spInvoiceCarryType" runat="server" visible="false">*</span>
							</div>
							<div class="form-element-group-content">
								<asp:DropDownList ID="ddlInvoiceCarryType" runat="server"
									DataTextField="text"
									DataValueField="value"
									OnSelectedIndexChanged="ddlInvoiceCarryType_SelectedIndexChanged"
									AutoPostBack="true"
									Style="margin-bottom: 2px" />
								<asp:TextBox ID="tbCarryTypeOption1" Visible="false" runat="server" placeholder="例:/AB201+9(限8個字)" MaxLength="8" />
								<asp:TextBox ID="tbCarryTypeOption2" Visible="false" runat="server" placeholder="例:TP03000001234567(限16個字)" MaxLength="16" />
							</div>
						</div>
						<div id="dvCarryTypeOptionRegist" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-content">
								<asp:CheckBox ID="cbCarryTypeOptionRegist" runat="server" class="w100" Text="電子発票管理情報を保存する" AutoPostBack="true"
									OnCheckedChanged="cbCarryTypeOptionRegist_CheckedChanged" />
							</div>
						</div>
						<div id="dvCarryTypeOptionName" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								電子発票情報名<span class="notice">*</span>
							</div>
							<div class="form-element-group-content">
								<asp:TextBox ID="tbCarryTypeOptionName" MaxLength="30" runat="server" class="w100" />
							</div>
						</div>
					</div>
				</div>
				<% } %>
				<!-- △電子発票△ -->
				<!-- ▽コンバージョン情報▽ -->
				<div class="block-order-regist-input-section" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-label">コンバージョン情報</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: StringUtility.ToHankaku("流入コンテンツタイプ") %>
							</div>
							<div class="form-element-group-content">
								<asp:DropDownList ID="ddlInflowContentsType" runat="server" Width="100%" />
							</div>
						</div>
						<div class="form-element-group form-element-group-horizontal-grid">
							<div class="form-element-group-title">
								<%: StringUtility.ToHankaku("流入コンテンツID") %>
							</div>
							<div class="form-element-group-content">
								<asp:TextBox ID="tbInflowContentsId" runat="server" MaxLength="30" class="w100" />
							</div>
						</div>
					</div>
				</div>
				<!-- △コンバージョン情報△ -->
				<!-- ▽注文拡張項目▽-->
				<% if (Constants.ORDER_EXTEND_OPTION_ENABLED) { %>
				<div class="block-order-regist-input-section" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-label">注文拡張項目</span>
						</h2>
					</div>
					<div class="block-order-regist-input-section-contents">
						<div id="dvOrderExtendErrorMessages" class="block-order-regist-input-section-error" runat="server" visible="false">
							<asp:Literal ID="lOrderExtendErrorMessages" runat="server" />
						</div>
						<asp:Repeater ID="rOrderExtendInput" ItemType="OrderExtendItemInput" runat="server">
							<ItemTemplate>
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										<%#: Item.SettingModel.SettingName %>
									</div>
									<div class="form-element-group-content" runat="server" visible="<%# Item.SettingModel.IsInputTypeText %>">
										<asp:TextBox runat="server" ID="tbSelect" class="w100" />
									</div>
									<div class="form-element-group-content" runat="server" visible="<%# Item.SettingModel.IsInputTypeDropDown %>">
										<asp:DropDownList runat="server" ID="ddlSelect" class="w100" />
									</div>
									<div class="form-element-group-content" runat="server" visible="<%# Item.SettingModel.IsInputTypeRadio %>">
										<asp:RadioButtonList runat="server" ID="rblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" class="w100" />
									</div>
									<div class="form-element-group-content" runat="server" visible="<%# Item.SettingModel.IsInputTypeCheckBox %>">
										<asp:CheckBoxList runat="server" ID="cblSelect" RepeatDirection="Horizontal" RepeatColumns="3" RepeatLayout="Flow" class="w100" />
									</div>
								</div>
								<%-- 検証文言 --%>
								<asp:HiddenField ID="hfSettingId" runat="server" Value="<%# Item.SettingModel.SettingId %>" />
								<asp:HiddenField ID="hfInputType" runat="server" Value="<%# Item.SettingModel.InputType %>" />
							</ItemTemplate>
						</asp:Repeater>
					</div>
				</div>
				<% } %>
				<!-- △注文拡張項目△-->
												<!-- ▽決済情報▽ -->
				<div class="block-order-regist-input-section payment-info" id="dvPayment" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-payment"></span>
							<span class="block-order-regist-input-section-title-label">決済情報</span>
							<%--▼▼ PayTg 端末状態保持用 ▼▼--%>
							<asp:HiddenField ID="hfCanUseDevice" runat="server" />
							<asp:HiddenField ID="hfStateMessage" runat="server" />
							<%--▲▲ PayTg 端末状態保持用 ▲▲--%>
						</h2>
					</div>
					<div id="dvHideOrderPayment" class="block-order-regist-input-section-contents" runat="server" style="text-align: left">
						<p class="note">注⽂者/商品情報確定後に表⽰されます</p>
					</div>
					<div id="dvShowOrderPayment" runat="server" visible="false">
						<div id="dvPaymentErrorMessages" class="block-order-regist-input-section-error" runat="server" visible="false">
							<asp:Literal ID="lPaymentErrorMessage" runat="server" />
						</div>
						<div class="block-order-regist-input-section-contents">
							<div class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									決済種別
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlOrderPaymentKbn" runat="server" OnSelectedIndexChanged="ddlOrderPaymentKbn_SelectedIndexChanged" AutoPostBack="true" Width="80%" /><br />
									<asp:Literal ID="lOrderPaymentInfo" runat="server" /><%#: string.IsNullOrEmpty(this.AccountEmail) ? "PayPalアカウントが紐づいていません。" : string.Format("ご利用可能なPayPal アカウント：{0}, ", this.AccountEmail) %>
															<%--▼▼ クレジット Token保持用 ▼▼--%>
															<asp:HiddenField ID="hfCreditToken" runat="server" />
															<asp:HiddenField ID="hfLastFourDigit" runat="server" />
															<asp:HiddenField ID="hfCreditBincode" runat="server" />
															<%--▲▲ クレジット Token保持用 ▲▲--%>
								</div>
							</div>
													<%-- ▽クレジット決済の場合は表示▽ --%>
							<div id="dvPaymentKbnCredit" runat="server" visible="false">
								<div class="form-element-group form-element-group-horizontal-grid">
									<div class="form-element-group-title">
										利用クレジットカード
													</div>
									<div class="form-element-group-content">
										<asp:DropDownList ID="ddlUserCreditCard" runat="server"
											DataSource="<%# this.CreditCardList %>"
											DataTextField="CardDispName"
											DataValueField="BranchNo"
											AutoPostBack="True"
											OnSelectedIndexChanged="ddlUserCreditCard_SelectedIndexChanged"
											Style="min-width: 12em; max-width: 12em;" />
									</div>
								</div>
								<div id="divUserCreditCard" runat="server" visible="false">
									<% if (OrderCommon.CreditCompanySelectable) { %>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title">
											カード会社
										</div>
										<div class="form-element-group-content">
											<asp:Literal ID="lCreditCompany" runat="server" />
										</div>
									</div>
									<% } %>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title">
											カード番号
										</div>
										<div class="form-element-group-content">
											************<asp:Literal ID="lCreditLastFourDigit" runat="server" />
										</div>
									</div>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title">
											有効期限
										</div>
										<div class="form-element-group-content">
											<asp:Literal ID="lCreditExpirationMonth" runat="server" />/<asp:Literal ID="lCreditExpirationYear" runat="server" />(月/年)
										</div>
									</div>
									<div class="form-element-group form-element-group-horizontal-grid">
										<div class="form-element-group-title">
											カード名義人
										</div>
										<div class="form-element-group-content" style="white-space: normal; word-break: break-all;">
											<asp:Literal ID="lCreditAuthorName" runat="server" />
										</div>
									</div>
								</div>
													<div id="divCreditCardInputNew" runat="server">
													<%--▼▼▼ カード情報入力フォーム表示 ▼▼▼--%>
									<% if (this.CanUseCreditCardNoForm) { %>
													<%--▼▼ カード情報入力（トークン未取得・利用なし） ▼▼--%>
													<div id="divCreditCardNoToken" runat="server">
										<% if (OrderCommon.CreditCompanySelectable) { %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												カード会社<span class="notice">*</span>
											</div>
											<div class="form-element-group-content">
												<asp:DropDownList ID="ddlCreditCardCompany" class="w100" runat="server" />
											</div>
										</div>
										<% } %>
										<asp:PlaceHolder ID="phCreditCardNotRakuten" runat="server">
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												<% if (this.CreditTokenizedPanUse) { %>永久トークン<% } else { %>カード番号<% } %><span class="notice">*</span>
											</div>
											<div id="tdCreditNumber" class="form-element-group-content" runat="server">
												<asp:TextBox ID="tbCreditCardNo1" pattern="[0-9]*" MaxLength="16" autocomplete="off" class="w100" runat="server" />
															<%--▼▼ カード情報取得用 ▼▼--%>
												<input type="hidden" id="hiddenCardInfo" name="hidCinfo" value="<%= CreateGetCardInfoJsScriptForCreditToken() %>" />
															<span id="spanErrorMessageForCreditCard" style="color: red; display: none" runat="server"></span>
															<%--▲▲ カード情報取得用 ▲▲--%>
											</div>
											<div id="tdGetCardInfo" class="form-element-group-content" runat="server">
												<asp:Button ID="btnGetCreditCardInfo" Text="  決済端末と接続  " class="btn btn-main btn-size-s" OnClick="btnGetCardInfo_Click" runat="server" />
												<br />
												<p>※決済端末と接続ボタンを押下したあと、決済端末でカード番号を入力してください。</p>
											</div>
											<div id="payTgModal" class="payTgModal">
												<div class="payTgModalOuter">
													<div class="payTgModalMargin"></div>
													<div class="payTgModalContents">
														<h1 style="font-size: 16px;">PayTG決済結果待機中・・・</h1>
														<br />
														<h1 style="font-size: 16px;">テンキー端末の操作を完了してください。</h1>
													</div>
												</div>
											</div>
										</div>
										<div id="trCreditExpire" class="form-element-group form-element-group-horizontal-grid" runat="server">
											<div class="form-element-group-title">
												有効期限<span class="notice">*</span>
											</div>
											<div class="form-element-group-content">
												<asp:DropDownList ID="ddlCreditExpireMonth" runat="server" />/<asp:DropDownList ID="ddlCreditExpireYear" runat="server" />
												(月/年)
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												カード名義人<span class="notice">*</span>
											</div>
											<div class="form-element-group-content">
												<asp:TextBox ID="tbCreditAuthorName" runat="server" class="w100" autocomplete="off" />
											</div>
										</div>
										<div id="dvSecurityCode" class="form-element-group form-element-group-horizontal-grid" runat="server">
											<div class="form-element-group-title">
												セキュリティコード<span class="notice">*</span>
											</div>
											<div class="form-element-group-content">
												<asp:TextBox ID="tbCreditSecurityCode" runat="server" class="w100" MaxLength="4" autocomplete="off" />
											</div>
										</div>
										</asp:PlaceHolder>
									</div>
													<%--▲▲ カード情報入力（トークン未取得・利用なし） ▲▲--%>

													<%--▼▼ カード情報入力（トークン取得済） ▼▼--%>
									<div id="divCreditCardForTokenAcquired" visible="false" runat="server">
										<% if (OrderCommon.CreditCompanySelectable) { %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												カード会社
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lCreditCardCompanyNameForTokenAcquired" runat="server" />
											</div>
										</div>
										<% } %>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												カード番号
											</div>
											<div class="form-element-group-content">
												************<asp:Literal ID="lLastFourDigitForTokenAcquired" runat="server" />
												<asp:LinkButton ID="lbEditCreditCardNoForToken" OnClick="lbEditCreditCardNoForToken_Click" runat="server" Text="  再入力  " />
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												有効期限
											</div>
											<div class="form-element-group-content">
												<asp:Literal ID="lExpirationMonthForTokenAcquired" runat="server" />
												/
											<asp:Literal ID="lExpirationYearForTokenAcquired" runat="server" />
												(月/年)
											</div>
										</div>
										<div class="form-element-group form-element-group-horizontal-grid">
											<div class="form-element-group-title">
												カード名義人
											</div>
											<div class="form-element-group-content" style="white-space: normal; word-break: break-all;">
												<asp:Literal ID="lCreditAuthorNameForTokenAcquired" runat="server" />
											</div>
										</div>
													</div>
													<%--▲▲ カード情報入力（トークン取得済） ▲▲ --%>
													<%--▲▲▲ カード情報入力フォーム表示 ▲▲▲--%>
									<% } else { %>
													<%--▼▼▼ カード情報入力フォーム非表示 ▼▼▼--%>
									<div class="form-element-group form-element-group-horizontal-grid">
										<p class="note" style="color: red; text-align: left">
															クレジットカード番号は入力できません。<br />
											<% if (this.NeedsRegisterProvisionalCreditCardCardKbn) { %>
												登録すると「<%: new PaymentService().Get(this.LoginOperatorShopId, Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID).PaymentName %>」として登録されます。
											<% } %>
										</p>
									</div>
									<% } %>
													<%--▲▲▲ カード情報入力フォーム非表示 ▲▲▲--%>
													</div>
													<%--▼▼▼ カード情報入力フォーム表示 ▼▼▼--%>
													<%-- ▽分割支払い有効の場合は表示▽ --%>
								<div id="dvInstallments" class="form-element-group form-element-group-horizontal-grid" runat="server" visible="false">
									<div class="form-element-group-title">
										支払回数<span class="notice">*</span>
									</div>
									<div class="form-element-group-content" style="padding-top: 2px;">
										<asp:DropDownList ID="dllCreditInstallments" runat="server" />
										<br />
										<p class="note">※AMEX/DINERSは一括のみとなります。</p>
									</div>
								</div>
													<%-- △分割支払い有効の場合は表示△ --%>
								<% if (this.CanUseCreditCardNoForm) { %>
								<div id="dvRegistCreditCard" class="form-element-group form-element-group-horizontal-grid" runat="server">
									<div class="form-element-group-title">
										登録する
									</div>
									<div class="form-element-group-content">
										<asp:CheckBox ID="cbRegistCreditCard" runat="server" Text="  登録する  " AutoPostBack="True"
											OnCheckedChanged="cbRegistCreditCard_CheckedChanged" />
									</div>
								</div>
								<div id="dvCreditCardName" class="form-element-group form-element-group-horizontal-grid" runat="server">
									<div class="form-element-group-title">
										クレジットカード登録名<span class="notice">*</span>
									</div>
									<div class="form-element-group-content">
										<asp:TextBox ID="tbUserCreditCardName" runat="server" MaxLength="30" /><br />
										※クレジットカードを保存する場合は、以下をご入力ください。
									</div>
								</div>
								<% } %>
													<%--▲▲▲ カード情報入力フォーム表示 ▲▲▲--%>
							</div>
							<div id="dvGmoCvsType" class="form-element-group form-element-group-horizontal-grid" runat="server">
								<div class="form-element-group-title">
									<%: StringUtility.ToHankaku("支払いコンビニ選択") %>
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlGmoCvsType" runat="server" Width="80%" />
								</div>
							</div>
							<div id="dvRakutenCvsType" class="form-element-group form-element-group-horizontal-grid" runat="server">
								<div class="form-element-group-title">
									支払いコンビニ選択
								</div>
								<div class="form-element-group-content">
									<asp:DropDownList ID="ddlRakutenCvsType" runat="server" Width="80%" />
								</div>
							</div>
							<div id="dvPaymentNoticeMessage" runat="server" visible="false" class="form-element-group form-element-group-horizontal-grid">
								<div class="form-element-group-title">
									<asp:Label runat="server" Text="注意喚起" ForeColor="red" />
								</div>
								<div class="form-element-group-content">
									<p>
										<asp:Label ID="lbPaymentUserManagementLevelMessage" runat="server" ForeColor="red" />
									</p>
									<p>
										<asp:Label ID="lbPaymentOrderOwnerKbnMessage" runat="server" ForeColor="red" />
									</p>
									<p>
										<asp:Label ID="lbPaymentLimitedMessage" runat="server" ForeColor="red" />
									</p>
								</div>
							</div>
						</div>
					</div>
												</div>
												<!-- △決済情報△ -->
				<%--▽注文同梱設定 ▽--%>
				<% if (Constants.ORDER_COMBINE_OPTION_ENABLED) { %>
				<div id="dvBeforeCombine" class="block-order-regist-input-section include-info" runat="server">
					<div class="block-order-regist-input-section-header">
						<h2 class="block-order-regist-input-section-title">
							<span class="block-order-regist-input-section-title-icon icon-box"></span>
							<span class="block-order-regist-input-section-title-label">注文同梱選択</span>
						</h2>
					</div>
					<div id="dvHideOrderCombine" class="block-order-regist-input-section-contents" runat="server" style="text-align: left">
						<p class="note">注⽂者/商品情報確定後に表⽰されます</p>
					</div>
					<div id="dvShowOrderCombine" visible="false" class="block-order-regist-input-section-contents" runat="server">
						<!-- ▽ 既存注文 ▽ -->
						<div class="block-order-regist-input-section-contents">
							<asp:CheckBox ID="cbOrderCombineSelect"
								Text="注文同梱可能既存注文一覧"
								Checked="false"
																	AutoPostBack="true"
								OnCheckedChanged="cbOrderCombineSelect_CheckedChangedExchange"
								runat="server" />
												<br />
						</div>
						<div id="dvNoneCombinable" class="block-order-regist-input-section-error" visible="false" runat="server">
							<asp:Literal ID="lOrderCombineAlertMessage" runat="server" />
						</div>
												<div>
							<div id="dvCombinable" class="order-history-section" runat="server" visible="false">
								<h3 class="order-title">注文同梱可能既存注文一覧</h3>
								<div id="dvOrderCombineSelectList" runat="server">
									<table class="user-info-table order ">
										<!-- ▽ テーブルヘッダ ▽ -->
														<tr>
											<th colspan="2">
												<div class="order-data">
													<span class="order-data-inner">
														<span class="order-data-row">
															<span class="order-data-date">注⽂⽇時</span>
														</span>
														<span class="order-data-row">
															<span class="order-data-id">注⽂ID</span>
														</span>
													</span>
													<span class="order-data-status">
														<span class="order-data-row">
															<span class="order-data-status-order-text">注⽂S</span>/<span class="order-data-status-payment-text">⼊⾦S</span>
														</span>
														<span class="order-data-row">
															<span class="order-data-status">注文区分</span>/<span class="order-data-status">決済種別</span>
														</span>
													</span>
												</div>
											</th>
														</tr>
														<tr>
											<th class="item-name">注⽂商品</th>
											<th class="total-price">合計⾦額</th>
														</tr>
													<tr>
											<td colspan="2" class="separate"></td>
													</tr>
										<!-- △ テーブルヘッダ △ -->
										<!-- ▽ 注文同梱可能注文リスト ▽ -->
										<asp:Repeater ID="rCombinableOrder" ItemType="OrderModel" runat="server">
														<ItemTemplate>
															<tr>
													<td colspan="2">
														<div class="order-data">
															<span class="order-data-inner">
																<span class="order-data-row">
																	<span class="order-data-date">
																		<%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
																	</span>
																</span>
																<span class="order-data-row">
																	<a href="javascript:open_window('<%#: CreateOrderDetailUrl(Item.OrderId, true, false) %>','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');" class="order-data-id">
																		<%#: Item.OrderId %>
																	</a>
																</span>
															</span>
															<span class="order-data-status">
																<span class="order-data-row">
																	<span class="order-data-status-order-icon <%#: GetCssClassForStatus(Item.OrderStatus, Constants.FIELD_ORDER_ORDER_STATUS) %>">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, Item.OrderStatus) %>
																	</span>
																	<span class="order-data-status-payment-icon <%#: GetCssClassForStatus(Item.OrderPaymentStatus, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS) %>">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Item.OrderPaymentStatus) %>
																	</span>
																</span>
																<span class="order-data-row">
																	<span class="order-data-status-payment-icon">
																		<%#: ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, Item.OrderKbn) %>
																	</span>
																	<span class="order-data-status-payment-icon">
																		<%#: ValueText.GetValueText(Constants.TABLE_PAYMENT, CONST_KEY_PAYMENT_TYPE, Item.OrderPaymentKbn) %>
																	</span>
																</span>
															</span>
														</div>
																</td>
															</tr>
													<tr>
													<td class="item-name">
														<ul>
															<li>
																<asp:Repeater ID="rOrderCombineItem" runat="server" DataSource="<%# Item.Items %>" ItemType="OrderItemModel">
													<ItemTemplate>
																		<a href="javascript:open_window('<%#: CreateProductDetailUrl(Item.ProductId, true) %>','productcontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');" class="order-data-id">
																			<%#: Item.ProductId %>
																		</a>
																		&nbsp;
																	<%#: Item.ProductName %><br />
													</ItemTemplate>
													</asp:Repeater>
															</li>
														</ul>
															</td>
													<td class="total-price">
														<span class="price-value"><%#: Item.OrderPriceTotal.ToPriceString(true) %></span>
														</td>
													</tr>
													<tr>
													<td colspan="2">
														<dl>
															<dt>配送先</dt>
															<dd>〒<%#: Item.Shippings.First().ShippingZip %>&nbsp;&nbsp;
															<%#: Item.Shippings.First().ShippingAddr1 %>
																<%#: Item.Shippings.First().ShippingAddr2 %>
																<%#: Item.Shippings.First().ShippingAddr3 %>
																<%#: Item.Shippings.First().ShippingAddr4 %>
															</dd>
															<dd>
																<%#: Item.Shippings.First().ShippingName %>
															</dd>
														</dl>
														<dl visible="<%# (Item.Shippings.First().ShippingDate != null) %>" runat="server">
															<dt>配送日時</dt>
															<dd>希望日：<%#: DateTimeUtility.ToStringForManager(Item.Shippings.First().ShippingDate, DateTimeUtility.FormatType.ShortDate2Letter) %></dd>
															<dd>時間帯：<%#: GetShippingTimeMessage(Item.Shippings.First().DeliveryCompanyId, Item.Shippings.First().ShippingTime) %></dd>
														</dl>
														<dl visible="<%# (string.IsNullOrEmpty(Item.FixedPurchaseId) == false) %>" runat="server">
															<dt>定期</dt>
															<dd>定期ID：
																<a href="javascript:open_window('<%#: FixedPurchasePage.CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId, true) %>','fixedpurchase','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');" class="order-data-id">
																	<%#: Item.FixedPurchaseId %>
																</a>
															</dd>
															<dd>配送周期：<%#: FixedPurchaseCombineUtility.GetFixedPachasePatternSettingMessage(Item.FixedPurchaseId) %></dd>
														</dl>
														<dl visible="<%# Item.Coupons.Any() %>" runat="server">
															<dt>クーポン</dt>
															<dd><%#: (Item.Coupons.Any() ? string.Format("利用あり（{0}）", Item.Coupons.First().CouponName) : string.Empty) %></dd>
														</dl>
														</td>
													</tr>
														<tr>
													<td colspan="2" class="combine-info-btn ta-right">
														<asp:Button ID="btnOrderCombine" class="combine-info-btn btn btn-main btn-size-s" runat="server" Text="  同梱する  " OnClick="btnOrderCombine_Click" />
														<asp:HiddenField ID="hfParentOrderId" Value="<%#: Item.OrderId %>" runat="server" />
															</td>
														</tr>
														<tr>
													<td colspan="2" class="separate"></td>
														</tr>
											</ItemTemplate>
										</asp:Repeater>
										<!-- △ 注文同梱可能注文リスト △ -->
												</table>
								</div>
							</div>
						</div>
						<!-- △ 既存注文 △ -->
					</div>
				</div>
												<% } %>
				<!-- △注文同梱設定△ -->
				<!-- ▽金額合計▽ -->
				<div class="fixed-bottom-area">
					<div class="total-price">
						<span class="total-price-title">金額合計</span>
						<span class="price-value">
							<asp:Literal ID="lOrderPriceTotalBottom" runat="server" /></span>
					</div>
													<asp:HiddenField ID="hfPayTgSendId" runat="server" />
													<asp:HiddenField ID="hfPayTgPostData" runat="server" />
													<asp:HiddenField ID="hfPayTgResponse" runat="server" />
					<asp:Button ID="btnProcessPayTgResponse" runat="server" Style="display: none;" OnClick="btnProcessPayTgResponse_Click" />
					<asp:Button ID="btnConfirm" class="fixed-bottom-area-btn btn btn-main btn-size-l" runat="server" Text="  確認  " OnClick="btnConfirm_Click" />
				</div>
				<!-- △金額合計△ -->
			</div>
		</div>
												</div>
<script type="text/javascript">
	var opneTableFlg = "0";
	var currentIndex = -1;
	$(function () {
		InitializeOrderShipping();
		$("#<%= cbAlowSaveNewAddress.ClientID %>").click(SetVisibleDvShippingName);
		$("#<%=ddlUserShipping.ClientID %>").change(function () {
			InitializeOrderShipping();
			<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
				__doPostBack('<%= btnRefreshPayment.UniqueID %>', '');
			<% } %>
		})

		setOrRemoveClassNeccessaryWhenChangeCountryForTextBox();
		$('.neccessary').filter(function () {
			if(this.value != '') {
				$(this).removeClass("required");
			} else {
				$(this).addClass("required");
		}
		});
		$(".neccessary").change(function (index) {
			if (this.value === '') {
				$(this).addClass("required");
			} else {
				$(this).removeClass("required");
		}
		});

		displayMemoPopup();
		SetVisibleDvShippingName();
		InitializeAdvCodeButton();
		setAutocompleteForTextBoxSearch();
	});

	// Initialize adv code button
	function InitializeAdvCodeButton() {
		var advName = $("#<%= lbAdvName.ClientID %>").text();
		if((advName === '')
			|| (advName === 'undefined')) {
			$("#<%= lbClearAdvCode.ClientID %>").attr("style", "display: none");
		} else {
			$("#<%= lbClearAdvCode.ClientID %>").attr("style", "display: block");
		}
	}

	// 初期化
	function InitializeOrderShipping() {
		var isShippingConvenience = false;
		if (document.getElementById('<%= dvOrderShipping.ClientID %>') != null) {
			var value = document.getElementById('<%= ddlShippingKbnList.ClientID %>').value;
			if (value == '<%= Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER %>') {
				document.getElementById('<%= dvShippingList.ClientID %>').style.display = 'none';
				$("#<%= dvRealStoreList.ClientID %>").hide();
				$("#<%= dvShippingInfo.ClientID %>").show();
				$(".shippingItem").hide();
				$("#<%= dvUserShipping.ClientID %>").hide();
				$("#<%=dvUserShippingList.ClientID %>").hide();
				$(".tbConvenienceStore").hide();
			} else if (value == '<%= Constants.SHIPPING_KBN_LIST_USER_INPUT %>') {
				document.getElementById('<%= dvShippingList.ClientID %>').style.display = '';
				$("#<%= dvRealStoreList.ClientID %>").hide();
				$("#<%= dvShippingInfo.ClientID %>").show();
				$("#<%= dvUserShipping.ClientID %>").show();
				$("#<%=dvUserShippingList.ClientID %>").show();
				var userShippingKbn = $("#<%= ddlUserShipping.ClientID %>").val();
				if (userShippingKbn == "CONVENIENCE_STORE") {
					$(".tbConvenienceStore").show();
					$(".shippingItem").hide();
					$("#<%= dvShippingTime.ClientID %>").hide();
					isShippingConvenience = true;
				} else if (userShippingKbn == "NEW") {
					$("#<%=dvOrderShipping.ClientID %>").show();
					$(".tbConvenienceStore").hide();
					$(".shippingItem").hide();
					$(".tbShippingItemNew").show();
					$(".btnCvsSearch").hide();
					$("#<%= dvShippingTime.ClientID %>").show();
					<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
					$("#<%= ddlShippingReceivingStoreType.ClientID %>").hide();
					<% } %>
				} else {
					var shippingNoConvenience = '<%= string.Join(",", this.UserShippingAddress.Where(item => item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON).Select(item => item.ShippingNo)) %>';
					var userShippingKbn = $(document.getElementsByClassName('UserShipping')).val();
					if(shippingNoConvenience.split(",").indexOf(userShippingKbn) != -1) {
						isShippingConvenience = true;
						$("#<%= dvShippingTime.ClientID %>").hide();
					} else {
						$("#<%= dvShippingTime.ClientID %>").show();
					}
					$(".tbConvenienceStore").hide();
					$("#<%=dvOrderShipping.ClientID %>").hide();
					$(".shippingItem").hide();
					$("#tbShippingItem_" + userShippingKbn).show();
					SetVisibleDvShippingName();
					$(".btnCvsSearch").hide();
					<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
					if (isShippingConvenience) {
						$("#<%= ddlShippingReceivingStoreType.ClientID %>").show();
					} else {
						$("#<%= ddlShippingReceivingStoreType.ClientID %>").hide();
					}
					<% } %>
				}
			} else {
				document.getElementById('<%= dvShippingList.ClientID %>').style.display = 'none';
				$("#<%= dvUserShipping.ClientID %>").hide();
				$("#<%= dvUserShippingList.ClientID %>").hide();
				$("#<%= dvShippingTime.ClientID %>").hide();
				$("#<%= dvOrderShipping.ClientID %>").hide();
				$(".shippingItem").hide();
				$(".tbConvenienceStore").hide();
				$("#<%= dvRealStoreList.ClientID %>").show();
				$("#<%= dvShippingInfo.ClientID %>").hide();
			}
		}
		$('#<%= hfDefineShippingConvenienceStore.ClientID %>').val(isShippingConvenience);
		if (($(document.getElementById('<%= dvOrderShippingErrorMessages.ClientID %>')).length > 0)
			&& ($(document.getElementById('<%= dvOrderOwnerErrorMessages.ClientID %>')).length == 0)) {
			if (document.getElementById('<%= tbShippingTel1_1.ClientID %>')) {
				document.getElementById('<%= tbShippingTel1_1.ClientID %>').focus();
			}
		}
	}
	<% if(Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED) { %>
	<%-- Open convenience store map popup --%>
	function openConvenienceStoreMapPopup() {
		var url = '<%= OrderCommon.CreateConvenienceStoreMapUrl() %>';
		window.open(url, "", "width=1000,height=800");
	}

	<%-- Set convenience store data --%>
	function setConvenienceStoreData(cvsspot, name, addr, tel) {
		var elements = document.getElementsByClassName('CONVENIENCE_STORE')[0];
		var userShippingKbn = $("#<%=ddlUserShipping.ClientID %>").val();
			if(userShippingKbn != 'CONVENIENCE_STORE') {
			elements = document.getElementById("tbShippingItem_" + userShippingKbn);
		}

		// For display
		elements.querySelector('[id$="tdCvsShopNo"] > span').innerHTML = cvsspot;
		elements.querySelector('[id$="tdCvsShopName"] > span').innerHTML = name;
		elements.querySelector('[id$="tdCvsShopAddress"] > span').innerHTML = addr;
		elements.querySelector('[id$="tdCvsShopTel"] > span').innerHTML = tel;

		// For get value
		elements.querySelector('[id$="hfCvsShopNo"]').value = cvsspot;
		elements.querySelector('[id$="hfCvsShopName"]').value = name;
		elements.querySelector('[id$="hfCvsShopAddress"]').value = addr;
		elements.querySelector('[id$="hfCvsShopTel"]').value = tel;
	}
	<% } %>

	<% if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) { %>
	<%-- Set convenience store Ec pay data --%>
		function setConvenienceStoreEcPayData(cvsspot, name, addr, tel) {
			if(cvsspot != "") {
			var elements = document.getElementsByClassName('CONVENIENCE_STORE')[0];

			// For display
			elements.querySelector('[id$="tdCvsShopNo"] > span').innerHTML = cvsspot;
			elements.querySelector('[id$="tdCvsShopName"] > span').innerHTML = name;
			elements.querySelector('[id$="tdCvsShopAddress"] > span').innerHTML = addr;
			elements.querySelector('[id$="tdCvsShopTel"] > span').innerHTML = tel;

			// For get value
			elements.querySelector('[id$="hfCvsShopNo"]').value = cvsspot;
			elements.querySelector('[id$="hfCvsShopName"]').value = name;
			elements.querySelector('[id$="hfCvsShopAddress"]').value = addr;
			elements.querySelector('[id$="hfCvsShopTel"]').value = tel;
		}
	}
	<% } %>

	// Open adv code list
	function open_advcode_list(window_name, window_type) {
		var link_file = '<%= CreateAdvCodeSearchUrl() %>'
		open_window(link_file, window_name, window_type);
		setTimeout(InitializeAdvCodeButton, 1000);
	}

	// Set adv code from list
	function set_advcode_from_list(adv_code, adv_name) {
		$('#<% =tbAdvCode.ClientID %>').val(adv_code);
		$('#<% =hfAdvName.ClientID %>').val(adv_name);
		$('#<% =lbAdvName.ClientID %>').text(adv_name);
		$('#<% =dvAdvCodeErrorMessage.ClientID %>').hide();
		$("#<% =lbClearAdvCode.ClientID %>").attr("style", "display:block");
		var ownerKbn = $('#<% =hfOwnerKbn.ClientID %>').val();
		if ($('#<%= hfUserId.ClientID %>').val() == "") {
			$.ajax({
				type: "POST",
				url: "<%= this.OrderRegistInputRootUrl %>/GetCorrectUserInfoByAdvCode",
				data: JSON.stringify({ advCode: adv_code, ownerKbn: ownerKbn }),
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				success: function(response) {
					var data = JSON.parse(response.d);
					var memberRankId = data.MemberRankId;
					var userManagementLevelId = data.UserManagementLevelId;
					$('#<% =hfMemberRankId.ClientID %>').val(memberRankId);
					$('#<% =ddlUserManagementLevel.ClientID %>').val(userManagementLevelId);
				}
			});
		}
	}

	// ユーザー検索アクション
	function action_user_search(user_id) {
		document.getElementById('<%= hfUserId.ClientID %>').value = user_id;
		__doPostBack('<%= btnGetUserData.UniqueID %>','');
	}

	// ユーザークーポンセット
	function action_set_coupon(coupon_code) {
		document.getElementById('<%= tbCouponUse.ClientID %>').value = coupon_code;
		__doPostBack('<%= btnApplyCoupon.UniqueID %>','');
	}

	// 再注文情報設定
	function action_set_reorder_data(order_id){
		document.getElementById('<%= hfReOrderId.ClientID %>').value = order_id;
		__doPostBack('<%= btnSetReOrderData.UniqueID %>', '');
	}

	// 表示非表示送料名
	function SetVisibleDvShippingName() {
		var saveNewChecked = $("#<%= cbAlowSaveNewAddress.ClientID %>").is(':checked');
		if (saveNewChecked) {
			$('#dvShippingName').show();
		} else {
			$('#dvShippingName').hide();
		}
	}

	// 再注文情報設定
	function set_reorder_data(order_id) {
		if (window.opener != null) {
			window.opener.action_set_reorder_data(order_id);
			window.close();
		}
	}

	// 再注文へ進む確認ボックス表示
	function confirm_reorder() {
		return confirm('再注文のため、注文情報登録画面へ遷移致します。\r\nよろしいでしょうか。');
	}

	// Set or remove class neccessary when change country for text box
	function setOrRemoveClassNeccessaryWhenChangeCountryForTextBox() {
		var tbOwnerAddr3 = document.getElementById("<%= tbOwnerAddr3.ClientID %>");
		var tbOwnerAddr4 = document.getElementById("<%= tbOwnerAddr4.ClientID %>");
		<% if (this.IsOwnerAddrJp == false) { %>
			tbOwnerAddr3.setAttribute("class", "w100");
			tbOwnerAddr4.setAttribute("class", "w100 neccessary");
		<% } %>
		<% else { %>
			tbOwnerAddr3.setAttribute("class", "w100 neccessary");
			tbOwnerAddr4.setAttribute("class", "w100");
		<% } %>

		var tbShippingAddr3 = document.getElementById("<%= tbShippingAddr3.ClientID %>");
		var tbShippingAddr4 = document.getElementById("<%= tbShippingAddr4.ClientID %>");
		<% if (this.IsShippingAddrJp == false) { %>
			tbShippingAddr3.setAttribute("class", "w100");
			tbShippingAddr4.setAttribute("class", "w100 neccessary");
		<% } else { %>
			tbShippingAddr3.setAttribute("class", "w100 neccessary");
			tbShippingAddr4.setAttribute("class", "w100");
		<% } %>

		var tbOwnerZipGlobal = document.getElementById("<%= tbOwnerZipGlobal.ClientID %>");
		if(tbOwnerZipGlobal != null) {
			<% if (this.IsOwnerAddrZipNecessary == false) { %>
				tbOwnerZipGlobal.setAttribute("class", "w100");
			<% } else { %>
				tbOwnerZipGlobal.setAttribute("class", "w100 neccessary");
			<% } %>
		}

		var tbShippingZipGlobal = document.getElementById("<%= tbShippingZipGlobal.ClientID %>");
		if(tbShippingZipGlobal != null) {
			<% if (this.IsShippingAddrZipNecessary == false) { %>
				tbShippingZipGlobal.setAttribute("class", "w100");
			<% } else { %>
				tbShippingZipGlobal.setAttribute("class", "w100 neccessary");
			<% } %>
		}
	}

	// Open user list
	function open_user_list(link_file, window_name, window_type) {
		open_window(link_file, window_name, window_type);
	}

	var selected_product_index = 0;

	// 商品一覧画面表示
	function open_product_list(link_file, window_name, window_type, index) {
		var fixed_purchase = '';
		var shipping_type_product_ids = '';
		<% foreach (RepeaterItem ri in rItemList.Items) { %>
			<%-- 定期が1つ以上存在する場合、設定「定期購入カート分離する？」に応じてパラメータを付与（TRUE:定期購入可, FALSE:ブランク）--%>
			<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
				if ($('#<%= ((CheckBox)ri.FindControl("cbFixedPurchase")).ClientID %>').prop('checked')) {
					fixed_purchase = '<%= Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION ? "1" : string.Empty %>';
				}
			<% } %>

			<%-- 商品IDをカンマ区切りで連結 --%>
		var product_id = $('#<%= ((TextBox)ri.FindControl("tbProductId")).ClientID %>').val();
		if (product_id != '') {
			if (shipping_type_product_ids != '') shipping_type_product_ids += ','
			shipping_type_product_ids += product_id;
		}
		var indexProduct = '<%= ri.ItemIndex %>';
			if(indexProduct == index) {
				var product_id_none = $('#<%= ((TextBox)ri.FindControl("tbProductIdNone")).ClientID %>').val();
				link_file += '&<%= Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID %>=' + product_id_none;
			}
		<% } %>
		link_file += '&<%= Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID %>=' + encodeURIComponent($('#<%= hfMemberRankId.ClientID %>').val());
		link_file += '&<%= Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT %>=' + encodeURIComponent(fixed_purchase);
		link_file += '&<%= Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS %>=' + encodeURIComponent(shipping_type_product_ids);

		// 選択商品を格納
		localStorage.setItem('index', index);

		// ウィンドウ表示
		open_window(link_file, window_name, window_type);
	}

	// 商品一覧で選択された商品情報を設定
	function set_productinfo(product_id, supplier_id, variation_id, product_name, product_display_price, product_special_price, product_price, sale_id, fixed_purchase_id, limitedfixedpurchasekbn1setting, limitedfixedpurchasekbn3setting, limitedfixedpurchasekbn4setting) {
		if (localStorage.getItem(['index']) != null) {
			selected_product_index = localStorage.getItem('index');
			localStorage.removeItem('index');
		}
		<%
		// 注文商品数分ループ
		int iLoop = 0;
		foreach (RepeaterItem ri in rItemList.Items)
		{
		%>
		var defaultInput= String.format("default-product-input-{0}", selected_product_index);
		var productInput= String.format("product-input-{0}", selected_product_index);
		var variationid = String.format("{0}{1}", product_id, variation_id);

		if (selected_product_index == '<%= iLoop %>') {
			$(document.getElementById(defaultInput)).hide();
			$(document.getElementById(productInput)).show();
			<% if (Constants.NOVELTY_OPTION_ENABLED) { %>
				// Set Novelty Id is Empty for OrderItem chose different OrderItem before
				if ((document.getElementById('<%= ((TextBox)ri.FindControl("tbProductId")).ClientID %>').value != product_id)
					|| (document.getElementById('<%= ((HiddenField)ri.FindControl("hfProductVariationId")).ClientID %>').value != variationid)) {
					document.getElementById('<%= ((HiddenField)ri.FindControl("hfNoveltyId")).ClientID %>').value = "";
				}
			<% } %>
			document.getElementById('<%= ((TextBox)ri.FindControl("tbProductId")).ClientID %>').value = product_id;
			document.getElementById('<%= ((HiddenField)ri.FindControl("hfSupplierId")).ClientID %>').value = supplier_id;
			document.getElementById('<%= ((HiddenField)ri.FindControl("hfProductVariationId")).ClientID %>').value = variationid;
			document.getElementById('<%= ((TextBox)ri.FindControl("tbProductName")).ClientID %>').value = product_name;
			<% if (HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false) { %>
			document.getElementById('<%= ((TextBox)ri.FindControl("tbProductPrice")).ClientID %>').value = product_price;
			<% } %>
			document.getElementById('<%= ((TextBox)ri.FindControl("tbItemQuantity")).ClientID %>').value = 1;
			<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
			document.getElementById('<%= ((CheckBox)ri.FindControl("cbFixedPurchase")).ClientID %>').checked = (fixed_purchase_id == "2") ? "checked" : "";
			document.getElementById('<%= ((HiddenField)ri.FindControl("hfLimitedFixedPurchaseKbn1Setting")).ClientID %>').value = limitedfixedpurchasekbn1setting;
			document.getElementById('<%= ((HiddenField)ri.FindControl("hfLimitedFixedPurchaseKbn3Setting")).ClientID %>').value = limitedfixedpurchasekbn3setting;
			<% } %>
			<% if (Constants.PRODUCT_SALE_OPTION_ENABLED) { %>
			document.getElementById('<%= ((TextBox)ri.FindControl("tbProductSaleId")).ClientID %>').value = sale_id;
			<% } %>
			document.getElementById("<%= hfProductItemSelectIndex.ClientID %>").value = selected_product_index;
			var btnProductOptionSettingButton = document.getElementById("<%= btnProductOptionValueSetting.ClientID %>");
			btnProductOptionSettingButton.click();
		}
		<%
			iLoop++;
		}
		%>
	}

	// Set autocomplete for text box search
	function setAutocompleteForTextBoxSearch() {
		autocomplete(document.getElementById("<%= tbUserOrderSearch.ClientID %>"), "<%= CONST_SEARCH_TYPE_USER %>");
		autocomplete(document.getElementById("<%= tbAdvCode.ClientID %>"), "<%= CONST_SEARCH_TYPE_ADVCODE %>");
		autocomplete(document.getElementById("<%= tbCouponUse.ClientID %>"), "<%= CONST_SEARCH_TYPE_COUPON %>");
		<% foreach (RepeaterItem ri in rItemList.Items) { %>
		autocomplete(document.getElementById("<%= ri.FindControl("tbProductIdNone").ClientID %>"), "<%= CONST_SEARCH_TYPE_PRODUCT %>");
		autocomplete(document.getElementById("<%= ri.FindControl("tbProductId").ClientID %>"), "<%= CONST_SEARCH_TYPE_PRODUCT %>");
		<% } %>
	}

	// Autocomplete
	function autocomplete(control, type) {
		// Disable autofill on a web form
		if (control == null) return;
		control.setAttribute("autocomplete", "off");

		// Execute function when input a keyword
		control.addEventListener("keydown", function (e) {
			controlSuggest(e, control, type);
		});
		control.addEventListener("blur", function (e) {
			if (opneTableFlg === "0") controlSuggest(e, control, type);
		});
		// Execute function when chose an item on autocomplete list
		document.addEventListener("click", function (e) {
			if (opneTableFlg === "0") {
				closeAllLists(e.target);
			}
		});
	}

	// サジェスト表示・操作
	function controlSuggest(e, control, type) {
		var queryExecFlag = false;
		var idHeaderExtend = control.id + "AutocompleteHeader";
		var idItemsExtend = control.id + "AutocompleteItems";
		var element = document.getElementById(idItemsExtend);
		if (element) { element = element.getElementsByTagName("table"); }
		if ((e.keyCode == 13 || e.type == "blur") && queryExecFlag == false) {
			queryExecFlag = true;
			var searchType = "";
			//サジェスト表示後のエンターキー処理
			e.preventDefault();
			if (currentIndex != -1) {
				if (element) {
					element[currentIndex].click();
					currentIndex = -1;
				}
			}
			var searchWord = control.value.trim();
			var elementControl = control;

			// Close already autocomplete list
			closeAllLists(elementControl);
			if (searchWord == "") return;

			// Create autocomplete header
			var header = document.createElement("div");
			header.setAttribute("id", idHeaderExtend);
			header.setAttribute("class", "autocomplete-header");

			// Create autocomplete items
			var items = document.createElement("div");
			items.setAttribute("id", idItemsExtend);
			items.setAttribute("class", "autocomplete-items scroll-vertical-autocomplete");
			var url;
			var sendData;
			switch (type) {
				case "<%= CONST_SEARCH_TYPE_ADVCODE %>":
					searchType = "広告コード";
					queryExecFlag = true;
					url = "<%: (this.OrderRegistInputRootUrl + "/SearchAdvCodesForAutosuggest") %>";
					sendData = JSON.stringify({ searchWord: searchWord });
					break;

				case "<%= CONST_SEARCH_TYPE_USER %>":
					searchType = "注文者情報";
					queryExecFlag = true;
					url = "<%: (this.OrderRegistInputRootUrl + "/SearchUsersForAutosuggest") %>";
					sendData = JSON.stringify({ searchWord: searchWord });
					break;

				case "<%= CONST_SEARCH_TYPE_COUPON %>":
					searchType = "クーポン";
					queryExecFlag = true;
					url = "<%: (this.OrderRegistInputRootUrl + "/SearchCouponsForAutosuggest") %>";
					sendData = JSON.stringify({ couponUserId: '<%= this.CouponUserId %>', couponMailAddress: '<%= this.CouponMailAddress ?? string.Empty %>', searchWord: searchWord });
					break;

				case "<%= CONST_SEARCH_TYPE_PRODUCT %>":
					searchType = "商品";
					queryExecFlag = true;
					url = "<%: (this.OrderRegistInputRootUrl + "/SearchProductsForAutosuggest") %>";
					sendData = JSON.stringify({ memberRankId: $('#<%= hfMemberRankId.ClientID %>').val(), searchWord: searchWord });
					break;
				}
				$.ajax({
					type: "POST",
					url: url,
					contentType: "application/json; charset=utf-8",
					dataType: "json",
					data: sendData,
					success: function (responce) {
						queryExecFlag = false;
						closeAllLists(elementControl);

						// Remove scroll vertical autocomplete
						var result = JSON.parse(responce.d).data;
						if (result.length == 0) return;

						if (result.length < parseInt('<%= CONST_MAX_DISPLAY_SHOW_FOR_SEARCH_AUTOCOMPLETE %>')) {
							items.classList.remove("scroll-vertical-autocomplete");
						}

						// Create match header of autocomplete list
						var rowHeader = "<tr class='suggestHeader'>";
						elementControl.parentNode.appendChild(header);
						switch (type) {
							case "<%= CONST_SEARCH_TYPE_ADVCODE %>":
								rowHeader += "<th style='width:40%'>広告コード</th>"
									+ "<th>媒体名</th>"
									+ "<th style='width:16px'>&times;</th>"
									+ "</tr>";
								break;

							case "<%= CONST_SEARCH_TYPE_USER %>":
								rowHeader += "<th style='width:30%'>ユーザーID</th>"
									+ "<th>ユーザー名</th>"
									+ "<th style='width:16px'>&times;</th>"
									+ "</tr>"
									+ "<tr>"
									+ "<th style='width: 200px'>電話番号</th>"
									+ "<th colspan='2' >番地</th>"
									+ "</tr>";
								break;

							case "<%= CONST_SEARCH_TYPE_PRODUCT %>":
								rowHeader += "<th style='width:55%'>商品ID</th>"
									+ "<th style='width:20%' rowspan='2'>単価</th>"
									+ "<th rowspan='2'>数量</th>"
									+ "<th style='width:16px; text-align:center;'>&times;</th>"
									+ "</tr>"
									+ "<tr>"
									+ "<th style='word-break: break-all; width: 250px;'>商品名</th>"
									+ "<th></th>"
									+ "</tr>";
								break;

							case "<%= CONST_SEARCH_TYPE_COUPON %>":
								rowHeader += "<th style='width:20%' rowspan='2'>クーポンコード</th>"
									+ "<th>クーポン名</th>"
									+ "<th style='width:15%' rowspan='2'>クーポン割引</th>"
									+ "<th style='width:150px' rowspan='2'>クーポン有効期間</th>"
									+ "<th rowspan='2' style='width:16px;'>&times;</th>"
									+ "</tr>"
									+ "<tr>"
									+ "<th>クーポン種別</th>"
									+ "</tr>";
								break;
						}
						var headerRow = document.createElement("table");
						headerRow.innerHTML = rowHeader;
						headerRow.addEventListener("click", function (e) {
							closeAllLists();
						});
						if (result.length > 0) {
							header.appendChild(headerRow);
						}

						// Create match items of autocomplete list
						for (var index = 0; index < result.length; index++) {
							var rowItems;
							elementControl.parentNode.appendChild(items);

							// Set row values
							switch (type) {
								case "<%= CONST_SEARCH_TYPE_ADVCODE %>":
									rowItems = "<tr>"
										+ "<td style='width:40%'>" + result[index].id + "</td>"
										+ "<td style='word-break: break-all;'>" + result[index].name + "</td>"
										+ "</tr>";
									break;

								case "<%= CONST_SEARCH_TYPE_USER %>":
									rowItems = "<tr>"
										+ "<td style='width:30%'>" + result[index].id + "</td>"
										+ "<td style='word-break: break-all'>" + result[index].name + "</td>"
										+ "</tr>"
										+ "<tr>"
										+ "<td style='width:200px'>" + result[index].phone + "</td>"
										+ "<td>" + result[index].address + "</td>"
										+ "</tr>";
									break;

								case "<%= CONST_SEARCH_TYPE_PRODUCT %>":
									var quantity = "";
									if (result[index].quantity != "") {
										quantity = result[index].quantity + "個";
									}
									rowItems = "<tr>"
										+ "<td style='width:55%'>" + result[index].id + "</td>"
										+ "<td style='width:20%; text-align:right;' rowspan='2'>" + result[index].unitPriceByKeyCurrency + "</td>"
										+ "<td style='text-align:right;' rowspan='2'>" + quantity + "</td>"
										+ "</tr>"
										+ "<tr>"
										+ "<td style='word-break: break-all; width: 250px;'>" + result[index].name + "</td>"
										+ "</tr>";
									break;

								case "<%= CONST_SEARCH_TYPE_COUPON %>":
									rowItems = "<tr>"
										+ "<td style='width:20%; vertical-align:top;' rowspan='2'>" + result[index].id + "</td>"
										+ "<td style='word-break: break-all;'>" + result[index].name + "</td>"
										+ "<td style='width:15%; vertical-align:top;' rowspan='2'>" + result[index].discount + "</td>"
										+ "<td style='width:150px; vertical-align:top;' rowspan='2'>" + result[index].expireDate + "まで有効</td>"
										+ "</tr>"
										+ "<tr>"
										+ "<td style='word-break:break-all'>" + result[index].type + "</td>"
										+ "</tr>";
									break;
							}

							var itemRow = document.createElement("table");
							itemRow.innerHTML = rowItems;
							itemRow.innerHTML += "<input type='hidden' value='" + JSON.stringify(result[index]) + "'>";
							itemRow.addEventListener("click", function (e) {
								setValue(elementControl, type, this.getElementsByTagName("input")[0].value);
								closeAllLists();
							});
							items.appendChild(itemRow);
						}
						opneTableFlg = "1";

						// Adjust height and width of autocomplete list
						var element = document.getElementById(elementControl.id);
						var itemsExtend = document.getElementById(idItemsExtend);
						var headerExtend = document.getElementById(idHeaderExtend);
						if ((itemsExtend !== null) && (headerExtend !== null)) {
							if (itemsExtend.clientWidth < element.clientWidth) {
								itemsExtend.style.width = element.clientWidth + "px";
							}
							headerExtend.style.width = itemsExtend.clientWidth + "px";
							itemsExtend.style.marginTop = headerExtend.clientHeight + "px";
						}
					},
					error: function () {
						queryExecFlag = false;
						var message = "<%= WebMessages.GetMessages(WebMessages.ERRMSG_ORDERREGISTINPUT_QUERY_TIMEOUT) %>";
						notification.show(message.replace('@@ 1 @@', searchType), 'warning', 'fadeout');
						pageReload;
					}
				});

				} else if (e.keyCode == 40) {
					// If arrow down key is pressed
					e.preventDefault();
					currentIndex++;
					// Add active class
					currentIndex = addActive(element, currentIndex);
				} else if (e.keyCode == 38) {
					// If arrow up key is pressed
					e.preventDefault();
					currentIndex--;
					// Add active class
					currentIndex = addActive(element, currentIndex);
				}
	}

	// Close autocomplete list
	function closeAllLists(element) {
		var autocompleteHeaders = document.getElementsByClassName("autocomplete-header");
		var autocompleteItems = document.getElementsByClassName("autocomplete-items");
		opneTableFlg = "0";

		// Close autocomplete headers
		for (var index = 0; index < autocompleteHeaders.length; index++) {
			if (element != autocompleteHeaders[index]) {
				autocompleteHeaders[index].parentNode.removeChild(autocompleteHeaders[index]);
			}
		}

		// Close autocomplete items
		for (var index = 0; index < autocompleteItems.length; index++) {
			if (element != autocompleteItems[index]) {
				autocompleteItems[index].parentNode.removeChild(autocompleteItems[index]);
			}
		}
	}

	// Add active class
	function addActive(element, currentIndex) {
		// Remove active class on all items
		removeActive(element);

		if (currentIndex < 0) { currentIndex = (element.length - 1); }
		if (currentIndex >= element.length) { currentIndex = 0; }

		// Add active class for current item
		element[currentIndex].classList.add("autocomplete-active");
		return currentIndex;
	}

	// Remove active class
	function removeActive(element) {
		for (var index = 0; index < element.length; index++) {
			element[index].classList.remove("autocomplete-active");
		}
	}

	// Set selected value
	function setValue(control, type, selectedValue) {
		var autoCompleteObject = JSON.parse(selectedValue)

		switch (type)
		{
			case "<%= CONST_SEARCH_TYPE_ADVCODE %>":
			set_advcode_from_list(autoCompleteObject.id, autoCompleteObject.name);
			break;

			case "<%= CONST_SEARCH_TYPE_USER %>":
				action_user_search(autoCompleteObject.id);
				break;

			case "<%= CONST_SEARCH_TYPE_PRODUCT %>":
				var indexId = "";
				if (control.id.endsWith("tbProductIdNone")) {
					indexId = control.id.replace("tbProductIdNone", "hfProductIdNoneSelectedIndex")
				} else {
					indexId = control.id.replace("tbProductId", "hfProductIdSelectedIndex")
				}
				selected_product_index = document.getElementById(indexId).value;
			localStorage.removeItem('index');

				set_productinfo(
					autoCompleteObject.id,
					autoCompleteObject.supplierId,
					autoCompleteObject.variationId,
					autoCompleteObject.name,
					autoCompleteObject.displayPrice,
					autoCompleteObject.specialPrice,
					autoCompleteObject.unitPrice,
					autoCompleteObject.saleId,
					autoCompleteObject.fixedPurchaseId,
					autoCompleteObject.limitedFixedPurchaseKbn1Setting,
					autoCompleteObject.limitedFixedPurchaseKbn3Setting);
				break;

			case "<%= CONST_SEARCH_TYPE_COUPON %>":
				action_set_coupon(autoCompleteObject.id)
				break;
		}
	}

	// Page reload
	function pageReload(xmlHttpRequest, status, error) {
		// Reload page when login session expired
		if (xmlHttpRequest.status == 401) {
			window.location.reload();
		}
		console.log(xmlHttpRequest.responseText);
		}

	// For case postback event
	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
		setAutocompleteForTextBoxSearch();
	});

	// Show confirm button area
	function showConfirmButtonArea() {
		$('.fixed-bottom-area').show();
	}
	
	// Hide confirm button area
	function hideConfirmButtonArea() {
		$('.fixed-bottom-area').hide();
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
</script>

<%--▼▼ クレジットカードToken用スクリプト ▼▼--%>
<%-- 戻る遷移のとき、テキストボックスがマスクされていたらポストバックさせる --%>
	<% if (OrderCommon.CreditTokenUse) { %>
<script type="text/javascript">
	var getTokenAndSetToFormJs = "<%= CreateGetCreditTokenAndSetToFormJsScript().Replace("\"", "\\\"") %>";
	var maskFormsForTokenJs = "<%= CreateMaskFormsForCreditTokenJsScript().Replace("\"", "\\\"") %>";

	// ページロード処理
	function bodyPageLoad(sender, args) {

		// 戻るボタンで戻ってきたとき、クレジットカード番号がマスキングされたままになるので再計算ボタンイベントを実行する
		var cis = GetCardInfo();
			if (cis
				&& cis[0]
				&& (cis[0].CardNo.indexOf("<%= Constants.CHAR_MASKING_FOR_TOKEN %>") != -1)
			&& $("#" + cis[0].TokenHiddenID).val()) {
				__doPostBack('<%= btnReCalculate.UniqueID %>', '');
		}
	}
</script>
<uc:CreditToken runat="server" ID="CreditToken" />
	<% } %>
<%--▲▲ クレジットカードToken用スクリプト ▲▲--%>

<script type="text/javascript">
	execAutoKanaWithKanaType(
		$("#<%= tbOwnerName1.ClientID %>"),
		$("#<%= tbOwnerNameKana1.ClientID %>"),
		$("#<%= tbOwnerName2.ClientID %>"),
		$("#<%= tbOwnerNameKana2.ClientID %>"));

	execAutoKanaWithKanaType(
		$("#<%= tbShippingName1.ClientID %>"),
		$("#<%= tbShippingNameKana1.ClientID %>"),
		$("#<%= tbShippingName2.ClientID %>"),
		$("#<%= tbShippingNameKana2.ClientID %>"));

	<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
	// Textbox change search owner zip global
	textboxChangeSearchGlobalZip(
		'<%= tbOwnerZipGlobal.ClientID %>',
		'<%= lbSearchAddrFromOwnerZipGlobal.UniqueID %>');

	// Textbox change search shipping zip global
	textboxChangeSearchGlobalZip(
		'<%= tbShippingZipGlobal.ClientID %>',
		'<%= lbSearchAddrFromShippingZipGlobal.UniqueID %>');
	<% } %>
</script>
</asp:Content>
