<%--
=========================================================================================================
  Module      : Advertisement Code Media Type(AdvertisementCodeMediaType.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>

<%@ Import Namespace="w2.Domain.AdvCode" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="AdvertisementCodeMediaType.aspx.cs" Inherits="Form_AdvertisementCode_AdvertisementCodeMediaType" %>
<%@ Register TagPrefix="uc" TagName="MasterDownLoad" Src="~/Form/Common/MasterDownLoadPanel.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h2 class="cmn-hed-h2">広告媒体区分情報</h2></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 検索 ▽-->
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
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td class="search_box">
												<table class="search_table" cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="100"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"
																border="0" />区分ID</td>
														<td class="search_item_bg" width="150">
															<asp:TextBox ID="tbSearchAdvCodeMediaTypeId" Width="140" runat="server"></asp:TextBox>
														</td>
														<td class="search_title_bg" width="80"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />区分名</td>
														<td class="search_item_bg" width="150"><asp:TextBox id="tbSearchAdvCodeMediaTypeName" runat="server" Width="140"></asp:TextBox></td>
														<td class="search_title_bg" width="80"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle" border="0" />並び順</td>
														<td class="search_item_bg" width="110">
															<asp:DropDownList id="ddlSearchSortKbn" runat="server" Width="110">
																<asp:ListItem Value="0">登録日/昇順</asp:ListItem>
																<asp:ListItem Value="1" Selected="True">登録日/降順</asp:ListItem>
																<asp:ListItem Value="2">区分ID/昇順</asp:ListItem>
																<asp:ListItem Value="3">区分ID/降順</asp:ListItem>
															</asp:DropDownList>
														</td>
														<td class="search_btn_bg" width="100" >
															<div class="search_btn_main"><asp:Button id="btnSearch" runat="server" Text="  検索  " onclick="btnSearch_Click"></asp:Button></div>
															<div class="search_btn_sub">
																<a href="<%= Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_MEDIA_TYPE %>">クリア</a>&nbsp;
																<a href="javascript:document.<%= this.Form.ClientID %>.reset();">リセット</a>
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
													<uc:MasterDownLoad runat="server" ID="uMasterDownload" DownloadType="AdvCodeMediaType" TableWidth="758" />
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
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr id="trList" runat="server">
		<td><h2 class="cmn-hed-h2">広告媒体区分情報一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<div id="divEdit" runat="server">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<!--▽ ページング ▽--> 
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="80%"><asp:label id="lbPager" Runat="server"></asp:label></td>
															<td width="20%" class="action_list_sp"><asp:Button id="btnUpdateTop" runat="server" Text="  このページの一括更新  " OnClientClick="return CheckRegistShortAddress()" onclick="btnUpdateTop_Click"></asp:Button></td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="list_title_bg">
															<td align="center" width="250">区分ID</td>
															<td align="center" width="342">区分名</td>
															<td align="center" width="70">表示順</td>
															<td align="center" width="58">削除</td>
														</tr>
														<asp:Repeater id="rEditList" Runat="server">
															<ItemTemplate>
																<tr>
																	<td align="left" class="list_item_bg1">
																		<asp:Label Text="<%# WebSanitizer.HtmlEncode(((AdvCodeMediaTypeModel)Container.DataItem) .AdvcodeMediaTypeId) %>" Width="220" runat="server"></asp:Label>
																		<asp:HiddenField ID="hfAdvCodeMediaTypeId" Value="<%# ((AdvCodeMediaTypeModel)Container.DataItem).AdvcodeMediaTypeId %>" runat="server" />
																	</td>
																	<td align="left" class="list_item_bg1">
																		<asp:TextBox ID="tbAdvCodeMediaTypeName" Text="<%# ((AdvCodeMediaTypeModel)Container.DataItem).AdvcodeMediaTypeName %>" Width="350" runat="server"></asp:TextBox>
																		<asp:HiddenField ID="hfAdvCodeMediaTypeName" Value="<%# ((AdvCodeMediaTypeModel)Container.DataItem).AdvcodeMediaTypeName %>" runat="server" />
																	</td>
																	<td align="center" class="list_item_bg1">
																		<asp:TextBox ID="tbAdvCodeMediaTypeDisplayOrder" runat="server" Text='<%# ((AdvCodeMediaTypeModel)Container.DataItem).DisplayOrder %>' Width="70" runat="server"></asp:TextBox>
																		<asp:HiddenField ID="hfAdvCodeMediaTypeDisplayOrder" Value="<%# ((AdvCodeMediaTypeModel)Container.DataItem).DisplayOrder %>" runat="server" />
																	</td>
																	<td align="center" class="list_item_bg1">
																		<asp:Button id="btnDelete" Text="  削除  " runat="server" OnClientClick="return confirm('削除してもよろしいですか？');" CommandArgument='<%# Container.ItemIndex %>' OnClick="btnDelete_Click" /></td>
																</tr>
															</ItemTemplate>
															<AlternatingItemTemplate>
																<tr>
																	<td align="left" class="list_item_bg2">
																		<asp:Label Text="<%# WebSanitizer.HtmlEncode(((AdvCodeMediaTypeModel)Container.DataItem).AdvcodeMediaTypeId) %>" Width="220" runat="server"></asp:Label>
																		<asp:HiddenField ID="hfAdvCodeMediaTypeId" Value="<%# ((AdvCodeMediaTypeModel)Container.DataItem).AdvcodeMediaTypeId %>" runat="server" />
																	</td>
																	<td align="left" class="list_item_bg2">
																		<asp:TextBox ID="tbAdvCodeMediaTypeName" Text="<%# ((AdvCodeMediaTypeModel)Container.DataItem).AdvcodeMediaTypeName %>" Width="350" runat="server"></asp:TextBox>
																		<asp:HiddenField ID="hfAdvCodeMediaTypeName" Value="<%# ((AdvCodeMediaTypeModel)Container.DataItem).AdvcodeMediaTypeName %>" runat="server" />
																	</td>
																	<td align="center" class="list_item_bg2">
																		<asp:TextBox ID="tbAdvCodeMediaTypeDisplayOrder" runat="server" Text='<%# ((AdvCodeMediaTypeModel)Container.DataItem).DisplayOrder %>' Width="70" runat="server"></asp:TextBox>
																		<asp:HiddenField ID="hfAdvCodeMediaTypeDisplayOrder" Value="<%# ((AdvCodeMediaTypeModel)Container.DataItem).DisplayOrder %>" runat="server" />
																	</td>
																	<td align="center" class="list_item_bg2">
																		<asp:Button id="btnDelete" Text="  削除  " runat="server" OnClientClick="return confirm('削除してもよろしいですか？');" CommandArgument='<%# Container.ItemIndex %>' OnClick="btnDelete_Click" /></td>
																</tr>
															</AlternatingItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" Visible="false">
															<td id="tdErrorMessage" runat="server" colspan="4" style="height: 17px"><asp:label ID="lUpdateErrorMessage" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server"></asp:label></td>
														</tr>
													</table>
													<table cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
														</tr>
													</table>
												</td>
											</tr>
											
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td class="action_list_sp"><asp:Button id="btnUpdateBottom" runat="server" Text="  このページの一括更新  " onclick="btnUpdateTop_Click"></asp:Button></td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</div>
									<div id="divComplete" runat="server" Visible="False">
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="list_pager" cellspacing="0" cellpadding="0" width="758" border="0">
														<tr>
															<td width="625">以下の広告媒体区分情報を更新いたしました｡</td>
															<td width="133" class="action_list_sp"><asp:Button id="btRedirectEditTop" Runat="server" Text="  続けて処理をする  " onclick="btRedirectEditTop_Click"></asp:Button></td>
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
														<tr class="list_title_bg">
															<td align="center" width="250">区分ID</td>
															<td align="center" width="342">区分名</td>
															<td align="center" width="70">表示順</td>
														</tr>
														<asp:Repeater id="rComplete" Runat="server">
															<ItemTemplate>
																<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>">
																	<td align="center"><%# WebSanitizer.HtmlEncode(((AdvCodeMediaTypeModel)Container.DataItem).AdvcodeMediaTypeId)%></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(((AdvCodeMediaTypeModel)Container.DataItem).AdvcodeMediaTypeName) %></td>
																	<td align="center"><%# WebSanitizer.HtmlEncode(((AdvCodeMediaTypeModel)Container.DataItem).DisplayOrder) %></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td class="action_list_sp"><asp:Button id="btRedirectEditBottom" Runat="server" Text="  続けて処理をする  " onclick="btRedirectEditTop_Click"></asp:Button></td>
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
	<tbody id="tbdyAddShortUrl" runat="server">
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
		</tr>
		<!--▽ 追加 ▽-->
		<tr id="tr1" runat="server">
			<td><h2 class="cmn-hed-h2">広告媒体区分情報追加</h2></td>
		</tr>
		<tr>
			<td>
				<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
					<tr>
						<td>
							<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<div id="divAddNew" runat="server">
											<table cellspacing="0" cellpadding="0" border="0">
												<tr>
													<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
												</tr>
												<tr>
													<td>
														<table class="list_table" cellspacing="1" cellpadding="3" width="758" border="0">
															<tr class="list_title_bg">
																<td align="center" width="250">区分ID</td>
																<td align="center" width="342">区分名</td>
																<td align="center" width="70">表示順</td>
																<td align="center" width="58">追加</td>
															</tr>
															<tr>
																<td align="left" class="list_item_bg1">
																	<asp:TextBox ID="tbAddAdvCodeMediaTypeId" Width="250" runat="server" MaxLength="30"></asp:TextBox></td>
																<td align="left" class="list_item_bg1">
																	<asp:TextBox ID="tbAddAdvCodeMediaTypeName" Width="342" runat="server" MaxLength="100"></asp:TextBox></td>
																<td align="center" class="list_item_bg1">
																	<asp:TextBox ID="tbAddAdvCodeMediaTypeDisplayOrder" Width="70" runat="server"></asp:TextBox></td>
																<td align="center" class="list_item_bg1">
																	<asp:Button id="btnAdd" Text="  追加  " runat="server" OnClick="btnAdd_Click" /></td>
															</tr>
															<tr id="trErrorInsert" class="list_alert" runat="server" Visible="false">
															<td runat="server" colspan="4" style="height: 17px"><asp:label ID="lErrorInsertMessage" CssClass="notice" style="font-weight:bold; line-height:normal;" runat="server"></asp:label></td>
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
	</tbody>
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
