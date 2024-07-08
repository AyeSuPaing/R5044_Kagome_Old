if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_RecommendHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_RecommendHistory]
GO
/*
=========================================================================================================
  Module      : ГМГRГБГУГhХ\ОжЧЪЧЁ (w2_RecommendHistory.sql)
 еееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееееее
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_RecommendHistory] (
	[shop_id] [nvarchar] (10) NOT NULL DEFAULT (N''),
	[recommend_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[user_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[recommend_history_no] [int] NOT NULL DEFAULT(1),
	[target_order_id] [nvarchar] (30) NOT NULL DEFAULT (N''),
	[display_kbn] [nvarchar] (10) NOT NULL DEFAULT (N'FRONT'),
	[ordered_flg] [nvarchar] (10) NOT NULL DEFAULT (N'DISP'),
	[date_created] [datetime] NOT NULL DEFAULT (GETDATE()),
	[date_changed] [datetime] NOT NULL DEFAULT (GETDATE()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_RecommendHistory] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_RecommendHistory] PRIMARY KEY  CLUSTERED
	(
		[shop_id],
		[recommend_id],
		[user_id],
		[recommend_history_no]
	) ON [PRIMARY]
GO
