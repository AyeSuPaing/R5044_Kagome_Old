<%--
=========================================================================================================
  Module      : クーポン設定登録ページ(CouponRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CouponRegister.aspx.cs" Inherits="Form_Coupon_CouponRegister" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content id="Content2" ContentPlaceHolderid="ContentPlaceHolderHead" Runat="Server">
	<meta http-equiv="Pragma" content="no-cache"/>
	<meta http-equiv="cache-control" content="no-cache"/>
	<meta http-equiv="expires" content="0"/>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">クーポン設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEdit" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">クーポン設定編集</h2></td>
	</tr>
	<tr id="trRegister" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">クーポン設定登録</h2></td>
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
											<td>
												<div class="action_part_top">
													<input type="button" onclick="Javascript: history.back();" value="  戻る  " />
													<asp:Button id="btnConfirmTop" runat="server" Text="  確認する  " onclick="btnConfirmTop_Click"></asp:Button>
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">クーポンコード<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbCouponCode" runat="server" Width="150" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">クーポン名(管理用)<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbCouponName" runat="server" Width="300" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">クーポン名(ユーザ表示用)</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbCouponDispName" runat="server" Width="300" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">クーポン説明文(管理用)</td>
															<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbCouponDiscription" runat="server" TextMode="MultiLine" Width="498" Rows="3"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">クーポン説明文(ユーザ表示用)</td>
															<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbCouponDispDiscription" runat="server" TextMode="MultiLine" Width="498" Rows="3"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">有効フラグ</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox id="cbValidFlg" runat="server" Checked="True" Text="有効"></asp:CheckBox></td>
														</tr>
													</tbody>
												</table>
												<br/>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">発行条件</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">
																発行パターン<span class="notice">*</span><br />
																(新規作成後の変更はできません)
															</td>
															<td class="edit_item_bg" align="left">
																<table class="edit_table" cellspacing="1" cellpadding="3" width="500" border="0" align="left">
																	<tr>
																		<td class="edit_title_bg" align="center" width="20"></td>
																		<td class="edit_title_bg" align="center" width="300">発行アクション</td>
																		<td class="edit_title_bg" align="center" width="90">利用可能対象</td>
																		<td class="edit_title_bg" align="center" width="90">利用可能回数</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeUserRegist" runat="server" GroupName="rbCouponType" Checked="true" /></td>
																		<td class="edit_item_bg" align="left">新規会員登録した人に発行</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">1回</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeBuy" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">購入した会員に発行</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">1回</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeFirstBuy" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">初回購入した会員に発行</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">1回</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeCreateLimitedgForRegisteredUser" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">利用回数制限付き会員用クーポンを管理者が作成</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">-</td>
																	</tr>
																	<% if (Constants.HIDE_SHIPPINGFREECOUPON_IN_COUPONREGISTER == false){ %>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeCreateLimitedFreeShippingForRegisteredUser" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">利用回数制限付き配送料無料会員用クーポンを管理者が作成</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">-</td>
																	</tr>
																	<% } %>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeCreateLimitedBirthdayForRegisteredUser" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">利用回数制限付き誕生日クーポンを管理者が作成</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">-</td>
																	</tr>
																	<% if (Constants.HIDE_SHIPPINGFREECOUPON_IN_COUPONREGISTER == false){ %>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeCreateLimitedBirthdayFreeShippingForRegisteredUser" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">利用回数制限付き配送料無料誕生日クーポンを管理者が作成</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">-</td>
																	</tr>
																	<% } %>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeCreateUnLimit" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">無制限利用可能クーポンを管理者が作成</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">無制限</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeCreateAllUnLimit" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">無制限利用可能クーポンを管理者が作成</td>
																		<td class="edit_item_bg" align="center">全て</td>
																		<td class="edit_item_bg" align="center">無制限</td>
																	</tr>
																	<%-- 利用回数制限ありクーポン --%>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeCreateLimit" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">回数制限付きクーポンを管理者が発行</td>
																		<td class="edit_item_bg" align="center">全て</td>
																		<td class="edit_item_bg" align="center"><asp:TextBox ID="tbCouponTypeCreateLimitCount" runat="server" MaxLength="5" Width="50"></asp:TextBox>回</td>
																	</tr>
																	<% if (Constants.HIDE_SHIPPINGFREECOUPON_IN_COUPONREGISTER == false){ %>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeLimitedFreeShipping" runat="server" GroupName="rbCouponType" /></td>
																		<td class="edit_item_bg" align="left">回数制限付き配送無料クーポンを管理者が発行</td>
																		<td class="edit_item_bg" align="center">全て</td>
																		<td class="edit_item_bg" align="center"><asp:TextBox ID="tbCouponLimitedFreeShipping" runat="server" MaxLength="5" Width="50"></asp:TextBox>回</td>
																	</tr>
																	<% } %>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeBlacklistForRegisteredUser" GroupName="rbCouponType" runat="server" /></td>
																		<td class="edit_item_bg" align="left">ブラックリスト型クーポンを管理者が発行</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">1人1回</td>
																	</tr>
																	<% if (Constants.HIDE_SHIPPINGFREECOUPON_IN_COUPONREGISTER == false){ %>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeBlacklistFreeShippingForRegisteredUser" GroupName="rbCouponType" runat="server" /></td>
																		<td class="edit_item_bg" align="left">ブラックリスト型配送無料クーポンを管理者が発行</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">1人1回</td>
																	</tr>
																	<% } %>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeBlacklistForAll" GroupName="rbCouponType" runat="server" /></td>
																		<td class="edit_item_bg" align="left">ブラックリスト型クーポンを管理者が発行</td>
																		<td class="edit_item_bg" align="center">全て</td>
																		<td class="edit_item_bg" align="center">1人1回</td>
																	</tr>
																	<% if (Constants.HIDE_SHIPPINGFREECOUPON_IN_COUPONREGISTER == false){ %>
																	<tr>
																		<td class="edit_item_bg" align="center"><asp:RadioButton ID="rbCouponTypeBlacklistFreeShippingForAll" GroupName="rbCouponType" runat="server" /></td>
																		<td class="edit_item_bg" align="left">ブラックリスト型配送無料クーポンを管理者が発行</td>
																		<td class="edit_item_bg" align="center">全て</td>
																		<td class="edit_item_bg" align="center">1人1回</td>
																	</tr>
																	<% } %>
																	<% if (Constants.INTRODUCTION_COUPON_OPTION_ENABLED) { %>
																	<tr>
																		<td class="edit_item_bg" align="center">
																			<asp:RadioButton ID="rbCouponTypeThanksForIntroducer" GroupName="rbCouponType" runat="server" />
																		</td>
																		<td class="edit_item_bg" align="left">紹介クーポン：紹介された人に発行</td>
																		<td class="edit_item_bg" align="center">全て</td>
																		<td class="edit_item_bg" align="center">1人1回</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="center">
																			<asp:RadioButton ID="rbCouponTypePurchaseGiveToIntroducer" GroupName="rbCouponType" runat="server" />
																		</td>
																		<td class="edit_item_bg" align="left">紹介クーポン：紹介された人が購入後に紹介した人に発行</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">1回</td>
																	</tr>
																	<tr>
																		<td class="edit_item_bg" align="center">
																			<asp:RadioButton ID="rbCouponTypeRegisterGiveToIntroducer" GroupName="rbCouponType" runat="server" />
																		</td>
																		<td class="edit_item_bg" align="left">紹介クーポン：紹介された人が会員登録後に紹介した人に発行</td>
																		<td class="edit_item_bg" align="center">会員のみ</td>
																		<td class="edit_item_bg" align="center">1回</td>
																	</tr>
																	<% } %>
																</table>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">発行期間<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<uc:DateTimePickerPeriodInput ID="ucPublishDatePeriod" runat="server" />
															</td>
														</tr>
														<tr id="trCouponDiscount">
															<td class="edit_title_bg" align="left">クーポン割引設定<span id="spDiscountNecessary" class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<span id="spProductDiscount">
																	<asp:RadioButton ID="rbDiscountPrice" GroupName="rbDiscount" Runat="server" Checked="true" ></asp:RadioButton>&nbsp;<asp:TextBox id="tbDiscountPrice" runat="server" Width="100"></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>&nbsp;(割引金額)<br />
																	<asp:RadioButton ID="rbDiscountRate" GroupName="rbDiscount" Runat="server" ></asp:RadioButton>&nbsp;<asp:TextBox id="tbDiscountRate" runat="server" Width="100" MaxLength="3" ></asp:TextBox>&nbsp;%&nbsp;(割引率)<br />
																	<asp:CheckBox id="cbFreeShipping" runat="server" Checked="false" Text="配送料無料割引"/>
																</span>
																<span id="spFreeShipping">配送無料</span>
															</td>
														</tr>
														<tr id="trExpire">
															<td class="edit_title_bg" align="left">有効期限または有効期間<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
															<asp:RadioButton ID="rbExpireDay" runat="server" CssClass="radio_button_list" Text="有効期限を指定する" GroupName="4" /><br />
															<span id="spanExpireDay">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;発行から<asp:TextBox id="tbExpireDay" runat="server" Text="" MaxLength="4" Width="50"></asp:TextBox>日間&nbsp;(当日を指定する場合は0日)<br /></span>
															<asp:RadioButton ID="rbExpireDate" runat="server" CssClass="radio_button_list" Text="有効期間を指定する" GroupName="4" />
															<span id="spanExpireDate">
																&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
																<uc:DateTimePickerPeriodInput ID="ucExpireDatePeriod" runat="server" />
															</span>
															</td>
														</tr>
														<tr id="trUserDispFlg">
															<td class="edit_title_bg" align="left">フロント表示</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox id="cbDispFlg" runat="server" Checked="false" Text="表示する"></asp:CheckBox></td>
														</tr>
													</tbody>
												</table>
												<br />
												<table id="tblProductTarget" class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">クーポン利用条件</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">対象商品<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButton ID="rbProductKbnTarget" runat="server" CssClass="radio_button_list" Text="全ての商品を対象" GroupName="rbProductKbn" Checked="true" />
																&nbsp;&nbsp;
																<asp:RadioButton ID="rbProductKbnUnTarget" runat="server" CssClass="radio_button_list" Text="対象商品を限定(または)" GroupName="rbProductKbn" />
																<a data-popover-message="設定した「キャンペーンアイコン」と「商品ID」を含む全ての商品が対象となります">[?]</a>
																&nbsp;&nbsp;
																<asp:RadioButton ID="rbProductKbnUnTargetByLogicalAnd" runat="server" CssClass="radio_button_list" Text="対象商品を限定(かつ)" GroupName="rbProductKbn" />
																<a data-popover-message="設定した「キャンペーンアイコン<%: Constants.PRODUCT_BRAND_ENABLED ? "・ブランドID" : "" %>・カテゴリID・除外商品IDではない」全てと一致する商品が対象となります">[?]</a>
																<br />
																<table class="info_table" cellspacing="1" cellpadding="3" width="500" border="0" align="left">
																	<tr id="trProductKbnUnTarget">
																		<td class="info_item_bg">除外対象の商品を指定する場合には、キャンペーンアイコンまたは、商品IDで指定してください </td>
																	</tr>
																	<tr id="trProductKbnTarget">
																		<td class="info_item_bg">対象とする商品を、キャンペーンアイコンまたは、商品IDで指定してください </td>
																	</tr>
																	<tr id="trProductKbnUnTargetByLogicalAnd">
																		<td class="info_item_bg">対象とする商品を、キャンペーンアイコン<% if (Constants.PRODUCT_BRAND_ENABLED) { %>、ブランドID<% } %>または、カテゴリIDで指定してください </td>
																	</tr>
																</table>
																<br /><br />
																<table class="edit_table" cellspacing="1" cellpadding="3" width="500" border="0" align="left">
																	<tr>
																		<td class="edit_title_bg" align="left">キャンペーンアイコン</td>
																		<td class="edit_item_bg" align="left">
																			<asp:CheckBox ID="cbExceptionalIco1" Runat="server" Text="1：" TextAlign="Left"></asp:CheckBox>&nbsp;
																			<asp:CheckBox ID="cbExceptionalIco2" Runat="server" Text="2：" TextAlign="Left"></asp:CheckBox>&nbsp;
																			<asp:CheckBox ID="cbExceptionalIco3" Runat="server" Text="3：" TextAlign="Left"></asp:CheckBox>&nbsp;
																			<asp:CheckBox ID="cbExceptionalIco4" Runat="server" Text="4：" TextAlign="Left"></asp:CheckBox>&nbsp;
																			<asp:CheckBox ID="cbExceptionalIco5" Runat="server" Text="5：" TextAlign="Left"></asp:CheckBox>&nbsp;<br />
																			<asp:CheckBox ID="cbExceptionalIco6" Runat="server" Text="6：" TextAlign="Left"></asp:CheckBox>&nbsp;
																			<asp:CheckBox ID="cbExceptionalIco7" Runat="server" Text="7：" TextAlign="Left"></asp:CheckBox>&nbsp;
																			<asp:CheckBox ID="cbExceptionalIco8" Runat="server" Text="8：" TextAlign="Left"></asp:CheckBox>&nbsp;
																			<asp:CheckBox ID="cbExceptionalIco9" Runat="server" Text="9：" TextAlign="Left"></asp:CheckBox>&nbsp;
																			<asp:CheckBox ID="cbExceptionalIco10" Runat="server" Text="10：" TextAlign="Left"></asp:CheckBox>&nbsp;
																		</td>
																	</tr>
																	<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
																	<tr id="trProductBrand">
																		<td class="edit_title_bg" align="left" width="30%">ブランドID<br />(カンマ区切り)</td>
																		<td class="edit_item_bg" align="left">
																			<div>
																				<asp:TextBox ID="tbProductBrand" runat="server" TextMode="MultiLine" width="330" Rows="6"></asp:TextBox>
																			</div>
																			<div>
																				<input type="button"
																						value="  検索  "
																						onclick="javascript:open_poppup_search(
																							'<%= WebSanitizer.UrlAttrHtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCT_BRAND_SEARCH)) %>',
																							'brand_search',
																							'width=850,height=670,top=120,left=420,status=NO,scrollbars=yes',
																							'<%= "" %>');"
																				/>
																			</div>
																		</td>
																	</tr>
																<% } %>
																	<tr id="trProductCategory">
																		<td class="edit_title_bg" align="left" width="30%">カテゴリID<br />(カンマ区切り)</td>
																		<td class="edit_item_bg" align="left">
																			<div>
																				<asp:TextBox ID="tbProductCategory" runat="server" TextMode="MultiLine" width="330" Rows="6"></asp:TextBox>
																			</div>
																			<div>
																				<input type="button"
																						value="  検索  "
																						onclick="javascript:open_poppup_search(
																							'<%= WebSanitizer.UrlAttrHtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCT_CATEGORY_SEARCH)) %>',
																							'category_search',
																							'width=850,height=670,top=120,left=420,status=NO,scrollbars=yes',
																							'<%= "" %>');"
																				/>
																			</div>
																		</td>
																	</tr>
																	<tr>
																		<td id="tdProductId" class="edit_title_bg" align="left" width="30%">商品ID<br />(カンマ区切り)</td>
																		<td class="edit_item_bg" align="left">
																			<div>
																				<asp:TextBox ID="tbExceptionalProduct" runat="server" TextMode="MultiLine" width="330" Rows="6"></asp:TextBox>
																			</div>
																			<div>
																				<input type="button"
																						value="  検索  "
																						onclick="javascript:open_poppup_search(
																							'<%= WebSanitizer.UrlAttrHtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_PRODUCT_SEARCH + "?" + Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN + "=" + Constants.KBN_PRODUCT_SEARCH_PRODUCT + "&" + Constants.REQUEST_KEY_PRODUCT_VALID_FLG + "=" + Constants.FLG_PRODUCTBRAND_VALID_FLG_VALID)) %>',
																							'product_search',
																							'width=850,height=670,top=120,left=420,status=NO,scrollbars=yes',
																							'<%= "" %>');"
																				/>
																			</div>
																		</td>
																	</tr>
																</table>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left">利用時の最低購入金額指定</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox id="cbUsablePrice" runat="server"></asp:CheckBox><span id="spanUsablePrice" visible="false">&nbsp;商品合計が、<asp:TextBox id="tbUsablePrice" runat="server" Text='' Width="100" ></asp:TextBox>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>以上の場合にクーポンの利用を可能とする</span></td>
														</tr>
													</tbody>
												</table>
												<br />
												<table class="info_table" width="758" border="0" cellspacing="1" cellpadding="3">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考（各項目の説明）<br />
															■利用回数制限付きクーポン<br />
															・ユーザー単位で枚数を指定して発行可能なクーポンです。<br />
															■利用回数制限付き誕生日クーポン<br />
															・ユーザー単位で枚数を指定して発行可能なクーポンです。<br />
															・発行対象ユーザの「最終誕生日クーポン発行年」を更新します。<br />
															※ターゲットリストで誕生日クーポン付与済みユーザーを除外する条件に利用できます。<br />
															■ブラックリスト型クーポン<br />
															・1ユーザーあたり1回のみ利用可能なクーポンです。<br />
															■対象商品の商品ID指定<br />
															・商品IDを複数入力する場合はカンマ「,」で区切って入力してください<br />
															■他クーポンとの併用<br />
															・注文する際、クーポンを併用して利用することはできません<br />
															■利用時の最低購入金額指定<br />
															・「商品合計」は、「対象商品の合計
															<% if (Constants.SETPROMOTION_OPTION_ENABLED || Constants.MEMBER_RANK_OPTION_ENABLED) { %>
																(
																<%= Constants.SETPROMOTION_OPTION_ENABLED ? "セットプロモーション割引" : "" %>
																<%= Constants.SETPROMOTION_OPTION_ENABLED && Constants.MEMBER_RANK_OPTION_ENABLED ? "、" : "" %>
																<%= Constants.MEMBER_RANK_OPTION_ENABLED ? "会員ランク割引" : ""%>
																後の金額)
															<% } %>」を指します。<br />
															■クーポンのフロント表示<br />
															・登録されたクーポンはフロント画面に表示されます。表示される画面および条件は以下のとおりです。<br />
																クーポンBOX画面<br />
																　会員が保有している利用可能なクーポンおよび利用しないで有効期限が切れたクーポンを表示<br />
																　※有効期限が切れたクーポンについては有効期限切れから何日経過分まで表示する設定かにより表示・非表示が変わります。<br />
																カートリスト画面・注文内容確認画面・ランディングページ<br />
																　表示されるクーポンは、会員が保有しているものでカートに対し利用可能なクーポン。<br />
																なお、両画面ともに管理者が発行するクーポンについてはフロント表示を表示するとしているもののみ表示されます。<br />
																また、管理者が発行するクーポンで対象が全てのクーポンをフロント表示とした場合、ゲストユーザーにも表示されます。<br />
															■発行期間<br />
															・新規会員登録した人に発行、購入した会員に発行、初回購入した会員に発行の3種の発行パターンの場合に発行期間を指定できます。<br />
															　必須登録のため、不要な発行パターンの場合は有効期間と同じ期間を入力してください。<br />
															■クーポン利用条件<br />
															・配送無料クーポンは、「全ての商品を対象」選択時に除外対象となる「対象商品」を含む注文には利用できません。<br />
															・配送無料クーポンは、「対象商品を限定する」選択時に「対象商品」にない商品を含む注文には利用できません。<br />
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<input type="button" onclick="Javascript: history.back();" value="  戻る  " />
													<asp:Button id="btnConfirmBottom" runat="server" Text="  確認する  " onclick="btnConfirmTop_Click"></asp:Button>
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
	<!--△ 登録 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// 初期設定
	function initialize()
	{
		change_discount();
		change_expire();
		change_exceptional();
		change_usableprice();
		change_dispflg();
	}

	// クーポン割引切替
	function change_discount()
	{
		<% if (Constants.HIDE_SHIPPINGFREECOUPON_IN_COUPONREGISTER == false) { %>
			// 配送料無料クーポンの場合
			if (document.getElementById("<%= rbCouponTypeLimitedFreeShipping.ClientID %>").checked
				|| document.getElementById("<%= rbCouponTypeBlacklistFreeShippingForRegisteredUser.ClientID %>").checked
				|| document.getElementById("<%= rbCouponTypeBlacklistFreeShippingForAll.ClientID %>").checked
				|| document.getElementById("<%= rbCouponTypeCreateLimitedFreeShippingForRegisteredUser.ClientID %>").checked
				|| document.getElementById("<%= rbCouponTypeCreateLimitedBirthdayFreeShippingForRegisteredUser.ClientID %>").checked)
			{
				document.getElementById("spProductDiscount").style.display = "none";
				document.getElementById("spFreeShipping").style.display = "";
				document.getElementById("spDiscountNecessary").style.display = "none";
			}
			else
			{
				document.getElementById("spProductDiscount").style.display = "";
				document.getElementById("spFreeShipping").style.display = "none";
				document.getElementById("spDiscountNecessary").style.display = "";
			}
		<% } else { %>
			if (document.getElementById("<%= rbCouponTypeUserRegist.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeBuy.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeFirstBuy.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeCreateLimitedgForRegisteredUser.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeCreateLimitedBirthdayForRegisteredUser.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeCreateUnLimit.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeCreateAllUnLimit.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeCreateLimit.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeBlacklistForRegisteredUser.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeBlacklistForAll.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeThanksForIntroducer.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypePurchaseGiveToIntroducer.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeRegisterGiveToIntroducer.ClientID %>").checked === false) {
				document.getElementById("spProductDiscount").style.display = "none";
				document.getElementById("spFreeShipping").style.display = "";
				document.getElementById("spDiscountNecessary").style.display = "none";
			} else {
				document.getElementById("spProductDiscount").style.display = "";
				document.getElementById("spFreeShipping").style.display = "none";
				document.getElementById("spDiscountNecessary").style.display = "";
			}
		<% } %>
	}
	
	// 有効期限・期間切替
	function change_expire()
	{
		// 有効期限の場合
		if (document.getElementById("<%= rbExpireDay.ClientID %>").checked)
		{
			document.getElementById("spanExpireDay").style.display = "block";
			document.getElementById("spanExpireDate").style.display = "none";
		}
		// 有効期間の場合
		else if (document.getElementById("<%= rbExpireDate.ClientID %>").checked)
		{
			document.getElementById("spanExpireDay").style.display = "none";
			document.getElementById("spanExpireDate").style.display = "block";
		}
		else
		{
			document.getElementById("spanExpireDay").style.display = "none";
			document.getElementById("spanExpireDate").style.display = "none";
		}
	}
	
	is_change = false;
	// 対象商品切替
	function change_exceptional()
	{
		// 全ての商品を対象の場合
		if (document.getElementById("<%= rbProductKbnTarget.ClientID %>").checked)
		{
			document.getElementById("trProductKbnTarget").style.display = "none";
			document.getElementById("trProductKbnUnTarget").style.display = "";
			document.getElementById("trProductKbnUnTargetByLogicalAnd").style.display = "none";
			<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
			document.getElementById("trProductBrand").style.display = "none";
			<% } %>
			document.getElementById("trProductCategory").style.display = "none";
			document.getElementById("tdProductId").innerText = "商品ID\n(カンマ区切り)";
		}
		// 対象商品を限定する（または）場合
		else if (document.getElementById("<%= rbProductKbnUnTarget.ClientID %>").checked) {
			document.getElementById("trProductKbnTarget").style.display = "";
			document.getElementById("trProductKbnUnTarget").style.display = "none";
			document.getElementById("trProductKbnUnTargetByLogicalAnd").style.display = "none";
			<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
			document.getElementById("trProductBrand").style.display = "none";
			<% } %>
			document.getElementById("trProductCategory").style.display = "none";
			document.getElementById("tdProductId").innerText = "商品ID\n(カンマ区切り)";
		}
		// 対象商品を限定する（かつ）場合
		else
		{
			document.getElementById("trProductKbnTarget").style.display = "none";
			document.getElementById("trProductKbnUnTarget").style.display = "none";
			document.getElementById("trProductKbnUnTargetByLogicalAnd").style.display = "";
			<% if (Constants.PRODUCT_BRAND_ENABLED) { %>
			document.getElementById("trProductBrand").style.display = "";
			<% } %>
			document.getElementById("trProductCategory").style.display = "";
			document.getElementById("tdProductId").innerText = "除外商品ID\n(カンマ区切り)";
		}

		if (is_change) {
			//切替する際にTextBoxとCheckBoxをリセットする
			document.getElementById("<%= tbProductCategory.ClientID %>").value = "";
			document.getElementById("<%= tbProductBrand.ClientID %>").value = "";
			document.getElementById("<%= tbExceptionalProduct.ClientID %>").value = "";
			document.getElementById("<%= cbExceptionalIco1.ClientID %>").checked = false;
			document.getElementById("<%= cbExceptionalIco2.ClientID %>").checked = false;
			document.getElementById("<%= cbExceptionalIco3.ClientID %>").checked = false;
			document.getElementById("<%= cbExceptionalIco4.ClientID %>").checked = false;
			document.getElementById("<%= cbExceptionalIco5.ClientID %>").checked = false;
			document.getElementById("<%= cbExceptionalIco6.ClientID %>").checked = false;
			document.getElementById("<%= cbExceptionalIco7.ClientID %>").checked = false;
			document.getElementById("<%= cbExceptionalIco8.ClientID %>").checked = false;
			document.getElementById("<%= cbExceptionalIco9.ClientID %>").checked = false;
			document.getElementById("<%= cbExceptionalIco10.ClientID %>").checked = false;
		}

		is_change = true;
	}

	// 利用時の最低購入金額指定切替
	function change_usableprice()
	{
		// 利用時の最低購入金額指定が指定されている場合
		if (document.getElementById("<%= cbUsablePrice.ClientID %>").checked)
		{
			document.getElementById("spanUsablePrice").style.display = "inline-block";
		}
		else
		{
			document.getElementById("spanUsablePrice").style.display = "none";
		}
	}

	// ユーザ表示フラグ切替
	function change_dispflg() {
		<% if (Constants.HIDE_SHIPPINGFREECOUPON_IN_COUPONREGISTER == false){ %>
			// 管理者発行クーポンの場合
			if (document.getElementById("<%= rbCouponTypeCreateUnLimit.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeCreateAllUnLimit.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeCreateLimit.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeLimitedFreeShipping.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeCreateLimitedgForRegisteredUser.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeCreateLimitedFreeShippingForRegisteredUser.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeCreateLimitedBirthdayForRegisteredUser.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeCreateLimitedBirthdayFreeShippingForRegisteredUser.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeBlacklistForRegisteredUser.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeBlacklistFreeShippingForRegisteredUser.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeBlacklistForAll.ClientID %>").checked ||
				document.getElementById("<%= rbCouponTypeBlacklistFreeShippingForAll.ClientID %>").checked
				|| document.getElementById("<%= rbCouponTypeThanksForIntroducer.ClientID %>").checked
				|| document.getElementById("<%= rbCouponTypePurchaseGiveToIntroducer.ClientID %>").checked
				|| document.getElementById("<%= rbCouponTypeRegisterGiveToIntroducer.ClientID %>").checked) {
				document.getElementById("trUserDispFlg").style.display = "";
			} else {
				document.getElementById("trUserDispFlg").style.display = "none";
			}
		<% } else { %>
			// 管理者発行クーポンの場合
			if (document.getElementById("<%= rbCouponTypeUserRegist.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeBuy.ClientID %>").checked === false &&
				document.getElementById("<%= rbCouponTypeFirstBuy.ClientID %>").checked === false) {
				document.getElementById("trUserDispFlg").style.display = "";
			} else {
				document.getElementById("trUserDispFlg").style.display = "none";
			}
		<% } %>
	}

	// クーポン割引切替およびユーザ表示フラグ切替
	function change_discount_and_dispflg() {
		change_discount();
		change_dispflg();
	}

	function set_categoryinfo(category_id, name) {
		
		document.getElementById('tbCategoryId').value = category_id;
	}

	// カテゴリ一覧画面表示
	function open_poppup_search(link_file, window_name, window_type, index) {
		// 選択商品を格納
		selected_index = index;
		// ウィンドウ表示
		open_window(link_file, window_name, window_type);
	}

	// 選択された商品カテゴリ情報を設定
	function set_categoryinfo(category_id, name) {
		var tbProductCategory = document.getElementById("<%= tbProductCategory.ClientID %>");

		// 重複する値を確認してください
		var mySplitResult = tbProductCategory.value.split(",");
		var isNotDuplicate = true;
		for( var i = 0; i < mySplitResult.length; i++){
			if( category_id == mySplitResult[i])
			{
				isNotDuplicate = false;
			}
		}
		if(isNotDuplicate)
		{
			if(tbProductCategory.value == "")
			{
				tbProductCategory.value += category_id;
			}
			else
			{
				tbProductCategory.value += "," + category_id;
			}
		}
	}

	// 選択された商品ブランド情報を設定
	function set_brandinfo(brand_id) {
		var tbProductBrand = document.getElementById("<%= tbProductBrand.ClientID %>");

		// 重複する値を確認してください
		var mySplitResult = tbProductBrand.value.split(",");
		var isNotDuplicate = true;
		for( var i = 0; i < mySplitResult.length; i++){
			if( brand_id == mySplitResult[i])
			{
				isNotDuplicate = false;
			}
		}
		if(isNotDuplicate)
		{
			if(tbProductBrand.value == "")
			{
				document.getElementById("<%= tbProductBrand.ClientID %>").value += brand_id;
			}
			else
			{
				document.getElementById("<%= tbProductBrand.ClientID %>").value += "," + brand_id;
			}
		}
	}

	// 選択された商品情報を設定
	function set_productinfo(product_id, supplier_id, v_id, product_name, display_price, display_special_price, product_price, sale_id, fixed_purchase_id, limitedfixedpurchasekbn1setting, limitedfixedpurchasekbn3setting, tax_rate, shipping_id) {
		var tbExceptionalProduct = document.getElementById("<%= tbExceptionalProduct.ClientID %>");

		// 重複する値を確認してください
		var mySplitResult = tbExceptionalProduct.value.split(",");
		var isNotDuplicate = true;
		for( var i = 0; i < mySplitResult.length; i++){
			if( product_id == mySplitResult[i])
			{
				isNotDuplicate = false;
			}
		}
		if(isNotDuplicate)
		{
			if(tbExceptionalProduct.value == "")
			{
				tbExceptionalProduct.value += product_id;
			}
			else
			{
				tbExceptionalProduct.value += "," + product_id;
			}
		}
	}
//-->
</script>
</asp:Content>