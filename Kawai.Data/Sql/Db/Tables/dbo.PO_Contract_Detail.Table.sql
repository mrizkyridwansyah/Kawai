SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PO_Contract_Detail](
	[Contract_No] [varchar](20) NOT NULL,
	[Seq_No] [int] NOT NULL,
	[PO_No] [varchar](20) NULL,
	[Parent_ItemCode] [varchar](30) NULL,
	[Item_Code] [varchar](30) NULL,
	[HS_Code] [varchar](20) NULL,
	[Item_Name] [varchar](100) NULL,
	[Receipt_Date] [date] NULL,
	[Currency_Code] [varchar](10) NULL,
	[Rate] [decimal](18, 5) NULL,
	[QtyBOM] [decimal](18, 5) NULL,
	[QtyPO] [decimal](18, 5) NULL,
	[QtySet] [decimal](18, 5) NULL,
	[Unit] [varchar](10) NULL,
	[Price] [decimal](18, 5) NULL,
	[Amount] [decimal](18, 5) NULL,
	[BC_Type] [varchar](10) NULL,
	[BC40_No] [varchar](30) NULL,
	[BC40_Date] [date] NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [varchar](50) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_PO_Contract_Detail] PRIMARY KEY CLUSTERED 
(
	[Contract_No] ASC,
	[Seq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [PO_Contract_Detail]  WITH CHECK ADD  CONSTRAINT [FK_PO_Contract_Detail_Master] FOREIGN KEY([Contract_No])
REFERENCES [PO_Contract_Master] ([Contract_No])
GO
ALTER TABLE [PO_Contract_Detail] CHECK CONSTRAINT [FK_PO_Contract_Detail_Master]
GO
