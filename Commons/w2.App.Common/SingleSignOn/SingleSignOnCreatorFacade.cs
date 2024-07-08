/*
=========================================================================================================
  Module      : シングルサインオンインスタンス作成Facade(SingleSignOnCreatorFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web;
using w2.App.Common.SingleSignOn.Executer;

namespace w2.App.Common.SingleSignOn
{
	/// <summary>
	/// シングルサインオンインスタンス作成Facade
	/// </summary>
	public class SingleSignOnCreatorFacade
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="context">HTTPコンテンツ</param>
		public SingleSignOnCreatorFacade(
			HttpContext context)
		{
			this.Context = context;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// シングルサインオンインスタンス作成
		/// </summary>
		/// <returns>シングルサインオンインスタンスインスタンス</returns>
		public ISingleSignOnExecuter CreateSingleSignOn()
		{
			switch (Constants.PROJECT_NO)
			{
				case "G3001_Funnel":
					return new G3001_Funnel_SingleSignOnExecuter(this.Context);
			
				case "P0078_Mackenyu":
					return new P0078_Mackenyu_SingleSignOnExecuter(this.Context);
			
				case "P0076_Roland":
				case "P0072_tAce":
					return new P0076_Roland_SingleSignOnExecuter(this.Context);
			
				case "R1031_Pioneer":
					return new R1031_Pioneer_SingleSignOnExecuter(this.Context);

				default:
					return new DoNothingSingleSignOnExecuter(this.Context);
			}
		}
		#endregion

		#region プロパティ
		/// <summary>HTTPコンテキスト</summary>
		public HttpContext Context { get; private set; }
		#endregion
	}
}