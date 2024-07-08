/*
=========================================================================================================
  Module      : コンテンツログリポジトリ (ContentsLogRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.ContentsLog.Helper;
using w2.Domain.ContentsSummaryAnalysis;

namespace w2.Domain.ContentsLog
{
	/// <summary>
	/// コンテンツログリポジトリ
	/// </summary>
	internal class ContentsLogRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ContentsLog";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ContentsLogRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal ContentsLogRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetContentsSummaryData コンテンツ解析データ取得
		/// <summary>
		/// コンテンツ解析データ取得
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <returns>コンテンツ解析データ（0件の場合は0配列）</returns>
		public ContentsSummaryData[] GetContentsSummaryData(string contentsType, params string[] contentsIds)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_TYPE, contentsType},
				{Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_ID, (contentsIds.Length > 0) ? contentsIds[0] : ""},
			};
			string param = string.Join(",", contentsIds.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ params @@", param);
			var dv = Get(XML_KEY_NAME, "GetContentsSummaryData", input, replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new ContentsSummaryData(drv)).ToArray();
		}
		#endregion

		#region ~GetContentsSummaryDataToday コンテンツ解析データ取得
		/// <summary>
		/// コンテンツ解析データ取得
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <returns>コンテンツ解析データ（0件の場合は0配列）</returns>
		public ContentsSummaryData[] GetContentsSummaryDataOfToday(string contentsType, params string[] contentsIds)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_TYPE, contentsType},
				{Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_ID, (contentsIds.Length > 0) ? contentsIds[0] : ""},
			};
			string param = string.Join(",", contentsIds.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ params @@", param);
			var dv = Get(XML_KEY_NAME, "GetContentsSummaryDataOfToday", input, replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new ContentsSummaryData(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="logNo">ログNo</param>
		/// <returns>コンテンツログ</returns>
		internal ContentsLogModel Get(int logNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_CONTENTSLOG_LOG_NO, logNo }
			};
			var dv = Get(XML_KEY_NAME, "Get", input);
			return (dv.Count > 0) ? new ContentsLogModel(dv[0]) : null;
		}
		#endregion

		#region ~GetByOrderId 注文IDから取得
		/// <summary>
		/// 注文ID取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>コンテンツログ</returns>
		internal ContentsLogModel[] GetByOrderId(string orderId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_CONTENTSLOG_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetByOrderId", input);
			var model = (dv.Count > 0) ? dv.Cast<DataRowView>().Select(drv => new ContentsLogModel(drv)).ToArray() : null;
			return model;
		}
		#endregion

		#region ~GetForAnalysisReport 分析レポート用取得
		/// <summary>
		/// 分析レポート用取得
		/// </summary>
		/// <param name="startDate">対象日始点</param>
		/// <param name="endDate">対象終点</param>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <returns>コンテンツログ</returns>
		internal ContentsSummaryAnalysisModel[] GetForAnalysisReport(DateTime startDate, DateTime endDate, string contentsType, params string[] contentsIds)
		{
			var input = new Hashtable
			{
				{ "start_date", startDate },
				{ "end_date", endDate },
				{ Constants.FIELD_CONTENTSLOG_CONTENTS_TYPE, contentsType },
				{ Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_ID, (contentsIds.Length > 0) ? contentsIds[0] : "" }
			};
			var param = string.Join(",", contentsIds.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ params @@", param);
			var dv = Get(XML_KEY_NAME, "GetForAnalysisReport", input, replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new ContentsSummaryAnalysisModel(drv)).ToArray();
		}
		#endregion

		#region ~GetForAnalysisReport 分析レポート用取得(当日)
		/// <summary>
		/// 分析レポート用取得(当日)
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <returns>コンテンツログ</returns>
		internal ContentsSummaryAnalysisModel[] GetForAnalysisReportOfToday(string contentsType, params string[] contentsIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_CONTENTSLOG_CONTENTS_TYPE, contentsType },
				{ Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_ID, (contentsIds.Length > 0) ? contentsIds[0] : "" }
			};
			var param = string.Join(",", contentsIds.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ params @@", param);
			var dv = Get(XML_KEY_NAME, "GetForAnalysisReportOfToday", input, replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new ContentsSummaryAnalysisModel(drv)).ToArray();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>新規ログ番号</returns>
		internal int Insert(ContentsLogModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);

			return result;
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(ContentsLogModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~DeleteByContentsIds コンテンツIDによるログ削除
		/// <summary>
		/// コンテンツIDによるログ削除
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteByContentsIds(string contentsType, params string[] contentsIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_TYPE, contentsType }
			};
			var param = string.Join(",", contentsIds.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ params @@", param);
			var result = Exec(XML_KEY_NAME, "DeleteByContentsIds", input, replaces: replace);
			return result;
		}
		#endregion

		#region ~DeleteContentsSummaryAnalysisByContentsIds コンテンツIDによるサマリー削除
		/// <summary>
		/// コンテンツIDによるサマリー削除
		/// </summary>
		/// <param name="contentsType">コンテンツタイプ</param>
		/// <param name="contentsIds">コンテンツIDリスト</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteContentsSummaryAnalysisByContentsIds(string contentsType, params string[] contentsIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_TYPE, contentsType }
			};
			var param = string.Join(",", contentsIds.Select(id => string.Format("'{0}'", id)));
			var replace = new KeyValuePair<string, string>("@@ params @@", param);
			var result = Exec(XML_KEY_NAME, "DeleteContentsSummaryAnalysisByContentsIds", input, replaces: replace);
			return result;
		}
		#endregion
	}
}
