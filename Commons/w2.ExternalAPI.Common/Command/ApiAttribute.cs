/*
=========================================================================================================
  Module      : APIコマンド属性(ApiAttribute.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
namespace w2.ExternalAPI.Common.Command
{
	/// <summary>
	///	APIコマンド用の属性
	/// </summary>
	/// <remarks>
	/// 属性値として名前を持つ
	/// </remarks>
	public class ApiAttribute : System.Attribute
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">属性名</param>
		public ApiAttribute(string name)
		{
			m_name = name;
		}
		#endregion

		#region 属性名プロパティ
		/// <summary>
		/// 属性名プロパティ
		/// </summary>
		public string Name
		{
			get { return m_name; }
		}
		/// <summary>属性名プロパティの内部変数</summary>
		private readonly string m_name;
		#endregion
	}
}
