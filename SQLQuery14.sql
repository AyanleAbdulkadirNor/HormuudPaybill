CREATE TABLE [dbo].[Hormuud_Test](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[tansactionId] [varchar](50) NULL,
	[totalAmount] [money] NULL,
	[currency] [varchar](50) NULL,
	[studentId] [varchar](50) NULL,
	[studentName] [varchar](50) NULL,
	[paidAt] [varchar](50) NULL,
	[accountNumber] [varchar](50) NULL,
	[amount] [money] NULL,
	[UserID] [varchar](50) NULL,
	[ActionDate] [smalldatetime] NULL
	)
	
	CREATE TABLE [dbo].[Hormuud_Test](
	[accountNumber] [varchar](50) NULL,
	[accountTitle] [varchar](50) NULL,
	[Amount] [money] NULL,
	[description] [varchar](50) NULL,
	[dueDate] [smalldatetime] NULL,
	[isPartialPayAllowed] [int] NULL
	)
	
	======================== Hormuud_Test_Paybill Procuders ========================================
	CREATE PROCEDURE [dbo].[Hormuud_Test_Paybill]
	-- Add the parameters for the stored procedure here
	@studentId VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @TotalAmount Money
	SET @TotalAmount = (SELECT SUM(Amount)totalAmount  FROM GetProducts_Paybill)
    -- Insert statements for procedure here
	SELECT studentId,studentName,@TotalAmount totalAmount,currency currencyCode
	 FROM Hormuud_Test WHERE studentId = @studentId
	 GROUP BY studentId,studentName,currency
END

=================== Get Product Procuders =============================
CREATE PROCEDURE [dbo].[GET_Products]
	-- Add the parameters for the stored procedure here
 AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT accountNumber,accountTitle,Amount,description,dueDate,isPartialPayAllowed FROM GetProducts_Paybill  
END

======================== Pay_Bill_Notification =================================

ALTER PROCEDURE [dbo].[Pay_Bill_Notification]
	-- Add the parameters for the stored procedure here
	@tansactionId VARCHAR(50),
	@totalAmount MONEY,
	@currency VARCHAR(50),
	@studentId VARCHAR(50),
	@studentName VARCHAR(50),
	@paidAt VARCHAR(50),
	@accountNumber VARCHAR(50),
	@amount VARCHAR(50)
	--tansactionId,totalAmount,currency,studentId,studentName,paidAt,amount,UserID

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	BEGIN
		--INSERT INTO Ayaanle_Test(tansactionId,amount,currency,billReference,paidBy,paidAt,billId,billAmount,UserID)
			INSERT INTO Hormuud_Test(tansactionId,totalAmount,currency,studentId,studentName,paidAt,accountNumber,amount,UserID,ActionDate)
		SELECT @tansactionId,@totalAmount,@currency,@studentId,@studentName,@paidAt,@accountNumber,@amount,'SYS',GETDATE()
		SELECT @studentId + REPLACE(CAST(CAST(GETDATE() AS DATE )AS VARCHAR),'-','')confirmationId 
	END
	END