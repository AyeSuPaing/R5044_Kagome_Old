/*
=========================================================================================================
  Module      : 特集ページサービス (FeaturePageService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;

namespace w2.Domain.FeaturePage
{
	/// <summary>
	/// 特集ページサービス
	/// </summary>
	public class FeaturePageService : ServiceBase, IFeaturePageService
	{
		#region +GetAllPage 全てのページ取得
		/// <summary>
		/// 全てのページ取得
		/// </summary>
		/// <returns>特集ページモデル配列</returns>
		public FeaturePageModel[] GetAllPage()
		{
			using (var repository = new FeaturePageRepository())
			{
				var model = repository.GetAllPage();
				return model;
			}
		}
		#endregion

		#region +GetAllPageWithContents 全てのページ取得
		/// <summary>
		/// 全てのページ取得
		/// </summary>
		/// <returns>特集ページモデル配列</returns>
		public FeaturePageModel[] GetAllPageWithContents()
		{
			using (var repository = new FeaturePageRepository())
			{
				var model = repository.GetAllPage();
				model.ToList().ForEach(item => item.Contents = repository.GetContents(item.FeaturePageId));
				return model;
			}
		}
		#endregion

		#region +Get 特集ページ取得
		/// <summary>
		/// 特集ページ取得
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>モデル</returns>
		public FeaturePageModel Get(long pageId)
		{
			using (var repository = new FeaturePageRepository())
			{
				var model = repository.Get(pageId);

				if (model != null) model.Contents = repository.GetContents(model.FeaturePageId);
				return model;
			}
		}
		#endregion

		#region +Get 特集ページ取得
		/// <summary>
		/// 取得件数を指定して特集ページ取得
		/// </summary>
		/// <param name="num">取得件数</param>
		/// <returns>モデル</returns>
		public FeaturePageModel GetBySpecifyingNumber(int num)
		{
			using (var repository = new FeaturePageRepository())
			{
				var model = repository.GetBySpecifyingNumber(num);

				if (model != null) model.Contents = repository.GetContents(model.FeaturePageId);
				return model;
			}
		}
		#endregion

		#region +GetByFileName ファイル名で取得
		/// <summary>
		/// ファイル名で取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>モデル</returns>
		public FeaturePageModel GetByFileName(string fileName)
		{
			using (var repository = new FeaturePageRepository())
			{
				var model = repository.GetByFileName(fileName);

				if (model == null)
				{
					return new FeaturePageModel
					{
						Contents = Enumerable.Empty<FeaturePageContentsModel>().ToArray()
					};
				}

				model.Contents = repository.GetContents(model.FeaturePageId);
				return model;
			}
		}
		#endregion

		#region +GetContents 特集ページIDでコンテンツをすべて取得
		/// <summary>
		/// 特集ページIDでコンテンツをすべて取得
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>特集ページコンテンツモデル配列</returns>
		public FeaturePageContentsModel[] GetContents(long pageId)
		{
			using (var repository = new FeaturePageRepository())
			{
				var model = repository.GetContents(pageId);
				return model;
			}
		}
		#endregion

		#region +Insert 特集ページ登録
		/// <summary>
		/// 特集ページ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>新規特集ページID</returns>
		public int Insert(FeaturePageModel model)
		{
			using (var repository = new FeaturePageRepository())
			{
				var id = repository.Insert(model);
				return id;
			}
		}
		#endregion

		#region +Update 特集ページ更新
		/// <summary>
		/// 特集ページ更新
		/// </summary>
		/// <param name="model">モデル</param>
		public void Update(FeaturePageModel model)
		{
			using (var repository = new FeaturePageRepository())
			{
				repository.Update(model);

				// 特集ページコンテンツ全削除処理
				repository.DeleteContents(model.FeaturePageId);

				foreach (var content in model.Contents)
				{
					repository.InsertContents(content);
				}
			}
		}
		#endregion

		#region +UpdatePageSort 特集ページの表示順更新
		/// <summary>
		/// 特集ページの表示順更新
		/// </summary>
		/// <param name="models">特集ページモデル配列</param>
		public void UpdatePageSort(FeaturePageModel[] models)
		{
			using (var repository = new FeaturePageRepository())
			{
				foreach (var model in models)
				{
					repository.UpdatePageSort(model);
				}
			}
		}
		#endregion

		#region +UpdateManagementTitle 管理用タイトル更新
		/// <summary>
		///  管理用タイトル更新
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="title">管理用タイトル</param>
		public void UpdateManagementTitle(long pageId, string title)
		{
			var model = Get(pageId);
			if (model == null) return;
			model.ManagementTitle = title;
			Update(model);
		}
		#endregion

		#region +Delete 特集ページ削除
		/// <summary>
		/// 特集ページ削除
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(long pageId)
		{
			using (var repository = new FeaturePageRepository())
			{
				var model = repository.Delete(pageId);
				return model;
			}
		}
		#endregion

		#region +GetRootCategoryItem 最上位カテゴリ取得
		/// <summary>
		/// 最上位カテゴリ取得
		/// </summary>
		/// <returns>最上位カテゴリ情報</returns>
		public FeaturePageModel[] GetRootCategoryItem()
		{
			using (var repository = new FeaturePageRepository())
			{
				var results = repository.GetRootCategoryItem();
				return results;
			}
		}
		#endregion

		#region +GetChildCategoryItem 子カテゴリ取得
		/// <summary>
		/// 子カテゴリ取得
		/// </summary>
		/// <returns>子カテゴリ情報</returns>
		public FeaturePageModel[] GetChildCategoryItem(string parentCategoryId)
		{
			using (var repository = new FeaturePageRepository())
			{
				var results = repository.GetChildCategoryItem(parentCategoryId);
				return results;
			}
		}
		#endregion

		#region +ブランド情報取得
		/// <summary>
		/// ブランド情報取得
		/// </summary>
		/// <returns>ブランド情報</returns>
		public FeaturePageModel[] GetBrand()
		{
			using (var repository = new FeaturePageRepository())
			{
				var result = repository.GetBrand();
				return result;
			}
		}
		#endregion
	}
}