/*
=========================================================================================================
  Module      : メニュー権限管理マスタモデル (MenuAuthorityModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.MenuAuthority
{
	/// <summary>
	/// メニュー権限管理マスタモデル
	/// </summary>
	public partial class MenuAuthorityModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>メニューカウント</summary>
		public int MenuCounts
		{
			get { return (int)this.DataSource["menu_counts"]; }
			set { this.DataSource["menu_counts"] = value; }
		}
		/// <summary>デフォルト表示か</summary>
		public bool IsDefaultDispOn
		{
			get { return (this.DefaultDispFlg == Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_ON); }
			set
			{
				this.DefaultDispFlg =
					value
						? Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_ON
						: Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_OFF;
			}
		}
		#endregion
	}
}
