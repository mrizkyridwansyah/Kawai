SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ProdMaterialReqDetail](
	[ParamKey] [varchar](100) NOT NULL,
	[ChildItemCode] [varchar](25) NOT NULL,
	[ProductionId] [bigint] NOT NULL,
	[Process] [varchar](25) NOT NULL,
	[Line] [varchar](25) NOT NULL,
	[ScheduleDate] [date] NOT NULL,
	[ParentItemCode] [varchar](25) NOT NULL,
	[ReqQty] [numeric](18, 9) NOT NULL,
	[ScanQty] [numeric](18, 9) NOT NULL,
	[FinalReqQty] [numeric](18, 9) NOT NULL
) ON [PRIMARY]
GO
