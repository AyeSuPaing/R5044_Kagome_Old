/*
=========================================================================================================
  Module      : メッセージアプリ向けコンテンツサービスクラス(MessagingAppContentsService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.MessagingAppContents
{
	/// <summary>
	/// メッセージアプリ向けコンテンツサービスクラス
	/// </summary>
	public class MessagingAppContentsService : ServiceBase
	{
		/// <summary>
		/// メッセージアプリ向けコンテンツ取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="messagingAppKbn">メッセージアプリ区分</param>
		/// <returns>メッセージアプリ向けコンテンツ</returns>
		public MessagingAppContentsModel[] GetAllContentsEachMessagingAppKbn(string deptId, string masterKbn, string masterId, string messagingAppKbn)
		{
			using (var repository = new MessagingAppContentsRepository())
			{
				var models = repository.GetAllContentsEachMessagingAppKbn(deptId, masterKbn, masterId, messagingAppKbn);
				return models;
			}
		}

		/// <summary>
		/// メッセージアプリ向けコンテンツアップサート
		/// </summary>
		/// <param name="model">メッセージアプリ向けコンテンツ</param>
		/// <returns>処理件数</returns>
		public int UpsertContents(MessagingAppContentsModel model)
		{
			using (var repository = new MessagingAppContentsRepository())
			{
				var result = repository.UpsertContents(model);
				return result;
			}
		}

		/// <summary>
		/// メッセージアプリ向けコンテンツ削除(マスタID毎)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterKbn">メールテンプレートID</param>
		/// <param name="masterId">メッセージサービス名</param>
		/// <returns>処理件数</returns>
		public int DeleteAllContentsEachMasterId(string deptId, string masterKbn, string masterId)
		{
			using (var repository = new MessagingAppContentsRepository())
			{
				var result = repository.DeleteAllContentsEachMasterId(deptId, masterKbn, masterId);
				return result;
			}
		}

		/// <summary>
		/// メッセージアプリ向けコンテンツ削除(枝番毎)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="messagingAppKbn">メッセージアプリ区分</param>
		/// <param name="branchNo">枝番</param>
		/// <returns>処理件数</returns>
		public int DeleteContentsEachBranchNo(string deptId, string masterKbn, string masterId, string messagingAppKbn, string branchNo)
		{
			using (var repository = new MessagingAppContentsRepository())
			{
				var result = repository.DeleteContentsEachBranchNo(deptId, masterKbn, masterId, messagingAppKbn, branchNo);
				return result;
			}
		}
	}
}
