using Microsoft.EntityFrameworkCore.Migrations;

namespace SourceOfFund.Data.Migrations
{
    public partial class CreateProcedureManageBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
	Create PROCEDURE [dbo].[ManageBalance]
			@SourceID int,
			@RequestID int ,
			@AccountID int , 
			@Amount decimal(18,3),
			@BalanceRequestTypeID int,
			@BalanceTypeID int,
			@TransactionID int
			,@StatusCode INT OUTPUT
			AS
			DECLARE @ISLedgerAccount int, @AccountFromBalanceBefore decimal(18,3), @TotalBalance decimal(18,3)
			DECLARE @RowsEffected int
			DECLARE @Updated table(BeforeBalance decimal(18, 3));
			BEGIN 
			
			IF @BalanceRequestTypeID = 1 --HOLD 
				BEGIN
				BEGIN TRANSACTION MainTrx
				SELECT  @AccountFromBalanceBefore = ASAB.Balance
									FROM [AccountServiceAvailableBalances] ASAB
									INNER JOIN [BalanceTypes] BT ON ASAB.BalanceTypeID = BT.ID
									WHERE
									ASAB.[AccountID] = @AccountID AND (ASAB.Balance-@Amount) >= 0
									AND ASAB.BalanceTypeID = @BalanceTypeID
				if @AccountFromBalanceBefore > 0
				BEGIN
					UPDATE AccountServiceAvailableBalances SET Balance = Balance - @Amount WHERE BalanceTypeID = @BalanceTypeID
					and AccountID = @AccountID
					
					SET @RowsEffected = @@ROWCOUNT

					IF @RowsEffected > 0 
						BEGIN
						INSERT INTO HoldBalances ([AccountID],[Balance],[RequestID],[SourceID], [Status], [AvailableBalanceBefore], [BalanceTypeID], [CreationDate])
						VALUES(@AccountID,@Amount,@RequestID,@SourceID,1, @AccountFromBalanceBefore, @BalanceTypeID, GETDATE());
						SELECT @StatusCode = 200;
						COMMIT TRAN MainTrx
						RETURN;
						END
					ELSE
					BEGIN
						SELECT @StatusCode = 0;
						COMMIT TRAN MainTrx
						RETURN;
					END
				END
					
				ELSE
				BEGIN
					SELECT @StatusCode = 0;
					COMMIT TRAN MainTrx
					RETURN;
				END
			END 


			ELSE IF @BalanceRequestTypeID = 2 --CONFIRM 
				BEGIN

				UPDATE HoldBalances  SET Status = 0 WHERE AccountID = @AccountID AND RequestID = @RequestID AND SourceID = @SourceID AND Status = 1;
		
				IF @@ROWCOUNT > 0 
					BEGIN

					BEGIN TRAN ConfirmTrx
					UPDATE AccountServiceBalances 
					SET Balance = (Balance - (SELECT HB.Balance FROM HoldBalances HB WHERE HB.AccountID = @AccountID AND HB.RequestID = @RequestID AND HB.SourceID = @SourceID ))
					OUTPUT deleted.Balance as BeforeBalance
					INTO @Updated
					WHERE [AccountID] = @AccountID 
					AND BalanceTypeID = @BalanceTypeID

					

					INSERT INTO [dbo].[BalanceHistories] ([CreationDate], [TransactionID], [BalanceBefore], [AccountID], [BalanceTypeID], [TotalBalance])
					VALUES
					(GETDATE(), @TransactionID, (SELECT BeforeBalance FROM @Updated), @AccountID, @BalanceTypeID, 
					(SELECT SUM(Balance) FROM AccountServiceBalances WHERE [AccountID] = @AccountID))
					COMMIT TRAN ConfirmTrx

					SELECT @StatusCode = 200;
					RETURN;
					END
				ELSE
					BEGIN
						SELECT @StatusCode = -1;
						RETURN;
					END  
			END
			ELSE IF @BalanceRequestTypeID = 3 --UNHOLD 
				BEGIN

				UPDATE HoldBalances  SET Status = 0 WHERE AccountID = @AccountID AND RequestID = @RequestID AND SourceID = @SourceID AND Status = 1;
		
				IF @@ROWCOUNT > 0 
					BEGIN
					UPDATE AccountServiceAvailableBalances 
					SET Balance = (Balance + (SELECT HB.Balance FROM HoldBalances HB WHERE HB.AccountID = @AccountID AND HB.RequestID = @RequestID AND HB.SourceID = @SourceID ))
					WHERE [AccountID] = @AccountID 
					AND BalanceTypeID = @BalanceTypeID
						
					SELECT @StatusCode = 200;
					RETURN;
					END
				ELSE
					BEGIN
						SELECT @StatusCode = -1;
						RETURN;
					END  
			END
			SELECT @StatusCode = -1;
			RETURN;	
		END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
