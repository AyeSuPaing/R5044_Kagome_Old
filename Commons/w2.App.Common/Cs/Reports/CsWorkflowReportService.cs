/*
=========================================================================================================
  Module      : CS業務フローレポートサービス(CsWorkflowReportService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.App.Common.Cs.CsOperator;

namespace w2.App.Common.Cs.Reports
{
	/// <summary>
	/// CS業務フローレポートサービス
	/// </summary>
	public class CsWorkflowReportService
	{
		/// <summary>レポジトリ</summary>
		private CsWorkflowReportRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public CsWorkflowReportService(CsWorkflowReportRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +GetOperatorReport オペレータ毎レポート取得
		/// <summary>
		/// オペレータ毎レポート取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportMatrixRowModelForCsWorkflow[] GetOperatorReport(string deptId, Hashtable ht)
		{
			var operatorRequestReport = GetOperatorRequestReport(ht);
			var operatorRequestCancelldReport = GetOperatorRequestCancelledReport(ht);
			var operatorResultReport = GetOperatorRequestResultReport(ht);

			var operatorService = new CsOperatorService(new CsOperatorRepository());
			var operators = operatorService.GetAll(deptId);

			var list = new List<ReportMatrixRowModelForCsWorkflow>();
			list.AddRange(operators.Select(op => CreateOperatorRow(operatorRequestReport, operatorRequestCancelldReport, operatorResultReport, op)));
			list.Add(CreateOperatorRow(operatorRequestReport, operatorRequestCancelldReport, operatorResultReport, new CsOperatorModel()));
			return list.ToArray();
		}
		#endregion

		#region -GetOperatorRequestReport オペレータ別依頼レポート取得
		/// <summary>
		/// オペレータ別依頼レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		private OperatorMessageRequestCountModel[] GetOperatorRequestReport(Hashtable ht)
		{
			var dv = this.Repository.GetOperatorRequestReport(ht);
			var models = (from DataRowView drv in dv select new OperatorMessageRequestCountModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region -GetOperatorRequestCancelledReport オペレータ別依頼取り下げ済レポート取得
		/// <summary>
		/// オペレータ別依頼取り下げ済レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		private OperatorMessageRequestCountModel[] GetOperatorRequestCancelledReport(Hashtable ht)
		{
			var dv = this.Repository.GetOperatorRequestCancelledReport(ht);
			var models = (from DataRowView drv in dv select new OperatorMessageRequestCountModel(drv)).ToArray();
			return models;
		}
		#endregion		

		#region -GetOperatorRequestResultReport オペレータ別依頼結果レポート取得
		/// <summary>
		/// オペレータ別依頼結果レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		private OperatorMessageRequestResultCountModel[] GetOperatorRequestResultReport(Hashtable ht)
		{
			var dv = this.Repository.GetOperatorRequestResultReport(ht);
			var models = (from DataRowView drv in dv select new OperatorMessageRequestResultCountModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region -CreateOperatorRow オペレータ行レポート作成
		/// <summary>
		/// オペレータ行レポート作成
		/// </summary>
		/// <param name="operatorRequestReport">オペレータ毎リクエストレポート</param>
		/// <param name="operatorRequestCancelldReport">オペレータ毎リクエスト取り下げ済みレポート</param>
		/// <param name="operatorResultReport">オペレータ毎リクエスト結果レポート</param>
		/// <param name="csOperator">ターゲットCSオペレータ</param>
		/// <returns>オペレータ行レポート</returns>
		private ReportMatrixRowModelForCsWorkflow CreateOperatorRow(
			OperatorMessageRequestCountModel[] operatorRequestReport,
			OperatorMessageRequestCountModel[] operatorRequestCancelldReport,
			OperatorMessageRequestResultCountModel[] operatorResultReport,
			CsOperatorModel csOperator)
		{
			var operatorRow = new ReportMatrixRowModelForCsWorkflow();
			var requestRep = operatorRequestReport.Where(o => (o.OperatorId == csOperator.OperatorId)).ToArray();
			var requestCancelledRep = operatorRequestCancelldReport.Where(o => (o.OperatorId == csOperator.OperatorId)).ToArray();
			var resultRep = operatorResultReport.Where(o => (o.OperatorId == csOperator.OperatorId)).ToArray();

			// 名称・有効フラグ
			operatorRow.Name = csOperator.EX_ShopOperatorName;
			operatorRow.Valid = (csOperator.EX_ValidFlag == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID);
			// 承認依頼：依頼数
			operatorRow.ApprReqCount = requestRep.Where(r => (r.RequestType == Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_APPROVE)).Sum(r => r.Count);
			// 承認依頼：取り下げ数
			operatorRow.ApprReqCancelCount = requestCancelledRep.Where(r => (r.RequestType == Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_APPROVE)).Sum(r => r.Count);
			// 承認対応：未対応、OK、NG
			var apprResultRep = resultRep.Where(r => (r.RequestType == Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_APPROVE)).ToArray();
			operatorRow.ApprOkCount = apprResultRep.Where(r => r.ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_OK).Sum(r => r.Count);
			operatorRow.ApprNgCount = apprResultRep.Where(r => r.ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NG).Sum(r => r.Count);
			// 送信依頼：依頼数
			operatorRow.SendReqCount = requestRep.Where(r => (r.RequestType == Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_MAILSEND)).Sum(r => r.Count);
			// 送信依頼：取り下げ数
			operatorRow.SendReqCancelCount = requestCancelledRep.Where(r => (r.RequestType == Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_MAILSEND)).Sum(r => r.Count);
			// 送信対応：未対応、OK、NG
			var sendResultRep = resultRep.Where(r => (r.RequestType == Constants.FLG_CSMESSAGEREQUEST_REQUEST_TYPE_MAILSEND)).ToArray();
			operatorRow.SendOkCount = sendResultRep.Where(r => r.ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_OK).Sum(r => r.Count);
			operatorRow.SendNgCount = sendResultRep.Where(r => r.ResultStatus == Constants.FLG_CSMESSAGEREQUESTITEM_RESULT_STATUS_NG).Sum(r => r.Count);

			return operatorRow;
		}
		#endregion
	}
}
