SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [POSub](
	[TanggalTOS] [datetime] NULL,
	[TanggalINT] [datetime] NULL,
	[Doku] [nvarchar](50) NULL,
	[Tanggal] [smalldatetime] NULL,
	[Kode_Dept] [nvarchar](20) NULL,
	[Kode] [nvarchar](20) NULL,
	[Alias] [nvarchar](30) NULL,
	[Jumlah] [float] NULL,
	[Kode_Satuan] [nvarchar](20) NULL,
	[Harga] [float] NULL,
	[HargaJasa] [float] NULL,
	[HargaMaterial] [float] NULL,
	[Diskon] [float] NULL,
	[Nilai] [float] NULL,
	[bPPN] [bit] NULL,
	[PPN] [float] NULL,
	[PPH22] [float] NULL,
	[TglKirim] [datetime] NULL,
	[JumlahKirim] [float] NULL,
	[NoUrut] [float] NULL,
	[EntryDate] [nvarchar](25) NULL,
	[UserID] [nvarchar](25) NULL,
	[Hapus] [nvarchar](25) NULL,
	[Nama] [nvarchar](255) NULL,
	[NamaSatuan] [nvarchar](50) NULL,
	[Status] [char](1) NULL,
	[DataReceivedCls] [char](1) NULL,
	[DataReceivedDate] [datetime] NULL,
	[PO_No] [nvarchar](50) NULL,
	[Item_Code] [char](15) NULL,
	[Last_Update] [datetime] NULL
) ON [PRIMARY]
GO
