/*
=========================================================================================================
  Module      : 特集ページリポジトリ (FeaturePageRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.FeaturePage
{
	/// <summary>
	/// 特集ページリポジトリ
	/// </summary>
	public class FeaturePageRepository : RepositoryBase
	{
		/// <returns>キー</returns>
		private const string XML_KEY_NAME = "FeaturePage";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">Sql accessor</param>
		public FeaturePageRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetAllPage 特集ページ全取得
		/// <summary>
		/// 特集ページ全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		internal FeaturePageModel[] GetAllPage()
		{
			var dv = Get(XML_KEY_NAME, "GetAllPage");
			return dv.Cast<DataRowView>().Select(item => new FeaturePageModel(item)).ToArray();
		}
		#endregion

		#region ~Get 特集ページ取得
		/// <summary>
		/// 特集ページ取得
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>モデル</returns>
		internal FeaturePageModel Get(long pageId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FEATUREPAGE_FEATURE_PAGE_ID, pageId },
			};
			var dv = Get(XML_KEY_NAME, "Get", input);
			if (dv.Count == 0) return null;
			return new FeaturePageModel(dv[0]);
		}
		#endregion

		#region ~Get 特集ページ取得
		/// <summary>
		/// 取得件数を指定して特集ページ取得
		/// </summary>
		/// <param name="num">取得件数</param>
		/// <returns>モデル</returns>
		internal FeaturePageModel GetBySpecifyingNumber(int num)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FEATUREPAGE_FEATURE_PAGE_ID, num },
			};
			var dv = Get(XML_KEY_NAME, "GetBySpecifyingNumber", input);
			if (dv.Count == 0) return null;
			return new FeaturePageModel(dv[0]);
		}
		#endregion

		#region ~GetByFileName ファイル名で取得
		/// <summary>
		/// ファイル名で取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>モデル</returns>
		internal FeaturePageModel GetByFileName(string fileName)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FEATUREPAGE_FILE_NAME, fileName },
			};
			var dv = Get(XML_KEY_NAME, "GetByFileName", input);
			if (dv.Count == 0) return null;
			return new FeaturePageModel(dv[0]);
		}
		#endregion

		#region ~GetContents 特集ページコンテンツ取得
		/// <summary>
		/// 特集ページコンテンツ取得
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>特集ページコンテンツモデル配列</returns>
		internal FeaturePageContentsModel[] GetContents(long pageId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FEATUREPAGECONTENTS_FEATURE_PAGE_ID, pageId }
			};
			var dv = Get(XML_KEY_NAME, "GetContents", input);
			return dv.Cast<DataRowView>().Select(item => new FeaturePageContentsModel(item)).ToArray();
		}
		#endregion

		#region ~Insert 特集ページ登録
		/// <summary>
		/// 特集ページ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>新規特集ページID</returns>
		internal int Insert(FeaturePageModel model)
		{
			var result = Get(XML_KEY_NAME, "Insert", model.DataSource);
			return int.Parse(result[0][0].ToString());
		}
		#endregion

		#region ~InsertContents 特集ページコンテンツ登録
		/// <summary>
		/// 特集ページコンテンツ登録
		/// </summary>
		/// <param name="model">特集ページコンテンツモデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int InsertContents(FeaturePageContentsModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertContents", model.DataSource);
			return result;
		}
		#endregion

		#region ~Update 特集ページ更新
		/// <summary>
		/// 特集ページ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(FeaturePageModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~UpdatePageSort 特集ページ順序更新
		/// <summary>
		/// 特集ページ順序 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdatePageSort(FeaturePageModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdatePageSort", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 特集ページ削除
		/// <summary>
		/// 特集ページ削除
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(long pageId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FEATUREPAGE_FEATURE_PAGE_ID, pageId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", input);
			return result;
		}
		#endregion

		#region ~DeleteContents 特集ページコンテンツ削除
		/// <summary>
		/// 特集ページコンテンツ削除
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteContents(long pageId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FEATUREPAGECONTENTS_FEATURE_PAGE_ID, pageId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteContents", input);
			return result;
		}
		#endregion

		#region +GetRootCategoryItem 最上位カテゴリを全て取得
		/// <summary>
		/// 最上位カテゴリ取得
		/// </summary>
		/// <returns>最上位カテゴリ情報</returns>
		public FeaturePageModel[] GetRootCategoryItem()
		{
			var dv = Get(XML_KEY_NAME, "GetRootCategoryItem");
			return dv.Cast<DataRowView>().Select(item => new FeaturePageModel(item)).ToArray();
		}
		#endregion

		#region +GetChildCategoryItem 子カテゴリ取得
		/// <summary>
		/// 子カテゴリ取得
		/// </summary>
		/// <returns>子カテゴリ情報</returns>
		public FeaturePageModel[] GetChildCategoryItem(string parentCategory)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID, parentCategory }
			};
			var dv = Get(XML_KEY_NAME, "GetChildCategoryItem", ht);
			return dv.Cast<DataRowView>().Select(item => new FeaturePageModel(item)).ToArray();
		}
		#endregion

		#region +GetBrandCount ブランド取得
		/// <summary>
		/// ブランド取得
		/// </summary>
		/// <returns>ブランド情報</returns>
		public FeaturePageModel[] GetBrand()
		{
			var dv = Get(XML_KEY_NAME, "GetBrand");
			return dv.Cast<DataRowView>().Select(item => new FeaturePageModel(item)).ToArray();
		}
		#endregion
	}
}