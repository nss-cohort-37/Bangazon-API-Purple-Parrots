 SELECT
    upt.Id, upt.CustomerId, upt.PaymentTypeId, upt.AcctNumber, upt.Active,  c.FirstName, c.LastName
    FROM UserPaymentType upt
    LEFT JOIN Customer c ON upt.CustomerId = c.Id   