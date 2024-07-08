if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LandingPageDesignElement]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_LandingPageDesignElement]
GO
/*
=========================================================================================================
  Module      : Lpページ要素デザイン (w2_LandingPageDesignElement.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_LandingPageDesignElement] (
	[page_id] [nvarchar] (32) NOT NULL DEFAULT (N''),
	[design_type] [nvarchar] (20) NOT NULL DEFAULT (N'PC'),
	[block_index] [int] NOT NULL DEFAULT (0),
	[element_index] [int] NOT NULL DEFAULT (0),
	[element_place_holder_name] [nvarchar] (256) NOT NULL DEFAULT (N''),
	[date_created] [datetime] NOT NULL DEFAULT (getdate()),
	[date_changed] [datetime] NOT NULL DEFAULT (getdate()),
	[last_changed] [nvarchar] (20) NOT NULL DEFAULT (N'')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_LandingPageDesignElement] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_LandingPageDesignElement] PRIMARY KEY  CLUSTERED
	(
		[page_id],
		[design_type],
		[block_index],
		[element_index]
	) ON [PRIMARY]
GO
