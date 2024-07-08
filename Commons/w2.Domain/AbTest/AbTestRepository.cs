/*
=========================================================================================================
  Module      : ABテストリポジトリ (AbTestRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.AbTest
{
	/// <summary>
	/// ABテストリポジトリ
	/// </summary>
	internal class AbTestRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		private const string XML_KEY_NAME = "AbTest";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal AbTestRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal AbTestRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="searchWord">検索条件：タイトル又はファイル名</param>
		/// <returns>検索結果</returns>
		internal AbTestModel[] Search(string searchWord)
		{
			var param = new Hashtable
			{
				{ "search_word", searchWord }
			};
			var dv = Get(XML_KEY_NAME, "Search", param);
			return dv.Cast<DataRowView>().Select(drv => new AbTestModel(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// ABテスト取得
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>モデル</returns>
		internal AbTestModel Get(string abTestId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ABTEST_AB_TEST_ID, abTestId }
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return (dv.Count > 0) ? new AbTestModel(dv[0]) : null;
		}
		#endregion

		#region ~GetAllItemByAbTestId ABテストアイテム取得
		/// <summary>
		/// ABテストアイテム取得
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>ABテストアイテム</returns>
		internal AbTestItemModel[] GetAllItemByAbTestId(string abTestId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ABTESTITEM_AB_TEST_ID, abTestId }
			};
			var dv = Get(XML_KEY_NAME, "GetAllItemByAbTestId", ht);
			return dv.Cast<DataRowView>().Select(drv => new AbTestItemModel(drv)).ToArray();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// ABテスト登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Insert(AbTestModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		#region ~InsertItem ABテストアイテム登録
		/// <summary>
		/// ABテストアイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int InsertItem(AbTestItemModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertItem", model.DataSource);
			return result;
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// ABテスト更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(AbTestModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// ABテスト削除
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(string abTestId)
		{
			var result = Exec(XML_KEY_NAME, "Delete", new Hashtable { { Constants.FIELD_ABTEST_AB_TEST_ID, abTestId } });
			return result;
		}
		#endregion

		#region ~DeleteAllItemByAbTestId ABテストアイテム削除
		/// <summary>
		/// ABテストアイテム削除
		/// </summary>
		/// <param name="abTestId">ABテストID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteAllItemByAbTestId(string abTestId)
		{
			var result = Exec(XML_KEY_NAME, "DeleteAllItemByAbTestId", new Hashtable { { Constants.FIELD_ABTEST_AB_TEST_ID, abTestId } });
			return result;
		}
		#endregion
	}
}
