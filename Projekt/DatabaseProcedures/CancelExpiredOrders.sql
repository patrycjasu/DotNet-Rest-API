create procedure CancelExpiredOrders as
begin
    update orders set status = 2 
    where(status = 0 and isdeleted = 0 and orderdate < dateadd(day, -7, getdate());
end