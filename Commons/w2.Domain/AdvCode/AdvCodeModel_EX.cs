/*
=========================================================================================================
  Module      : 広告コードマスタモデル (AdvCodeModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Util;

namespace w2.Domain.AdvCode
{
	/// <summary>
	/// 広告コードマスタモデル
	/// </summary>
	public partial class AdvCodeModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>媒体掲載期間</summary>
		public string PublicationDateString
		{
			get
			{
				if ((this.PublicationDateFrom != null) || (this.PublicationDateTo != null))
				{
					return StringUtility.ToDateString(this.PublicationDateFrom, "yyyy/MM/dd") + "～" + StringUtility.ToDateString(this.PublicationDateTo, "yyyy/MM/dd");
				}

				return string.Empty;
			}
		}

		/// <summary>有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_ADVCODE_VALID_FLG_VALID); }
		}
		#endregion
	}
}
