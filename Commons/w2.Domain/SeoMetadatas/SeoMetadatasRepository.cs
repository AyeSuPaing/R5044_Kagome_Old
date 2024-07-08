/*
=========================================================================================================
  Module      : SEOメタデータリポジトリ (SeoMetadatasRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.SeoMetadatas
{
	/// <summary>
	/// SEOメタデータリポジトリ
	/// </summary>
	public class SeoMetadatasRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "SeoMetadatas";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SeoMetadatasRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SeoMetadatasRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="dataKbn">データ区分</param>
		/// <returns>モデル</returns>
		internal SeoMetadatasModel GetSeoMetadata(string shopId, string dataKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SEOMETADATAS_SHOP_ID, shopId},
				{Constants.FIELD_SEOMETADATAS_DATA_KBN, dataKbn},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new SeoMetadatasModel(dv[0]);
		}
		#endregion

		#region +GetAll すべて取得
		/// <summary>
		/// すべて取得
		/// </summary>
		/// <returns>モデルリスト</returns>
		public SeoMetadatasModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			return dv.Cast<DataRowView>().Select(drv => new SeoMetadatasModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(SeoMetadatasModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(SeoMetadatasModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdateForDefaultText デフォルト文言更新
		/// <summary>
		/// デフォルト文言更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateForDefaultText(SeoMetadatasModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateForDefaultText", model.DataSource);
			return result;
		}
		#endregion
	}
}
