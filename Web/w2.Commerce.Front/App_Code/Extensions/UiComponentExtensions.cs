/*
=========================================================================================================
  Module      : UIコンポーネント拡張クラス (UiComponentExtensions.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Web.WebCustomControl;

namespace Extensions
{
	/// <summary>
	/// UIコンポーネント拡張クラス
	/// </summary>
	public static class UiComponentExtensions
	{
		/// <summary>再帰呼び出し上限値</summary>
		/// <remarks>スタックオーバーフロー対策</remarks>
		private const int MAX_RECURSIVE_CALLS = 100;

		/// <summary>
		/// コントロール取得、キャスト
		/// </summary>
		/// <typeparam name="T">キャストする型</typeparam>
		/// <param name="control">探索対象コントロール</param>
		/// <param name="controlId">コントロールID</param>
		/// <returns>UIコントロール</returns>
		public static T FindControlAs<T>(this Control control, string controlId)
			where T : Control
		{
			var found = control.FindControl(controlId);
			return (T)found;
		}

		/// <summary>
		/// コントロール取得（再帰）
		/// </summary>
		/// <param name="page">Page</param>
		/// <param name="controlId">コントロールID</param>
		/// <param name="onlyVisible">Visibleなもののみを対象とする</param>
		/// <returns>コントロール</returns>
		public static Control[] FindControlsRecursive(this Page page, string controlId, bool onlyVisible)
		{
			var result = Enumerable.Empty<Control>()
				.Concat(page.Controls.FindControls(controlId, onlyVisible))
				.Concat(
					page.Controls
						.Cast<Control>()
						.Where(c => c.HasControls())
						.SelectMany(c => c.FindControlsRecursive(controlId, onlyVisible)))
				.ToArray();

			return result;
		}

		/// <summary>
		/// コントロール取得（再帰）
		/// </summary>
		/// <param name="control">Control</param>
		/// <param name="controlId">コントロールID</param>
		/// <param name="onlyVisible">Visibleなもののみを対象とする</param>
		/// <param name="recursionRemains">再帰残り回数</param>
		/// <returns>コントロール</returns>
		private static Control[] FindControlsRecursive(
			this Control control,
			string controlId,
			bool onlyVisible,
			int recursionRemains = MAX_RECURSIVE_CALLS)
		{
			if (recursionRemains > 0)
			{
				var repeater = control as Repeater;
				if (repeater != null)
				{
					var result = repeater.Items
						.Cast<RepeaterItem>()
						.SelectMany(
							ri => Enumerable.Empty<Control>()
								.Concat(ri.Controls.FindControls(controlId, onlyVisible))
								.Concat(ri.FindControlsRecursive(controlId, onlyVisible, recursionRemains - 1)))
						.ToArray();
					return result;
				}

				if (control.HasControls())
				{
					var result = Enumerable.Empty<Control>()
						.Concat(control.Controls.FindControls(controlId, onlyVisible))
						.Concat(
							control.Controls
								.Cast<Control>()
								.Where(c => c.HasControls())
								.SelectMany(
									c => c.FindControlsRecursive(controlId, onlyVisible, recursionRemains - 1)))
						.ToArray();
					return result;
				}
			}

			return Enumerable.Empty<Control>().ToArray();
		}

		/// <summary>
		/// 内部コントロール取得
		/// </summary>
		/// <remarks>
		/// Control#FindControlで意図しないコントロールが取得されるケースがあったため独自実装
		/// </remarks>
		/// <param name="controlCollection">ControlCollection</param>
		/// <param name="controlId">コントロールID</param>
		/// <param name="onlyVisible">Visibleなもののみを対象とする</param>
		/// <returns>コントロール</returns>
		private static Control[] FindControls(this ControlCollection controlCollection, string controlId, bool onlyVisible)
		{
			var result = controlCollection
				.Cast<Control>()
				.Where(c => (c.ID == controlId) && (c.Visible || onlyVisible == false))
				.ToArray();
			return result;
		}

		/// <summary>
		/// すべてにフォーカス
		/// </summary>
		/// <param name="controls">フォーカスするコントロール</param>
		public static void FocusAll(this IEnumerable<Control> controls)
		{
			foreach (var control in controls)
			{
				if (control is RadioButtonGroup)
				{
					var rbg = (RadioButtonGroup)control;
					if (rbg.Checked) rbg.Focus();
				}
				else
				{
					control.Focus();
				}
			}
		}
	}
}