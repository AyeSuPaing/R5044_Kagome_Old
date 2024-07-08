/*
=========================================================================================================
  Module      : コンテンツタグリポジトリ (ContentsTagRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ContentsTag
{
	/// <summary>
	/// コンテンツタグリポジトリ
	/// </summary>
	internal class ContentsTagRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ContentsTag";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ContentsTagRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal ContentsTagRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="contentsTagId">コンテンツタグID</param>
		/// <returns>モデル</returns>
		internal ContentsTagModel Get(long contentsTagId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_CONTENTSTAG_CONTENTS_TAG_ID, contentsTagId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new ContentsTagModel(dv[0]);
		}
		#endregion

		#region ~GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		internal ContentsTagModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new ContentsTagModel(drv)).ToArray();
		}
		#endregion

		#region ~GetByName 名前で取得
		/// <summary>
		/// 名前で取得
		/// </summary>
		/// <param name="tagName">コンテンツタグ名</param>
		/// <returns>モデル</returns>
		internal ContentsTagModel GetByName(string tagName)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_CONTENTSTAG_CONTENTS_TAG_NAME, tagName},
			};
			var dv = Get(XML_KEY_NAME, "GetByName", ht);
			if (dv.Count == 0) return null;
			return new ContentsTagModel(dv[0]);
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(ContentsTagModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion
	}
}
