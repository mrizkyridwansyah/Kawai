SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartMaterialRequestDetailPallet](
	[RequestDetailID] [bigint] NULL,
	[RefNo] [varchar](100) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](100) NULL,
	[Stop_Point] [varchar](100) NULL
) ON [PRIMARY]
GO
