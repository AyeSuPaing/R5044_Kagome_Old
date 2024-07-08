/*
=========================================================================================================
  Module      : コンテンツタグサービス (ContentsTagService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ContentsTag
{
	/// <summary>
	/// コンテンツタグサービス
	/// </summary>
	public class ContentsTagService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="contentsTagId">コンテンツタグID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public ContentsTagModel Get(long contentsTagId, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsTagRepository(accessor))
			{
				var model = repository.Get(contentsTagId);
				return model;
			}
		}
		#endregion

		#region +GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		public ContentsTagModel[] GetAll()
		{
			using (var repository = new ContentsTagRepository())
			{
				var model = repository.GetAll();
				return model;
			}
		}
		#endregion

		#region +GetByName 名前で取得
		/// <summary>
		/// 名前で取得
		/// </summary>
		/// <param name="tagName">コンテンツタグ名</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>モデル</returns>
		public ContentsTagModel GetByName(string tagName, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsTagRepository(accessor))
			{
				var model = repository.GetByName(tagName);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(ContentsTagModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsTagRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +InsertAndGet 登録＆取得
		/// <summary>
		/// 登録＆取得
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public ContentsTagModel InsertAndGet(ContentsTagModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsTagRepository(accessor))
			{
				repository.Insert(model);

				var result = repository.GetByName(model.ContentsTagName);
				return result;
			}
		}
		#endregion
	}
}
