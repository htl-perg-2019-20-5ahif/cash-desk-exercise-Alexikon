using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CashDesk.Model
{
    public class Member : IMember
    {
        public int MemberNumber { get; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public List<Membership> Memberships { get; set; }
    }
}
