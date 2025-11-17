SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [DO_Master](
	[Cust_Code] [char](15) NULL,
	[DO_No] [char](25) NOT NULL,
	[DO_Date] [datetime] NULL,
	[Amount] [numeric](22, 5) NULL,
	[Remarks] [char](35) NULL,
	[Reissue_Cls] [char](1) NULL,
	[Revised_Cls] [char](1) NULL,
	[Fix_Cls] [char](1) NULL,
	[List_PO] [char](750) NULL,
	[WHCode] [char](15) NULL,
	[Forwarder_Code] [char](15) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Remarks_Cls] [varchar](2) NULL,
	[BC_Type] [varchar](15) NULL,
	[BC40_No] [varchar](30) NULL,
	[BC40_Date] [datetime] NULL,
	[No_Register] [varchar](30) NULL,
	[Delivery_Cls] [varchar](2) NULL,
 CONSTRAINT [PK_DO_Master] PRIMARY KEY CLUSTERED 
(
	[DO_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [DO_Master] ADD  CONSTRAINT [DF_DO_Master_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [DO_Master]  WITH CHECK ADD  CONSTRAINT [FK_DO_Master_Trade_Master] FOREIGN KEY([Cust_Code])
REFERENCES [Trade_Master] ([Trade_Code])
GO
ALTER TABLE [DO_Master] CHECK CONSTRAINT [FK_DO_Master_Trade_Master]
GO
