/*
=========================================================================================================
  Module      : 認証コード送信用モーダル(SendAuthenticationCodeModal.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Web.WrappedContols;

public partial class Form_Common_Order_SendAuthenticationCodeModal : BaseUserControl
{
	/// <summary>Wrapped hidden field reset authentication code</summary>
	protected WrappedHiddenField WhfUserId { get { return GetWrappedControl<WrappedHiddenField>("hfUserId"); } }
	/// <summary>Wrapped text box authentication code</summary>
	protected WrappedTextBox WtbAuthenticationCode { get { return GetWrappedControl<WrappedTextBox>("tbAuthenticationCode"); } }
	/// <summary>Wrapped radio button send mail</summary>
	protected WrappedRadioButton WrbSendMail { get { return GetWrappedControl<WrappedRadioButton>("rbSendMail"); } }
}
