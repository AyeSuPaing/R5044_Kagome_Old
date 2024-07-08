<%--
=========================================================================================================
  Module      : 配送料情報登録ページ(ShippingRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Domain.DeliveryCompany" %>
<%@ Import Namespace="w2.App.Common.Global.Region.Currency" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ShippingRegister.aspx.cs" Inherits="Form_Shipping_ShippingRegister" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="w2cm" Assembly="w2.App.Common" Namespace="w2.App.Common.Web.WebCustomControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<asp:UpdatePanel runat="server">
<ContentTemplate>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">配送種別設定</h1></td>
	</tr>
	<tr>
		<td style="width: 797px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 登録 ▽-->
	<tr id="trEditTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送種別設定編集</h2></td>
	</tr>
	<tr id="trRegisterTop" runat="server" Visible="False">
		<td><h2 class="cmn-hed-h2">配送種別設定登録</h2></td>
	</tr>
	<tr>
		<td style="width: 797px">
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
												<asp:Button ID="btnBackTop" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
												<asp:Button id="btnNextTop" runat="server" Text="  配送料を指定する  " OnClick="btnNext_Click" />
												</div>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">基本情報</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">配送種別ID<span class="notice">*</span></td>
															<td id="tdShippingIdEdit" class="edit_item_bg" align="left" runat="server" visible="false">
																<asp:TextBox ID="tbShippingId" runat="server" MaxLength="4" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID] %>"></asp:TextBox>
															</td>
															<td id="tdShippingIdView" class="edit_item_bg" align="left" runat="server" visible="false">
																<%# WebSanitizer.HtmlEncode(m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]) %>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">配送種別<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbName" runat="server" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME] %>" Width="250" MaxLength="30"></asp:TextBox></td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">配送拠点<span class="notice">*</span></td>
															<td class="edit_item_bg" align="left"><asp:DropDownList ID="ddlShippingBase" runat="server" /></td>
														</tr>
													</tbody>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">配送可能日付範囲情報</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="center" colspan="2">配送希望日の指定が可能な場合に設定します。日付の計算には注文の実行した日を基準としています。</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">日付範囲設定の利用の有無</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox id="cbShippingDateSetFlg" Checked="<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG] == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID %>" OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" Runat="server"></asp:CheckBox></td>
														</tr>
														<tr id="trShippingDateSet" runat="server">
															<td class="edit_title_bg" align="left" width="30%">日付範囲</td>
															<td class="edit_item_bg" align="left">
																<font face="MS UI Gothic">翌</font><asp:DropDownList ID="ddlBusinessDaysForShipping" runat="server" SelectedValue="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_BUSINESS_DAYS_FOR_SHIPPING].ToString() %>">
																	<asp:ListItem Value="0">0</asp:ListItem>
																	<asp:ListItem Value="1">1</asp:ListItem>
																	<asp:ListItem Value="2">2</asp:ListItem>
																	<asp:ListItem Value="3">3</asp:ListItem>
																	<asp:ListItem Value="4">4</asp:ListItem>
																	<asp:ListItem Value="5">5</asp:ListItem>
																	<asp:ListItem Value="6">6</asp:ListItem>
																	<asp:ListItem Value="7">7</asp:ListItem>
																	<asp:ListItem Value="8">8</asp:ListItem>
																	<asp:ListItem Value="9">9</asp:ListItem>
																	<asp:ListItem Value="10">10</asp:ListItem>
																 </asp:DropDownList>
																<font face="MS UI Gothic">営業日から</font>
																<asp:DropDownList id="ddlShippingDateSetBegin" runat="server" SelectedValue="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_BEGIN].ToString() %>">
																	<asp:ListItem Value="1">01</asp:ListItem>
																	<asp:ListItem Value="2">02</asp:ListItem>
																	<asp:ListItem Value="3">03</asp:ListItem>
																	<asp:ListItem Value="4">04</asp:ListItem>
																	<asp:ListItem Value="5">05</asp:ListItem>
																	<asp:ListItem Value="6">06</asp:ListItem>
																	<asp:ListItem Value="7">07</asp:ListItem>
																	<asp:ListItem Value="8">08</asp:ListItem>
																	<asp:ListItem Value="9">09</asp:ListItem>
																	<asp:ListItem Value="10">10</asp:ListItem>
																	<asp:ListItem Value="11">11</asp:ListItem>
																	<asp:ListItem Value="12">12</asp:ListItem>
																	<asp:ListItem Value="13">13</asp:ListItem>
																	<asp:ListItem Value="14">14</asp:ListItem>
																	<asp:ListItem Value="15">15</asp:ListItem>
																	<asp:ListItem Value="16">16</asp:ListItem>
																	<asp:ListItem Value="17">17</asp:ListItem>
																	<asp:ListItem Value="18">18</asp:ListItem>
																	<asp:ListItem Value="19">19</asp:ListItem>
																	<asp:ListItem Value="20">20</asp:ListItem>
																	<asp:ListItem Value="21">21</asp:ListItem>
																	<asp:ListItem Value="22">22</asp:ListItem>
																	<asp:ListItem Value="23">23</asp:ListItem>
																	<asp:ListItem Value="24">24</asp:ListItem>
																	<asp:ListItem Value="25">25</asp:ListItem>
																	<asp:ListItem Value="26">26</asp:ListItem>
																	<asp:ListItem Value="27">27</asp:ListItem>
																	<asp:ListItem Value="28">28</asp:ListItem>
																	<asp:ListItem Value="29">29</asp:ListItem>
																	<asp:ListItem Value="30">30</asp:ListItem>
																	<asp:ListItem Value="31">31</asp:ListItem>
																	<asp:ListItem Value="32">32</asp:ListItem>
																	<asp:ListItem Value="33">33</asp:ListItem>
																	<asp:ListItem Value="34">34</asp:ListItem>
																	<asp:ListItem Value="35">35</asp:ListItem>
																	<asp:ListItem Value="36">36</asp:ListItem>
																	<asp:ListItem Value="37">37</asp:ListItem>
																	<asp:ListItem Value="38">38</asp:ListItem>
																	<asp:ListItem Value="39">39</asp:ListItem>
																	<asp:ListItem Value="40">40</asp:ListItem>
																	<asp:ListItem Value="41">41</asp:ListItem>
																	<asp:ListItem Value="42">42</asp:ListItem>
																	<asp:ListItem Value="43">43</asp:ListItem>
																	<asp:ListItem Value="44">44</asp:ListItem>
																	<asp:ListItem Value="45">45</asp:ListItem>
																	<asp:ListItem Value="46">46</asp:ListItem>
																	<asp:ListItem Value="47">47</asp:ListItem>
																	<asp:ListItem Value="48">48</asp:ListItem>
																	<asp:ListItem Value="49">49</asp:ListItem>
																	<asp:ListItem Value="50">50</asp:ListItem>
																	<asp:ListItem Value="51">51</asp:ListItem>
																	<asp:ListItem Value="52">52</asp:ListItem>
																	<asp:ListItem Value="53">53</asp:ListItem>
																	<asp:ListItem Value="54">54</asp:ListItem>
																	<asp:ListItem Value="55">55</asp:ListItem>
																	<asp:ListItem Value="56">56</asp:ListItem>
																	<asp:ListItem Value="57">57</asp:ListItem>
																	<asp:ListItem Value="58">58</asp:ListItem>
																	<asp:ListItem Value="59">59</asp:ListItem>
																	<asp:ListItem Value="60">60</asp:ListItem>
																</asp:DropDownList><font face="MS UI Gothic">日以降</font>
																<asp:DropDownList id="ddlShippingDateSetTerm" runat="server" SelectedValue="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_END].ToString() %>">
																	<asp:ListItem Value="1">01</asp:ListItem>
																	<asp:ListItem Value="2">02</asp:ListItem>
																	<asp:ListItem Value="3">03</asp:ListItem>
																	<asp:ListItem Value="4">04</asp:ListItem>
																	<asp:ListItem Value="5">05</asp:ListItem>
																	<asp:ListItem Value="6">06</asp:ListItem>
																	<asp:ListItem Value="7">07</asp:ListItem>
																	<asp:ListItem Value="8">08</asp:ListItem>
																	<asp:ListItem Value="9">09</asp:ListItem>
																	<asp:ListItem Value="10">10</asp:ListItem>
																	<asp:ListItem Value="11">11</asp:ListItem>
																	<asp:ListItem Value="12">12</asp:ListItem>
																	<asp:ListItem Value="13">13</asp:ListItem>
																	<asp:ListItem Value="14">14</asp:ListItem>
																	<asp:ListItem Value="15">15</asp:ListItem>
																	<asp:ListItem Value="16">16</asp:ListItem>
																	<asp:ListItem Value="17">17</asp:ListItem>
																	<asp:ListItem Value="18">18</asp:ListItem>
																	<asp:ListItem Value="19">19</asp:ListItem>
																	<asp:ListItem Value="20">20</asp:ListItem>
																	<asp:ListItem Value="21">21</asp:ListItem>
																	<asp:ListItem Value="22">22</asp:ListItem>
																	<asp:ListItem Value="23">23</asp:ListItem>
																	<asp:ListItem Value="24">24</asp:ListItem>
																	<asp:ListItem Value="25">25</asp:ListItem>
																	<asp:ListItem Value="26">26</asp:ListItem>
																	<asp:ListItem Value="27">27</asp:ListItem>
																	<asp:ListItem Value="28">28</asp:ListItem>
																	<asp:ListItem Value="29">29</asp:ListItem>
																	<asp:ListItem Value="30">30</asp:ListItem>
																	<asp:ListItem Value="31">31</asp:ListItem>
																	<asp:ListItem Value="32">32</asp:ListItem>
																	<asp:ListItem Value="33">33</asp:ListItem>
																	<asp:ListItem Value="34">34</asp:ListItem>
																	<asp:ListItem Value="35">35</asp:ListItem>
																	<asp:ListItem Value="36">36</asp:ListItem>
																	<asp:ListItem Value="37">37</asp:ListItem>
																	<asp:ListItem Value="38">38</asp:ListItem>
																	<asp:ListItem Value="39">39</asp:ListItem>
																	<asp:ListItem Value="40">40</asp:ListItem>
																	<asp:ListItem Value="41">41</asp:ListItem>
																	<asp:ListItem Value="42">42</asp:ListItem>
																	<asp:ListItem Value="43">43</asp:ListItem>
																	<asp:ListItem Value="44">44</asp:ListItem>
																	<asp:ListItem Value="45">45</asp:ListItem>
																	<asp:ListItem Value="46">46</asp:ListItem>
																	<asp:ListItem Value="47">47</asp:ListItem>
																	<asp:ListItem Value="48">48</asp:ListItem>
																	<asp:ListItem Value="49">49</asp:ListItem>
																	<asp:ListItem Value="50">50</asp:ListItem>
																	<asp:ListItem Value="51">51</asp:ListItem>
																	<asp:ListItem Value="52">52</asp:ListItem>
																	<asp:ListItem Value="53">53</asp:ListItem>
																	<asp:ListItem Value="54">54</asp:ListItem>
																	<asp:ListItem Value="55">55</asp:ListItem>
																	<asp:ListItem Value="56">56</asp:ListItem>
																	<asp:ListItem Value="57">57</asp:ListItem>
																	<asp:ListItem Value="58">58</asp:ListItem>
																	<asp:ListItem Value="59">59</asp:ListItem>
																	<asp:ListItem Value="60">60</asp:ListItem>
																</asp:DropDownList><font face="MS UI Gothic">日間指定可能</font>
															</td>
														</tr>
													</tbody>
												</table>
												<br />
												<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">定期購入配送パターン情報</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="center" colspan="2">定期購入の指定が可能な場合に設定します。</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">定期購入の利用の有無</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox id="cbFixedPurchaseFlg" Checked="<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID %>" OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" Runat="server"></asp:CheckBox></td>
														</tr>
													</tbody>
													<tbody id="tbFixedPurchase" runat="server">
														<tr>
															<td class="edit_title_bg" align="left" width="30%">配送パターン</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox id="cbFixedPurchaseKbn1Flg" Checked="<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_VALID %>" Text="月間隔日付指定" Runat="server"/>
																<asp:CheckBox id="cbFixedPurchaseKbn2Flg" Checked="<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_VALID %>" Text="月間隔・週・曜日指定" Runat="server" />
																<asp:CheckBox id="cbFixedPurchaseKbn3Flg" Checked="<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_VALID %>" Text="配送日間隔指定" OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" Runat="server"/>
																<asp:CheckBox id="cbFixedPurchaseKbn4Flg" Checked="<%# ((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_VALID) && (Constants.BOTCHAN_OPTION == false) %>" Text="週間隔・曜日指定" Runat="server" Enabled="<%# Constants.BOTCHAN_OPTION == false %>" />
																<span id="spFixedPurchaseShippingNotDisplayCheckBox" Runat="server">
																	（<asp:CheckBox id="cbFixedPurchaseShippingNotDisplayFlg" Checked="<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_NOTDISPLAY_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_DEFAULT_SETTING_FLG_VALID %>" Text="フロント非表示&emsp;※チェックを入れるとすべての配送パターンが非表示になります。" Runat="server"/>）
																</span>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">月間隔選択肢</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbFixedPurchaseKbn1Setting" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING] %>" runat="server" Width="250" MaxLength="200"></asp:TextBox>
															<br />「月間隔日付指定」または「月間隔・週・曜日指定」を選んだときに選択可能な配送間隔月（","カンマ区切りで複数指定）</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">日付選択肢</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbFixedPurchaseKbn1Setting2" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2] %>" runat="server" Width="250" MaxLength="150"></asp:TextBox>
																
															<br /> 1～28の数字、または"<%:ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST, Constants.DATE_PARAM_END_OF_MONTH_VALUE) %>"をカンマ区切りで入力してください
															<br />「月間隔日付指定」を選んだときに選択可能な日付選択肢（","カンマ区切りで複数指定）
															<br />「( )で括られた日付選択肢は、管理画面でのみ選択可能です。（例：1,(2),(3),(<%:ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST, Constants.DATE_PARAM_END_OF_MONTH_VALUE) %>) → フロント：1日&emsp;&emsp;管理：1日 or 2日 or 3日 or <%:ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST, Constants.DATE_PARAM_END_OF_MONTH_VALUE) %>）」
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">配送間隔選択肢</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbFixedPurchaseKbn3Setting" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING] %>" runat="server" Width="250" MaxLength="200"></asp:TextBox>
																<br />「配送日間隔指定」を選んだときに選択可能な配送間隔（","カンマ区切りで複数指定）
																<br />「( )で括られた配送間隔は、管理画面でのみ選択可能です。（例：30,(60),(90) → フロント：30日&emsp;&emsp;管理：30日 or 60日 or 90日）」
																<br /><span visible="<%# Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG %>" Runat="server">「※先頭の値がフロントに表示される初期値になります。」</span>
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">週間隔選択肢</td>
															<td class="edit_item_bg" align="left"><asp:TextBox id="tbFixedPurchaseKbn4Setting1" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1] %>" runat="server" Width="250" MaxLength="200" Enabled="<%# Constants.BOTCHAN_OPTION == false %>"></asp:TextBox>
																<br />「週間隔・曜日指定」を選んだときに選択可能な週間隔（","カンマ区切りで複数指定）
																<br />「( )で括られた週間隔は、管理画面でのみ選択可能です。（例：1,(2),(3) → フロント：1週間&emsp;&emsp;管理：1週間 or 2週間 or 3週間）」
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">曜日選択肢</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBoxlist id="cblDayOfWeek" runat="server" DataTextField="Key" DataValueField="Value"	RepeatDirection="Horizontal" RepeatLayout="Flow" Enabled="<%# Constants.BOTCHAN_OPTION == false %>">
																	<asp:ListItem Value="0">日曜日</asp:ListItem>
																	<asp:ListItem Value="1">月曜日</asp:ListItem>
																	<asp:ListItem Value="2">火曜日</asp:ListItem>
																	<asp:ListItem Value="3">水曜日</asp:ListItem>
																	<asp:ListItem Value="4">木曜日</asp:ListItem>
																	<asp:ListItem Value="5">金曜日</asp:ListItem>
																	<asp:ListItem Value="6">土曜日</asp:ListItem>
																</asp:CheckBoxList>
																<br />「週間隔・曜日指定」を選んだときに選択可能な曜日
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">配送キャンセル期限</td>
															<td class="edit_item_bg" align="left">
																次回配送日の <asp:TextBox id="tbFixedPurchaseDaysCancel" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_CANCEL_DEADLINE] %>" runat="server" Width="50" MaxLength="3"></asp:TextBox> 日前 （マイページでスキップ可能な期限／次回配送日変更可能期限）
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">自動注文タイミング</td>
															<td class="edit_item_bg" align="left">
																次回配送日の <asp:TextBox id="tbFixedPurchaseDaysOrder" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_ORDER_ENTRY_TIMING] %>" runat="server" Width="50" MaxLength="3"></asp:TextBox> 日前 （バッチ処理による自動注文登録）
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">配送所要日数</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbFixedPurchaseDaysRequired" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED] %>" runat="server" Width="50" MaxLength="3"></asp:TextBox> 日 （配送希望日が指定されないときの初回配送日計算に利用／次回配送日変更時に、今日から最短配送日までにあける日数）　※営業日は考慮しておりません
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">最低配送間隔</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox id="tbFixedPurchaseDaysSpan" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN] %>" runat="server" Width="50" MaxLength="3"></asp:TextBox> 日 （今回の配送日と次回の配送日の間に最低限あける日数）
															</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">次回配送日の選択可能最大日数</td>
															<td class="edit_item_bg" align="left">
																次回配送日の
																<asp:TextBox id="tbNextShippingMaxChangeDays" Text="<%# m_htParam[Constants.FIELD_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS] %>" runat="server" Width="50" MaxLength="2" /> 日後まで（マイページで次回配送日変更時に選択可能な最大日数）
															</td>
														</tr>
														<% if (Constants.FIXED_PURCHASE_FIRST_SHIPPING_DATE_NEXT_MONTH_ENABLED) { %>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">初回配送日</td>
															<td class="edit_item_bg" align="left">
																<asp:RadioButtonList
																	ID="rblFixedPurchaseFirstShippingNextMonthFlg"
																	DataTextField="Text"
																	DataValueField="Value"
																	AutoPostBack="true"
																	SelectedValue='<%#: (string.IsNullOrEmpty((string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG]) == false)
																		? m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG]
																		: Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG_INVALID %>'
																	RepeatLayout="Flow"
																	runat="server">
																</asp:RadioButtonList>
																<div style="margin-left: 30px;">※配送方法が宅配便かつ、配送パターンが月間隔日付指定、月間隔・週・曜日指定の場合のみ、適用されます。</div>
															</td>
														</tr>
														<% } %>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">定期配送料無料フラグ</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBox id="cbFixedPurchaseFreeShippingFlg" Checked="<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_VALID %>" Runat="server"></asp:CheckBox>
																定期配送料を無料にします
															</td>
														</tr>
													</tbody>
												</table>
												<% } %>
												<% if (Constants.GIFTORDER_OPTION_ENABLED) { %>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">のし情報</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="center" colspan="2">のしの種類の指定が可能な場合に設定します。</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">のし設定の利用の有無</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox id="cbWrappingPaperFlg" Checked='<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID %>' OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" Runat="server"></asp:CheckBox></td>
														</tr>
													</tbody>
													<tbody id="tbWrappingPaper" runat="server">
														<tr>
															<td class="edit_title_bg" align="left">のしの種類</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbWrappingPaperTypes" Text='<%# string.Join("\r\n", StringUtility.SplitCsvLine((string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_TYPES])) %>' TextMode="MultiLine" Width="250" Rows="5" runat="server"></asp:TextBox>
																<br />「のしの種類を指定」を選んだときに選択可能な種類（改行区切りで複数指定）
															</td>
														</tr>
													</tbody>
												</table>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">包装情報</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="center" colspan="2">包装の種類の指定が可能な場合に設定します。</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">包装設定の利用の有無</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox id="cbWrappingBagFlg" Checked='<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID %>' OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" Runat="server"></asp:CheckBox></td>
														</tr>
													</tbody>
													<tbody id="tbWrappingBag" runat="server">
														<tr>
															<td class="edit_title_bg" align="left">包装の種類</td>
															<td class="edit_item_bg" align="left">
																<asp:TextBox ID="tbWrappingBagTypes" Text='<%# string.Join("\r\n", StringUtility.SplitCsvLine((string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_TYPES])) %>' TextMode="MultiLine" Width="250" Rows="5" runat="server"></asp:TextBox>
																<br />「包装の種類の指定」を選んだときに選択可能な種類（改行区切りで複数指定）
															</td>
														</tr>
													</tbody>
												</table>
												<% } %>
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
														<tr>
															<td class="edit_title_bg" align="center" colspan="2">決済種別</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="center" colspan="2">決済種別を個別に指定する場合に設定します。</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" width="30%">決済種別任意指定の利用の有無</td>
															<td class="edit_item_bg" align="left"><asp:CheckBox id="cbPaymentFlg" Checked='<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG] == Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID %>' OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" Runat="server"></asp:CheckBox></td>
														</tr>
													</tbody>
													<tbody id="tbPayment" runat="server">
														<tr>
															<td class="edit_title_bg" align="left">決済種別の種類</td>
															<td class="edit_item_bg" align="left">
																<asp:CheckBoxlist id="cblPaymentKbn" runat="server" DataTextField="Key" DataValueField="Value"	RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="4"></asp:CheckBoxlist>
															</td>
														</tr>
													</tbody>
												</table>
												<br />
												<table class="detail_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="detail_title_bg" align="center" colspan="2">配送サービス</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">宅配便</td>
														<td class="detail_item_bg" align="left">
															<table class="detail_table" cellspacing="1" cellpadding="3" width="520" border="0">
																<tr>
																	<td class="detail_item_bg" align="left" width="10%">利用</td>
																	<td class="detail_item_bg" align="left" width="10%">初期</td>
																	<td class="detail_item_bg" align="left" >配送サービス名</td>
																</tr>
																<asp:Repeater ID="rExpress" DataSource="<%# this.DeliveryCompanyList %>" runat="server">
																<ItemTemplate>
																	<tr>
																		<td class="detail_item_bg" align="left" width="10%"><asp:CheckBox ID="cbUseExpress" Checked="<%# IsShopShippingCompanyUse(Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS, ((DeliveryCompanyModel)Container.DataItem).DeliveryCompanyId) %>" value="<%# ((DeliveryCompanyModel)Container.DataItem).DeliveryCompanyId %>" runat="server" /></td>
																		<td class="detail_item_bg" align="left" width="10%"><w2cm:RadioButtonGroup ID="rbgShopShippingCompanyDefaultExpress" Checked="<%# IsShopShippingCompanyDefault(Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS, ((DeliveryCompanyModel)Container.DataItem).DeliveryCompanyId) %>" GroupName='rbDefaultExpress' CssClass="radioBtn" runat="server" /></td>
																		<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(((DeliveryCompanyModel)Container.DataItem).DeliveryCompanyName) %></td>
																	</tr>
																</ItemTemplate>
																</asp:Repeater>
															</table>
														</td>
													</tr>
													<tr>
														<td class="detail_title_bg" align="left" width="30%">メール便</td>
														<td class="detail_item_bg" align="left">
															<table class="detail_table" cellspacing="1" cellpadding="3" width="520" border="0">
																<tr>
																	<td class="detail_item_bg" align="left" width="10%">利用</td>
																	<td class="detail_item_bg" align="left" width="10%">初期</td>
																	<td class="detail_item_bg" align="left">配送サービス名</td>
																	<% if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED) { %>
																		<td class="detail_item_bg" align="left" width="35%">メール便配送サイズ上限欄</td>
																	<%} %>
																</tr>
																<asp:Repeater ID="rMail" DataSource="<%# this.DeliveryCompanyList %>" runat="server">
																<ItemTemplate>
																	<tr>
																		<td class="detail_item_bg" align="left" width="10%"><asp:CheckBox ID="cbUseMail" Checked="<%# IsShopShippingCompanyUse(Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL, ((DeliveryCompanyModel)Container.DataItem).DeliveryCompanyId) %>" value="<%# ((DeliveryCompanyModel)Container.DataItem).DeliveryCompanyId %>" runat="server" /></td>
																		<td class="detail_item_bg" align="left" width="10%"><w2cm:RadioButtonGroup ID="rbgShopShippingCompanyDefaultMail" Checked="<%# IsShopShippingCompanyDefault(Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL, ((DeliveryCompanyModel)Container.DataItem).DeliveryCompanyId) %>" GroupName='rbDefaultMail' CssClass="radioBtn" runat="server" /></td>
																		<td class="detail_item_bg" align="left"><%# WebSanitizer.HtmlEncode(((DeliveryCompanyModel)Container.DataItem).DeliveryCompanyName) %></td>
																		<% if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED) { %>
																			<td class="detail_item_bg" align="left"><%#: ((DeliveryCompanyModel)Container.DataItem).DeliveryCompanyMailSizeLimit %></td>
																		<%} %>
																	</tr>
																	</tr>
																</ItemTemplate>
																</asp:Repeater>
															</table>
														</td>
													</tr>
												</table>
												<br />
												<% if (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED) { %>
												<!-- ▽配送料別途見積もり表示利用する場合▽ -->
												<br />
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tbody>
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">配送料の別途見積もり情報（<span class="edit_btn_sub"><a href="#note">備考</a></span>）</td>
													</tr>
													<tr>
														<td class="edit_item_bg" align="center" colspan="2">配送料を注文時に決めず、別途タイミングでオペレータが見積もり後に入力する場合に設定します。</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="30%">配送料の別途見積もり利用の有無</td>
														<td class="edit_item_bg" align="left">
															<asp:CheckBox id="cbShippingPriceSeparateEstimatesFlg" Runat="server" Checked='<%# (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID %>' OnCheckedChanged="RefreshComponents_OnCheckedChanged" AutoPostBack="true" ></asp:CheckBox>
														</td>
													</tr>
													<tr id="trShippingPriceSeparateEstimates" runat="server">
														<td class="edit_title_bg" align="left" width="30%">表記文言</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox id="tbShippingPriceSeparateEstimatesMessage" runat="server" Width="300" MaxLength="30" Text='<%# m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE] %>' ></asp:TextBox>
															<br />
															<asp:TextBox id="tbShippingPriceSeparateEstimatesMessageMobile" runat="server" Width="300" MaxLength="30" Text='<%# m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE_MOBILE] %>' ></asp:TextBox>（モバイル）
														</td>
													</tr>
													</tbody>
												</table>
												<br />
												<!-- △配送料別途見積もり表示利用する場合△ -->
												<%} %>
												<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr class="info_item_bg">
														<td id="payment_note" align="left" class="info_item_bg" colspan="2">備考<br />
                                                        ■配送可能時間帯情報<br />
                                                        <%: this.IsRepeatPlus ? "リピートPLUS" : "w2commerce" %>の送り状CSVをご利用の場合、時間帯コードを変更する際は、送り状CSVも変更が必要となりますので、<br />
                                                        変更前に必ずサポートまでご連絡ください。<br />
                                                        <br />
															<% if (Constants.FIXEDPURCHASE_OPTION_ENABLED) { %>
															■定期購入配送パターン情報<br />
																定期購入商品を運用する場合は「定期購入の利用の有無」を有効にする必要があります。<br />
																配送パターンはいずれかを選択する必要があり、「配送日間隔指定」が有効な場合は間隔を設定する必要があります。<br />
																<br />
															<dl>
																<dt>配送キャンセル期限：</dt>
																<dd style="margin-left:10px;">
																・マイページの定期購入情報で、エンドユーザが次の注文をスキップ・キャンセルできる期限を設定します。<br />
																　例）配送キャンセル期限=5日前 であれば、2013/1/15(火) に配送予定の注文は 2013/1/10(木) 中まではスキップ可能となります。<br />
																・自動注文タイミングよりも大きな値を設定してください。<br />
																</dd>
																<dt>自動注文タイミング：</dt>
																<dd style="margin-left:10px;">
																・バッチ処理で注文をたてるタイミングを、次回配送予定日の何日前にするかを設定します。<br />
																　例）自動注文タイミング=5日 のとき、次回配送予定日=2013/1/15(火) の注文については、5日前の 2013/1/10(木) の早朝のバッチで注文登録されます。<br />
																</dd>
																<dt>配送所要日数：</dt>
																<dd style="margin-left:10px;">
																・初回注文で配送希望日が選択されなかった場合に、初回配送予定日を算出するための目安の日数として利用します。<br />
																　例）配送所要日数=5日 のとき、2013/1/15(火) に 配送希望日"<%# ReplaceTag("@@DispText.shipping_date_list.none@@") %>" を選択して注文すると、初回配送予定日を1/20(日)とみなして以降の配送予定日が計算されます。<br />
																</dd>
																<dt>最低配送間隔：</dt>
																<dd style="margin-left:10px;">
																・注文登録時、初回配送予定日と次回配送日の間に最低限あける日数を設定します。<br />
																　例）最低配送間隔=5日 のとき、初回配送予定日=2013/1/15(火) 配送パターン=18日（毎月日付指定） と選択しても、1/18(金) は最低配送間隔(5日)以上あいていないので、次回配送日は翌月の 2013/2/18(月) となります。<br />
																</dd>
															</dl>

															<%} %>
															<br />
															■決済種別<br />
																支払い方法を配送種別毎に制限することが可能になります。<br />
																　※本設定を有効にした後、決済種別情報で無効とした場合はその決済は注文時に選択出来ないようになります。<br />
																<br />
															<% if (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED){ %>
															<span id="note">
															■配送料の別途見積もり情報<br />
																利用有無にチェックを付けている場合、Frontサイトで注文する際に配送料金の表記部分が設定した文言に置き換わります。<br />
																受注編集から見積もり後の配送料を入力することが可能です。また入力するまでデータ上は0円となります。<br />
																<br />
															</span>
															<%} %>
														</td>
													</tr>
												</table>
												<div class="action_part_bottom">
													<asp:Button ID="btnBackBottom" runat="server" Text="  戻る  " OnClick="btnBack_Click" />
													<asp:Button id="btnNextBottom" runat="server" Text="  配送料を指定する  " OnClick="btnNext_Click" />
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
		<td style="width: 797px; height: 10px"><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
