SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ShippingInstruction_Detail](
	[SI_No] [nvarchar](50) NOT NULL,
	[PO_NO] [nvarchar](35) NOT NULL,
	[PO_SeqNo] [int] NOT NULL,
	[Item_Code] [nvarchar](35) NOT NULL,
	[Serial_No] [nvarchar](50) NOT NULL,
	[Address] [nvarchar](50) NULL,
	[IsPicking] [char](1) NULL,
	[Picking_Date] [datetime] NULL,
	[Picking_By] [nvarchar](15) NULL,
	[Register_Date] [datetime] NULL,
	[Register_By] [nvarchar](15) NULL,
	[Update_Date] [datetime] NULL,
	[Update_By] [nvarchar](15) NULL,
 CONSTRAINT [PK_ShippingInstruction_Detail] PRIMARY KEY CLUSTERED 
(
	[SI_No] ASC,
	[PO_NO] ASC,
	[PO_SeqNo] ASC,
	[Item_Code] ASC,
	[Serial_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


