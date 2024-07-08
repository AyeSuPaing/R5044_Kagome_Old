/*
=========================================================================================================
  Module      : タイムスパンオブジェクト(AbsoluteTimeSpan.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Util
{
	/// <summary>
	/// 何時何分何秒～何時何分何秒まで、というような厳密な期間を表現するクラス
	/// </summary>
	public class AbsoluteTimeSpan : IEquatable<AbsoluteTimeSpan>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="beginTime">開始日時</param>
		/// <param name="endTime">終了日時</param>
		public AbsoluteTimeSpan(DateTime beginTime, DateTime endTime)
		{
			this.BeginTime = beginTime;
			this.EndTime = endTime;

			Validate();
		}
		#endregion

		#region +Validate 開始時間と終了時間の関係性チェック
		/// <summary>
		/// 開始時間と終了時間のチェック
		/// </summary>
		public virtual void Validate()
		{
			if (this.BeginTime > this.EndTime) throw  new ArgumentException("開始時間は終了時間より過去である必要があります。");
		}
		#endregion

		#region +Equals 同値チェック

		/// <summary>
		/// 同値チェック
		/// </summary>
		/// <param name="other">チェック対象</param>
		/// <returns>true/false</returns>
		public bool Equals(AbsoluteTimeSpan other)
		{
			if (other == null) return false;

			return ((this.BeginTime == other.BeginTime)
					&& (this.EndTime == other.EndTime));
		}
		/// <summary>
		/// 同値チェック
		/// </summary>
		/// <param name="obj">チェック対象</param>
		/// <returns>true/false</returns>
		public override bool Equals(object obj)
		{
			if ((obj == null) || (this.GetType() != obj.GetType())) return false;

			return this.Equals((AbsoluteTimeSpan)obj);
		}
		#endregion

		#region +GetHashCode ハッシュコード
		/// <summary>
		/// このインスタンスのハッシュコードを返します。
		/// </summary>
		/// <returns>ハッシュコード</returns>
		public override int GetHashCode()
		{
			// INFO: xor演算を利用する
			//       http://msdn.microsoft.com/ja-jp/library/ms182358%28v=vs.100%29.aspx
			return this.BeginTime.GetHashCode() ^ this.EndTime.GetHashCode();
		}
		#endregion

		#region プロパティ

		/// <summary>開始時間</summary>
		public DateTime BeginTime { get; private set; }
		/// <summary>終了時間</summary>
		public DateTime EndTime { get; private set; }

		#endregion
	}

	/// <summary>
	/// 過去における厳密期間
	/// </summary>
	public class PastAbsoluteTimeSpan : AbsoluteTimeSpan
	{
		
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ（開始時間、終了時間の指定あり）
		/// </summary>
		/// <param name="maxEndTimeSpan">終了時間加算分</param>
		/// <param name="beginTime">開始日時</param>
		/// <param name="endTime">終了日時</param>
		public PastAbsoluteTimeSpan(int maxEndTimeSpan, DateTime beginTime, DateTime endTime) : base(beginTime, endTime)
		{
			// 終了時間最大値にシステム日時に終了時間加算分を加算して終了時間を設定
			this.MaxEndTime = DateTime.Now.AddMinutes(maxEndTimeSpan);
		}

		/// <summary>
		/// コンストラクタ（開始時間のみ指定あり）
		/// </summary>
		/// <param name="maxEndTimeSpan">終了時間加算分</param>
		/// <param name="beginTime">開始日時</param>
		public PastAbsoluteTimeSpan(int maxEndTimeSpan, DateTime beginTime) : base(beginTime, DateTime.Now.AddMinutes(maxEndTimeSpan))
		{
			// 終了時間最大値に終了時間を設定
			this.MaxEndTime = this.EndTime;
		}
		#endregion

		#region +ValidateMaxEndTime 終了時間最大値のチェック
		/// <summary>
		/// 終了時間最大値のチェック
		/// </summary>
		public void ValidateMaxEndTime()
		{
			base.Validate();
			if (this.MaxEndTime > DateTime.Now) throw new ArgumentException("終了時間最大値は現在より過去である必要があります。");
			if (this.MaxEndTime < this.EndTime) throw new ArgumentException("終了時間は終了時間最大値より過去である必要があります。");
		}
		#endregion

		#region +ToString 文字列にする
		/// <summary>
		/// 文字列型にする
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("begin:{0}, end:{1}", BeginTime, EndTime);
		}
		#endregion

		#region プロパティ
		/// <summary>終了時間最大値</summary>
		public DateTime MaxEndTime
		{
			get { return m_maxEndTime; }
			set
			{
				m_maxEndTime = value;

				// 設定したらチェック
				ValidateMaxEndTime();
			}
		}
		private DateTime m_maxEndTime;

		#endregion
	}
}
