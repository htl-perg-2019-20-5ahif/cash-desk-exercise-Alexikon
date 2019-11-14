using CashDesk.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashDesk
{
    /// <inheritdoc />
    public class DataAccess : IDataAccess
    {
        private static CashDeskContext _context = null;

        /// <inheritdoc />
        public Task InitializeDatabaseAsync()
        {
            if (_context != null)
            {
                throw new InvalidOperationException("Already initialised");
            }

            _context = new CashDeskContext();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<int> AddMemberAsync(string firstName, string lastName, DateTime birthday)
        {
            ThrowIfNotInitialised();
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("FirstName is invalid");
            }
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("LastName is invalid");
            }
            if (birthday == null)
            {
                throw new ArgumentException("Birthday is invalid");
            }

            if (_context == null)
            {
                throw new InvalidOperationException();
            }

            if (_context.Members.Where(m => m.LastName.Equals(lastName)).Any())
            {
                throw new DuplicateNameException();
            }

            var member = new Member
            {
                FirstName = firstName,
                LastName = lastName,
                Birthday = birthday
            };
            _context.Members.Add(member);

            await _context.SaveChangesAsync();

            return member.MemberNumber;
        }

        /// <inheritdoc />
        public async Task DeleteMemberAsync(int memberNumber)
        {
            ThrowIfNotInitialised();

            Member member = await _context.Members.FirstOrDefaultAsync(m => m.MemberNumber == memberNumber);

            if(member == null)
            {
                throw new ArgumentException();
            }

            _context.Members.Remove(member);

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IMembership> JoinMemberAsync(int memberNumber)
        {
            if (await _context.Memberships.AnyAsync(m => m.Member.MemberNumber == memberNumber && m.Begin != null && m.End != null && DateTime.Now >= m.Begin && DateTime.Now <= m.End))
            {
                throw new AlreadyMemberException();
            }

            var newMembership = new Membership
            {
                Member = await _context.Members.FirstAsync(m => m.MemberNumber == memberNumber),
                Begin = DateTime.Now
            };
            _context.Memberships.Add(newMembership);

            await _context.SaveChangesAsync();

            return newMembership;
        }

        /// <inheritdoc />
        public async Task<IMembership> CancelMembershipAsync(int memberNumber)
        {
            ThrowIfNotInitialised();

            Membership membership = await _context.Memberships.FirstOrDefaultAsync(m => m.Member.MemberNumber == memberNumber && m.End == null);

            if(membership == null)
            {
                throw new NoMemberException();
            }

            membership.End = DateTime.Now;

            await _context.SaveChangesAsync();

            return membership;
        }

        /// <inheritdoc />
        public async Task DepositAsync(int memberNumber, decimal amount)
        {
            ThrowIfNotInitialised();

            Member member = await _context.Members.FirstOrDefaultAsync(m => m.MemberNumber == memberNumber);
            
            if(member == null)
            {
                throw new ArgumentException();
            }

            Membership membership = await _context.Memberships.FirstOrDefaultAsync(m => m.Member.MemberNumber == memberNumber && m.Begin != null && DateTime.Now >= m.Begin && m.End == null);

            if (membership == null)
            {
                throw new NoMemberException();
            }

            var newDeposit = new Deposit
            {
                Membership = membership,
                Amount = amount
            };

            _context.Deposits.Add(newDeposit);

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IDepositStatistics>> GetDepositStatisticsAsync()
        {
            ThrowIfNotInitialised();

            return (await _context.Deposits.Include("Membership.Member").ToArrayAsync())
                .GroupBy(d => new { d.Membership.Begin.Year, d.Membership.Member })
                .Select(i => new DepositStatistic
                {
                    Year = i.Key.Year,
                    Member = i.Key.Member,
                    TotalAmount = i.Sum(d => d.Amount)
                });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        public void ThrowIfNotInitialised()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Not initialized");
            }
        }
    }
}