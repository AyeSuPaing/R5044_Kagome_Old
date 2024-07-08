<%--
=========================================================================================================
  Module      : 自動翻訳設定情報一覧ページ(AutoTranslationWordList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="AutoTranslationWordList.aspx.cs" Inherits="Form_AutomaticTranslation_AutomaticTranslation" %>
<%@ Import Namespace="w2.Domain.AutoTranslationWord.Helper" %>
<%@ Import Namespace="w2.App.Common.Global.Config" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<%-- 
テキストボックス内でEnterキーを押してSubmit（一番上に配置されているTextBoxのSubmit）送信しようとすると、
IEのバグでテキストボックスが画面上に一つのみ配置されている場合にSubmit送信されない不具合の対応として、
ダミーのTextBoxを非表示で配置している。
--%>
<asp:TextBox id="tbDummy" runat="server" style="visibility:hidden;display:none;"></asp:TextBox>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">自動翻訳設定</h1></td>
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
											<td class="search_table">
												<table cellspacing="1" cellpadding="2" width="758" border="0">
													<tr>
														<td class="search_title_bg" width="130"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"border="0" />
															翻訳元ワード
														</td>
														<td class="search_item_bg" width="110" colspan="3"><asp:TextBox id="tbWordBefore" runat="server" Width="250"></asp:TextBox></td>
														<td class="search_btn_bg" width="83" rowspan="2">
															<div class="search_btn_main">
																<asp:Button id="btnSearch" runat="server" Text="  検索  " OnClick="btnSearch_Click" /></div>
														</td>
													</tr>
													<tr>
														<td class="search_title_bg" width="130"><img height="5" alt="" src="../../Images/Common/arrow_01.gif" width="10" style="vertical-align:middle"border="0" />
															言語コード
														</td>
														<td class="search_item_bg" width="110" colspan="3"><asp:TextBox id="tbLanguageCode" runat="server" Width="50" MaxLength="10"></asp:TextBox></td>
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
	<!--△ 検索 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">自動翻訳設定一覧</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="0" border="0">
										<tr>
											<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td align="right">
												<!--▽ ページング ▽-->
												<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
													<tr>
														<td style="height: 22px"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
														<td class="action_list_sp"><asp:Button id="btnInsert" Runat="server" Text="  新規登録  " OnClick="btnInsert_Click" /></td>
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
												<div>
												<table class="list_table" cellspacing="1" cellpadding="3" width="750" border="0">
													<tr class="list_title_bg">
														<td align="center" width="240" rowspan="2">翻訳元ワード</td>
														<td align="center" width="80" rowspan="2">言語コード</td>
														<td align="center" width="240" rowspan="2">翻訳後ワード</td>
														<td align="center" width="80" rowspan="2">クリアフラグ</td>
													</tr>
													<tr class="list_title_bg" >
														<td align="center" width="130">最終利用日時</td>
													</tr>
													<asp:Repeater id="rList" Runat="server">
														<ItemTemplate>
															<tr class="list_item_bg<%# Container.ItemIndex % 2 + 1 %>" onmouseover="listselect_mover(this)" onmouseout="listselect_mout<%# Container.ItemIndex % 2 + 1 %>(this)" onmousedown="listselect_mdown(this)" onmousedown="listselect_mdown(this)" onclick="listselect_mclick(this, '<%#: CreateDetailUrl(((AutoTranslationWordListSearchResult)Container.DataItem).WordHashKey, ((AutoTranslationWordListSearchResult)Container.DataItem).LanguageCode) %>')">
																<td align="center"><%#: DisplayLimit(((AutoTranslationWordListSearchResult)Container.DataItem).WordBefore, 30)%></td>
																<td align="center"><%#: DisplayLimit(((AutoTranslationWordListSearchResult)Container.DataItem).LanguageCode,10)%></td>
																<td align="center"><%#: DisplayLimit(((AutoTranslationWordListSearchResult)Container.DataItem).WordAfter, 30)%></td>
																<td align="center"><%#: ValueText.GetValueText(Constants.TABLE_AUTOTRANSLATIONWORD, Constants.FIELD_AUTOTRANSLATIONWORD_CLEAR_FLG, ((AutoTranslationWordListSearchResult)Container.DataItem).ClearFlg)%></td>
																<td align="center"><%#: ((AutoTranslationWordListSearchResult)Container.DataItem).DateUsed%></td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<tr id="trListError" class="list_alert" runat="server" Visible="False">
														<td id="tdErrorMessage" colspan="5" runat="server"></td>
													</tr>
												</table>
												</div>
											</td>
										</tr>
										<tr>
											<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td align="left">備考<br />
															・クリアフラグが「〇」のものは、削除バッチ実行時に対象となるものです。最終利用日時が本日より<%: GlobalConfigs.GetInstance().GlobalSettings.Translation.TranslationDeletingIntervalDay %>日以前のデータが対象となります。<br />
														・言語コードはこちらをご参照ください：<a href="<%: URL_GOOGLE_TRANSLATE %>" target="_blank"><%: URL_GOOGLE_TRANSLATE %></a>
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
			</table>
		</td>
	</tr>
	<!--△ 一覧 △-->
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</asp:Content>
