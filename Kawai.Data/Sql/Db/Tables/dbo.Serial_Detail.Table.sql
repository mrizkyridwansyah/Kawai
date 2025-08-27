SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Serial_Detail](
	[Item_Code] [char](25) NOT NULL,
	[Serial_No] [char](10) NOT NULL,
	[PO_No] [char](35) NULL,
	[PO_SeqNo] [numeric](18, 0) NULL,
	[Product_No] [char](35) NULL,
	[Result_No] [char](35) NULL,
	[DO_No] [char](35) NULL,
	[DO_SeqNo] [numeric](18, 0) NULL,
	[Packing_No] [char](35) NULL,
	[Serial_Status] [char](1) NULL,
 CONSTRAINT [PK_Serial_Detail] PRIMARY KEY CLUSTERED 
(
	[Item_Code] ASC,
	[Serial_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
