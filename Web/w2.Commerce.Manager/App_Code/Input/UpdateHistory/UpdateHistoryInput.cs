/*
=========================================================================================================
  Module      : 更新履歴情報入力クラス (UpdateHistoryInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// 更新履歴情報入力クラス
/// </summary>
[Serializable]
public class UpdateHistoryInput
{
	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public UpdateHistoryInput()
	{
		this.DataSource = new Hashtable();
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="result">更新履歴前後検索結果</param>SetExtendStatus
	public UpdateHistoryInput(UpdateHistoryBeforeAndAfterSearchResult result)
		: this()
	{
		var afterUpdateHistory = result.AfterUpdateHistory;
		var beforeUpdateHistory = result.BeforeUpdateHistory;
		this.UpdateHistoryNo = afterUpdateHistory.UpdateHistoryNo;
		this.UpdateHistoryKbn = afterUpdateHistory.UpdateHistoryKbn;
		this.MasterId = afterUpdateHistory.MasterId;
		this.UpdateHistoryAction = afterUpdateHistory.UpdateHistoryAction;
		this.UpdateData = afterUpdateHistory.UpdateData;
		this.DateCreated = afterUpdateHistory.DateCreated.ToString();
		this.LastChanged = afterUpdateHistory.LastChanged;
		BeforeAndAfterUpdateDataListCreatorBase creator = null;
		switch (this.UpdateHistoryKbn)
		{
			// ユーザー
			case Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_USER:
				creator = new BeforeAndAfterUpdateDataListCreatorUser();
				break;

			// 注文
			case Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_ORDER:
				creator = new BeforeAndAfterUpdateDataListCreatorOrder();
				break;

			// 定期購入
			case Constants.FLG_UPDATEHISTORY_UPDATE_HISTORY_KBN_FIXEDPURCHASE:
				creator = new BeforeAndAfterUpdateDataListCreatorFixedPurchase();
				break;
		}
		this.BeforeAndAfterUpdateDataList = creator.CreateBeforeAfterUpdateDataList(beforeUpdateHistory, afterUpdateHistory);
		this.IsOpenAllField = false;
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
	/// <summary>更新データ</summary>
	public byte[] UpdateData
	{
		get { return (byte[])this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_DATA]; }
		set { this.DataSource[Constants.FIELD_UPDATEHISTORY_UPDATE_DATA] = value; }
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
	/// 更新データ（前後）
	/// </summary>
	public BeforeAndAfterUpdateData[] BeforeAndAfterUpdateDataList
	{
		get { return (BeforeAndAfterUpdateData[])this.DataSource["BeforeAndAfterUpdateDataList"]; }
		set { this.DataSource["BeforeAndAfterUpdateDataList"] = value; }
	}
	/// <summary>全項目表示？</summary>
	public bool IsOpenAllField
	{
		get { return (bool)this.DataSource["IsOpenAllField"]; }
		set { this.DataSource["IsOpenAllField"] = value; }
	}
	#endregion
}
