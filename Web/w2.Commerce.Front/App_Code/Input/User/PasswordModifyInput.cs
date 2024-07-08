/*
=========================================================================================================
  Module      : パスワード編集入力クラス(PasswordModifyInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Input;
using w2.Domain.User;

/// <summary>
/// パスワード編集入力クラス
/// </summary>
public class PasswordModifyInput : InputBase<PasswordReminderModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public PasswordModifyInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="passwordReminder">モデル</param>
	public PasswordModifyInput(PasswordReminderModel passwordReminder)
		: this()
	{
		this.UserId = passwordReminder.UserId;
		this.AuthenticationKey = passwordReminder.AuthenticationKey;
		this.ChangeTrialLimitCount = passwordReminder.ChangeTrialLimitCount;
		this.DateCreated = passwordReminder.DateCreated.ToString();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override PasswordReminderModel CreateModel()
	{
		var passwordReminder = new PasswordReminderModel
		{
			UserId = this.UserId,
			AuthenticationKey = this.AuthenticationKey,
			ChangeTrialLimitCount = this.ChangeTrialLimitCount,
		};

		return passwordReminder;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>成功:空/失敗:エラーメッセージ</returns>
	public Dictionary<string, string> Validate()
	{
		var input = new Hashtable();

		if (this.EasyRegisterFlg == Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL)
		{
			// 生年月日による本人確認の場合
			if (Constants.PASSWORDRIMINDER_AUTHITEM == Constants.PasswordReminderAuthItem.Birth)
			{
				input.Add(Constants.FIELD_USER_BIRTH, this.Birth);
				input.Add("registed_birth", this.RegistedBirth);
			}
			// 電話番号による本人確認の場合
			else if (Constants.PASSWORDRIMINDER_AUTHITEM == Constants.PasswordReminderAuthItem.Tel)
			{
				if (this.IsTelShortInput == false)
				{
					// 入力したTELと登録したTEL2が一致しない場合、TEL1で比較する
					if ((string.IsNullOrEmpty(this.Tel1_1)
						|| string.IsNullOrEmpty(this.Tel1_2)
						|| string.IsNullOrEmpty(this.Tel1_3))
						|| (this.Tel1_1 != this.RegistedTel2_1)
						|| (this.Tel1_2 != this.RegistedTel2_2)
						|| (this.Tel1_3 != this.RegistedTel2_3))
					{
						input.Add(Constants.FIELD_USER_TEL1_1, this.Tel1_1);
						input.Add(Constants.FIELD_USER_TEL1_2, this.Tel1_2);
						input.Add(Constants.FIELD_USER_TEL1_3, this.Tel1_3);
						input.Add(Constants.REGISTED_TEL1_1, this.RegistedTel1_1);
						input.Add(Constants.REGISTED_TEL1_2, this.RegistedTel1_2);
						input.Add(Constants.REGISTED_TEL1_3, this.RegistedTel1_3);
					}
				}
				else
				{
					input[Constants.FIELD_USER_TEL1] = StringUtility.ReplaceDelimiter(this.Tel1);
					input[Constants.REGISTED_TEL1] = StringUtility.ReplaceDelimiter(this.RegistedTel1);
				}
			}
		}
		input.Add(Constants.FIELD_USER_LOGIN_ID, this.LoginId);
		input.Add(Constants.FIELD_USER_PASSWORD, this.Password);
		input.Add(Constants.FIELD_USER_PASSWORD + Constants.FIELD_COMMON_CONF, this.PasswordConf);

		var result = Validator.ValidateAndGetErrorContainer("PasswordModify", input);
		return result;
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_PASSWORDREMINDER_USER_ID]; }
		set { this.DataSource[Constants.FIELD_PASSWORDREMINDER_USER_ID] = value; }
	}
	/// <summary>認証キー</summary>
	public string AuthenticationKey
	{
		get { return (string)this.DataSource[Constants.FIELD_PASSWORDREMINDER_AUTHENTICATION_KEY]; }
		set { this.DataSource[Constants.FIELD_PASSWORDREMINDER_AUTHENTICATION_KEY] = value; }
	}
	/// <summary>変更試行回数制限</summary>
	public int ChangeTrialLimitCount
	{
		get { return (int)this.DataSource[Constants.FIELD_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT]; }
		set { this.DataSource[Constants.FIELD_PASSWORDREMINDER_CHANGE_TRIAL_LIMIT_COUNT] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_PASSWORDREMINDER_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_PASSWORDREMINDER_DATE_CREATED] = value; }
	}
	/// <summary>生年月日</summary>
	public string Birth { get; set; }
	/// <summary>登録した生年月日</summary>
	public string RegistedBirth { get; set; }
	/// <summary>電話番号1-1</summary>
	public string Tel1_1 { get; set; }
	/// <summary>電話番号1-2</summary>
	public string Tel1_2 { get; set; }
	/// <summary>電話番号1-3</summary>
	public string Tel1_3 { get; set; }
	/// <summary>登録した電話番号1-1</summary>
	public string RegistedTel1_1 { get; set; }
	/// <summary>登録した電話番号1-2</summary>
	public string RegistedTel1_2 { get; set; }
	/// <summary>登録した電話番号1-3</summary>
	public string RegistedTel1_3 { get; set; }
	/// <summary>登録した電話番号2-1</summary>
	public string RegistedTel2_1 { get; set; }
	/// <summary>登録した電話番号2-2</summary>
	public string RegistedTel2_2 { get; set; }
	/// <summary>登録した電話番号2-3</summary>
	public string RegistedTel2_3 { get; set; }
	/// <summary>ログインID</summary>
	public string LoginId { get; set; }
	/// <summary>パスワード</summary>
	public string Password { get; set; }
	/// <summary>パスワード確認用</summary>
	public string PasswordConf { get; set; }
	/// <summary>かんたん会員フラグ</summary>
	public string EasyRegisterFlg { get; set; }
	/// <summary>Telephone 1</summary>
	public string Tel1 { get; set; }
	/// <summary>Registed telephone 1</summary>
	public string RegistedTel1 { get; set; }
	/// <summary>Registed telephone 2</summary>
	public string RegistedTel2 { get; set; }
	/// <summary>Is telephone short input</summary>
	public bool IsTelShortInput { get; set; }
	#endregion
}