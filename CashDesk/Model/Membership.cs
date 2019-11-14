using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CashDesk.Model
{
    public class Membership : IMembership
    {
        public int MembershipId { get; }

        public Member Member { get; set; }

        public DateTime Begin { get; set; }

        private DateTime _end;
        public DateTime End
        {
            get { return _end; }
            set
            {
                if (Begin > value)
                {
                    throw new ArgumentException("End must be greater than Begin");
                }

                _end = value;
            }
        }

        public List<Deposit> Deposits { get; set; }

        [NotMapped]
        IMember IMembership.Member => Member;
    }
}
