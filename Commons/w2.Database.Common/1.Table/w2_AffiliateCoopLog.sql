if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_AffiliateCoopLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_AffiliateCoopLog]
GO
/*
=========================================================================================================
  Module      : アフィリエイト連携ログ(w2_AffiliateCoopLog.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_AffiliateCoopLog] (
	[log_no] [int] IDENTITY(1,1) NOT NULL,
	[affiliate_kbn] [nvarchar] (50) NOT NULL,
	[master_id] [nvarchar] (50) NOT NULL,
	[coop_status] [nvarchar] (10) NOT NULL DEFAULT ('WAIT'),
	[coop_data1] [nvarchar] (max) NOT NULL DEFAULT (''),
	[coop_data2] [nvarchar] (100) NOT NULL DEFAULT (''),
	[coop_data3] [nvarchar] (100) NOT NULL DEFAULT (''),
	[coop_data4] [nvarchar] (100) NOT NULL DEFAULT (''),
	[coop_data5] [nvarchar] (100) NOT NULL DEFAULT (''),
	[coop_data6] [nvarchar] (100) NOT NULL DEFAULT (''),
	[coop_data7] [nvarchar] (100) NOT NULL DEFAULT (''),
	[coop_data8] [nvarchar] (100) NOT NULL DEFAULT (''),
	[coop_data9] [nvarchar] (100) NOT NULL DEFAULT (''),
	[coop_data10] [nvarchar] (100) NOT NULL DEFAULT (''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_AffiliateCoopLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_AffiliateCoopLog] PRIMARY KEY  CLUSTERED
	(
		[log_no]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_AffiliateCoopLog_1] ON [dbo].[w2_AffiliateCoopLog]([affiliate_kbn], [master_id]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_AffiliateCoopLog_2] ON [dbo].[w2_AffiliateCoopLog]([affiliate_kbn], [coop_status]) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_AffiliateCoopLog_3] ON [dbo].[w2_AffiliateCoopLog]([date_created]) ON [PRIMARY]
GO
