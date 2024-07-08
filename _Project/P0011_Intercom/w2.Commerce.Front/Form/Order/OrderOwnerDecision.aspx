<%--
=========================================================================================================
  Module      : �����Ҍ�����(OrderOwnerDecision.aspx)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>

<%@ Page Language="C#" MasterPageFile="~/Form/Common/OrderPage.master" AutoEventWireup="true" CodeFile="~/Form/Order/OrderOwnerDecision.aspx.cs" Inherits="Form_Order_OrderOwnerDecision" Title="�����Ҍ���y�[�W" MaintainScrollPositionOnPostback="true" %>

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
	
		#region ���b�v�ς݃R���g���[���錾
		WrappedTextBox WtbLoginIdInMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbLoginIdInMailAddr"); } }
		WrappedTextBox WtbLoginId { get { return GetWrappedControl<WrappedTextBox>("tbLoginId", ""); } }
		WrappedTextBox WtbPassword { get { return GetWrappedControl<WrappedTextBox>("tbPassword", ""); } }
		WrappedHtmlGenericControl WdLoginErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("dLoginErrorMessage", ""); } }
		protected WrappedCheckBox WcbAutoCompleteLoginIdFlg { get { return GetWrappedControl<WrappedCheckBox>("cbAutoCompleteLoginIdFlg", false); } }
		#endregion

		/// <summary>
		/// �y�[�W���[�h
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.UserService = new UserService();

			//------------------------------------------------------
			// HTTPS�ʐM�`�F�b�N�iHTTP�ʐM�̏ꍇ�A�V���b�s���O�J�[�g�ցj
			//------------------------------------------------------
			CheckHttps();

			//------------------------------------------------------
			// �J�[�g���݃`�F�b�N�i�J�[�g�����݂��Ȃ��ꍇ�A�G���[�y�[�W�ցj
			//------------------------------------------------------
			CheckCartExists();

			if (!IsPostBack)
			{
				//------------------------------------------------------
				// ��ʑJ�ڂ̐������`�F�b�N
				//------------------------------------------------------
				CheckOrderUrlSession();

				//------------------------------------------------------
				// ���[�U�Z�b�V�����`�F�b�N�i���O�C���ς݂̏ꍇ�͂��͂���w��ցj
				//------------------------------------------------------
				if (this.IsLoggedIn)
				{
					// ��ʑJ�ڂ̐������`�F�b�N�̂��ߑJ�ڐ�y�[�WURL��ݒ�
					Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;

					// ��ʑJ��
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
				}

				// ���̓t�H�[���Ƀ��O�C��ID�ɐݒ�
				SetLoginIdToInputForm(UserCookieManager.GetLoginIdFromCookie());

				//------------------------------------------------------
				// �R���|�[�l���g������
				//------------------------------------------------------
				InitComponents();
			}
		}

		/// <summary>
		/// �R���|�[�l���g������
		/// </summary>
		private void InitComponents()
		{
			//------------------------------------------------------
			// ���O�C��ID�^�p�X���[�h��Enter�Ń��O�C��
			//------------------------------------------------------
			// Enter�����ŃT�u�~�b�g ��FireFox�ł͊֐�������event.keyCode���ĂׂȂ��炵��
			string strOnKeypress = "if (event.keyCode==13){__doPostBack('" + lbLogin.UniqueID + "',''); return false;}";

			this.WtbLoginId.Attributes["onkeypress"] = strOnKeypress;
			this.WtbPassword.Attributes["onkeypress"] = strOnKeypress;
		}

		/// <summary>
		/// ���փ����N�N���b�N
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbOrderShipping_Click(object sender, EventArgs e)
		{
			//------------------------------------------------------
			// ��ʑJ�ڏ���
			//------------------------------------------------------
			// ��ʑJ�ڂ̐������`�F�b�N�̂��ߑJ�ڐ�y�[�WURL��ݒ�
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;

			//------------------------------------------------------
			// �Z�b�V�������蒼���̂��߂̃f�[�^�i�[�i�Z�b�V�����n�C�W���b�N�΍�j
			//------------------------------------------------------
			SessionSecurityManager.SaveSesstionContetnsToDatabaseForChangeSessionId(Request, Response, Session);

			//------------------------------------------------------
			// ���������O�C�����Ă����猳�̉�ʂ֑J�ځi�J�ڐ���p�����[�^�œn���j
			//------------------------------------------------------
			StringBuilder sbRedirectUrl = new StringBuilder();
			sbRedirectUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_RESTORE_SESSION);
			sbRedirectUrl.Append("?").Append(Constants.REQUEST_KEY_NEXT_URL).Append("=").Append(Server.UrlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING));
			sbRedirectUrl.Append("&").Append(Constants.REQUEST_KEY_LOGIN_FLG).Append("=").Append("1");

			Response.Redirect(sbRedirectUrl.ToString());
		}

		/// <summary>
		/// ���O�C���{�^������
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbLogin_Click(object sender, EventArgs e)
		{
			//------------------------------------------------------
			// �A�J�E���g���b�N�`�F�b�N�i�A�J�E���g���b�N������Ă���ꍇ�́A�G���[��ʂ֑J�ځj
			//------------------------------------------------------
			string loginId = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? this.WtbLoginIdInMailAddr.Text : this.WtbLoginId.Text;
			string password = this.WtbPassword.Text;

			if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, password))
			{
				RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK), this.RawUrl);
			}

			//�C���^�[�R���Ή��ASSO�̓v���O�C����NextUrl���g������
			this.NextUrl = Constants.PROTOCOL_HTTPS + Request.Url.Authority + this.RawUrl;

			// ���O�C������
			var loggedUser = new UserService().TryLogin(loginId, password, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
			if (loggedUser == null)
			{
				string errorMessage = GetLoginDeniedErrorMessage(loginId, password);
				// ���O�C�����s�\�񐔂������Ă����ꍇ�A�G���[��ʂ֑J��
				if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, password))
				{
					RedirectErrorPage(errorMessage, this.RawUrl);
				}
				else
				{
					// ���[�U�[�����݂����A���O�C�����s�\�񐔈ȓ��̏ꍇ
					this.WdLoginErrorMessage.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(errorMessage);
				}
				return;
			}

			// ���O�C�������������s�����̉�ʂ֑J�ځi���O�C�������j
			ExecLoginSuccessProcessAndGoNextForLogin(
				loggedUser,
				this.NextUrl,
				this.WcbAutoCompleteLoginIdFlg.Checked,
				LoginType.Normal,
				UpdateHistoryAction.Insert);
		}

		/// <summary>
		/// �V�K�o�^�{�^���N���b�N
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
		/// �G���[��ʑJ��
		/// </summary>
		/// <param name="strErrorMessageKey">�G���[���b�Z�[�W�L�[</param>
		/// <param name="strBackUrl">�߂�URL</param>
		private void RedirectErrorPage(string errorMessage, string backUrl)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;

			StringBuilder sbUrl = new StringBuilder();
			sbUrl.Append(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);

			// �߂�URL��null�ȊO�̏ꍇ�A�߂�URL��J�ڗpURL�ɕt�^
			//�i�G���[��ʂ���߂����ۂɃL���b�V�����폜����Ă��ĕ\���o���Ȃ��\��������ׁA�����I�ɖ߂�URL��ݒ�j
			if (backUrl != null)
			{
				sbUrl.Append("?").Append(Constants.REQUEST_KEY_BACK_URL).Append("=").Append(HttpUtility.UrlEncode(backUrl));
			}

			Response.Redirect(sbUrl.ToString());
		}

		/// <summary>
		/// ���̓t�H�[���Ƀ��O�C��ID�ɐݒ�
		/// </summary>
		/// <param name="loginId">Cookie����擾�������O�C��ID</param>
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
		///  ���[�U�[�T�[�r�X
		/// </summary>
		private UserService UserService { get; set; }

		/// <summary>���y�[�WURL</summary>
		private string NextUrl
		{
			get { return (string)ViewState["NextUrl"]; }
			set { ViewState["NextUrl"] = value; }
		}
	
	</script>


	<!--#################�@�p�����i�r�J�n�@#################-->
	<div id="id-navi-wrap">
		<div id="id-navi">
			<ul>
				<li class="home"><a href="<%= Constants.PATH_ROOT %>">�z�[��</a></li>
				<li>���i�̂��w���i�������҂Ƃ��͂���̎w��j</li>
			</ul>
		</div>
	</div>
	<!--#################�@�p�����i�r�I���@#################-->


	<!--#################�@���C�������͂݊J�n�@#################-->
	<div id="id-main-wrap" class="clearfix">

		<!--#################�@�R���e���c�����͂݊J�n�i�T�C�h���j���[�Ȃ��j�@#################-->
		<div id="id-onewrap">

			<h2>���i�̂��w���i�������҂Ƃ��͂���̎w��j</h2>

			<div id="idform">

				<p class="flow">
					<img src="<%= Constants.PATH_ROOT %>img/sys/step/order_step1.gif" alt="�������҂Ƃ��͂���̎w��" /></p>

				<p class="announce">���L�̂����ꂩ�̃{�^���������Ă��������A�葱�����s���Ă��������B</p>

				<div class="buywaywrap clearfix">
					<div class="blockl">
						<table width="231" height="260" class="common">
							<tr>
								<th height="25">
								���߂Ă����p�̕�</td>
							</tr>
							<tr>
								<td>
									<p class="mg_b20">���߂Ă����p�̂��q�l�́A�����炩��C���^�[�R������o�^���s���Ă��������B�o�^���E�N���͖����ł��B</p>
									<div align="center">
										<asp:LinkButton ID="lbUserRegist" runat="server" OnClick="lbUserRegist_Click"><img src="<%: Constants.PATH_ROOT %>images13/common/btn_cart_regist_off.gif" alt="�V�K����o�^" type="image" vspace="0" border="0"></asp:LinkButton></a></div>
								</td>
							</tr>
						</table>
					</div>
					<div class="blockc">
						<table width="231" class="common">
							<tr>
								<th height="25">
								�C���^�[�R������̕�</td>
							</tr>
							<tr>
								<td>
									<div class="loginform_s">
										<dl>
											<%if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			 { %>
											<dt>���[���A�h���X</dt>
											<dd>
												<asp:TextBox ID="tbLoginIdInMailAddr" runat="server" CssClass="input_widthC input_border loginmail_s" MaxLength="256" Width="200"></asp:TextBox></dd>
											<%}
			 else
			 { %>
											<dt>���O�C��ID</dt>
											<dd>
												<asp:TextBox ID="tbLoginId" runat="server" CssClass="input_widthC input_border loginmail_s" MaxLength="15" Width="200"></asp:TextBox></dd>
											<%} %>
											<dt>�p�X���[�h</dt>
											<dd>
												<asp:TextBox ID="tbPassword" TextMode="Password" runat="server" CssClass="input_widthC input_border" MaxLength="15" Width="200"></asp:TextBox></dd>
										</dl>
									</div>
									<div class="center mg_b10">
										<div><small id="dLoginErrorMessage" class="fred" runat="server"></small></div>
										<asp:LinkButton ID="lbLogin" runat="server" OnClick="lbLogin_Click"><img name="order" src="<%= Constants.PATH_ROOT %>images13/common/btn_cart_login_off.gif" alt="���O�C�����ė��p����" vspace="0" border="0"></asp:LinkButton>
									</div>
									<ul>
										<li class="li_blank"><a target="_blank" href="<%= Constants.PATH_ROOT + "Extern/IntercomReminderStatusCheck.aspx" %>">�p�X���[�h��Y�ꂽ���͂�����</a></li>
									</ul>
								</td>
							</tr>
						</table>
					</div>
					<div class="blockr">
						<table width="231" height="260" class="common">
							<tr>
								<th height="25">
								�Q�X�g�w������]������</td>
							</tr>
							<tr>
								<td>
									<%if (this.CartList.HasFixedPurchase)
		   { %>
                    ����w�����i�̓C���^�[�R������l�݂̂����w��������悤�ɂȂ��Ă���܂��B���O�C��ID���������łȂ����q�l�͂����炩�����o�^���s���Ă��������B
                  <% }
		   else
		   { %>
									<p>
										����o�^�����Ȃ��ōw������]���邨�q�l�͂����炩�炲���p���������B<br />
										�����q�l���͔z���y�т��⍇���ȊO�ɂ͗��p�������܂���B
									</p>
									<div class="center">
										<asp:LinkButton ID="lbOrderShipping" runat="server" OnClick="lbOrderShipping_Click">
                      <img name="guest" src="<%: Constants.PATH_ROOT %>images13/common/btn_cart_guest_off.gif" alt="�Q�X�g�w��" tabindex="1" type="image" vspace="0" border="0">
										</asp:LinkButton>
									</div>
									<%} %>
								</td>
							</tr>
						</table>
					</div>
				</div>

				<div class="box_common">
					<p class="f_bold c_red">���̉�ʂł͂��̂悤�Ȏ葱�����s���܂��B</p>
					<ul>
						<li class="li_list">���Ƀ��O�C���ς݂̂��q�l�́A���̉�ʂł��͂���E���x�������@�����w�肢�������܂��B</li>
						<li class="li_list">�C���^�[�R������ł܂����O�C�����Ă��Ȃ����q�l�́A���O�C�����Ă��炲�w���葱����i�߂��܂��B</li>
						<li class="li_list">�C���^�[�R������ɐV�K�ɓo�^���Ă��炲�w���葱����i�߂邱�Ƃ��ł��܂��B</li>
						<li class="li_list">�C���^�[�R������ɓo�^�����A<u>�y <b>�Q�X�g�w������</b> �z�ł����ɂ��w���葱����i�߂邱�Ƃ��ł��܂��B</u></li>
					</ul>
				</div>

			</div>

		</div>
		<!--#################�@�R���e���c�����͂ݏI���i�T�C�h���j���[�Ȃ��j�@#################-->

	</div>
	<!--#################�@���C�������͂ݏI���@#################-->

</asp:Content>
