<%--
=========================================================================================================
  Module      : 海外配送料編集ページ(GlobalShippingPostageEdit.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="GlobalShippingPostageEdit.aspx.cs" Inherits="Form_Global_Shipping_Postage_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">配送地域設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 詳細 ▽-->
	<tr >
		<td><h2 class="cmn-hed-h2">配送エリア重量別送料</h2></td>
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
													<asp:Button ID="btnBackToListTop" runat="server" Text="  配送種別詳細へ戻る  " OnClick="btnBackToListTop_Click" />
												</div>
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="4">基本情報</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">配送種別ID</td>
														<td class="detail_item_bg" align="left" width="10%"><%#: this.KeepingShippingId %></td>
														<td class="detail_title_bg" align="left" width="20%">配送種別</td>
														<td class="detail_item_bg" align="left" width="50%"><%#: this.KeepingShippingName %></td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="20%">配送サービスID</td>
														<td class="detail_item_bg" align="left" width="10%"><%#: this.KeepingDeliveryCompanyId %></td>
														<td class="detail_title_bg" align="left" width="20%">配送サービス</td>
														<td class="detail_item_bg" align="left" width="50%"><%#: this.KeepingDeliveryCompanyName %></td>
													</tr>
												</table>
												<br />
												<div class="action_part_bottom"></div>
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
	<tr>
		<td><h2 class="cmn-hed-h2">配送料金表</h2></td>
	</tr>
	<tr>
		<td>
			<table cellspacing="1" cellpadding="3" border="0">
				<tr>
					<td>
						<asp:UpdatePanel runat="server" ID="up1">
						<ContentTemplate>
						<table cellspacing="0" cellpadding="0" border="0" >
							<tr>
								<td>配送種別：<asp:DropDownList runat="server" ID="ddlShipping" OnSelectedIndexChanged="ddlShipping_OnSelectedIndexChanged" AutoPostBack="true" />
									の配送サービス：<asp:DropDownList runat="server" ID="ddlDelivery" />
									<asp:Button runat="server" ID="btnCopyGlobalPostage" Text="から送料表をコピーする" OnClick="btnCopyGlobalPostage_Click" OnClientClick="return confirm('コピーしてもよろしいですか？');" />
								</td>
							</tr>
							<tr>
								<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
						</table>
						<table cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td><asp:Button runat="server" ID="btnClearGlobalPostage" text="送料表をクリアする" OnClick="btnClearGlobalPostage_Click" OnClientClick="return confirm('クリアしてもよろしいですか？');" /> </td>
							</tr>
							<tr>
								<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<asp:Repeater ID="rAnchor" runat="server" ItemType="GlobalPostageMap">
								<ItemTemplate>
									<tr>
										<td><a href="#<%#:Item.GlobalShippingAreaId %>"><%#:Item.GlobalShippingAreaName %></a></td>
									</tr>
								</ItemTemplate>
							</asp:Repeater>
							<tr>
								<td align="left">
									<asp:Repeater id="repGlobalShippingArea" Runat="server" ItemType="GlobalPostageMap" OnItemCommand="repGlobalShippingArea_ItemCommand">
										<ItemTemplate>
											<span id="<%#:Item.GlobalShippingAreaId %>"></span><span id="a_idx_<%#:Container.ItemIndex %>"></span>
											<br />
											<h2 class="cmn-hed-h2">
												<%#:Item.GlobalShippingAreaName %>　<span runat="server" visible="<%# Container.ItemIndex < Item.AreaCount - 1 %>"><a href='#a_idx_<%#Container.ItemIndex + 1 %>'>↓</a></span>　<span runat="server" visible="<%# Container.ItemIndex > 0 %>"><a href='#a_idx_<%#Container.ItemIndex - 1 %>'>↑</a></span>
											</h2>
											<table class="detail_table" cellspacing="1" cellpadding="3" width="300" border="0">
												<tr>
													<td align="left" class="detail_title_bg" width="30%">重量（g以上）～重量（g以下）</td>
													<td align="left" class="detail_title_bg" width="30%">送料</td>
													<td align="left" class="detail_title_bg" width="20%">削除</td>
												</tr>
												<asp:Repeater id="repGlobalShippingAreaPostage" Runat="server" DataSource="<%#Item.Postage %>" ItemType="w2.Domain.GlobalShipping.GlobalShippingPostageModel" OnItemCommand="repGlobalShippingAreaPostage_ItemCommand">
													<ItemTemplate>
														<tr>
															<td class="detail_item_bg" align="left"><%#:StringUtility.ToNumeric(Item.WeightGramGreaterThanOrEqualTo) %>g～<%#:StringUtility.ToNumeric(Item.WeightGramLessThan) %>g</td>
															<td class="detail_item_bg" align="left"><asp:TextBox runat="server" ID="tbChangePostage" Text="<%#:GlobalPriceDisplayControlToTextbox(Item.GlobalShippingPostage) %>" /><asp:Button runat="server" ID="btnChangePostage" CommandName="ChangePostage" CommandArgument="<%# Item.Seq %>" Text="更新" /><asp:Literal runat="server" ID="ltMsg" /></td>
															<td class="detail_item_bg" align="center"><asp:Button runat="server" ID="btnDelGlobalPostage" CommandName="DelGlobalPostage" CommandArgument="<%# Item.Seq %>" Text="削除" /></td>
														</tr>
													</ItemTemplate>
												</asp:Repeater>
												<tr>
													<td class="detail_item_bg" align="left"><asp:TextBox runat="server" ID="tbWeightGramGreaterThanOrEqualTo" />ｇ～<asp:TextBox runat="server" ID="tbWeightGramLessThan" />ｇ</td>
													<td class="detail_item_bg" align="left"><asp:TextBox runat="server" ID="tbAddAreaPostage"  /><asp:Button runat="server" ID="btnAddAreaPostaeg" CommandName="AddAreaPostage" CommandArgument="<%# Item.GlobalShippingAreaId %>" Text="追加" /><asp:Literal runat="server" ID="ltMsg" /></td>
													<td class="detail_item_bg" align="center"></td>
												</tr>
											</table>
										</ItemTemplate>
									</asp:Repeater>
								</td>
							</tr>
						</table>
						</ContentTemplate>
						</asp:UpdatePanel>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	
	<!--△ 詳細 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
