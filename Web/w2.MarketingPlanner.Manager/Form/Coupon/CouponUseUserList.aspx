<%--
=========================================================================================================
  Module      : クーポン利用ユーザー一覧ページ(CouponUseUserList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="CouponUseUserList.aspx.cs" Inherits="Form_Coupon_CouponUseUserList" %>
<%@ Import Namespace="w2.Domain.Coupon.Helper" %>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="DateTimeInput" Src="~/Form/Common/DateTimeInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">クーポンOP</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<%--▽ 検索 ▽--%>
	<tr>
		<td>
		<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
		<tr>
		<td class="search_box_bg">
		<table cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
		<td align="center">
		<table cellspacing="0" cellpadding="0" border="0">
			<tr>
				<td class="search_box">
				<div class="action_part_top">
					<asp:Button ID="btnBack" OnClick="btnBack_Click" Text=" 一覧へ戻る " runat="server" />
				</div>
				<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
					<tr>
						<td class="search_title_bg" width="95">
							<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle;" border="0" />
							利用クーポン
						</td>
						<td class="search_item_bg" width="130" style="height:20px;" colspan="3">
							<span><%: string.Format("{0} {1}", this.CouponCode, this.CouponName) %></span>
						</td>
						<td class="search_btn_bg" width="83" rowspan="5">
							<div class="search_btn_main">
								<asp:Button ID="btnSearch" Text=" 検索 " OnClick="btnSearch_Click" runat="server" />
							</div>
							<div class="search_btn_sub">
								<a href="<%= CreateResetUrl() %>">クリア</a>&nbsp
								<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
							</div>
						</td> 
					</tr>
					<tr>
						<td class="search_title_bg" width="95">
							<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
							ユーザーID
						</td>
						<td class="search_item_bg" width="220">
							<asp:TextBox id="tbUserId" runat="server" Width="200"></asp:TextBox>
						</td>
						<td class="search_title_bg" width="95">
							<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
							氏名
						</td>
						<td class="search_item_bg" width="220">
							<asp:TextBox id="tbUserName" runat="server" Width="200"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="search_title_bg" width="95">
							<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
							メールアドレス
						</td>
						<td class="search_item_bg" width="220">
							<asp:TextBox id="tbMailAddress" runat="server" Width="200"></asp:TextBox>
						</td>
						<td class="search_title_bg" width="95">
							<img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
							退会者
						</td>
						<td class="search_item_bg" width="130">
							<asp:CheckBox id="cbDeleteFlag" runat="server" Text="退会者を含む"></asp:CheckBox>
						</td>
					</tr>
					<tr>
						<td class="search_title_bg" width="95"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />
							利用日時
						</td>
						<td class="search_item_bg" colspan="3">
							<div id="useDate">
								<uc:DateTimeInput ID="ucUseDateFrom" runat="server" YearList="<%# DateTimeUtility.GetShortRangeYearListItem() %>" HasTime="False" HasBlankSign="True" HasBlankValue="True" />
								～
								<uc:DateTimeInput ID="ucUseDateTo" runat="server" YearList="<%# DateTimeUtility.GetShortRangeYearListItem() %>" HasTime="False" HasBlankSign="True" HasBlankValue="True" />
								の間
								<span class="search_btn_sub">(<a href="Javascript:SetToday();">今日</a>｜<a href="Javascript:SetThisMonth();">今月</a>｜<a href="Javascript:datePeriodClear('useDate');">クリア</a>)</span>
							</div>
						</td>
					</tr>
				</table>
				</td>
			</tr>
			<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
			<%-- マスタ出力 --%>
			<tr>
				<td class="search_table">
					<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="CouponUseUser" TableWidth="758" />
				</td>
			</tr>
			<tr><td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td></tr>
		</table>
		</td>
		</tr>
		</table>
		</td>
		</tr>
		</table>
		</td>
	</tr>
	<%--△ 検索 △--%>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<%--▽ 一覧 ▽--%>
	<tr id="trSubTitle" runat="server">
		<td><h2 class="cmn-hed-h2">クーポン利用ユーザー一覧</h2></td>
	</tr>
	<tr id="trList" runat="server">
		<td>
		<table class="box_border" cellpadding="0" cellspacing="1" width="784" border="0">
			<tr>
			<td>
			<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
				<td><img height="12" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				<td align="center">
					<table cellspacing="0" cellpadding="0" border="0">
						<tr>
							<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<tr>
						<td align="left">
							<%-- ページング --%>
							<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
								<tr>
									<td width="650"><asp:Label ID="lbPager" runat="server"></asp:Label></td>
									<td width="108" class="action_list_sp">
									<asp:Button ID="btnMassUpdateTop" Text=" 一括解除 " OnClick="btnMassUpdate_Click" runat="server" />
									</td>
								</tr>
							</table>
						</td>
						</tr>
						<tr>
							<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<tr>
							<td>
							<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
								<%-- 表示項目 --%>
								<tr class="list_title_bg">
									<td align="center" width="45"><input type="checkbox" id="cbSelectAll" onclick="javascript: ToggleAllCancelFlag();" runat="server" /></td>
									<td align="center" width="90">ユーザーID</td>
									<td align="center" width="145">メールアドレス</td>
									<td align="center" width="125">氏名</td>
									<td align="center" width="100">顧客区分</td>
									<td align="center" width="130">利用日時</td>
									<td align="center" width="120">注文ID</td>
									<td align="center" width="120">定期購入ID</td>
								</tr>
								<asp:Repeater ID="rCouponUseUserList" ItemType="w2.Domain.Coupon.Helper.CouponUseUserListSearchResult" runat="server">
									<ItemTemplate>
										<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
											<td align="center"><asp:CheckBox ID="cbCancelFlag" Checked="false" runat="server" /></td>
											<td align="center">
												<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_USER_CONFIRM)) { %>
													<a href="<%# WebSanitizer.UrlAttrHtmlEncode(CreateUserCouponListUrl(Item.UserId)) %>"><%#: Item.UserId %></a>
												<% } else { %>
													<%#: Item.UserId %>
												<% } %>
											</td>
											<td align="center"><%#: Item.MailAddress %></td>
											<td align="center"><%#: Item.UserName %></td>
											<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, Item.UserKbn) %></td>
											<td align="center"><%#: DateTimeUtility.ToStringForManager(Item.DateCreated, DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter) %></td>
											<td align="center">
												<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM)) { %>
													<a href="#" onclick="<%# CreateOrderConfirmUrl(Item.OrderId) %>"><%#: Item.OrderId %></a>
												<% } else { %>
													<%#: Item.OrderId %>
												<% } %>
											</td>
											<td align="center">
												<% if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM)) { %>
													<a href="#" onclick="<%# CreateFixedPurchaseDetailUrl(Item.FixedPurchaseId) %>"><%#: Item.FixedPurchaseId %> </a>
												<% } else { %>
													<%#: Item.FixedPurchaseId %>
												<% } %>
											</td>
										</tr>
									</ItemTemplate>
								</asp:Repeater>
								<%-- エラーメッセージ --%>
								<tr id="trListError" class="list_alert" runat="server" Visible="false">
									<td id="tdErrorMessage" colspan="8" runat="server">
									</td>
								</tr>
							</table>
							</td>
						</tr>
						<tr>
							<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<tr>
							<td class="action_list_sp">
								<asp:Button ID="btnMassUpdateBottom" Text=" 一括解除 " OnClick="btnMassUpdate_Click" runat="server" />
							</td>
						</tr>
						<tr>
							<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<tr id="trSpace" runat="server">
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<%--△ 一覧 △--%>
	<%--▽ 利用ユーザー登録 ▽--%>
	<tr>
		<td><h2 class="cmn-hed-h2">クーポン利用ユーザー追加</h2></td>
	</tr>
	<tr>
		<td>
		<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
			<tr id="trEdit" runat="server">
				<td>
				<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td><img height="12" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
						<td align="center">
							<table cellspacing="0" cellpadding="0" border="0">
								<tr>
									<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
								</tr>
								<tr>
									<td>
									<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
										<tr class="list_title_bg">
											<td align="center" width="180">
												<%: ValueText.GetValueText(Constants.TABLE_COUPONUSEUSER, Constants.FLG_COUPONUSEUSER_USED_USER_JUDGE_TYPE, Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE) %>
												<span class="notice">*</span>
											</td>
											<td align="center" width="350">利用クーポン</td>
											<td align="center" width="120">注文ID</td>
											<td align="center" width="120">定期購入ID</td>
											<td align="center" width="48">追加</td>
										</tr>
										<tr class="list_title_bg">
											<td align="center" width="180">
												<asp:TextBox ID="tbCouponUseUser" Width="180" runat="server"></asp:TextBox>
											</td>
											<td align="left" width="350">
												<span runat="server"><%: string.Format("{0} {1}", this.CouponCode, this.CouponName) %></span>
											</td>
											<td align="center" width="120">
												<asp:TextBox ID="tbOrderId" Width="120" runat="server"></asp:TextBox>
											</td>
											<td align="center" width="120">
												<asp:TextBox ID="tbFixedPurchaseId" Width="120" runat="server"></asp:TextBox>
											</td>
											<td align="center" width="48">
												<asp:Button ID="btnAdd" Text=" 追加 " OnClick="btnAdd_Click" runat="server" />
											</td>
										</tr>
									</table>
									</td>
								</tr>
								<tr>
									<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
								</tr>
								<tr>
									<td>
									<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
										<tr>
											<td align="left" class="info_item_bg" colspan="2">
												備考<br />
												・注文IDを指定してクーポン利用ユーザーを追加する場合、ユーザーのクーポン利用履歴に注文が紐づくだけで、実際に値引きは行われません。<br />
												　注文情報へのクーポン利用情報登録も行いません。<br />
												・定期購入IDを指定してクーポン利用ユーザーを追加する場合、ユーザーのクーポン利用履歴に定期台帳が紐づくだけで、実際に値引きは行われません。<br />
												　定期台帳情報へのクーポン利用情報登録も行いません。<br />
												<% if (Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS) { %>
												・注文ID／定期購入IDの指定せずクーポン利用ユーザーを追加する場合、クーポン利用ユーザー一覧でそのユーザーのユーザーID、氏名、顧客区分は表示されません。<br />
												・退会済みユーザーの注文との紐づけは行えず、エラーとなります。(ユーザー退会時にユーザー情報からメールアドレスが消去されるため)
												<%} %>
											</td>
										</tr>
									</table>
									</td>
								</tr>
								<tr>
									<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
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
	<tr id="trComplete" runat="server">
		<td>
		<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
				<td><img height="12" alt="" src="../../Images/Common/sp.gif" border="0" /></td>
				<td align="center">
					<table cellspacing="0" cellpadding="0" border="0">
						<tr>
							<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<tr>
							<td class="action_list_sp">
								<asp:Button ID="btnRedirectEditTop" Text=" 続けて処理をする " OnClick="btnRedirectEdit_Click" runat="server" />
							</td>
						</tr>
						<tr>
							<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<tr>
							<td>
							<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
								<tr class="list_title_bg">
									<td align="center" width="45">解除</td>
									<td align="center" width="100">ユーザーID</td>
									<td align="center" width="125">メールアドレス</td>
									<td align="center" width="100">氏名</td>
									<td align="center" width="100">顧客区分</td>
									<td align="center" width="160">利用日時</td>
									<td align="center" width="125">注文ID</td>
								</tr>
								<asp:Repeater ID="rCouponUseUserListComplete" ItemType="CouponUseUserListSearchResult" runat="server">
									<ItemTemplate>
										<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
											<td align="center">解除済</td>
											<td align="center"><%#: Item.UserId %></td>
											<td align="center"><%#: Item.MailAddress %></td>
											<td align="center"><%#: Item.UserName %></td>
											<td align="center"><%#: Item.UserKbn %></td>
											<td align="center"><%#: Item.LastChanged %></td>
											<td align="center"><%#: Item.OrderId %></td>
										</tr>
									</ItemTemplate>
								</asp:Repeater>
							</table>
							</td>
						</tr>
						<tr>
							<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
						<tr>
							<td class="action_list_sp">
								<asp:Button ID="btnRedirectEditBottom" Text=" 続けて処理をする " OnClick="btnRedirectEdit_Click" runat="server" />
							</td>
						</tr>
						<tr>
							<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
		</td>
	</tr>
	<%--△ 利用ユーザー登録 △--%>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
<script type="text/javascript">
<!--
	// 今日
	function SetToday() {
		var today = new Date();
		document.getElementById('<%= ucUseDateFrom.DdlYear.ClientID %>').value = today.getFullYear();
		document.getElementById('<%= ucUseDateFrom.DdlMonth.ClientID %>').value = ('0' + (today.getMonth() + 1)).slice(-2);
		document.getElementById('<%= ucUseDateFrom.DdlDay.ClientID %>').value = ('0' + today.getDate()).slice(-2);
		document.getElementById('<%= ucUseDateTo.DdlYear.ClientID %>').value = today.getFullYear();
		document.getElementById('<%= ucUseDateTo.DdlMonth.ClientID %>').value = ('0' + (today.getMonth() + 1)).slice(-2);
		document.getElementById('<%= ucUseDateTo.DdlDay.ClientID %>').value = ('0' + today.getDate()).slice(-2);
	}

	// 今月
	function SetThisMonth() {
		var thisMonth = new Date();
		document.getElementById('<%= ucUseDateFrom.DdlYear.ClientID %>').value = thisMonth.getFullYear();
		document.getElementById('<%= ucUseDateFrom.DdlMonth.ClientID %>').value = ('0' + (thisMonth.getMonth() + 1)).slice(-2);
		document.getElementById('<%= ucUseDateFrom.DdlDay.ClientID %>').value = '01';
		document.getElementById('<%= ucUseDateTo.DdlYear.ClientID %>').value = thisMonth.getFullYear();
		document.getElementById('<%= ucUseDateTo.DdlMonth.ClientID %>').value = ('0' + (thisMonth.getMonth() + 1)).slice(-2);
		document.getElementById('<%= ucUseDateTo.DdlDay.ClientID %>').value = ('0' + (new Date(thisMonth.getFullYear(), thisMonth.getMonth() + 1, 0).getDate())).slice(-2);
	}
	// 解除対象全件選択
	function ToggleAllCancelFlag() {
		var checked = document.getElementById('<%= cbSelectAll.ClientID %>').checked;
		for (i = 0; i < document.getElementsByTagName("input").length; i++) {
			var checkBox = document.getElementsByTagName("input")[i];
			if ((checkBox.type == "checkbox")
				&& (/cbCancelFlag/.test(checkBox.id))) {
				checkBox.checked = checked;
			}
		}
	}
//-->
</script>
</asp:Content>