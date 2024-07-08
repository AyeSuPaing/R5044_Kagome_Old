/*
=========================================================================================================
  Module      : LINE仮会員インプットクラス(LineTemporaryUserInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.LineTemporaryUser;
using w2.Domain.User;

/// <summary>
/// LINE仮会員インプットクラス
/// </summary>
public class LineTemporaryUserInput : InputBase<LineTemporaryUserModel>
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="lineUserId">LINEユーザーID</param>
	public LineTemporaryUserInput(string lineUserId)
	{
		this.LineUserId = lineUserId;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>検証結果</returns>
	public string Validate()
	{
		var existsUser = new UserService().GetByExtendColumn(Constants.SOCIAL_PROVIDER_ID_LINE, this.LineUserId);
		if (existsUser != null)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_TEMPORARY_USER_DUPLICATED);
		}

		return string.Empty;
	}

	/// <summary>
	/// モデル生成
	/// </summary>
	/// <returns>モデル</returns>
	public override LineTemporaryUserModel CreateModel()
	{
		var model = new LineTemporaryUserModel
		{
			LineUserId = this.LineUserId,
			TemporaryUserRegistrationDate = DateTime.Now,
		};
		return model;
	}

	/// <summary>LINEユーザーID</summary>
	public string LineUserId
	{
		get { return (string)this.DataSource[Constants.FIELD_LINETEMPORARYUSER_LINE_USER_ID]; }
		set { this.DataSource[Constants.FIELD_LINETEMPORARYUSER_LINE_USER_ID] = value; }
	}
}