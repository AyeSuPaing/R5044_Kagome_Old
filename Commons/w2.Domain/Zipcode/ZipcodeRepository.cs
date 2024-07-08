/*
=========================================================================================================
  Module      : 郵便番号リポジトリ (ZipcodeRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.Zipcode
{
	/// <summary>
	/// 郵便番号リポジトリ
	/// </summary>
	public class ZipcodeRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "Zipcode";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZipcodeRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region +GetByZipcode 住所取得
		/// <summary>
		/// 住所取得
		/// </summary>
		/// <param name="zipcode">郵便番号</param>
		public ZipcodeModel[] GetByZipcode(string zipcode)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ZIPCODE_ZIPCODE, zipcode },
			};
			var dv = Get(XML_KEY_NAME, "GetByZipcode", ht);
			return dv.Cast<DataRowView>().Select(drv => new ZipcodeModel(drv)).ToArray();
		}
		#endregion
	}
}
