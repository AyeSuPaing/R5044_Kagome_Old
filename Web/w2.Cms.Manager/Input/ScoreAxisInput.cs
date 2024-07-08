/*
=========================================================================================================
  Module      : Score Axis Input (ScoreAxisInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Input;
using w2.Common.Util;
using w2.Domain.ScoreAxis;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// Score axis input
	/// </summary>
	public class ScoreAxisInput : InputBase<ScoreAxisModel>
	{
		#region +Method
		/// <summary>
		/// Create model
		/// </summary>
		/// <returns>Score axis model</returns>
		public override ScoreAxisModel CreateModel()
		{
			var model = new ScoreAxisModel
			{
				ScoreAxisId = this.ScoreAxisId,
				ScoreAxisTitle = this.ScoreAxisTitle,
				AxisName1 = StringUtility.ToEmpty(this.AxisName1),
				AxisName2 = StringUtility.ToEmpty(this.AxisName2),
				AxisName3 = StringUtility.ToEmpty(this.AxisName3),
				AxisName4 = StringUtility.ToEmpty(this.AxisName4),
				AxisName5 = StringUtility.ToEmpty(this.AxisName5),
				AxisName6 = StringUtility.ToEmpty(this.AxisName6),
				AxisName7 = StringUtility.ToEmpty(this.AxisName7),
				AxisName8 = StringUtility.ToEmpty(this.AxisName8),
				AxisName9 = StringUtility.ToEmpty(this.AxisName9),
				AxisName10 = StringUtility.ToEmpty(this.AxisName10),
				AxisName11 = StringUtility.ToEmpty(this.AxisName11),
				AxisName12 = StringUtility.ToEmpty(this.AxisName12),
				AxisName13 = StringUtility.ToEmpty(this.AxisName13),
				AxisName14 = StringUtility.ToEmpty(this.AxisName14),
				AxisName15 = StringUtility.ToEmpty(this.AxisName15),
				LastChanged = this.LastChanged,
			};

			return model;
		}

		/// <summary>
		/// Validate
		/// </summary>
		/// <param name="isRegister">Is register</param>
		/// <returns>Error message list</returns>
		public List<string> Validate(bool isRegister)
		{
			var checkKbn = isRegister ? "ScoreAxisRegister" : "ScoreAxisModify";
			var errorMessageList = Validator.Validate(checkKbn, this.DataSource)
				.Select(item => item.Value)
				.ToList();

			return errorMessageList;
		}

		/// <summary>
		/// Create error join message
		/// </summary>
		/// <param name="errorMessageList">Error message list</param>
		/// <returns>Combined error messages</returns>
		public string CreateErrorJoinMessage(IEnumerable<string> errorMessageList)
		{
			var result = string.Join("<br />", errorMessageList);
			return result;
		}
		#endregion

		#region +Properties
		/// <summary>Score axis id</summary>
		public string ScoreAxisId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_SCORE_AXIS_ID]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_SCORE_AXIS_ID] = value; }
		}
		/// <summary>Score axis title</summary>
		public string ScoreAxisTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_SCORE_AXIS_TITLE]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_SCORE_AXIS_TITLE] = value; }
		}
		/// <summary>Axis name 1</summary>
		public string AxisName1
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME1]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME1] = value; }
		}
		/// <summary>Axis name 2</summary>
		public string AxisName2
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME2]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME2] = value; }
		}
		/// <summary>Axis name 3</summary>
		public string AxisName3
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME3]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME3] = value; }
		}
		/// <summary>Axis name 4</summary>
		public string AxisName4
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME4]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME4] = value; }
		}
		/// <summary>Axis name 5</summary>
		public string AxisName5
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME5]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME5] = value; }
		}
		/// <summary>Axis name 6</summary>
		public string AxisName6
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME6]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME6] = value; }
		}
		/// <summary>Axis name 7</summary>
		public string AxisName7
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME7]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME7] = value; }
		}
		/// <summary>Axis name 8</summary>
		public string AxisName8
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME8]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME8] = value; }
		}
		/// <summary>Axis name 9</summary>
		public string AxisName9
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME9]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME9] = value; }
		}
		/// <summary>Axis name 10</summary>
		public string AxisName10
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME10]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME10] = value; }
		}
		/// <summary>Axis name 11</summary>
		public string AxisName11
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME11]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME11] = value; }
		}
		/// <summary>Axis name 12</summary>
		public string AxisName12
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME12]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME12] = value; }
		}
		/// <summary>Axis name 13</summary>
		public string AxisName13
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME13]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME13] = value; }
		}
		/// <summary>Axis name 14</summary>
		public string AxisName14
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME14]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME14] = value; }
		}
		/// <summary>Axis name 15</summary>
		public string AxisName15
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME15]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME15] = value; }
		}
		/// <summary>Date created</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_DATE_CREATED] = value; }
		}
		/// <summary>Date changed</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_DATE_CHANGED] = value; }
		}
		/// <summary>Last changed</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_LAST_CHANGED] = value; }
		}
		/// <summary>Axis names</summary>
		public string[] AxisNames { get; set; }
		#endregion
	}
}
