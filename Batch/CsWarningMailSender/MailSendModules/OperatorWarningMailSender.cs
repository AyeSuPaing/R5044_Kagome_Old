/*
=========================================================================================================
  Module      : オペレータ向け警告メール送信クラス(OperatorWarningMailSender.cs)
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
	/// オペレータ向け警告メール送信クラス
	/// </summary>
	public class OperatorWarningMailSender : BaseMailSender
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OperatorWarningMailSender()
		{
		}
		#endregion

		#region +SendToTargetForOperator オペレータ(本人)向けメール送信
		/// <summary>
		/// オペレータ(本人)向けメール送信
		/// </summary>
		public void SendToTargetForOperator()
		{
			var incidentService = new CsIncidentService(new CsIncidentRepository());
			foreach (var incident in incidentService.CountWarning(Constants.CONST_DEFAULT_DEPT_ID, Constants.WARNING_NO_ACTION_LIMIT_DAYS).First)
			{
				if (incident.OperatorId == "") continue;

				var operatorService = new CsOperatorService(new CsOperatorRepository());
				var ope = operatorService.Get(Constants.CONST_DEFAULT_DEPT_ID, incident.OperatorId);
				if ((ope != null) && ope.EX_NotifyWarnFlg && (ope.EX_ValidFlag == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID)) SetBodyAndSend(Constants.CONST_MAIL_ID_CS_WARNING_OPERATOR, ope, incident.EX_SearchCount, Constants.WARNING_NO_ACTION_LIMIT_DAYS);
			}
		}
		#endregion

		#region +SendToTargetForGroup オペレータ(グループ)向けメール送信
		/// <summary>
		/// オペレータ(グループ)向けメール送信
		/// </summary>
		public void SendToTargetForGroup()
		{
			var incidentService = new CsIncidentService(new CsIncidentRepository());
			foreach (var incident in incidentService.CountWarning(Constants.CONST_DEFAULT_DEPT_ID, Constants.WARNING_NO_ACTION_LIMIT_DAYS).Second)
			{
				var operatorService = new CsOperatorService(new CsOperatorRepository());
				var ope = operatorService.Get(Constants.CONST_DEFAULT_DEPT_ID, incident.OperatorId);
				if ((ope != null) && ope.EX_NotifyWarnFlg && (ope.EX_ValidFlag == Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID)) SetBodyAndSend(Constants.CONST_MAIL_ID_CS_WARNING_GROUP, ope, incident.EX_SearchCount, Constants.WARNING_NO_ACTION_LIMIT_DAYS);
			}
		}
		#endregion
	}
}
