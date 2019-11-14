using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CashDesk.Model
{
    public class Deposit : IDeposit
    {
        public int DepositId { get; }

        public Membership Membership { get; set; }

        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("End must be greater than Begin");
                }

                _amount = value;
            }
        }

        [NotMapped]
        IMembership IDeposit.Membership => Membership;
    }
}
