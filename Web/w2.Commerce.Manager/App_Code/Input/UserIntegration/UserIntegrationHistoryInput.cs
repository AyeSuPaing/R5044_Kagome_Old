/*
=========================================================================================================
  Module      : ユーザー統合履歴情報入力クラス (UserIntegrationHistoryInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.UserIntegration;
using w2.Domain.UserIntegration.Helper;	
using w2.Domain.Point;
using w2.Domain.UserCreditCard;

/// <summary>
/// ユーザー統合履歴情報入力クラス
/// </summary>
[Serializable]
public class UserIntegrationHistoryInput : InputBase<UserIntegrationHistoryModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public UserIntegrationHistoryInput()
	{
		this.UserPointHistory = null;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public UserIntegrationHistoryInput(UserIntegrationHistoryContainer container)
		: this()
	{
		this.UserIntegrationNo = container.UserIntegrationNo.ToString();
		this.UserId = container.UserId;
		this.BranchNo = container.BranchNo.ToString();
		this.TableName = container.TableName;
		this.PrimaryKey1 = container.PrimaryKey1;
		this.PrimaryKey2 = container.PrimaryKey2;
		this.PrimaryKey3 = container.PrimaryKey3;
		this.PrimaryKey4 = container.PrimaryKey4;
		this.PrimaryKey5 = container.PrimaryKey5;
		this.DateCreated = container.DateCreated.ToString();
		this.DateChanged = container.DateChanged.ToString();
		this.LastChanged = container.LastChanged;
		// ユーザーポイント履歴
		this.UserPointHistory = container.UserPointHistory;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override UserIntegrationHistoryModel CreateModel()
	{
		var model = new UserIntegrationHistoryModel
		{
			UserIntegrationNo = this.UserIntegrationNo != null ? long.Parse(this.UserIntegrationNo) : 0,
			UserId = this.UserId,
			BranchNo = int.Parse(this.BranchNo),
			TableName = this.TableName,
			PrimaryKey1 = this.PrimaryKey1,
			PrimaryKey2 = this.PrimaryKey2,
			PrimaryKey3 = this.PrimaryKey3,
			PrimaryKey4 = this.PrimaryKey4,
			PrimaryKey5 = this.PrimaryKey5,
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		return "";
	}
	#endregion

	#region プロパティ
	/// <summary>ユーザー統合No</summary>
	public string UserIntegrationNo
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_USER_INTEGRATION_NO]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_USER_INTEGRATION_NO] = value; }
	}
	/// <summary>ユーザID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_USER_ID]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_USER_ID] = value; }
	}
	/// <summary>枝番</summary>
	public string BranchNo
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_BRANCH_NO] = value; }
	}
	/// <summary>テーブル名</summary>
	public string TableName
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_TABLE_NAME]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_TABLE_NAME] = value; }
	}
	/// <summary>主キー1</summary>
	public string PrimaryKey1
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY1]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY1] = value; }
	}
	/// <summary>主キー2</summary>
	public string PrimaryKey2
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY2]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY2] = value; }
	}
	/// <summary>主キー3</summary>
	public string PrimaryKey3
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY3]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY3] = value; }
	}
	/// <summary>主キー4</summary>
	public string PrimaryKey4
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY4]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY4] = value; }
	}
	/// <summary>主キー5</summary>
	public string PrimaryKey5
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY5]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY5] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_USERINTEGRATIONHISTORY_LAST_CHANGED] = value; }
	}
	/// <summary>ユーザーポイント履歴情報</summary>
	public UserPointHistoryModel UserPointHistory
	{
		get { return (UserPointHistoryModel)this.DataSource["UserPointHistory"]; }
		set { this.DataSource["UserPointHistory"] = value; }
	}
	/// <summary>クレジット情報</summary>
	public UserCreditCardModel UserCreditCard
	{
		get
		{
			var credit = new UserCreditCardService().Get(this.UserId, Int32.Parse(this.PrimaryKey1));
			return credit ?? new UserCreditCardModel();
		}
	}
	#endregion
}
