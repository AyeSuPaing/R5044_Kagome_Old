/*
=========================================================================================================
  Module      : ユーザー統合情報入力クラス (UserIntegrationInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.UserIntegration;
using w2.Domain.UserIntegration.Helper;

/// <summary>
/// ユーザー統合情報入力クラス
/// </summary>
[Serializable]
public class UserIntegrationInput : InputBase<UserIntegrationModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public UserIntegrationInput()
	{
		this.Status = Constants.FLG_USERINTEGRATION_STATUS_NONE;
		this.Users = new UserIntegrationUserInput[0];
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public UserIntegrationInput(UserIntegrationContainer container)
		: this()
	{
		this.UserIntegrationNo = container.UserIntegrationNo.ToString();
		this.Status = container.Status;
		this.DateCreated = container.DateCreated.ToString();
		this.DateChanged = container.DateChanged.ToString();
		this.LastChanged = container.LastChanged;
		// ユーザリスト
		this.Users = container.Users.Select(s => new UserIntegrationUserInput(s)).ToArray();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override UserIntegrationModel CreateModel()
	{
		var model = new UserIntegrationModel
		{
			UserIntegrationNo = this.UserIntegrationNo != null ? long.Parse(this.UserIntegrationNo) : 0,
			Status = this.Status,
			LastChanged = this.LastChanged,
		};
		model.Users = this.Users.Select(u => u.CreateModel()).ToArray();
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var input = new Hashtable()
		{
			 {"user_count", this.Users.Length.ToString()},
			 {Constants.FIELD_USERINTEGRATIONUSER_REPRESENTATIVE_FLG, this.Users.Any(u => u.IsOnRepresentativeFlg) ? Constants.FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_ON : ""}
		};
		return Validator.Validate("UserIntegrationModity", input);
	}

	/// <summary>
	/// ユーザー統合履歴入力情報取得
	/// </summary>
	/// <returns>ユーザー統合履歴入力情報列</returns>
	public UserIntegrationHistoryInput[] GetHistores()
	{
		return this.Users.SelectMany(u => u.Histories).OrderByDescending(u => DateTime.Parse(u.DateCreated)).ToArray();
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザー統合No</summary>
	public string UserIntegrationNo
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO] = value; }
	}
	/// <summary>ステータス</summary>
	public string Status
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATION_STATUS]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATION_STATUS] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATION_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATION_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATION_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATION_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATION_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATION_LAST_CHANGED] = value; }
	}
	/// <summary>ステータス（テキスト）</summary>
	public string StatusText
	{
		get
		{
			return ValueText.GetValueText(Constants.TABLE_USERINTEGRATION, Constants.FIELD_USERINTEGRATION_STATUS, this.Status);
		}
	}
	/// <summary>ステータスが未確定か</summary>
	public bool IsNoneStatus
	{
		get { return (this.Status == Constants.FLG_USERINTEGRATION_STATUS_NONE); }
	}
	/// <summary>ステータスが保留か</summary>
	public bool IsSuspendStatus
	{
		get { return (this.Status == Constants.FLG_USERINTEGRATION_STATUS_SUSPEND); }
	}
	/// <summary>ステータスが確定か</summary>
	public bool IsDoneStatus
	{
		get { return (this.Status == Constants.FLG_USERINTEGRATION_STATUS_DONE); }
	}
	/// <summary>ステータスが除外か</summary>
	public bool IsExcludedStatus
	{
		get { return (this.Status == Constants.FLG_USERINTEGRATION_STATUS_EXCLUDED); }
	}
	/// <summary>ユーザリスト</summary>
	public UserIntegrationUserInput[] Users
	{
		get { return (UserIntegrationUserInput[])this.DataSource["Users"]; }
		set { this.DataSource["Users"] = value; }
	}
	#endregion
}
