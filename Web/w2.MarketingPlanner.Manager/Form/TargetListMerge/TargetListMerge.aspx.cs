/*
=========================================================================================================
  Module      : ターゲットリストマージページ処理(TargetListMerge.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.TargetList;
using w2.Common.Util;

public partial class Form_TargetListMerge_TargetListMerge : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>
	/// 選択されたマージパターン＆ターゲットリストにてマージ実施する。
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnMerge_Click(object sender, EventArgs e)
	{
		// データの入力チェックします。
		CheckInputData();

		// 有効なデータをチェックします。
		CheckExistData();

		// 抽出実行
		if (cbTargetExtract1.Checked) TargetListUtility.ExecuteExtractTargetListData(this.LoginOperatorDeptId, hfTargetListId1.Value);
		if (cbTargetExtract2.Checked) TargetListUtility.ExecuteExtractTargetListData(this.LoginOperatorDeptId, hfTargetListId2.Value);

		// データをマージします。
		var newTargetListId = ExecuteMergeTargetList();

		// 結果を表示します。
		DisplayMergeResult(newTargetListId);
	}

	/// <summary>
	/// データの入力チェックします。
	/// </summary>
	private void CheckInputData()
	{
		var errorMsg = string.Empty;

		if (string.IsNullOrEmpty(hfTargetListId1.Value))
		{
			errorMsg += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SELECT_TARGER_LIST1_INFORMATION);
		}
		if (string.IsNullOrEmpty(hfTargetListId2.Value))
		{
			errorMsg += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SELECT_TARGER_LIST2_INFORMATION);
		}
		if ((rbMergeKbnA.Checked == false) && (rbMergeKbnC.Checked == false) && (rbMergeKbnD.Checked == false) && (rbMergeKbnB.Checked == false))
		{
			errorMsg += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SELECT_MERGE_PATTERN);
		}
		errorMsg += Validator.CheckNecessaryError(
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_TARGET_LIST_NAME),
			tbTargetListName.Text);
		errorMsg += Validator.CheckByteLengthMaxError(
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_TARGET_LIST_NAME),
			tbTargetListName.Text,
			60);

		if (string.IsNullOrEmpty(errorMsg) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMsg;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// データの存在チェック
	/// </summary>
	private void CheckExistData()
	{
		var errorMsg = string.Empty;

		var targetListAExist = TargetListUtility.IsTargetListExists(this.LoginOperatorDeptId, hfTargetListId1.Value);
		var targetListBExist = TargetListUtility.IsTargetListExists(this.LoginOperatorDeptId, hfTargetListId2.Value);

		if (targetListAExist == false)
		{
			errorMsg += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SELECTED_TARGET1_NOT_EXIST);
		}
		if (targetListBExist == false)
		{
			errorMsg += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SELECTED_TARGET2_NOT_EXIST);
		}
		if (string.IsNullOrEmpty(errorMsg) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMsg;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// ターゲットリストをマージします。
	/// </summary>
	/// <returns>作成したターゲットリスト</returns>
	private string ExecuteMergeTargetList()
	{
		var newTargetListId = string.Empty;
		using (var sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();
			try
			{
				// 新規ターゲットリストを挿入
				newTargetListId = PreCreateTargetList(sqlAccessor);

				// データをマージします。
				MergeTargetListDataOnDb(newTargetListId, sqlAccessor);

				// マージ結果をチェック
				CheckMergeResult(newTargetListId, sqlAccessor);

				// コミット
				sqlAccessor.CommitTransaction();
			}
			catch (System.Threading.ThreadAbortException)
			{
				sqlAccessor.RollbackTransaction();
			}
			catch (Exception)
			{
				sqlAccessor.RollbackTransaction();
				throw;
			}
		}
		return newTargetListId;
	}

	/// <summary>
	/// 結果を表示します。
	/// </summary>
	/// <param name="newTargetListId">ターゲットリストID</param>
	private void DisplayMergeResult(string newTargetListId)
	{
		var mergeResult = new Hashtable();
		var targetList1 = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, hfTargetListId1.Value);
		var targetList2 = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, hfTargetListId2.Value);
		var targetListMerge = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, newTargetListId);
		mergeResult.Add(Constants.FIELD_TARGETLIST_TARGET_NAME + "1", hfTargetListName1.Value);
		mergeResult.Add(Constants.FIELD_TARGETLIST_TARGET_NAME + "2", hfTargetListName2.Value);
		mergeResult.Add(Constants.FIELD_TARGETLIST_DATA_COUNT + "1", targetList1[0][Constants.FIELD_TARGETLIST_DATA_COUNT]);
		mergeResult.Add(Constants.FIELD_TARGETLIST_DATA_COUNT + "2", targetList2[0][Constants.FIELD_TARGETLIST_DATA_COUNT]);
		mergeResult.Add(Constants.FIELD_TARGETLIST_TARGET_NAME, tbTargetListName.Text);
		mergeResult.Add(Constants.FIELD_TARGETLIST_DATA_COUNT, targetListMerge[0][Constants.FIELD_TARGETLIST_DATA_COUNT]);
		mergeResult.Add(Constants.FIELD_TARGETLIST_TARGET_TYPE, (new List<CheckBox> { rbMergeKbnA, rbMergeKbnD, rbMergeKbnB, rbMergeKbnC }.FirstOrDefault(cd => cd.Checked)).Text);
		Session[Constants.SESSION_KEY_PARAM] = mergeResult;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_MERGE_COMPLETE);
	}

	/// <summary>
	/// マージ結果検証
	/// </summary>
	/// <param name="newTargetListId">ターゲットリストID</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	private void CheckMergeResult(string newTargetListId, SqlAccessor sqlAccessor)
	{
		// マージ件数取得
		var targetListResult = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, newTargetListId, sqlAccessor);

		// 0件の場合はエラー画面へリダイレクト。
		// tryの中ではリダイレクトの際に ThreadAbortException が発生するのでロールバックはその中で行う
		if ((int) targetListResult[0][Constants.FIELD_TARGETLIST_DATA_COUNT] == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COULD_NOT_MERGE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// データをマージします。
	/// </summary>
	/// <param name="newTargetListId">ターゲットリストID</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	private void MergeTargetListDataOnDb(string newTargetListId, SqlAccessor sqlAccessor)
	{
		var sqlSegment = rbMergeKbnA.Checked ? "MergeTargetListFullOuterJoin" : rbMergeKbnB.Checked ? "MergeTargetListInnerJoin" : rbMergeKbnC.Checked ? "MergeTargetListLeftJoinNotInner" : rbMergeKbnD.Checked ? "MergeTargetListFullOuterJoinNotInner" : string.Empty;

		using (var sqlStatement = new SqlStatement("TargetListData", sqlSegment))
		{
			var sqlParams = new Hashtable 
			{ 
				{ Constants.FIELD_TARGETLISTDATA_DEPT_ID, this.LoginOperatorDeptId },
				{ Constants.FIELD_TARGETLISTDATA_MASTER_ID+"_1", hfTargetListId1.Value },
				{ Constants.FIELD_TARGETLISTDATA_MASTER_ID+"_2", hfTargetListId2.Value },
				{ Constants.FIELD_TARGETLISTDATA_MASTER_ID, newTargetListId },
				{ Constants.FIELD_TARGETLISTDATA_TARGET_KBN, Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST },
			};
			sqlStatement.ExecStatement(sqlAccessor, sqlParams);
		}

		// ターゲット リストのデータ カウントを更新します。
		TargetListUtility.UpdateTargetListDataCountByTargetListData(this.LoginOperatorDeptId, newTargetListId, sqlAccessor);
	}

	/// <summary>
	/// 新規ターゲットリストを挿入
	/// </summary>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>ターゲットリストID</returns>
	private string PreCreateTargetList(SqlAccessor sqlAccessor)
	{
		// 新規ターゲットリストを挿入
		using (var sqlStatement = new SqlStatement("TargetList", "InsertTargetList"))
		{
			var sqlParams = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_DEPT_ID, this.LoginOperatorDeptId},
				{Constants.FIELD_TARGETLIST_TARGET_ID, NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_TARGET_LIST_ID, 10)},
				{Constants.FIELD_TARGETLIST_TARGET_NAME, tbTargetListName.Text},
				{Constants.FIELD_TARGETLIST_TARGET_TYPE, Constants.FLG_TARGETLIST_TARGET_TYPE_MERGE},
				{Constants.FIELD_TARGETLIST_TARGET_CONDITION, string.Empty},
				{Constants.FIELD_TARGETLIST_LAST_CHANGED, this.LoginOperatorName},
				{Constants.FIELD_TARGETLIST_EXEC_TIMING, string.Empty},
				{Constants.FIELD_TARGETLIST_SCHEDULE_KBN, string.Empty},
				{Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK, string.Empty},
				{Constants.FIELD_TARGETLIST_SCHEDULE_YEAR, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_MONTH, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_DAY, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_HOUR, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE, null},
				{Constants.FIELD_TARGETLIST_SCHEDULE_SECOND, null},
				{Constants.FIELD_TARGETLIST_DEL_FLG, "0"}
			};
			var result = sqlStatement.SelectSingleStatement(sqlAccessor, sqlParams);
			return result.Table.Rows[0][Constants.FIELD_TARGETLIST_TARGET_ID].ToString();
		}
	}
}