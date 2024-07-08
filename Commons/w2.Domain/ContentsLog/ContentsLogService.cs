/*
=========================================================================================================
  Module      : コンテンツログサービス (ContentsLogService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;
using w2.Domain.ContentsLog.Helper;
using w2.Domain.ContentsSummaryAnalysis;

namespace w2.Domain.ContentsLog
{
	/// <summary>
	/// コンテンツログサービス
	/// </summary>
	public class ContentsLogService : ServiceBase
	{
		/// <summary>
		/// コンテンツ解析データ取得
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <returns>コンテンツ解析データ（0件の場合は0配列）</returns>
		public ContentsSummaryData[] GetContentsSummaryData(string contentsType, params string[] contentsIds)
		{
			using (var repository = new ContentsLogRepository())
			{
				var result = repository.GetContentsSummaryData(contentsType, contentsIds);
				return result;
			}
		}

		/// <summary>
		/// コンテンツ解析データ取得(当日)
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <returns>コンテンツ解析データ（0件の場合は0配列）</returns>
		public ContentsSummaryData[] GetContentsSummaryDataOfToday(string contentsType, params string[] contentsIds)
		{
			using (var repository = new ContentsLogRepository())
			{
				var result = repository.GetContentsSummaryDataOfToday(contentsType, contentsIds);
				return result;
			}
		}

		#region +Get 取得
		/// <summary>
		/// ログNoから取得
		/// </summary>
		/// <param name="logNo">ログNo</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>コンテンツログ</returns>
		public ContentsLogModel Get(int logNo, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsLogRepository(accessor))
			{
				var result = repository.Get(logNo);
				return result;
			}
		}
		#endregion

		#region +GetByOrderId 注文IDから取得
		/// <summary>
		/// 注文IDから取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>コンテンツログ</returns>
		public ContentsLogModel[] GetByOrderId(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsLogRepository(accessor))
			{
				var result = repository.GetByOrderId(orderId);
				return result;
			}
		}
		#endregion

		#region +GetForAnalysisReportOfToday 分析レポート用取得
		/// <summary>
		/// 分析レポート用取得
		/// </summary>
		/// <param name="startDate">対象日始点</param>
		/// <param name="endDate">対象終点</param>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>コンテンツログ</returns>
		public ContentsSummaryAnalysisModel[] GetForAnalysisReport(DateTime startDate, DateTime endDate, string contentsType, string[] contentsIds, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsLogRepository(accessor))
			{
				var result = repository.GetForAnalysisReport(startDate, endDate, contentsType, contentsIds);
				return result;
			}
		}
		#endregion

		#region +GetForAnalysisReportOfToday 分析レポート用取得(当日)
		/// <summary>
		/// 分析レポート用取得(当日)
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>コンテンツログ</returns>
		public ContentsSummaryAnalysisModel[] GetForAnalysisReportOfToday(string contentsType, string[] contentsIds, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsLogRepository(accessor))
			{
				var result = repository.GetForAnalysisReportOfToday(contentsType,contentsIds);
				return result;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(ContentsLogModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsLogRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Insert＆GetLogNo 登録＆取得
		/// <summary>
		/// 登録＆取得
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新規ログ番号</returns>
		public int InsertAndGetLogNo(ContentsLogModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsLogRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Update(ContentsLogModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsLogRepository(accessor))
			{
				repository.Update(model);
			}
		}
		#endregion

		#region +DeleteByContentsIds コンテンツIDによる削除
		/// <summary>
		/// コンテンツIDによる削除
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteByContentsIds(string contentsType, string[] contentsIds, SqlAccessor accessor = null)
		{
			using (var repository = new ContentsLogRepository(accessor))
			{
				repository.DeleteByContentsIds(contentsType, contentsIds);
				repository.DeleteContentsSummaryAnalysisByContentsIds(contentsType,contentsIds);
			}
		}
		#endregion
	}
}