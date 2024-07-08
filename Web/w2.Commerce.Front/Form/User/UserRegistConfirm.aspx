<%--
=========================================================================================================
  Module      : 会員登録確認画面(UserRegistConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Register TagPrefix="uc" TagName="BodyUserExtendRegist" Src="~/Form/Common/User/BodyUserExtendRegist.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/UserRegistConfirm.aspx.cs" Inherits="Form_User_UserRegistConfirm" Title="会員新規登録確認ページ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="dvUserContents">
	<%-- 会員登録系パンくず --%>
	<div id="dvHeaderRegistClumbs">
		<p>
			<img src="../../Contents/ImagesPkg/user/clumbs_regist_3.gif" alt="入力内容の確認" /></p>
	</div>
	
	<div id="dvUserRegistConfirm" class="unit">
		<%-- メッセージ --%>
		<div class="dvContentsInfo">
			<p>
				お客様の入力された内容は以下の通りでよろしいでしょうか？<br />
				よろしければ「登録する」ボタンを押して下さい。</p>
		</div>
		
		<div class="dvUserInfo">
			<h3>お客様情報</h3>
			<table cellspacing="0">
				<tr>
					<th>
						<%-- 氏名 --%>
						<%: ReplaceTag("@@User.name.name@@") %></th>
					<td>
						<%: this.UserInput.Name1 %>
						<%: this.UserInput.Name2 %>
						<% if (this.IsUserAddrJp) { %>
						（<%: this.UserInput.NameKana1 %>
						<%: this.UserInput.NameKana2 %>）
						<% } %>
					</td>
				</tr>
				<%if (Constants.PRODUCTREVIEW_ENABLED) { %>
				<tr>
					<th>
						<%-- ニックネーム --%>
						<%: ReplaceTag("@@User.nickname.name@@") %></th>
					<td>
						<%: this.UserInput.NickName %></td>
				</tr>
				<%} %>
				<tr>
					<th>
						<%-- 生年月日 --%>
						<%: ReplaceTag("@@User.birth.name@@") %></th>
					<td>
					<%if (this.UserInput.Birth != null) {%>
						<%: this.UserInput.BirthYear %>年
						<%: this.UserInput.BirthMonth %>月
						<%: this.UserInput.BirthDay %>日
					<%} %></td>
				</tr>
				<tr>
					<th>
						<%-- 性別 --%>
						<%: ReplaceTag("@@User.sex.name@@") %></th>
					<td>
						<%: this.UserInput.SexValueText %></td>
				</tr>
				<tr>
					<th>
						<%-- ＰＣメールアドレス --%>
						<%: ReplaceTag("@@User.mail_addr.name@@") %></th>
					<td>
						<%: this.UserInput.MailAddr %>
						<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED) {%>
						<span class="notes">※ログイン時に利用します</span>
						<%} %></td>
				</tr>
				<% if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) { %>
				<tr>
					<th>
						<%-- モバイルメールアドレス --%>
						<%: ReplaceTag("@@User.mail_addr2.name@@") %></th>
					<td>
						<%: this.UserInput.MailAddr2 %></td>
				</tr>
				<% } %>
				<tr>
					<th>
						<%: ReplaceTag("@@User.addr.name@@") %>
					</th>
					<td>
						<% if (this.IsUserAddrJp) { %>〒<%: this.UserInput.Zip %><br /><% } %>
						<%: this.UserInput.Addr1 %> <%: this.UserInput.Addr2 %><br />
						<%: this.UserInput.Addr3 %> <%: this.UserInput.Addr4 %><br />
						<% if (this.IsUserAddrJp == false) { %><%: this.UserInput.Addr5 %> <%: this.UserInput.Zip %><br /><% } %>
						<%: this.UserInput.AddrCountryName %>
					</td>
				</tr>
				<% if (Constants.DISPLAY_CORPORATION_ENABLED){ %>
				<tr>
					<th>
						<!-- 企業名 -->
						<%: ReplaceTag("@@User.company_name.name@@")%>
					</th>
					<td>
						<%: this.UserInput.CompanyName %></td>
				</tr>
				<tr>
					<th>
						<!-- 部署名 -->
						<%: ReplaceTag("@@User.company_post_name.name@@")%>
					</th>
					<td>
						<%: this.UserInput.CompanyPostName %></td>
				</tr>
				<%}%>
				<tr>
					<%-- 電話番号 --%>
					<th><%: ReplaceTag("@@User.tel1.name@@", this.UserAddrCountryIsoCode) %></th>
					<td><%: this.UserInput.Tel1 %></td>
				</tr>
				<tr>
					<th><%: ReplaceTag("@@User.tel2.name@@", this.UserAddrCountryIsoCode) %></th>
					<td><%: this.UserInput.Tel2 %></td>
				</tr>
				<tr>
					<th>
						<%: ReplaceTag("@@User.mail_flg.name@@") %>
					</th>
					<td>
						<%: this.UserInput.MailFlgValueText %></td>
				</tr>
				<%-- ユーザー拡張項目　HasInput:true(入力画面)/false(確認画面)　HasRegist:true(新規登録)/false(登録編集) --%>
				<uc:BodyUserExtendRegist ID="ucBodyUserExtendRegist" runat="server" HasInput="false" HasRegist="true" />
			</table>
		</div>
		<%if ((Constants.PAYMENT_GMO_POST_ENABLED) && (this.UserInput.BusinessOwner != null)) { %>
			<div class="dvUserInfo">
				<h3>GMO</h3>
				<table cellspacing="0">
					<%--GMO--%>
						<tr>
							<th>
								<%: ReplaceTag("@@User.OwnerName1.name@@") %>
							</th>
							<td>
								<%: this.UserInput.BusinessOwner.OwnerName1 %>
							</td>
						</tr>
						<tr>
							<th>
								<%: ReplaceTag("@@User.OwnerName2.name@@") %>
							</th>
							<td>
								<%: this.UserInput.BusinessOwner.OwnerName2 %>
							</td>
						</tr>
						<tr>
							<th>
								<%: ReplaceTag("@@User.OwnerNameKana1.name@@") %>
							</th>
							<td>
								<%: this.UserInput.BusinessOwner.OwnerNameKana1 %>
							</td>
						</tr>
						<tr>
							<th>
								<%: ReplaceTag("@@User.OwnerNameKana2.name@@") %>
							</th>
							<td>
								<%: this.UserInput.BusinessOwner.OwnerNameKana2 %>
							</td>
						</tr>
						<tr>
							<th>
								<%: ReplaceTag("@@User.OwnerBirth.name@@") %>
							</th>
							<td>
								<%if ((this.UserInput.BusinessOwner.BirthYear + this.UserInput.BusinessOwner.BirthMonth +
									this.UserInput.BusinessOwner.BirthDay).Length != 0){%>
									<%: this.UserInput.BusinessOwner.BirthYear %>年
										<%: this.UserInput.BusinessOwner.BirthMonth %>月
											<%: this.UserInput.BusinessOwner.BirthDay %>日
												<%} %>
							</td>
						</tr>
						<tr>
							<th>
								<%: ReplaceTag("@@User.RequestBudget.name@@") %>
							</th>
							<td>
								<%: CurrencyManager.ToPrice(this.UserInput.BusinessOwner.RequestBudget) %> 
							</td>
						</tr>
						
				</table>
			</div>
		<%}%>
		<%if ((Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) || this.IsVisible_UserPassword) { %>
		<div class="dvLoginInfo">
			<h3>ログイン情報</h3>
			<table cellspacing="0">
				<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED == false) { %>
				<tr>
					<th>
						<%-- ログインID --%>
						<%: ReplaceTag("@@User.login_id.name@@") %></th>
					<td>
						<%: this.UserInput.LoginId %></td>
				</tr>
				<%} %>
				<%-- ソーシャルログイン連携されている場合はパスワードスキップ --%>
				<%if (this.IsVisible_UserPassword){ %>
				<tr>
					<th>
						<%-- パスワード --%>
						<%: ReplaceTag("@@User.password.name@@") %></th>
					<td>
						<%: StringUtility.ChangeToAster(this.UserInput.Password) %></td>
				</tr>
				<% } %>
			</table>
		</div>
		<% } %>

		<div class="dvUserBtnBox">
			<p>
				<span><asp:LinkButton ID="lbBack" runat="server" OnClientClick="return exec_submit()" OnClick="lbBack_Click" class="btn btn-large">戻る</asp:LinkButton></span>
				<span><asp:LinkButton ID="lbSend" runat="server" OnClientClick="return exec_submit()" OnClick="lbSend_Click" class="btn btn-large btn-inverse">登録する</asp:LinkButton></span>
			</p>
		</div>
	</div>
</div>

</asp:Content>