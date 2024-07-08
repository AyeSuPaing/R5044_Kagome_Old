/*
=========================================================================================================
  Module      : 組合せ抽出クラス(Combination.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Util
{
	/// <summary>
	/// 組み合わせクラス
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class Combination<T>
	{
		#region コンストラクタ
		public Combination(IList<T> items) : this(items, items.Count) { }
		public Combination(IList<T> items, int select)
		{
			if (select < 0) throw new ArgumentException("抽出要素数は、1以上を指定してください");
			if (items.Count < select) throw new ArgumentException("抽出要素数は、対象要素数以下を指定してください");

			this.Items = items;
			this.Length = select;

			if (select == 0) return;

			this.Indices = Enumerable.Range(0, this.Length).ToArray();
			this.FinalIndices = GetFinalIndices(select);
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 選択要素数に応じた組み合わせとなる最後の値を取得(終端の確定)
		/// </summary>
		/// <param name="select">抽出数</param>
		/// <returns></returns>
		private int[] GetFinalIndices(int select)
		{
			var finalIndices = new int[select];

			int j = select - 1;
			for (var i = this.Items.Count - 1; i > this.Items.Count - 1 - select; i--)
			{
				finalIndices[j] = i;
				j--;
			}
			return finalIndices;
		}

		/// <summary>
		/// 条件をもとに組み合わせを算出
		/// </summary>
		/// <returns>抽出数に対しての全組合せ</returns>
		public List<IList<T>> ComputeCombination()
		{
			List<IList<T>> combinations = new List<IList<T>>();
			var indices = Enumerable.Range(0, this.Length).ToArray();
			combinations.Add(GetCurrentCombination());

			while (NextCombination())
			{
				combinations.Add(GetCurrentCombination());
			}
			return combinations;
		}

		/// <summary>
		/// 現在の組み合わせを取得
		/// </summary>
		/// <returns>組合せ</returns>
		public T[] GetCurrentCombination()
		{
			T[] combination = new T[this.Length];
			for (var k = 0; k < this.Length; k++)
			{
				combination[k] = this.Items[this.Indices[k]];
			}
			return combination;
		}

		/// <summary>
		/// 次の組み合わせへ
		/// </summary>
		/// <returns>次の組合せが存在するか</returns>
		public bool NextCombination()
		{
			for (var j = this.FinalIndices.Length - 1; j > -1; j--)
			{
				if (this.Indices[j] < this.FinalIndices[j])
				{
					this.Indices[j]++;
					for (var k = 1; j + k < this.FinalIndices.Length; k++)
					{
						this.Indices[j + k] = this.Indices[j] + k;
					}
					break;
				}
				if (j == 0) return false;
			}
			return true;
		}

		#endregion

		#region プロパティ
		/// <summary>要素</summary>
		private IList<T> Items { get; set; }
		/// <summary>抽出数</summary>
		private int Length { get; set; }
		/// <summary>探索位置インデクス配列</summary>
		private int[] Indices { get; set; }
		/// <summary>終端インデクス</summary>
		private int[] FinalIndices { get; set; }
		#endregion
	}
}
