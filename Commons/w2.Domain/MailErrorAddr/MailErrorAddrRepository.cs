/*
=========================================================================================================
  Module      : メールエラーアドレスリポジトリ (MailErrorAddrRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MailErrorAddr
{
	/// <summary>
	/// メールエラーアドレスリポジトリ
	/// </summary>
	internal class MailErrorAddrRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MailErrorAddr";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal MailErrorAddrRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MailErrorAddrRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="mailAddr">対象メールアドレス</param>
		/// <returns>モデル</returns>
		internal MailErrorAddrModel Get(string mailAddr)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILERRORADDR_MAIL_ADDR, mailAddr},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new MailErrorAddrModel(dv[0]);
		}
		#endregion

		/// <summary>
		/// エラーポイント登録（更新）
		/// </summary>
		/// <param name="mailAddr">対象メールアドレス</param>
		/// <param name="errorPoint">追加エラーポイント</param>
		/// <returns>更新件数</returns>
		internal int Upsert(string mailAddr, int errorPoint)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MAILERRORADDR_MAIL_ADDR, mailAddr },
				{ Constants.FIELD_MAILERRORADDR_ERROR_POINT, errorPoint }
			};
			var result = Exec(XML_KEY_NAME, "Upsert", ht);
			return result;
		}
	}
}
