/*
=========================================================================================================
  Module      : Scoring Sale (w2_ScoringSale.sql)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[w2_ScoringSale]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [dbo].[w2_ScoringSale]
GO

CREATE TABLE [dbo].[w2_ScoringSale] (
	[scoring_sale_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[scoring_sale_title] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[publish_status] [nvarchar] (20) NOT NULL DEFAULT (N'PUBLISHED'),
	[public_start_datetime] [datetime],
	[public_end_datetime] [datetime],
	[score_axis_id] [nvarchar] (20) NOT NULL DEFAULT (N''),
	[theme_color] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[top_page_use_flg] [nvarchar] (5) NOT NULL DEFAULT (N'ON'),
	[top_page_title] [nvarchar] (250),
	[top_page_sub_title] [nvarchar] (250),
	[top_page_body] [nvarchar] (MAX),
	[top_page_img_path] [nvarchar] (360),
	[top_page_btn_caption] [nvarchar] (40),
	[result_page_title] [nvarchar] (250) NOT NULL DEFAULT (N''),
	[result_page_body_above] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[result_page_body_below] [nvarchar] (MAX) NOT NULL DEFAULT (N''),
	[radar_chart_use_flg] [nvarchar] (5) NOT NULL DEFAULT (N'ON'),
	[radar_chart_title] [nvarchar] (50) NOT NULL DEFAULT (N''),
	[result_page_btn_caption] [nvarchar] (40) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_ScoringSale] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_ScoringSale] PRIMARY KEY CLUSTERED
	(
		[scoring_sale_id]
	) ON [PRIMARY]
GO
