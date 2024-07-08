/*
=========================================================================================================
  Module      : 管理者向け警告メール送信クラス(AdminWarningMailSender.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Cs.Incident;
using w2.App.Common.Cs.CsOperator;

namespace w2.CustomerSupport.Batch.CsWarningMailSender.MailSendModules
{
	/// <summary>
	/// 管理者向け警告メール送信クラス
	/// </summary>
	public class AdminWarningMailSender : BaseMailSender
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AdminWarningMailSender()
		{
		}
		#endregion

		#region +SendToTarget ターゲットへ送信
		/// <summary>
		/// ターゲットへ送信
		/// </summary>
		public void SendToTarget()
		{
			int targetCount = 0;

			var incidentService = new CsIncidentService(new CsIncidentRepository());
			foreach (var incident in incidentService.CountWarning(Constants.CONST_DEFAULT_DEPT_ID, Constants.WARNING_NO_ASSIGN_LIMIT_DAYS).First)
			{
				// 担当オペレータが空の件数を加算
				if (incident.OperatorId == "")
				{
					targetCount += incident.EX_SearchCount;
					continue;
				}

				// 担当オペレータが無効な件数を加算
				var operatorService = new CsOperatorService(new CsOperatorRepository());
				var ope = operatorService.Get(Constants.CONST_DEFAULT_DEPT_ID, incident.OperatorId);
				if ((ope == null) || (ope.EX_ValidFlag == Constants.FLG_SHOPOPERATOR_VALID_FLG_INVALID)) targetCount += incident.EX_SearchCount;
			}

			// 送信
			if (targetCount > 0) Send(targetCount);
		}
		#endregion

		#region -Send 送信
		/// <summary>
		///	送信
		/// </summary>
		/// <param name="unassignedCount">未振り分け件数</param>
		private void Send(int unassignedCount)
		{
			var operatorService = new CsOperatorService(new CsOperatorRepository());
			var admins = (from ope in operatorService.GetValidAll(Constants.CONST_DEFAULT_DEPT_ID) where ope.EX_ReceiveNoAssignWarningFlg select ope).ToArray();

			admins.Where(ope => ope.EX_NotifyWarnFlg).ToList().ForEach(ope => SetBodyAndSend(Constants.CONST_MAIL_ID_CS_WARNING_ADMIN, ope, unassignedCount, Constants.WARNING_NO_ASSIGN_LIMIT_DAYS));
		}
		#endregion
	}
}
