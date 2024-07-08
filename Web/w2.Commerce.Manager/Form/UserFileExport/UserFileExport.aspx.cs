/*
=========================================================================================================
  Module      : ユーザー関連ファイル出力ページ(UserFileExport.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.Common.Extensions;
using w2.Domain.UserCreditCard;

public partial class Form_UserFileExport_UserFileExport : System.Web.UI.Page
{
	/// <summary>
	/// ページ読み込み
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
		//------------------------------------------------------
		// 対象データ取得
		//------------------------------------------------------
		// 削除対象日付取得
		var deleteTargetDateFrom = DateTime.Now.AddDays(Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_DELETED_DAY_FROM * -1);
		var deleteTargetDateTo = DateTime.Now.AddDays(Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_MEMBER_DELETED_DAY_TO * -1);

		var dvSource =
			MasterFileExport.ExtractExportData(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER,
				"User",
				"GetUserMaster",
				(Hashtable)Session[Constants.SESSION_KEY_PARAM],
				"w2_User.user_id");

		var userIdList = dvSource
			.ToHashtableList()
			.Select(dv => (string)dv[Constants.FIELD_USER_USER_ID]).ToList();

		// 削除対象のクレカ取得
		var deleteCreditCard =
			new UserCreditCardService()
				.GetDeleteMemberByEScott(userIdList, deleteTargetDateFrom, deleteTargetDateTo);

		var deleteKaiinnIdGroup =
			deleteCreditCard.Select(uc => new DeleteMember(uc.UserId, uc.CooperationId)).ToList();
		//------------------------------------------------------
		// マスタ情報出力
		//------------------------------------------------------
		var fileName = "e-SCOTT_Kaiin_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
		Response.ContentEncoding = Encoding.UTF8;
		Response.ContentType = "application/csv";
		Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
		//------------------------------------------------------
		// データ加工
		//------------------------------------------------------
		var body = CreateCsvDeleteMember(deleteKaiinnIdGroup);
		Response.Write(body);
		Response.End();
    }

	/// <summary>
	/// 削除会員CSV生成
	/// </summary>
	/// <param name="deleteMember">削除会員</param>
	/// <returns>CSV文字列</returns>
	private string CreateCsvDeleteMember(List<DeleteMember> deleteMember)
	{
		var body = new StringBuilder();

		body.AppendLine(Constants.FIELD_USER_USER_ID + ",kaiin_id");

		foreach (var member in deleteMember)
		{
			body.AppendLine(member.UserId + "," + member.KaiinId);
		}

		var result = body.ToString();
		return result;
	}

	/// <summary>
	/// 削除会員クラス
	/// </summary>
	private class DeleteMember
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="kaiinId">会員ID</param>
		public DeleteMember(string userId, string kaiinId)
		{
			UserId = userId;
			KaiinId = kaiinId;
		}

		/// <summary>ユーザーID</summary>
		public string UserId { get; private set; }
		/// <summary>会員ID</summary>
		public string KaiinId { get; private set; }
	}
}