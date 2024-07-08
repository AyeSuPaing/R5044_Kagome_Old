<%--
=========================================================================================================
  Module      : 商品コンバータ 設定ページ(ProductConverterDetail.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="ProductConverterDetail.aspx.cs" Inherits="Form_ProductConverter_ProductConverterDetail" maintainScrollPositionOnPostBack="true" %>
<%@ MasterType VirtualPath="~/Form/Common/DefaultPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript" language="javascript">
<!--
// フィールド追加制御
function add_field(arg)
{
	if (document.getElementById('<%= hfProductConverterColumnFocus.ClientID%>').value != "")
	{
		document.getElementById(document.getElementById('<%= hfProductConverterColumnFocus.ClientID%>').value).value += arg;
	}
}

// 文字列の引用符チェックボックス制御
function on_product_converter_export_quote() 
{
	if (document.getElementById('<%# cbProductConverterIsQuote.ClientID %>').checked) 
	{
		document.getElementById('<%# divProductConverterExportQuote.ClientID %>').style.display = 'block';
	}
	else 
	{
		document.getElementById('<%# divProductConverterExportQuote.ClientID %>').style.display = 'none';
	}
}

// マスター種別ラジオボタン制御
function on_master_type_change() 
{
	if (document.getElementById('<%# rbMasterTypeProduct.ClientID %>').checked) 
	{
		document.getElementById('<%# divSourceTableStyleProduct.ClientID %>').style.display = 'inline';
		document.getElementById('<%# divSourceTableStyleProductView.ClientID %>').style.display = 'none';
	}
	else if (document.getElementById('<%# rbMasterTypeProductView.ClientID %>').checked)
	{
		document.getElementById('<%# divSourceTableStyleProduct.ClientID %>').style.display = 'none';
		document.getElementById('<%# divSourceTableStyleProductView.ClientID %>').style.display = 'inline';
	}
}
//-->
</script>
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">商品コンバータ設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td valign="top">
			<table class="box_border" cellspacing="1" cellpadding="3" width="784" border="0">
				<tr>
					<td class="info_box_bg" align="center">
						<table cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<tr>
								<td align="center">
									<table cellspacing="1" cellpadding="2" width="758px" border="0">
										<tr>
											<td>
												<div id="divComplete" runat="server" class="action_part_top" Visible="False">
													<table class="info_table" cellspacing="1" cellpadding="3" width="100%" border="0">
														<tr class="info_item_bg">
															<td align="left"><asp:Label ID="lMessage" runat="server"></asp:Label>
															</td>
														</tr>
													</table>
												</div>
												<div class="action_part_top">
													<asp:Button ID="btnBackTop" Text="  一覧へ戻る  " runat="server" OnClick="btnBack_Click" />
													<asp:Button ID="btnInsertTop" Text="  登録する  " runat="server" OnClick="UpdateProductConverter" />
													<asp:Button ID="btnCopyInsertTop" Text="  コピー新規登録する  " runat="server" OnClick="btnCopyInsert_Click" />
													<asp:Button ID="btnDeleteTop" Text="  削除する  " runat="server" OnClientClick="return confirm('本当に削除しますか？');" OnClick="btnDelete_Click" />
													<asp:Button ID="btnUpdateTop" Text="  更新する  " runat="server" OnClick="UpdateProductConverter" />
												</div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<tr>
								<td align="center">
									<table cellspacing="0" cellpadding="2" width="784px" border="0">
										<tr>
											<td valign="top" align="center">
												<table cellspacing="1" cellpadding="2" width="758px" border="0" class="list_table">
													<tr>
														<td class="edit_title_bg" align="center" colspan="4">基本情報</td>
													</tr>
													<tr>
														<td class="edit_title_bg" width="30%" colspan="1">
															商品コンバータ 設定名<span class="notice">*</span>
														</td>
														<td class="edit_item_bg" colspan="3">
															<asp:HiddenField ID="hfAdtoId" runat="server" />
															<asp:TextBox ID="tbProductConverterName" runat="server" width="280px" MaxLength="100" />
														</td>
													</tr>
													<tr>
 														<td class="edit_title_bg">
															出力ファイルパス
														</td>
														<td class="edit_item_bg" colspan="3">
															ROOT/<asp:TextBox ID="tbPath" runat="server" width="350px" MaxLength="256"/>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg">
															出力ファイル名<span class="notice">*</span>
														</td>
														<td class="edit_item_bg">
															<asp:TextBox width="180px" ID="tbFilename" runat="server" MaxLength="50"/><br />
														拡張子含め指定してください。例：sample.csv<br />
														ファイル名に日付を付ける事ができます。設定例：sample[yyyyMMdd].csv→sample20090324.csv
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" colspan="1">
															区切り文字
														</td>
														<td class="edit_item_bg" colspan="3">
															<asp:RadioButton ID="rbProductConverterSeparaterCSV" GroupName="separater" Text="カンマ区切り(CSV)" runat="server" />
															<asp:RadioButton ID="rbProductConverterSeparaterTSV" GroupName="separater" Text="タブ区切り(TSV)" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" colspan="1">
															文字コード
														</td>
														<td class="edit_item_bg" colspan="3">
															<asp:RadioButton ID="rbProductConverterCharacterCodeSHIFT" GroupName="CharacterCode" Text="ShiftJIS" runat="server" />
															<asp:RadioButton ID="rbProductConverterCharacterCodeUTF" GroupName="CharacterCode" Text="UTF-8" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" colspan="1">
															改行コード
														</td>
														<td class="edit_item_bg" colspan="3">
															<asp:RadioButton ID="rbProductConverterNewLineCRLF" GroupName="NewLine" Text="CRLF" runat="server" />
															<asp:RadioButton ID="rbProductConverterNewLineCR" GroupName="NewLine" Text="CR" runat="server" />
															<asp:RadioButton ID="rbProductConverterNewLineLF" GroupName="NewLine" Text="LF" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg">
															ヘッダ行出力
														</td>
														<td class="edit_item_bg" colspan="3">
															<asp:CheckBox ID="cbIsHeader" Text="出力する" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg">
															文字列の引用符
														</td>
														<td class="edit_item_bg" colspan="3">
															<div style="display:block;float:left;">
																<asp:CheckBox ID="cbProductConverterIsQuote" Text="利用する" runat="server" />
															</div>
															<div id="divProductConverterExportQuote" style="float:left;" runat="server">
																&nbsp;&nbsp;(項目の囲い文字)
																<asp:TextBox ID="tbQuote" runat="server" MaxLength="1" Width="40px"/>
																&nbsp;※ 文字中の引用符はエスケープされます（例：「"」→「""」）
															</div>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg">
															マスタ種別
														</td>
														<td class="edit_item_bg" colspan="3">
															<asp:RadioButton runat="server" ID="rbMasterTypeProduct" GroupName="masterType" Text="商品マスタ" OnClick="javascript:on_master_type_change();" />
															<asp:RadioButton runat="server" ID="rbMasterTypeProductView" GroupName="masterType" Text="商品マスタ＋商品バリエーションマスタ" OnClick="javascript:on_master_type_change();" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg">抽出条件</td>
														<td class="edit_item_bg" colspan="3">
															<asp:DropDownList width="300px" ID="lbExtractionPattern" runat="server"></asp:DropDownList>
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td>
									<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
								</td>
							</tr>
							<tr>
								<td align="center">
									<table cellspacing="1" cellpadding="2" width="758" border="0">
										<tr>
											<td valign="top" align="left" width="50%">
												<table cellspacing="1" cellpadding="3" width="370" border="0" class="list_table">
													<tr>
														<td class="edit_title_bg" align="center" colspan="2">出力項目設定</td>
													</tr>
													<tr class="list_title_bg">
														<td align="center" style="width:120px;">列名</td>
														<td align="center" style="width:250px;">出力フォーマット</td>
													</tr>
													<asp:Repeater id="rColumns" Runat="server">
														<ItemTemplate>
															<tr class="edit_item_bg" id="trProductConverterColumnRow<%# Container.ItemIndex %>">
																<td align="center" style="width:120px">
																	<asp:HiddenField runat="server" ID="hfProductConverterColumnNo" Value="<%# Container.ItemIndex %>"/>
																	<asp:TextBox width="110px" runat="server" ID="tbProductConverterColumnName" MaxLength="50" Text="<%# Eval(Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME) %>" />
																</td>
																<td align="center" style="width:250px">
																	<asp:TextBox width="240px" runat="server" ID="tbProductConverterColumnFormat" MaxLength="512" Text="<%# Eval(Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT) %>" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
												</table>
												<asp:HiddenField ID="hfProductConverterColumnSelected" Value="-1" runat="server"/>
												<asp:HiddenField ID="hfProductConverterColumnFocus" Value="" runat="server"/>
												<script type="text/javascript">
												<!--
													document.getElementById('<%=hfProductConverterColumnSelected.ClientID%>').value = -1;
												//-->
												</script>
												<table width="370px">
													<tr>
														<td  style="text-align:right">
															<asp:Button runat="server" Text="  追加  " ID="btnAddColumn" OnClick="btnAddColumn_Click" />
															<asp:Button runat="server" Text="  削除  " ID="btnDelColumn" OnClick="btnDelColumn_Click"/>
															<asp:Button runat="server" Text="▲" ID="btnUpColumn" OnClick="btnUpColumn_Click"/>
															<asp:Button runat="server" Text="▼" ID="btnDownColumn" OnClick="btnDownColumn_Click"/>
														</td>
													</tr>
												</table>
											</td>
											<td valign="top" align="right" width="50%">
												<div id="divSourceTableStyleProduct" runat="server">
													<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
														<tr>
															<td class="edit_title_bg" align="left" width="30%">マスタ種別</td>
															<td class="edit_item_bg" align="left">商品マスタ</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" colspan="2">フィールド</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="2">
															<div style="overflow:auto;height:300px;">
																<table class="edit_table" cellspacing="0" cellpadding="0" border="0">
																	<asp:Repeater id="rProductColumnNames" Runat="server">
																		<ItemTemplate>
																			<tr>
																				<td class="edit_item_bg" align="left">
																					<a href="javascript:add_field('<%# WebSanitizer.UrlAttrHtmlEncode(((Hashtable)Container.DataItem)[XML_MASTEREXPORTSETTING_NAME]) %>');">
																					←&nbsp;
																					<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[XML_MASTEREXPORTSETTING_J_NAME])%></a>&nbsp;
																					(<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[XML_MASTEREXPORTSETTING_NAME])%>)
																				</td>
																			</tr>
																		</ItemTemplate>
																	</asp:Repeater>
																</table>
															</div>
															</td>
														</tr>
													</table>
												</div>
												<div id="divSourceTableStyleProductView" runat="server">
													<table class="edit_table" cellspacing="1" cellpadding="3" width="370" border="0">
														<tr>
															<td class="edit_title_bg" align="left" width="30%">マスタ種別</td>
															<td class="edit_item_bg" align="left">商品マスタ＋商品バリエーションマスタ</td>
														</tr>
														<tr>
															<td class="edit_title_bg" align="left" colspan="2">フィールド</td>
														</tr>
														<tr>
															<td class="edit_item_bg" align="left" colspan="2">
															<div style="overflow:auto;height:300px;">
																<table class="edit_table" cellspacing="0" cellpadding="0" border="0">
																	<asp:Repeater id="rProductViewColumnNames" Runat="server">
																		<ItemTemplate>
																			<tr>
																				<td class="edit_item_bg" align="left">
																					<a href="javascript:add_field('<%# WebSanitizer.UrlAttrHtmlEncode(((Hashtable)Container.DataItem)[XML_MASTEREXPORTSETTING_NAME]) %>');">
																					←&nbsp;
																					<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[XML_MASTEREXPORTSETTING_J_NAME])%></a>&nbsp;
																					(<%# WebSanitizer.HtmlEncode(((Hashtable)Container.DataItem)[XML_MASTEREXPORTSETTING_NAME])%>)
																				</td>
																			</tr>
																		</ItemTemplate>
																	</asp:Repeater>
																</table>
															</div>
															</td>
														</tr>
													</table>
												</div>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td>
									<img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" />
								</td>
							</tr>
							<tr>
								<td align="center">
									<table cellspacing="1" cellpadding="2" width="758" border="0">
										<tr>
											<td valign="top">
												<table cellspacing="1" cellpadding="3" width="525px" border="0" class="list_table">
													<tr>
														<td class="edit_title_bg" align="center" colspan="3">文字列置換設定</td>
													</tr>
													<tr class="list_title_bg">
														<td style="width:175px" align="center">変換前</td>
														<td style="width:175px" align="center">変換後</td>
														<td style="width:175px" align="center">対象</td>
													</tr>
													<asp:Repeater id="rConvert" Runat="server">
														<ItemTemplate>
															<tr class="edit_item_bg" id="trProductConverterConvertRow<%# Container.ItemIndex %>">
																<td align="left" width="175px"><asp:TextBox width="165px" runat="server" ID="tbConvertFrom" MaxLength="100" Text="<%# Eval(Constants.FIELD_MALLPRDCNVRULE_CONVERTFROM) %>"/></td>
																<td align="left" width="175px"><asp:TextBox width="165px" runat="server" ID="tbConvertTo" MaxLength="100" Text="<%# Eval(Constants.FIELD_MALLPRDCNVRULE_CONVERTTO) %>"/></td>
																<td align="left" width="175px">
																	<asp:DropDownList Width="165px" runat="server" ID="ddlConvertTarget"  DataTextField="Text" DataValueField="Value" />
																	<asp:HiddenField runat="server" ID="hfConvertTargetValue" />
																</td>
															</tr>
														</ItemTemplate>
													</asp:Repeater>
													<asp:HiddenField id="hfProductConverterConvertSelected" Value="-1" runat="server" />
													<script type="text/javascript">
													<!--
													document.getElementById('<%=hfProductConverterConvertSelected.ClientID%>').value = -1;
													//-->
													</script>
												</table>
											</td>
										</tr>
										<tr>
											<td>
												<table width="525px">
													<tr>
														<td style="text-align:right">
															<asp:Button runat="server" Text="  追加  " ID="btnAddConvert" onclick="btnAddConvert_Click" />
															<asp:Button runat="server" Text="  削除  " ID="btnDelConvert" onclick="btnDelConvert_Click" />
															<asp:Button runat="server" Text="▲" ID="btnUpConvert"	onclick="btnUpConvert_Click"  />
															<asp:Button runat="server" Text="▼" ID="btnDownConvert"  onclick="btnDownConvert_Click" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr>
											<td>
												<table class="info_table" cellspacing="1" cellpadding="3" width="760" border="0">
													<tr class="info_item_bg">
														<td align="left">
<pre>備考
<strong>■出力項目設定の書式</strong>
 ○固定の文字列を出力する場合、ダブルクォート「"」で囲む。
 ○フィールド値を出力する場合、フィールド名を入力する。
 ○文字列長指定をする場合、出力フォーマット指定の末尾に、:LEN[文字列長]を指定する。
 ○バイト長指定をする場合、出力フォーマット指定の末尾に、:LENB[バイト長]を指定する。
 ○条件分岐の書式（IF/ELSE式）
   条件書式：「?=[フィールド名](?[演算子]値,["文字列" または フィールド名])(["文字列" または フィールド名])」
   使用可能な演算子：「=」「!」「<」「>」「<=」「>=」　
   ※「!」は「=」と逆の意味を持つ否定演算子です。「=」「!」は文字列型のフィールドにも使用可能です。
   （使用例１：「商品在庫数が1以上の場合はそのままの商品在庫数、商品在庫数が0以下の場合は商品在庫数を0で出力する」場合、
   　　　　　　　条件式を「?=stock(?>0,stock)("0")」と指定します。
   　　　　　　・「?=stock」は、「?」を在庫マスタの商品在庫数「stock」と扱うための宣言です。
   　　　　　　・「(?>0,stock)("0")」は、「商品在庫数は0を超える」の条件を満たす場合に
   　　　　　　　在庫マスタの商品在庫数「stock」を出力し、条件を満たさない場合は固定文字列「"0"」を出力する式を意味します。
   　使用例２：「商品名が"商品Ａ"と一致しない場合に固定文字列"Ｂ"、"商品Ａ"と一致する場合に固定文字列"Ａ"と出力する」場合、
   　　　　　　　条件式を「?=name(?!"商品Ａ","Ｂ")("Ａ")」と指定します。
   　　　　　　・「?=name」は、「?」を商品マスタの商品名「name」と扱うための宣言です。
   　　　　　　・「(?!"商品Ａ","Ｂ")("Ａ")」は、「商品名は"商品Ａ"と一致しない」の条件を満たす場合に
   　　　　　　　固定文字列「"Ｂ"」を出力し、条件を満たさない場合に固定文字列「"Ａ"」を出力する式を意味します。）
   ※単純なIF/ELSE式のみに対応しています。条件式を入れ子にすることはできません。
 ○特殊タグ
    <strong>[SP:point]</strong>
     商品情報にて加算数設定(pt)の場合はそのまま、加算率(%)の場合は計算して出力します。
    <% if (this.ProductIncludedTaxFlg == false){ %><strong>[SP:PricePretax]</strong>
     商品情報に設定された税率カテゴリを元に、税込み価格を出力します。
    <%} %><strong>[SP:MallStockAlertDispatch(拡張項目フィールド,{拡張項目フィールドの設定値:モールID})]…({}部分は繰返し可能)</strong>
     在庫安全基準値より在庫数が下回る商品の在庫数を、条件パラメータにより振り分けて出力します。
     ※使用例：[SP:MallStockAlertDispatch(extendXX,Param1:rakuten1,Param2:yahoo_1)]
     　拡張項目フィールド[extendXX]の値が[Param1]と等しい場合、モールID[rakuten1]に残りの在庫数を出力し、
     　モールID[yahoo_1]に対しては在庫数を"0"で出力します。
 ○楽天用特殊タグ
    <strong>[SP:RakutenSellRange]</strong>
 　　商品情報の販売期間Fromと販売期間Toを楽天フォーマット用に出力します。
 　　（出力例：2005/09/01 09:15 2010/08/31 12:31）
    <strong>[SP:RakutenCategory]</strong>
 　　拡張項目フィールド[拡張項目112]～[拡張項目116]に置き換えられ、レコードを出力します。
 　　空でない拡張項目フィールド[拡張項目112]～[拡張項目116]が複数ある場合、それらについて複数レコードを作成します。
 　　例：拡張項目フィールド[拡張項目112]、[拡張項目113]にデータがある場合は以下のように出力されます。
 　　　　----------------------------------------------------
　　　　　　…,[拡張項目112],…　　※商品１についてのレコード
　　　　　　…,[拡張項目113],…　　※商品１についてのレコード
　　　　　　…,[拡張項目112],…　　※商品２についてのレコード
　　　　　　…,[拡張項目113],…　　※商品２についてのレコード
 　　　　----------------------------------------------------
    <strong>[SP:RakutenTaxRate]</strong>
 　　商品情報の商品税率カテゴリIDの税率を楽天フォーマット用に出力します。
 　　楽天フォーマットで設定できる税率は「10%」「8%」「0%」のみです。
 　　※上記以外の税率または空を設定した場合は楽天店舗税率が適用されます。
 　　（出力例：10.00% → 0.1）
 ○Yahoo用特殊タグ
    <strong>[SP:YahooPath]</strong>
     YahooのPathをカテゴリ階層に合わせ分割指定します。
     拡張項目フィールド[拡張項目112]～[拡張項目116]に設定されているカテゴリを対象とします。
 　　（出力例：AAA:BBB:CCC →AAA[改行]AAA:BBB[改行]AAA:BBB:CCC）
    <strong>[SP:YahooSalePeriodStart]</strong>
 　　Yahooの「SalePeriodStart」の値を出力するタグです。
 　　※商品情報の販売期間FromをYahooフォーマット（yyyyMMddHH）用に出力します。
 　　（出力例：2009100109）
    <strong>[SP:YahooSalePeriodEnd]</strong>
 　　Yahooの「YahooSalePeriodEnd」の値を出力するタグです。
 　　※商品情報の販売期間ToをYahooフォーマット（yyyyMMddHH）用に出力します。
 　　（出力例：2010033018）
    <strong>[SP:YahooSubCode]</strong>
 　　Yahooの「YahooSubCode」の値を出力するタグです。（モールバリエーション種別が「s」以外の場合に出力します。）
 　　※拡張項目フィールド（[拡張項目49]と[拡張項目50]）と商品バリエーションごとの[表示名1][表示名2][商品バリエーションID]
 　　　を紐付ける情報を「&」区切りで出力します。
 　　　拡張項目フィールド[拡張項目49]と[拡張項目50]との区切りは「#」区切りで出力します。
 　　例1：拡張項目フィールド[拡張項目49]を設定済みの場合、以下の通りに出力します。
 　　　　 ----------------------------------------------------------------------------------------------
 　　　　　　[拡張項目49]:[表示名1(1つ目)]&[拡張項目49]:[表示名1(2つ目)]&[拡張項目49]:[表示名1(3つ目)]&…
 　　　　 ----------------------------------------------------------------------------------------------
 　　例2：拡張項目フィールド[拡張項目49]と[拡張項目50]を設定済みの場合、以下の通りに出力されます。
 　　　　 ----------------------------------------------------------------------------------------------
 　　　　　　[拡張項目49]:[表示名1(1つ目)]#[拡張項目50]:[表示名2(1つ目)]=[商品バリエーションID(1つ目)]&[改行なし]
 　　　　　　[拡張項目49]:[表示名1(2つ目)]#[拡張項目50]:[表示名2(2つ目)]=[商品バリエーションID(2つ目)]&[改行なし]
 　　　　　　[拡張項目49]:[表示名1(3つ目)]#[拡張項目50]:[表示名2(3つ目)]=[商品バリエーションID(3つ目)]&…
 　　　　 ----------------------------------------------------------------------------------------------
    <strong>[SP:YahooOptions]</strong>
 　　Yahooの「YahooOptions」の値を出力するタグです。（モールバリエーション種別の値によって出力値が異なります。）
 　　※拡張項目フィールド[拡張項目49]又は[拡張項目50]の設定値を出力後、
 　　　商品バリエーションごとに商品バリエーションの[表示名1][表示名2]を半角スペース区切りで出力します。
 　　例1：モールバリエーション種別が「s」の場合は、商品バリエーションの[表示名1]でグループ化し、以下の通りに出力します。
 　　　　 -------------------------------------------------------------------------------------------------
 　　　　　　[表示名1(GroupA)] [表示名2(GroupA:1つ目)] [表示名2(GroupA:2つ目)] [表示名2(GroupA:3つ目)]…[改行]
 　　　　　　[改行]
 　　　　　　[表示名1(GroupB)] [表示名2(GroupB:1つ目)] [表示名2(GroupB:2つ目)] [表示名2(GroupB:3つ目)]…
 　　　　 -------------------------------------------------------------------------------------------------
 　　例2：モールバリエーション種別が「s」以外の場合、以下の２通りの出力パターンがあります。
　　　　　1.拡張項目フィールド[拡張項目49]を設定済みの場合、以下の通りに出力します。
 　　　　   --------------------------------------------------------------------------
 　　　　　　　[拡張項目49] [表示名1(1つ目)]　[表示名1(2つ目)] [表示名1(3つ目)]…
 　　　　 　--------------------------------------------------------------------------
 　　　　 2.拡張項目フィールド[拡張項目49]と[拡張項目50]を設定済みの場合、以下の通りに出力します。
 　　　　 　--------------------------------------------------------------------------
 　　　　　　　[拡張項目49] [表示名1(1つ目)]　[表示名1(2つ目)] [表示名1(3つ目)]…[改行]
 　　　　　　　[改行]
 　　　　　　　[拡張項目50] [表示名2(1つ目)]　[表示名2(2つ目)] [表示名2(3つ目)]…
 　　　　 　--------------------------------------------------------------------------　　　　　　
    <strong>[SP:YahooTaxRate]</strong>
 　　商品情報の商品税率カテゴリIDの税率をYahooフォーマット用に出力します。
 　　Yahooフォーマットで設定できる税率は「10%」「8%」のみです。
 　　※上記以外の税率または空を設定した場合はYahoo店舗税率が適用されます。
 　　（出力例：10.00% → 0.1）

<strong>■文字列置換設定の書式</strong>
 ○「対象」ドロップダウンで選択した項目の出力文字列を「変換前」→「変換後」の文字列に置き換えます。
 ○出力項目設定の条件分岐式で出力する文字列についても同様に文字列を置き換えることができます。
</pre>
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td align="center">
									<table cellspacing="1" cellpadding="2" width="758px" border="0">
										<tr>
											<td align="right" width="784px">
												<asp:Button ID="btnBackBottom" Text="  一覧へ戻る  " runat="server" OnClick="btnBack_Click" />
												<asp:Button ID="btnInsertBottom" Text="  登録する  " runat="server" OnClick="UpdateProductConverter" />
												<asp:Button ID="btnCopyInsertBottom" Text="  コピー新規登録する  " runat="server" OnClick="btnCopyInsert_Click" />
												<asp:Button ID="btnDeleteBottom" Text="  削除する  " runat="server" OnClientClick="return confirm('本当に削除しますか？');" OnClick="btnDelete_Click" />
												<asp:Button ID="btnUpdateBottom" Text="  更新する  " runat="server" OnClick="UpdateProductConverter" />
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
		<td>
			<table>
				<tr>
					<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
</asp:Content>



