SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MS_Area](
	[WarehouseCode] [varchar](25) NOT NULL,
	[AreaCode] [varchar](25) NOT NULL,
	[AreaName] [varchar](200) NULL,
	[PictureName] [varchar](max) NULL,
	[PictureRealName] [varchar](max) NULL,
	[QRCode] [image] NULL,
	[RegisterBy] [varchar](50) NULL,
	[RegisterDate] [datetime] NULL,
	[UpdateBy] [varchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[ItemType] [varchar](25) NULL,
	[PickingSequence] [int] NULL
 CONSTRAINT [PK_MS_Location] PRIMARY KEY CLUSTERED 
(
	[WarehouseCode] ASC,
	[AreaCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
