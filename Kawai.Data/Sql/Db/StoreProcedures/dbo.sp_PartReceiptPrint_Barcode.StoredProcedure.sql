CREATE PROCEDURE [dbo].[sp_PartReceiptPrint_Barcode]
--DECLARE	
	@SJNo			VARCHAR(25) = 'HMJ005/IV/26-KAWAI',
	@FactoryCode	VARCHAR(6)	= '00000' ,
	@UserID			VARCHAR(20) = 'Admin'
	
AS
BEGIN
	SET NOCOUNT ON;

	DELETE  A
    FROM    dbo.PartReceiptDetailBarcodeTemp A
            LEFT JOIN dbo.Part_Receipt B ON A.ReciptSeqNo = B.Seq_No
    WHERE   B.SuratJalan_No = @SJNo;

	IF NOT EXISTS (
		SELECT *
		FROM dbo.PartReceiptDetailBarcodeTemp A
		LEFT JOIN dbo.Part_Receipt B 
			ON A.ReciptSeqNo = B.Seq_No
		WHERE B.SuratJalan_No = @SJNo
	)
	BEGIN
			INSERT  INTO dbo.PartReceiptDetailBarcodeTemp
					( ReceiptId ,
					  ReceiptDetailId ,
					  ReciptSeqNo ,
					  ReceiptDate ,
					  PONumber ,
					  ItemCode ,
					  BarcodeNo ,
					  LotNo ,
					  SublotNo ,
					  Qty ,
					  IsVerified ,
					  VerifiedBy ,
					  VerifiedDate ,
					  PrintStatus ,
					  PrintDate ,
					  PrintUser ,
					  WarehouseCode ,
					  ShippingLabelNo
					)
					SELECT  Seq_No ,
							Seq_No ,
							Seq_No ,
							PR.Receipt_Date ,
							PO_No ,
							PR.Item_Code ,
							BarcodeNo = 'RM' + CONVERT(VARCHAR(8), PR.Receipt_Date, 112) , 
							LotNo = 'L.RM' + CONVERT(VARCHAR(8), PR.Receipt_Date, 112) ,
							SubLotNo = NULL ,
							 Qty = PR.Qty ,
							IsVerified = NULL ,
							VerifiedBy = NULL ,
							VerifiedDate = NULL ,
							PrintStatus = NULL ,
							PrintDate = NULL ,
							PrintUser = NULL ,
							Warehouse_Code ,
							ShippingLabel = NULL 							
					FROM    dbo.Part_Receipt PR
							LEFT JOIN dbo.Item_Master IM ON IM.Item_Code = PR.Item_Code
							LEFT JOIN dbo.Trade_Master TM ON PR.Supplier_Code = TM.Trade_Code
							OUTER APPLY ( SELECT    *
										  FROM      dbo.Company_Profile
										  WHERE     Company_Code = @FactoryCode
										) CP
	END

	UPDATE  A
	SET     A.PrintStatus = '01' ,
			PrintDate = GETDATE() ,
			PrintUser = @UserID
	FROM    PartReceiptDetailBarcodeTemp A
			LEFT JOIN dbo.Part_Receipt B ON A.ReceiptDetailId = B.Seq_No
	WHERE   B.SuratJalan_No = @SJNo
	
   ;
WITH    N AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY ( SELECT NULL
                                                     ) ) AS No
               FROM     master..spt_values
             )
    SELECT  BarcodeNo = BarcodeNo + '' + RIGHT('000' + CAST(N.No AS VARCHAR),
                                               3) ,
            Trade_Name ,
            Company_Name ,
            PONumber ,
            ShippingLotNo ,
            ReceiptDate ,
            ItemCode = Item_Code ,
            Item_Name ,
            Qty = CAST(CASE WHEN N.No < BoxCalc.Box_No THEN IM.Number_Box
                            ELSE Qty - ( IM.Number_Box * ( BoxCalc.Box_No - 1 ) )
                       END AS VARCHAR(20)) + ' ' + ISNULL(A.Unit_Cls, '') ,
            A.Unit_Cls ,
            SuratJalan_No ,
            ShippingLabelNo = CAST(N.No AS VARCHAR) + '/'
            + CAST(BoxCalc.Box_No AS VARCHAR) ,
            No_Seri
    FROM    ( SELECT    BarcodeNo = A.BarcodeNo ,
                        TM.Trade_Name ,
                        CP.Company_Name ,
                        A.PONumber ,
                        ShippingLotNo = MONTH(PR.Receipt_Date) ,
                        A.ReceiptDate ,
                        ItemCode = A.ItemCode ,
                        Qty = SUM(A.Qty) ,
                        Unit_Cls = UPPER(RTRIM(UC.Description)) ,
                        PR.SuratJalan_No ,
                        ShippingLabelNo = '' ,
                        No_Seri = ROW_NUMBER() OVER ( PARTITION BY CAST(PR.Receipt_Date AS DATE) ORDER BY ( A.ItemCode ) )
              FROM      PartReceiptDetailBarcodeTemp A
                        LEFT JOIN dbo.Part_Receipt PR ON A.ReceiptDetailId = PR.Seq_No
                        LEFT JOIN dbo.Trade_Master TM ON PR.Supplier_Code = TM.Trade_Code
                        LEFT JOIN dbo.Unit_Cls UC ON PR.Unit_C