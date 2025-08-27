SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Masterlist](
	[Year] [varchar](4) NULL,
	[Month] [varchar](2) NULL,
	[DP_Kawai] [numeric](30, 11) NULL,
	[DP_KE] [numeric](30, 11) NULL,
	[DP_China] [numeric](30, 11) NULL,
	[DP_Import] [numeric](30, 11) NULL,
	[DP_HI] [numeric](30, 11) NULL,
	[DP_MPFI] [numeric](30, 11) NULL,
	[DP_KAI] [numeric](30, 11) NULL,
	[DP_Local] [numeric](30, 11) NULL,
	[KU_Kawai] [numeric](30, 11) NULL,
	[KU_China] [numeric](30, 11) NULL,
	[KU_Import] [numeric](30, 11) NULL,
	[KU_HI] [numeric](30, 11) NULL,
	[KU_Local] [numeric](30, 11) NULL,
	[KU_KAI] [numeric](30, 11) NULL,
	[Parent_itemcode] [char](15) NULL,
	[Description] [char](75) NULL,
	[Amount] [numeric](30, 11) NULL
) ON [PRIMARY]
GO
