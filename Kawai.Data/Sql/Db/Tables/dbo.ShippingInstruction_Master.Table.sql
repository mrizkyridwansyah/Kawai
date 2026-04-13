SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ShippingInstruction_Master](
	[Cust_Code] [nvarchar](15) NOT NULL,
	[SI_NO] [nvarchar](50) NOT NULL,
	[SI_Date] [datetime] NULL,
	[PO_NO] [nvarchar](35) NOT NULL,
	[PO_SeqNo] [int] NOT NULL,
	[Item_Code] [nvarchar](35) NOT NULL,
	[Item_Name] [nvarchar](max) NULL,
	[Unit_Cls] [nvarchar](5) NULL,
	[Unit_Desc] [nvarchar](50) NULL,
	[Qty] [numeric](18, 5) NULL,
	[PO_DelivDate] [datetime] NULL,
	[SerialNo_From] [nvarchar](50) NULL,
	[SerialNo_To] [nvarchar](50) NULL,
	[Regsister_Date] [datetime] NULL,
	[Register_By] [nvarchar](15) NULL,
	[Update_Date] [datetime] NULL,
	[Update_By] [nvarchar](15) NULL,
 CONSTRAINT [PK_ShippingInstruction_Master] PRIMARY KEY CLUSTERED 
(
	[Cust_Code] ASC,
	[PO_NO] ASC,
	[PO_SeqNo] ASC,
	[SI_NO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


