<%--
=========================================================================================================
  Module      : 注文者決定画面(OrderOwnerDecision.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderOwnerDecision.aspx.cs" Inherits="Form_Order_OrderOwnerDecision" Title="注文者決定ページ" MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="w2.Cryptography" %>
<%@ Import Namespace="w2.Domain.User" %>
<%@ Import Namespace="w2.Domain.UpdateHistory.Helper" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.App.Common.Web.WrappedContols" %>
<%@ Import Namespace="w2.App.Common.User" %>
<%@ Import Namespace="w2.App.Common.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<script runat="server">
	
		#region ラップ済みコントロール宣言
		WrappedTextBox WtbLoginIdInMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbLoginIdInMailAddr"); } }
		WrappedTextBox WtbLoginId { get { return GetWrappedControl<WrappedTextBox>("tbLoginId", ""); } }
		WrappedTextBox WtbPassword { get { return GetWrappedControl<WrappedTextBox>("tbPassword", ""); } }
		WrappedHtmlGenericControl WdLoginErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("dLoginErrorMessage", ""); } }
		protected WrappedCheckBox WcbAutoCompleteLoginIdFlg { get { return GetWrappedControl<WrappedCheckBox>("cbAutoCompleteLoginIdFlg", false); } }
		#endregion

		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.UserService = new UserService();

			//------------------------------------------------------
			// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
			//------------------------------------------------------
			CheckHttps();

			//------------------------------------------------------
			// カート存在チェック（カートが存在しない場合、エラーページへ）
			//------------------------------------------------------
			CheckCartExists();

			if (!IsPostBack)
			{
				//------------------------------------------------------
				// 画面遷移の正当性チェック
				//------------------------------------------------------
				CheckOrderUrlSession();

				//------------------------------------------------------
				// ユーザセッションチェック（ログイン済みの場合はお届け先指定へ）
				//------------------------------------------------------
				if (this.IsLoggedIn)
				{
					// 画面遷移の正当性チェックのため遷移先ページURLを設定
					Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;

					// 画面遷移
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
				}

				// 入力フォームにログインIDに設定
				SetLoginIdToInputForm(UserCookieManager.GetLoginIdFromCookie());

				//------------------------------------------------------
				// コンポーネント初期化
				//------------------------------------------------------
				InitComponents();
			}
		}

		/// <summary>
		/// コンポーネント初期化
		/// </summary>
		private void InitComponents()
		{
			//------------------------------------------------------
			// ログインID／パスワードはEnterでログイン
			//------------------------------------------------------
			// Enter押下でサブミット ※FireFoxでは関数内からevent.keyCodeが呼べないらしい
			string strOnKeypress = "if (event.keyCode==13){__doPostBack('" + lbLogin.UniqueID + "',''); return false;}";

			this.WtbLoginId.Attributes["onkeypress"] = strOnKeypress;
			this.WtbPassword.Attributes["onkeypress"] = strOnKeypress;
		}

		/// <summary>
		/// 次へリンククリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbOrderShipping_Click(object sender, EventArgs e)
		{
			//------------------------------------------------------
			// 画面遷移準備
			//------------------------------------------------------
			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;

			//------------------------------------------------------
			// セッション張り直しのためのデータ格納（セッションハイジャック対策）
			//------------------------------------------------------
			SessionSecurityManager.SaveSesstionContetnsToDatabaseForChangeSessionId(Request, Response, Session);

			//------------------------------------------------------
			// 正しくログインしていたら元の画面へ遷移（遷移先をパラメータで渡す）
			//------------------------------------------------------
			StringBuilder sbRedirectUrl = new StringBuilder();
			sbRedirectUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_RESTORE_SESSION);
			sbRedirectUrl.Append("?").Append(Constants.REQUEST_KEY_NEXT_URL).Append("=").Append(Server.UrlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING));
			sbRedirectUrl.Append("&").Append(Constants.REQUEST_KEY_LOGIN_FLG).Append("=").Append("1");

			Response.Redirect(sbRedirectUrl.ToString());
		}

		/// <summary>
		/// ログインボタン押下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbLogin_Click(object sender, EventArgs e)
		{
			//------------------------------------------------------
			// アカウントロックチェック（アカウントロックがされている場合は、エラー画面へ遷移）
			//------------------------------------------------------
			string loginId = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? this.WtbLoginIdInMailAddr.Text : this.WtbLoginId.Text;
			string password = this.WtbPassword.Text;

			if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, password))
			{
				RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK), this.RawUrl);
			}

			//インターコム対応、SSOはプラグインでNextUrlを使うため
			this.NextUrl = Constants.PROTOCOL_HTTPS + Request.Url.Authority + this.RawUrl;

			// ログイン処理
			var loggedUser = new UserService().TryLogin(loginId, password, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
			if (loggedUser == null)
			{
				string errorMessage = GetLoginDeniedErrorMessage(loginId, password);
				// ログイン試行可能回数が超えていた場合、エラー画面へ遷移
				if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, password))
				{
					RedirectErrorPage(errorMessage, this.RawUrl);
				}
				else
				{
					// ユーザーが存在せず、ログイン試行可能回数以内の場合
					this.WdLoginErrorMessage.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(errorMessage);
				}
				return;
			}

			// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
			ExecLoginSuccessProcessAndGoNextForLogin(
				loggedUser,
				this.NextUrl,
				this.WcbAutoCompleteLoginIdFlg.Checked,
				LoginType.Normal,
				UpdateHistoryAction.Insert);
		}

		/// <summary>
		/// 新規登録ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbUserRegist_Click(object sender, EventArgs e)
		{
			StringBuilder sbUrl = new StringBuilder();
			sbUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_REGIST_REGULATION);
			sbUrl.Append("?").Append(Constants.REQUEST_KEY_NEXT_URL).Append("=").Append(HttpUtility.UrlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING));

			Response.Redirect(sbUrl.ToString());
		}

		/// <summary>
		/// エラー画面遷移
		/// </summary>
		/// <param name="strErrorMessageKey">エラーメッセージキー</param>
		/// <param name="strBackUrl">戻りURL</param>
		private void RedirectErrorPage(string errorMessage, string backUrl)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;

			StringBuilder sbUrl = new StringBuilder();
			sbUrl.Append(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);

			// 戻りURLがnull以外の場合、戻りURLを遷移用URLに付与
			//（エラー画面から戻った際にキャッシュが削除されていて表示出来ない可能性がある為、明示的に戻りURLを設定）
			if (backUrl != null)
			{
				sbUrl.Append("?").Append(Constants.REQUEST_KEY_BACK_URL).Append("=").Append(HttpUtility.UrlEncode(backUrl));
			}

			Response.Redirect(sbUrl.ToString());
		}

		/// <summary>
		/// 入力フォームにログインIDに設定
		/// </summary>
		/// <param name="loginId">Cookieから取得したログインID</param>
		private void SetLoginIdToInputForm(string loginId)
		{
			if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			{
				this.WtbLoginIdInMailAddr.Text = loginId;
			}
			else
			{
				this.WtbLoginId.Text = loginId;
			}
			this.WcbAutoCompleteLoginIdFlg.Checked = (loginId != "");
		}

		/// <summary>
		///  ユーザーサービス
		/// </summary>
		private UserService UserService { get; set; }

		/// <summary>次ページURL</summary>
		private string NextUrl
		{
			get { return (string)ViewState["NextUrl"]; }
			set { ViewState["NextUrl"] = value; }
		}
	
	</script>


	<!--#################　パン屑ナビ開始　#################-->
	<div id="id-navi-wrap">
		<div id="id-navi">
			<ul>
				<li class="home"><a href="<%= Constants.PATH_ROOT %>">ホーム</a></li>
				<li>商品のご購入（ご注文者とお届け先の指定）</li>
			</ul>
		</div>
	</div>
	<!--#################　パン屑ナビ終了　#################-->


	<!--#################　メイン部分囲み開始　#################-->
	<div id="id-main-wrap" class="clearfix">

		<!--#################　コンテンツ部分囲み開始（サイドメニューなし）　#################-->
		<div id="id-onewrap">

			<h2>商品のご購入（ご注文者とお届け先の指定）</h2>

			<div id="idform">

				<p class="flow">
					<img src="<%= Constants.PATH_ROOT %>img/sys/step/order_step1.gif" alt="ご注文者とお届け先の指定" /></p>

				<p class="announce">下記のいずれかのボタンを押していただき、手続きを行ってください。</p>

				<div class="buywaywrap clearfix">
					<div class="blockl">
						<table width="231" height="260" class="common">
							<tr>
								<th height="25">
								初めてご利用の方</td>
							</tr>
							<tr>
								<td>
									<p class="mg_b20">初めてご利用のお客様は、こちらからインターコム会員登録を行ってください。登録料・年会費は無料です。</p>
									<div align="center">
										<asp:LinkButton ID="lbUserRegist" runat="server" OnClick="lbUserRegist_Click"><img src="<%: Constants.PATH_ROOT %>images13/common/btn_cart_regist_off.gif" alt="新規会員登録" type="image" vspace="0" border="0"></asp:LinkButton></a></div>
								</td>
							</tr>
						</table>
					</div>
					<div class="blockc">
						<table width="231" class="common">
							<tr>
								<th height="25">
								インターコム会員の方</td>
							</tr>
							<tr>
								<td>
									<div class="loginform_s">
										<dl>
											<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			 { %>
											<dt>メールアドレス</dt>
											<dd>
												<asp:TextBox ID="tbLoginIdInMailAddr" runat="server" CssClass="input_widthC input_border loginmail_s" MaxLength="256" Width="200"></asp:TextBox></dd>
											<%}
			 else
			 { %>
											<dt>ログインID</dt>
											<dd>
												<asp:TextBox ID="tbLoginId" runat="server" CssClass="input_widthC input_border loginmail_s" MaxLength="15" Width="200"></asp:TextBox></dd>
											<%} %>
											<dt>パスワード</dt>
											<dd>
												<asp:TextBox ID="tbPassword" TextMode="Password" runat="server" CssClass="input_widthC input_border" MaxLength="15" Width="200"></asp:TextBox></dd>
										</dl>
									</div>
									<div class="center mg_b10">
										<div><small id="dLoginErrorMessage" class="fred" runat="server"></small></div>
										<asp:LinkButton ID="lbLogin" runat="server" OnClick="lbLogin_Click"><img name="order" src="<%= Constants.PATH_ROOT %>images13/common/btn_cart_login_off.gif" alt="ログインして利用する" vspace="0" border="0"></asp:LinkButton>
									</div>
									<ul>
										<li class="li_blank"><a target="_blank" href="<%= Constants.PATH_ROOT + "Extern/IntercomReminderStatusCheck.aspx" %>">パスワードを忘れた方はこちら</a></li>
									</ul>
								</td>
							</tr>
						</table>
					</div>
					<div class="blockr">
						<table width="231" height="260" class="common">
							<tr>
								<th height="25">
								ゲスト購入を希望される方</td>
							</tr>
							<tr>
								<td>
									<%if (this.CartList.HasFixedPurchase)
		   { %>
                    定期購入商品はインターコム会員様のみがご購入頂けるようになっております。ログインIDをお持ちでないお客様はこちらから会員登録を行ってください。
                  <% }
		   else
		   { %>
									<p>
										会員登録をしないで購入を希望するお客様はこちらからご利用ください。<br />
										※お客様情報は配送及びお問合せ以外には利用いたしません。
									</p>
									<div class="center">
										<asp:LinkButton ID="lbOrderShipping" runat="server" OnClick="lbOrderShipping_Click">
                      <img name="guest" src="<%: Constants.PATH_ROOT %>images13/common/btn_cart_guest_off.gif" alt="ゲスト購入" tabindex="1" type="image" vspace="0" border="0">
										</asp:LinkButton>
									</div>
									<%} %>
								</td>
							</tr>
						</table>
					</div>
				</div>

				<div class="box_common">
					<p class="f_bold c_red">この画面ではこのような手続きを行います。</p>
					<ul>
						<li class="li_list">既にログイン済みのお客様は、次の画面でお届け先・お支払い方法をご指定いただきます。</li>
						<li class="li_list">インターコム会員でまだログインしていないお客様は、ログインしてからご購入手続きを進められます。</li>
						<li class="li_list">インターコム会員に新規に登録してからご購入手続きを進めることもできます。</li>
						<li class="li_list">インターコム会員に登録せず、<u>【 <b>ゲスト購入する</b> 】ですぐにご購入手続きを進めることもできます。</u></li>
					</ul>
				</div>

			</div>

		</div>
		<!--#################　コンテンツ部分囲み終了（サイドメニューなし）　#################-->

	</div>
	<!--#################　メイン部分囲み終了　#################-->

</asp:Content>
