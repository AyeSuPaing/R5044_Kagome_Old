<%--
=========================================================================================================
  Module      : モール連携 設定編集ページ(MallLiaiseList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/DefaultPage.master" AutoEventWireup="true" CodeFile="MallConfig.aspx.cs" Inherits="Form_MallLiaise_MallConfig" %>
<%@ Register TagPrefix="uc" TagName="DateTimePickerPeriodInput" Src="~/Form/Common/DateTimePickerPeriodInput.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellspacing="0" cellpadding="0" width="791" border="0">
	<tr>
		<td><h1 class="page-title">モール連携基本設定</h1></td>
	</tr>
	<tr>
		<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
	</tr>
	<tr>
		<td><h1 class="cmn-hed-h2">モール連携基本設定編集</h1></td>
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
									<table cellspacing="1" cellpadding="2" width="758" border="0">

									<tr id="trCompMessage" runat="server" visible="false">
										<td>
											<div>
											<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
												<tr class="info_item_bg">
													<td align="left">モール連携基本設定を登録/更新しました。</td>
												</tr>
											</table>
											</div>
										</td>
									</tr>
									<tr>
										<td align="right" width="784">
											<asp:Button ID="btnBackTop" Text="  一覧へ戻る  " runat="server" OnClick="goList" />
											<asp:Button ID="btnInsertTop" Text="  登録する  " runat="server" OnClick="btnInsert_Click"  />
											<asp:Button ID="btnDeleteTop" Text="  削除する  " runat="server" OnClientClick="return confirm('設定を削除します。本当によろしいですか？')" OnClick="btnDelete_Click" />
											<asp:Button ID="btnCopyInsertTop" Text="  コピー新規登録する  " runat="server" OnClientClick="return confirm('現状を保存しますが、宜しいですか？');" OnClick="btnCopyInsert_Click" />
											<asp:Button ID="btnUpdateTop" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />
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
									<table cellspacing="0" cellpadding="2" width="784" border="0">
										<tr>
											<td class="info_box_bg" valign="top" align="center">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" colspan="2" align="center">モール連携基本設定</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" width="150">モールＩＤ<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbMallId" runat="server" width="150px" MaxLength="256"/></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">モール区分<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlMallKbn" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlMallKbn_SelectedIndexChanged"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">モール設定名<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbMallName" runat="server" width="250px" MaxLength="50"/></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">モール出品設定<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlMallExhibitsConfig" runat="server">
																<asp:ListItem Value=""> 全ての商品を出品 </asp:ListItem>
															</asp:DropDownList>
														</td>
													</tr>
													<% if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_FACEBOOK) { %>
													<tr id="trFacebook" runat="server">
														<td class="edit_title_bg" align="left">FaceBookアカウント設定<span class="notice"></span></td>
														<td class="edit_item_bg" align="left">
															カタログID<br />
															<asp:TextBox ID="tbFacebookCatalogId" runat="server" width="150px" MaxLength="30" /><br />
															アクセストークン<br />
															<asp:TextBox ID="tbFacebookAccessToken" runat="server" width="300px" MaxLength="256" /><br />
														</td>
													</tr>
													<% } %>
													<% if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL) { %>
													<tr>
														<td class="edit_title_bg" align="left">テナントコード設定<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left"><asp:TextBox ID="tbAndMallTenantCode" runat="server" width="150px" MaxLength="5"/></td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">ショップID設定<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbAndMallBaseStoreCode" runat="server" width="150px" MaxLength="8"/>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">店番設定<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbAndMallShopNo" runat="server" width="300px" MaxLength="32"/>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">SFTP 設定</td>
														<td class="edit_item_bg" align="left">
															SFTP サーバ接続先<br />
															<asp:TextBox ID="tbSftpHost" runat="server" width="150px" MaxLength="256"/><br />
															SFTP ポート<br />
															<asp:TextBox ID="tbSftpPort" runat="server" width="150px"/><br />
															SFTP ユーザ名<br />
															<asp:TextBox ID="tbSftpUserName" runat="server" width="150px" MaxLength="50"/><br />
															SFTP パスフレーズ<br />
															<asp:TextBox ID="tbSftpPassPhrase" runat="server" width="150px" MaxLength="50"/><br />
															SFTP 秘密鍵ファイル(.ppk)
															<% if (this.IsSftpPrivateKeyFileUploaded) { %>
																&nbsp;&nbsp;<span style="color: red">※アップロード済み</span>
															<% } %>
															<span id="spCopyFileNotice" style="color: red" visible="false" runat="server">※コピー元のファイルからコピーして更新します。</span>
															<br />
															<input id="fSftpPrivateKeyFileUpload" contentEditable="false" size="90" type="file" name="fSftpUpload" runat="server" />
															<asp:HiddenField ID="hfSftpPrivateKeyFilePath" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">在庫引当API設定</td>
														<td class="edit_item_bg" align="left">
															サイトコード<br />
															<asp:TextBox ID="tbSiteCode" runat="server" width="150px" MaxLength="30"/><br />
															サイト認証キー<br />
															<asp:TextBox ID="tbSignatureKey" runat="server" width="150px" MaxLength="30"/><br />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">商品連携ID<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlCooperationId" runat="server" ></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">商品バリエーション<br>連携ID<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:DropDownList ID="ddlVariationCooperationId" runat="server" ></asp:DropDownList>
														</td>
													</tr>
													<% } %>
													<% if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO) { %>
													<tr>
														<td class="edit_title_bg" align="left">ロハコ秘密鍵<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbLohacoPrivateKey" runat="server" TextMode="MultiLine" Rows="8" Width="600"></asp:TextBox>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">在庫連携有無</td>
														<td class="edit_item_bg" align="left"><asp:CheckBox ID="cbLohacoStockUpdateUseFlg" Checked="true" runat="server" Text="連携する"/></td>
													</tr>
													<% } %>
													<% if ((ddlMallKbn.SelectedValue != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO)
														&& (ddlMallKbn.SelectedValue != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON)
														&& (ddlMallKbn.SelectedValue != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL)
														&& (ddlMallKbn.SelectedValue != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE)
														&& (ddlMallKbn.SelectedValue != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_FACEBOOK)) { %>
													<tr>
														<td class="edit_title_bg" align="left">メール受信設定</td>
														<td class="edit_item_bg" align="left">
															対象メールアドレス<br />
															<asp:TextBox ID="tbTgtAddr" runat="server" width="300px" MaxLength="256"/><br />
															POP サーバ<br />
															<asp:TextBox ID="tbPopServer" runat="server" width="150px" MaxLength="100"/><br />
															POP サーバポート<br />
															<asp:TextBox ID="tbPopPort" runat="server" width="50px" MaxLength="5"/><br />
															POP ユーザ名<br />
															<asp:TextBox ID="tbPopUserName" runat="server" width="150px" MaxLength="50"/><br />
															POP パスワード<br />
															<asp:TextBox ID="tbPopPassword" runat="server" width="150px" MaxLength="50"/><br />
															<asp:CheckBox ID="cbPopApop" runat="server" />APOP有効
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">FTP 設定</td>
														<td class="edit_item_bg" align="left">
															FTP サーバ<br />
															<asp:TextBox ID="tbFtpHost" runat="server" width="150px" MaxLength="256"/><br />
															FTP ユーザ名<br />
															<asp:TextBox ID="tbFtpUserName" runat="server" width="150px" MaxLength="50"/><br />
															FTP パスワード<br />
															<asp:TextBox ID="tbFtpPassword" runat="server" width="150px" MaxLength="50"/><br />
															FTP アップロード先<br />
															<asp:TextBox ID="tbFtpUploadDir" runat="server" width="300px" MaxLength="256"/>
														</td>
													</tr>
													<% } %>
													<%if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO) { %>
													<tr>
														<td class="edit_title_bg" align="left">Yahoo API 設定</td>
														<td class="edit_item_bg" align="left">
															Client ID (アプリケーションID)<br />
															<asp:TextBox ID="tbYahooApiClientId" runat="server" width="500px" MaxLength="56"/><br />
															Client Secret<br />
															<asp:TextBox ID="tbYahooApiClientSecret" runat="server" width="350px" MaxLength="40"/><br />
															セラーID<br />
															<asp:TextBox ID="tbYahooApiSellerId" runat="server" width="500px" MaxLength="128"/><br />
															アクセストークン<br />
															<asp:TextBox ID="tbYahooApiAccessToken" runat="server" width="700px" Enabled="False"/><br />
															アクセストークン有効期限日時<br />
															<asp:TextBox ID="tbYahooApiAccessTokenExpirationDatetime" runat="server" width="150px" Enabled="False"/><br />
															リフレッシュトークン<br />
															<asp:TextBox ID="tbYahooApiRefreshToken" runat="server" width="700px" Enabled="False"/><br />
															リフレッシュトークン有効期限日時<br />
															<asp:TextBox ID="tbYahooApiRefreshTokenExpirationDatetime" runat="server" width="150px" Enabled="False"/><br /><br />
															<dv style="color:red;font-weight:bold"><asp:Label ID="lTokenExpirationAlert" runat="server" CssClass=""/></dv><br />
															<asp:LinkButton ID="lbRefreshYahooApiToken" runat="server" OnClick="lbRefreshYahooApiToken_OnClick" OnClientClick="return confirm('保存していない変更は破棄されます。トークンを取得しますか？')" width="150px" Text="トークンを取得する"/>
															<asp:LinkButton ID="lbForceRefreshYahooApiToken" runat="server" OnClick="lbForceRefreshYahooApiToken_OnClick" OnClientClick="return confirm('保存していない変更は破棄されます。トークンを取得しますか？')" Text="※リフレッシュトークンを更新"/>
															<br /><br />
															<asp:LinkButton ID="lbDeleteYahooApiToken" runat="server" OnClick="lbDeleteYahooApiToken_OnClick" OnClientClick="return confirm('トークンは削除してもよろしいでしょうか？')" width="150px" Text="トークンを削除する"/><br /><br /><br />
															公開鍵<br />
															<asp:TextBox ID="tbYahooApiPublicKey" runat="server" width="700px" TextMode="MultiLine" Rows="5"/><br />
															公開鍵バージョン<br />
															<asp:TextBox ID="tbYahooApiPublicKeyVersion" runat="server" width="50px"/><br />
															<dv style="color:red;font-weight:bold"><asp:Label ID="lPublicKeyAlert" Visible="False" runat="server" CssClass=""/></dv><br />
															最終認証日時<br />
															<asp:TextBox ID="tbYahooApiPublicKeyAuthorizedDatetime" runat="server" width="150px" Enabled="False"/><br /><br />
														</td>
													</tr>
													<%} %>
													<%if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN) { %>
													<tr>
														<td class="edit_title_bg" align="left">楽天あんしんメルアドサービス設定</td>
														<td class="edit_item_bg" align="left">
															SMTPサーバ<br /><asp:TextBox ID="tbSmtpServerName" runat="server" width="150px" MaxLength="50"/><br />
															ポート番号<br /><asp:TextBox ID="tbSmtpServerPort" runat="server" width="50px" MaxLength="5"/><br />
															SMTP AUTH ID<br /><asp:TextBox ID="tbSmtpServerUserName" runat="server" width="150px" MaxLength="256"/><br />
															SMTP AUTH パスワード<br /><asp:TextBox ID="tbSmtpServerPassword" runat="server" width="150px" MaxLength="50"/><br />
															店舗連絡先メールアドレス<br /><asp:TextBox ID="tbRakutenStoreMailAddress" runat="server" width="300px" MaxLength="256"/><br />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">楽天APIサービス設定<span class="notice">*</span></td>
														<td class="edit_item_bg" align="left">
															ユーザ－名<br /><asp:TextBox ID="tbRakutenApiUserName" runat="server" width="150px" MaxLength="50" /><br />
															店舗URL　※http://www.rakuten.co.jp/xxx/ の"xxx"部分を登録して下さい <br />
															<asp:TextBox ID="tbRakutenApiShopUrl" runat="server" width="150px" MaxLength="256"/><br />
															サービスシークレット<br /><asp:TextBox ID="tbRakutenApiServiceSecret" runat="server" width="300px" MaxLength="256"/><br />
															ライセンスキー　※ライセンスキーの有効期限が90日となりますので、再発行をご注意ください<br />
															<asp:TextBox ID="tbRakutenApiLicenseKey" runat="server" width="300px" MaxLength="50"/><br />
														</td>
													</tr>
												<%} %>
													<% if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON) { %>
													<tr>
														<td class="edit_title_bg" align="left">Amazonアカウント設定</td>
														<td class="edit_item_bg" align="left">
															出品者ID<br /><asp:TextBox ID="tbAmazonMerchantId" runat="server" width="150" MaxLength="30"/><br />
															マーケットプレイスID<br /><asp:TextBox ID="tbAmazonMarketPlaceId" runat="server" width="150" MaxLength="30"/><br />
															AWSアクセスキーID<br /><asp:TextBox ID="tbAwsAccessKeyId" runat="server" width="200" MaxLength="50"/><br />
															秘密キー<br /><asp:TextBox ID="tbAmazonSecretKey" runat="server" width="350" MaxLength="50"/><br />
															MWS認証トークン<br /><asp:TextBox ID="tbMwsAuthToken" runat="server" width="350" MaxLength="100"/><br />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">在庫連携有無</td>
														<td class="edit_item_bg" align="left"><asp:CheckBox ID="cbStockUpdateUseFlg" runat="server" Text="連携する"/></td>
													</tr>
													<% } %>
												<% if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE) { %>
													<tr>
														<td class="edit_title_bg" align="left">在庫連携設定</td>
														<td class="edit_item_bg" align="left">
															ストアアカウント<br /><asp:TextBox ID="tbNextEngineStockStoreAccount" runat="server" width="150" MaxLength="50"/><br />
															認証キー<br /><asp:TextBox ID="tbNextEngineStockAuthKey" runat="server" width="150" MaxLength="50"/><br />
														</td>
													</tr>
												<% } %>
													<tr>
														<td class="edit_title_bg" align="left">メンテナンス期間設定</td>
														<td class="edit_item_bg" align="left">
															<uc:DateTimePickerPeriodInput id="ucDateTimePickerPeriod" IsNullStartDateTime="true" IsNullEndDateTime="true" runat="server" />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">有効フラグ</td>
														<td class="edit_item_bg" align="left"><asp:CheckBox ID="cbVValidFlg" runat="server" Text="有効" /></td>
													</tr>
													<tr id="trDateCreated" runat="server" Visible="False">
														<td class="edit_title_bg" align="left">作成日</td>
														<td class="edit_item_bg" align="left"><asp:Literal ID="lDateCreated" runat="server"></asp:Literal></td>														</tr>
													<tr id="trDateChanged" runat="server" Visible="False">
														<td class="edit_title_bg" align="left">更新日</td>
														<td class="edit_item_bg" align="left"><asp:Literal ID="lDateChanged" runat="server"></asp:Literal></td>
													</tr>
													<tr id="trLastChanged" runat="server" Visible="False">
														<td class="edit_title_bg" align="left">最終更新者</td>
														<td class="edit_item_bg" align="left"><asp:Literal ID="lLastChanged" runat="server"></asp:Literal></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><img height="20" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
										</tr>
										<tr id="trProductConvRakuten" runat="server">
											<td class="info_box_bg" valign="top" align="center">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" colspan="3" align="center">商品コンバータ設定(楽天向け)</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" rowspan="<%: Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? 4 : 3 %>">バリエーションなし</td>
														<td class="edit_title_bg" align="left">商品登録</td>
														<td class="edit_item_bg" align="left">
															<%: Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? "「normal-item.csv」" : "「item.csv」" %><br />
															<asp:DropDownList ID="ddlCnvidRtnNInsItemcsv" runat="server"></asp:DropDownList>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">商品更新</td>
														<td class="edit_item_bg" align="left">
															<%: Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? "「normal-item.csv」" : "「item.csv」" %><br />
															<asp:DropDownList ID="ddlCnvidRtnNUpdItemcsv" runat="server"></asp:DropDownList><br />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">在庫更新</td>
														<td class="edit_item_bg" align="left">
															<%: Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? "「normal-item.csv」" : "「item.csv」" %><br />
															<asp:DropDownList ID="ddlCnvidRtnNStkItemcsv" runat="server"></asp:DropDownList><br />
														</td>
													</tr>
													<% if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION) { %>
													<tr>
														<td class="edit_title_bg" align="left">SKU管理番号フィールド名</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbRakutenSkuManagementIdOutputFormatForNormal" runat="server" width="350" MaxLength="500" /><br />
															入力した場合、商品コンバータ設定の列名が「SKU管理番号」の列の出力フォーマットがこの値で置き換わります<br />
															※楽天モールで複数店舗に連携する場合に設定します
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left" rowspan="<%: Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? 4 : 3 %>">バリエーションあり</td>
														<td class="edit_title_bg" align="left">商品登録</td>
														<td class="edit_item_bg" align="left">
															<%: Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? "「normal-item.csv」" : "「item.csv」" %><br />
															<asp:DropDownList ID="ddlCnvidRtnVInsItemcsv" runat="server"></asp:DropDownList><br />
															<% if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION == false){ %>
																「select.csv」<br />
																<asp:DropDownList ID="ddlCnvidRtnVInsSelectcsv" runat="server"></asp:DropDownList><br />
															<% } %>
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">商品更新</td>
														<td class="edit_item_bg" align="left">
															<%: Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? "「normal-item.csv」" : "「item.csv」" %><br />
															<asp:DropDownList ID="ddlCnvidRtnVUpdItemcsv" runat="server"></asp:DropDownList><br />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">在庫更新</td>
														<td class="edit_item_bg" align="left">
															<%: Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION ? "「normal-item.csv」" : "「item.csv」" %><br />
															<asp:DropDownList ID="ddlCnvidRtnVStkSelectcsv" runat="server"></asp:DropDownList><br />
														</td>
													</tr>
													<% if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION) { %>
													<tr>
														<td class="edit_title_bg" align="left">SKU管理番号フィールド名</td>
														<td class="edit_item_bg" align="left">
															<asp:TextBox ID="tbRakutenSkuManagementIdOutputFormatForVariation" runat="server" width="350" MaxLength="500" /><br />
															入力した場合、商品コンバータ設定の列名が「SKU管理番号」の列の出力フォーマットがこの値で置き換わります<br />
															※楽天モールで複数店舗に連携する場合に設定します
														</td>
													</tr>
													<% } %>
													<tr>
														<td class="edit_title_bg" align="left">共通</td>
														<td class="edit_title_bg" align="left">カテゴリ登録</td>
														<td class="edit_item_bg" align="left">
															「item-cat.csv」<br />
															<asp:DropDownList ID="ddlCnvidRtnItemCatcsv" runat="server"></asp:DropDownList>
														</td>
													</tr>
												</table>
											</td>
										</tr>
										
										<tr id="trProductConvYahoo" runat="server">
											<td class="info_box_bg" valign="top" align="center">
												<table class="edit_table" cellspacing="1" cellpadding="3" width="758" border="0">
													<tr>
														<td class="edit_title_bg" colspan="3" align="center">商品コンバータ設定(Yahoo!向け)</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" rowspan="3">バリエーションなし</td>
														<td class="edit_title_bg" align="left">商品登録</td>
														<td class="edit_item_bg" align="left">
															「data_add.csv」<br />
															<asp:DropDownList ID="ddlCnvidYhoAddDatacsv" runat="server"></asp:DropDownList><br />
															「quantity.csv」<br />
															<asp:DropDownList ID="ddlCnvidYhoAddQuantitycsv" runat="server"></asp:DropDownList>
														</td>														
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">商品更新</td>
														<td class="edit_item_bg" align="left">
															「data_add.csv」<br />
															<asp:DropDownList ID="ddlCnvidYhoDatacsv" runat="server"></asp:DropDownList><br />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">在庫更新</td>
														<td class="edit_item_bg" align="left">
															「quantity.csv」<br />
															<asp:DropDownList ID="ddlCnvidYhoQuantitycsv" runat="server"></asp:DropDownList><br />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left" rowspan="3">バリエーションあり</td>
														<td class="edit_title_bg" align="left">商品登録</td>
														<td class="edit_item_bg" align="left">
															「data_add.csv」<br />
															<asp:DropDownList ID="ddlCnvidYhoVAddDatacsv" runat="server"></asp:DropDownList><br />
															「quantity.csv」<br />
															<asp:DropDownList ID="ddlCnvidYhoVAddQuantitycsv" runat="server"></asp:DropDownList><br />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">商品更新</td>
														<td class="edit_item_bg" align="left">
															「data_add.csv」<br />
															<asp:DropDownList ID="ddlCnvidYhoVDatacsv" runat="server"></asp:DropDownList><br />
														</td>
													</tr>
													<tr>
														<td class="edit_title_bg" align="left">在庫更新</td>
														<td class="edit_item_bg" align="left">
															「quantity.csv」<br />
															<asp:DropDownList ID="ddlCnvidYhoVQuantitycsv" runat="server"></asp:DropDownList><br />
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
							<tr>
								<td align="center">
									<table cellspacing="1" cellpadding="2" width="758" border="0">
										<tr>
											<td align="right" width="784">
												<asp:Button ID="btnBackButtom" Text="  一覧へ戻る  " runat="server" OnClick="goList" />
												<asp:Button ID="btnInsertButtom" Text="  登録する  " runat="server" OnClick="btnInsert_Click" />
												<asp:Button ID="btnDeleteButtom" Text="  削除する  " runat="server" OnClientClick="return confirm('設定を削除します。本当によろしいですか？')" OnClick="btnDelete_Click" />
												<asp:Button ID="btnCopyInsertButtom" Text="  コピー新規登録する  " runat="server" OnClientClick="return confirm('現状を保存しますが、宜しいですか？');" OnClick="btnCopyInsert_Click" />
												<asp:Button ID="btnUpdateButtom" Text="  更新する  " runat="server" OnClick="btnUpdate_Click" />
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td><img height="4" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
							</tr>
						</table>
						<% if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO) { %>
						<table class="info_table" cellspacing="1" cellpadding="3" width="758" border="0">
							<tr class="info_item_bg">
								<td align="left">備考<br />
									・アクセストークン<br />
									　Yahoo!ストアクリエーターから注文情報を取得するために必要な文字列データです。<br /><br />
									・リフレッシュトークン<br />
									　アクセストークンを更新するための文字列データです。<br />
									　有効期限内であれば、自動でアクセストークンを再取得できます。<br /><br />
									・「トークンを取得する」ボタン<br />
									　アクセストークンとリフレッシュトークンを取得するためのボタンです。<br />
									　以下タイミングで押下してください。<br />
									　－初めてアクセストークン・リフレッシュトークンを取得する際<br />
									　－リフレッシュトークンの有効期限が切れた後に、<br />
									　　アクセストークンとリフレッシュトークンを再取得する際<br /><br />
									・「※リフレッシュトークンを更新」ボタン<br />
									　リフレッシュトークンの有効期限に関わらず、<br />
									　アクセストークンとリフレッシュトークンを取得するためのボタンです。<br />
									　以下タイミングで押下してください。<br />
									　－リフレッシュトークンの有効期限が近づいた時に、<br />
									　　事前に再取得をして期限を延長しておく際<br />
								</td>
							</tr>
						</table>
						<% } %>
					</td>
				</tr>
			</table>
			<table>
				<tr>
					<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
</asp:Content>
