/*
=========================================================================================================
  Module      : 外部リンク設定サービス(CsExternalLinkService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.Domain.ExternalLink
{
	/// <summary>
	/// 外部リンクサービス
	/// </summary>
	public class CsExternalLinkService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="linkId">リンクID</param>
		/// <returns>該当例リンク</returns>
		public CsExternalLinkModel Get(string deptId, string linkId)
		{
			using (var repository = new CsExternalLinkRepository())
			{
				var model = repository.Get(deptId, linkId);
				return model;
			}
		}

		/// <summary>
		/// フラグオンリンク取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <returns>フラグ付けリスト</returns>
		public CsExternalLinkModel[] GetValidFlg(string deptId, string validFlg)
		{
			using (var repository = new CsExternalLinkRepository())
			{
				var model = repository.GetValidFlg(deptId, validFlg);
				return model;
			}
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="externalLinkId">外部リンク設定ID</param>
		/// <param name="externalLinkTitle">外部リンク設定タイトル</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="beginRow">開始NO</param>
		/// <param name="endRow">終了NO</param>
		/// <returns>検索値数</returns>
		public CsExternalLinkModel[] Search(
			string deptId,
			string externalLinkId,
			string externalLinkTitle,
			string validFlg,
			int beginRow,
			int endRow)
		{
			using (var repository = new CsExternalLinkRepository())
			{
				var result = repository.Search(
					deptId,
					externalLinkId,
					externalLinkTitle,
					validFlg,
					beginRow,
					endRow);
				return result;
			}
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">登録用データ</param>
		public bool Register(CsExternalLinkModel model)
		{
			// Create new answer template ID
			model.LinkId = NumberingUtility.CreateKeyId(model.DeptId, Constants.NUMBER_KEY_CS_EXTERNALLINK_ID, 3);
			using (var repository = new CsExternalLinkRepository())
			{
				// 登録
				var register = repository.Register(model);
				return (register > 0);
			}
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">更新用データ</param>
		public bool Update(CsExternalLinkModel model)
		{
			using (var repository = new CsExternalLinkRepository())
			{
				// 更新
				var update = repository.Update(model);
				return (update > 0);
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="model">削除用データ</param>
		public bool Delete(CsExternalLinkModel model)
		{
			using (var repository = new CsExternalLinkRepository())
			{
				// 削除
				var delete = repository.Delete(model);
				return (delete > 0);
			}
		}
	}
}
