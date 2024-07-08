/*
=========================================================================================================
  Module      : Dbマップ名属性クラス (DbMapName.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.Helper.Attribute
{
	/// <summary>
	/// Dbマップ名属性クラス
	/// </summary>
	public class DbMapName : System.Attribute
	{
		/// <summary>マップ名 </summary>
		public string MapName { private set; get; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mapName">マップ名</param>
		public DbMapName(string mapName)
		{
			this.MapName = mapName;
		}
	}
}

