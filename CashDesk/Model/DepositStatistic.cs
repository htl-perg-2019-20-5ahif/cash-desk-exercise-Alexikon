using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CashDesk.Model
{
    public class DepositStatistic : IDepositStatistics
    {
        public Member Member { get; set; }

        public int Year { get; set; }

        public decimal TotalAmount { get; set; }

        IMember IDepositStatistics.Member => Member;
    }
}
