/*
=========================================================================================================
  Module      : CSオペレータ入力クラス (CsOperatorInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Input;
using w2.App.Common.Cs.CsOperator;

/// <summary>
/// CSオペレータマスタ入力クラス
/// </summary>
public class CsOperatorInput : InputBase<CsOperatorModel>
{
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public CsOperatorInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public CsOperatorInput(CsOperatorModel model)
		: this()
	{
		this.DeptId = model.DeptId;
		this.OperatorId = model.OperatorId;
		this.OperatorAuthorityId = model.OperatorAuthorityId;
		this.MailFromId = model.MailFromId;
		this.NotifyInfoFlg = model.NotifyInfoFlg;
		this.NotifyWarnFlg = model.NotifyWarnFlg;
		this.MailAddr = model.MailAddr;
		this.DisplayOrder = model.DisplayOrder.ToString();
	}

	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override CsOperatorModel CreateModel()
	{
		var model = new CsOperatorModel
		{
			DeptId = this.DeptId,
			OperatorId = this.OperatorId,
			OperatorAuthorityId = this.OperatorAuthorityId,
			MailFromId = this.MailFromId,
			NotifyInfoFlg = this.NotifyInfoFlg,
			NotifyWarnFlg = this.NotifyWarnFlg,
			MailAddr = this.MailAddr,
			DisplayOrder = int.Parse(this.DisplayOrder),
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var error = string.Join(
			Environment.NewLine,
			w2.Common.Util.Validator.Validate("CsOperator", this.DataSource)
				.Select(err => err.Value)
				.ToArray());
		return error;
	}

	/// <summary>識別ID</summary>
	public string DeptId
	{
		get { return (string)this.DataSource[Constants.FIELD_CSOPERATOR_DEPT_ID]; }
		set { this.DataSource[Constants.FIELD_CSOPERATOR_DEPT_ID] = value; }
	}
	/// <summary>オペレータID</summary>
	public string OperatorId
	{
		get { return (string)this.DataSource[Constants.FIELD_CSOPERATOR_OPERATOR_ID]; }
		set { this.DataSource[Constants.FIELD_CSOPERATOR_OPERATOR_ID] = value; }
	}
	/// <summary>オペレータ権限ID</summary>
	public string OperatorAuthorityId
	{
		get { return (string)this.DataSource[Constants.FIELD_CSOPERATOR_OPERATOR_AUTHORITY_ID]; }
		set { this.DataSource[Constants.FIELD_CSOPERATOR_OPERATOR_AUTHORITY_ID] = value; }
	}
	/// <summary>メール送信元ID</summary>
	public string MailFromId
	{
		get { return (string)this.DataSource[Constants.FIELD_CSOPERATOR_MAIL_FROM_ID]; }
		set { this.DataSource[Constants.FIELD_CSOPERATOR_MAIL_FROM_ID] = value; }
	}
	/// <summary>情報メール通知フラグ</summary>
	public string NotifyInfoFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_CSOPERATOR_NOTIFY_INFO_FLG]; }
		set { this.DataSource[Constants.FIELD_CSOPERATOR_NOTIFY_INFO_FLG] = value; }
	}
	/// <summary>警告メール通知フラグ</summary>
	public string NotifyWarnFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_CSOPERATOR_NOTIFY_WARN_FLG]; }
		set { this.DataSource[Constants.FIELD_CSOPERATOR_NOTIFY_WARN_FLG] = value; }
	}
	/// <summary>メールアドレス</summary>
	public string MailAddr
	{
		get { return (string)this.DataSource[Constants.FIELD_CSOPERATOR_MAIL_ADDR]; }
		set { this.DataSource[Constants.FIELD_CSOPERATOR_MAIL_ADDR] = value; }
	}
	/// <summary>表示順</summary>
	public string DisplayOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_CSOPERATOR_DISPLAY_ORDER]; }
		set { this.DataSource[Constants.FIELD_CSOPERATOR_DISPLAY_ORDER] = value; }
	}
}
