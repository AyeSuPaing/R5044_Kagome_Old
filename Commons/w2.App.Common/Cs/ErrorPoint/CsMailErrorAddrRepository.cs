/*
=========================================================================================================
  Module      : メールエラーアドレスリポジトリ(CsMailErrorAddrRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.Cs.ErrorPoint
{
	public class CsMailErrorAddrRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "MailErrorAddr";

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>取得データ</returns>
		public DataView Get(string mailAddr)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_MAILERRORADDR_MAIL_ADDR, mailAddr);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
	}
}
