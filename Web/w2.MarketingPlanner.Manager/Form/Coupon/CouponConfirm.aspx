<%--
=========================================================================================================
  Module      : クーポン設定確認ページ(CouponConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CouponConfirm.aspx.cs" Inherits="Form_Coupon_CouponConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">クーポン設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr id="trDetail" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">クーポン設定詳細</h2></td>
	</tr>
	<tr id="trConfirm" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">クーポン設定確認</h2></td>
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
												<div id="divComp" runat="server" class="action_part_top" Visible="False">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left">クーポン設定を登録/更新しました。
															</td>
														</tr>
													</table>
												</div>
												<div class="action_part_top">
													<input id="btnGoBackTop" runat="server" type="button" Visible="False" onclick="Javascript: history.back();" value="  戻る  " />
													<asp:Button id="btnBackTop" runat="server" Text="  一覧へ戻る  " Visible="False" onclick="btnBack_Click"></asp:Button>
													<asp:Button id="btnEditTop" runat="server" Text="  編集する  " Visible="False" onclick="btnEditTop_Click"></asp:Button>
													<asp:Button id="btnCopyInsertTop" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsertTop_Click"></asp:Button>
													<asp:Button id="btnDeleteTop" runat="server" Text="  削除する  " Visible="False" onclick="btnDeleteTop_Click" OnClientClick="return confirm('削除してもよろしいですか？')" ></asp:Button>
													<asp:Button id="btnInsertTop" runat="server" Text="  登録する  " Visible="False" onclick="btnInsertTop_Click"></asp:Button>
													<asp:Button id="btnUpdateTop" runat="server" Text="  更新する  " Visible="False" onclick="btnUpdateTop_Click"></asp:Button>
												</div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">クーポンコード</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_couponInput.CouponCode) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">クーポン名(管理用)</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_couponInput.CouponName) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">クーポン名(ユーザ表示用)</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_couponInput.CouponDispName) %></td>
													</tr>
													<%-- クーポン名(ユーザ表示用)翻訳設定情報 --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater runat="server" ID="rTranslationCouponDispName"
														 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel"
														 DataSource="<%# this.CouponTranslationData.Where(d => d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COUPON_COUPON_DISP_NAME) %>">
													<ItemTemplate>
													<tr>
														<td class="detail_title_bg" align="left">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="detail_item_bg" align="left"><%#: Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr>
														<td class="detail_title_bg" align="left">クーポン説明文(管理用)</td>
														<td class="detail_item_bg" align="left"><%# StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(m_couponInput.CouponDiscription)) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">クーポン説明文(ユーザ表示用)</td>
														<td class="detail_item_bg" align="left"><%# StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(m_couponInput.CouponDispDiscription)) %></td>
													</tr>
													<%-- クーポン説明文(ユーザ表示用)翻訳設定情報  --%>
													<% if (Constants.GLOBAL_OPTION_ENABLE) { %>
													<asp:Repeater runat="server" ID="rTranslationCouponDispDescription"
														 ItemType="w2.Domain.NameTranslationSetting.NameTranslationSettingModel"
														 DataSource="<%# this.CouponTranslationData.Where(d => d.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_COUPON_COUPON_DISP_DISCRIPTION) %>">
													<ItemTemplate>
													<tr>
														<td class="detail_title_bg" align="left">　言語コード:<%#: Item.LanguageCode %> 言語ロケールID:<%#: Item.LanguageLocaleId %></td>
														<td class="detail_item_bg" align="left"><%#: Item.AfterTranslationalName %></td>
													</tr>
													</ItemTemplate>
													</asp:Repeater>
													<% } %>
													<tr>
														<td class="detail_title_bg" align="left">有効フラグ</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_VALID_FLG, m_couponInput.ValidFlg))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">発行条件</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">発行パターン</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_COUPON_TYPE, m_couponInput.CouponType))%></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">利用可能回数</td>
														<td class="detail_item_bg" align="left"><%# DisplayCouponCount(m_couponInput) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">発行期間</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_couponInput.PublishDateBgn, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %>
															～<%#: DateTimeUtility.ToStringForManager(m_couponInput.PublishDateEnd, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">クーポン割引設定</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncodeChangeToBr(DisplayDiscount(m_couponInput)) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">有効期限または有効期間</td>
														<td class="detail_item_bg" align="left"><%#: DisplayExpire(m_couponInput) %></td>
													</tr>
													<tr runat="server" visible="<%# IsDisplayUserDispFlg(m_couponInput.CouponType) %>">
														<td class="detail_title_bg" align="left">フロント表示</td>
														<td class="detail_item_bg" align="left"><%#: ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_DISP_FLG, m_couponInput.DispFlg) %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">クーポン利用条件</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">対象商品</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_PRODUCT_KBN, m_couponInput.ProductKbn)) %>
															<table class="detail_table" cellspacing="1" cellpadding="3" width="500" border="0" align="left">
																<tr>
																	<td class="detail_title_bg" align="left">キャンペーンアイコン</td>
																	<td class="detail_item_bg" align="left"><%#:DisplayExceptionalIcon(m_couponInput) %></td>
																</tr>
																<tr runat="server" id="trBrandId">
																	<td class="detail_title_bg" align="left" width="30%">ブランドID</td>
																	<td class="detail_item_bg" align="left"><%#: StringUtility.ChangeToBrTag(m_couponInput.ExceptionalBrandIds).Replace(",",", ") %></td>
																</tr>
																<tr runat="server" id="trCategoryId">
																	<td class="detail_title_bg" align="left" width="30%">カテゴリID</td>
																	<td class="detail_item_bg" align="left"><%#: StringUtility.ChangeToBrTag(m_couponInput.ExceptionalProductCategoryIds).Replace(",",", ") %></td>
																</tr>
																<tr>
																	<td class="detail_title_bg" align="left" width="30%"><%#: (m_couponInput.ProductKbn == Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND) ? "除外商品ID" : "商品ID" %></td>
																	<td class="detail_item_bg" align="left"><%#: StringUtility.ChangeToBrTag(m_couponInput.ExceptionalProduct).Replace(",",", ") %></td>
																</tr>
															</table>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left">他のクーポンとの併用</td>
														<td class="detail_item_bg" align="left">併用不可</td>
													</tr>													
													<tr>
														<td class="detail_title_bg" align="left">利用時の最低購入金額指定</td>
														<td class="detail_item_bg" align="left>
															<span id="spanUsablePriceOn" visible="<%# (m_couponInput.UsablePrice != null) %>" runat="server">商品合計が、<%# WebSanitizer.HtmlEncode(m_couponInput.UsablePrice.ToPriceString()) %>&nbsp;<%= (CurrencyManager.IsJapanKeyCurrencyCode) ? "円" : "" %>以上の場合にクーポンの利用を可能とする</span>
															<span id="spanUsablePriceOff" visible="<%# (m_couponInput.UsablePrice == null) %>" runat="server">指定なし</span>
														</td>
													</tr>													
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="detail_title_bg" align="left">作成日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_couponInput.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left">更新日</td>
														<td class="detail_item_bg" align="left"><%#: DateTimeUtility.ToStringForManager(m_couponInput.DateChanged, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="detail_title_bg" align="left">最終更新者</td>
														<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(m_couponInput.LastChanged) %></td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<input type="button" id="btnGoBackBottom" runat="server" Visible="False" onclick="Javascript: history.back();" value="  戻る  " />
													<asp:Button id="btnBackBottom" runat="server" Text="  一覧へ戻る  " Visible="False" onclick="btnBack_Click"></asp:Button>
													<asp:Button id="btnEditBottom" runat="server" Text="  編集する  " Visible="False" onclick="btnEditTop_Click"></asp:Button>
													<asp:Button id="btnCopyInsertBottom" runat="server" Text="  コピー新規登録する  " Visible="False" onclick="btnCopyInsertTop_Click"></asp:Button>
													<asp:Button id="btnDeleteBottom" runat="server" Text="  削除する  " Visible="False" onclick="btnDeleteTop_Click" OnClientClick="return confirm('削除してもよろしいですか？')" ></asp:Button>
													<asp:Button id="btnInsertBottom" runat="server" Text="  登録する  " Visible="False" onclick="btnInsertTop_Click"></asp:Button>
													<asp:Button id="btnUpdateBottom" runat="server" Text="  更新する  " Visible="False" onclick="btnUpdateTop_Click"></asp:Button>
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
</asp:Content>