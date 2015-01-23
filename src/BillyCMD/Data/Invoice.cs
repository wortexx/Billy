using System;

namespace BillyCMD.Data
{
    public class Invoice
    {
        public string Number { get; set; }

        public DateTime Date { get; set; }
        public DateTime BankTransactionDate { get; set; }

        public Period Period { get; set; }
        public decimal Total { get; set; }
    }
}