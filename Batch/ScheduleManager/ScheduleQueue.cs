/*
=========================================================================================================
  Module      : スケジュールキュークラス(ScheduleQueue.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.MarketingPlanner.Win.ScheduleManager
{
	class ScheduleQueue
	{
		private string m_strType = null;
		private string m_strKeyId = null;
		private DateTime m_dtSchedule = new DateTime(0);

		//=========================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strType"></param>
		/// <param name="strKeyId"></param>
		/// <param name="dtSchedule"></param>
		//=========================================================================================
		public ScheduleQueue(string strType, string strKeyId, DateTime dtSchedule)
		{
			m_strType = strType;
			m_strKeyId = strKeyId;
			m_dtSchedule = dtSchedule;
		}

		public string Type
		{
			get { return m_strType; }
		}

		public string KeyId
		{
			get { return m_strKeyId; }
		}

		public DateTime Schedule
		{
			get { return m_dtSchedule; }
		}

	}
}
