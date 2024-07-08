<%--
=========================================================================================================
  Module      : 商品別販売数ランキングページ(ProductSaleRankingReport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Product" %>
 <%@ Page Title="商品別販売数ランキングページ" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="ProductSaleRankingReport.aspx.cs" Inherits="Form_OrderConditionReport_ProductSaleRankingReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<!--▽ 一覧 ▽-->
	<tr>
		<td><h2 class="cmn-hed-h2">商品別販売数ランキング</h2></td>
	</tr>
	<tr>
		<td>
			<table class="box_border" cellspacing="1" cellpadding="0" width="784" border="0">
				<tr>
					<td>
						<table class="list_box_bg" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="center">
									<asp:UpdatePanel runat="server">
									<ContentTemplate>
										<table cellspacing="0" cellpadding="0" border="0">
											<tr>
												<td><img height="6" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td align="left">
													<!--▽ ページング ▽-->
													<table class="list_pager" cellspacing="0" cellpadding="0" border="0">
														<tr>
															<td width="450"><asp:Label id="lbPager1" Runat="server"></asp:Label></td>
															<td width="268" align="right" style="vertical-align:text-bottom" >
																<asp:RadioButtonList id="rblDisplayUnit" Width="100%" Runat="server" RepeatDirection="Horizontal" AutoPostBack="True" CssClass="radio_button_list" OnSelectedIndexChanged="rblDisplayUnit_SelectedIndexChanged">
																	<asp:ListItem Value="0">商品単位</asp:ListItem>
																	<asp:ListItem Value="1">バリエーション単位</asp:ListItem>
																</asp:RadioButtonList>
															</td>
															<td width="100" align="right" style="vertical-align:text-bottom" ><span class="search_btn_sub_rt" ><asp:LinkButton id="lbReportExport" Enabled="False" OnClick="lbReportExport_Click" Runat="server" >CSVダウンロード</asp:LinkButton></span></td>
														</tr>
													</table>
													<!-- ページング-->
												</td>
											</tr>
											<tr>
												<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr><td align="center">
												<div runat="server" ID="dvLoadingImg">
													<img alt="" src="../../Images/Common/loading.gif" width="20" height="20" border="0" />
													<asp:Literal runat="server" Text="現在レポート作成待ちです.." /><br/>
												</div>
											</td></tr>
											<tr><td align="center"><asp:Literal id="lProcessMessage" runat="server" /></td></tr>
											<tr>
												<td>
													<table class="list_table" cellspacing="1" cellpadding="3" width="750px" border="0">
														<tr class="list_title_bg">
															<td align="center" colspan="11">販売数リスト（<a href="#note">備考へ</a>）<span style="float:right">集計期間：<asp:Literal id="lTotalPeriod" runat="server"/></span></td>
														</tr>
														<!-- ▽バリエーション単位▽ -->
														<asp:Repeater Runat="server" DataSource="<%# this.OrderProductList %>" Visible="<%# this.IsVariationDisplay %>">
															<ItemTemplate>
																<tr class="list_title_bg" runat="server" visible="<%# (Container.ItemIndex % 20) == 0 %>">
																	<td align="center" width="80px" rowspan="3">画像</td>
																	<td align="center" width="150px">商品ID</td>
																	<td align="center" width="50px" rowspan="3">アイコン</td>
																	<td align="center" width="90px" rowspan="3">期中<br>販売数</td>
																	<td align="center" width="110px" rowspan="3">期中<br>売上計<br /><%= this.IsIncludedTax ? "（税込）" : "（税抜）"%></td>
																	<td align="center" width="90px" rowspan="3">平均売価<br /><%= this.IsIncludedTax ? "（税込）" : "（税抜）"%></td>
																	<td align="center" width="50px" rowspan="3">期中<br>消化率</td>
																	<td align="center" width="50px" rowspan="3">総<br>投入数</td>
																	<td align="center" width="80px" rowspan="3">総<br>販売数</td>
																	<td align="center" width="50px" rowspan="3">総<br>消化率</td>
																	<td align="center" width="80px" rowspan="3">在庫数</td>
																</tr>
																<tr class="list_title_bg" runat="server" visible="<%# (Container.ItemIndex % 20) == 0 %>">
																	<td align="center">バリエーションID</td>
																</tr>
																<tr class="list_title_bg" runat="server" visible="<%# (Container.ItemIndex % 20) == 0 %>">
																	<td align="center">商品名</td>
																</tr>
																<tr class="list_item_bg1" <%# ((decimal)Eval("total_digestive_rate") == 100) ? "style='background-color: #fde0e0;'" : "" %> >
																	<td align="center" rowspan="3"><%# ProductImage.GetHtmlImageTag(Container.DataItem, ProductType.Variation, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_M)%></td>
																	<td align="left"><%# Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID) %></td>
																	<td align="center" rowspan="3">
																		<span runat="server" visible='<%# ((string)Eval("icon1") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON1 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon2") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON2 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon3") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON3 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon4") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON4 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon5") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON5 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon6") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON6 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon7") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON7 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon8") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON8 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon9") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON9 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon10") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON10 %>" border="0" alt="" /><br></span>
																	</td>
																	<td align="right" rowspan="3"><%# Eval("item_quantity_total")%></td>
																	<td align="right" rowspan="3"><%# Eval("product_price_total").ToPriceString(true)%></td>
																	<td align="right" rowspan="3"><%# Eval("average_price").ToPriceString(true)%></td>
																	<td align="right" rowspan="3"><%# ((decimal)Eval("period_digestive_rate")).ToString("0.00")%>% </td>
																	<td align="right" rowspan="3"><%# Eval("total_input_count")%></td>
																	<td align="right" rowspan="3"><%# Eval("item_quantity_all_total")%></td>
																	<td align="right" rowspan="3"><%# ((decimal)Eval("total_digestive_rate")).ToString("0.00")%>% </td>
																	<td align="right" rowspan="3"><%# Eval(Constants.FIELD_PRODUCTSTOCK_STOCK)%></td>
																</tr>
																<tr class="list_item_bg1" <%# ((decimal)Eval("total_digestive_rate") == 100) ? "style='background-color: #fde0e0;'" : "" %> >
																	<td align="left"><%# ((string)Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID) != (string)Eval(Constants.FIELD_ORDERITEM_VARIATION_ID)) ? Eval(Constants.FIELD_ORDERITEM_VARIATION_ID) : "-"%></td>
																</tr>
																<tr class="list_item_bg1" <%# ((decimal)Eval("total_digestive_rate") == 100) ? "style='background-color: #fde0e0;'" : "" %> >
																	<td align="left"><%# Eval("name") %></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<!-- ▽商品単位▽ -->
														<asp:Repeater Runat="server" DataSource="<%# this.OrderProductList %>" Visible="<%# this.IsVariationDisplay == false%>">
															<ItemTemplate>
																<tr class="list_title_bg" runat="server" visible="<%# (Container.ItemIndex % 20) == 0 %>">
																	<td align="center" width="80px" rowspan="2">画像</td>
																	<td align="center" width="150px">商品ID</td>
																	<td align="center" width="50px" rowspan="2">アイコン</td>
																	<td align="center" width="90px" rowspan="2">期中<br>販売数</td>
																	<td align="center" width="110px" rowspan="2">期中<br>売上計<br /><%= this.IsIncludedTax ? "（税込）" : "（税抜）"%></td>
																	<td align="center" width="90px" rowspan="2">平均売価<br /><%= this.IsIncludedTax ? "（税込）" : "（税抜）"%></td>
																	<td align="center" width="50px" rowspan="2">期中<br>消化率</td>
																	<td align="center" width="50px" rowspan="2">総<br>投入数</td>
																	<td align="center" width="80px" rowspan="2">総<br>販売数</td>
																	<td align="center" width="50px" rowspan="2">総<br>消化率</td>
																	<td align="center" width="80px" rowspan="2">在庫数</td>
																</tr>
																<tr class="list_title_bg" runat="server" visible="<%# (Container.ItemIndex % 20) == 0 %>">
																	<td align="center">商品名</td>
																</tr>
																<tr class="list_item_bg1" <%# ((decimal)Eval("total_digestive_rate") == 100) ? "style='background-color: #fde0e0;'" : "" %> >
																	<td align="center" rowspan="2"><%# ProductImage.GetHtmlImageTag(Container.DataItem, ProductType.Product, SiteType.Pc, Constants.PRODUCTIMAGE_FOOTER_M)%></td>
																	<td align="left"><%# Eval(Constants.FIELD_ORDERITEM_PRODUCT_ID) %></td>
																	<td align="center" rowspan="2">
																		<span runat="server" visible='<%# ((string)Eval("icon1") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON1 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon2") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON2 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon3") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON3 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon4") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON4 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon5") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON5 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon6") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON6 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon7") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON7 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon8") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON8 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon9") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON9 %>" border="0" alt="" /><br></span>
																		<span runat="server" visible='<%# ((string)Eval("icon10") == Constants.FLG_PRODUCT_ICON_ON) %>'><img src="<%# Constants.PATH_ROOT_FRONT_PC + Constants.IMG_FRONT_PRODUCT_ICON10 %>" border="0" alt="" /><br></span>
																	</td>
																	<td align="right" rowspan="2"><%# Eval("item_quantity_total")%></td>
																	<td align="right" rowspan="2"><%# Eval("product_price_total").ToPriceString(true)%></td>
																	<td align="right" rowspan="2"><%# Eval("average_price").ToPriceString(true)%></td>
																	<td align="right" rowspan="2"><%# ((decimal)Eval("period_digestive_rate")).ToString("0.00")%>% </td>
																	<td align="right" rowspan="2"><%# Eval("total_input_count")%></td>
																	<td align="right" rowspan="2"><%# Eval("item_quantity_all_total")%></td>
																	<td align="right" rowspan="2"><%# ((decimal)Eval("total_digestive_rate")).ToString("0.00")%>% </td>
																	<td align="right" rowspan="2"><%# Eval(Constants.FIELD_PRODUCTSTOCK_STOCK)%></td>
																</tr>
																<tr class="list_item_bg1" <%# ((decimal)Eval("total_digestive_rate") == 100) ? "style='background-color: #fde0e0;'" : "" %> >
																	<td align="left"><%# Eval("name") %></td>
																</tr>
															</ItemTemplate>
														</asp:Repeater>
														<tr id="trListError" class="list_alert" runat="server" Visible="False">
															<td id="tdErrorMessage" colspan="11" runat="server"></td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
														<tr class="info_item_bg">
															<td align="left" id="note">備考<br />
																<dl style="margin-left:10px;">
																	<dt>
																	・売上状況レポートの検索条件を元にレポートを抽出しています。<br />
																	・背景が赤の場合、総消化率が100%であることを指します。<br />
																	・期中販売数は返品・交換個数を含んでいません。<br /><br />
																	</dt>
																	
																	<dt>画像</dt>
																	<dd style="margin-left:20px;">商品画像を表示します。&nbsp;※PC用画像を表示</dd>
																	<dt>商品ID</dt>
																	<dd style="margin-left:20px;">商品IDを表示します。</dd>
																	<dt>バリエーションID</dt>
																	<dd style="margin-left:20px;">バリエーションIDを表示します。</dd>
																	<dt>商品名</dt>
																	<dd style="margin-left:20px;">商品名を表示します。&nbsp;※商品マスタが存在しない場合は注文時の商品名を表示します。</dd>
																	<dt>アイコン</dt>
																	<dd style="margin-left:20px;">キャンペーンアイコン１－１０&nbsp;※注文当時の情報ではなく現在の商品マスタの値を参照</dd>
																	<dt>期中販売数</dt>
																	<dd style="margin-left:20px;">集計期間内の販売個数</dd>
																	<dt>期中売上計</dt>
																	<dd style="margin-left:20px;">集計期間内の売上金額</dd>
																	<dt>平均売価</dt>
																	<dd style="margin-left:20px;">期中売上計/期中販売数（%表示で、小数点四捨五入）</dd>
																	<dt>期中消化率</dt>
																	<dd style="margin-left:20px;">期中販売数/総投入数（%表示で、小数点3桁目四捨五入）</dd>
																	<dt>総投入数</dt>
																	<dd style="margin-left:20px;">総販売数＋在庫数</dd>
																	<dt>総販売数</dt>
																	<dd style="margin-left:20px;">期間に関係なく、今までに販売した全ての個数</dd>
																	<dt>総消化率</dt>
																	<dd style="margin-left:20px;">総販売数/総投入数（%表示で、小数点3桁目四捨五入）</dd>
																	<dt>在庫数</dt>
																	<dd style="margin-left:20px;">現在の在庫数を表示します。<br />
																	「論理在庫数が0以下」「在庫管理してない」「商品情報が存在しない」場合は「0」として表示/計算します。</dd>
																</dl>
															</td>
														</tr>
													</table>
												</td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</ContentTemplate>
									<Triggers>
										<asp:AsyncPostBackTrigger ControlID="tProcessTimer" EventName="Tick" />
										<asp:PostBackTrigger ControlID="lbReportExport" />
									</Triggers>
									</asp:UpdatePanel>
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
<asp:Timer id="tProcessTimer" Interval="1000" OnTick="tProcessTimer_Tick" Enabled="False" runat="server"/>
</asp:Content>