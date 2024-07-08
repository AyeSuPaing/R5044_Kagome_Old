/*
=========================================================================================================
  Module      : メッセージアプリ向けコンテンツリポジトリクラス(MessagingAppContentsRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MessagingAppContents
{
	/// <summary>
	/// メッセージアプリ向けコンテンツリポジトリクラス
	/// </summary>
	public class MessagingAppContentsRepository : RepositoryBase
	{
		/// <returns>クエリ用XML</returns>
		private const string XML_KEY_NAME = "MessagingAppContents";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal MessagingAppContentsRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MessagingAppContentsRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}

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
			var ht = new Hashtable
			{
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_DEPT_ID, deptId },
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_KBN, masterKbn },
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_ID, masterId },
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_MESSAGING_APP_KBN, messagingAppKbn }
			};
			var dv = Get(XML_KEY_NAME, "GetAllContentsEachMessagingAppKbn", ht);
			return (dv.Count > 0)
				? dv.Cast<DataRowView>().Select(x => new MessagingAppContentsModel(x)).ToArray()
				: new MessagingAppContentsModel[0];
		}

		/// <summary>
		/// メッセージアプリ向けコンテンツアップサート
		/// </summary>
		/// <param name="model">メッセージアプリ向けコンテンツ</param>
		/// <returns>処理件数</returns>
		public int UpsertContents(MessagingAppContentsModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpsertContents", model.DataSource);
			return result;
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
			var ht = new Hashtable
			{
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_DEPT_ID, deptId },
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_KBN, masterKbn },
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_ID, masterId }
			};
			var result = Exec(XML_KEY_NAME, "DeleteAllContentsEachMasterId", ht);
			return result;
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
			var ht = new Hashtable
			{
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_DEPT_ID, deptId },
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_KBN, masterKbn },
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_MASTER_ID, masterId },
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_MESSAGING_APP_KBN, messagingAppKbn },
				{ Constants.FIELD_MESSAGINGAPPCONTENTS_BRANCH_NO, branchNo }
			};
			var result = Exec(XML_KEY_NAME, "DeleteContentsEachBranchNo", ht);
			return result;
		}
	}
}
