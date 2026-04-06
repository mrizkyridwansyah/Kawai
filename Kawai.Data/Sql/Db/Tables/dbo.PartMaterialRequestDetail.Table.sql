SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartMaterialRequestDetail](
	[RequestDetailID] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestDetailNo] [varchar](50) NOT NULL,
	[RequestID] [bigint] NOT NULL,
	[WorkStationCode] [char](15) NOT NULL,
	[AreaCode] [varchar](25) NOT NULL,
	[SEQ] [int] NOT NULL,
	[Trolley_No] [varchar](25) NULL,
	[RefNumber] [varchar](25) NULL,
	[RequestStatusID] [int] NOT NULL,
	[Remarks] [varchar](255) NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [PartMaterialRequestDetail]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [PartMaterialRequestHeader] ([RequestID])
GO
ALTER TABLE [PartMaterialRequestDetail]  WITH CHECK ADD FOREIGN KEY([RequestStatusID])
REFERENCES [RequestStatusCls] ([RequestStatusID])
GO
ALTER TABLE [PartMaterialRequestDetail]  WITH CHECK ADD FOREIGN KEY([WorkStationCode])
REFERENCES [MS_WorkStation] ([WorkStationCode])
GO
