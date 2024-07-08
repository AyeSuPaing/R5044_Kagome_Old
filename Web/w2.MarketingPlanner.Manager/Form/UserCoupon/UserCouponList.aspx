<%--
=========================================================================================================
  Module      : ユーザクーポン情報一覧ページ(UserCouponList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Manager" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="UserCouponList.aspx.cs" Inherits="Form_UserCoupon_UserCouponList" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="w2.Domain.MenuAuthority.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">ユーザークーポン情報</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<%--▽ 編集 ▽--%>
	<tr id="trDetail" runat="server">
		<td><h2 class="cmn-hed-h2">ユーザクーポン情報編集</h2></td>
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
													<input type="button" onclick="Javascript:location.href='<%= CreateUserCouponHistoryListUrl() %>';" value="  履歴一覧へ  " />
													<input type="button" onclick="Javascript:location.href='<%= CreateUserListUrl() %>';" value="  一覧へ戻る  " />
												</div>
												<%-- 基本情報 --%>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">ユーザID</td>
														<td class="detail_item_bg" align="left">
															<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
																<a href="#" onclick="javascript:open_window('<%= WebSanitizer.HtmlEncode(SingleSignOnUrlCreator.CreateForWebForms(MenuAuthorityHelper.ManagerSiteType.Ec, CreateUserDetailUrl(this.UserId))) %>','userdetail','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');"><%: this.UserId %></a>
															<% } else { %>
																<%#: this.UserId %>
															<% } %>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">顧客区分</td>
														<td class="detail_item_bg" align="left"><asp:Literal ID="lUserKbn" runat="server"></asp:Literal></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%"><%: ReplaceTag("@@User.name.name@@") %></td>
														<td class="detail_item_bg" align="left"><asp:Literal ID="lName" runat="server"></asp:Literal></td>
													</tr>
												</table>
												<br />
												<%-- クーポン発行 --%>
												<div runat="server" id="divErrorMessage" class="notice"><%= this.ErrorMessage %></div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">ユーザー向けクーポン発行</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">クーポン選択</td>
														<td class="detail_item_bg" align="left"><asp:DropDownList ID="ddlCoupon" runat="server" OnSelectedIndexChanged="ddlCoupon_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>&nbsp;&nbsp;<asp:Button ID="btnPublish" runat="server" Text="  クーポン発行  " OnClick="btnPublish_OnClick" OnClientClick="javascript:return check_selected_coupon();" /><span class="notice"><asp:Literal ID="lError" runat="server"></asp:Literal></span></td>
													</tr>
													<tr runat="server" id="trCouponQuantity"><%-- 選択中のクーポン種別によって表示制御 --%>
														<td class="detail_title_bg" align="left" width="30%">発行枚数<span class="notice">*</span></td>
														<td class="detail_item_bg" align="left"><asp:TextBox ID="tbCouponQuantity" runat="server"></asp:TextBox></td>
													</tr>
													<%-- その他履歴系と纏めて対応 --%>
													<!--
													<tr>
														<td class="detail_title_bg" align="left" width="30%">発行理由</td>
														<td class="detail_item_bg" align="left"><asp:TextBox ID="tbMemo" runat="server" TextMode="MultiLine" Columns="50" Rows="3"></asp:TextBox></td>
													</tr>
													-->
												</table>
												<br />
												<%-- 利用可能クーポン情報 --%>
												<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="list_title_bg">
														<td align="center" colspan="6">利用可能クーポン情報</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center" width="120" rowspan="2">クーポンコード</td>
														<td align="center" width="318">クーポン名(管理用)</td>
														<td align="center" width="100" rowspan="2">割引金額/率</td>
														<td align="center" width="190">発行日</td>
														<td align="center" width="40" rowspan="2">枚数</td>
														<td align="center" width="50" rowspan="2"></td>
													</tr>
													<tr class="list_title_bg">
														<td align="center">発行パターン</td>
														<td align="center">有効期間</td>
													</tr>
													<asp:Repeater id="rList" ItemType="w2.Domain.Coupon.Helper.UserCouponDetailInfo" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																<td align="center"><a href="<%# CreateCouponDetailUrl(((UserCouponDetailInfo)Container.DataItem).CouponId) %>"><%# WebSanitizer.HtmlEncode(((UserCouponDetailInfo)Container.DataItem).CouponCode) %></a></td>
																<td align="center" style="padding:0px">
																	<table cellpadding="0" cellspacing="0" width="100%">
																		<tr>
																			<td align="left" class="item_bottom_line"><%# WebSanitizer.HtmlEncode(((UserCouponDetailInfo)Container.DataItem).CouponName)%></td>
																		</tr>
																		<tr>
																			<td align="left"><%# WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_COUPON, Constants.FIELD_COUPON_COUPON_TYPE, ((UserCouponDetailInfo)Container.DataItem).CouponType))%></td>
																		</tr>
																	</table>
																</td>
																<td align="center"><%# WebSanitizer.HtmlEncodeChangeToBr(DisplayDiscount((UserCouponDetailInfo)Container.DataItem))%></td>
																<td align="center" style="padding:0px">
																	<table cellpadding="0" cellspacing="0" width="100%">
																		<tr>
																			<td align="center" class="item_bottom_line"><%# WebSanitizer.HtmlEncode(DisplayDateCreated((UserCouponDetailInfo)Container.DataItem))%></td>
																		</tr>
																		<tr>
																			<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.ExpireBgn, DateTimeUtility.FormatType.ShortDate2Letter) + "～" + DateTimeUtility.ToStringForManager(Item.ExpireEnd, DateTimeUtility.FormatType.ShortDate2Letter) %><span class="notice"><%# (Item.ExpireDateBgn != null) ? "*" : "" %></span></td>
																		</tr>
																	</table>
																</td>
																<td align="center"><%# WebSanitizer.HtmlEncode(((UserCouponDetailInfo)Container.DataItem).UserCouponCount)%></td>
																<td align="center">
																	<asp:Button ID="btnDelete" runat="server" Text="  削除  " OnClick="btnDelete_Click" Enabled="<%# DisplayDeleteButton((UserCouponDetailInfo)Container.DataItem) %>" OnClientClick="return confirm('クーポンを削除してもよろしいですか？');" />
																	<asp:HiddenField ID="hfCouponId" runat="server" Value="<%# ((UserCouponDetailInfo)Container.DataItem).CouponId %>" />
																	<asp:HiddenField ID="hfCouponNo" runat="server" Value="<%# ((UserCouponDetailInfo)Container.DataItem).CouponNo %>" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="false">
														<td id="tdErrorMessage" runat="server" colspan="6"></td>
													</tr>
												</table>
												<table cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
													</tr>
												</table>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td align="left" class="info_item_bg" colspan="2">備考<br/>
														・有効期間の<span class="notice">*</span>は、時分秒の指定があります。<br/>
														・クーポン発行機能で発行できるクーポンは次の通りです。<br />
														　１．「新規会員登録した人に発行」クーポン<br />
														　２．「購入した会員に発行」クーポン<br />
														　３．「初回購入した会員に発行」クーポン<br />
														　４．利用回数制限つきの会員用クーポン<br/>
															※利用回数制限つきの会員用クーポンの場合、「発行枚数」を指定する必要があります。
														</td>
													</tr>
												</table>													
												<div class="action_part_bottom">
													<input type="button" onclick="Javascript:location.href='<%= CreateUserCouponHistoryListUrl() %>';" value="  履歴一覧へ  " />
													<input type="button" onclick="Javascript:location.href='<%= CreateUserListUrl() %>';" value="  一覧へ戻る  " />
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
	<%--△ 編集 △--%>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// クーポン選択チェック
	function check_selected_coupon()
	{
		// クーポン指定なし
		if (document.getElementById("<%= ddlCoupon.ClientID %>").options[document.getElementById("<%= ddlCoupon.ClientID %>").selectedIndex].value == '')
		{
			alert('クーポンを選択してください。');
			return false;
		}
		else
		{
			return confirm('クーポンを発行してもよろしいですか？');
		}
	}
//-->
</script>
</asp:Content>
