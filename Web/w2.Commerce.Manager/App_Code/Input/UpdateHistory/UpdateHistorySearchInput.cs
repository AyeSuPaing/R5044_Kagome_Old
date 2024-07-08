/*
=========================================================================================================
  Module      : 更新履歴情報入力クラス (UpdateHistorySearchInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// 更新履歴情報入力クラス
/// </summary>
[Serializable]
public class UpdateHistorySearchInput
{
	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public UpdateHistorySearchInput()
	{
		this.DataSource = new Hashtable();
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="result">更新履歴検索結果</param>
	public UpdateHistorySearchInput(UpdateHistoryListSearchResult result)
		: this()
	{
		this.UpdateHistoryNo = result.UpdateHistoryNo;
		this.UpdateHistoryKbn = result.UpdateHistoryKbn;
		this.UserId = result.UserId;
		this.MasterId = result.MasterId;
		this.UpdateHistoryAction = result.UpdateHistoryAction;
		this.DateCreated = result.DateCreated.ToString();
		this.LastChanged = result.LastChanged;
		this.IsDisplay = false;
	}
	#endregion

	#region プロパティ
	/// <summary>データソース</summary>
	public Hashtable DataSource { get; set; }
	/// <summary>更新履歴No</summary>
	public long UpdateHistoryNo
	{
		get { return (long)this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_NO]; }
		set { this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_NO] = value; }
	}
	/// <summary>更新履歴区分</summary>
	public string UpdateHistoryKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_KBN]; }
		set { this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_KBN] = value; }
	}
	/// <summary>ユーザーID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_USER_ID]; }
		set { this.DataSource[Constants.FIELD_UPDATEHISTORY_USER_ID] = value; }
	}
	/// <summary>マスタID</summary>
	public string MasterId
	{
		get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_MASTER_ID]; }
		set { this.DataSource[Constants.FIELD_UPDATEHISTORY_MASTER_ID] = value; }
	}
	/// <summary>更新履歴アクション</summary>
	public string UpdateHistoryAction
	{
		get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_ACTION]; }
		set { this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_ACTION] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_UPDATEHISTORY_DATE_CREATED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_UPDATEHISTORY_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_UPDATEHISTORY_LAST_CHANGED] = value; }
	}
	/// <summary>
	/// 拡張項目_更新履歴区分表示テキスト
	/// </summary>
	public string UpdateHistoryKbnText
	{
		get
		{
			return ValueText.GetValueText(Constants.TABLE_UPDATEHISTORY, Constants.FIELD_UPDATEHISTORY_UPDATE_HISTORY_KBN, this.UpdateHistoryKbn);
		}
	}
	/// <summary>表示選択されている？</summary>
	public bool IsDisplay
	{
		get { return (bool)this.DataSource["IsDisplay"]; }
		set { this.DataSource["IsDisplay"] = value; }
	}
	#endregion
}