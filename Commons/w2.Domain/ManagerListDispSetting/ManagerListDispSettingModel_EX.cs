/*
=========================================================================================================
  Module      : 表示設定管理モデル (ManagerListDispSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Domain.ManagerListDispSetting
{
	/// <summary>
	/// 表示設定管理モデル
	/// </summary>
	public partial class ManagerListDispSettingModel
	{
		#region プロパティ
		/// <summary>表示フラグ</summary>
		public bool CanDisplay
		{
			get { return this.DispFlag == Constants.FLG_MANAGERLISTDISPSETTING_DISP_FLAG_ON; }
		}
		/// <summary>固定カラムの時False</summary>
		public bool IsNotFixedColmun
		{
			get { return ((this.DispColmunName == Constants.FIELD_ORDER_ORDER_ID) == false); }
		}
		#endregion
	}
}