/*
=========================================================================================================
  Module      : ユーザー拡張項目出力クラス(ActionExportUserExtend.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.User;
using w2.Domain.UserExtendSetting;

namespace w2.Commerce.Batch.CustomerRingsExport.Action
{
	/// <summary>
	/// 出力クラス
	/// </summary>
	public class ActionExportUserExtend : ActionExportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ActionExportUserExtend(FileDefines.FileDefine define)
			: base(define)
		{
			this.ExportFieldString = "";

			var list = new UserService().GetUserExtendSettingList();
			foreach (UserExtendSettingModel model in list)
			{
				this.ExportFieldString += "," + model.SettingId;
			}
		}
	}
}
