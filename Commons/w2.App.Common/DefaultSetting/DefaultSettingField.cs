/*
=========================================================================================================
  Module      : Default Setting Field (DefaultSettingField.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.DefaultSetting
{
	/// <summary>
	/// Default setting field
	/// </summary>
	[Serializable]
	public class DefaultSettingField
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="fieldName">Field name</param>
		/// <param name="fieldDefault">Field default</param>
		/// <param name="fieldComment">Field comment</param>
		/// <param name="fieldDisplay">Field display</param>
		public DefaultSettingField(
			string fieldName,
			string fieldDefault,
			string fieldComment,
			bool fieldDisplay)
		{
			this.Name = fieldName;
			this.Default = fieldDefault;
			this.Comment = fieldComment;
			this.Display = fieldDisplay;
		}
		#endregion

		#region +Properties
		/// <summary>Name</summary>
		public string Name { get; set; }
		/// <summary>Default</summary>
		public string Default { get; set; }
		/// <summary>Comment</summary>
		public string Comment { get; set; }
		/// <summary>Display</summary>
		public bool Display { get; set; }
		#endregion
	}
}
